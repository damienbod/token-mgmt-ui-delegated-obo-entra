var builder = DistributedApplication.CreateBuilder(args);

builder.AddDotnetTool<Projects.WebApiEntraIdObo>("webapientraidobo");

builder.AddDotnetTool<Projects.WebApiEntraId>("webapientraid");

builder.AddDotnetTool<Projects.RazorPageEntraId>("razorpageentraid");

builder.Build().Run();
