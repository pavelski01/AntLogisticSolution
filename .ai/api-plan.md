# REST API Plan

## 1. Resources
- `Warehouses` - master data for physical locations; stored in `warehouses`.
- `Commodities` - catalog of stock keeping units and control rules; stored in `commodities`.
- `Readings` - append-only intake transactions linking warehouses, commodities, and operators; stored in `readings`.
- `Inventory Aggregates` - virtual resource produced by summing readings per warehouse and commodity (optionally backed by snapshots).
- `Operators` - single-role user accounts plus admin flag; stored in `operators`.
- `Operator Sessions` - issued tokens and idle timeout metadata; stored in `operator_sessions`.
- `Activity Feed` - virtual view of the authenticated operator's recent readings (sourced from `readings`).

## 2. Endpoints
### Warehouses
- **GET `/api/v1/warehouses`**  
  Description: Paginated, filterable list (active by default).  
  Query: `page` (default 1), `pageSize` (default 25, max 100), `sort` (`name|code|country|-updatedAt`), `code`, `name`, `countryCode`, `city`, `isActive` (bool).  
  Response JSON:
  ```json
  {
    "items": [
      {
        "id": "uuid",
        "code": "wh-north",
        "name": "North Hub",
        "addressLine": "Street 1",
        "city": "Warsaw",
        "countryCode": "PL",
        "postalCode": "00-001",
        "defaultZone": "DEFAULT",
        "capacity": 1200.00,
        "isActive": true,
        "createdAt": "2025-01-02T12:00:00Z",
        "updatedAt": "2025-01-03T08:00:00Z"
      }
    ],
    "page": 1,
    "pageSize": 25,
    "total": 120
  }
  ```  
  Success: `200 OK`.  
  Errors: `400` invalid filters, `401` unauthenticated.

- **POST `/api/v1/warehouses`**  
  Description: Create warehouse; enforce lowercase unique `code`, ISO `countryCode`, and `capacity > 0`.  
  Request JSON mirrors item fields minus identifiers; server normalizes `code`.  
  Response: `201 Created` with body and `Location` header.  
  Errors: `400` validation, `409` duplicate code, `422` business rule, `401`.

- **GET `/api/v1/warehouses/{id}`**  
  Description: Fetch single warehouse (inactive allowed for admins).  
  Success: `200 OK`.  
  Errors: `404` missing or hidden, `401`.

- **PUT `/api/v1/warehouses/{id}`**  
  Description: Replace mutable properties (address, capacity, defaultZone).  
  Request JSON: full representation.  
  Success: `200 OK`.  
  Errors: `400`, `404`, `409` concurrency via `If-Unmodified-Since`, `422`.

- **PATCH `/api/v1/warehouses/{id}/status`**  
  Description: Activate or deactivate; body `{ "isActive": false, "reason": "Maintenance" }`.  
  Success: `200 OK`.  
  Errors: `404`, `409` already at requested status.

### Commodities
- **GET `/api/v1/commodities`**  
  Query: `page`, `pageSize`, `sort` (`name|sku|-updatedAt`), `sku`, `name`, `unitOfMeasure`, `isActive`, `batchRequired`.  
  Response: same pagination envelope as warehouses.  
  Success: `200 OK`; errors `400`, `401`.

- **POST `/api/v1/commodities`**  
  Request JSON:
  ```json
  {
    "sku": "item-001",
    "name": "Steel Bolt",
    "unitOfMeasure": "pcs",
    "batchRequired": false,
    "controlParameters": {
      "temperatureMin": -5,
      "temperatureMax": 40
    }
  }
  ```  
  Success: `201 Created`.  
  Errors: `400`, `409` duplicate SKU, `422` invalid `controlParameters`.

- **GET `/api/v1/commodities/{id}`** - `200 OK` / `404 Not Found`.

- **PUT `/api/v1/commodities/{id}`** - Full update, optionally reject SKU changes; `200 OK` / `409`.

- **PATCH `/api/v1/commodities/{id}/status`** - Activate or deactivate; identical behavior to warehouse status endpoint.

### Readings
- **GET `/api/v1/readings`**  
  Description: Paginated append-only history with optional includes.  
  Query: `page`, `pageSize` (max 200), `sort` (`-occurredAt` default), `warehouseId`, `commodityId`, `sku`, `operatorId`, `source`, `from`, `to`, `batchNumber`, `warehouseZone`, `include=warehouse,commodity`.  
  Response JSON:
  ```json
  {
    "items": [
      {
        "id": 12345,
        "warehouseId": "uuid",
        "commodityId": "uuid",
        "sku": "item-001",
        "quantity": 12.500,
        "unitOfMeasure": "kg",
        "batchNumber": "LOT-2025-01",
        "warehouseZone": "FREEZER-1",
        "operatorId": "uuid",
        "createdBy": "operator@antlogistics",
        "source": "manual",
        "occurredAt": "2025-02-01T12:00:00Z",
        "createdAt": "2025-02-01T12:00:01Z"
      }
    ],
    "page": 1,
    "pageSize": 50,
    "total": 870
  }
  ```  
  Success: `200 OK`.  
  Errors: `400`, `401`, `403` (blocked by RLS).

- **POST `/api/v1/readings`**  
  Description: Capture intake; server enforces positive quantity, active references, batch rules, and fills `sku`, `createdBy`, `operatorId`, `source`.  
  Request JSON:
  ```json
  {
    "warehouseId": "uuid",
    "commodityId": "uuid",
    "quantity": 5.250,
    "unitOfMeasure": "kg",
    "batchNumber": "LOT-2025-01",
    "warehouseZone": "DEFAULT",
    "occurredAt": "2025-02-01T11:59:00Z",
    "metadata": {
      "temperature": "-2C"
    }
  }
  ```  
  Response: `201 Created`.  
  Errors: `400` invalid quantity or date, `403` inactive warehouse or commodity, `409` batch required, `422` metadata schema, `429` throttled.

- **GET `/api/v1/readings/{id}`** - Detail lookup; `200 OK` / `404`.

### Inventory Aggregates
- **GET `/api/v1/inventory`**  
  Description: Summed quantities per warehouse and commodity with optional buckets.  
  Query: `warehouseId`, `commodityId`, `sku`, `from`, `to`, `bucket` (`none|day|hour`), `includeInactive` (admin only).  
  Response JSON:
  ```json
  {
    "items": [
      {
        "warehouseId": "uuid",
        "warehouseCode": "wh-north",
        "commodityId": "uuid",
        "sku": "item-001",
        "quantity": 128.75,
        "lastUpdate": "2025-02-01T12:00:00Z"
      }
    ],
    "generatedAt": "2025-02-01T12:05:00Z"
  }
  ```  
  Success: `200 OK`.  
  Errors: `400` invalid filters, `401`, `403`.

### Operator Sessions
- **POST `/api/v1/operator-sessions`**  
  Description: Username/password login; issues UUID token stored as HttpOnly Secure cookie and in response body.  
  Request JSON: `{ "username": "operator", "password": "secret" }`.  
  Response JSON includes `sessionId`, `operatorId`, `token`, `idleTimeoutMinutes`, `issuedAt`, `expiresAt`.  
  Success: `201 Created`.  
  Errors: `400` missing fields, `401` invalid credentials or inactive account, `423` locked, `429` rate limited.

- **PATCH `/api/v1/operator-sessions/{sessionId}`**  
  Description: Refresh `lastSeenAt`, optionally rotate token, update `userAgent` or `clientIp`.  
  Success: `200 OK`.  
  Errors: `401`, `404` revoked or expired session.

- **DELETE `/api/v1/operator-sessions/{sessionId}`**  
  Description: Logout and revoke session.  
  Success: `204 No Content`.  
  Errors: `401`, `404`.

### Operators
- **GET `/api/v1/operators/me`** - Return profile, idle timeout, last login, and role; `200 OK` / `401`.
- **PATCH `/api/v1/operators/me`** - Update mutable preferences (e.g., `idleTimeoutMinutes` between 5 and 180); `200 OK` or `400`/`401`/`422`.
- **Admin only**: `GET /api/v1/operators` and `PATCH /api/v1/operators/{id}` for technical support with `app.is_admin = true`.

### Activity Feed
- **GET `/api/v1/activity`**  
  Description: Returns the authenticated operator's last `n` readings with filters.  
  Query: `limit` (default 20, max 100), `warehouseId`, `from`, `to`.  
  Response JSON:
  ```json
  {
    "items": [
      {
        "readingId": 12345,
        "warehouseName": "North Hub",
        "commodityName": "Steel Bolt",
        "quantity": 12.5,
        "unitOfMeasure": "kg",
        "occurredAt": "2025-02-01T12:00:00Z"
      }
    ],
    "limit": 20
  }
  ```  
  Success: `200 OK`.  
  Errors: `401`.

## 3. Authentication and Authorization
- **Mechanism**: Session tokens from `operator_sessions`. Tokens stored as Secure, HttpOnly cookies with optional Bearer header for API clients. Server hashes tokens at rest and tracks `issued_at`, `last_seen_at`, and `expires_at`.
- **Login flow**: `POST /operator-sessions` validates bcrypt hash, enforces `is_active`, and logs failed attempts without sensitive payloads. Idle timeout defaults to 30 minutes but respects operator setting between 5 and 180 minutes. `PATCH /operator-sessions/{id}` updates `last_seen_at`; middleware revokes sessions exceeding idle timeout or absolute `expires_at`.
- **Authorization**: Minimal roles (`operator`, `admin`). Admin flag toggles `app.is_admin` for maintenance endpoints (e.g., viewing inactive warehouses). PostgreSQL RLS restricts operators to active warehouses/commodities and their own sessions; API re-checks `is_active` before writes.
- **Transport and security controls**: HTTPS enforced via Aspire proxy, CORS limited to Astro frontend origin. CSRF mitigated with SameSite=strict cookies plus anti-CSRF header (double submit token). Rate limiting applied to login, session refresh, and receipt creation (for example 5 requests per minute per user/IP).
- **Observability**: All auth failures, warehouse edits, and readings emit OpenTelemetry spans and structured logs tagged with `operatorId`, `warehouseId`, and `correlationId`. Auditability relies on DB timestamps plus telemetry (dedicated audit tables are roadmap).

## 4. Validation and Business Logic
- **Warehouses**
  - `code` lowercased, 1-50 chars, unique; duplicates return `409`.
  - `countryCode` matches `^[A-Z]{2}$`.
  - `capacity` numeric(18,2) greater than zero.
  - `defaultZone` required; `postalCode` optional.
  - Deactivation hides record from selectors (GET defaults to `isActive=true`).
  - Mutations emit OpenTelemetry events including operator ID and change summary.

- **Commodities**
  - `sku` lowercased unique; treat as immutable unless admin override.
  - `unitOfMeasure` required (max 20 chars); `name` max 200 chars.
  - `batchRequired` enforces `batchNumber` on readings when true.
  - `controlParameters` must be a JSON object; stored as JSONB with GIN index.

- **Readings**
  - `quantity` numeric(18,3) greater than zero; enforce scale <= 3.
  - `warehouseId` and `commodityId` must reference active rows; service validates with helper functions before insert.
  - `occurredAt` defaults to current UTC; reject timestamps more than 5 minutes in the future or before retention cutoff.
  - `metadata` limited to 8 KB, JSON object only.
  - Append-only rule: no update/delete endpoints; corrections require compensating POST with negative quantity or explanatory metadata.
  - Validation failures return `422` with Polish error messages and correlation ID (PRD US-007).
  - Success invalidates inventory cache or pushes SSE/SignalR update for live UI refresh.

- **Inventory Aggregates**
  - Aggregation sums only active warehouses/commodities unless `includeInactive=true` and admin role.
  - Date range limited to 90 days per request; bucketed responses include `bucketStart` and `bucketEnd` fields.
  - Endpoint should prefer indexed queries or snapshot tables to maintain <800 ms response time.

- **Operator Sessions**
  - `username` stored lowercase unique, 1-100 chars.
  - `idleTimeoutMinutes` constrained between 5 and 180; updates recalc `expires_at` for active sessions.
  - Sessions revoked on logout, operator deactivation, idle timeout, or admin action to keep RLS context accurate.

- **Operators**
  - `fullName` required; `role` limited to `operator` or `admin`.
  - `lastLoginAt` updated on successful login for security analytics.

- **Activity Feed**
  - Always scoped to authenticated operator (server injects `operatorId` filter); no override parameter.
  - Sorted by `occurredAt DESC`; optional `from` and `to` validated like readings list.

- **Error responses**
  - Standard envelope:
    ```json
    {
      "errors": [
        { "field": "code", "message": "Kod magazynu jest wymagany." }
      ],
      "correlationId": "uuid"
    }
    ```
  - Messages localized to Polish and logged with correlation ID.

- **Performance and reliability**
  - All list endpoints paginate; defaults optimized for UI but adjustable within bounds.
  - `POST /readings` honors `Idempotency-Key` header to avoid duplicate submissions.
  - Background snapshot job keeps inventory responses fast; endpoint falls back to live aggregation when snapshot stale.
