using DeviceManagementService;
using DeviceManagementService.DbContext;
using DeviceManagementService.Repository.Implementation;
using DeviceManagementService.Repository.Interface;
using DeviceManagementService.Repository.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IDeviceGroupRepository, DeviceGroupRepository>();
builder.Services.AddSingleton<IDeviceTypeRepository, DeviceTypeRepository>();

builder.Services.AddGrpc();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGrpcService<DeviceCheckerService>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
