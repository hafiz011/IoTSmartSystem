using DeviceManagementService.DbContext;
using DeviceManagementService.Models;
using DeviceManagementService.Repository.Interface;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeviceManagementService.Repository.Repo
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IMongoCollection<Device> _devices;

        public DeviceRepository(MongoDbContext context)
        {
            _devices = context.Devices;
        }

        public async Task<List<Device>> GetAllDeviceAsync()
        {
            return await _devices.Find(_ => true).ToListAsync();
        }

        public async Task<Device> GetDeviceByIdAsync(string deviceId)
        {
            return await _devices.Find(d => d.DeviceId == deviceId).FirstOrDefaultAsync();
        }

        public async Task CreateDeviceAsync(Device device)
        {
            device.DeviceId = ObjectId.GenerateNewId().ToString(); // Auto generate _id if not already set
            device.CreatedAt = DateTime.UtcNow;
            await _devices.InsertOneAsync(device);
        }

        public async Task UpdateDeviceAsync(Device device)
        {
            var filter = Builders<Device>.Filter.Eq(d => d.DeviceId, device.DeviceId);
            var update = Builders<Device>.Update
                .Set(d => d.Address, device.Address)
                .Set(d => d.GroupId, device.GroupId);

            await _devices.UpdateOneAsync(filter, update);
        }

        public async Task DeleteDeviceAsync(string deviceId)
        {
            var filter = Builders<Device>.Filter.Eq(d => d.DeviceId, deviceId);
            await _devices.DeleteOneAsync(filter);
        }
    }
}
