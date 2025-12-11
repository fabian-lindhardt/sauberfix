using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sauberfix.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEndzeitToTermin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Endzeit",
                table: "Termine",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Update existing records to set Endzeit = DatumUhrzeit + 60 minutes
            migrationBuilder.Sql(
                @"UPDATE ""Termine""
                  SET ""Endzeit"" = ""DatumUhrzeit"" + INTERVAL '60 minutes'
                  WHERE ""Endzeit"" = '0001-01-01 00:00:00'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endzeit",
                table: "Termine");
        }
    }
}
