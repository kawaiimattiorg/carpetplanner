using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetPlannerB2c.Migrations
{
    public partial class RemoveUsernameFromCarpet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Carpets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Carpets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
