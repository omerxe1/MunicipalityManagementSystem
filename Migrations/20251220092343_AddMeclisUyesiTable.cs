using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddMeclisUyesiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeclisUyeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Parti = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResimYolu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeclisUyeleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeclisUyeleri");
        }
    }
}
