using DeviceManagementService.Models;
using DeviceManagementService.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.ComponentModel.DataAnnotations;

namespace DeviceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IDeviceRepository _adminRepository;


        public AdminController(IDeviceRepository adminRepository
           )
        {
            _adminRepository = adminRepository;
        }

        //register device 
        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice(Device dto)
        {
            try
            {
                dto.DeviceId = Guid.NewGuid().ToString();
                var existing = await _adminRepository.GetDeviceByIdAsync(dto.DeviceId);
                if (existing != null)
                    return BadRequest(new { Message = "Device already exists" });

                await _adminRepository.CreateDeviceAsync(dto);
                return Ok(new { Message = "Device registered successfully!", DeviceId = dto.DeviceId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // get device info
        [HttpGet("GetDevice")]
        public async Task<IActionResult> GetDevice([FromQuery] string deviceId)
        {
            try
            {
                var device = await _adminRepository.GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    return NotFound();
                }


                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllDevices([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                // Get total count separately
                var allDevices = await _adminRepository.GetAllDeviceAsync(); // Assuming this returns IQueryable or IEnumerable

                var totalCount = allDevices.Count();

                var devices = allDevices
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new
                    {
                        d.DeviceId,
                        d.Name,
                        d.Status,
                        d.LastUpdated
                    })
                    .ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Devices = devices
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // PUT: api/Device/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDevice([FromBody] Device device)
        {
            try
            {
                var existingDevice = await _adminRepository.GetDeviceByIdAsync(device.DeviceId);
                if (existingDevice == null)
                {
                    return NotFound(new { Message = "Device not found" });
                }

                await _adminRepository.UpdateDeviceAsync(device);
                return Ok(new { Message = "Device updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Device/delete/{deviceId}
        [HttpDelete("delete/{deviceId}")]
        public async Task<IActionResult> DeleteDevice(string deviceId)
        {
            try
            {
                var existingDevice = await _adminRepository.GetDeviceByIdAsync(deviceId);
                if (existingDevice == null)
                {
                    return NotFound(new { Message = "Device not found" });
                }

                await _adminRepository.DeleteDeviceAsync(deviceId);
                return Ok(new { Message = "Device deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }




    }
}
