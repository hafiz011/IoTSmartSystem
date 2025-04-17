using DeviceManagementService.DbContext;
using DeviceManagementService.Models;
using DeviceManagementService.Repository.Interface;
using MongoDB.Driver;

namespace DeviceManagementService.Repository.Repo
{
    public class DeviceTypeRepository : IDeviceTypeRepository
    {
        private readonly IMongoCollection<DeviceType> _deviceTypes;

        public DeviceTypeRepository(MongoDbContext context)
        {
            _deviceTypes = context.DeviceType;
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            // Create unique index on Name
            var nameIndex = new CreateIndexModel<DeviceType>(
                Builders<DeviceType>.IndexKeys.Ascending(dt => dt.Name),
                new CreateIndexOptions { Unique = true });

            // Create index on Manufacturer
            var manufacturerIndex = new CreateIndexModel<DeviceType>(
                Builders<DeviceType>.IndexKeys.Ascending(dt => dt.Manufacturer));

            _deviceTypes.Indexes.CreateMany(new[] { nameIndex, manufacturerIndex });
        }

        public async Task<DeviceType> CreateAsync(DeviceType deviceType)
        {
            await _deviceTypes.InsertOneAsync(deviceType);
            return deviceType;
        }

        public async Task<DeviceType> GetByIdAsync(string id)
        {
            return await _deviceTypes.Find(dt => dt.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<DeviceType>> GetAllAsync()
        {
            return await _deviceTypes.Find(_ => true).ToListAsync();
        }

        public async Task UpdateAsync(string id, DeviceType deviceType)
        {
            await _deviceTypes.ReplaceOneAsync(dt => dt.Id == id, deviceType);
        }

        public async Task DeleteAsync(string id)
        {
            await _deviceTypes.DeleteOneAsync(dt => dt.Id == id);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _deviceTypes.Find(dt => dt.Id == id).AnyAsync();
        }

    }
}