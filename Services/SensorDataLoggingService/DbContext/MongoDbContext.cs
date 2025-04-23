using Microsoft.Extensions.Options;
using MongoDB.Driver;
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


        //public IMongoCollection<SensorLog> Devices => _database.GetCollection<SensorLog>("Devices");
        //public IMongoCollection<SensorLog> DeviceGroups => _database.GetCollection<SensorLog>("Groups");
        //public IMongoCollection<DeviceType> DeviceType => _database.GetCollection<DeviceType>("Device_Types");

    }

}
