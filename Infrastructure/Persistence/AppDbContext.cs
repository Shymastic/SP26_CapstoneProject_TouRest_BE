using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProviderUser> ProviderUsers => Set<ProviderUser>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<PackageService> PackageServices => Set<PackageService>();
        public DbSet<Agency> Agencies => Set<Agency>();
        public DbSet<AgencyUser> AgencyUsers => Set<AgencyUser>();
        public DbSet<Itinerary> Itineraries => Set<Itinerary>();
        public DbSet<ItineraryStop> ItineraryStops => Set<ItineraryStop>();
        public DbSet<ItineraryActivity> ItineraryActivities => Set<ItineraryActivity>();
        public DbSet<ItinerarySchedule> ItinerarySchedules => Set<ItinerarySchedule>();
        public DbSet<ItineraryTracking> ItineraryTrackings => Set<ItineraryTracking>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingItinerary> BookingItineraries => Set<BookingItinerary>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Refund> Refunds => Set<Refund>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
        public DbSet<Payout> Payouts => Set<Payout>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User - Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User - Image relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Image)
                .WithOne(i => i.User)
                .HasForeignKey<User>(u => u.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Booking - User relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Agency - AgencyUser relationship
            modelBuilder.Entity<AgencyUser>()
                .HasOne(au => au.Agency)
                .WithMany()
                .HasForeignKey(au => au.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgencyUser>()
                .HasOne(au => au.User)
                .WithMany()
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Agency - User relationship (CreateByUserId)
            modelBuilder.Entity<Agency>()
            .HasOne(a => a.User)
             .WithMany()
             .HasForeignKey(a => a.CreateByUserId)
            .OnDelete(DeleteBehavior.Restrict);

            // Configure Itinerary - Agency relationship
            modelBuilder.Entity<Itinerary>()
                .HasOne(i => i.Agency)
                .WithMany()
                .HasForeignKey(i => i.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Itinerary>()
                .HasOne(i => i.TourGuide)
                .WithMany()
                .HasForeignKey(i => i.TourGuideId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ItinerarySchedule - Itinerary relationship
            modelBuilder.Entity<ItinerarySchedule>()
                .HasOne(s => s.Itinerary)
                .WithMany()
                .HasForeignKey(s => s.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryTracking - ItinerarySchedule relationship
            modelBuilder.Entity<ItineraryTracking>()
                .HasOne(t => t.ItinerarySchedule)
                .WithMany()
                .HasForeignKey(t => t.ItineraryScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryStop - Itinerary relationship
            modelBuilder.Entity<ItineraryStop>()
                .HasOne(s => s.Itinerary)
                .WithMany(i => i.Stops)
                .HasForeignKey(s => s.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryStop - Provider relationship (nullable)
            modelBuilder.Entity<ItineraryStop>()
                .HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Configure ItineraryActivity - ItineraryStop relationship
            modelBuilder.Entity<ItineraryActivity>()
                .HasOne(a => a.ItineraryStop)
                .WithMany(s => s.Activities)
                .HasForeignKey(a => a.ItineraryStopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryActivity - Service relationship
            modelBuilder.Entity<ItineraryActivity>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BookingItinerary relationships
            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.Booking)
                .WithMany(b => b.BookingItineraries)  // add collection reference
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.ItinerarySchedule)
                .WithMany()
                .HasForeignKey(bi => bi.ItineraryScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.Voucher)
                .WithMany()
                .HasForeignKey(bi => bi.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Service - Provider relationship
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PackageService (many-to-many)
            modelBuilder.Entity<PackageService>()
                .HasKey(ps => new { ps.PackageId, ps.ServiceId });

            modelBuilder.Entity<PackageService>()
                .HasOne(ps => ps.Package)
                .WithMany(p => p.PackageServices)
                .HasForeignKey(ps => ps.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PackageService>()
                .HasOne(ps => ps.Service)
                .WithMany()
                .HasForeignKey(ps => ps.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ProviderUser (many-to-many)
            modelBuilder.Entity<ProviderUser>()
                .HasOne(pu => pu.Provider)
                .WithMany()
                .HasForeignKey(pu => pu.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProviderUser>()
                .HasOne(pu => pu.User)
                .WithMany()
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Provider - User relationship (CreateByUserId)
            modelBuilder.Entity<Provider>()
                .HasOne(p => p.User)
             .WithMany()
             .HasForeignKey(p => p.CreateByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            // Configure Feedback - BookingItinerary relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.BookingItinerary)
                .WithMany()
                .HasForeignKey(f => f.BookingItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Refund)
                .HasForeignKey<Refund>(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Wishlist - User relationship
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Notification - User relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RecipientUser)
                .WithMany()
                .HasForeignKey(n => n.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Report - User relationship
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RefreshToken - User relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure AuditLog - User relationship
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.Actor)
                .WithMany()
                .HasForeignKey(al => al.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Configure AuditLog - Target User relationship
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.TargetUser)
                .WithMany()
                .HasForeignKey(al => al.TargetUserId)
                .OnDelete(DeleteBehavior.SetNull);
            // ============= DECIMAL PRECISION =============

            modelBuilder.Entity<Agency>()
                .Property(a => a.Latitude)
                .HasPrecision(11, 8);

            modelBuilder.Entity<Agency>()
                .Property(a => a.Longitude)
                .HasPrecision(11, 8);

            modelBuilder.Entity<Provider>()
                .Property(p => p.Latitude)
                .HasPrecision(11, 8);

            modelBuilder.Entity<Provider>()
                .Property(p => p.Longitude)
                .HasPrecision(11, 8);

            // Payment - Booking (one booking, many payments)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict); // don't delete payments if booking deleted
                                                    // Wallet - User
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Wallet - Agency
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.Agency)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Wallet - Provider
            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.Provider)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // WalletTransaction - Wallet
            modelBuilder.Entity<WalletTransaction>()
                .HasOne(wt => wt.Wallet)
                .WithMany()
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payout - Wallet
            modelBuilder.Entity<Payout>()
                .HasOne(p => p.Wallet)
                .WithMany()
                .HasForeignKey(p => p.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Agency)
                .WithMany()
                .HasForeignKey(v => v.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItineraryStop>()
                .HasOne(s => s.Vehicle)
                .WithMany()
                .HasForeignKey(s => s.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Feedback>()
                  .HasOne(f => f.RepliedBy)
                 .WithMany()
                .HasForeignKey(f => f.RepliedByUserId)
                 .OnDelete(DeleteBehavior.SetNull);

            // ============= UNIQUE CONSTRAINTS =============

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.BookingId);
            // Unique constraint on OrderCode
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.OrderCode)
                .IsUnique();
            // Unique constraint for Role Code
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Code)
                .IsUnique();

            // Unique constraint for User Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Unique constraint for Voucher Code
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();

            // Unique constraint for Package Code
            modelBuilder.Entity<Package>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Unique constraint for Booking Code
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Code)
                .IsUnique();

            // Unique constraint for Provider ContactEmail
            modelBuilder.Entity<Provider>()
                .HasIndex(p => p.ContactEmail)
                .IsUnique();

            // Unique constraint for Wishlist (ItemId, UserId)
            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.ItemId, w.UserId })
                .IsUnique();

            // Unique constraint for BookingItinerary (BookingId, ItineraryScheduleId)
            modelBuilder.Entity<BookingItinerary>()
                .HasIndex(bi => new { bi.BookingId, bi.ItineraryScheduleId })
                .IsUnique();

            // Unique constraint for PackageService (PackageId, SortOrder)
            modelBuilder.Entity<PackageService>()
                .HasIndex(ps => new { ps.PackageId, ps.SortOrder })
                .IsUnique();

            // Unique constraint for ItineraryStop (ItineraryId, StopOrder)
            modelBuilder.Entity<ItineraryStop>()
                .HasIndex(ist => new { ist.ItineraryId, ist.StopOrder })
                .IsUnique();

            // Unique constraint for ItineraryActivity (ItineraryStopId, ActivityOrder)
            modelBuilder.Entity<ItineraryActivity>()
                .HasIndex(ia => new { ia.ItineraryStopId, ia.ActivityOrder })
                .IsUnique();
            // Only one wallet per user/agency/provider
            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.UserId)
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.AgencyId)
                .IsUnique()
                .HasFilter("[AgencyId] IS NOT NULL");

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.ProviderId)
                .IsUnique()
                .HasFilter("[ProviderId] IS NOT NULL");
            modelBuilder.Entity<Itinerary>()
                .HasIndex(i => i.AgencyId);

            modelBuilder.Entity<ItineraryStop>()
                .HasIndex(s => s.VehicleId);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.UserId);
            // ============= SEED DATA =============

            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "CUSTOMER",
                    Name = "Khách hàng",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "ADMIN",
                    Name = "Quản trị viên",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = "PROVIDER",
                    Name = "Nhà cung cấp dịch vụ",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Code = "AGENCY",
                    Name = "Đại lý du lịch",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
