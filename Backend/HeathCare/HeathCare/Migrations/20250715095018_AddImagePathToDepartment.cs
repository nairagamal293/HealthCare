using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeathCare.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagePath",
                value: "/uploads/departments/default-cardiology.jpg");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagePath",
                value: "/uploads/departments/default-neurology.jpg");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagePath",
                value: "/uploads/departments/default-pediatrics.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Departments");
        }
    }
}
