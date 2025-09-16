using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TodoAPI.Application.Mapping;
using TodoAPI.Application.Services;
using TodoAPI.Application.Services.Interfaces;

namespace TodoAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}