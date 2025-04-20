using Microsoft.OpenApi.Models;
using System.Reflection;
using Zoo.Application.Interfaces;
using Zoo.Application.Services;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Repositories;
using Zoo.Infrastructure.Swagger;

namespace Zoo.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
            builder.Services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();
            builder.Services.AddSingleton<IFeedingScheduleRepository, InMemoryFeedingScheduleRepository>();
            
            builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
            
            builder.Services.AddScoped<AnimalTransferService>();
            builder.Services.AddScoped<FeedingOrganizationService>();
            builder.Services.AddScoped<ZooStatisticsService>();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HSE Zoo API",
                    Version = "v1"
                });
                
                c.EnableAnnotations();
                
                c.SchemaFilter<EnumSchemaFilter>();
                c.SchemaFilter<ValueObjectSchemaFilter>();
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
            
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HSE Zoo API");
                    c.RoutePrefix = string.Empty;
                });
            }
            
            app.UseCors("AllowAll");
            
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred", detail = ex.Message });
                }
            });
            
            app.UseAuthorization();
            
            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}