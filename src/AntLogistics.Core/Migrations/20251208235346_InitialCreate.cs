using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AntLogistics.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false, defaultValue: "Operator"),
                    IdleTimeoutMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DefaultZone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "DEFAULT"),
                    Capacity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionToken = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClientIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatorSessions_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commodities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WeightPerUnit = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    VolumePerUnit = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    MinimumStockLevel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    BatchRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ControlParameters = table.Column<string>(type: "text", nullable: false, defaultValue: "{}"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commodities_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Readings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    CommodityId = table.Column<int>(type: "integer", nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WarehouseZone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "DEFAULT"),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "manual"),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: false, defaultValue: "{}")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Readings_Commodities_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Readings_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Readings_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "Capacity", "City", "Code", "Country", "CreatedAt", "DeactivatedAt", "DefaultZone", "IsActive", "Name", "PostalCode", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Industrial Boulevard", 50000.00m, "Chicago", "CDC-001", "United States", new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), null, "DEFAULT", true, "Central Distribution Center", "60601", null },
                    { 2, "456 Commerce Street", 35000.00m, "New York", "ERH-002", "United States", new DateTime(2024, 2, 20, 14, 30, 0, 0, DateTimeKind.Utc), null, "DEFAULT", true, "Eastern Regional Hub", "10001", null },
                    { 3, "789 Pacific Avenue", 42000.00m, "Los Angeles", "WLC-003", "United States", new DateTime(2024, 3, 10, 9, 15, 0, 0, DateTimeKind.Utc), null, "DEFAULT", true, "Western Logistics Center", "90001", null }
                });

            migrationBuilder.CreateIndex(
                name: "idx_commodities_active",
                table: "Commodities",
                columns: new[] { "IsActive", "Sku" });

            migrationBuilder.CreateIndex(
                name: "IX_Commodities_Sku",
                table: "Commodities",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commodities_WarehouseId",
                table: "Commodities",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_Username",
                table: "Operators",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatorSessions_OperatorId",
                table: "OperatorSessions",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatorSessions_SessionToken",
                table: "OperatorSessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_readings_commodity_time",
                table: "Readings",
                columns: new[] { "CommodityId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "idx_readings_sku",
                table: "Readings",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "idx_readings_wh_time",
                table: "Readings",
                columns: new[] { "WarehouseId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Readings_OperatorId",
                table: "Readings",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "idx_warehouses_active",
                table: "Warehouses",
                columns: new[] { "IsActive", "Country" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperatorSessions");

            migrationBuilder.DropTable(
                name: "Readings");

            migrationBuilder.DropTable(
                name: "Commodities");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "Warehouses");
        }
    }
}
