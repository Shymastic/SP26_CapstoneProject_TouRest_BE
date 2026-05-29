using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using TouRest.Infrastructure.Repositories;
using TouRest.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace TouRest.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IPackageServiceRepository, PackageServiceRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IProviderUserRepository, ProviderUserRepository>();
            services.AddScoped<IAgencyRepository, AgencyRepository>();
            services.AddScoped<IAgencyUserRepository, AgencyUserRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IBookingPassengerRepository, BookingPassengerRepository>();

            // Register Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStorageService, StorageService>();

            return services;
        }
    }
}
