using System.Text.Json.Nodes;

namespace Oicana.Example.Models;

/// <summary>
/// Request to compile a template with given input
/// </summary>
/// <example>
/// {
///    "jsonInputs": {
///         "input": {
///            "description": "from sample data",
///            "rows": [
///                {
///                    "name": "Frank",
///                    "one": "first",
///                    "two": "second",
///                    "three": "third"
///                }
///            ]
///         }
///     },
///     "blobInputs": {
///         "logo": "00000000-0000-0000-0000-000000000000"
///     }
/// }
/// </example>
public class CompilePdfRequest
{
    /// <summary>
    /// Input json to compile the template with (key -> JsonNode)
    /// </summary>
    /// <example>
    /// {
    ///     "data": {
    ///        "description": "from sample data",
    ///        "rows": [
    ///            {
    ///                "name": "Frank",
    ///                "one": "first",
    ///                "two": "second",
    ///                "three": "third"
    ///            }
    ///        ]
    ///     }
    /// }
    /// </example>
    public required IDictionary<string, JsonNode> JsonInputs { get; init; }

    /// <summary>
    /// Blob inputs referencing stored blobs (key -> blob ID)
    /// </summary>
    /// <example>
    /// {
    ///     "logo": "00000000-0000-0000-0000-000000000000"
    /// }
    /// </example>
    public required IDictionary<string, Guid> BlobInputs { get; init; }
}
