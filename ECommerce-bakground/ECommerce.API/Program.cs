using ECommerce.Application.Services;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

        // Add Payment and Inventory Services
        builder.Services.AddScoped<IPaymentService, DefaultPaymentService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();

        // Add Outbox and Event Services
        builder.Services.AddScoped<IOutboxService, OutboxService>();
        builder.Services.AddScoped<IEventPublisher, EventPublisher>();
        builder.Services.AddHostedService<OutboxProcessorService>();

        // Add Business Services
        builder.Services.AddScoped<IEmailService, DefaultEmailService>();
        builder.Services.AddScoped<ICacheService, DefaultCacheService>();
        builder.Services.AddScoped<IStatisticsService, DefaultStatisticsService>();
        builder.Services.AddScoped<INotificationService, DefaultNotificationService>();

        // Add Event Handler Factory
        builder.Services.AddScoped<IEventHandlerFactory, EventHandlerFactory>();

        // Add Event Handlers
        builder.Services.AddScoped<OrderCreatedEventHandler>();
        builder.Services.AddScoped<OrderStatusChangedEventHandler>();
        builder.Services.AddScoped<OrderCancelledEventHandler>();
        builder.Services.AddScoped<InventoryUpdatedEventHandler>();
        builder.Services.AddScoped<PaymentProcessedEventHandler>();
        builder.Services.AddScoped<UserRegisteredEventHandler>();

// Add Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();