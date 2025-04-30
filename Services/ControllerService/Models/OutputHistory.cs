using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ControllerService.Models
{
    public class OutputHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string OutputType { get; set; }
        public string Status { get; set; }
        public int Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
