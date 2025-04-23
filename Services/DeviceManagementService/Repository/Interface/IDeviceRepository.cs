
using DeviceManagementService.Models;

namespace DeviceManagementService.Repository.Interface
{
    public interface IDeviceRepository
    {
        Task CreateDeviceAsync(Device device);
        Task<List<Device>> GetAllDeviceAsync();
        Task<Device> GetDeviceByIdAsync(string deviceId);
        Task UpdateDeviceAsync(Device device);
        Task DeleteDeviceAsync(string deviceId);


    }
}
