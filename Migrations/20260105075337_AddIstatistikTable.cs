using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddIstatistikTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Istatistikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Deger = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ikon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Istatistikler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Istatistikler");
        }
    }
}
