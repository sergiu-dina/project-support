using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectSupport.Migrations
{
    public partial class NotificationUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desctiption",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Desctiption",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
