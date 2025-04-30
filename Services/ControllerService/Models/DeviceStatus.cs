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

        [BsonRepresentation(BsonType.Document)]
        public Dictionary<string, DeviceOutput> Outputs { get; set; }

        public bool IsManualOverride { get; set; } // Is the device in manual mode?

        public DateTime LastUpdated { get; set; }
    }

    public class DeviceOutput
    {
        public string Status { get; set; }  // "On", "Off"
        public int Value { get; set; }      // For example: brightness level
    }
}
