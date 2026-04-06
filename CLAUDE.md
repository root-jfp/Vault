# CLAUDE.md — Vault

## Project overview

Vault is a household productivity and life-management web application. It is a standalone service that integrates with Nova (React/Fastify/SQLite voice assistant PWA) via linking — Nova links to Vault, but they are separate codebases and processes.

Vault is built for a two-person household (José and wife) with per-user data separation and no authentication — users switch via a simple profile selector (tap-to-switch, no password).

## Tech stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 8 (ASP.NET Core MVC) |
| **Views** | Razor Views (.cshtml) |
| **Database** | SQLite (via EF Core / Microsoft.EntityFrameworkCore.Sqlite) |
| **CSS** | Tailwind CSS (via CDN or CLI build) |
| **Interactivity** | HTMX (server-driven partial updates, modals, swaps) |
| **Lightweight reactivity** | Alpine.js (toggles, dropdowns, theme switcher, small UI state) |
| **Drag-and-drop** | SortableJS (kanban board reordering) |
| **Push notifications** | Web Push API (VAPID keys, service worker) |
| **PWA** | Service worker + manifest.json for installability |

### Why this stack

- ASP.NET Core MVC + Razor is José's primary learning/interview focus — building Vault reinforces that skill
- HTMX eliminates the need for a SPA framework while keeping the UI snappy (partial page swaps, no full reloads)
- Alpine.js handles small client-side state (theme toggle, dropdown menus, modal open/close) without a build step
- SortableJS is the lightest viable drag-and-drop library for kanban
- SQLite keeps deployment simple and matches Nova's DB choice — no separate DB server needed

## Architecture

```
Vault/
├── CLAUDE.md
├── Vault.sln
├── src/
│   └── Vault.Web/
│       ├── Program.cs                  # App entry, DI, middleware
│       ├── appsettings.json
│       ├── vault.db                    # SQLite database file (gitignored)
│       ├── Controllers/
│       │   ├── DashboardController.cs  # Home dashboard
│       │   ├── HabitController.cs      # Habit CRUD + tracking
│       │   ├── MaintenanceController.cs # Home maintenance items
│       │   ├── TaskController.cs       # Task/kanban management
│       │   ├── ProjectController.cs    # Project management
│       │   └── CalendarController.cs   # Calendar views
│       ├── Models/
│       │   ├── Habit.cs
│       │   ├── HabitEntry.cs           # Daily check-ins
│       │   ├── MaintenanceItem.cs
│       │   ├── MaintenanceLog.cs       # Completion history
│       │   ├── TaskItem.cs
│       │   ├── TaskColumn.cs           # Kanban columns
│       │   ├── Project.cs
│       │   ├── CalendarEvent.cs
│       │   └── UserProfile.cs          # Simple user (name, avatar color)
│       ├── Data/
│       │   ├── VaultDbContext.cs
│       │   └── Migrations/
│       ├── Services/
│       │   ├── HabitService.cs
│       │   ├── MaintenanceService.cs
│       │   ├── NotificationService.cs  # Push notification scheduling
│       │   └── UserContext.cs          # Current user from cookie/session
│       ├── Views/
│       │   ├── Shared/
│       │   │   ├── _Layout.cshtml      # Shell: sidebar, theme, Alpine init
│       │   │   ├── _Sidebar.cshtml     # Navigation partial
│       │   │   ├── _CogMenu.cshtml     # Reusable cog popup partial
│       │   │   └── _UserSwitcher.cshtml
│       │   ├── Dashboard/
│       │   │   └── Index.cshtml        # Widget grid dashboard
│       │   ├── Habit/
│       │   │   ├── Index.cshtml        # Management page (GitHub grid, stats)
│       │   │   ├── _Grid.cshtml        # HTMX partial: contribution grid
│       │   │   └── _QuickSettings.cshtml
│       │   ├── Maintenance/
│       │   │   ├── Index.cshtml
│       │   │   └── _QuickSettings.cshtml
│       │   ├── Task/
│       │   │   ├── Index.cshtml        # Full kanban board
│       │   │   └── _QuickSettings.cshtml
│       │   ├── Project/
│       │   │   ├── Index.cshtml
│       │   │   └── _QuickSettings.cshtml
│       │   └── Calendar/
│       │       ├── Index.cshtml        # Monthly + weekly views
│       │       └── _QuickSettings.cshtml
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── site.css            # Tailwind output + custom overrides
│       │   ├── js/
│       │   │   ├── app.js              # Global: theme toggle, HTMX config
│       │   │   ├── kanban.js           # SortableJS init for task board
│       │   │   └── notifications.js    # Service worker registration, push
│       │   ├── manifest.json           # PWA manifest
│       │   ├── service-worker.js
│       │   └── icons/                  # PWA icons (192, 512)
│       └── ViewModels/
│           ├── DashboardViewModel.cs   # Aggregated stats for all widgets
│           ├── HabitGridViewModel.cs   # 52-week grid data
│           └── ...
```

## Modules

### 1. Habit tracking
- **Dashboard widget**: Compact GitHub-style contribution grid (52 weeks × 7 days), today's completion count, current streak
- **Management page** (`/habits`): Full-size contribution grid per habit, habit list with CRUD, streak stats, weekly/monthly summaries
- **Data model**: `Habit` (name, frequency, color, created_at, user_id) → `HabitEntry` (habit_id, date, count, note)
- **GitHub grid logic**: Color intensity = number of completions that day across all habits (0=empty, 1-2=light, 3-4=medium, 5+=dark)

### 2. Home maintenance
- **Dashboard widget**: Next 3-5 items due, with overdue/soon/OK badges
- **Management page** (`/maintenance`): Full item list, add/edit items, completion history log
- **Data model**: `MaintenanceItem` (name, icon, interval_days, last_completed, next_due, user_id) → `MaintenanceLog` (item_id, completed_at, notes)
- **Notifications**: Push notification when item becomes due, escalate if overdue
- **Example items**: Replace toothbrush (90 days), water plants (3 days), car oil change (180 days), HVAC filter (90 days), check smoke detectors (180 days)

### 3. Task management
- **Dashboard widget**: Mini kanban with 3 columns (To Do, In Progress, Done), top 3 items per column
- **Management page** (`/tasks`): Full kanban board with drag-and-drop (SortableJS), task CRUD, due dates, priority colors, labels
- **Data model**: `TaskColumn` (name, position, user_id) → `TaskItem` (title, description, column_id, position, due_date, priority, label, user_id)
- **HTMX**: Drag events trigger `PATCH /tasks/{id}/move` to persist column + position changes

### 4. Project manager
- **Dashboard widget**: Project cards with task counts, completion percentage, days remaining
- **Management page** (`/projects`): Project list, linked tasks, progress tracking
- **Data model**: `Project` (name, description, status, deadline, user_id) → tasks linked via `TaskItem.project_id`

### 5. Daily performance overview
- **Dashboard widget**: Today's summary — habits done, tasks completed, maintenance status
- **Management page** (`/performance`): Weekly/monthly performance charts (habits completed %, tasks closed, streaks)

### 6. Weekly & monthly calendar
- **Dashboard widget**: Current week mini-calendar with event dots
- **Management page** (`/calendar`): Full month view, week view toggle, events from tasks (due dates), maintenance (next due), habits (scheduled days)
- **Data model**: `CalendarEvent` (title, date, time, type, linked_module, linked_id, user_id)

## Navigation & UX patterns

### Sidebar
- Always visible on desktop (220px)
- Collapses to icon-only (56px) on tablet/small screens
- Hamburger overlay on mobile portrait
- Supports both portrait and landscape orientations
- Contains: logo, user switcher, module nav items, settings

### Cog menu pattern (every module)
Each module card on the dashboard (and each management page header) has a cog icon button:
1. **Click cog** → Alpine.js-driven popup appears with quick settings (e.g., toggle visibility, reorder, quick add)
2. **"Manage" link** inside popup → navigates to the full management page for that module
3. Quick settings popup uses HTMX to persist changes without page reload

### User switching
- Profile pill in sidebar shows current user avatar + name
- Tap to toggle between household members
- Selection stored in a cookie (`vault_user_id`)
- All data queries filter by `user_id`

### Theme
- Light + Dark mode toggle
- Stored in `localStorage` + cookie (so server can render correct theme on first load)
- CSS custom properties for all colors
- Toggle in sidebar settings area

## Database conventions

- All tables include `id` (INTEGER PRIMARY KEY AUTOINCREMENT), `created_at`, `updated_at`
- All user-scoped tables include `user_id` (INTEGER, FK to UserProfile)
- Soft delete via `deleted_at` (nullable DATETIME) — never hard delete
- EF Core with code-first migrations
- SQLite WAL mode enabled for better concurrent read performance

## HTMX conventions

- All partial responses return HTML fragments (no JSON APIs)
- Use `hx-get`, `hx-post`, `hx-patch`, `hx-delete` on elements
- Target swaps use `hx-target` + `hx-swap="innerHTML"` or `outerHTML`
- Loading indicators via `hx-indicator`
- Modals/popups: HTMX loads partial into a modal container, Alpine.js handles open/close state
- Response headers: use `HX-Trigger` for events (e.g., refresh a widget after task update)

## PWA & push notifications

- `manifest.json` with app name "Vault", theme colors, display: standalone
- Service worker: cache-first for static assets, network-first for API/HTMX calls
- Web Push via VAPID keys (stored in appsettings)
- `NotificationService` schedules checks: runs on app startup + periodic background (via `IHostedService`)
- Notification triggers: maintenance item due/overdue, habit streak at risk (missed yesterday)

## Development workflow

```bash
# First time setup
dotnet new mvc -n Vault.Web -o src/Vault.Web
cd src/Vault.Web
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# Install Tailwind CSS (CLI)
npx tailwindcss init
# Configure tailwind.config.js to scan .cshtml files

# Run dev
dotnet watch run

# Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Coding standards

- C# naming: PascalCase for classes/methods/properties, camelCase for local variables
- Razor views: use ViewModels, never pass raw EF entities to views
- One controller per module, keep controllers thin — business logic in Services
- HTMX partials: prefix with underscore (`_Grid.cshtml`, `_QuickSettings.cshtml`)
- Alpine.js: use `x-data` on the nearest relevant container, not on `<body>`
- Tailwind: use utility classes in views, extract to `@apply` in site.css only for heavily repeated patterns
- All user-facing text in English (no i18n needed for household app)
- All dates stored as UTC, displayed in local timezone (Portugal/Lisbon)

## Integration with Nova

- Nova links to Vault via a configurable URL in Nova's sidebar/menu
- No shared auth, no shared database, no API calls between them
- Both are separate processes — can run on same machine on different ports
- Future possibility: Nova voice commands trigger Vault actions via internal API (not in MVP)

## Out of scope for MVP

- Multi-household / multi-tenant
- Authentication / authorization
- Data export / import
- Mobile native app
- Nova integration beyond linking
- Recurring task templates
- File attachments on tasks
- Comments / collaboration features
