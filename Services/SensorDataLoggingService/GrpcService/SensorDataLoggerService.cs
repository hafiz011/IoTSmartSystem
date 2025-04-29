using Grpc.Core;
using Grpc.Net.Client;
using SensorDataLoggingService.Models;
using SensorDataLoggingService.Service.Interface;

namespace SensorDataLoggingService.GrpcService
{
    public class SensorDataLoggerService : SensorDataLogger.SensorDataLoggerBase
    {
        private readonly ILogger<SensorDataLoggerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITemperatureSensorDataRepository _temperatureSensorDataRepository;
        private readonly ILightSensorDataRepository _lightSensorDataRepository;
        private readonly IMotionSensorDataRepository _motionSensorDataRepository;

        public SensorDataLoggerService(ILogger<SensorDataLoggerService> logger,
            IConfiguration configuration,
            ITemperatureSensorDataRepository temperatureSensorDataRepository,
            ILightSensorDataRepository lightSensorDataRepository,
            IMotionSensorDataRepository motionSensorDataRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _lightSensorDataRepository = lightSensorDataRepository;
            _motionSensorDataRepository = motionSensorDataRepository;
            _temperatureSensorDataRepository = temperatureSensorDataRepository;
        }

        public override async Task<LogSensorDataResponse> LogSensorData(SensorDataRequest request, ServerCallContext context)
        {
           

            try
            {

                switch (request.Type.ToLower())
                {
                    case "temperature":
                        var tempData = new TemperatureSensorData
                        {
                            DeviceId = request.DeviceId,
                            Value = double.Parse(request.Value),
                            Timestamp = DateTime.UtcNow
                        };
                        await _temperatureSensorDataRepository.CreateAsync(tempData);
                        break;

                    case "light":
                        var lightData = new LightSensorData
                        {
                            DeviceId = request.DeviceId,
                            Value = double.Parse(request.Value),
                            Timestamp = DateTime.UtcNow
                        };
                        await _lightSensorDataRepository.CreateAsync(lightData);
                        break;

                    case "motion":
                        var motionData = new MotionSensorData
                        {
                            DeviceId = request.DeviceId,
                            Value = bool.Parse(request.Value),
                            Timestamp = DateTime.UtcNow
                        };
                        await _motionSensorDataRepository.CreateAsync(motionData);
                        break;

                    default:
                       _logger.LogError($"Fail to store Sensor Data:\n DeviceId: {request.DeviceId}\n Value: {request.Value}\n Type: {request.Type}\n ReceivedAt: {request.ReceivedAt = DateTime.UtcNow.ToString()}");
                        return new LogSensorDataResponse
                        {
                            Success = false,
                            Message = $"Sensor type '{request.Type}' is not supported."
                        };
                }

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
                using var channel = GrpcChannel.ForAddress(_configuration["OtherGrpcService:Url"]);
                var client = new SensorDataLogger.SensorDataLoggerClient(channel);

                var forwardRequest = new ForwardDataRequest
                {
                    DeviceId = data.DeviceId,
                    Value = data.Value,
                    Type = data.Type,
                    ReceivedAt = data.ReceivedAt
                };

                var response = await client.ForwardSensorDataAsync(forwardRequest);
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
