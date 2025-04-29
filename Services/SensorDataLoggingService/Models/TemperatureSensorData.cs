using MongoDB.Bson.Serialization.Attributes;

namespace SensorDataLoggingService.Models
{
    public class TemperatureSensorData
    {
        [BsonId]
        public string Id { get; set; }
        public string DeviceId { get; set; } 
        public double Value { get; set; } 
        public DateTime Timestamp { get; set; }
    }
}
