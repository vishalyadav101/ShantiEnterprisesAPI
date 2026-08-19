using ShantiEnterprises.API.DTOs.Return;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IReturnRepository _returnRepository;
        private readonly IPaymentRepository _paymentRepository;

        public RefundService(
            IRefundRepository refundRepository,
            IReturnRepository returnRepository,
            IPaymentRepository paymentRepository)
        {
            _refundRepository = refundRepository;
            _returnRepository = returnRepository;
            _paymentRepository = paymentRepository;
        }


        // ==========================================
        // CREATE REFUND
        // ADMIN
        // ==========================================

        public async Task<RefundResponseDto> CreateRefundAsync(
            int returnId)
        {
            // ==========================================
            // GET RETURN
            // ==========================================

            var returnRequest =
                await _returnRepository
                    .GetByIdAsync(returnId);

            if (returnRequest == null)
            {
                throw new Exception(
                    "Return request not found.");
            }


            // ==========================================
            // CHECK RETURN STATUS
            // ==========================================

            if (!string.Equals(
                    returnRequest.ReturnStatus,
                    "ProductReceived",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Refund can only be created after the returned product is received.");
            }


            // ==========================================
            // CHECK EXISTING REFUND
            // ==========================================

            var existingRefund =
                await _refundRepository
                    .GetByReturnIdAsync(returnId);

            if (existingRefund != null)
            {
                throw new Exception(
                    "Refund already exists for this return.");
            }


            // ==========================================
            // GET PAYMENT
            // ==========================================

            var payment =
                await _paymentRepository
                    .GetByOrderIdAsync(
                        returnRequest.OrderId);

            if (payment == null)
            {
                throw new Exception(
                    "Payment not found for this order.");
            }


            // ==========================================
            // PAYMENT STATUS
            // ==========================================

            if (!string.Equals(
                    payment.PaymentStatus,
                    "Paid",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Refund can only be processed for a paid order.");
            }


            // ==========================================
            // GET ORDER ITEM
            // ==========================================

            if (returnRequest.OrderItem == null)
            {
                throw new Exception(
                    "Order item not found for this return.");
            }


            var refundAmount =
                returnRequest.OrderItem.TotalPrice;

            if (refundAmount <= 0)
            {
                throw new Exception(
                    "Refund amount must be greater than zero.");
            }


            // ==========================================
            // CREATE REFUND
            // ==========================================

            var refund = new Refund
            {
                ReturnId =
                    returnRequest.ReturnId,

                OrderId =
                    returnRequest.OrderId,

                PaymentId =
                    payment.PaymentId,

                RefundAmount =
                    refundAmount,

                RefundStatus =
                    "Pending",

                RefundReference =
                    null,

                RefundDate =
                    null,

                FailureReason =
                    null,

                CreatedDate =
                    DateTime.UtcNow,

                UpdatedDate =
                    null
            };


            var createdRefund =
                await _refundRepository
                    .CreateAsync(refund);


            // ==========================================
            // UPDATE RETURN STATUS
            // ==========================================

            returnRequest.ReturnStatus =
                "RefundProcessing";

            returnRequest.UpdatedDate =
                DateTime.UtcNow;

            returnRequest.Refund =
                createdRefund;

            await _returnRepository
                .UpdateAsync(returnRequest);


            // ==========================================
            // RESPONSE
            // ==========================================

            return MapToResponse(createdRefund);
        }


        // ==========================================
        // GET REFUND BY ID
        // ==========================================

        public async Task<RefundResponseDto> GetByIdAsync(
            int refundId,
            int userId,
            bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByIdAsync(refundId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found.");
            }


            // ==========================================
            // CUSTOMER OWNERSHIP
            // ==========================================

            if (!isAdmin)
            {
                if (refund.Order == null)
                {
                    throw new Exception(
                        "Refund order not found.");
                }

                if (refund.Order.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You are not authorized to access this refund.");
                }
            }


            return MapToResponse(refund);
        }


        // ==========================================
        // GET REFUND BY RETURN
        // ==========================================

        public async Task<RefundResponseDto> GetByReturnIdAsync(
            int returnId,
            int userId,
            bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByReturnIdAsync(returnId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found for this return.");
            }


            // ==========================================
            // CUSTOMER OWNERSHIP
            // ==========================================

            if (!isAdmin)
            {
                if (refund.Order == null)
                {
                    throw new Exception(
                        "Refund order not found.");
                }

                if (refund.Order.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You are not authorized to access this refund.");
                }
            }


            return MapToResponse(refund);
        }


        // ==========================================
        // GET REFUND BY ORDER
        // ==========================================

        public async Task<RefundResponseDto?> GetByOrderIdAsync(
            int orderId,
            int userId,
            bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByOrderIdAsync(orderId);

            if (refund == null)
            {
                return null;
            }


            // ==========================================
            // CUSTOMER OWNERSHIP
            // ==========================================

            if (!isAdmin)
            {
                if (refund.Order == null)
                {
                    throw new Exception(
                        "Refund order not found.");
                }

                if (refund.Order.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You are not authorized to access this refund.");
                }
            }


            return MapToResponse(refund);
        }

        // ==========================================
        // UPDATE REFUND STATUS
        // ADMIN
        // ==========================================

        public async Task<RefundResponseDto> UpdateStatusAsync(
            int refundId,
            RefundStatusUpdateDto dto)
        {
            // ==========================================
            // GET REFUND
            // ==========================================

            var refund =
                await _refundRepository
                    .GetByIdAsync(refundId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found.");
            }


            // ==========================================
            // VALIDATE STATUS
            // ==========================================

            var allowedStatuses =
                new[]
                {
            "Pending",
            "Processing",
            "Completed",
            "Failed"
                };

            if (!allowedStatuses.Contains(
                    dto.RefundStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid refund status.");
            }


            var newStatus =
                dto.RefundStatus.Trim();


            // ==========================================
            // PREVENT COMPLETED REFUND UPDATE
            // ==========================================

            if (string.Equals(
                    refund.RefundStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Completed refund cannot be updated.");
            }


            // ==========================================
            // UPDATE STATUS
            // ==========================================

            refund.RefundStatus =
                newStatus;


            // ==========================================
            // REFUND REFERENCE
            // ==========================================

            if (!string.IsNullOrWhiteSpace(
                    dto.RefundReference))
            {
                refund.RefundReference =
                    dto.RefundReference.Trim();
            }


            // ==========================================
            // FAILURE REASON
            // ==========================================

            if (string.Equals(
                    newStatus,
                    "Failed",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(
                        dto.FailureReason))
                {
                    throw new Exception(
                        "Failure reason is required when refund fails.");
                }

                refund.FailureReason =
                    dto.FailureReason.Trim();
            }
            else
            {
                refund.FailureReason = null;
            }


            // ==========================================
            // COMPLETED
            // ==========================================

            if (string.Equals(
                    newStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                refund.RefundDate =
                    DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(
                        refund.RefundReference))
                {
                    refund.RefundReference =
                        $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                }

                // ==========================================
                // UPDATE RETURN STATUS
                // ==========================================

                if (refund.Return != null)
                {
                    refund.Return.ReturnStatus =
                        "Completed";

                    refund.Return.CompletedDate =
                        DateTime.UtcNow;

                    refund.Return.UpdatedDate =
                        DateTime.UtcNow;

                    await _returnRepository
                        .UpdateAsync(refund.Return);
                }
            }


            // ==========================================
            // UPDATED DATE
            // ==========================================

            refund.UpdatedDate =
                DateTime.UtcNow;


            // ==========================================
            // SAVE REFUND
            // ==========================================

            await _refundRepository
                .UpdateAsync(refund);


            // ==========================================
            // RESPONSE
            // ==========================================

            return MapToResponse(refund);
        }

        // ==========================================
        // RESPONSE MAPPING
        // ==========================================

        private static RefundResponseDto MapToResponse(
            Refund refund)
        {
            return new RefundResponseDto
            {
                RefundId =
                    refund.RefundId,

                ReturnId =
                    refund.ReturnId,

                OrderId =
                    refund.OrderId,

                PaymentId =
                    refund.PaymentId,

                RefundAmount =
                    refund.RefundAmount,

                RefundStatus =
                    refund.RefundStatus,

                RefundReference =
                    refund.RefundReference,

                RefundDate =
                    refund.RefundDate,

                FailureReason =
                    refund.FailureReason,

                CreatedDate =
                    refund.CreatedDate,

                UpdatedDate =
                    refund.UpdatedDate
            };
        }
    }
}