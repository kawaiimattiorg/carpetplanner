using Microsoft.EntityFrameworkCore.Migrations;

namespace CarpetPlanner.Migrations
{
    public partial class AddOrdinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ordinal",
                table: "Stripes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"
                    UPDATE Stripes
                    SET Ordinal = Id;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ordinal",
                table: "Stripes");
        }
    }
}
