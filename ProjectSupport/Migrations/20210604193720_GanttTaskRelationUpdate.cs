using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectSupport.Migrations
{
    public partial class GanttTaskRelationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GanttTaskRelations_GanttTasks_GanttTaskId",
                table: "GanttTaskRelations");

            migrationBuilder.AddForeignKey(
                name: "FK_GanttTaskRelations_GanttTasks_GanttTaskId",
                table: "GanttTaskRelations",
                column: "GanttTaskId",
                principalTable: "GanttTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GanttTaskRelations_GanttTasks_GanttTaskId",
                table: "GanttTaskRelations");

            migrationBuilder.AddForeignKey(
                name: "FK_GanttTaskRelations_GanttTasks_GanttTaskId",
                table: "GanttTaskRelations",
                column: "GanttTaskId",
                principalTable: "GanttTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
