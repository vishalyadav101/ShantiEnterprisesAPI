using ShantiEnterprises.API.DTOs.ContactEnquiry;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ContactEnquiryService : IContactEnquiryService
    {
        private readonly IContactEnquiryRepository _repository;

        public ContactEnquiryService(
            IContactEnquiryRepository repository)
        {
            _repository = repository;
        }


        // ==========================================
        // CREATE CONTACT ENQUIRY
        // ==========================================

        public async Task<ContactEnquiryResponseDto> CreateAsync(
            CreateContactEnquiryDto dto)
        {
            // ======================================
            // VALIDATION
            // ======================================

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Mobile))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Subject))
            {
                throw new ArgumentException(
                    "Subject is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new ArgumentException(
                    "Message is required.");
            }


            // ======================================
            // CREATE ENTITY
            // ======================================

            var contactEnquiry = new ContactEnquiry
            {
                FullName = dto.FullName.Trim(),

                Email = dto.Email.Trim(),

                Mobile = dto.Mobile.Trim(),

                Subject = dto.Subject.Trim(),

                Message = dto.Message.Trim(),

                Status = "Pending",

                AdminReply = null,

                RepliedDate = null,

                CreatedDate = DateTime.UtcNow,

                UpdatedDate = null
            };


            var created =
                await _repository.AddAsync(contactEnquiry);


            return MapToDto(created);
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<ContactEnquiryResponseDto>> GetAllAsync()
        {
            var enquiries =
                await _repository.GetAllAsync();

            return enquiries
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<ContactEnquiryResponseDto?> GetByIdAsync(
            int contactEnquiryId)
        {
            var enquiry =
                await _repository.GetByIdAsync(
                    contactEnquiryId);

            if (enquiry == null)
            {
                return null;
            }

            return MapToDto(enquiry);
        }


        // ==========================================
        // UPDATE STATUS / REPLY
        // ==========================================

        public async Task<ContactEnquiryResponseDto> UpdateAsync(
            int contactEnquiryId,
            UpdateContactEnquiryDto dto)
        {
            var enquiry =
                await _repository.GetByIdAsync(
                    contactEnquiryId);

            if (enquiry == null)
            {
                throw new KeyNotFoundException(
                    "Contact enquiry not found.");
            }


            // ======================================
            // STATUS VALIDATION
            // ======================================

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new ArgumentException(
                    "Status is required.");
            }


            var status =
                dto.Status.Trim();


            var allowedStatuses = new[]
            {
                "Pending",
                "InProgress",
                "Replied",
                "Closed"
            };


            if (!allowedStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid status. Allowed values: Pending, InProgress, Replied, Closed.");
            }


            // ======================================
            // UPDATE
            // ======================================

            enquiry.Status = status;

            enquiry.AdminReply =
                string.IsNullOrWhiteSpace(dto.AdminReply)
                    ? null
                    : dto.AdminReply.Trim();

            enquiry.UpdatedDate =
                DateTime.UtcNow;


            // ======================================
            // REPLIED DATE
            // ======================================

            if (status.Equals(
                    "Replied",
                    StringComparison.OrdinalIgnoreCase))
            {
                enquiry.RepliedDate =
                    DateTime.UtcNow;
            }
            else
            {
                enquiry.RepliedDate = null;
            }


            var updated =
                await _repository.UpdateAsync(enquiry);


            return MapToDto(updated);
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task DeleteAsync(
            int contactEnquiryId)
        {
            var deleted =
                await _repository.DeleteAsync(
                    contactEnquiryId);

            if (!deleted)
            {
                throw new KeyNotFoundException(
                    "Contact enquiry not found.");
            }
        }


        // ==========================================
        // ENTITY → DTO
        // ==========================================

        private static ContactEnquiryResponseDto MapToDto(
            ContactEnquiry enquiry)
        {
            return new ContactEnquiryResponseDto
            {
                ContactEnquiryId =
                    enquiry.ContactEnquiryId,

                FullName =
                    enquiry.FullName,

                Email =
                    enquiry.Email,

                Mobile =
                    enquiry.Mobile,

                Subject =
                    enquiry.Subject,

                Message =
                    enquiry.Message,

                Status =
                    enquiry.Status,

                AdminReply =
                    enquiry.AdminReply,

                RepliedDate =
                    enquiry.RepliedDate,

                CreatedDate =
                    enquiry.CreatedDate,

                UpdatedDate =
                    enquiry.UpdatedDate
            };
        }
    }
}