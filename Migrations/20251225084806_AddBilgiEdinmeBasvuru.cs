using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddBilgiEdinmeBasvuru : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BilgiEdinmeBasvurulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SahisTuru = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KimlikNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DogumGunu = table.Column<int>(type: "int", nullable: true),
                    DogumAyi = table.Column<int>(type: "int", nullable: true),
                    DogumYili = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvTelefonu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CepTelefonu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IletisimAdresi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BasvuruMetni = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CevapNasilVerilsin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Unvan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VergiDairesi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VergiNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsTelefonu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TuzelCepTelefonu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TuzelEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TuzelIletisimAdresi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TuzelBasvuruMetni = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TuzelCevapNasilVerilsin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BilgiEdinmeBasvurulari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BilgiEdinmeBasvurulari");
        }
    }
}
