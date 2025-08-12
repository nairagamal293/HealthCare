using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeathCare.Migrations
{
    /// <inheritdoc />
    public partial class RemovecontentfromContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "content",
                table: "Contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
