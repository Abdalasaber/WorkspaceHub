using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkspaceHub.Infrastructure.Migrations;

public partial class FixWorkspaceCode : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Workspaces_BranchId_Code",
            table: "Workspaces",
            columns: new[] { "BranchId", "Code" },
            unique: true,
            filter: "[IsDeleted] = 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Workspaces_BranchId_Code", table: "Workspaces");
    }
}
