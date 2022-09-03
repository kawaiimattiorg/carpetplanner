using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetPlannerB2c.Migrations
{
    public partial class FixStripeDoubleForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stripes_Carpets_CarpetEntityId",
                table: "Stripes");

            migrationBuilder.DropIndex(
                name: "IX_Stripes_CarpetEntityId",
                table: "Stripes");

            migrationBuilder.DropColumn(
                name: "CarpetEntityId",
                table: "Stripes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarpetEntityId",
                table: "Stripes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stripes_CarpetEntityId",
                table: "Stripes",
                column: "CarpetEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stripes_Carpets_CarpetEntityId",
                table: "Stripes",
                column: "CarpetEntityId",
                principalTable: "Carpets",
                principalColumn: "Id");
        }
    }
}
