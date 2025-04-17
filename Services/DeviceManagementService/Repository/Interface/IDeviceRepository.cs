
using DeviceManagementService.Models;

namespace DeviceManagementService.Repository.Interface
{
    public interface IDeviceRepository
    {
        Task CreateDeviceAsync(Device group);
        Task<List<Device>> GetAllDeviceAsync();
        Task<Device> GetDeviceByIdAsync(string groupId);
        Task UpdateDeviceAsync(Device group);
        Task DeleteDeviceAsync(string groupId);


    }
}
