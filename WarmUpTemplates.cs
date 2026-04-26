namespace Oicana.Example;

/// <summary>
/// Warm up the cache for all included templates
/// </summary>
public class WarmUpTemplates(IServiceProvider serviceProvider) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WarmUpTemplates>>();
        var service = scope.ServiceProvider.GetRequiredService<IOicanaService>();

        logger.LogInformation("Warming up the template cache. This can take a moment...");
        foreach (var template in TemplateRegistry.Registry)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Aborting template warm up after cancellation request");
                return;
            }

            try
            {
                var file = await File.ReadAllBytesAsync($"templates/{template.Key}-{template.Value}.zip", cancellationToken);
                service.RegisterTemplate(template.Key, file);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to compile '{template}' with it's sample input for warming up the cache", template);
            }
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}