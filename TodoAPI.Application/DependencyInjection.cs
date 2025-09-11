using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TodoAPI.Application.Mapping;

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

            return services;
        }
    }
}