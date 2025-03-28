using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using WebApiEntraId;

var builder = WebApplication.CreateBuilder(args);

// Open up security restrictions to allow this to work
// Not recommended in production
var deploySwaggerUI = builder.Configuration.GetValue<bool>("DeploySwaggerUI");
var isDev = builder.Environment.IsDevelopment();

builder.Services.AddSecurityHeaderPolicies()
    .SetPolicySelector((PolicySelectorContext ctx) =>
    {
        // sum is weak security headers due to Swagger UI deployment
        // should only use in development
        if (deploySwaggerUI)
        {
            // Weakened security headers for Swagger UI
            if (ctx.HttpContext.Request.Path.StartsWithSegments("/swagger"))
            {
                return SecurityHeadersDefinitionsSwagger.GetHeaderPolicyCollection(isDev);
            }

            // Strict security headers
            return SecurityHeadersDefinitionsAPI.GetHeaderPolicyCollection(isDev);
        }
        // Strict security headers for production
        else
        {
            return SecurityHeadersDefinitionsAPI.GetHeaderPolicyCollection(isDev);
        }
    });

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpClient();
builder.Services.AddOptions();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(
    builder.Configuration, "EntraID");

builder.Services.AddOpenApi(options =>
{
    //options.UseTransformer((document, context, cancellationToken) =>
    //{
    //    document.Info = new()
    //    {
    //        Title = "My API",
    //        Version = "v1",
    //        Description = "API for Damien"
    //    };
    //    return Task.CompletedTask;
    //});
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

app.UseSecurityHeaders();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

//app.MapOpenApi(); // /openapi/v1.json
app.MapOpenApi("/openapi/v1/openapi.json");
//app.MapOpenApi("/openapi/{documentName}/openapi.json");

if (deploySwaggerUI)
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
    });
}

app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
        document.Info = new()
        {
            Title = "Microsoft Entra ID delegated API Bearer scheme",
            Version = "v1",
            Description = "Microsoft Entra ID delegated API"
        };
    }
}