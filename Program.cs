using Oicana;
using Oicana.Example;
using Oicana.Example.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IOicanaService, OicanaService>();
builder.Services.AddScoped<ITemplatingService, TemplatingService>()
    .AddScoped<ICertificateService, CertificateService>()
    .AddScoped<IStoredBlobService, StoredBlobService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
builder.Services.AddHostedService<WarmUpTemplates>();

builder
    .RegisterTemplate("accessibility", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("certificate", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("dependency", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("fonts", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("invoice", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("invoice_zugferd", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("minimal", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("table", TemplateVersion.From(0, 1, 0))
    .RegisterTemplate("multi_input", TemplateVersion.From(0, 1, 0));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Content("Visit the API documentation at <a href=\"/scalar\">/scalar</a>", "text/html")).ExcludeFromDescription();

app.Run();