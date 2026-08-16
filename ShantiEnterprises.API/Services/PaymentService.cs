using Microsoft.Extensions.Options;
using RazorpayClient = Razorpay.Api.RazorpayClient;
using RazorpayUtils = Razorpay.Api.Utils;
using ShantiEnterprises.API.DTOs.Payment;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;
using ShantiEnterprises.API.Settings;

namespace ShantiEnterprises.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly RazorpaySettings _razorpaySettings;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IOptions<RazorpaySettings> razorpaySettings)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _razorpaySettings = razorpaySettings.Value;
        }

        // =========================================================
        // CREATE PAYMENT
        // =========================================================

        public async Task<PaymentResponseDto> CreatePaymentAsync(
            int userId,
            CreatePaymentDto dto)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    dto.OrderId,
                    userId);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            var existingPayment =
                await _paymentRepository.GetByOrderIdAsync(
                    dto.OrderId);

            if (existingPayment != null)
            {
                throw new Exception(
                    "Payment already exists for this order.");
            }

            var paymentMethod =
                dto.PaymentMethod.Trim();

            if (!paymentMethod.Equals(
                    "CashOnDelivery",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !paymentMethod.Equals(
                    "Online",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !paymentMethod.Equals(
                    "Razorpay",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid payment method. Use CashOnDelivery, Online or Razorpay.");
            }

            if (paymentMethod.Equals(
                    "CashOnDelivery",
                    StringComparison.OrdinalIgnoreCase))
            {
                paymentMethod = "CashOnDelivery";
            }
            else if (paymentMethod.Equals(
                         "Razorpay",
                         StringComparison.OrdinalIgnoreCase))
            {
                paymentMethod = "Razorpay";
            }
            else
            {
                paymentMethod = "Online";
            }

            string paymentStatus;
            string orderStatus;
            string remarks;
            DateTime? paymentDate;

            // =====================================================
            // COD
            // =====================================================

            if (paymentMethod == "CashOnDelivery")
            {
                paymentStatus = "Pending";

                orderStatus = "Confirmed";

                paymentDate = null;

                remarks =
                    "Cash on Delivery selected. Payment will be collected at delivery.";
            }
            else
            {
                // =================================================
                // ONLINE / RAZORPAY
                // =================================================

                paymentStatus = "Pending";

                orderStatus = "Pending";

                paymentDate = null;

                remarks =
                    "Online payment initiated. Awaiting payment gateway confirmation.";
            }

            var payment = new Payment
            {
                OrderId = order.OrderId,

                PaymentMethod = paymentMethod,

                TransactionId =
                    GenerateTransactionId(
                        paymentMethod),

                Amount = order.GrandTotal,

                PaymentStatus = paymentStatus,

                PaymentDate = paymentDate,

                Remarks = remarks
            };

            var createdPayment =
                await _paymentRepository.CreateAsync(
                    payment);

            order.OrderStatus = orderStatus;

            order.PaymentStatus = paymentStatus;

            order.UpdatedDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            createdPayment.Order = order;

            return MapToResponse(createdPayment);
        }

        // =========================================================
        // CREATE RAZORPAY ORDER
        // =========================================================

        public async Task<PaymentResponseDto> CreateRazorpayOrderAsync(
            int userId,
            int orderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    userId);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            if (order.OrderStatus != "Pending")
            {
                throw new Exception(
                    "Payment can only be initiated for a pending order.");
            }

            var existingPayment =
                await _paymentRepository.GetByOrderIdAsync(
                    orderId);

            // If Razorpay order already exists,
            // return existing payment information.
            if (existingPayment != null &&
                !string.IsNullOrWhiteSpace(
                    existingPayment.RazorpayOrderId))
            {
                existingPayment.Order = order;

                return MapToResponse(existingPayment);
            }

            // Razorpay amount is required in paise.
            var amountInPaise =
                Convert.ToInt64(
                    Math.Round(
                        order.GrandTotal * 100,
                        0,
                        MidpointRounding.AwayFromZero));

            if (amountInPaise <= 0)
            {
                throw new Exception(
                    "Order amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(
                    _razorpaySettings.KeyId)
                ||
                string.IsNullOrWhiteSpace(
                    _razorpaySettings.KeySecret))
            {
                throw new Exception(
                    "Razorpay API keys are not configured.");
            }

            var client =
                new RazorpayClient(
                    _razorpaySettings.KeyId,
                    _razorpaySettings.KeySecret);

            var options =
                new Dictionary<string, object>
                {
                    {
                        "amount",
                        amountInPaise
                    },
                    {
                        "currency",
                        _razorpaySettings.Currency
                    },
                    {
                        "receipt",
                        order.OrderNumber
                    }
                };

            var razorpayOrder =
                client.Order.Create(options);

            var razorpayOrderId =
                razorpayOrder["id"]?.ToString();

            if (string.IsNullOrWhiteSpace(
                    razorpayOrderId))
            {
                throw new Exception(
                    "Failed to create Razorpay order.");
            }

            Payment payment;

            // =====================================================
            // CREATE NEW PAYMENT
            // =====================================================

            if (existingPayment == null)
            {
                payment = new Payment
                {
                    OrderId = order.OrderId,

                    PaymentMethod = "Razorpay",

                    TransactionId = string.Empty,

                    Amount = order.GrandTotal,

                    PaymentStatus = "Pending",

                    PaymentDate = null,

                    RazorpayOrderId =
                        razorpayOrderId,

                    RazorpayPaymentId = null,

                    RazorpaySignature = null,

                    Remarks =
                        "Razorpay order created. Awaiting payment."
                };

                payment =
                    await _paymentRepository.CreateAsync(
                        payment);
            }
            else
            {
                // =================================================
                // UPDATE EXISTING PAYMENT
                // =================================================

                existingPayment.PaymentMethod =
                    "Razorpay";

                existingPayment.Amount =
                    order.GrandTotal;

                existingPayment.PaymentStatus =
                    "Pending";

                existingPayment.PaymentDate =
                    null;

                existingPayment.RazorpayOrderId =
                    razorpayOrderId;

                existingPayment.RazorpayPaymentId =
                    null;

                existingPayment.RazorpaySignature =
                    null;

                existingPayment.Remarks =
                    "Razorpay order created. Awaiting payment.";

                await _paymentRepository.UpdateAsync(
                    existingPayment);

                payment = existingPayment;
            }

            // Keep order pending until payment succeeds.
            order.PaymentStatus = "Pending";

            order.OrderStatus = "Pending";

            order.UpdatedDate =
                DateTime.UtcNow;

            await _orderRepository.UpdateAsync(
                order);

            payment.Order = order;

            return MapToResponse(payment);
        }

        // =========================================================
        // VERIFY RAZORPAY PAYMENT
        // =========================================================

        public async Task<PaymentResponseDto>
            VerifyRazorpayPaymentAsync(
                int userId,
                PaymentVerifyDto dto)
        {
            var payment =
                await _paymentRepository.GetByIdAsync(
                    dto.PaymentId);

            if (payment == null)
            {
                throw new Exception(
                    "Payment not found.");
            }

            if (payment.Order == null)
            {
                throw new Exception(
                    "Payment order not found.");
            }

            if (payment.Order.UserId != userId)
            {
                throw new Exception(
                    "You are not authorized to verify this payment.");
            }

            if (string.IsNullOrWhiteSpace(
                    payment.RazorpayOrderId))
            {
                throw new Exception(
                    "Razorpay order was not created.");
            }

            if (!string.Equals(
                    payment.RazorpayOrderId,
                    dto.RazorpayOrderId,
                    StringComparison.Ordinal))
            {
                throw new Exception(
                    "Razorpay order ID mismatch.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.RazorpayPaymentId))
            {
                throw new Exception(
                    "Razorpay payment ID is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.RazorpaySignature))
            {
                throw new Exception(
                    "Razorpay signature is required.");
            }

            var options =
                new Dictionary<string, string>
                {
                    {
                        "razorpay_order_id",
                        payment.RazorpayOrderId
                    },
                    {
                        "razorpay_payment_id",
                        dto.RazorpayPaymentId
                    },
                    {
                        "razorpay_signature",
                        dto.RazorpaySignature
                    }
                };

            try
            {
                RazorpayUtils.verifyPaymentSignature(options);
            }
            catch
            {
                payment.PaymentStatus =
                    "Failed";

                payment.Remarks =
                    "Razorpay signature verification failed.";

                await _paymentRepository.UpdateAsync(
                    payment);

                throw new Exception(
                    "Payment verification failed.");
            }

            // =====================================================
            // PAYMENT SUCCESS
            // =====================================================

            payment.RazorpayPaymentId =
                dto.RazorpayPaymentId;

            payment.RazorpaySignature =
                dto.RazorpaySignature;

            payment.TransactionId =
                dto.RazorpayPaymentId;

            payment.PaymentStatus =
                "Paid";

            payment.PaymentDate =
                DateTime.UtcNow;

            payment.Remarks =
                "Razorpay payment verified successfully.";

            payment.Order.PaymentStatus =
                "Paid";

            payment.Order.OrderStatus =
                "Confirmed";

            payment.Order.UpdatedDate =
                DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(
                payment);

            await _orderRepository.UpdateAsync(
                payment.Order);

            return MapToResponse(payment);
        }

        // =========================================================
        // GET PAYMENT BY ORDER
        // =========================================================

        public async Task<PaymentResponseDto?>
            GetPaymentByOrderIdAsync(
                int userId,
                int orderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    userId);

            if (order == null)
            {
                return null;
            }

            var payment =
                await _paymentRepository.GetByOrderIdAsync(
                    orderId);

            if (payment == null)
            {
                return null;
            }

            return MapToResponse(payment);
        }

        // =========================================================
        // GENERATE TRANSACTION ID
        // =========================================================

        private static string GenerateTransactionId(
            string paymentMethod)
        {
            var prefix =
                paymentMethod == "CashOnDelivery"
                    ? "COD"
                    : "PAY";

            return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"
                .Substring(0, 28)
                .ToUpper();
        }

        // =========================================================
        // RESPONSE MAPPING
        // =========================================================

        private static PaymentResponseDto MapToResponse(
            Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId =
                    payment.PaymentId,

                OrderId =
                    payment.OrderId,

                OrderNumber =
                    payment.Order?.OrderNumber
                    ?? string.Empty,

                PaymentMethod =
                    payment.PaymentMethod,

                TransactionId =
                    payment.TransactionId,

                Amount =
                    payment.Amount,

                PaymentStatus =
                    payment.PaymentStatus,

                RazorpayOrderId =
                    payment.RazorpayOrderId,

                RazorpayPaymentId =
                    payment.RazorpayPaymentId,

                PaymentDate =
                    payment.PaymentDate,

                Remarks =
                    payment.Remarks
            };
        }
    }
}