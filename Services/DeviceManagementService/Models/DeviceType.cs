﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace DeviceManagementService.Models
{
    public class DeviceType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public Dictionary<string, object> Capabilities { get; set; }

    }
}
