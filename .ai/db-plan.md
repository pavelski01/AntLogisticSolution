# AntLogistics PostgreSQL Schema

1. **List of tables with columns, data types, and constraints**

    #### `warehouses`
    | Column | Type | Constraints | Description |
    | --- | --- | --- | --- |
    | id | uuid | PK, default gen_random_uuid() | Stable identifier |
    | code | varchar(50) | NOT NULL, unique, check (code = lower(code)) | Human-friendly short code |
    | name | varchar(200) | NOT NULL | Display name |
    | address_line | text | NOT NULL | Street and number |
    | city | varchar(100) | NOT NULL | City |
    | country_code | char(2) | NOT NULL, check (country_code ~ '^[A-Z]{2}$') | ISO 3166-1 alpha-2 |
    | postal_code | varchar(20) | NULL | Postal/zip code |
    | default_zone | varchar(100) | NOT NULL | Default operational zone |
    | capacity | numeric(18,2) | NOT NULL, check (capacity > 0) | Warehouse capacity in configured units |
    | is_active | boolean | NOT NULL default true | Soft-active flag |
    | deactivated_at | timestamptz | NULL | Timestamp when disabled |
    | created_at | timestamptz | NOT NULL default now() | Audit |
    | updated_at | timestamptz | NOT NULL default now() | Audit |

    #### `commodities`
    | Column | Type | Constraints | Description |
    | --- | --- | --- | --- |
    | id | uuid | PK, default gen_random_uuid() | Stable identifier |
    | sku | varchar(100) | NOT NULL, unique (lower(sku)), check (sku = lower(sku)) | Global SKU |
    | name | varchar(200) | NOT NULL | Item name |
    | unit_of_measure | varchar(20) | NOT NULL | Base UOM (e.g. kg) |
    | batch_required | boolean | NOT NULL default false | Requires batch tracking |
    | control_parameters | jsonb | NOT NULL default '{}'::jsonb | Additional constraints (temperature, etc.) |
    | is_active | boolean | NOT NULL default true | Soft-active flag |
    | deactivated_at | timestamptz | NULL | Timestamp when disabled |
    | created_at | timestamptz | NOT NULL default now() | Audit |
    | updated_at | timestamptz | NOT NULL default now() | Audit |

    #### `operators`
    | Column | Type | Constraints | Description |
    | --- | --- | --- | --- |
    | id | uuid | PK, default gen_random_uuid() | Operator identifier |
    | username | varchar(100) | NOT NULL, unique (lower(username)) | Login name |
    | password_hash | varchar(255) | NOT NULL | Bcrypt hash |
    | full_name | varchar(200) | NOT NULL | Display name |
    | role | varchar(30) | NOT NULL default 'operator', check (role in ('operator','admin')) | Single-role model with admin override |
    | idle_timeout_minutes | integer | NOT NULL default 30, check (idle_timeout_minutes between 5 and 180) | Session timeout policy |
    | is_active | boolean | NOT NULL default true | Soft-active flag |
    | last_login_at | timestamptz | NULL | Last successful login |
    | created_at | timestamptz | NOT NULL default now() | Audit |
    | updated_at | timestamptz | NOT NULL default now() | Audit |

    #### `readings`
    | Column | Type | Constraints | Description |
    | --- | --- | --- | --- |
    | id | bigint | PK, generated always as identity | Append-only identifier |
    | warehouse_id | uuid | NOT NULL, FK -> warehouses(id) | Warehouse |
    | commodity_id | uuid | NOT NULL, FK -> commodities(id) | Item |
    | sku | varchar(100) | NOT NULL | Denormalized SKU copy |
    | unit_of_measure | varchar(20) | NOT NULL | UOM at capture time |
    | quantity | numeric(18,3) | NOT NULL, check (quantity > 0) | Captured quantity |
    | batch_number | varchar(100) | NULL | Optional batch/lot |
    | warehouse_zone | varchar(100) | NOT NULL default 'DEFAULT' | Zone of the reading |
    | operator_id | uuid | NULL, FK -> operators(id) ON DELETE SET NULL | Capturing operator |
    | created_by | text | NOT NULL | Immutable operator label |
    | source | varchar(50) | NOT NULL default 'manual' | Manual/API/import |
    | occurred_at | timestamptz | NOT NULL default now() | Physical event time |
    | created_at | timestamptz | NOT NULL default now() | Persisted timestamp |
    | metadata | jsonb | NOT NULL default '{}'::jsonb | Free-form attributes |

2. **Relationships between tables**

- `warehouses` 1:N `readings`; deleting a warehouse is blocked while dependent data exists.
- `commodities` 1:N `readings`; commodities remain referenced even when inactive.
- `operators` 1:N `readings` (nullable FK to preserve history for removed operators).

3. **Indexes**

- `warehouses`: unique index on `lower(code)`; partial index `idx_warehouses_active` on `(code)` where `is_active = true` to accelerate lookups; btree index on `(is_active, country_code)` for filtered listings.
- `commodities`: unique index on `lower(sku)`; partial index on `(sku)` where `is_active = true`; gin index on `control_parameters` for JSON containment predicates.
- `operators`: unique index on `lower(username)`; partial index on `(id)` where `is_active = true` to speed authentication checks.
- `readings`: composite index `idx_readings_wh_time` on `(warehouse_id, occurred_at DESC)`; composite index on `(commodity_id, occurred_at DESC)`; hash index on `sku`; partial index `idx_readings_active_wh` on `(warehouse_id, commodity_id)` where `quantity > 0`; gin index on `metadata` for filtering by custom attributes.

4. **PostgreSQL policies (RLS)**

```
-- Role setup
CREATE ROLE app_reader NOINHERIT;
CREATE ROLE api_writer NOINHERIT;

-- Warehouses (read-only to operators, full access to API)
ALTER TABLE warehouses ENABLE ROW LEVEL SECURITY;
CREATE POLICY warehouse_active_select
    ON warehouses FOR SELECT TO app_reader
    USING (is_active);
CREATE POLICY warehouse_writer
    ON warehouses FOR ALL TO api_writer
    USING (true) WITH CHECK (true);

-- Commodities
ALTER TABLE commodities ENABLE ROW LEVEL SECURITY;
CREATE POLICY commodity_active_select
    ON commodities FOR SELECT TO app_reader
    USING (is_active);
CREATE POLICY commodity_writer
    ON commodities FOR ALL TO api_writer
    USING (true) WITH CHECK (true);

-- Readings (operators see only their active warehouse/item rows; API can see all)
ALTER TABLE readings ENABLE ROW LEVEL SECURITY;
CREATE POLICY readings_operator_select
    ON readings FOR SELECT TO app_reader
    USING (
        (is_active_warehouse(warehouse_id) AND is_active_commodity(commodity_id))
        AND (
            operator_id = current_setting('app.operator_id', true)::uuid
            OR current_setting('app.is_admin', true)::boolean
        )
    );
CREATE POLICY readings_writer
    ON readings FOR INSERT TO api_writer
    WITH CHECK (is_active_warehouse(warehouse_id) AND is_active_commodity(commodity_id));

```

_Helper functions `is_active_warehouse(uuid)` and `is_active_commodity(uuid)` return true when the referenced row exists with `is_active = true`; they are implemented as stable SQL functions to keep policies readable._

5. **Additional notes**

- `gen_random_uuid()` requires the `pgcrypto` extension; enable it in the migration bootstrap.
- `readings` remains append-only via a `BEFORE UPDATE OR DELETE` trigger that raises an exception, ensuring auditability; corrections are handled by compensating entries.
- Application services must set `SET LOCAL app.operator_id = '<uuid>';` and `SET LOCAL app.is_admin = 't'/'f';` per request so RLS policies can evaluate operator-level access.
- `control_parameters` and `metadata` fields allow future validations (e.g., enforcing cold-chain range) without schema churn while still remaining queryable through GIN indexes.
- Archiving strategy: monitor `readings` row count; when retention thresholds are exceeded, move the oldest rows to a `readings_history` table that shares the schema and inherits indexes/RLS policies.
