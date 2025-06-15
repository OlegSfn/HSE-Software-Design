using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrdersService.Data;
using OrdersService.Repositories;
using OrdersService.Services;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Infrastructure.Outbox;

namespace OrdersService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders Service API" });

            c.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
            {
                Description = "User ID for testing. Enter a string value to set X-User-Id",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-User-Id",
                Scheme = "ApiKeyScheme"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "UserId"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddDbContext<OrderDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("OrdersDb")
                ?? "Host=orders-db;Port=5432;Database=ordersdb;Username=postgres;Password=postgres";
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOutboxDbContext, OrderDbContext>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

        builder.Services.AddSingleton<IMessageBroker>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RabbitMqMessageBroker>>();
            var hostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            return new RabbitMqMessageBroker(logger, hostName);
        });

        builder.Services.AddScoped<IOrderService, OrderService>();

        builder.Services.AddHostedService<OutboxProcessorService>();
        builder.Services.AddHostedService<PaymentStatusConsumerService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            app.Logger.LogInformation("Database schema created");
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}