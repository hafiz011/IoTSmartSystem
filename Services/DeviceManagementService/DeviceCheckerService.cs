using DeviceManagementService.Repository.Interface;
using DeviceManagementService.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
namespace DeviceManagementService
{
    public class DeviceCheckerService : DeviceChecker.DeviceCheckerBase
    {
        private readonly ILogger<DeviceCheckerService> _logger;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceCheckerService(ILogger<DeviceCheckerService> logger, IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
        }

        public override async Task<DeviceResponse> CheckDevice(DeviceRequest request, ServerCallContext context)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);

            if (device != null && device.MACAddress == request.MacAddress)
            {
                return new DeviceResponse
                {
                    IsValid = true,
                    Message = "Device is valid."
                };
            }

            return new DeviceResponse
            {
                IsValid = false,
                Message = "Device not found or MAC address mismatch."
            };
        }
    }

    public class DeviceRequest
    { 
        public string DeviceId { get; set;}
        public string MacAddress { get; set;}
    }
}