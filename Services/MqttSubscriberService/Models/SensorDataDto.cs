using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSubscriberService.Models
{
    public class SensorDataDto
    {
        public string DeviceId { get; set; }
        public double Value { get; set; } // e.g., temperature or humidity
        public string Type { get; set; }  // e.g., "temperature"
        public DateTime ReceivedAt { get; set; }
    }
}
