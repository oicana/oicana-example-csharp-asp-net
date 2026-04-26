using System.Text.Json.Nodes;
using Oicana.Config;
using Oicana.Inputs;

using Oicana.Example.Models;

namespace Oicana.Example.Services;

/// <inheritdoc />
public class TemplatingService(IOicanaService oicanaService, IStoredBlobService storedBlobService, ILogger<TemplatingService> logger) : ITemplatingService
{
    /// <inheritdoc />
    public async Task<Stream?> Compile(string templateId, IList<JsonInputDto> jsonInputList, IList<BlobInputDto> storedBlobInputs)
    {
        var jsonInput = jsonInputList.ToDictionary(i => i.Key, i => i.Value);
        var template = oicanaService.GetTemplate(templateId);
        if (template == null)
        {
            return null;
        }
        var blobInputs = await LoadBlobInputs(storedBlobInputs);

        return await Task.Run<Stream?>(() => template.Compile(jsonInput, blobInputs, ExportFormat.Pdf(), new CompilationOptions(CompilationMode.Production)));
    }

    /// <inheritdoc />
    public async Task<Stream?> Preview(string templateId, IList<JsonInputDto> jsonInputList, IList<BlobInputDto> storedBlobInputs)
    {
        var jsonInput = jsonInputList.ToDictionary(i => i.Key, i => i.Value);
        var template = oicanaService.GetTemplate(templateId);
        if (template == null)
        {
            return null;
        }
        var blobInputs = await LoadBlobInputs(storedBlobInputs);

        return await Task.Run<Stream?>(() => template.Compile(jsonInput, blobInputs, ExportFormat.Png(1.0f), new CompilationOptions(CompilationMode.Production)));
    }

    /// <inheritdoc />
    public bool RemoveTemplate(string templateId)
    {
        var template = oicanaService.RemoveTemplate(templateId);
        return template != null;
    }

    private async Task<IDictionary<string, BlobInput>> LoadBlobInputs(IList<BlobInputDto> storedBlobInputs)
    {
        var blobInputs = new Dictionary<string, BlobInput>();
        foreach (var dto in storedBlobInputs)
        {
            var blob = await storedBlobService.RetrieveBlob(dto.BlobId);
            if (blob == null)
            {
                logger.LogWarning("The stored blob {BlobId} could not be loaded.", dto.BlobId);
                continue;
            }
            blobInputs[dto.Key] = new BlobInput(blob);
        }

        return blobInputs;
    }
}
