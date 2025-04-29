using SensorDataLoggingService.Models;

namespace SensorDataLoggingService.Service.Interface
{
    public interface ITemperatureSensorDataRepository
    {
        Task CreateAsync(TemperatureSensorData data);
        Task<List<TemperatureSensorData>> GetAllByDeviceIdAsync(string deviceId);
        Task DeleteByDeviceIdAsync(string deviceId);
    }
}
