using DeviceManagementService.Models;
using DeviceManagementService.Repository.Interface;
using Grpc.Core;
using MongoDB.Driver;

namespace DeviceManagementService.GrpcServices
{
    public class DeviceGrpcService : DeviceService.DeviceServiceBase
    {
        private readonly IDeviceRepository _Repository;

        public DeviceGrpcService(IDeviceRepository Repository)
        {
            _Repository = Repository;
        }
        public override async Task<DeviceResponse> GetDeviceInfo(DeviceRequest request, ServerCallContext context)
        {

            var device = await _Repository.GetDeviceByIdAsync(request.Deviceid);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Device {request.Deviceid} not found"));
            }

            var response = new DeviceResponse
            {
                Deviceid = device.DeviceId,
                Macaddress = device.MACAddress,
                Status = device.Status.ToString(),
            };

            return response;
        }
    }
}
