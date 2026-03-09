using System.Text.Json.Nodes;

namespace Oicana.Example.Models;

/// <summary>
/// Request to compile a template with given input
/// </summary>
/// <example>
/// {
///    "jsonInputs": [
///         {
///             "key": "input",
///             "value": {
///                "description": "from sample data",
///                "rows": [
///                    {
///                        "name": "Frank",
///                        "one": "first",
///                        "two": "second",
///                        "three": "third"
///                    }
///                ]
///             }
///         }
///     ],
///     "blobInputs": [
///         {
///             "key": "logo",
///             "blobId": "00000000-0000-0000-0000-000000000000"
///         }
///     ]
/// }
/// </example>
public class CompilePdfRequest
{
    /// <summary>
    /// Input json to compile the template with
    /// </summary>
    public required IList<JsonInputDto> JsonInputs { get; init; }

    /// <summary>
    /// Blob inputs referencing stored blobs
    /// </summary>
    public required IList<BlobInputDto> BlobInputs { get; init; }
}

/// <summary>
/// A named JSON input for template compilation
/// </summary>
public class JsonInputDto
{
    /// <summary>
    /// The key of the json input
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The JSON value for this input
    /// </summary>
    public required JsonNode Value { get; init; }
}

/// <summary>
/// A named blob input referencing a stored blob
/// </summary>
public class BlobInputDto
{
    /// <summary>
    /// The key of the blob input
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Identifier of the blob file
    /// </summary>
    public required Guid BlobId { get; init; }
}
