using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sauberfix.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVariableReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErinnerungVorlaufMinuten",
                table: "Termine",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErinnerungVorlaufMinuten",
                table: "Termine");
        }
    }
}
