# FLM Spare Part RFID Tracking — Migration Context

## Source Project
- **Repo:** https://10.200.1.66/Development/FLM_MachineSparePartRFIDTracking
- **Local path:** `C:\ORIGINAL PROJECT TO CONVERT\FLM_MachineSparePartRFIDTracking`
- **Original language:** VB.NET / WinForms (.NET Framework)
- **New project path:** `C:\NET8 PROJECT\FLM_MAchineSparePartRFIDTracking\01. Web\FLM_MachineSparePart`
- **New GitHub:** https://github.com/Kelvinlau25/FLM_MAchineSparePartRFIDTracking
- **RFID SDK:** Symbol.RFID3 (Zebra) — legacy DLL, referenced manually (not NuGet)

---

## What This Project Does (Summary)
Tracks forklift spare parts via Zebra RFID readers across 3 physical locations.  
Records IN/OUT transactions to MSSQL, controls tower lights via GPIO, sends email notifications via DB stored procedure.

---

## RFID Readers (Physical Setup)
| IP Address   | Location           | Notes                                         |
|--------------|--------------------|-----------------------------------------------|
| 10.28.92.50  | Tower Zone         | Tag email enabled, special IN/OUT tower logic |
| 10.28.92.51  | Green Tent House 1 | Loop coil IN/OUT logic                        |
| 10.28.92.52  | Green Tent House 2 | Loop coil IN/OUT logic                        |

- GPI Port 1 = Loop Coil 1
- GPI Port 2 = Loop Coil 2
- Direction logic: both coils must agree on IN or OUT before DB update triggers

---

## Database (MSSQL)
| Stored Procedure           | Purpose                                      |
|----------------------------|----------------------------------------------|
| `SP_FILM_GET_RFID_CONFIG`  | Load reader config (IP, port, SP, server...) |
| `SP_FILM_UPDATE_TRAN`      | Record tag IN/OUT transaction (main readers) |
| Tower SP (via `db_MS_Tower`) | Record tag IN/OUT for Tower Zone           |
| `SEND_HTML_EMAIL2`         | Send email via DB                            |

### Connection Strings (confirmed from original `app.config`)
Both `db_MS` and `db_MS_Tower` point to the **same server and database** (live):



### SP Parameters
- `@pTRAN_TYPE` = "IN" or "OUT"
- `@pRFID` = tag ID string
- `@pIPADDR` = reader IP address (**not passed for Tower SP**)
- `@pRETURN_VALUE1` (OUTPUT) = "0" means success, anything else is an error message

---

## Key Business Logic (Replicated Exactly)

### Tag Detection Flow
1. RFID reader fires `ReadNotify` event
2. `GetReadTags(50)` fetches buffered tags
3. New tags added to `ConcurrentDictionary` (TagDetected + TagTime) with UTC timestamp
4. Tags older than **2 minutes** purged by `PurgeStaleTagsAsync()` every 30s

### IN/OUT Direction Detection (Loop Coil Logic)
1. GPI event fires → `HandleLoopCoilGpi()` checks which port triggered
2. `LoopCoil1` is set and 500ms debounce timer starts
3. Timer ticks → `Check2ndLoopAndUpdate()` checks `LoopCoil2`
4. If `LoopCoil1 == LoopCoil2` → confirmed direction → `StartUpdateDb(IN/OUT)`
5. Tower Zone (10.28.92.50) uses `Tower` variable → `StartUpdateDbTower(IN/OUT)` instead

### Tag Locking (Tower Zone only)
- `_lockTag` string (thread-safe with lock) prevents duplicate email for same tag
- Cleared after 30 seconds via `System.Threading.Timer`
- Only tags starting with `"E"` or `"B"` trigger tag email

### Tower Light Control
- GPO 1 = Red (error), GPO 2 = Green (success), GPO 3 = unused
- ON for **4.5 seconds** then auto-reset via `Task.Delay(4500)`
- Broadcasts `OnTowerLight` SignalR event on state change

### Email Notification Batching
- Tag emails batched in groups of **33**
- Failed emails queued and retried every 30s
- All emails via `SEND_HTML_EMAIL2` stored procedure
- Recipients: `joanna.ching.g4@mail.toray;joe.tan.x6@mail.toray;khalisah.abdulbasid.d5@mail.toray;nurfarhanah@maxsys.com.my;hammudifariz.yahaya.u9@mail.toray;izarulhisham.hassan.b3@mail.toray`

---

## Migration Goal
**Single ASP.NET Core (.NET 8) web app** — no separate WinForms exe.
- `RFIDService` runs as `BackgroundService` inside the web app
- Auto-starts and connects all readers on web app startup
- Readers keep running even when no browser is open
- Pushes real-time events to browser dashboard via SignalR (`Clients.All`)

---

## Target Architecture


---

## SignalR Events
| Hub Method       | Trigger                          | Payload                              |
|------------------|----------------------------------|--------------------------------------|
| `OnTagRead`      | New tag detected                 | hostName, tagId, location, antennaId |
| `OnReaderStatus` | Connect / Disconnect / Reconnect | hostName, status, message            |
| `OnTransaction`  | DB update (IN/OUT)               | tagId, direction, success, message   |
| `OnTowerLight`   | Tower light state change         | hostName, color (green/red/off)      |
| `OnPong`         | Browser ping response            | service, time                        |

---

## Ported Files

| Original File                            | New File                        | Notes                                      |
|------------------------------------------|---------------------------------|--------------------------------------------|
| `Form1.cs`                               | `Services/RFIDService.cs`       | Full `BackgroundService`                   |
| `Library.Database/RFID_COMMON.cs`        | Inline in `RFIDService.cs`      | `Microsoft.Data.SqlClient`                 |
| `Library.Reader/Helpers/Mailer.cs`       | Inline in `RFIDService.cs`      | Uses `SEND_HTML_EMAIL2` SP                 |
| `Library.Reader/Objects/ReaderObject.cs` | `ReaderState` inner class       | `ConcurrentDictionary` replaces Hashtable  |
| WinForms UI                              | `Views/RfidConnect/Index.cshtml`| Live dashboard via SignalR                 |

---

## Key Implementation Decisions

### `RFIDService.cs`
- Inherits `BackgroundService` — auto-starts with web app
- Registered as **Singleton** so hub and controllers share the same running instance
- Per-reader state in `ReaderState` inner class (no shared globals between readers)
- `ConcurrentDictionary` replaces all `Hashtable` usage
- `PeriodicTimer` (30s) for stale tag purge and reconnect loop
- `Task.Delay(4500)` for tower light auto-reset
- `System.Threading.Timer` (500ms debounce) for GPI loop coil logic
- Reader key in `_readers` dictionary uses `IP_ADDRESS`

### `Program.cs` — Critical Registration Pattern



### `RfidHub.cs`
- Only exposes `GetReaders()` and `Ping()`
- No connect/disconnect methods — readers auto-connect via `BackgroundService`
- No `connectionId` parameters — `RFIDService` broadcasts to `Clients.All`

---

## `appsettings.json` Configuration

\

---

## Razor View Gotchas
- Never use bare `@` inside JS strings in `.cshtml` — Razor tries to parse it (RZ1003 error)  
  ❌ `tagId + " @ " + location` → ✅ `tagId + " at " + location`
- Never call `.Replace()` inside HTML attribute quotes  
  ❌ `id="status-@item.HOST_NAME.Replace(".", "-")"` → ✅ assign to `var statusId` first
- Emoji in JS strings inside Razor can cause encoding issues — use plain text instead

---

## Startup Flow Summary



---

## Technology Choices
- **Target framework:** ASP.NET Core (.NET 8)
- **Language:** C#
- **SignalR:** Microsoft.AspNetCore.SignalR (built-in)
- **Database:** `Microsoft.Data.SqlClient` (replaces `System.Data.SqlClient`)
- **RFID SDK:** Symbol.RFID3 (same DLL, referenced manually)
- **Email:** DB stored proc `SEND_HTML_EMAIL2` (no SMTP)