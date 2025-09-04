using ECommerce.Application.Services;
using ECommerce.Application.EventHandlers;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using ECommerce.Infrastructure.EventBus;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerce.API.BackgroundServices;
using ECommerce.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
#if DEBUG
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseInMemoryDatabase("ECommerceTestDb"));
#else
Add DbContext
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
       ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
#endif


// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
// Add Core Business Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, DefaultPaymentService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add Infrastructure Services
builder.Services.AddScoped<IEmailService, DefaultEmailService>();
builder.Services.AddScoped<ICacheService, DefaultCacheService>();
builder.Services.AddScoped<INotificationService, DefaultNotificationService>();
builder.Services.AddScoped<IStatisticsService, DefaultStatisticsService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add Event Bus Services
builder.Services.AddRabbitMQEventBus(builder.Configuration);

// Add Event Handlers (简化版本，只依赖Logger)
builder.Services.AddScoped<OrderCreatedEventHandler>();
builder.Services.AddScoped<OrderPaidEventHandler>();
builder.Services.AddScoped<OrderStatusChangedEventHandler>();
builder.Services.AddScoped<OrderCancelledEventHandler>();
builder.Services.AddScoped<InventoryUpdatedEventHandler>();
builder.Services.AddScoped<PaymentProcessedEventHandler>();
builder.Services.AddScoped<UserRegisteredEventHandler>();
builder.Services.AddScoped<PaymentSucceededEventHandler>();
builder.Services.AddScoped<PaymentFailedEventHandler>();
builder.Services.AddScoped<StockLockedEventHandler>();

builder.Services.AddHostedService<EventBusStartupService>();
builder.Services.AddHostedService<OrderExpirationConsumer>();
builder.Services.AddSingleton<IRabbitMqDelayPublisher, RabbitMqDelayPublisher>();
builder.Services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 添加CORS中间件
app.UseCors("AllowFrontend");

// 添加全局异常处理中间件
app.UseGlobalExceptionMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();