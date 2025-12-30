using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.LifecycleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExitInterviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exit_interviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    offboarding_checklist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conducted_by = table.Column<Guid>(type: "uuid", nullable: false),
                    interview_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reason_for_leaving = table.Column<string>(type: "text", nullable: true),
                    feedback_on_manager = table.Column<string>(type: "text", nullable: true),
                    feedback_on_team = table.Column<string>(type: "text", nullable: true),
                    feedback_on_company = table.Column<string>(type: "text", nullable: true),
                    improvement_suggestions = table.Column<string>(type: "text", nullable: true),
                    would_recommend_company = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exit_interviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_exit_interviews__offboarding_checklists_offboarding_checklist~",
                        column: x => x.offboarding_checklist_id,
                        principalTable: "offboarding_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exit_interviews_offboarding_checklist_id",
                table: "exit_interviews",
                column: "offboarding_checklist_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exit_interviews");
        }
    }
}
