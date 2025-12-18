using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PROJECT_energy_monitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Generators",
                columns: table => new
                {
                    GeneratorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    MaxPowerOutput = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generators", x => x.GeneratorId);
                });

            migrationBuilder.CreateTable(
                name: "Thresholds",
                columns: table => new
                {
                    ThresholdId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SensorId = table.Column<int>(type: "integer", nullable: false),
                    MinValue = table.Column<double>(type: "double precision", nullable: true),
                    MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    AlertMessage = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thresholds", x => x.ThresholdId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Acknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedBy = table.Column<int>(type: "integer", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneratorId = table.Column<int>(type: "integer", nullable: false),
                    ThresholdId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Generators_GeneratorId",
                        column: x => x.GeneratorId,
                        principalTable: "Generators",
                        principalColumn: "GeneratorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    SensorReadingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Voltage = table.Column<double>(type: "double precision", nullable: false),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    Power = table.Column<double>(type: "double precision", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    Rpm = table.Column<int>(type: "integer", nullable: false),
                    GeneratorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.SensorReadingId);
                    table.ForeignKey(
                        name: "FK_SensorReadings_Generators_GeneratorId",
                        column: x => x.GeneratorId,
                        principalTable: "Generators",
                        principalColumn: "GeneratorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_GeneratorId",
                table: "Alerts",
                column: "GeneratorId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_GeneratorId",
                table: "SensorReadings",
                column: "GeneratorId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_Timestamp",
                table: "SensorReadings",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "SensorReadings");

            migrationBuilder.DropTable(
                name: "Thresholds");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Generators");
        }
    }
}
