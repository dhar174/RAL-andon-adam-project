## Architecture and Code Flow

- **Entry point & configuration**  
  - `RAL.EntryPoint/Program.cs` boots the service, compiles a user-provided `UserConfiguration.cs` script via CSScript, runs `UserConfigLoad()` and `Validate()`, then delegates to `Configuration` to wire the system.  
  - `RAL.EntryPoint/Configuration.cs` builds Serilog pipelines, constructs `MachineRepositoryCache`, creates device clients (`Adam6051Client`), stacks rules and reports, and feeds them into `ManagerFactory`.

- **Orchestration (Manager layer)**  
  - `ManagerFactory` aggregates machines, rules, and reports, then builds a `Manager` with scoped loggers for collectors, rules, and reports.  
  - `Manager.Start()` spins up `CollectorManager`, `RulesManager.StartAsync()`, and `ReportsManager.Start()` in sequence; `WaitForClose()` waits on collector and rules tasks.

- **Data collection**  
  - Each `Machine` wraps device metadata and an `Adam6051Client`. Status messages are translated via `IMachineStatusPayloadConverter` and persisted to Influx through `IMachineRepository`.  
  - `CollectorManager` builds MQTT clients from machines, instantiates `MessageProcessor`, and starts `MQTTCollector` to feed messages into a blocking queue.
  - `MessageProcessor` dispatches queued `MQTTMessage` payloads to topic handlers (`ITopicMessageHandlerForProcessor`) registered per client.

- **Rules processing**  
  - `RulesManager` (DAL.Andon) hosts `IRule` instances (e.g., `RuleMachineIsRunning`) and starts them concurrently. Rules can trigger actions such as updating repositories or commanding stack lights.

- **Reporting**  
  - `ReportsManager` (Scheduler) wraps FluentScheduler jobs. `Configuration` adds hourly and shift reports (`DepartmentTimePeriodReportForCurrentDay`) with schedules derived from user config, then `ReportsManager.Start()` starts the scheduler.

- **Persistence**  
  - `MachineRepository` and related repositories (InfluxDB via Vibrant.InfluxDB) persist status, connectivity, and “is running” metrics. `Machine` writes status upon changes; rules write derived metrics (e.g., `RuleActionWriteIsRunning`).

- **Devices & UI**  
  - Device abstractions live in `RAL.Devices.*` and stack light integrations in `RAL.Devices.StackLights`. Real-time UI (console/WPF) projects consume view models in `RAL.RealTime.*` (not wired in the main entrypoint flow).

## Key Gaps / Risks (with evidence and recommendations)

1) **Shutdown leaves scheduler running**  
   - Evidence: `Manager.WaitForClose()` awaits collector and rules only, never stops `ReportsManager`/FluentScheduler (`RAL.Manager/Manager.cs`).  
   - Impact: Background report jobs continue after shutdown signals, risking orphaned tasks and process hangs.  
   - Recommendation: Add a close/stop path for `ReportsManager` (or `JobManager.StopAndBlock()`) and wait on it inside `WaitForClose()`.

2) **Status change detection ignores most state**  
   - Evidence: `MachineStatusMessage` defines `IsCycling`, `IsInAutomatic`, and `IsFaulted` (`RAL.Collector/MachineStatusMessage.cs`), but `Machine.StatusIsChanged` only compares `IsCycling` (`RAL.Manager/Machine.cs`).  
   - Impact: Fault/automation changes are never persisted; downstream rules and reports get stale data, undermining reliability of KPIs and alerts.  
   - Recommendation: Expand change detection to all relevant fields (faulted/auto/connectivity), or persist every sample with deduplication at the repository layer.

3) **Async void event handler drops exceptions**  
   - Evidence: `_adam6051PressClient_StatusReceived` is `async void` and performs repository writes (`RAL.Manager/Machine.cs`).  
   - Impact: Exceptions are unobserved and can terminate the process; failed writes are silently ignored except for logging, making reliability dependent on log monitoring.  
   - Recommendation: Change to `async Task` handlers (or queue work) and await in the event pipeline; add retry/backoff around repository writes.

4) **Report scheduler depends on unvalidated required config**  
   - Evidence: `LoadConfiguration` only guards against `EmailReportConfigs` being null and otherwise schedules hourly/shift reports for every department without checking for emptiness, duplicates, or invalid time windows (`RAL.EntryPoint/Configuration.cs`).  
   - Impact: Misconfigured user scripts can crash startup or schedule overlapping jobs; hard-coded start times make timezone/shift changes brittle.  
   - Recommendation: Validate report configs for completeness, uniqueness, and time windows before scheduling; surface user-friendly errors and allow configurable start windows.

5) **Hard-coded database defaults and credentials**  
   - Evidence: Fallback DB connection uses baked-in IP/username/password when user config omits values (`RAL.EntryPoint/Configuration.cs`).  
   - Impact: Security exposure (plaintext credentials) and accidental connection to production-like endpoints if user config is incomplete.  
   - Recommendation: Require explicit database configuration; if missing, fail fast with clear errors and move secrets to secure configuration (env/KeyVault).

6) **Test suite currently blocked by missing runtime**  
   - Evidence: `dotnet test RAL.Solution.sln` fails because `Microsoft.NETCore.App` 2.1/2.2 is not installed. NU1902/NU1903 warnings reference GHSA advisories including GHSA-6px8-22w5-w334 and GHSA-g5vf-38cp-4px9 for Microsoft.NETCore.App, and GHSA-5crp-9r3c-p9vr for Newtonsoft.Json 12.0.1.  
   - Impact: CI/CD cannot validate regressions; vulnerability warnings (NU1902/NU1903) remain unaddressed.  
   - Recommendation: Modernize target frameworks (>= net6.0), or install the required runtimes in CI; update vulnerable dependencies (e.g., Newtonsoft.Json 12.0.1).

## Test Notes
- `dotnet test RAL.Solution.sln` (in repo root) aborts: missing runtime `Microsoft.NETCore.App` 2.1/2.2; multiple NU1902/NU1903 vulnerability warnings observed.
