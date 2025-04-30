using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ControllerService.Models
{
    public class DeviceStatus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Location { get; set; }

        public List<DeviceOutput> Outputs { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class DeviceOutput
    {
        public string Type { get; set; } // AC, Light, Fan
        public string Status { get; set; } // On / Off
        public int Value { get; set; } // Temp for AC, Level 1–10 for Light/Fan
    }
}
