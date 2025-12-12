using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AntLogistics.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "commodities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    control_parameters = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    deactivated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commodities", x => x.id);
                    table.CheckConstraint("ck_commodities_sku_lower", "sku = lower(sku)");
                });

            migrationBuilder.CreateTable(
                name: "operators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValueSql: "'operator'"),
                    idle_timeout_minutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operators", x => x.id);
                    table.CheckConstraint("ck_operators_idle_timeout", "idle_timeout_minutes BETWEEN 5 AND 180");
                    table.CheckConstraint("ck_operators_role", "role in ('operator','admin')");
                    table.CheckConstraint("ck_operators_username_lower", "username = lower(username)");
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address_line = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    default_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "DEFAULT"),
                    capacity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    deactivated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                    table.CheckConstraint("ck_warehouses_capacity_positive", "capacity > 0");
                    table.CheckConstraint("ck_warehouses_code_lower", "code = lower(code)");
                    table.CheckConstraint("ck_warehouses_country_code", "country_code ~ '^[A-Z]{2}$'");
                });

            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    commodity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    warehouse_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "DEFAULT"),
                    operator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "manual"),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    metadata = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocks", x => x.id);
                    table.CheckConstraint("ck_stocks_quantity_positive", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_stocks_commodities_commodity_id",
                        column: x => x.commodity_id,
                        principalTable: "commodities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stocks_operators_operator_id",
                        column: x => x.operator_id,
                        principalTable: "operators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_stocks_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_commodities_active",
                table: "commodities",
                column: "sku",
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "idx_commodities_control_parameters",
                table: "commodities",
                column: "control_parameters")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_operators_active",
                table: "operators",
                column: "id",
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "ux_operators_username",
                table: "operators",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_stocks_active_wh",
                table: "stocks",
                columns: new[] { "warehouse_id", "commodity_id" },
                filter: "quantity > 0");

            migrationBuilder.CreateIndex(
                name: "idx_stocks_commodity_time",
                table: "stocks",
                columns: new[] { "commodity_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "idx_stocks_metadata",
                table: "stocks",
                column: "metadata")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_stocks_sku",
                table: "stocks",
                column: "sku")
                .Annotation("Npgsql:IndexMethod", "hash");

            migrationBuilder.CreateIndex(
                name: "idx_stocks_wh_time",
                table: "stocks",
                columns: new[] { "warehouse_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_stocks_operator_id",
                table: "stocks",
                column: "operator_id");

            migrationBuilder.CreateIndex(
                name: "idx_warehouses_active",
                table: "warehouses",
                column: "code",
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "idx_warehouses_active_country",
                table: "warehouses",
                columns: new[] { "is_active", "country_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stocks");

            migrationBuilder.DropTable(
                name: "commodities");

            migrationBuilder.DropTable(
                name: "operators");

            migrationBuilder.DropTable(
                name: "warehouses");
        }
    }
}
