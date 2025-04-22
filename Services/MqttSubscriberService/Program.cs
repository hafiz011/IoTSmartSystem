using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();
//builder.Services.AddGrpcClient<DeviceChecker.DeviceCheckerClient>(o =>
//{
//    o.Address = new Uri("http://localhost:5005"); // replace with your actual DeviceManagementService address
//});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var host = builder.Build();
host.Run();
