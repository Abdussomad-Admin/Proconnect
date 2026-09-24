using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("695078e2-cf76-47ee-b37f-095659a41dbe"));

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePublicId",
                table: "Companies",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Companies",
                type: "longtext",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 19, 9, 6, 45, 157, DateTimeKind.Utc).AddTicks(8126));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("bc766de4-bea8-457d-aedd-f25488533e49"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 19, 9, 6, 45, 158, DateTimeKind.Utc).AddTicks(7606), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 19, 9, 6, 45, 77, DateTimeKind.Utc).AddTicks(7259), "AQAAAAIAAYagAAAAECnhGgrvv4LsoGmxxcN/AotzwhVHYpk5jfMZ0mf+zCgepBUqbO0vZmeTc94z1oHzQw==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification",
                column: "ActorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("bc766de4-bea8-457d-aedd-f25488533e49"));

            migrationBuilder.DropColumn(
                name: "CoverImagePublicId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Companies");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 13, 11, 58, 54, 575, DateTimeKind.Utc).AddTicks(2954));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("695078e2-cf76-47ee-b37f-095659a41dbe"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 13, 11, 58, 54, 576, DateTimeKind.Utc).AddTicks(3017), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 13, 11, 58, 54, 489, DateTimeKind.Utc).AddTicks(7113), "AQAAAAIAAYagAAAAEJVlEBdRzNH/xbobmSxIaqNLUFysHlcoxrjmERaOxrwgFMmZvBqZFsY7UlazkLn1Gw==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification",
                column: "ActorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
