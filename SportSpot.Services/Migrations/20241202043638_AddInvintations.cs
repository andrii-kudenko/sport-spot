using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSpot.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddInvintations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Invintations",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invintations",
                table: "Users");
        }
    }
}
