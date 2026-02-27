using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.LifecycleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResolveReviewFindings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exit_interviews__offboarding_checklists_offboarding_checklist~",
                table: "exit_interviews");

            migrationBuilder.RenameIndex(
                name: "IX_onboarding_templates_department_id",
                table: "onboarding_templates",
                newName: "ix_onboarding_templates_department_id");

            migrationBuilder.RenameIndex(
                name: "IX_onboarding_items_assigned_to",
                table: "onboarding_items",
                newName: "ix_onboarding_items_assigned_to");

            migrationBuilder.RenameIndex(
                name: "IX_onboarding_checklists_status",
                table: "onboarding_checklists",
                newName: "ix_onboarding_checklists_status");

            migrationBuilder.RenameIndex(
                name: "IX_onboarding_checklists_employee_id",
                table: "onboarding_checklists",
                newName: "ix_onboarding_checklists_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_offboarding_checklists_status",
                table: "offboarding_checklists",
                newName: "ix_offboarding_checklists_status");

            migrationBuilder.RenameIndex(
                name: "IX_offboarding_checklists_employee_id",
                table: "offboarding_checklists",
                newName: "ix_offboarding_checklists_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_timestamp",
                table: "audit_logs",
                newName: "ix_audit_logs_timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_entity_id",
                table: "audit_logs",
                newName: "ix_audit_logs_entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_exit_interviews_offboarding_checklists_offboarding_checklis~",
                table: "exit_interviews",
                column: "offboarding_checklist_id",
                principalTable: "offboarding_checklists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exit_interviews_offboarding_checklists_offboarding_checklis~",
                table: "exit_interviews");

            migrationBuilder.RenameIndex(
                name: "ix_onboarding_templates_department_id",
                table: "onboarding_templates",
                newName: "IX_onboarding_templates_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_onboarding_items_assigned_to",
                table: "onboarding_items",
                newName: "IX_onboarding_items_assigned_to");

            migrationBuilder.RenameIndex(
                name: "ix_onboarding_checklists_status",
                table: "onboarding_checklists",
                newName: "IX_onboarding_checklists_status");

            migrationBuilder.RenameIndex(
                name: "ix_onboarding_checklists_employee_id",
                table: "onboarding_checklists",
                newName: "IX_onboarding_checklists_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_offboarding_checklists_status",
                table: "offboarding_checklists",
                newName: "IX_offboarding_checklists_status");

            migrationBuilder.RenameIndex(
                name: "ix_offboarding_checklists_employee_id",
                table: "offboarding_checklists",
                newName: "IX_offboarding_checklists_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_audit_logs_timestamp",
                table: "audit_logs",
                newName: "IX_audit_logs_timestamp");

            migrationBuilder.RenameIndex(
                name: "ix_audit_logs_entity_id",
                table: "audit_logs",
                newName: "IX_audit_logs_entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_exit_interviews__offboarding_checklists_offboarding_checklist~",
                table: "exit_interviews",
                column: "offboarding_checklist_id",
                principalTable: "offboarding_checklists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
