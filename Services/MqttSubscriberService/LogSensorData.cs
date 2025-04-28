using Grpc.Net.Client;
using SensorDataLoggingService.Grpc; // This is your generated proto classes
using MqttSubscriberService.Models;

namespace MqttSubscriberService
{
    public class LogSensorData
    {
        public async Task<LogSensorDataResponse> SensorData(SensorDataDto data)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5004");
            var client = new SensorDataLogger.SensorDataLoggerClient(channel);

            var request = new SensorDataRequest
            {
                DeviceId = data.DeviceId,
                ReceivedAt = data.ReceivedAt.ToString("o"),
                Value = data.Value,
                Type = data.Type
            };

            var response = await client.LogSensorDataAsync(request);
            return response;
        }
    }
}
