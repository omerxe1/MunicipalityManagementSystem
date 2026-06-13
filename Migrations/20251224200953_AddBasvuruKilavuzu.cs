using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddBasvuruKilavuzu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasvuruKilavuzlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DosyaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruKilavuzlari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasvuruKilavuzlari");
        }
    }
}
