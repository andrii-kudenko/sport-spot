using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSpot.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendToUserEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendRequests",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Friends",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendRequests",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Friends",
                table: "Users");
        }
    }
}
