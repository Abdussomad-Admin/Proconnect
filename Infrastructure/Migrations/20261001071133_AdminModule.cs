using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("e199a51e-def7-49d4-9663-08c2e0616969"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Report",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Report",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 10, 1, 7, 11, 30, 798, DateTimeKind.Utc).AddTicks(8103));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("468d8cc8-2303-4128-aff8-689e01e5478a"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 10, 1, 7, 11, 30, 800, DateTimeKind.Utc).AddTicks(1470), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword", "Status" },
                values: new object[] { new DateTime(2026, 10, 1, 7, 11, 30, 693, DateTimeKind.Utc).AddTicks(4637), "AQAAAAIAAYagAAAAEL9NXwCKuScg5iF1EazblSt0vKcD/QYV+2opRua2lfl13OUYcJzCKMy+z7GKDalZkQ==", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("468d8cc8-2303-4128-aff8-689e01e5478a"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Companies");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Report",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

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
                columns: new[] { "DateCreated", "HashedPassword", "IsActive" },
                values: new object[] { new DateTime(2026, 9, 24, 4, 29, 58, 783, DateTimeKind.Utc).AddTicks(6202), "AQAAAAIAAYagAAAAEFQwdoK83o9eLb8A3e5UgZTrxXzV4uRqy0uXH98fyubyEhyZPyUMS7WMGRO8imKYbg==", true });
        }
    }
}
