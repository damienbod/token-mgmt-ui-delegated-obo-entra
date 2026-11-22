var builder = DistributedApplication.CreateBuilder(args);

var webApiEntraId = builder.AddDotnetProject("web-api-entraid", "../WebApiEntraId/WebApiEntraId.csproj")
    .WithExternalHttpEndpoints();

var webApiEntraIdObo = builder.AddDotnetProject("web-api-entraidobo", "../WebApiEntraIdObo/WebApiEntraIdObo.csproj")
    .WithReference(webApiEntraId)
    .WaitFor(webApiEntraId)
    .WithExternalHttpEndpoints();

builder.AddDotnetProject("razor-page-entraid", "../RazorPageEntraId/RazorPageEntraId.csproj")
    .WithReference(webApiEntraId)
    .WithReference(webApiEntraIdObo)
    .WaitFor(webApiEntraId)
    .WaitFor(webApiEntraIdObo)
    .WithExternalHttpEndpoints();

builder.Build().Run();
