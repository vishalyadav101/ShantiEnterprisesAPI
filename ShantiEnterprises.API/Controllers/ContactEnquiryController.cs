using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.ContactEnquiry;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactEnquiryController : ControllerBase
    {
        private readonly IContactEnquiryService _service;

        public ContactEnquiryController(
            IContactEnquiryService service)
        {
            _service = service;
        }


        // ==========================================
        // CREATE CONTACT ENQUIRY
        // PUBLIC
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(
            [FromBody] CreateContactEnquiryDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(dto);

                return Ok(result);
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
        // GET ALL
        // ADMIN ONLY
        // ==========================================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result =
                    await _service.GetAllAsync();

                return Ok(result);
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
        // GET BY ID
        // ADMIN ONLY
        // ==========================================

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var result =
                    await _service.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Contact enquiry not found."
                    });
                }

                return Ok(result);
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
        // UPDATE STATUS / REPLY
        // ADMIN ONLY
        // ==========================================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateContactEnquiryDto dto)
        {
            try
            {
                var result =
                    await _service.UpdateAsync(
                        id,
                        dto);

                return Ok(result);
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
        // DELETE
        // ADMIN ONLY
        // ==========================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "Contact enquiry deleted successfully."
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