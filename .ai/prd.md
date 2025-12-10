# Product Requirements Document (PRD) - AntLogisticSolution

## 1. Product Overview
AntLogistics WMS MVP is a web application that lets warehouse teams record foundational data about warehouses, items, and goods readings. The solution is built on .NET 9 with .NET Aspire (AppHost, Core API, React/Vite UI) and uses PostgreSQL plus Entity Framework Core. The MVP delivers a simple browser interface where a single authenticated operator can manage master data and capture and save inventory readings. This minimal release focuses on intake processing and displaying current stock levels, enabling quick rollout and laying the groundwork for future modules (orders, reporting, integrations).

## 2. User Problem
Warehouse teams currently track stock movements manually (spreadsheets, email), which leads to errors, stale data, and poor visibility. Operators lack a single source of truth about locations, cannot quickly register readings, and logistics decisions rely on incomplete information. Basic access control to warehouse data is also missing. The MVP addresses these issues by providing a simplified WMS that supports adding warehouses and items, registering readings, and browsing up-to-date inventory once logged in.

## 3. Functional Requirements
1. Warehouse registration: name, code, address, active status, capacity, default zone.
2. Item definition: name, SKU, unit of measure, control parameters (e.g., batch/lot requirement), active status.
3. Receipt (reading) entry for a selected warehouse with item, quantity, batch (if required), operator, and timestamp.
4. UI data validation (required fields, numeric ranges, code uniqueness) with error messages in Polish.
5. List views for warehouses, items, and receipt history with basic filters (name/code, date range).
6. Inventory status calculation based directly on aggregated readings. Each reading entry automatically updates the current stock level for the specific warehouse and item (no manual adjustments in MVP).
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
Feature: Core warehouse operations in AntLogistics WMS MVP

Scenario: US-001 User login
   Given a warehouse operator is on the login form
   And both the username and password fields are required
   When the operator submits credentials
   Then the system validates that neither field is empty
   And the submitted password is compared against the stored bcrypt hash
   And a valid login issues a session that auto-expires after 30 minutes of inactivity
   And an inactive account blocks the login attempt and displays an access denied message

Scenario: US-002 Add warehouse
   Given the operator is on the add warehouse form
   And the form collects name, unique code, address, city, country, and positive capacity
   When the operator submits the warehouse details
   Then the system creates the warehouse record and shows a confirmation in the UI
   And the new warehouse appears in selection lists without a full page reload
   And entering a duplicate code triggers an error message and prevents creation

Scenario: US-003 Edit or deactivate warehouse
   Given an existing warehouse with editable fields (address, capacity, active flag)
   When the operator updates those fields or toggles activity
   Then the changes are persisted and tagged in OpenTelemetry logs
   And deactivated warehouses are hidden in receipt selection lists
   And historical readings retain read-only references to the warehouse

Scenario: US-004 Register item
   Given the operator is on the add item form with required fields name, SKU, unit
   And SKU values must be unique
   When the operator submits the item and optionally flags batch requirements
   Then the system validates required fields and rejects incomplete input with errors
   And a valid item becomes available in all relevant selection lists

Scenario: US-005 Record receipt
   Given the operator is on the receipt form with warehouse, item, quantity, date/time, and optional batch fields
   And quantity must be greater than zero and batch is mandatory for batch-tracked items
   When the operator submits the receipt
   Then the system stores the operator identity and UTC timestamp
   And the system immediately recalculates the inventory status by adding the reading quantity to the current stock
   And invalid conditions (no permission, inactive warehouse) show an error and cancel creation

Scenario: US-006 View warehouse inventory
   Given the operator opens the inventory view
   When the operator applies filters for warehouse, item, or date range
   Then the view displays the calculated inventory status derived from the sum of all valid readings
   And the view shows columns warehouse, item, total quantity, last update without reloading the page
   And data refreshes after each receipt to reflect the new inventory status
   And empty results display “No readings found for selected filters.”

Scenario: US-007 Handle invalid readings
   Given the receipt form enforces validations on quantity, batch, and active selections
   When the operator submits incomplete or incorrect data
   Then front-end and back-end validation block the submission
   And the UI lists errors in Polish while preserving previously entered values (except sensitive fields)
   And the invalid attempt is logged without sensitive payloads
   And once corrected, the operator can resubmit without re-entering all details

Scenario: US-008 Operator activity list
   Given the operator opens the activity list view
   When the system fetches the last n readings for the logged-in user
   Then the view shows date, warehouse, item, and quantity for each reading
   And the list updates automatically after new readings without a full reload
   And filters for warehouse and date limit the results
   And the UI indicates that CSV export is deferred beyond MVP

## 6. Success Metrics
1. 95% of readings recorded in the system within 2 minutes of physical intake (measured during pilot rollout).
2. At least 90% of active items and warehouses modeled in the system within 4 weeks of MVP launch.
3. No more than 1 critical error (HTTP 5xx) per 500 requests over a month.
4. Average API response time under 800 ms for receipt creation under MVP load.
5. At least 25% of pilot users report improved stock visibility compared to the previous manual process (survey).

Checklist:
- Every user story contains testable criteria, covering basic and alternate scenarios.  
- Acceptance criteria are concrete (validations, messages, data effects).  
- Stories cover all MVP interactions (login, warehouse/item CRUD, readings, views).  
- Authentication and password storage requirements (bcrypt, no reset) are documented.  
- Change audit is flagged as a roadmap item outside MVP.  
- Document stored at .ai/prd.md for the team’s follow-up work.
