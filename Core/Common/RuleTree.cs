using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Cli;

// ──────────────────────────────────────────────
//  Parámetro de función
// ──────────────────────────────────────────────

/// <summary>
/// Define un parámetro aceptado por una función de la API.
/// </summary>
public sealed class ParameterDefinition
{
    [JsonPropertyName("IsObligatory")]
    public bool IsObligatory { get; init; }

    /// <summary>
    /// Tipo de dato esperado: "string", "int", "bool", "date".
    /// El parser usará este campo para intentar convertir el valor recibido.
    /// </summary>
    [JsonPropertyName("DataType")]
    public string DataType { get; init; } = "string";

    /// <summary>
    /// Valor por defecto cuando el parámetro es opcional y el usuario no lo provee.
    /// null indica que no hay default y el parámetro simplemente se omite.
    /// </summary>
    [JsonPropertyName("Default")]
    public string? Default { get; init; }

    [JsonPropertyName("Description")]
    public string Description { get; init; } = string.Empty;
}

// ──────────────────────────────────────────────
//  Nodo del árbol de verbos
// ──────────────────────────────────────────────

/// <summary>
/// Nodo del árbol de verbos. Puede ser:
///   - Terminal puro:   FunctionName != null, Children vacío.
///   - Interno puro:    FunctionName == null, Children con entradas.
///   - Mixto (_fn):     FunctionName != null, Children con entradas.
///     Usado cuando una palabra es verbo por sí sola Y prefijo de verbos compuestos.
/// </summary>
public sealed class VerbNode
{
    /// <summary>
    /// Nombre de la función a invocar cuando este nodo es el destino final del comando.
    /// Corresponde a una clave en RuleTree.Functions.
    /// null si el nodo es solo un nodo intermediario.
    /// </summary>
    public string? FunctionName { get; init; }

    /// <summary>
    /// Sub-verbos disponibles desde este nodo.
    /// </summary>
    public IReadOnlyDictionary<string, VerbNode> Children { get; init; }
        = new Dictionary<string, VerbNode>();

    /// <summary>
    /// True si este nodo por sí solo resuelve a una función
    /// (independientemente de si tiene hijos).
    /// </summary>
    public bool IsTerminal => FunctionName is not null;
}

// ──────────────────────────────────────────────
//  RuleTree
// ──────────────────────────────────────────────

/// <summary>
/// Árbol de reglas completo de un Plug.
/// Contiene los nombres/aliases del Plug, el árbol de verbos
/// y las definiciones de parámetros de cada función.
/// </summary>
public sealed class RuleTree
{
    /// <summary>
    /// Nombres y aliases con los que el Plug puede ser referenciado.
    /// Ej: ["Kanban", "KB"]. La comparación se hace case-insensitive.
    /// </summary>
    public IReadOnlyList<string> Names { get; init; } = [];

    /// <summary>
    /// Raíz del árbol de verbos. Las claves son las palabras del primer nivel.
    /// </summary>
    public IReadOnlyDictionary<string, VerbNode> Verbs { get; init; }
        = new Dictionary<string, VerbNode>();

    /// <summary>
    /// Definiciones de parámetros por función.
    /// Clave: nombre de la función (igual al string en los nodos terminales del árbol).
    /// Valor: diccionario de flag → definición (ej: "n" → ParameterDefinition).
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, ParameterDefinition>> Functions { get; init; }
        = new Dictionary<string, IReadOnlyDictionary<string, ParameterDefinition>>();

    // ── Navegación ──────────────────────────────

    /// <summary>
    /// Intenta resolver una secuencia de palabras de verbo a un nombre de función.
    /// Navega el árbol de forma atómica: cada palabra debe existir como hijo del nodo actual.
    /// </summary>
    /// <param name="verbTokens">Palabras del comando tras el nombre del Plug.</param>
    /// <param name="functionName">Nombre de la función si se resuelve con éxito.</param>
    /// <param name="consumedCount">Cuántos tokens del arreglo fueron consumidos como verbos.</param>
    /// <returns>True si se encontró una función válida.</returns>
    public bool TryResolveVerbs(
        IReadOnlyList<string> verbTokens,
        out string? functionName,
        out int consumedCount)
    {
        functionName = null;
        consumedCount = 0;

        var current = Verbs;
        string? lastFunction = null;
        int lastConsumed = 0;

        for (int i = 0; i < verbTokens.Count; i++)
        {
            var token = verbTokens[i].ToLowerInvariant();

            if (!current.TryGetValue(token, out var node))
                break;

            if (node.IsTerminal)
            {
                lastFunction = node.FunctionName;
                lastConsumed = i + 1;
            }

            if (node.Children.Count == 0)
                break;

            current = node.Children;
        }

        if (lastFunction is null)
            return false;

        functionName = lastFunction;
        consumedCount = lastConsumed;
        return true;
    }

    /// <summary>
    /// Devuelve la definición de parámetros de una función dado su nombre.
    /// </summary>
    public bool TryGetFunctionParams(
        string functionName,
        out IReadOnlyDictionary<string, ParameterDefinition>? parameters)
    {
        return Functions.TryGetValue(functionName, out parameters);
    }
}