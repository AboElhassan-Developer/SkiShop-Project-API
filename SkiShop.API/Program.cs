
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkiShop.API.Middleware;
using SkiShop.API.SignalR;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace SkiShop.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.hhh

            builder.Services.AddControllers();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddCors();

            builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                var connString = builder.Configuration.GetConnectionString("Redis")
                ?? throw new Exception("Connection string 'Redis' not found.");
                var configration = ConfigurationOptions.Parse(connString, true);
                return ConnectionMultiplexer.Connect(configration);
            });
            builder.Services.AddSingleton<ICartService, CartService>();
           
            builder.Services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreContext>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

           

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors(x => x
        .WithOrigins("https://localhost:4200", "http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.MapControllers();

            app.MapGroup("api").MapIdentityApi<AppUser>(); // API/login

            app.MapHub<NotificationHub>("/hub/notifications");
            app.MapFallbackToFile("index.html");
            //builder.Services.AddAuthorization();
            try
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<StoreContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(context,userManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;

            }
            app.Run();
        }
    }
}
