using ControllerService.Models;
using ControllerService.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControllerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        private readonly IDeviceStatusRepository _deviceStatusRepository;

        public ControlController(IDeviceStatusRepository deviceStatusRepository)
        {
            _deviceStatusRepository = deviceStatusRepository;
        }

        [HttpPost("update-output")]
        public async Task<IActionResult> UpdateOutput([FromBody] DeviceOutputUpdateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.DeviceId))
                return BadRequest("Invalid request");

            var device = await _deviceStatusRepository.GetDeviceAsync(request.DeviceId);
            if (device == null)
            {
                // Create new device
                device = new DeviceStatus
                {
                    DeviceId = request.DeviceId,
                    DeviceName = request.DeviceName,
                    Location = request.Location,
                    Outputs = new(),
                    LastUpdated = DateTime.UtcNow
                };
            }

            // Check if output exists
            var output = device.Outputs.FirstOrDefault(o => o.Type == request.Type);
            if (output == null)
            {
                output = new DeviceOutput
                {
                    Type = request.Type,
                    Status = request.Status,
                    Value = request.Value
                };
                device.Outputs.Add(output);
            }
            else
            {
                output.Status = request.Status;
                output.Value = request.Value;
            }

            device.LastUpdated = DateTime.UtcNow;

            // Update device in DB
            await _deviceStatusRepository.UpdateDeviceAsync(device);

            // Save history
            var history = new OutputHistory
            {
                DeviceId = request.DeviceId,
                OutputType = request.Type,
                Status = request.Status,
                Value = request.Value,
                Timestamp = DateTime.UtcNow
            };
            await _deviceStatusRepository.AddHistoryAsync(history);

            return Ok(new { message = "Device output updated successfully." });
        }
    }

    public class DeviceOutputUpdateRequest
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; } // Optional
        public string Location { get; set; }    // Optional
        public string Type { get; set; }        // AC, Light, Fan
        public string Status { get; set; }      // on/off
        public int Value { get; set; }          // Temp or 1–10
    }
}

