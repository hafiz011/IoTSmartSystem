﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SensorDataLoggingService.Models
{
    public class MotionSensorData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public bool Value { get; set; }  // true = motion detected
        public DateTime Timestamp { get; set; }
    }
}
