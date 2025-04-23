using Grpc.Net.Client;
using DeviceManagementService.Grpc; // ✅ Namespace from .proto

namespace MqttSubscriberService
{
    public class DeviceClientService
    {
        public async Task<DeviceResponse> GetDeviceInfoAsync(string deviceId)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5002"); // URL of DeviceManagementService
            var client = new DeviceService.DeviceServiceClient(channel);

            var response = await client.GetDeviceInfoAsync(new DeviceRequest { Deviceid = deviceId });
            return response;
        }
    }
}
