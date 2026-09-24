using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("19b794c6-6815-4862-b96a-c77f3a77186c"));

            migrationBuilder.CreateTable(
                name: "SavedCandidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    RecruiterProfileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProfessionalProfileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedCandidates_ProfessionalProfiles_ProfessionalProfileId",
                        column: x => x.ProfessionalProfileId,
                        principalTable: "ProfessionalProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedCandidates_RecruiterProfiles_RecruiterProfileId",
                        column: x => x.RecruiterProfileId,
                        principalTable: "RecruiterProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_SavedCandidates_ProfessionalProfileId",
                table: "SavedCandidates",
                column: "ProfessionalProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCandidates_RecruiterProfileId",
                table: "SavedCandidates",
                column: "RecruiterProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedCandidates");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("8bbade0c-4e9a-4303-9a4f-5df751b11e33"));

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 20, 18, 43, 9, 125, DateTimeKind.Utc).AddTicks(8601));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("19b794c6-6815-4862-b96a-c77f3a77186c"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 20, 18, 43, 9, 126, DateTimeKind.Utc).AddTicks(8302), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 20, 18, 43, 9, 44, DateTimeKind.Utc).AddTicks(8146), "AQAAAAIAAYagAAAAECbyXdueiLkOUjcqQTYQrV68VeTkrduM/l/Qw0s4Y9xWKWjgTfNu37c7xksX5FD7GA==" });
        }
    }
}
