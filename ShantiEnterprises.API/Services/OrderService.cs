using ShantiEnterprises.API.DTOs.Order;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IProductPriceTierRepository _priceTierRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressRepository addressRepository,
            IProductPriceTierRepository priceTierRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _priceTierRepository = priceTierRepository;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(
            int userId,
            CreateOrderDto dto)
        {
            // =========================
            // 1. GET ADDRESS
            // =========================

            var address =
                await _addressRepository.GetByIdAsync(
                    dto.AddressId,
                    userId);

            if (address == null)
            {
                throw new Exception(
                    "Delivery address not found.");
            }

            // =========================
            // 2. GET CART
            // =========================

            var cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart == null ||
                cart.CartItems == null ||
                !cart.CartItems.Any())
            {
                throw new Exception(
                    "Your cart is empty.");
            }

            // =========================
            // 3. CALCULATE ORDER
            // =========================

            decimal subtotal = 0;

            decimal gstAmount = 0;

            var orderItems =
                new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var product = cartItem.Product;

                if (product == null)
                {
                    throw new Exception(
                        $"Product not found for cart item {cartItem.CartItemId}.");
                }

                if (!product.IsActive)
                {
                    throw new Exception(
                        $"Product '{product.ProductName}' is currently inactive.");
                }

                if (cartItem.Quantity <= 0)
                {
                    throw new Exception(
                        $"Invalid quantity for product '{product.ProductName}'.");
                }

                // =========================
                // STOCK VALIDATION
                // =========================

                if (cartItem.Quantity > product.Stock)
                {
                    throw new Exception(
                        $"Only {product.Stock} units of '{product.ProductName}' are available.");
                }

                // =========================
                // WHOLESALE PRICE
                // =========================

                var unitPrice =
                    await CalculatePriceAsync(
                        product,
                        cartItem.Quantity);

                var totalPrice =
                    unitPrice * cartItem.Quantity;

                // =========================
                // GST
                // =========================

                var itemGst =
                    totalPrice *
                    product.GSTPercentage /
                    100;

                subtotal += totalPrice;

                gstAmount += itemGst;

                // =========================
                // ORDER ITEM
                // =========================

                orderItems.Add(new OrderItem
                {
                    ProductId = product.ProductId,

                    ProductName =
                        product.ProductName,

                    SKU =
                        product.SKU,

                    Quantity =
                        cartItem.Quantity,

                    UnitPrice =
                        unitPrice,

                    GSTPercentage =
                        product.GSTPercentage,

                    GSTAmount =
                        itemGst,

                    TotalPrice =
                        totalPrice
                });
            }

            // =========================
            // 4. SHIPPING
            // =========================

            decimal shippingCharge = 0;

            // =========================
            // 5. GRAND TOTAL
            // =========================

            decimal grandTotal =
                subtotal +
                gstAmount +
                shippingCharge;

            // =========================
            // 6. CREATE ORDER
            // =========================

            var order = new Order
            {
                UserId = userId,

                OrderNumber =
                    GenerateOrderNumber(),

                // Shipping Address Snapshot
                ShippingFullName =
                    address.FullName,

                ShippingMobile =
                    address.Mobile,

                ShippingAddressLine1 =
                    address.AddressLine1,

                ShippingAddressLine2 =
                    address.AddressLine2,

                ShippingCity =
                    address.City,

                ShippingState =
                    address.State,

                ShippingPincode =
                    address.Pincode,

                ShippingCountry =
                    address.Country,

                // Amounts
                Subtotal =
                    subtotal,

                GSTAmount =
                    gstAmount,

                ShippingCharge =
                    shippingCharge,

                GrandTotal =
                    grandTotal,

                // Status
                OrderStatus =
                    "Pending",

                PaymentStatus =
                    "Pending",

                CreatedDate =
                    DateTime.UtcNow,

                OrderItems =
                    orderItems
            };

            // =========================
            // 7. SAVE ORDER
            // =========================

            var createdOrder =
                await _orderRepository.CreateAsync(order);

            // =========================
            // 8. CLEAR CART
            // =========================

            await _cartRepository.ClearAsync(cart);

            // =========================
            // 9. RESPONSE
            // =========================

            return MapToResponse(createdOrder);
        }

        // ==========================================
        // GET MY ORDERS
        // ==========================================

        public async Task<List<OrderResponseDto>>
            GetMyOrdersAsync(int userId)
        {
            var orders =
                await _orderRepository.GetByUserIdAsync(
                    userId);

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        // ==========================================
        // GET MY ORDER BY ID
        // ==========================================

        public async Task<OrderResponseDto?>
            GetMyOrderByIdAsync(
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

            return MapToResponse(order);
        }

        // ==========================================
        // PRICE CALCULATION
        // ==========================================

        private async Task<decimal>
            CalculatePriceAsync(
                Product product,
                int quantity)
        {
            var tiers =
                await _priceTierRepository
                    .GetByProductIdAsync(
                        product.ProductId);

            var tier =
                tiers.FirstOrDefault(x =>
                    quantity >= x.MinQuantity &&
                    (!x.MaxQuantity.HasValue ||
                     quantity <= x.MaxQuantity.Value));

            if (tier != null)
            {
                return tier.Price;
            }

            return product.WholesalePrice;
        }

        // ==========================================
        // ORDER NUMBER
        // ==========================================

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"
                .Substring(0, 24)
                .ToUpper();
        }

        // ==========================================
        // RESPONSE MAPPING
        // ==========================================

        private static OrderResponseDto MapToResponse(
            Order order)
        {
            return new OrderResponseDto
            {
                OrderId =
                    order.OrderId,

                OrderNumber =
                    order.OrderNumber,

                UserId =
                    order.UserId,

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

                Items =
                    order.OrderItems
                        .Select(x => new OrderItemResponseDto
                        {
                            OrderItemId =
                                x.OrderItemId,

                            ProductId =
                                x.ProductId,

                            ProductName =
                                x.ProductName,

                            SKU =
                                x.SKU,

                            Quantity =
                                x.Quantity,

                            UnitPrice =
                                x.UnitPrice,

                            GSTPercentage =
                                x.GSTPercentage,

                            GSTAmount =
                                x.GSTAmount,

                            TotalPrice =
                                x.TotalPrice
                        })
                        .ToList()
            };
        }
    }
}