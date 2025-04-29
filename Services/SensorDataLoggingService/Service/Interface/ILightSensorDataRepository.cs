using SensorDataLoggingService.Models;

namespace SensorDataLoggingService.Service.Interface
{
    public interface ILightSensorDataRepository
    {
        Task CreateAsync(LightSensorData data);
        Task<List<LightSensorData>> GetAllByDeviceIdAsync(string deviceId);
        Task DeleteByDeviceIdAsync(string deviceId);
    }
}
