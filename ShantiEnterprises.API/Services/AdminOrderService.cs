using ShantiEnterprises.API.DTOs.AdminOrder;
using ShantiEnterprises.API.DTOs.Notification;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IAdminOrderRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IPaymentRepository _paymentRepository;

        public AdminOrderService(
            IAdminOrderRepository repository,
            INotificationService notificationService,
            IPaymentRepository paymentRepository)
        {
            _repository = repository;
            _notificationService = notificationService;
            _paymentRepository = paymentRepository;
        }

        // =========================
        // GET ALL ORDERS
        // =========================

        public async Task<List<AdminOrderResponseDto>>
            GetAllAsync()
        {
            var orders =
                await _repository.GetAllAsync();

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        // =========================
        // GET ORDER BY ID
        // =========================

        public async Task<AdminOrderResponseDto?>
            GetByIdAsync(int orderId)
        {
            var order =
                await _repository.GetByIdAsync(
                    orderId);

            if (order == null)
            {
                return null;
            }

            return MapToResponse(order);
        }

        // =========================
        // UPDATE ORDER STATUS
        // =========================

        public async Task<AdminOrderResponseDto>
            UpdateOrderStatusAsync(
                int orderId,
                UpdateOrderStatusDto dto)
        {
            var order =
                await _repository.GetByIdAsync(
                    orderId);

            if (order == null)
            {
                throw new Exception(
                    "Order not found.");
            }

            var status =
                dto.OrderStatus.Trim();

            // Validate status
            var validStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            if (!validStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid order status. " +
                    "Allowed values: Pending, Confirmed, Processing, Shipped, Delivered, Cancelled.");
            }

            // Normalize status
            status =
                validStatuses.First(x =>
                    x.Equals(
                        status,
                        StringComparison.OrdinalIgnoreCase));

            // Prevent changing delivered order
            if (order.OrderStatus == "Delivered" &&
                status != "Delivered")
            {
                throw new Exception(
                    "A delivered order cannot be changed.");
            }

            // Prevent changing cancelled order
            if (order.OrderStatus == "Cancelled" &&
                status != "Cancelled")
            {
                throw new Exception(
                    "A cancelled order cannot be changed.");
            }

            // Update
            order.OrderStatus = status;

            order.UpdatedDate =
                DateTime.UtcNow;

            await _repository.UpdateAsync(order);

            // =========================
            // CREATE NOTIFICATION
            // =========================

            await _notificationService.CreateAsync(
                order.UserId,
                new CreateNotificationDto
                {
                    Title = "Order Status Updated",

                    Message =
                        $"Your order {order.OrderNumber} status has been updated to {order.OrderStatus}.",

                    Type = "Order",

                    ReferenceType = "Order",

                    ReferenceId = order.OrderId
                });

            return MapToResponse(order);
        }

        // =========================
        // UPDATE PAYMENT STATUS
        // =========================

        public async Task<AdminOrderResponseDto>
            UpdatePaymentStatusAsync(
                int orderId,
                UpdatePaymentStatusDto dto)
        {
            var order =
                await _repository.GetByIdAsync(
                    orderId);

            if (order == null)
            {
                throw new Exception(
                    "Order not found.");
            }

            var paymentStatus =
                dto.PaymentStatus.Trim();

            // =========================
            // VALIDATE PAYMENT STATUS
            // =========================

            var validStatuses = new[]
            {
        "Pending",
        "Paid",
        "Failed",
        "Refunded"
    };

            if (!validStatuses.Contains(
                    paymentStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid payment status. " +
                    "Allowed values: Pending, Paid, Failed, Refunded.");
            }

            // =========================
            // NORMALIZE
            // =========================

            paymentStatus =
                validStatuses.First(x =>
                    x.Equals(
                        paymentStatus,
                        StringComparison.OrdinalIgnoreCase));

            // =========================
            // REFUNDED VALIDATION
            // =========================

            if (paymentStatus == "Refunded")
            {
                if (order.OrderStatus != "Cancelled")
                {
                    throw new Exception(
                        "Payment can be refunded only after order cancellation.");
                }
            }

            // =========================
            // PAYMENT RECORD
            // =========================

            var payment =
                await _paymentRepository
                    .GetByOrderIdAsync(orderId);

            // =========================
            // CREATE PAYMENT RECORD
            // IF PAYMENT DOES NOT EXIST
            // =========================

            if (payment == null)
            {
                payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = "CashOnDelivery",
                    TransactionId =
                        $"ADMIN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"
                            .Substring(0, 28)
                            .ToUpper(),
                    Amount = order.GrandTotal,
                    PaymentStatus = paymentStatus,
                    PaymentDate =
                        paymentStatus == "Paid"
                            ? DateTime.UtcNow
                            : null,
                    Remarks =
                        "Payment status updated by Admin."
                };

                payment =
                    await _paymentRepository
                        .CreateAsync(payment);
            }
            else
            {
                // =========================
                // UPDATE EXISTING PAYMENT
                // =========================

                payment.PaymentStatus =
                    paymentStatus;

                payment.PaymentDate =
                    paymentStatus == "Paid"
                        ? DateTime.UtcNow
                        : payment.PaymentDate;

                await _paymentRepository
                    .UpdateAsync(payment);
            }

            // =========================
            // UPDATE ORDER
            // =========================

            order.PaymentStatus =
                paymentStatus;

            order.UpdatedDate =
                DateTime.UtcNow;

            await _repository.UpdateAsync(order);

            // =========================
            // CREATE NOTIFICATION
            // =========================

            await _notificationService.CreateAsync(
                order.UserId,
                new CreateNotificationDto
                {
                    Title = "Payment Status Updated",

                    Message =
                        $"Payment status for order {order.OrderNumber} has been updated to {order.PaymentStatus}.",

                    Type = "Payment",

                    ReferenceType = "Order",

                    ReferenceId = order.OrderId
                });

            return MapToResponse(order);
        }

        // =========================
        // MAP ORDER
        // =========================

        private static AdminOrderResponseDto
            MapToResponse(Order order)
        {
            return new AdminOrderResponseDto
            {
                OrderId =
                    order.OrderId,

                OrderNumber =
                    order.OrderNumber,

                UserId =
                    order.UserId,

                CustomerName =
                    order.User?.FullName
                    ?? string.Empty,

                CustomerEmail =
                    order.User?.Email
                    ?? string.Empty,

                ShippingFullName =
                    order.ShippingFullName,

                ShippingMobile =
                    order.ShippingMobile,

                ShippingAddressLine1 =
                    order.ShippingAddressLine1,

                ShippingAddressLine2 =
                    order.ShippingAddressLine2,

                ShippingCity =
                    order.ShippingCity,

                ShippingState =
                    order.ShippingState,

                ShippingPincode =
                    order.ShippingPincode,

                ShippingCountry =
                    order.ShippingCountry,

                Subtotal =
                    order.Subtotal,

                GSTAmount =
                    order.GSTAmount,

                ShippingCharge =
                    order.ShippingCharge,

                GrandTotal =
                    order.GrandTotal,

                OrderStatus =
                    order.OrderStatus,

                PaymentStatus =
                    order.PaymentStatus,

                CreatedDate =
                    order.CreatedDate,

                UpdatedDate =
                    order.UpdatedDate,

                Items =
                    order.OrderItems
                        .Select(MapItem)
                        .ToList()
            };
        }

        // =========================
        // MAP ORDER ITEM
        // =========================

        private static AdminOrderItemResponseDto
            MapItem(OrderItem item)
        {
            return new AdminOrderItemResponseDto
            {
                OrderItemId =
                    item.OrderItemId,

                ProductId =
                    item.ProductId,

                ProductName =
                    item.ProductName,

                SKU =
                    item.SKU,

                Quantity =
                    item.Quantity,

                UnitPrice =
                    item.UnitPrice,

                GSTPercentage =
                    item.GSTPercentage,

                GSTAmount =
                    item.GSTAmount,

                TotalPrice =
                    item.TotalPrice
            };
        }
    }
}