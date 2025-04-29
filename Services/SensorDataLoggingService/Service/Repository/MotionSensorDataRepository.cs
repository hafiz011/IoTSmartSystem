using MongoDB.Bson;
using MongoDB.Driver;
using SensorDataLoggingService.DbContext;
using SensorDataLoggingService.Models;
using SensorDataLoggingService.Service.Interface;

namespace SensorDataLoggingService.Service.Repository
{
    public class MotionSensorDataRepository : IMotionSensorDataRepository
    {
        private readonly IMongoCollection<MotionSensorData> _sensor;
        public MotionSensorDataRepository(MongoDbContext context)
        {
            _sensor = context.MotionSensorData;
        }

        public async Task CreateAsync(MotionSensorData data)
        {
            data.Id = ObjectId.GenerateNewId().ToString();
            await _sensor.InsertOneAsync(data);
        }

        public async Task<List<MotionSensorData>> GetAllByDeviceIdAsync(string deviceId)
        {
            return await _sensor.Find(d => d.DeviceId == deviceId).ToListAsync();
        }

        public async Task DeleteByDeviceIdAsync(string deviceId)
        {
            var totalCount = await _sensor.CountDocumentsAsync(d => d.DeviceId == deviceId);

            if (totalCount > 1000)
            {
                // Get only the _ids of the documents to delete (oldest ones)
                var oldRecords = await _sensor
                    .Find(d => d.DeviceId == deviceId)
                    .SortBy(d => d.Timestamp) // Assuming 'Timestamp' field exists
                    .Limit((int)(totalCount - 1000))
                    .Project(d => d.Id)
                    .ToListAsync();

                var filter = Builders<MotionSensorData>.Filter.In(d => d.Id, oldRecords);
                await _sensor.DeleteManyAsync(filter);
            }

        }
    }
}
