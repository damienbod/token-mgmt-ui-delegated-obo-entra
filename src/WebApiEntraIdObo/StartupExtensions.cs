using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;
using Serilog;
using WebApiEntraIdObo.WebApiEntraId;

namespace WebApiEntraIdObo;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        builder.Services.AddTransient<WebApiDownstreamService>();
        builder.Services.AddHttpClient();
        builder.Services.AddOptions();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "EntraID")
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddDistributedTokenCaches();

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

        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSerilogRequestLogging();

        if (app.Environment!.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        //app.MapOpenApi(); // /openapi/v1.json
        app.MapOpenApi("/openapi/v1/openapi.json");
        //app.MapOpenApi("/openapi/{documentName}/openapi.json");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
            });
        }
        return app;
    }
}