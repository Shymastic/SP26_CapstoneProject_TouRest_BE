using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixModelSync_20260501 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NumberOfGuests and SpotLeft already exist in DB from prior manual changes
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
