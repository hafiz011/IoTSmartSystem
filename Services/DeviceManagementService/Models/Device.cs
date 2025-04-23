

using MongoDB.Bson.Serialization.Attributes;

namespace DeviceManagementService.Models
{
    public class Device 
    {
        [BsonId]
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string MACAddress { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GroupId { get; set; }
        public string FirmwareVersion { get; set; }
        public List<string> Tags { get; set; }
        public DateTime LastOnline { get; set; }
        public string DeviceTypeId { get; set; }
        public string SerialNumber { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdated { get; set; }
        public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
        public bool IsActive { get; set; } = true;
    }

    public enum DeviceStatus
    {
        Online,
        Offline,
        Maintenance,
        Retired
    }
}
