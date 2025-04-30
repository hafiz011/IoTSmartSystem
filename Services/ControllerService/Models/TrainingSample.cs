using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TrainingSample
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string DeviceId { get; set; }

    // Input sensors at the moment of decision
    public double Temperature { get; set; }
    public double Light { get; set; }
    public bool Motion { get; set; }

    // Manual override flag
    public bool IsManual { get; set; }

    // Optional: source of manual input
    public string ManualSource { get; set; } // "MobileApp", "WebDashboard", etc.

    // Final output decisions
    public Dictionary<string, string> OutputStatus { get; set; } // Example: { "Light": "ON" }

    public DateTime Timestamp { get; set; }
}
