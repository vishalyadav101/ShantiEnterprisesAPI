using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShantiEnterprises.API.Data;
using System.Text;

using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Services;
using ShantiEnterprises.API.Repositories;
using ShantiEnterprises.API.Settings;
using ShantiEnterprises.API.Models;


var builder = WebApplication.CreateBuilder(args);


// =========================
// RAZORPAY SETTINGS
// =========================

builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("Razorpay"));


// =========================
// EMAIL SETTINGS
// =========================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


// =========================
// AUTH
// =========================

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();


// =========================
// CATEGORY
// =========================

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


// =========================
// PRODUCT
// =========================

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


// =========================
// PRODUCT PRICE TIER
// =========================

builder.Services.AddScoped<IProductPriceTierRepository, ProductPriceTierRepository>();
builder.Services.AddScoped<IProductPriceTierService, ProductPriceTierService>();


// =========================
// PRODUCT IMAGE
// =========================

builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();


// =========================
// HTTP CONTEXT
// =========================

builder.Services.AddHttpContextAccessor();


// =========================
// CART
// =========================

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();


// =========================
// ADDRESS
// =========================

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();


// =========================
// ORDER
// =========================

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


// =========================
// ADMIN ORDER
// =========================

builder.Services.AddScoped<IAdminOrderRepository, AdminOrderRepository>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();


// =========================
// PAYMENT
// =========================

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


// =========================
// NOTIFICATION
// =========================

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();


// =========================
// WEBSITE SETTINGS
// =========================

builder.Services.AddScoped<IWebsiteSettingRepository, WebsiteSettingRepository>();
builder.Services.AddScoped<IWebsiteSettingService, WebsiteSettingService>();


// =========================
// OFFER
// =========================

builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IOfferService, OfferService>();


// =========================
// BANNER
// =========================

builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IBannerService, BannerService>();


// =========================
// BULK ENQUIRY
// =========================

builder.Services.AddScoped<IBulkEnquiryRepository, BulkEnquiryRepository>();
builder.Services.AddScoped<IBulkEnquiryService, BulkEnquiryService>();


// =========================
// INVENTORY
// =========================

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();


// =========================
// COUPON
// =========================

builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();


// =========================
// WISHLIST
// =========================

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistService>();


// =========================
// REVIEW
// =========================

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();


// =========================
// SHIPMENT
// =========================

builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();


// =========================
// RETURN
// =========================

builder.Services.AddScoped<IReturnRepository, ReturnRepository>();
builder.Services.AddScoped<IReturnService, ReturnService>();


// =========================
// REFUND
// =========================

builder.Services.AddScoped<IRefundRepository, RefundRepository>();
builder.Services.AddScoped<IRefundService, RefundService>();


// =========================
// AUDIT LOG
// =========================

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();


// =========================
// DASHBOARD
// =========================

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


// =========================
// ADMIN USER
// =========================

builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();


// =========================
// CONTACT ENQUIRY
// =========================

builder.Services.AddScoped<IContactEnquiryRepository, ContactEnquiryRepository>();
builder.Services.AddScoped<IContactEnquiryService, ContactEnquiryService>();


// =========================
// FAQ
// =========================

builder.Services.AddScoped<IFAQRepository, FAQRepository>();
builder.Services.AddScoped<IFAQService, FAQService>();


// =========================
// EMAIL SERVICE
// =========================

builder.Services.AddScoped<IEmailService, EmailService>();


// =========================
// CONTROLLERS
// =========================

builder.Services.AddControllers();


// =========================
// DATABASE
// =========================

builder.Services.AddDbContext<ShantiEnterprisesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// =========================
// JWT AUTHENTICATION
// =========================

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });


// =========================
// AUTHORIZATION
// =========================

builder.Services.AddAuthorization();


// =========================
// SWAGGER
// =========================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter JWT token as: Bearer {your token}"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});


var app = builder.Build();


// =========================
// SWAGGER
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// =========================
// HTTPS
// =========================

app.UseHttpsRedirection();


// =========================
// STATIC FILES
// =========================

app.UseStaticFiles();


// =========================
// AUTHENTICATION
// =========================

app.UseAuthentication();


// =========================
// AUTHORIZATION
// =========================

app.UseAuthorization();


// =========================
// CONTROLLERS
// =========================

app.MapControllers();


// =========================
// RUN
// =========================

app.Run();