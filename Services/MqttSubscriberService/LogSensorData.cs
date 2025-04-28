using Grpc.Net.Client;
using MqttSubscriberService.Models;
using SensorDataLoggingService.Grpc;
namespace MqttSubscriberService
{
    public class LogSensorData
    {
        public async Task<LogSensorDataResponse> SensorData(SensorDataDto data)
        {

                using var channel = GrpcChannel.ForAddress("https://localhost:5007");
                var client = new SensorDataLoggingService.SensorDataLoggerService.LogSensorDataC(channel);

                var forwardRequest = new SensorDataDto
                {
                    DeviceId = data.DeviceId,
                    Value = data.Value,
                    Type= data.Type,
                    ReceivedAt = data.ReceivedAt
                };

                var response = await client.LogSensorData(forwardRequest);

                return response.Success;
            
        }
    }
}
