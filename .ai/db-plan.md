# Database Planning Summary

## Decisions
1. The MVP schema covers the `warehouses`, global `commodities`, append-only `readings`, and asynchronous `inventory_snapshots` tables.
2. `warehouses` store the required `default_zone` column and `is_active` flag to provide a complete location description without a dedicated zones table.
3. SKUs in `commodities` are shared globally, so a `unique(sku)` constraint is maintained without a direct FK to a warehouse.
4. `readings` remain append-only and include `warehouse_id`, `sku`, `quantity`, timestamps, and a text `created_by` field instead of a link to an operators table.
5. `inventory_snapshots` store current stock levels with PK `(warehouse_id, sku)` and the `last_calculated_at` column, updated by an asynchronous job.
6. RLS will be enabled on the key tables, filtering `is_active=true`, while all write operations are executed by a trusted API role.
7. No partitioning is introduced initially; possible archiving will only be considered after the dataset grows significantly.

## Recommendations
1. Enforce `default_zone varchar(100) not null` and extend `AntLogisticsDbContext` with the new columns and entities before generating standalone SQL.
2. Preserve the uniqueness of `warehouses.code` and `commodities.sku`, adding indexes filtered by `is_active` plus time-based indexes on `readings`.
3. Use `batch_number varchar(100)` as an optional field in `readings`, while handling batch-required logic in the domain layer.
4. Keep `quantity numeric(18,3) not null` and `last_calculated_at timestamptz not null` in `inventory_snapshots`; the sync job should overwrite records after every update.
5. Add a trigger/constraint that blocks new `readings` for inactive warehouses or SKUs, leaving historical data read-only.
6. Monitor table and index sizes; if needed, implement logical archiving (e.g., moving the oldest `readings` to a history table) to avoid partitioning pressure.
7. Document the RLS process and database roles (API role vs. end users) to keep access rules consistent as the system evolves.

## Summary
The agreed decisions produce a streamlined yet scalable data backbone for the AntLogistics MVP. Warehouses and items are described globally, while material movements are persisted in append-only `readings`, simplifying auditing. Current stock levels will be maintained in the asynchronously updated `inventory_snapshots` table. Security rests on RLS with `is_active` filtering, and the lack of partitioning is mitigated by monitoring plus optional archiving. This plan sets the stage for generating standalone SQL and updating `AntLogisticsDbContext`, ensuring a coherent direction for the next iteration of database work.
