using Microsoft.EntityFrameworkCore.Migrations;

namespace DiceApi.Migrations
{
    public partial class FixedMaxValueMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MavValue",
                table: "Rolls",
                newName: "MaxValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxValue",
                table: "Rolls",
                newName: "MavValue");
        }
    }
}
