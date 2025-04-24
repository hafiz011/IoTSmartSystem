namespace SensorDataLoggingService.Models
{
    public class SensorDataDto
    {
        public string DeviceId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }
}
