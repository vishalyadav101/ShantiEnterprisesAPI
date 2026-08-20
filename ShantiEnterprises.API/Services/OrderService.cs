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
        private readonly INotificationService _notificationService;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IAuditLogService _auditLogService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAddressRepository addressRepository,
            IProductPriceTierRepository priceTierRepository,
            INotificationService notificationService,
            IInventoryRepository inventoryRepository,
            ICouponRepository couponRepository,
            IAuditLogService auditLogService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _priceTierRepository = priceTierRepository;
            _notificationService = notificationService;
            _inventoryRepository = inventoryRepository;
            _couponRepository = couponRepository;
            _auditLogService = auditLogService;
        }

        // ==========================================
        // CREATE ORDER
        // ==========================================

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
                    ProductId =
                        product.ProductId,

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
            // COUPON
            // =========================

            decimal couponDiscount = 0;

            string? couponCode = null;

            // Keep coupon outside transaction
            // so it can be used later inside transaction
            Coupon? appliedCoupon = null;

            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                couponCode =
                    dto.CouponCode
                        .Trim()
                        .ToUpper();

                appliedCoupon =
                    await _couponRepository.GetByCodeAsync(
                        couponCode);

                if (appliedCoupon == null)
                {
                    throw new Exception(
                        "Invalid coupon code.");
                }

                var now = DateTime.UtcNow;

                if (!appliedCoupon.IsActive)
                {
                    throw new Exception(
                        "This coupon is inactive.");
                }

                if (now < appliedCoupon.StartDate)
                {
                    throw new Exception(
                        "This coupon is not active yet.");
                }

                if (now > appliedCoupon.EndDate)
                {
                    throw new Exception(
                        "This coupon has expired.");
                }

                if (appliedCoupon.UsageLimit.HasValue &&
                    appliedCoupon.UsedCount >=
                    appliedCoupon.UsageLimit.Value)
                {
                    throw new Exception(
                        "This coupon usage limit has been reached.");
                }

                if (appliedCoupon.MinimumOrderAmount.HasValue &&
                    subtotal <
                    appliedCoupon.MinimumOrderAmount.Value)
                {
                    throw new Exception(
                        $"Minimum order amount should be {appliedCoupon.MinimumOrderAmount.Value:0.00}.");
                }

                // =========================
                // CALCULATE DISCOUNT
                // =========================

                if (appliedCoupon.DiscountType == "Percentage")
                {
                    couponDiscount =
                        subtotal *
                        appliedCoupon.DiscountValue /
                        100;

                    // Maximum discount limit
                    if (appliedCoupon.MaximumDiscountAmount.HasValue &&
                        couponDiscount >
                        appliedCoupon.MaximumDiscountAmount.Value)
                    {
                        couponDiscount =
                            appliedCoupon.MaximumDiscountAmount.Value;
                    }
                }
                else if (appliedCoupon.DiscountType == "Fixed")
                {
                    couponDiscount =
                        appliedCoupon.DiscountValue;
                }

                // Discount cannot exceed subtotal
                if (couponDiscount > subtotal)
                {
                    couponDiscount = subtotal;
                }

                couponDiscount =
                    Math.Round(
                        couponDiscount,
                        2);
            }

            // =========================
            // 5. GRAND TOTAL
            // =========================

            decimal grandTotal =
                subtotal +
                gstAmount +
                shippingCharge -
                couponDiscount;

            grandTotal =
                Math.Max(
                    0,
                    Math.Round(
                        grandTotal,
                        2));

            // =========================
            // 6. CREATE ORDER
            // =========================

            var order = new Order
            {
                UserId =
                    userId,

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

                CouponDiscount =
                    couponDiscount,

                CouponCode =
                    couponCode,

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

            // =====================================================
            // 7. ORDER + INVENTORY + COUPON + CART TRANSACTION
            // =====================================================

            Order createdOrder = null!;

            await _orderRepository.ExecuteInTransactionAsync(
                async () =>
                {
                    // =========================
                    // SAVE ORDER
                    // =========================

                    createdOrder =
                        await _orderRepository.CreateAsync(
                            order);

                    // =========================
                    // UPDATE INVENTORY
                    // =========================

                    foreach (var cartItem in cart.CartItems)
                    {
                        var product =
                            await _inventoryRepository
                                .GetProductByIdAsync(
                                    cartItem.ProductId);

                        if (product == null)
                        {
                            throw new Exception(
                                $"Product not found: {cartItem.ProductId}");
                        }

                        // Double-check stock
                        if (product.Stock < cartItem.Quantity)
                        {
                            throw new Exception(
                                $"Insufficient stock for product '{product.ProductName}'. " +
                                $"Available stock: {product.Stock}, " +
                                $"Requested quantity: {cartItem.Quantity}.");
                        }

                        // =========================
                        // DEDUCT STOCK
                        // =========================

                        product.Stock -=
                            cartItem.Quantity;

                        // =========================
                        // INVENTORY TRANSACTION
                        // =========================

                        var transaction =
                            new InventoryTransaction
                            {
                                ProductId =
                                    product.ProductId,

                                Quantity =
                                    -cartItem.Quantity,

                                TransactionType =
                                    "Order",

                                ReferenceId =
                                    createdOrder.OrderId,

                                Remarks =
                                    $"Stock deducted for order {createdOrder.OrderNumber}",

                                CreatedDate =
                                    DateTime.UtcNow
                            };

                        _inventoryRepository.AddTransaction(
                            transaction);
                    }

                    // =========================
                    // SAVE INVENTORY
                    // =========================

                    await _inventoryRepository
                        .SaveChangesAsync();

                    // =========================
                    // UPDATE COUPON USAGE
                    // =========================

                    if (appliedCoupon != null)
                    {
                        appliedCoupon.UsedCount++;

                        _couponRepository.Update(
                            appliedCoupon);
                    }

                    // =========================
                    // CLEAR CART
                    // =========================

                    await _cartRepository.ClearAsync(
                        cart);
                });

            // =====================================================
            // 8. CREATE NOTIFICATION
            // =====================================================

            await _notificationService.CreateAsync(
                userId,
                new DTOs.Notification.CreateNotificationDto
                {
                    Title =
                        "Order Placed Successfully",

                    Message =
                        $"Your order {createdOrder.OrderNumber} has been placed successfully.",

                    Type =
                        "Order",

                    ReferenceType =
                        "Order",

                    ReferenceId =
                        createdOrder.OrderId
                });

            // =====================================================
            // 9. CREATE AUDIT LOG
            // =====================================================

            await _auditLogService.CreateAsync(
                userId,
                null,
                "CREATE",
                "Order",
                createdOrder.OrderId,
                $"Order {createdOrder.OrderNumber} created successfully.",
                null);

            // =========================
            // 10. RESPONSE
            // =========================

            return MapToResponse(
                createdOrder);
        }

        // ==========================================
        // GET MY ORDERS
        // ==========================================

        public async Task<List<OrderResponseDto>>
            GetMyOrdersAsync(
                int userId)
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

            return MapToResponse(
                order);
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

                CouponDiscount =
                    order.CouponDiscount,

                CouponCode =
                    order.CouponCode,

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