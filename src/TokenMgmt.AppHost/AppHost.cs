var builder = DistributedApplication.CreateBuilder(args);

builder.AddDotnetProject("web-api-entraidobo", "../WebApiEntraIdObo/WebApiEntraIdObo.csproj");

builder.AddDotnetProject("web-api-entraid", "../WebApiEntraId/WebApiEntraId.csproj");

builder.AddDotnetProject("razor-page-entraid", "../RazorPageEntraId/RazorPageEntraId.csproj");

builder.Build().Run();
