using ShantiEnterprises.API.DTOs.Payment;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        // =========================
        // CREATE PAYMENT
        // =========================

        public async Task<PaymentResponseDto> CreatePaymentAsync(
            int userId,
            CreatePaymentDto dto)
        {
            // Get order
            var order =
                await _orderRepository.GetByIdAsync(
                    dto.OrderId,
                    userId);

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            // Check existing payment
            var existingPayment =
                await _paymentRepository.GetByOrderIdAsync(
                    dto.OrderId);

            if (existingPayment != null)
            {
                throw new Exception(
                    "Payment already exists for this order.");
            }

            // Validate payment method
            var paymentMethod =
                dto.PaymentMethod.Trim();

            if (!paymentMethod.Equals(
                    "CashOnDelivery",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !paymentMethod.Equals(
                    "Online",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid payment method. Use CashOnDelivery or Online.");
            }

            // Normalize payment method
            paymentMethod =
                paymentMethod.Equals(
                    "CashOnDelivery",
                    StringComparison.OrdinalIgnoreCase)
                    ? "CashOnDelivery"
                    : "Online";

            string paymentStatus;
            string orderStatus;
            string remarks;

            DateTime? paymentDate;

            // =========================
            // COD
            // =========================

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
                // =========================
                // ONLINE
                // =========================

                paymentStatus = "Pending";

                orderStatus = "Pending";

                paymentDate = null;

                remarks =
                    "Online payment initiated. Awaiting payment gateway confirmation.";
            }

            // =========================
            // CREATE PAYMENT
            // =========================

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

            // =========================
            // UPDATE ORDER
            // =========================

            order.OrderStatus = orderStatus;

            order.PaymentStatus = paymentStatus;

            order.UpdatedDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            // Attach order for response
            createdPayment.Order = order;

            return MapToResponse(createdPayment);
        }

        // =========================
        // GET PAYMENT BY ORDER
        // =========================

        public async Task<PaymentResponseDto?>
            GetPaymentByOrderIdAsync(
                int userId,
                int orderId)
        {
            // First verify that order belongs to user
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

        // =========================
        // GENERATE TRANSACTION ID
        // =========================

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

        // =========================
        // RESPONSE MAPPING
        // =========================

        private static PaymentResponseDto MapToResponse(
            Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,

                OrderId = payment.OrderId,

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

                PaymentDate =
                    payment.PaymentDate,

                Remarks =
                    payment.Remarks
            };
        }
    }
}