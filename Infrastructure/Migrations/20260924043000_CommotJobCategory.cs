using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CommotJobCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_JobCategory_JobCategoryId",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobSearch_JobCategory_JobCategoryId",
                table: "SavedJobSearch");

            migrationBuilder.DropTable(
                name: "JobCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("8bbade0c-4e9a-4303-9a4f-5df751b11e33"));

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Deparment");

            migrationBuilder.RenameColumn(
                name: "JobCategoryId",
                table: "SavedJobSearch",
                newName: "DepartmentId1");

            migrationBuilder.RenameIndex(
                name: "IX_SavedJobSearch_JobCategoryId",
                table: "SavedJobSearch",
                newName: "IX_SavedJobSearch_DepartmentId1");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_CompanyId",
                table: "Deparment",
                newName: "IX_Deparment_CompanyId");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "SavedJobSearch",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Deparment",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deparment",
                table: "Deparment",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 24, 4, 29, 58, 864, DateTimeKind.Utc).AddTicks(5503));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("e199a51e-def7-49d4-9663-08c2e0616969"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 24, 4, 29, 58, 865, DateTimeKind.Utc).AddTicks(4942), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 24, 4, 29, 58, 783, DateTimeKind.Utc).AddTicks(6202), "AQAAAAIAAYagAAAAEFQwdoK83o9eLb8A3e5UgZTrxXzV4uRqy0uXH98fyubyEhyZPyUMS7WMGRO8imKYbg==" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobSearch_DepartmentId",
                table: "SavedJobSearch",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deparment_Name",
                table: "Deparment",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deparment_Companies_CompanyId",
                table: "Deparment",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Deparment_JobCategoryId",
                table: "Job",
                column: "JobCategoryId",
                principalTable: "Deparment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobSearch_Deparment_DepartmentId",
                table: "SavedJobSearch",
                column: "DepartmentId",
                principalTable: "Deparment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobSearch_Deparment_DepartmentId1",
                table: "SavedJobSearch",
                column: "DepartmentId1",
                principalTable: "Deparment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deparment_Companies_CompanyId",
                table: "Deparment");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Deparment_JobCategoryId",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobSearch_Deparment_DepartmentId",
                table: "SavedJobSearch");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedJobSearch_Deparment_DepartmentId1",
                table: "SavedJobSearch");

            migrationBuilder.DropIndex(
                name: "IX_SavedJobSearch_DepartmentId",
                table: "SavedJobSearch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deparment",
                table: "Deparment");

            migrationBuilder.DropIndex(
                name: "IX_Deparment_Name",
                table: "Deparment");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("e199a51e-def7-49d4-9663-08c2e0616969"));

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "SavedJobSearch");

            migrationBuilder.RenameTable(
                name: "Deparment",
                newName: "Departments");

            migrationBuilder.RenameColumn(
                name: "DepartmentId1",
                table: "SavedJobSearch",
                newName: "JobCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedJobSearch_DepartmentId1",
                table: "SavedJobSearch",
                newName: "IX_SavedJobSearch_JobCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Deparment_CompanyId",
                table: "Departments",
                newName: "IX_Departments_CompanyId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategory", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 21, 7, 41, 7, 172, DateTimeKind.Utc).AddTicks(4053));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("8bbade0c-4e9a-4303-9a4f-5df751b11e33"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 21, 7, 41, 7, 173, DateTimeKind.Utc).AddTicks(5188), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 21, 7, 41, 7, 88, DateTimeKind.Utc).AddTicks(1519), "AQAAAAIAAYagAAAAEB4ce3yxBvsMw02x2RHyEpf1leXXeHjqnyq2jp/LhZ2EHj6M+mxgnVp1gKOZKGUXgQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_Name",
                table: "JobCategory",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_JobCategory_JobCategoryId",
                table: "Job",
                column: "JobCategoryId",
                principalTable: "JobCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedJobSearch_JobCategory_JobCategoryId",
                table: "SavedJobSearch",
                column: "JobCategoryId",
                principalTable: "JobCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
