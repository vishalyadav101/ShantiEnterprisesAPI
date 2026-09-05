using Microsoft.Extensions.Options;

using RazorpayClient = Razorpay.Api.RazorpayClient;
using RazorpayPayment = Razorpay.Api.Payment;
using RazorpayRefund = Razorpay.Api.Refund;

using ShantiEnterprises.API.DTOs.Return;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;
using ShantiEnterprises.API.Settings;

namespace ShantiEnterprises.API.Services
{
    public class RefundService : IRefundService
    {
        // =========================================================
        // DEPENDENCIES
        // =========================================================

        private readonly IRefundRepository _refundRepository;

        private readonly IReturnRepository _returnRepository;

        private readonly IPaymentRepository _paymentRepository;

        private readonly INotificationService _notificationService;

        private readonly RazorpaySettings _razorpaySettings;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public RefundService(
            IRefundRepository refundRepository,
            IReturnRepository returnRepository,
            IPaymentRepository paymentRepository,
            INotificationService notificationService,
            IOptions<RazorpaySettings> razorpaySettings)
        {
            _refundRepository = refundRepository;

            _returnRepository = returnRepository;

            _paymentRepository = paymentRepository;

            _notificationService = notificationService;

            _razorpaySettings =
                razorpaySettings.Value;
        }


        // =========================================================
        // CREATE REFUND
        // ADMIN
        // =========================================================

        public async Task<RefundResponseDto>
            CreateRefundAsync(
                int returnId)
        {
            // =====================================================
            // GET RETURN
            // =====================================================

            var returnRequest =
                await _returnRepository
                    .GetByIdAsync(
                        returnId);

            if (returnRequest == null)
            {
                throw new Exception(
                    "Return request not found.");
            }


            // =====================================================
            // RETURN STATUS
            // =====================================================

            if (!string.Equals(
                    returnRequest.ReturnStatus,
                    "ProductReceived",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Refund can only be created after the returned product is received.");
            }


            // =====================================================
            // CHECK EXISTING REFUND
            // =====================================================

            var existingRefund =
                await _refundRepository
                    .GetByReturnIdAsync(
                        returnId);

            if (existingRefund != null)
            {
                throw new Exception(
                    "Refund already exists for this return.");
            }


            // =====================================================
            // GET PAYMENT
            // =====================================================

            var payment =
                await _paymentRepository
                    .GetByOrderIdAsync(
                        returnRequest.OrderId);

            if (payment == null)
            {
                throw new Exception(
                    "Payment not found for this order.");
            }


            // =====================================================
            // PAYMENT STATUS
            // =====================================================

            if (!string.Equals(
                    payment.PaymentStatus,
                    "Paid",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Refund can only be processed for a paid order.");
            }


            // =====================================================
            // RAZORPAY PAYMENT ID
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                    payment.RazorpayPaymentId))
            {
                throw new Exception(
                    "Razorpay payment ID not found for this order.");
            }


            // =====================================================
            // ORDER ITEM
            // =====================================================

            if (returnRequest.OrderItem == null)
            {
                throw new Exception(
                    "Order item not found for this return.");
            }


            // =====================================================
            // REFUND AMOUNT
            // =====================================================

            var refundAmount =
                returnRequest
                    .OrderItem
                    .TotalPrice;

            if (refundAmount <= 0)
            {
                throw new Exception(
                    "Refund amount must be greater than zero.");
            }


            // =====================================================
            // RAZORPAY SETTINGS
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                    _razorpaySettings.KeyId)
                ||
                string.IsNullOrWhiteSpace(
                    _razorpaySettings.KeySecret))
            {
                throw new Exception(
                    "Razorpay API keys are not configured.");
            }


            // =====================================================
            // CREATE RAZORPAY CLIENT
            // =====================================================

            var client =
                new RazorpayClient(
                    _razorpaySettings.KeyId,
                    _razorpaySettings.KeySecret);


            // =====================================================
            // REFUND AMOUNT IN PAISE
            // =====================================================

            var amountInPaise =
                Convert.ToInt64(
                    Math.Round(
                        refundAmount * 100m,
                        0,
                        MidpointRounding.AwayFromZero));


            if (amountInPaise <= 0)
            {
                throw new Exception(
                    "Invalid refund amount.");
            }


            // =====================================================
            // CREATE UNIQUE RECEIPT
            // =====================================================

            var receipt =
                $"RET-{returnRequest.ReturnId}-{DateTime.UtcNow:yyyyMMddHHmmss}";


            // =====================================================
            // RAZORPAY REFUND OPTIONS
            // =====================================================

            var refundOptions =
                new Dictionary<string, object>
                {
                    {
                        "amount",
                        amountInPaise
                    },

                    {
                        "currency",
                        "INR"
                    },

                    {
                        "receipt",
                        receipt
                    }
                };


            // =====================================================
            // FETCH RAZORPAY PAYMENT
            // =====================================================

            RazorpayPayment razorpayPayment;

            try
            {
                razorpayPayment =
                    client.Payment.Fetch(
                        payment.RazorpayPaymentId);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Unable to fetch Razorpay payment: {ex.Message}",
                    ex);
            }


            // =====================================================
            // VERIFY CAPTURED PAYMENT
            // =====================================================

            var razorpayPaymentStatus =
                razorpayPayment["status"]
                    ?.ToString();

            if (!string.Equals(
                    razorpayPaymentStatus,
                    "captured",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    $"Razorpay payment is not captured. Current status: {razorpayPaymentStatus ?? "Unknown"}.");
            }


            // =====================================================
            // CHECK PAYMENT AMOUNT
            // =====================================================

            var razorpayPaymentAmount =
                Convert.ToInt64(
                    razorpayPayment["amount"]);


            if (amountInPaise >
                razorpayPaymentAmount)
            {
                throw new Exception(
                    "Refund amount cannot be greater than the Razorpay payment amount.");
            }


            // =====================================================
            // CREATE ACTUAL RAZORPAY REFUND
            // =====================================================

            RazorpayRefund razorpayRefund;

            try
            {
                razorpayRefund =
                    razorpayPayment.Refund(
                        refundOptions);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Razorpay refund failed: {ex.Message}",
                    ex);
            }


            // =====================================================
            // GET RAZORPAY REFUND ID
            // =====================================================

            var razorpayRefundId =
                razorpayRefund["id"]
                    ?.ToString();

            if (string.IsNullOrWhiteSpace(
                    razorpayRefundId))
            {
                throw new Exception(
                    "Razorpay refund was created but refund ID was not returned.");
            }


            // =====================================================
            // GET RAZORPAY REFUND STATUS
            // =====================================================

            var razorpayRefundStatus =
                razorpayRefund["status"]
                    ?.ToString();


            // =====================================================
            // MAP RAZORPAY STATUS TO LOCAL STATUS
            // =====================================================

            var localRefundStatus =
                MapRazorpayRefundStatus(
                    razorpayRefundStatus);


            // =====================================================
            // CREATE LOCAL REFUND
            // =====================================================

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
                    localRefundStatus,

                RefundReference =
                    razorpayRefundId,

                RefundDate =
                    IsRefundCompleted(
                        localRefundStatus)
                        ? DateTime.UtcNow
                        : null,

                FailureReason =
                    null,

                CreatedDate =
                    DateTime.UtcNow,

                UpdatedDate =
                    DateTime.UtcNow
            };


            // =====================================================
            // SAVE LOCAL REFUND
            // =====================================================

            var createdRefund =
                await _refundRepository
                    .CreateAsync(
                        refund);


            // =====================================================
            // UPDATE RETURN
            // =====================================================

            returnRequest.Refund =
                createdRefund;

            returnRequest.UpdatedDate =
                DateTime.UtcNow;


            if (IsRefundCompleted(
                localRefundStatus))
            {
                returnRequest.ReturnStatus =
                    "Completed";

                returnRequest.CompletedDate =
                    DateTime.UtcNow;
            }
            else
            {
                returnRequest.ReturnStatus =
                    "RefundProcessing";
            }


            await _returnRepository
                .UpdateAsync(
                    returnRequest);


            // =====================================================
            // CUSTOMER NOTIFICATION
            // =====================================================

            await CreateRefundNotificationAsync(
                createdRefund,
                localRefundStatus,
                returnRequest);


            // =====================================================
            // RESPONSE
            // =====================================================

            return MapToResponse(
                createdRefund);
        }


        // =========================================================
        // GET REFUND BY ID
        // =========================================================

        public async Task<RefundResponseDto>
            GetByIdAsync(
                int refundId,
                int userId,
                bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByIdAsync(
                        refundId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found.");
            }


            // =====================================================
            // CUSTOMER OWNERSHIP
            // =====================================================

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


            return MapToResponse(
                refund);
        }


        // =========================================================
        // GET REFUND BY RETURN
        // =========================================================

        public async Task<RefundResponseDto>
            GetByReturnIdAsync(
                int returnId,
                int userId,
                bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByReturnIdAsync(
                        returnId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found for this return.");
            }


            // =====================================================
            // CUSTOMER OWNERSHIP
            // =====================================================

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


            return MapToResponse(
                refund);
        }


        // =========================================================
        // GET REFUND BY ORDER
        // =========================================================

        public async Task<RefundResponseDto?>
            GetByOrderIdAsync(
                int orderId,
                int userId,
                bool isAdmin)
        {
            var refund =
                await _refundRepository
                    .GetByOrderIdAsync(
                        orderId);

            if (refund == null)
            {
                return null;
            }


            // =====================================================
            // CUSTOMER OWNERSHIP
            // =====================================================

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


            return MapToResponse(
                refund);
        }


        // =========================================================
        // UPDATE REFUND STATUS
        // ADMIN
        // =========================================================

        public async Task<RefundResponseDto>
            UpdateStatusAsync(
                int refundId,
                RefundStatusUpdateDto dto)
        {
            // =====================================================
            // GET LOCAL REFUND
            // =====================================================

            var refund =
                await _refundRepository
                    .GetByIdAsync(
                        refundId);

            if (refund == null)
            {
                throw new Exception(
                    "Refund not found.");
            }


            // =====================================================
            // VALIDATE STATUS
            // =====================================================

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
                allowedStatuses.First(
                    x => x.Equals(
                        dto.RefundStatus.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            var oldStatus =
                refund.RefundStatus;

            // =====================================================
            // PREVENT UPDATE AFTER COMPLETION
            // =====================================================

            if (string.Equals(
                    oldStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Completed refund cannot be updated.");
            }

            // =====================================================
            // UPDATE REFUND STATUS
            // =====================================================

            refund.RefundStatus = newStatus;


            // =====================================================
            // IMPORTANT:
            // ACTUAL RAZORPAY REFUND WAS ALREADY CREATED
            //
            // Here we only update local status.
            // =====================================================


            // =====================================================
            // UPDATE REFERENCE
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                    dto.RefundReference))
            {
                refund.RefundReference =
                    dto.RefundReference.Trim();
            }


            // =====================================================
            // FAILED
            // =====================================================

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
                refund.FailureReason =
                    null;
            }


            // =====================================================
            // COMPLETED
            // =====================================================

            if (string.Equals(
                    newStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                refund.RefundDate =
                    refund.RefundDate
                    ?? DateTime.UtcNow;


                if (string.IsNullOrWhiteSpace(
                    refund.RefundReference))
                {
                    refund.RefundReference =
                        $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                }


                if (refund.Return != null)
                {
                    refund.Return.ReturnStatus =
                        "Completed";

                    refund.Return.CompletedDate =
                        DateTime.UtcNow;

                    refund.Return.UpdatedDate =
                        DateTime.UtcNow;


                    await _returnRepository
                        .UpdateAsync(
                            refund.Return);
                }
            }


            // =====================================================
            // UPDATE DATE
            // =====================================================

            refund.UpdatedDate =
                DateTime.UtcNow;


            // =====================================================
            // SAVE
            // =====================================================

            await _refundRepository
                .UpdateAsync(
                    refund);


            // =====================================================
            // NOTIFICATION
            // =====================================================

            if (!string.Equals(
                    oldStatus,
                    newStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                await CreateRefundNotificationAsync(
                    refund,
                    newStatus,
                    refund.Return);
            }


            return MapToResponse(
                refund);
        }


        // =========================================================
        // MAP RAZORPAY REFUND STATUS
        // =========================================================

        private static string
            MapRazorpayRefundStatus(
                string? razorpayStatus)
        {
            if (string.IsNullOrWhiteSpace(
                razorpayStatus))
            {
                return "Processing";
            }


            return razorpayStatus
                .Trim()
                .ToLowerInvariant() switch
            {
                "processed" =>
                    "Completed",

                "failed" =>
                    "Failed",

                "pending" =>
                    "Pending",

                _ =>
                    "Processing"
            };
        }


        // =========================================================
        // CHECK COMPLETED
        // =========================================================

        private static bool
            IsRefundCompleted(
                string status)
        {
            return string.Equals(
                status,
                "Completed",
                StringComparison.OrdinalIgnoreCase);
        }


        // =========================================================
        // REFUND NOTIFICATION
        // =========================================================

        private async Task
            CreateRefundNotificationAsync(
                Refund refund,
                string eventStatus,
                Return? returnRequest)
        {
            int? userId = null;


            // =====================================================
            // USER ID
            // =====================================================

            if (returnRequest != null)
            {
                userId =
                    returnRequest.UserId;
            }
            else if (refund.Return != null)
            {
                userId =
                    refund.Return.UserId;
            }


            if (!userId.HasValue)
            {
                return;
            }


            // =====================================================
            // ORDER NUMBER
            // =====================================================

            var orderNumber =
                refund.Order?.OrderNumber
                ?? returnRequest?.Order?.OrderNumber
                ?? $"#{refund.OrderId}";


            string title;

            string message;


            // =====================================================
            // MESSAGE
            // =====================================================

            switch (
                eventStatus.ToLowerInvariant())
            {
                case "created":

                case "pending":

                    title =
                        "Refund Pending";

                    message =
                        $"Your refund for order {orderNumber} has been initiated and is pending.";

                    break;


                case "processing":

                    title =
                        "Refund Processing";

                    message =
                        $"Your refund for order {orderNumber} is currently being processed.";

                    break;


                case "completed":

                    title =
                        "Refund Completed";

                    message =
                        $"Your refund of ₹{refund.RefundAmount:0.##} for order {orderNumber} has been completed successfully.";

                    if (!string.IsNullOrWhiteSpace(
                        refund.RefundReference))
                    {
                        message +=
                            $" Reference: {refund.RefundReference}.";
                    }

                    break;


                case "failed":

                    title =
                        "Refund Failed";

                    message =
                        $"Your refund for order {orderNumber} could not be completed.";

                    if (!string.IsNullOrWhiteSpace(
                        refund.FailureReason))
                    {
                        message +=
                            $" Reason: {refund.FailureReason}";
                    }

                    break;


                default:

                    title =
                        "Refund Status Updated";

                    message =
                        $"Your refund for order {orderNumber} has been updated to {refund.RefundStatus}.";

                    break;
            }


            // =====================================================
            // CREATE NOTIFICATION
            // =====================================================

            await _notificationService.CreateAsync(
                userId.Value,
                new DTOs.Notification.CreateNotificationDto
                {
                    Title =
                        title,

                    Message =
                        message,

                    Type =
                        "Refund",

                    ReferenceType =
                        "Order",

                    ReferenceId =
                        refund.OrderId
                });
        }


        // =========================================================
        // RESPONSE MAPPING
        // =========================================================

        private static RefundResponseDto
            MapToResponse(
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