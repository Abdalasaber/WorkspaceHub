using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkspaceHub.Infrastructure.Migrations;

public partial class BackfillPaymentAppliedAmount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE Payments SET AppliedAmount = Amount WHERE AppliedAmount = 0 AND Amount > 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Data backfill is intentionally not reversed.
    }
}
