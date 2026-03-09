using System.Text.Json.Nodes;
using Oicana.Example.Controllers;
using Oicana.Example.Models;

namespace Oicana.Example.Services;

/// <summary>
/// Compile Oicana templates with input.
/// </summary>
public interface ITemplatingService
{
    /// <summary>
    /// Compile an Oicana template with input to a PDF.
    /// </summary>
    /// <param name="templateId">This id has to be registered in the <see cref="Oicana.Template.OicanaService"/></param>
    /// <param name="jsonInput">Json inputs to pass into the template</param>
    /// <param name="storedBlobInputs">Blob inputs to pass into the template</param>
    /// <returns>A stream containing a PDF export of the template or <see langword="null"/> if the template is not registered</returns>
    Task<Stream?> Compile(string templateId, IList<JsonInputDto> jsonInput, IList<BlobInputDto> storedBlobInputs);

    /// <summary>
    /// Preview an Oicana template with input as a PNG image.
    /// </summary>
    /// <param name="templateId">This id has to be registered in the <see cref="Oicana.Template.OicanaService"/></param>
    /// <param name="jsonInput">Json inputs to pass into the template</param>
    /// <param name="storedBlobInputs">Blob inputs to pass into the template</param>
    /// <returns>A stream containing a PNG image of the template or <see langword="null"/> if the template is not registered</returns>
    Task<Stream?> Preview(string templateId, IList<JsonInputDto> jsonInput, IList<BlobInputDto> storedBlobInputs);

    /// <summary>
    /// Remove a registered Oicana template from this service.
    /// </summary>
    /// <param name="templateId">This id has to be registered in the <see cref="Oicana.Template.OicanaService"/></param>
    /// <returns>true if the template was removed, false if the template id was not registered</returns>
    bool RemoveTemplate(string templateId);
}
