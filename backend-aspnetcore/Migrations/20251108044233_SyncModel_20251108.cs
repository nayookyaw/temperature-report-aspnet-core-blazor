using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAspNetCore.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20251108 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorLog_Sensors_SensorId",
                table: "SensorLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorLog",
                table: "SensorLog");

            migrationBuilder.RenameTable(
                name: "SensorLog",
                newName: "SensorLogs");

            migrationBuilder.RenameIndex(
                name: "IX_SensorLog_SensorId",
                table: "SensorLogs",
                newName: "IX_SensorLogs_SensorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorLogs",
                table: "SensorLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorLogs_Sensors_SensorId",
                table: "SensorLogs",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorLogs_Sensors_SensorId",
                table: "SensorLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorLogs",
                table: "SensorLogs");

            migrationBuilder.RenameTable(
                name: "SensorLogs",
                newName: "SensorLog");

            migrationBuilder.RenameIndex(
                name: "IX_SensorLogs_SensorId",
                table: "SensorLog",
                newName: "IX_SensorLog_SensorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorLog",
                table: "SensorLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorLog_Sensors_SensorId",
                table: "SensorLog",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
