﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSubscriberService.Models
{
    public class SensorDataDto
    {
        public string DeviceId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
