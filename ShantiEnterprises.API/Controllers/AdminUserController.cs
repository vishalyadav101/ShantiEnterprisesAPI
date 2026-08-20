using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.AdminUser;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(
            IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }


        // ==========================================
        // GET ALL USERS
        // ADMIN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users =
                    await _adminUserService
                        .GetAllAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // GET USER BY ID
        // ADMIN
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var user =
                    await _adminUserService
                        .GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        message = "User not found."
                    });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // UPDATE USER
        // ADMIN
        // ==========================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] AdminUserUpdateDto dto)
        {
            try
            {
                var user =
                    await _adminUserService
                        .UpdateAsync(
                            id,
                            dto);

                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // UPDATE USER STATUS
        // ADMIN
        // ==========================================

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] AdminUserStatusDto dto)
        {
            try
            {
                var user =
                    await _adminUserService
                        .UpdateStatusAsync(
                            id,
                            dto);

                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // DELETE USER
        // ADMIN
        // ==========================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                await _adminUserService
                    .DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "User deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}