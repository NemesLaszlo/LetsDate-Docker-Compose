using LestDate_API.Database;
using LestDate_API.Helpers;
using LestDate_API.Interfaces;
using LestDate_API.MapperProfiles;
using LestDate_API.Repositories;
using LestDate_API.Services;
using LestDate_API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LestDate_API.StartupExtensions
{
    public static  class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (env == "Development")
                {
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                }
                else
                {
                    var server = config.GetSection("AppSettings:DbServer").Value;
                    var port = config.GetSection("AppSettings:DbPort").Value;
                    var user = config.GetSection("AppSettings:DbUser").Value;
                    var password = config.GetSection("AppSettings:Password").Value;
                    var database = config.GetSection("AppSettings:Database").Value;

                    options.UseSqlServer($"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}");
                }
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddScoped<ITokenService, TokenService>(); // http request lifetime - alived (scoped)
            // services.AddScoped<IUserRepository, UserRepository>();
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            // services.AddScoped<ILikesRepository, LikesRepository>();
            // services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>(); // (Our) SignalR Presence Tracker Service
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LestDate_API", Version = "v1" });
            });

            return services;
        }
    }
}
