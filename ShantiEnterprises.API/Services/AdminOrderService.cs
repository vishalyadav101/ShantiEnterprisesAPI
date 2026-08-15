using ShantiEnterprises.API.DTOs.AdminOrder;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IAdminOrderRepository _repository;

        public AdminOrderService(
            IAdminOrderRepository repository)
        {
            _repository = repository;
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

            // Validate payment status
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

            // Normalize
            paymentStatus =
                validStatuses.First(x =>
                    x.Equals(
                        paymentStatus,
                        StringComparison.OrdinalIgnoreCase));

            // =========================
            // PAYMENT STATUS RULES
            // =========================

            if (paymentStatus == "Refunded")
            {
                if (order.OrderStatus != "Cancelled")
                {
                    throw new Exception(
                        "Payment can be refunded only after order cancellation.");
                }
            }

            order.PaymentStatus =
                paymentStatus;

            order.UpdatedDate =
                DateTime.UtcNow;

            await _repository.UpdateAsync(order);

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