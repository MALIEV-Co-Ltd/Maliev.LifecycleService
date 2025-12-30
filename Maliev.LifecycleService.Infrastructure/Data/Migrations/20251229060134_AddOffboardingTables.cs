using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.LifecycleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOffboardingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "offboarding_checklists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    termination_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    termination_reason = table.Column<string>(type: "text", nullable: false),
                    eligible_for_rehire = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    paycheck_release_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offboarding_checklists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "offboarding_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    offboarding_checklist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    assigned_to = table.Column<Guid>(type: "uuid", nullable: true),
                    is_paycheck_blocker = table.Column<bool>(type: "boolean", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offboarding_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_offboarding_tasks_offboarding_checklists_offboarding_checkl~",
                        column: x => x.offboarding_checklist_id,
                        principalTable: "offboarding_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_offboarding_checklists_employee_id",
                table: "offboarding_checklists",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_offboarding_checklists_status",
                table: "offboarding_checklists",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_offboarding_tasks_offboarding_checklist_id",
                table: "offboarding_tasks",
                column: "offboarding_checklist_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offboarding_tasks");

            migrationBuilder.DropTable(
                name: "offboarding_checklists");
        }
    }
}
