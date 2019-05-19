using Microsoft.EntityFrameworkCore.Migrations;

namespace DiceApi.Migrations
{
    public partial class AddedRollModifierMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Modifier",
                table: "Rolls",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modifier",
                table: "Rolls");
        }
    }
}
