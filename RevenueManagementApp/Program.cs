using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RevenueManagementApp.Models;
using RevenueManagementApp.Repositories;
using RevenueManagementApp.Services;

namespace RevenueManagementApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<MasterContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5; 
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<MasterContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.ConfigureApplicationCookie(options => {
            options.Cookie.HttpOnly = true; 
            options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 hour session
            options.SlidingExpiration = true;
            
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.Headers.Add("Content-Type", "application/json");
                return context.Response.WriteAsync("{\"error\":\"Not authenticated\"}");
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.Headers.Add("Content-Type", "application/json");
                return context.Response.WriteAsync("{\"error\":\"Access denied\"}");
            };
        });

        builder.Services.AddControllers();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<ISalesRepository, SalesRepository>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ISalesService, SalesService>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
    }
}