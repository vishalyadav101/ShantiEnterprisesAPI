using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Data
{
    public class ShantiEnterprisesDbContext : DbContext
    {
        public ShantiEnterprisesDbContext(
            DbContextOptions<ShantiEnterprisesDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ProductPriceTier> ProductPriceTiers { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<BulkEnquiry> BulkEnquiries { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        public DbSet<Banner> Banners { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<WebsiteSetting> WebsiteSettings { get; set; }
        public DbSet<Coupon> Coupons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // USER
            // =========================

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserId);

                entity.Property(x => x.FullName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Mobile)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });


            // =========================
            // CATEGORY
            // =========================

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.CategoryId);

                entity.Property(x => x.CategoryName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(500);
            });


            // =========================
            // PRODUCT
            // =========================

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.ProductId);

                entity.Property(x => x.ProductName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(2000);

                entity.Property(x => x.MRP)
                    .HasPrecision(18, 2);

                entity.Property(x => x.WholesalePrice)
                    .HasPrecision(18, 2);

                entity.Property(x => x.GSTPercentage)
                    .HasPrecision(5, 2);

                entity.Property(x => x.SKU)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(500);

                entity.HasIndex(x => x.SKU)
                    .IsUnique();


                // Category -> Products
                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Products)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // PRODUCT IMAGE
            // =========================

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(x => x.ProductImageId);

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(500)
                    .IsRequired();


                // Product -> ProductImages
                entity.HasOne(x => x.Product)
                    .WithMany(x => x.ProductImages)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // PRODUCT PRICE TIER
            // =========================

            modelBuilder.Entity<ProductPriceTier>(entity =>
            {
                entity.HasKey(x => x.ProductPriceTierId);

                entity.Property(x => x.Price)
                    .HasPrecision(18, 2);


                // Product -> Price Tiers
                entity.HasOne(x => x.Product)
                    .WithMany(x => x.PriceTiers)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // ADDRESS
            // =========================

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(x => x.AddressId);

                entity.Property(x => x.FullName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Mobile)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(x => x.AddressLine1)
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(x => x.AddressLine2)
                    .HasMaxLength(250);

                entity.Property(x => x.City)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.State)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Pincode)
                    .HasMaxLength(10)
                    .IsRequired();


                // User -> Addresses
                entity.HasOne(x => x.User)
                    .WithMany(x => x.Addresses)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // CART
            // =========================

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(x => x.CartId);


                // User -> Carts
                entity.HasOne(x => x.User)
                    .WithMany(x => x.Carts)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // CART ITEM
            // =========================

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(x => x.CartItemId);

                entity.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(x => x.TotalPrice)
                    .HasPrecision(18, 2);


                // Cart -> CartItems
                entity.HasOne(x => x.Cart)
                    .WithMany(x => x.CartItems)
                    .HasForeignKey(x => x.CartId)
                    .OnDelete(DeleteBehavior.Cascade);


                // Product -> CartItems
                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // ORDER
            // =========================

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.OrderId);

                entity.Property(x => x.OrderNumber)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.OrderNumber)
                    .IsUnique();

                // =========================
                // AMOUNTS
                // =========================

                entity.Property(x => x.Subtotal)
                    .HasPrecision(18, 2);

                entity.Property(x => x.GSTAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.ShippingCharge)
                    .HasPrecision(18, 2);

                entity.Property(x => x.CouponDiscount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.GrandTotal)
                    .HasPrecision(18, 2);

                // =========================
                // STATUS
                // =========================

                entity.Property(x => x.OrderStatus)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.PaymentStatus)
                    .HasMaxLength(30)
                    .IsRequired();

                // =========================
                // SHIPPING ADDRESS
                // =========================

                entity.Property(x => x.ShippingFullName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.ShippingMobile)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(x => x.ShippingAddressLine1)
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(x => x.ShippingAddressLine2)
                    .HasMaxLength(250);

                entity.Property(x => x.ShippingCity)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.ShippingState)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.ShippingPincode)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.ShippingCountry)
                    .HasMaxLength(100)
                    .IsRequired();

                // =========================
                // USER -> ORDERS
                // =========================

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Orders)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // ORDER ITEM
            // =========================

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(x => x.OrderItemId);

                entity.Property(x => x.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.SKU)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(x => x.GSTPercentage)
                    .HasPrecision(5, 2);

                entity.Property(x => x.GSTAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.TotalPrice)
                    .HasPrecision(18, 2);

                // Order -> OrderItems
                entity.HasOne(x => x.Order)
                    .WithMany(x => x.OrderItems)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Product -> OrderItems
                entity.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            }); 


            // =========================
            // BULK ENQUIRY
            // =========================

            modelBuilder.Entity<BulkEnquiry>(entity =>
            {
                entity.HasKey(x => x.BulkEnquiryId);

                entity.Property(x => x.CustomerName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Mobile)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Message)
                    .HasMaxLength(1000);

                entity.Property(x => x.Status)
                    .HasMaxLength(30)
                    .IsRequired();


                // User -> Bulk Enquiries
                entity.HasOne(x => x.User)
                    .WithMany(x => x.BulkEnquiries)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);


                // Product -> Bulk Enquiries
                entity.HasOne(x => x.Product)
                    .WithMany(x => x.BulkEnquiries)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            // =========================
            // BANNER
            // =========================

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.HasKey(x => x.BannerId);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Subtitle)
                    .HasMaxLength(500);

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.ButtonText)
                    .HasMaxLength(50);

                entity.Property(x => x.ButtonUrl)
                    .HasMaxLength(500);
            });


            // =========================
            // OFFER
            // =========================

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasKey(x => x.OfferId);

                entity.Property(x => x.OfferName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.DiscountPercentage)
                    .HasPrecision(5, 2);

                entity.Property(x => x.MinimumOrderAmount)
                    .HasPrecision(18, 2);
            });

            // =========================
            // PAYMENT
            // =========================

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(x => x.PaymentId);

                entity.Property(x => x.PaymentMethod)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.TransactionId)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.PaymentStatus)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.Remarks)
                    .HasMaxLength(500);

                entity.HasOne(x => x.Order)
                    .WithMany()
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // =========================
            // NOTIFICATION
            // =========================

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.NotificationId);

                entity.Property(x => x.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Message)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(x => x.Type)
                    .HasMaxLength(50);

                entity.Property(x => x.ReferenceType)
                    .HasMaxLength(50);

                // User -> Notifications
                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // WEBSITE SETTINGS
            // =========================

            modelBuilder.Entity<WebsiteSetting>(entity =>
            {
                entity.HasKey(x => x.WebsiteSettingId);

                entity.Property(x => x.CompanyName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.LogoUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.FaviconUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.Email)
                    .HasMaxLength(150);

                entity.Property(x => x.Phone)
                    .HasMaxLength(20);

                entity.Property(x => x.WhatsAppNumber)
                    .HasMaxLength(20);

                entity.Property(x => x.Address)
                    .HasMaxLength(500);

                entity.Property(x => x.FacebookUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.InstagramUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.TwitterUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.LinkedInUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.YouTubeUrl)
                    .HasMaxLength(500);

                entity.Property(x => x.FooterText)
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.ToTable("InventoryTransaction");

                entity.HasKey(x => x.InventoryTransactionId);

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.InventoryTransactions)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //===========================
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.Property(x => x.DiscountValue)
                    .HasPrecision(18, 2);

                entity.Property(x => x.MinimumOrderAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.MaximumDiscountAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.DiscountType)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(x => x.Code)
                    .IsUnique();
            });
        }
    }
}