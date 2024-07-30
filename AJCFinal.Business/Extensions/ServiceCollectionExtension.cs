using AJCFinal.Business.Abstractions;
using AJCFinal.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AJCFinal.Business.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }

}
