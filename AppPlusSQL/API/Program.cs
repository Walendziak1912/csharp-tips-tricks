using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AppPlusSQL.Persistence.Data;
using Microsoft.Extensions.Configuration;
using AppPlusSQL.Infrastructure.Configuration;
using AppPlusSQL.Application.Interfaces;
using AppPlusSQL.Persistence.Repositories;
using AppPlusSQL.API;
using StackExchange.Profiling;

namespace AppPlusSQL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Dodanie usług
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        var dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
                        sqlOptions.CommandTimeout(dbSettings?.CommandTimeout ?? 30);
                    }
                ));

            // Rejestracja repozytoriów
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();

            // Konfiguracja MiniProfiler
            builder.Services.AddMiniProfiler(options => 
            {
                options.RouteBasePath = "/profiler";
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            }).AddEntityFramework();

            var app = builder.Build();

            // Konfiguracja pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiniProfiler();
            app.UseRouting();
            app.MapControllers();

            // Mapowanie endpointów API
            app.MapApiEndpoints();

            // Inicjalizacja bazy danych
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(context);
            }

            Console.WriteLine("API uruchomione na:");
            Console.WriteLine("- https://localhost:5001/swagger - Swagger UI");
            Console.WriteLine("- https://localhost:5001/api/members/last-activities - Ostatnie aktywności");
            Console.WriteLine("- https://localhost:5001/profiler/results-index - MiniProfiler");

            app.Run();
        }
    }
}
