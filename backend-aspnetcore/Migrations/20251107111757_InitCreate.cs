using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAspNetCore.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Humidity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Temperature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Humidity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorLog_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorLog_SensorId",
                table: "SensorLog",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MacAddress",
                table: "Sensors",
                column: "MacAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MacAddress_SerialNumber",
                table: "Sensors",
                columns: new[] { "MacAddress", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SerialNumber",
                table: "Sensors",
                column: "SerialNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorLog");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
