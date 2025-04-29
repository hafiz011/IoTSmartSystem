using SensorDataLoggingService.DbContext;
using SensorDataLoggingService.Service.Interface;
using SensorDataLoggingService.Service.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();


builder.Services.AddSingleton<ILightSensorDataRepository, LightSensorDataRepository>();
builder.Services.AddSingleton<IMotionSensorDataRepository, MotionSensorDataRepository>();
builder.Services.AddSingleton<ITemperatureSensorDataRepository, TemperatureSensorDataRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
