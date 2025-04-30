using ControllerService.DbContext;
using ControllerService.Models;
using ControllerService.Service.Interface;
using MongoDB.Driver;

namespace ControllerService.Service.Repository
{
    public class DeviceStatusRepository : IDeviceStatusRepository
    {
        private readonly MongoDbContext _context;

        public DeviceStatusRepository(MongoDbContext context)
        {
            _context = context;
        }


        public async Task AddHistoryAsync(OutputHistory history)
        {
            await _context.OutputHistories.InsertOneAsync(history);
        }

        //public async Task CreateAsync(DeviceStatus device)
        //{
        //    await _context.Devices.InsertOneAsync(device);
        //}

        public async Task<DeviceStatus> GetDeviceAsync(string deviceId)
        {
            return await _context.Devices.Find(d => d.DeviceId == deviceId).FirstOrDefaultAsync();
        }

        public async Task UpdateDeviceAsync(DeviceStatus device)
        {
            var filter = Builders<DeviceStatus>.Filter.Eq(d => d.DeviceId, device.DeviceId);
            await _context.Devices.ReplaceOneAsync(filter, device, new ReplaceOptions { IsUpsert = true });
        }
    }
}
