using System.Text.Json.Nodes;
using Oicana.Config;
using Oicana.Inputs;
using Oicana.Template;

namespace Oicana.Example.Services;

/// <inheritdoc />
public class TemplatingService(IOicanaService oicanaService, IStoredBlobService storedBlobService, ILogger<TemplatingService> logger) : ITemplatingService
{
    /// <inheritdoc />
    public async Task<Stream?> Compile(string templateId, IDictionary<string, JsonNode> jsonInput, IDictionary<string, Guid> storedBlobInputs)
    {
        var template = oicanaService.GetTemplate(templateId);
        if (template == null)
        {
            return null;
        }
        var blobInputs = await LoadBlobInputs(storedBlobInputs);

        return await Task.Run<Stream?>(() => template.Compile(jsonInput, blobInputs, ExportOptions.Pdf(), new CompilationOptions(CompilationMode.Production)));
    }

    /// <inheritdoc />
    public async Task<Stream?> Preview(string templateId, IDictionary<string, JsonNode> jsonInput, IDictionary<string, Guid> storedBlobInputs)
    {
        var template = oicanaService.GetTemplate(templateId);
        if (template == null)
        {
            return null;
        }
        var blobInputs = await LoadBlobInputs(storedBlobInputs);

        return await Task.Run<Stream?>(() => template.Compile(jsonInput, blobInputs, ExportOptions.Png(1.0f), new CompilationOptions(CompilationMode.Production)));
    }

    /// <inheritdoc />
    public bool RemoveTemplate(string templateId)
    {
        var template = oicanaService.RemoveTemplate(templateId);
        return template != null;
    }

    private async Task<IDictionary<string, BlobInput>> LoadBlobInputs(IDictionary<string, Guid> storedBlobInputs)
    {
        var blobInputs = new Dictionary<string, BlobInput>();
        foreach (var (key, blobId) in storedBlobInputs)
        {
            var blob = await storedBlobService.RetrieveBlob(blobId);
            if (blob == null)
            {
                logger.LogWarning("The stored blob {BlobId} could not be loaded.", blobId);
                continue;
            }
            blobInputs[key] = new BlobInput(blob);
        }

        return blobInputs;
    }
}
