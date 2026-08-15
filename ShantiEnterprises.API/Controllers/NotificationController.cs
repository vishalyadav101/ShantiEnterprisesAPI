using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Notification;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(
            INotificationService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetUserId();

            var result =
                await _service.GetMyNotificationsAsync(userId);

            return Ok(result);
        }

        // GET: api/Notification/unread
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = GetUserId();

            var result =
                await _service.GetUnreadNotificationsAsync(userId);

            return Ok(result);
        }

        // GET: api/Notification/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();

            var result =
                await _service.GetByIdAsync(
                    id,
                    userId);

            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message = "Notification not found."
                    });
            }

            return Ok(result);
        }

        // POST: api/Notification
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateNotificationDto dto)
        {
            var userId = GetUserId();

            var result =
                await _service.CreateAsync(
                    userId,
                    dto);

            return Ok(result);
        }

        // PUT: api/Notification/{id}/read
        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();

            var result =
                await _service.MarkAsReadAsync(
                    id,
                    userId);

            if (!result)
            {
                return NotFound(
                    new
                    {
                        message = "Notification not found."
                    });
            }

            return Ok(
                new
                {
                    message =
                        "Notification marked as read."
                });
        }

        // PUT: api/Notification/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();

            await _service.MarkAllAsReadAsync(userId);

            return Ok(
                new
                {
                    message =
                        "All notifications marked as read."
                });
        }

        // DELETE: api/Notification/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var result =
                await _service.DeleteAsync(
                    id,
                    userId);

            if (!result)
            {
                return NotFound(
                    new
                    {
                        message = "Notification not found."
                    });
            }

            return Ok(
                new
                {
                    message =
                        "Notification deleted successfully."
                });
        }
    }
}