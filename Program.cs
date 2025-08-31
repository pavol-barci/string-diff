using StringDiff.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCoreServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Environment);

var app = builder.Build();
app.AddCore();
app.Run();