using DeviceManagementService.Models;
using DeviceManagementService.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private readonly IDeviceRepository _deviceRepository;

        public DevicesController(ILogger<DevicesController> logger, IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
        }

        // get device info
        [HttpGet("GetDevice")]
        public async Task<IActionResult> GetDevice([FromQuery] string deviceId)
        {
            try
            {
                var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    return NotFound();
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device {Id}", deviceId);
                return StatusCode(500, "Internal server error");
            }
        }

        // update device
        [HttpPost("location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateDto Update)
        {
            try
            {
                var device = await _deviceRepository.GetDeviceByIdAsync(Update.DeviceId);

                if (device == null)
                {
                    return NotFound(new { Message = "Device not found" });
                }
                
                device.GroupId = Update.GroupId;
                device.Address = Update.Address;
                device.LastUpdated = Update.LastUpdated = DateTime.UtcNow;

                 await _deviceRepository.UpdateDeviceAsync(device);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update device location");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }





    }

    public class UpdateDto
    {
        public string DeviceId { get; set; }
        public string GroupId { get; set; }
        public string Address { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}