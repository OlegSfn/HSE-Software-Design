using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Repositories;
using PaymentsService.Services;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Infrastructure.Outbox;

namespace PaymentsService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<PaymentDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("PaymentsDb")
                ?? "Host=payments-db;Port=5432;Database=paymentsdb;Username=postgres;Password=postgres";
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IOutboxDbContext, PaymentDbContext>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
        builder.Services.AddScoped<IInboxRepository, InboxRepository>();

        builder.Services.AddSingleton<IMessageBroker>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RabbitMqMessageBroker>>();
            var hostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            return new RabbitMqMessageBroker(logger, hostName);
        });

        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();

        builder.Services.AddHostedService<PaymentRequestConsumerService>();
        builder.Services.AddHostedService<OutboxProcessorService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            dbContext.Database.EnsureCreated();
            app.Logger.LogInformation("Database schema created");
        }

        app.Run();
    }
}