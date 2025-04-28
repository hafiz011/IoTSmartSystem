using Grpc.Net.Client;
using MqttSubscriberService.Models;
using SensorDataLoggingService.Grpc;
namespace MqttSubscriberService
{
    public class LogSensorData
    {
        public async Task<LogSensorDataResponse> SensorData(SensorDataRequest data)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5007"); // URL of DeviceManagementService
                var client = new SensorDataLoggingService.SensorDataLoggerServiceClient(channel);

                var forwardRequest = new SensorDataDto
                {
                    DeviceId = data.DeviceId,
                    Value = data.Value,
                    Type= data.Type,
                    ReceivedAt = data.ReceivedAt
                };

                var response = await client.SaveForwardedSensorDataAsync(forwardRequest);

                return response.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
