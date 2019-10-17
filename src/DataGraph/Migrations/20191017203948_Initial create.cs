using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataGraph.Migrations
{
    public partial class Initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataGraph",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Schema = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataGraph", x => new { x.CustomerId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    GraphId = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    ObjectType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => new { x.CustomerId, x.GraphId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_Objects_DataGraph_CustomerId_GraphId",
                        columns: x => new { x.CustomerId, x.GraphId },
                        principalTable: "DataGraph",
                        principalColumns: new[] { "CustomerId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListOfLiterals",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    GraphId = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    PropertyName = table.Column<string>(nullable: false),
                    ListItemId = table.Column<int>(nullable: false),
                    ListItemValueJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListOfLiterals", x => new { x.CustomerId, x.GraphId, x.ObjectId, x.PropertyName, x.ListItemId });
                    table.UniqueConstraint("AK_ListOfLiterals_CustomerId_GraphId_ListItemId_ObjectId_PropertyName", x => new { x.CustomerId, x.GraphId, x.ListItemId, x.ObjectId, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_ListOfLiterals_Objects_CustomerId_GraphId_ObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListOfReferences",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    GraphId = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    PropertyName = table.Column<string>(nullable: false),
                    ReferencedObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListOfReferences", x => new { x.CustomerId, x.GraphId, x.ObjectId, x.PropertyName, x.ReferencedObjectId });
                    table.ForeignKey(
                        name: "FK_ListOfReferences_Objects_CustomerId_GraphId_ObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListOfReferences_Objects_CustomerId_GraphId_ReferencedObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ReferencedObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiteralPropertyValues",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    GraphId = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    PropertyName = table.Column<string>(nullable: false),
                    ProperyValueJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiteralPropertyValues", x => new { x.CustomerId, x.GraphId, x.ObjectId, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_LiteralPropertyValues_Objects_CustomerId_GraphId_ObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferencePropertyValues",
                columns: table => new
                {
                    CustomerId = table.Column<string>(nullable: false),
                    GraphId = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    PropertyName = table.Column<string>(nullable: false),
                    ReferencedObjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferencePropertyValues", x => new { x.CustomerId, x.GraphId, x.ObjectId, x.PropertyName });
                    table.ForeignKey(
                        name: "FK_ReferencePropertyValues_Objects_CustomerId_GraphId_ObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferencePropertyValues_Objects_CustomerId_GraphId_ReferencedObjectId",
                        columns: x => new { x.CustomerId, x.GraphId, x.ReferencedObjectId },
                        principalTable: "Objects",
                        principalColumns: new[] { "CustomerId", "GraphId", "ObjectId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListOfReferences_CustomerId_GraphId_ReferencedObjectId",
                table: "ListOfReferences",
                columns: new[] { "CustomerId", "GraphId", "ReferencedObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePropertyValues_CustomerId_GraphId_ReferencedObjectId",
                table: "ReferencePropertyValues",
                columns: new[] { "CustomerId", "GraphId", "ReferencedObjectId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListOfLiterals");

            migrationBuilder.DropTable(
                name: "ListOfReferences");

            migrationBuilder.DropTable(
                name: "LiteralPropertyValues");

            migrationBuilder.DropTable(
                name: "ReferencePropertyValues");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "DataGraph");
        }
    }
}
