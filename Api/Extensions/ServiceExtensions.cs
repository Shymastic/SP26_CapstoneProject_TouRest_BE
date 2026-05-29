using TouRest.Application.Common.Helpers;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Repositories;

namespace TouRest.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            //Add repositories to the DI container, count = 14
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAgencyRepository, AgencyRepository>();
            services.AddScoped<IAgencyUserRepository, AgencyUserRepository>();
            services.AddScoped<IBookingItineraryRepository, BookingItineraryRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IItineraryActivityRepository, ItineraryActivityRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<IItineraryScheduleRepository, ItineraryScheduleRepository>();
            services.AddScoped<IItineraryStopRepository, ItineraryStopRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPackageServiceRepository, PackageServiceRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IProviderUserRepository, ProviderUserRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IPayoutRepository, PayoutRepository>();
            services.AddScoped<IItineraryScheduleRepository, ItineraryScheduleRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IAgencyDashboardRepository, AgencyDashboardRepository>();
            services.AddScoped<IProviderDashboardRepository, ProviderDashboardRepository>();
            services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();


            //Add services to the DI container, count = 15
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAgencyService, AgencyService>();
            services.AddScoped<IAgencyUserService, AgencyUserService>();    
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingItineraryService, BookingItineraryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IItineraryActivityService, ItineraryActivityService>();
            services.AddScoped<IItineraryScheduleService, ItineraryScheduleService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<IItineraryStopService, ItineraryStopService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPackageServiceService, PackageServiceService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IProviderUserService, ProviderUserService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRefundService, RefundService>();
            services.AddScoped<IWalletService,  WalletService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddScoped<IPayoutService, PayoutService>();
            services.AddScoped<IRouteOptimizerService, RouteOptimizerService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAgencyDashboardService, AgencyDashboardService>();
            services.AddScoped<IProviderDashboardService, ProviderDashboardService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            return services;
        }
    }
}
