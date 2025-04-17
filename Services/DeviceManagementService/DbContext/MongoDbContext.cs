using DeviceManagementService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace DeviceManagementService.DbContext
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }


        public IMongoCollection<Device> Devices => _database.GetCollection<Device>("Devices");
        public IMongoCollection<DeviceGroup> DeviceGroups => _database.GetCollection<DeviceGroup>("Groups");
        public IMongoCollection<DeviceType> DeviceType => _database.GetCollection<DeviceType>("Device_Types");

    }

}
