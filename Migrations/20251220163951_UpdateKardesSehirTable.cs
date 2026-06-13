using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kocaaliv2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKardesSehirTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnlasmaTarihi",
                table: "KardesSehirler",
                newName: "ProtokolTarihi");

            migrationBuilder.AddColumn<string>(
                name: "BelediyeAdi",
                table: "KardesSehirler",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KararNo",
                table: "KardesSehirler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BelediyeAdi",
                table: "KardesSehirler");

            migrationBuilder.DropColumn(
                name: "KararNo",
                table: "KardesSehirler");

            migrationBuilder.RenameColumn(
                name: "ProtokolTarihi",
                table: "KardesSehirler",
                newName: "AnlasmaTarihi");
        }
    }
}
