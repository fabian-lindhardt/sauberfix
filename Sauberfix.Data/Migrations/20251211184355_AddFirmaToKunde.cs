using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sauberfix.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFirmaToKunde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Firma",
                table: "Kunden",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firma",
                table: "Kunden");
        }
    }
}
