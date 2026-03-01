using CinemaX.API.Extensions;
using CinemaX.API.Middleware;
using CinemaX.API.Validators;
using CinemaX.Infrastructure;
using FluentValidation;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token"
            }
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("My API")
        .WithTheme(ScalarTheme.Purple)
        .WithHttpBearerAuthentication(bearer =>
        {
            bearer.Token = "your-default-token";
        });
});

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CinemaX API v1");
    });
}*/

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.MigrateAndSeedAsync();

app.Run();
