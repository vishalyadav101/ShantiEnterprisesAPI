using ShantiEnterprises.API.DTOs.Cart;
using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductPriceTierRepository _priceTierRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IProductPriceTierRepository priceTierRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _priceTierRepository = priceTierRepository;
        }

        // =========================================================
        // GET CART
        // =========================================================

        public async Task<CartResponseDto> GetCartAsync(
            int userId)
        {
            var cart =
                await GetOrCreateCartAsync(userId);

            return await BuildCartResponse(cart);
        }

        // =========================================================
        // ADD TO CART
        // =========================================================

        public async Task<CartResponseDto> AddToCartAsync(
            int userId,
            AddToCartDto dto)
        {
            var product =
                await _productRepository.GetByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            if (!product.IsActive)
            {
                throw new Exception(
                    "This product is currently inactive.");
            }

            if (dto.Quantity <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero.");
            }

            if (dto.Quantity > product.Stock)
            {
                throw new Exception(
                    $"Only {product.Stock} units are available.");
            }

            var cart =
                await GetOrCreateCartAsync(userId);

            var existingItem =
                await _cartRepository.GetItemAsync(
                    cart.CartId,
                    dto.ProductId);

            int finalQuantity =
                dto.Quantity;

            // =====================================================
            // EXISTING ITEM
            // =====================================================

            if (existingItem != null)
            {
                finalQuantity =
                    existingItem.Quantity +
                    dto.Quantity;

                if (finalQuantity > product.Stock)
                {
                    throw new Exception(
                        $"Only {product.Stock} units are available.");
                }

                existingItem.Quantity =
                    finalQuantity;

                existingItem.UnitPrice =
                    await CalculatePriceAsync(
                        product,
                        finalQuantity);

                existingItem.TotalPrice =
                    existingItem.UnitPrice *
                    finalQuantity;

                await _cartRepository.UpdateItemAsync(
                    existingItem);
            }

            // =====================================================
            // NEW ITEM
            // =====================================================

            else
            {
                var unitPrice =
                    await CalculatePriceAsync(
                        product,
                        finalQuantity);

                var cartItem =
                    new CartItem
                    {
                        CartId =
                            cart.CartId,

                        ProductId =
                            product.ProductId,

                        Quantity =
                            finalQuantity,

                        UnitPrice =
                            unitPrice,

                        TotalPrice =
                            unitPrice *
                            finalQuantity
                    };

                await _cartRepository.AddItemAsync(
                    cartItem);
            }

            // =====================================================
            // RELOAD CART
            // =====================================================

            cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart == null)
            {
                throw new Exception(
                    "Unable to load cart.");
            }

            return await BuildCartResponse(
                cart);
        }

        // =========================================================
        // UPDATE CART ITEM
        // =========================================================

        public async Task<CartResponseDto>
            UpdateCartItemAsync(
                int userId,
                int cartItemId,
                UpdateCartItemDto dto)
        {
            var cart =
                await GetOrCreateCartAsync(
                    userId);

            var item =
                await _cartRepository.GetItemByIdAsync(
                    cartItemId);

            if (item == null ||
                item.CartId != cart.CartId)
            {
                throw new Exception(
                    "Cart item not found.");
            }

            if (item.Product == null)
            {
                throw new Exception(
                    "Product associated with this cart item was not found.");
            }

            if (dto.Quantity <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero.");
            }

            if (dto.Quantity > item.Product.Stock)
            {
                throw new Exception(
                    $"Only {item.Product.Stock} units are available.");
            }

            // =====================================================
            // UPDATE QUANTITY
            // =====================================================

            item.Quantity =
                dto.Quantity;

            // =====================================================
            // RECALCULATE UNIT PRICE
            // =====================================================

            item.UnitPrice =
                await CalculatePriceAsync(
                    item.Product,
                    dto.Quantity);

            // =====================================================
            // RECALCULATE TOTAL
            // =====================================================

            item.TotalPrice =
                item.UnitPrice *
                dto.Quantity;

            await _cartRepository.UpdateItemAsync(
                item);

            // =====================================================
            // RELOAD CART
            // =====================================================

            cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart == null)
            {
                throw new Exception(
                    "Unable to load cart.");
            }

            return await BuildCartResponse(
                cart);
        }

        // =========================================================
        // REMOVE CART ITEM
        // =========================================================

        public async Task<bool>
            RemoveCartItemAsync(
                int userId,
                int cartItemId)
        {
            var cart =
                await GetOrCreateCartAsync(
                    userId);

            var item =
                await _cartRepository.GetItemByIdAsync(
                    cartItemId);

            if (item == null ||
                item.CartId != cart.CartId)
            {
                return false;
            }

            await _cartRepository.RemoveItemAsync(
                item);

            return true;
        }

        // =========================================================
        // CLEAR CART
        // =========================================================

        public async Task ClearCartAsync(
            int userId)
        {
            var cart =
                await GetOrCreateCartAsync(
                    userId);

            await _cartRepository.ClearAsync(
                cart);
        }

        // =========================================================
        // GET OR CREATE CART
        // =========================================================

        private async Task<Cart>
            GetOrCreateCartAsync(
                int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart != null)
            {
                return cart;
            }

            cart =
                new Cart
                {
                    UserId =
                        userId
                };

            return await _cartRepository.CreateAsync(
                cart);
        }

        // =========================================================
        // CALCULATE PRICE
        // =========================================================
        //
        // Priority:
        //
        // 1. Matching admin-defined price tier
        // 2. Retail Price
        //
        // WholesalePrice is NOT used as fallback.
        // =========================================================

        private async Task<decimal>
            CalculatePriceAsync(
                Product product,
                int quantity)
        {
            if (quantity <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero.");
            }

            var tiers =
                await _priceTierRepository
                    .GetByProductIdAsync(
                        product.ProductId);

            var tier =
                tiers
                    .OrderByDescending(
                        x => x.MinQuantity)
                    .FirstOrDefault(
                        x =>
                            quantity >=
                                x.MinQuantity
                            &&
                            (
                                !x.MaxQuantity.HasValue
                                ||
                                quantity <=
                                    x.MaxQuantity.Value
                            ));

            if (tier != null)
            {
                return tier.Price;
            }

            // =====================================================
            // NORMAL RETAIL PRICE
            // =====================================================

            return product.RetailPrice;
        }

        // =========================================================
        // CALCULATE SHIPPING
        // =========================================================
        //
        // One shipping charge per product line.
        //
        // Example:
        //
        // Product A → ₹40
        // Product B → ₹60
        //
        // Total Shipping → ₹100
        //
        // Same product quantity does NOT multiply shipping.
        // =========================================================

        private static decimal
            CalculateShippingCharge(
                IEnumerable<CartItem> cartItems)
        {
            return cartItems
                .Where(
                    x => x.Product != null)
                .Sum(
                    x =>
                        Math.Max(
                            0,
                            x.Product!
                                .ShippingCharge));
        }

        // =========================================================
        // BUILD CART RESPONSE
        // =========================================================

        private async Task<CartResponseDto>
            BuildCartResponse(
                Cart cart)
        {
            var items =
                new List<CartItemResponseDto>();

            decimal subtotal = 0;

            decimal gstAmount = 0;

            // =====================================================
            // CART ITEMS
            // =====================================================

            foreach (var item in cart.CartItems)
            {
                if (item.Product == null)
                {
                    throw new Exception(
                        $"Product not found for cart item {item.CartItemId}.");
                }

                var product =
                    item.Product;

                // =================================================
                // CURRENT UNIT PRICE
                // =================================================

                var unitPrice =
                    await CalculatePriceAsync(
                        product,
                        item.Quantity);

                // =================================================
                // LOAD PRICE TIERS
                // =================================================

                var priceTiers =
                    await _priceTierRepository
                        .GetByProductIdAsync(
                            product.ProductId);

                // =================================================
                // ITEM TOTAL
                // =================================================

                var totalPrice =
                    unitPrice *
                    item.Quantity;

                // =================================================
                // GST
                // =================================================

                var itemGst =
                    totalPrice *
                    product.GSTPercentage /
                    100;

                // =================================================
                // ADD RESPONSE
                // =================================================

                items.Add(
                    new CartItemResponseDto
                    {
                        CartItemId =
                            item.CartItemId,

                        ProductId =
                            item.ProductId,

                        ProductName =
                            product.ProductName,

                        ImageUrl =
                            product.ImageUrl,

                        Quantity =
                            item.Quantity,

                        // =========================================
                        // RETAIL PRICE
                        // =========================================

                        RetailPrice =
                            product.RetailPrice,

                        // =========================================
                        // CURRENT APPLIED PRICE
                        // =========================================

                        UnitPrice =
                            unitPrice,

                        TotalPrice =
                            totalPrice,

                        GSTPercentage =
                            product.GSTPercentage,

                        GSTAmount =
                            itemGst,

                        // =========================================
                        // PRICE TIERS
                        // =========================================

                        PriceTiers =
                            priceTiers
                                .OrderBy(
                                    x =>
                                        x.MinQuantity)
                                .Select(
                                    x =>
                                        new ProductPriceTierResponseDto
                                        {
                                            ProductPriceTierId =
                                                x.ProductPriceTierId,

                                            ProductId =
                                                x.ProductId,

                                            MinQuantity =
                                                x.MinQuantity,

                                            MaxQuantity =
                                                x.MaxQuantity,

                                            Price =
                                                x.Price
                                        })
                                .ToList()
                    });

                subtotal +=
                    totalPrice;

                gstAmount +=
                    itemGst;
            }

            // =====================================================
            // SHIPPING
            // =====================================================

            var shippingCharge =
                CalculateShippingCharge(
                    cart.CartItems);

            // =====================================================
            // GRAND TOTAL
            // =====================================================

            var grandTotal =
                subtotal +
                gstAmount +
                shippingCharge;

            // =====================================================
            // RESPONSE
            // =====================================================

            return new CartResponseDto
            {
                CartId =
                    cart.CartId,

                UserId =
                    cart.UserId,

                Items =
                    items,

                Subtotal =
                    Math.Round(
                        subtotal,
                        2),

                GSTAmount =
                    Math.Round(
                        gstAmount,
                        2),

                ShippingCharge =
                    Math.Round(
                        shippingCharge,
                        2),

                GrandTotal =
                    Math.Round(
                        grandTotal,
                        2),

                TotalItems =
                    items.Sum(
                        x =>
                            x.Quantity)
            };
        }
    }
}