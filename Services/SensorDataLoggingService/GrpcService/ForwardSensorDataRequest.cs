namespace SensorDataLoggingService.GrpcService
{
    internal class ForwardSensorDataRequest
    {
        public string DeviceId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string ReceivedAt { get; set; }
    }
}