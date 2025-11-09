using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAspNetCore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniquesInSensor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sensors_MacAddress",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MacAddress_SerialNumber",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_SerialNumber",
                table: "Sensors");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MacAddress",
                table: "Sensors",
                column: "MacAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MacAddress_SerialNumber",
                table: "Sensors",
                columns: new[] { "MacAddress", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SerialNumber",
                table: "Sensors",
                column: "SerialNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sensors_MacAddress",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MacAddress_SerialNumber",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_SerialNumber",
                table: "Sensors");

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
    }
}
