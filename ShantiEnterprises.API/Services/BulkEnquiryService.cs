using ShantiEnterprises.API.DTOs.BulkEnquiry;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class BulkEnquiryService
        : IBulkEnquiryService
    {
        private readonly IBulkEnquiryRepository _repository;
        private readonly IProductRepository _productRepository;

        public BulkEnquiryService(
            IBulkEnquiryRepository repository,
            IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<BulkEnquiryResponseDto>>
            GetAllAsync()
        {
            var enquiries =
                await _repository.GetAllAsync();

            return enquiries
                .Select(Map)
                .ToList();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<BulkEnquiryResponseDto?>
            GetByIdAsync(int id)
        {
            var enquiry =
                await _repository.GetByIdAsync(id);

            if (enquiry == null)
            {
                return null;
            }

            return Map(enquiry);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<BulkEnquiryResponseDto>
            CreateAsync(
                BulkEnquiryCreateDto dto)
        {
            ValidateCustomerData(dto);

            if (dto.Quantity <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than 0.");
            }

            if (dto.ProductId.HasValue)
            {
                var product =
                    await _productRepository.GetByIdAsync(
                        dto.ProductId.Value);

                if (product == null)
                {
                    throw new Exception(
                        "Product not found.");
                }
            }

            var enquiry = new BulkEnquiry
            {
                UserId =
                    dto.UserId,

                CustomerName =
                    dto.CustomerName.Trim(),

                Mobile =
                    dto.Mobile.Trim(),

                Email =
                    dto.Email.Trim(),

                ProductId =
                    dto.ProductId,

                Quantity =
                    dto.Quantity,

                Message =
                    dto.Message.Trim(),

                Status =
                    "Pending",

                CreatedDate =
                    DateTime.UtcNow
            };

            var result =
                await _repository.AddAsync(enquiry);

            return Map(result);
        }

        // =========================
        // UPDATE
        // =========================

        public async Task<BulkEnquiryResponseDto>
            UpdateAsync(
                int id,
                BulkEnquiryUpdateDto dto)
        {
            var enquiry =
                await _repository.GetByIdAsync(id);

            if (enquiry == null)
            {
                throw new Exception(
                    "Bulk enquiry not found.");
            }

            ValidateCustomerData(dto);

            if (dto.Quantity <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than 0.");
            }

            if (dto.ProductId.HasValue)
            {
                var product =
                    await _productRepository.GetByIdAsync(
                        dto.ProductId.Value);

                if (product == null)
                {
                    throw new Exception(
                        "Product not found.");
                }
            }

            var validStatuses = new[]
            {
                "Pending",
                "Contacted",
                "Quoted",
                "Converted",
                "Rejected"
            };

            var status =
                dto.Status.Trim();

            if (!validStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid enquiry status. " +
                    "Allowed values: Pending, Contacted, Quoted, Converted, Rejected.");
            }

            status =
                validStatuses.First(x =>
                    x.Equals(
                        status,
                        StringComparison.OrdinalIgnoreCase));

            enquiry.CustomerName =
                dto.CustomerName.Trim();

            enquiry.Mobile =
                dto.Mobile.Trim();

            enquiry.Email =
                dto.Email.Trim();

            enquiry.ProductId =
                dto.ProductId;

            enquiry.Quantity =
                dto.Quantity;

            enquiry.Message =
                dto.Message.Trim();

            enquiry.Status =
                status;

            await _repository.UpdateAsync(
                enquiry);

            return Map(enquiry);
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool>
            DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // =========================
        // VALIDATION
        // =========================

        private static void ValidateCustomerData(
            string customerName,
            string mobile,
            string email)
        {
            if (string.IsNullOrWhiteSpace(
                    customerName))
            {
                throw new Exception(
                    "Customer name is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    mobile))
            {
                throw new Exception(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    email))
            {
                throw new Exception(
                    "Email is required.");
            }
        }

        private static void ValidateCustomerData(
            BulkEnquiryCreateDto dto)
        {
            ValidateCustomerData(
                dto.CustomerName,
                dto.Mobile,
                dto.Email);
        }

        private static void ValidateCustomerData(
            BulkEnquiryUpdateDto dto)
        {
            ValidateCustomerData(
                dto.CustomerName,
                dto.Mobile,
                dto.Email);
        }

        // =========================
        // MAP
        // =========================

        private static BulkEnquiryResponseDto Map(
            BulkEnquiry enquiry)
        {
            return new BulkEnquiryResponseDto
            {
                BulkEnquiryId =
                    enquiry.BulkEnquiryId,

                UserId =
                    enquiry.UserId,

                CustomerName =
                    enquiry.CustomerName,

                Mobile =
                    enquiry.Mobile,

                Email =
                    enquiry.Email,

                ProductId =
                    enquiry.ProductId,

                ProductName =
                    enquiry.Product?.ProductName,

                Quantity =
                    enquiry.Quantity,

                Message =
                    enquiry.Message,

                Status =
                    enquiry.Status,

                CreatedDate =
                    enquiry.CreatedDate
            };
        }
    }
}