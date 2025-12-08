# Product Requirements Document (PRD) - AntLogistics WMS MVP
## 1. Product Overview
AntLogistics WMS MVP is a web application that lets warehouse teams record foundational data about warehouses, items, and goods receipts. The solution is built on .NET 9 with .NET Aspire (AppHost, Core API, React/Vite UI) and uses PostgreSQL plus Entity Framework Core. The MVP delivers a simple browser interface where a single authenticated operator can manage master data and capture and save inventory readings. This minimal release focuses on intake processing and displaying current stock levels, enabling quick rollout and laying the groundwork for future modules (orders, reporting, integrations).

## 2. User Problem
Warehouse teams currently track stock movements manually (spreadsheets, email), which leads to errors, stale data, and poor visibility. Operators lack a single source of truth about locations, cannot quickly register receipts, and logistics decisions rely on incomplete information. Basic access control to warehouse data is also missing. The MVP addresses these issues by providing a simplified WMS that supports adding warehouses and items, registering receipts, and browsing up-to-date inventory once logged in.

## 3. Functional Requirements
1. Warehouse registration: name, code, address, active status, capacity, default zone.
2. Item definition: name, SKU, unit of measure, control parameters (e.g., batch/lot requirement), active status.
3. Receipt (reading) entry for a selected warehouse with item, quantity, batch (if required), operator, and timestamp.
4. UI data validation (required fields, numeric ranges, code uniqueness) with error messages in Polish.
5. List views for warehouses, items, and receipt history with basic filters (name/code, date range).
6. Inventory view per warehouse aggregating total receipts (no manual adjustments in MVP – raw receipt sums only).
7. Simple account system: single user role, login-based authentication (username + bcrypt-hashed password), no password resets, ability for a technical admin to deactivate accounts outside the UI.
8. Browser session management (token/cookie) with automatic logout after idle timeout (configurable, default 30 minutes).
9. Logging of errors and unauthorized access attempts to the Observability center (Aspire/OpenTelemetry) for operations.
10. Basic API documentation (OpenAPI) and user instructions describing the flow: login → add warehouse → add item → record receipt → view inventory.

## 4. Product Boundaries
In scope (MVP):
- Web application (React/Vite + ASP.NET Core API) orchestrated via .NET Aspire AppHost.
- PostgreSQL data layer with Warehouse and Item models plus receipt storage.
- Single user role, local login (bcrypt), no self-service registration.
- Basic receipt handling without automatic allocation to bin locations.
- UI/API-side validations and error messaging.

Out of scope (roadmap after MVP):
- Integrations with ERP, e-commerce, carriers, or labeling systems.
- Mobile apps (Android/iOS) and offline mode.
- Advanced warehouse processes: picking, shipping, relocations, cycle counts.
- Multi-role account system, SSO, password reset, MFA.
- Detailed change audit logs (flagged as roadmap item post-MVP).
- BI reports, analytics dashboards, bulk exports.

## 5. User Stories
1. ID: US-001  
   Title: User login  
   Description: As a warehouse operator I want to log into the application with username and password so I can access warehouse data.  
   Acceptance criteria:  
   - Login form requires username and password and validates empty fields.  
   - System compares the password with the stored bcrypt hash and rejects invalid credentials with a message.  
   - On success the user receives a session that auto-expires after 30 minutes of inactivity.  
   - Inactive accounts block login attempts and show an access denied message.

2. ID: US-002  
   Title: Add warehouse  
   Description: As an operator I want to add a new warehouse with full address details so the system can assign receipts to it.  
   Acceptance criteria:  
   - Form requires name, unique code, address, city, country, and positive capacity.  
   - Saving creates a record in the database and shows confirmation in the UI.  
   - Duplicate codes trigger an error message.  
   - The new warehouse appears in the list without a full page reload.

3. ID: US-003  
   Title: Edit or deactivate warehouse  
   Description: As an operator I want to update warehouse data or deactivate it to prevent use of outdated locations.  
   Acceptance criteria:  
   - Editing allows changing address, capacity, and active flag.  
   - Deactivated warehouses are hidden in receipt selection lists.  
   - Receipt history retains references to the warehouse (read-only).  
   - Changes are tagged in technical logs (OpenTelemetry attribute).

4. ID: US-004  
   Title: Register item  
   Description: As an operator I want to add a new item with SKU and unit so receipts can be captured.  
   Acceptance criteria:  
   - SKU is unique; required fields: name, SKU, unit.  
   - Optionally flag whether the item requires batch/lot numbers.  
   - After saving, the item is available in selection lists.  
   - Missing required fields block submission with validation errors.

5. ID: US-005  
   Title: Record receipt (reading)  
   Description: As an operator I want to register an item receipt into a warehouse so the system updates stock levels.  
   Acceptance criteria:  
   - Form requires warehouse, item, quantity > 0, date/time, and batch when the item requires one.  
   - System stores the operator and UTC timestamp.  
   - Inventory view updates immediately after saving.  
   - Errors (e.g., no permission, inactive warehouse) show a message and abort creation.

6. ID: US-006  
   Title: View warehouse inventory  
   Description: As an operator I want to see items with total quantity per warehouse so I can gauge availability quickly.  
   Acceptance criteria:  
   - View shows columns: warehouse, item, quantity, last update.  
   - Filtering by warehouse, item, and date range works without reloading the page.  
   - Data refreshes after every receipt (live update or refresh indicator).  
   - Empty results display “No readings found for selected filters.”

7. ID: US-007  
   Title: Handle invalid receipts  
   Description: As an operator I want warnings when I submit incomplete or incorrect receipt data so I can fix mistakes.  
   Acceptance criteria:  
   - Front-end and back-end validation block negative quantities, missing batch for batch-required items, or inactive warehouse/item selections.  
   - UI lists errors in Polish and preserves entered values (except sensitive fields).  
   - Invalid attempts are logged without storing sensitive content.  
   - After correcting inputs, the receipt can be saved without re-entering everything.

8. ID: US-008  
   Title: Operator activity list  
   Description: As an operator I want to view my recent receipts to verify my work.  
   Acceptance criteria:  
   - View shows the last n (configurable) receipts for the logged-in user with date, warehouse, item, quantity.  
   - Export to CSV is deferred beyond MVP (message displayed in UI).  
   - List updates after each new receipt without page reload.  
   - User can filter by warehouse and date.

## 6. Success Metrics
1. 95% of receipts recorded in the system within 2 minutes of physical intake (measured during pilot rollout).
2. At least 90% of active items and warehouses modeled in the system within 4 weeks of MVP launch.
3. No more than 1 critical error (HTTP 5xx) per 500 requests over a month.
4. Average API response time under 800 ms for receipt creation under MVP load.
5. At least 85% of pilot users report improved stock visibility compared to the previous manual process (survey).

Checklist:
- Every user story contains testable criteria, covering basic and alternate scenarios.  
- Acceptance criteria are concrete (validations, messages, data effects).  
- Stories cover all MVP interactions (login, warehouse/item CRUD, receipts, views).  
- Authentication and password storage requirements (bcrypt, no reset) are documented.  
- Change audit is flagged as a roadmap item outside MVP.  
- Document stored at .ai/prd.md for the team’s follow-up work.
