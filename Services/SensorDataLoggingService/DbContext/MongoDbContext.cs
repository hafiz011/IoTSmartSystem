using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SensorDataLoggingService.Models;
namespace SensorDataLoggingService.DbContext
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }


        public IMongoCollection<LightSensorData> LightSensorData => _database.GetCollection<LightSensorData>("LightSensorData");
        public IMongoCollection<TemperatureSensorData> TemperatureSensorData => _database.GetCollection<TemperatureSensorData>("TemperatureSensorData");
        public IMongoCollection<MotionSensorData> MotionSensorData => _database.GetCollection<MotionSensorData>("MotionSensorData");
        

    }

}
