using Microsoft.EntityFrameworkCore.Migrations;

namespace DataGraph.Migrations
{
    public partial class AddGlobalObjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlobalObjectId",
                table: "DataGraph",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalObjectId",
                table: "DataGraph");
        }
    }
}
