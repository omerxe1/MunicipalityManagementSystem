using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddAfadAcilToplanmaAlanlariTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "KardesSehirler");

            migrationBuilder.CreateTable(
                name: "AfadAcilToplanmaAlanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ilce = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mahalle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Enlem = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Boylam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Elektrik = table.Column<bool>(type: "bit", nullable: false),
                    Yol = table.Column<bool>(type: "bit", nullable: false),
                    WcKanSistemi = table.Column<bool>(type: "bit", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfadAcilToplanmaAlanlari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AfadAcilToplanmaAlanlari");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "KardesSehirler",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
