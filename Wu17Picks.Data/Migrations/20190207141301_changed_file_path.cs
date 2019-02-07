using Microsoft.EntityFrameworkCore.Migrations;

namespace Wu17Picks.Data.Migrations
{
    public partial class changed_file_path : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "GalleryImages",
                newName: "FileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "GalleryImages",
                newName: "Url");
        }
    }
}
