using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Oicana.Example.Services;
using System.Text.RegularExpressions;
using Oicana.Example.Models;

namespace Oicana.Example.Controllers;

/// <summary>
/// Manage and compile PDF templates
/// </summary>
/// <param name="logger"></param>
/// <param name="templatingService"></param>
[ApiController]
[Route("templates")]
public class PdfTemplatingController(ILogger<PdfTemplatingController> logger, ITemplatingService templatingService) : ControllerBase
{
    /// <summary>
    /// Compile any example template to PDF with given inputs
    /// </summary>
    /// <param name="template" example="table"></param>
    /// <param name="request"></param>
    /// <returns>The compiled PDF document</returns>
    [HttpPost("{template}/compile")]
    public async Task<IActionResult> CompilePdfTemplate([FromRoute] String template, [FromBody] CompilePdfRequest request)
    {
        try
        {
            var watch = new Stopwatch();
            watch.Start();
            var stream = await templatingService.Compile(template, request.JsonInputs, request.BlobInputs);
            if (stream == null)
            {
                return StatusCode(404);
            }
            watch.Stop();
            var file = new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"{template}_{DateTimeOffset.Now:yyyy_MM_dd_HH_mm_ss_ffff}.pdf"
            };

            logger.LogInformation("PDF generated in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            return file;
        }
        catch (Exception e)
        {
            string unescapedMessage = Regex.Unescape(e.Message);
            logger.LogError(unescapedMessage);
            return Problem(detail: unescapedMessage, statusCode: 400);
        }
    }

    /// <summary>
    /// Compile any example template to PNG with given inputs
    /// </summary>
    /// <param name="template" example="table"></param>
    /// <param name="request"></param>
    /// <returns>The compiled document as a png file</returns>
    [HttpPost("{template}/preview")]
    public async Task<IActionResult> PreviewPngTemplate([FromRoute] String template, [FromBody] CompilePdfRequest request)
    {
        try
        {
            var watch = new Stopwatch();
            watch.Start();
            var stream = await templatingService.Preview(template, request.JsonInputs, request.BlobInputs);
            if (stream == null)
            {
                return StatusCode(404);
            }
            watch.Stop();
            var file = new FileStreamResult(stream, "image/png")
            {
                FileDownloadName = $"{template}_{DateTimeOffset.Now:yyyy_MM_dd_HH_mm_ss_ffff}.png"
            };

            logger.LogInformation("PNG generated in {ElapsedMilliseconds}ms", watch.ElapsedMilliseconds);
            return file;
        }
        catch (Exception e)
        {
            string unescapedMessage = Regex.Unescape(e.Message);
            logger.LogError(unescapedMessage);
            return Problem(detail: unescapedMessage, statusCode: 400);
        }
    }

    /// <summary>
    /// Reset the cache for the given template
    /// </summary>
    /// <param name="template" example="table"></param>
    [HttpPost("{template}/reset")]
    public IActionResult ResetTemplate([FromRoute] String template)
    {
        var success = templatingService.RemoveTemplate(template);
        return success ? StatusCode(204) : StatusCode(404);
    }

    /// <summary>
    /// Get a list of all template IDs known to the service
    /// </summary>
    /// <returns>Array of template identifiers</returns>
    [HttpGet]
    public IActionResult GetTemplates()
    {
        var templateIds = TemplateRegistry.Registry.Keys.ToList();
        return Ok(templateIds);
    }

    /// <summary>
    /// Download a packed template file
    /// </summary>
    /// <param name="template" example="table">The template identifier</param>
    /// <returns>The packed template as a .zip file</returns>
    [HttpGet("{template}")]
    public IActionResult DownloadTemplate([FromRoute] String template)
    {
        if (!TemplateRegistry.Registry.TryGetValue(template, out var version))
        {
            return NotFound($"Template '{template}' not found");
        }

        var filePath = $"templates/{template}-{version}.zip";
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"Template file not found: {filePath}");
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/zip", $"{template}.zip");
    }
}