using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class AddKategoriAndMekanToEtkinlik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Announcements");

            migrationBuilder.AddColumn<string>(
                name: "EtkinlikKategori",
                table: "Etkinlikler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EtkinlikMekan",
                table: "Etkinlikler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtkinlikKategori",
                table: "Etkinlikler");

            migrationBuilder.DropColumn(
                name: "EtkinlikMekan",
                table: "Etkinlikler");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Announcements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
