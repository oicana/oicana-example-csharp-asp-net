using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Oicana.Config;
using Oicana.Example.Models;
using Oicana.Inputs;

namespace Oicana.Example.Services;

/// <inheritdoc />
public class CertificateService(IOicanaService oicanaService, ILogger<CertificateService> logger) : ICertificateService
{
    private static readonly string Template = "certificate";

    /// <inheritdoc />
    public async Task<Stream?> CreateCertificate(CreateCertificate request)
    {
        var template = oicanaService.GetTemplate(Template);
        if (template == null)
        {
            return null;
        }

        // You could load any additional data here that your template needs
        // For example, you could have a database with grades to look up and pass into your template
        try
        {
            var watch = new Stopwatch();
            watch.Start();

            // "certificate" is the key defined in the template
            // See https://github.com/oicana/oicana-example-templates/blob/672967c5b667dfa845228cac443d32b8b3c7ae0a/templates/certificate/typst.toml#L12
            // In a production system you most likely want separate types as API Model and for the input representation
            var jsonInputs = new Dictionary<string, JsonNode>
            {
                ["certificate"] = JsonSerializer.SerializeToNode(request, new JsonSerializerOptions(JsonSerializerDefaults.Web))!
            };
            var result = await Task.Run<Stream?>(() => template.Compile(jsonInputs, new Dictionary<string, BlobInput>(), ExportFormat.Pdf(), new CompilationOptions(CompilationMode.Production)));
            watch.Stop();

            logger.LogInformation("Certificate generated in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error while compiling a certificate: {e}", e);
            return null;
        }
    }
}
