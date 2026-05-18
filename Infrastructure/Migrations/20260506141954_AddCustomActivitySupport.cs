using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomActivitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                table: "itinerary_activities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "CustomName",
                table: "itinerary_activities",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomName",
                table: "itinerary_activities");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                table: "itinerary_activities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
