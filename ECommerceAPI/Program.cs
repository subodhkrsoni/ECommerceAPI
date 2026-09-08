using ECommerceAPI.Data;
using ECommerceAPI.Middleware;
using ECommerceAPI.Repositories;
using ECommerceAPI.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// Add Controllers
// ==========================================

builder.Services.AddControllers();


// ==========================================
// JWT Authentication
// ==========================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });


// ==========================================
// Authorization
// ==========================================

builder.Services.AddAuthorization();


// ==========================================
// Swagger
// ==========================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token"
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});


// ==========================================
// Database
// ==========================================

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection")
        );
    });


// ==========================================
// Repository + Service DI
// ==========================================


// ==========================================
// Product
// ==========================================

builder.Services.AddScoped<
    IProductRepository,
    ProductRepository>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();


// ==========================================
// Cart
// ==========================================

builder.Services.AddScoped<
    ICartRepository,
    CartRepository>();

builder.Services.AddScoped<
    ICartService,
    CartService>();


// ==========================================
// Checkout
// ==========================================

builder.Services.AddScoped<
    ICheckoutService,
    CheckoutService>();


// ==========================================
// Coupon
// ==========================================

builder.Services.AddScoped<
    ICouponRepository,
    CouponRepository>();

builder.Services.AddScoped<
    ICouponService,
    CouponService>();

// ==========================================
// Payment
// ==========================================

builder.Services.AddScoped<
    IPaymentRepository,
    PaymentRepository>();

builder.Services.AddScoped<
    IPaymentService,
    PaymentService>();

// =========================
// Admin Dashboard
// =========================

builder.Services.AddScoped<
    IAdminDashboardService,
    AdminDashboardService>();


// ==========================================
// Build Application
// ==========================================

var app = builder.Build();


// ==========================================
// Global Exception Middleware
// ==========================================

app.UseMiddleware<ExceptionMiddleware>();


// ==========================================
// Swagger
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ==========================================
// HTTPS Redirection
// ==========================================

app.UseHttpsRedirection();


// ==========================================
// Authentication
// IMPORTANT:
// Authentication must come
// before Authorization
// ==========================================

app.UseAuthentication();


// ==========================================
// Authorization
// ==========================================

app.UseAuthorization();


// ==========================================
// Controllers
// ==========================================

app.MapControllers();


// ==========================================
// Run Application
// ==========================================

app.Run();