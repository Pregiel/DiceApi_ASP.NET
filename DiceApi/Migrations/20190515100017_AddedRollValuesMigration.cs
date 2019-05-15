using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiceApi.Migrations
{
    public partial class AddedRollValuesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "Rolls");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Rolls");

            migrationBuilder.CreateTable(
                name: "RollValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaxValue = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    UserRoomId = table.Column<int>(nullable: false),
                    UserRoomUserId = table.Column<int>(nullable: true),
                    UserRoomRoomId = table.Column<int>(nullable: true),
                    RollId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RollValue_Rolls_RollId",
                        column: x => x.RollId,
                        principalTable: "Rolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollValue_UserRooms_UserRoomUserId_UserRoomRoomId",
                        columns: x => new { x.UserRoomUserId, x.UserRoomRoomId },
                        principalTable: "UserRooms",
                        principalColumns: new[] { "UserId", "RoomId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RollValue_RollId",
                table: "RollValue",
                column: "RollId");

            migrationBuilder.CreateIndex(
                name: "IX_RollValue_UserRoomUserId_UserRoomRoomId",
                table: "RollValue",
                columns: new[] { "UserRoomUserId", "UserRoomRoomId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RollValue");

            migrationBuilder.AddColumn<int>(
                name: "MaxValue",
                table: "Rolls",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "Rolls",
                nullable: false,
                defaultValue: 0);
        }
    }
}
