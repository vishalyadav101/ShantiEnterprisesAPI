using ShantiEnterprises.API.DTOs.ContactEnquiry;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ContactEnquiryService : IContactEnquiryService
    {
        private readonly IContactEnquiryRepository _repository;
        private readonly IEmailService _emailService;

        public ContactEnquiryService(
            IContactEnquiryRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
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


            // ======================================
            // SEND ADMIN EMAIL
            // ======================================

            try
            {
                await _emailService.SendEmailAsync(
                    "vishalsagar2bhai@gmail.com",
                    "New Contact Enquiry - Shanti Enterprises",
                    $"""
                    <h2>New Contact Enquiry</h2>

                    <p><strong>Name:</strong> {created.FullName}</p>

                    <p><strong>Email:</strong> {created.Email}</p>

                    <p><strong>Mobile:</strong> {created.Mobile}</p>

                    <p><strong>Subject:</strong> {created.Subject}</p>

                    <p><strong>Message:</strong></p>

                    <p>{created.Message}</p>

                    <hr />

                    <p><strong>Status:</strong> Pending</p>

                    <p>
                        Please login to the admin panel to manage this enquiry.
                    </p>
                    """,
                    true);
            }
            catch
            {
                // Email failure should not rollback
                // successfully saved enquiry.
            }


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


            // ======================================
            // SEND REPLY EMAIL TO CUSTOMER
            // ======================================

            if (status.Equals(
                    "Replied",
                    StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(updated.AdminReply))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        updated.Email,
                        "Response to Your Enquiry - Shanti Enterprises",
                        $"""
                        <h2>Dear {updated.FullName},</h2>

                        <p>
                            Thank you for contacting Shanti Enterprises.
                        </p>

                        <p>
                            We have reviewed your enquiry regarding:
                        </p>

                        <p>
                            <strong>{updated.Subject}</strong>
                        </p>

                        <hr />

                        <h3>Our Response</h3>

                        <p>{updated.AdminReply}</p>

                        <hr />

                        <p>
                            If you have any further questions, please feel free
                            to contact us again.
                        </p>

                        <p>
                            Regards,<br />
                            <strong>Shanti Enterprises</strong>
                        </p>
                        """,
                        true);
                }
                catch
                {
                    // Email failure should not rollback
                    // successfully updated enquiry.
                }
            }


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