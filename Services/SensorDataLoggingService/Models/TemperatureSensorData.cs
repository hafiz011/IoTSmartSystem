using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorDataLoggingService.Models
{
    public class TemperatureSensorData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DeviceId { get; set; } 
        public double Value { get; set; } 
        public DateTime Timestamp { get; set; }
    }
}
