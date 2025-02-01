using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanningPoker.Migrations
{
    /// <inheritdoc />
    public partial class iconpack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconPack",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsModerator",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconPack",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsModerator",
                table: "Users");
        }
    }
}
