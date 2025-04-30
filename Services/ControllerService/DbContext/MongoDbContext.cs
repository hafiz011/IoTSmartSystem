using ControllerService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace ControllerService.DbContext
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }


        public IMongoCollection<DeviceStatus> Devices => _database.GetCollection<DeviceStatus>("Devices_Status");
        public IMongoCollection<OutputHistory> OutputHistories => _database.GetCollection<OutputHistory>("Output_Histories");


    }

}
