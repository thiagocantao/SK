using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SK.Report.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessao",
                columns: table => new
                {
                    Ip = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    WorkspaceID = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EditMode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    UserID = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ObjectID = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ObjectType = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessao", x => x.Ip);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessao");
        }
    }
}
