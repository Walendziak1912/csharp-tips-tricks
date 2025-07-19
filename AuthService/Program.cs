using AuthService.Data;
using AuthService.Extensions;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Database Settings using Options Pattern
            builder.Services.Configure<DatabaseSettings>(
                builder.Configuration.GetSection(DatabaseSettings.SectionName));

            // Add Authentication
            builder.Services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme)
                .AddCookie(IdentityConstants.ApplicationScheme);

            // Add Authorization
            builder.Services.AddAuthorization();

            // Configure DBContext with SQL Server
            var databaseSettings = builder.Configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            builder.Services.AddDbContext<AppDbContext>(opt => 
                opt.UseSqlServer(databaseSettings?.ConnectionString ?? 
                    "Server=localhost;Database=AuthServiceDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"));

            // Add IdentityCore
            builder.Services
                .AddIdentityCore<User>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddApiEndpoints();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable Custom Identity APIs
            app.CustomMapIdentityApi<User>();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
