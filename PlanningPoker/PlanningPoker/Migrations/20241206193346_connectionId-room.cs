using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanningPoker.Migrations
{
    /// <inheritdoc />
    public partial class connectionIdroom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "UserRooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "UserRooms");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
