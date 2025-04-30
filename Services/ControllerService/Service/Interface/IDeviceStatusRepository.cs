using ControllerService.Models;

namespace ControllerService.Service.Interface
{
    public interface IDeviceStatusRepository
    {
        Task<DeviceStatus> GetDeviceAsync(string deviceId);
        Task UpdateDeviceAsync(DeviceStatus device);
        //Task CreateAsync(DeviceStatus device);
        Task AddHistoryAsync(OutputHistory history);
    }
}
