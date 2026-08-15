 using ShantiEnterprises.API.DTOs.Cart;
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

        // =========================
        // GET CART
        // =========================

        public async Task<CartResponseDto> GetCartAsync(
            int userId)
        {
            var cart =
                await GetOrCreateCartAsync(userId);

            return await BuildCartResponse(cart);
        }

        // =========================
        // ADD TO CART
        // =========================

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

            int finalQuantity = dto.Quantity;

            if (existingItem != null)
            {
                finalQuantity =
                    existingItem.Quantity + dto.Quantity;

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
            else
            {
                var unitPrice =
                    await CalculatePriceAsync(
                        product,
                        finalQuantity);

                var cartItem = new CartItem
                {
                    CartId = cart.CartId,

                    ProductId = product.ProductId,

                    Quantity = finalQuantity,

                    UnitPrice = unitPrice,

                    TotalPrice =
                        unitPrice * finalQuantity
                };

                await _cartRepository.AddItemAsync(
                    cartItem);
            }

            cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart == null)
            {
                throw new Exception(
                    "Unable to load cart.");
            }

            return await BuildCartResponse(cart);
        }

        // =========================
        // UPDATE CART ITEM
        // =========================

        public async Task<CartResponseDto>
            UpdateCartItemAsync(
                int userId,
                int cartItemId,
                UpdateCartItemDto dto)
        {
            var cart =
                await GetOrCreateCartAsync(userId);

            var item =
                await _cartRepository.GetItemByIdAsync(
                    cartItemId);

            if (item == null ||
                item.CartId != cart.CartId)
            {
                throw new Exception(
                    "Cart item not found.");
            }

            // Product must exist
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

            item.Quantity =
                dto.Quantity;

            item.UnitPrice =
                await CalculatePriceAsync(
                    item.Product,
                    dto.Quantity);

            item.TotalPrice =
                item.UnitPrice *
                dto.Quantity;

            await _cartRepository.UpdateItemAsync(
                item);

            cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart == null)
            {
                throw new Exception(
                    "Unable to load cart.");
            }

            return await BuildCartResponse(cart);
        }

        // =========================
        // REMOVE CART ITEM
        // =========================

        public async Task<bool> RemoveCartItemAsync(
            int userId,
            int cartItemId)
        {
            var cart =
                await GetOrCreateCartAsync(userId);

            var item =
                await _cartRepository.GetItemByIdAsync(
                    cartItemId);

            if (item == null ||
                item.CartId != cart.CartId)
            {
                return false;
            }

            await _cartRepository.RemoveItemAsync(item);

            return true;
        }

        // =========================
        // CLEAR CART
        // =========================

        public async Task ClearCartAsync(
            int userId)
        {
            var cart =
                await GetOrCreateCartAsync(userId);

            await _cartRepository.ClearAsync(cart);
        }

        // =========================
        // GET OR CREATE CART
        // =========================

        private async Task<Cart> GetOrCreateCartAsync(
            int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(
                    userId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart
            {
                UserId = userId
            };

            return await _cartRepository.CreateAsync(
                cart);
        }

        // =========================
        // CALCULATE PRICE
        // =========================

        private async Task<decimal> CalculatePriceAsync(
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
                    (
                        !x.MaxQuantity.HasValue ||
                        quantity <= x.MaxQuantity.Value
                    ));

            if (tier != null)
            {
                return tier.Price;
            }

            return product.WholesalePrice;
        }

        // =========================
        // BUILD CART RESPONSE
        // =========================

        private async Task<CartResponseDto>
            BuildCartResponse(Cart cart)
        {
            var items =
                new List<CartItemResponseDto>();

            decimal subtotal = 0;

            decimal gstAmount = 0;

            foreach (var item in cart.CartItems)
            {
                // Product must exist
                if (item.Product == null)
                {
                    throw new Exception(
                        $"Product not found for cart item {item.CartItemId}.");
                }

                var product =
                    item.Product;

                var unitPrice =
                    await CalculatePriceAsync(
                        product,
                        item.Quantity);

                var totalPrice =
                    unitPrice * item.Quantity;

                var itemGst =
                    totalPrice *
                    product.GSTPercentage /
                    100;

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

                        UnitPrice =
                            unitPrice,

                        TotalPrice =
                            totalPrice,

                        GSTPercentage =
                            product.GSTPercentage,

                        GSTAmount =
                            itemGst
                    });

                subtotal += totalPrice;

                gstAmount += itemGst;
            }

            return new CartResponseDto
            {
                CartId =
                    cart.CartId,

                UserId =
                    cart.UserId,

                Items =
                    items,

                Subtotal =
                    subtotal,

                GSTAmount =
                    gstAmount,

                GrandTotal =
                    subtotal + gstAmount,

                TotalItems =
                    items.Sum(x => x.Quantity)
            };
        }
    }
}   