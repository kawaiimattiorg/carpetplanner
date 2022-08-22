using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetPlannerB2c.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carpets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    StripeSeparator = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 100),
                    Removed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carpets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false),
                    Rgb = table.Column<string>(type: "TEXT", nullable: true),
                    Removed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stripes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarpetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<double>(type: "REAL", nullable: false),
                    Ordinal = table.Column<int>(type: "INTEGER", nullable: false),
                    CarpetEntityId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stripes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stripes_Carpets_CarpetEntityId",
                        column: x => x.CarpetEntityId,
                        principalTable: "Carpets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stripes_CarpetEntityId",
                table: "Stripes",
                column: "CarpetEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Stripes");

            migrationBuilder.DropTable(
                name: "Carpets");
        }
    }
}
