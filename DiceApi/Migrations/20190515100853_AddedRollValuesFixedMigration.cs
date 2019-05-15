using Microsoft.EntityFrameworkCore.Migrations;

namespace DiceApi.Migrations
{
    public partial class AddedRollValuesFixedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RollValue_Rolls_RollId",
                table: "RollValue");

            migrationBuilder.DropForeignKey(
                name: "FK_RollValue_UserRooms_UserRoomUserId_UserRoomRoomId",
                table: "RollValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RollValue",
                table: "RollValue");

            migrationBuilder.DropIndex(
                name: "IX_RollValue_UserRoomUserId_UserRoomRoomId",
                table: "RollValue");

            migrationBuilder.DropColumn(
                name: "UserRoomId",
                table: "RollValue");

            migrationBuilder.DropColumn(
                name: "UserRoomRoomId",
                table: "RollValue");

            migrationBuilder.DropColumn(
                name: "UserRoomUserId",
                table: "RollValue");

            migrationBuilder.RenameTable(
                name: "RollValue",
                newName: "RollValues");

            migrationBuilder.RenameIndex(
                name: "IX_RollValue_RollId",
                table: "RollValues",
                newName: "IX_RollValues_RollId");

            migrationBuilder.AlterColumn<int>(
                name: "RollId",
                table: "RollValues",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RollValues",
                table: "RollValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RollValues_Rolls_RollId",
                table: "RollValues",
                column: "RollId",
                principalTable: "Rolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RollValues_Rolls_RollId",
                table: "RollValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RollValues",
                table: "RollValues");

            migrationBuilder.RenameTable(
                name: "RollValues",
                newName: "RollValue");

            migrationBuilder.RenameIndex(
                name: "IX_RollValues_RollId",
                table: "RollValue",
                newName: "IX_RollValue_RollId");

            migrationBuilder.AlterColumn<int>(
                name: "RollId",
                table: "RollValue",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "UserRoomId",
                table: "RollValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserRoomRoomId",
                table: "RollValue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRoomUserId",
                table: "RollValue",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RollValue",
                table: "RollValue",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RollValue_UserRoomUserId_UserRoomRoomId",
                table: "RollValue",
                columns: new[] { "UserRoomUserId", "UserRoomRoomId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RollValue_Rolls_RollId",
                table: "RollValue",
                column: "RollId",
                principalTable: "Rolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RollValue_UserRooms_UserRoomUserId_UserRoomRoomId",
                table: "RollValue",
                columns: new[] { "UserRoomUserId", "UserRoomRoomId" },
                principalTable: "UserRooms",
                principalColumns: new[] { "UserId", "RoomId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
