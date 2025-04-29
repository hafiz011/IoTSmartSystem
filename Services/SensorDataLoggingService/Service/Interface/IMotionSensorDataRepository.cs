using SensorDataLoggingService.Models;

namespace SensorDataLoggingService.Service.Interface
{
    public interface IMotionSensorDataRepository
    {
        Task CreateAsync(MotionSensorData data);
        Task<List<MotionSensorData>> GetAllByDeviceIdAsync(string deviceId);
        Task DeleteByDeviceIdAsync(string deviceId);
    }
}
