using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.LifecycleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "onboarding_checklists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    completed_items = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_onboarding_checklists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "onboarding_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_onboarding_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "onboarding_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    onboarding_checklist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    assigned_to = table.Column<Guid>(type: "uuid", nullable: true),
                    days_due = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_onboarding_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_onboarding_items_onboarding_checklists_onboarding_checklist~",
                        column: x => x.onboarding_checklist_id,
                        principalTable: "onboarding_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "onboarding_template_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    default_assignee_role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    days_due = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_onboarding_template_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_onboarding_template_items_onboarding_templates_template_id",
                        column: x => x.template_id,
                        principalTable: "onboarding_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_checklists_employee_id",
                table: "onboarding_checklists",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_checklists_status",
                table: "onboarding_checklists",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_items_assigned_to",
                table: "onboarding_items",
                column: "assigned_to");

            migrationBuilder.CreateIndex(
                name: "ix_onboarding_items_onboarding_checklist_id",
                table: "onboarding_items",
                column: "onboarding_checklist_id");

            migrationBuilder.CreateIndex(
                name: "ix_onboarding_template_items_template_id",
                table: "onboarding_template_items",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_onboarding_templates_department_id",
                table: "onboarding_templates",
                column: "department_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "onboarding_items");

            migrationBuilder.DropTable(
                name: "onboarding_template_items");

            migrationBuilder.DropTable(
                name: "onboarding_checklists");

            migrationBuilder.DropTable(
                name: "onboarding_templates");
        }
    }
}
