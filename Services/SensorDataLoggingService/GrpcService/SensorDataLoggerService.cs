using Grpc.Core;
using Grpc.Net.Client;

namespace SensorDataLoggingService.GrpcService
{
    public class SensorDataLoggerService : SensorDataLogger.SensorDataLoggerBase
    {
        private readonly ILogger<SensorDataLoggerService> _logger;
        private readonly IConfiguration _configuration;

        public SensorDataLoggerService(ILogger<SensorDataLoggerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<LogSensorDataResponse> LogSensorData(SensorDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received Sensor Data:\n DeviceId: {request.DeviceId}\n Value: {request.Value}\n Type: {request.Type}\n ReceivedAt: {request.ReceivedAt}");

            try
            {
                // Now act as gRPC Client to forward this data to another service
                var forwardResult = await ForwardSensorDataToOtherServiceAsync(request);

                if (forwardResult)
                {
                    return new LogSensorDataResponse
                    {
                        Success = true,
                        Message = "Sensor data logged and forwarded successfully."
                    };
                }
                else
                {
                    return new LogSensorDataResponse
                    {
                        Success = false,
                        Message = "Failed to forward sensor data."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in LogSensorData: {ex.Message}");
                return new LogSensorDataResponse
                {
                    Success = false,
                    Message = "Error while processing sensor data."
                };
            }
        }

        private async Task<bool> ForwardSensorDataToOtherServiceAsync(SensorDataRequest data)
        {
            try
            {
                using var channel = Grpc.Net.Client.GrpcChannel.ForAddress(_configuration["OtherGrpcService:Url"]);
                var client = new SensorDataLogger.SensorDataLoggerClient(channel);

                var forwardRequest = new ForwardSensorDataRequest
                {
                    DeviceId = data.DeviceId,
                    Value = data.Value,
                    Type = data.Type,
                    ReceivedAt = data.ReceivedAt
                };

                var response = await client.AIservice(forwardRequest);

                return response.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to forward data: {ex.Message}");
                return false;
            }
        }

    }
}
