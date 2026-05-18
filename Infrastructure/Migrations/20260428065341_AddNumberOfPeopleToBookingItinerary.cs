using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfPeopleToBookingItinerary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPeople",
                table: "booking_itineraries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPeople",
                table: "booking_itineraries");
        }
    }
}
