# CefGlue Internals & Development Notes

Companion to [CLAUDE.md](CLAUDE.md). Deep reference for mechanisms, quirks, and maintenance procedures that span multiple files. Accurate as of version `120.6099.211` (CEF 120.1.8, main branch).

## CEF upgrade playbook

> **The full, evidence-based upgrade process lives in [CEF-UPGRADE.md](CEF-UPGRADE.md)** — including version selection, redist publishing prerequisites, generator breakpoint fixes per CEF version, the validation checklist derived from historical regressions, the CEF-134 branch state, and automation notes. The summary below covers only the in-repo mechanical core.

Upgrading CEF touches every layer. The steps, in order:

1. Bump `CefRedistVersion` / `CefRedistOSXVersion` / `CefRedistLinuxVersion` and `Version` in `Directory.Build.props` (version scheme: `<chromium major>.<chromium build, i.e. the CEF branch>.<fork revision>` — e.g. `120.6099.211` pairs with CEF `120.1.8` / Chromium `120.0.6099.109`).
2. Replace the vendored CEF headers in `CefGlue.Interop.Gen\include\` with the new CEF version's headers.
3. Add a `classdef` entry in `CefGlue.Interop.Gen\schema_cef3.py` for every **new** CEF class (role `ROLE_PROXY` or `ROLE_HANDLER`, optional `name`/`reversible`/`autodispose`/`abstract`) — the generator raises `Class role must be defined` otherwise.
4. Run `CefGlue.Interop.Gen\gen-cef3.cmd` (Windows) or `gen-cef3.sh` (POSIX). This regenerates `CefGlue\Classes.g\`, `CefGlue\Interop\Classes.g\`, `Interop\libcef.g.cs`, `Interop\version.g.cs`, and non-compiled `*.tmpl.g.cs` scaffolds under `CefGlue.Interop.Gen\Classes.{Proxies,Handlers}.tmpl\`.
5. Hand-port new/changed methods into `Classes.Proxies\` / `Classes.Handlers\` using the regenerated `.tmpl.g.cs` scaffolds as reference (they are `NotImplementedException` stubs with xmldoc; not compiled).
6. **Manually diff the hand-written mirrors** — the generator does NOT update `Interop\Structs\` (e.g. `cef_settings_t`, `cef_window_info_t` with its three per-platform layouts), `Interop\Base\`, or `Enums\`. Field-layout drift here causes memory corruption at runtime, not compile errors.
7. Remove or update the DEBUG-only guard in `CefGlue.Common\CefRuntimeLoader.cs` that throws `"Remove this fix block after CEF upgrade"` when the CEF major version ≠ 120 (tied to the `disable-features=FirstPartySets` YouTube-crash workaround for CEF issue 3643).
8. Precedent commits: `ffa89aa` (CEF 117→120), `6301718` (→117). A CEF 134 bump is in flight on branch `RDEV-8412-bump-cef-to-134.3.9`.

Generator internals: Python 3, entry `cefglue_interop_gen.py`; `cef_parser.py` is CEF's own BSD-licensed header parser; `make_interop.py` does emission (generated-code bugs must be fixed there, then regenerated); `schema.py` maps C→C# types. Excluded from parsing: `cef_application_mac.h`, `cef_version.h`, `cef_thread.h`, `cef_waitable_event.h`.

## Initialization & lifecycle

- `CefRuntimeLoader.Initialize(settings, flags, customSchemes)` is **deferred**: it only stores a lambda. Real initialization (`CefRuntime.Load` + `CefRuntime.Initialize`) runs on the first `BaseCefBrowser` construction via `CefRuntimeLoader.Load()` (internal). `IsLoaded` stays false until then.
- Per-platform settings applied at load (`CefRuntimeLoader.cs`): Windows → `MultiThreadedMessageLoop`; macOS → `ExternalMessagePump` + bundle paths + `NoSandbox`; Linux → `NoSandbox` + `MultiThreadedMessageLoop` (the `no-zygote` switch is appended separately in `BrowserCefApp.OnBeforeCommandLineProcessing`, browser process only). `UncaughtExceptionStackSize` is hard-coded to 100.
- Subprocess probing order (`CefRuntimeLoader.GetSubProcessPaths`): `<AppContext.BaseDirectory>\CefGlueBrowserProcess\Xilium.CefGlue.BrowserProcess[.exe]`, then flat next to the app, then the same two relative to the executing assembly's directory (plugin scenario). Missing everywhere → `FileNotFoundException` listing all probed paths.
- macOS message pump: `AvaloniaCefBrowser`'s **static** ctor registers `AvaloniaBrowserProcessHandler` (Rx `Observable.Interval` on `AvaloniaScheduler` driving `CefRuntime.DoMessageLoopWork`) — only when `CefRuntime.Platform == MacOS`. WPF has no equivalent.
- Subprocess side (`CefGlue.BrowserProcess\Program.cs`): installs a `ResolvingUnmanagedDll` hook resolving natives from the parent directory, starts `ParentProcessMonitor` (waits on the `--parent-pid` process, then `Environment.Exit(0)` after a 10 s grace — expect briefly lingering subprocesses after killing the host), parses `--custom-scheme`, and calls `CefRuntime.ExecuteProcess` with `RendererCefApp`. DEBUG builds call `Debugger.Launch()` on startup exceptions on Windows — looks like a hang.
- Shutdown: `CefRuntime.Shutdown()` is hooked on `ProcessExit` by `CefRuntimeLoader`.
- `Address` set before the browser is created is stashed in `_initialUrl` and loaded at `OnBrowserCreated`; navigation is marshaled through `CefThreadId.UI` because loading before creation aborts navigation.
- `DoClose`/`OnBrowserClose` return values are platform-sensitive: windowed returns `true` except on Linux and for popups; OSR always returns `false` (CEF destroys immediately). Changing this breaks close/dispose semantics.

## Renderer IPC in detail

### Process messages
All host↔renderer traffic uses `CefProcessMessage` via `frame.SendProcessMessage`, dispatched by name through `MessageDispatcher` (name → handler dictionary). Contracts are structs in `CefGlue.Common.Shared\RendererProcessCommunication\Messages.cs` (message name == struct name):

| Message | Direction | Correlation |
|---|---|---|
| `JsEvaluationRequest` / `JsEvaluationResult` | host → renderer / back | `TaskId` |
| `NativeObjectRegistrationRequest` / `NativeObjectUnregistrationRequest` | host → renderer | object name |
| `NativeObjectCallRequest` / `NativeObjectCallResult` | renderer → host / back | `CallId` |
| `JsContextCreated` / `JsContextReleased` / `JsUncaughtException` | renderer → host | — |
| `UnhandledException` | renderer → host | — |

`CommonCefClient.OnProcessMessageReceived` disposes the message (`using`) before calling `base.OnProcessMessageReceived` — handlers must materialize all data eagerly during dispatch (the `FromCefMessage` methods do).

### Crash pipe (fallback channel)
For .NET crashes in the render process when process messages can't be used: `CommonBrowserAdapter.CreateBrowser` generates a GUID pipe name and passes it in the browser `extraInfo` dictionary under key `CrashPipeName`; the renderer captures it in `OnBrowserCreated` and writes an XML-serialized `SerializableException` via `PipeClient` (10 s connect timeout). Host listens with `PipeServer` (one connection at a time; the accept loop tolerates 5 errors total over its lifetime — the count is never reset on success — and gives up permanently on the 6th) and raises the `UnhandledException` browser event. Wire format: 4-byte little-endian length prefix + UTF-16 payload — both sides must match exactly. Commit `37fb0f3` handles `extraInfo == null`.

## JS interop internals

### Object binding
- `RegisterJavascriptObject(targetObject, name, methodHandler)` reflects **public instance methods only** (no properties; `IsSpecialName` filtered), keys them by camelCase (first char lowered). Registrations are (re)sent to the renderer **main frame** on browser creation — iframes never receive bound objects (known limitation, TODO in `NativeObjectRegistry`).
- Renderer creates one `CefV8Value` function per method (backed by `V8FunctionHandler`) and exposes the object on `window` wrapped in a JS `Proxy` (`createInterceptor` in `CefGlueGlobalScript.js`). The Proxy serializes **all call arguments into a single JSON string**; `V8FunctionHandler` rejects calls with more than one raw argument.
- Calls return a V8 Promise (`PromiseHolder`), resolved when `NativeObjectCallResult` arrives. Pending promises are dropped on context release.
- Host-side dispatch (`NativeObjectMethodDispatcher`): deserializes the JSON args against the target method's parameter types (supports trailing `params` arrays), invokes via reflection, awaits `Task` results via `GenericTaskAwaiter`; an un-awaited raw `Task` result produces the error `Unexpected Task type result`. The optional `MethodCallHandler` delegate (`object MethodCallHandler(Func<object> originalFunction)`) intercepts the invocation.
- `CefGlueGlobalScript.js` is an embedded resource registered as a V8 extension named `cefglue` (`CefRuntime.RegisterExtension`), with `native function Bind()/Unbind()` implemented by `V8BuiltinFunctionHandler` (used by `cefglue.checkObjectBound` / `deleteObjectBound`).

### Evaluation
- `EvaluateJavaScript<T>` sends `JsEvaluationRequest`; the renderer wraps the script as `cefglue.evaluateScript(function() {<script>\n})` — the injected `\n` protects against a trailing `//` comment swallowing the closing brace.
- Timeout semantics (`JavascriptExecutionEngine.ProcessResult`): a timeout **throws `TaskCanceledException`**; `default(T)` is returned only when the pending task was cancelled out-of-band (e.g. engine dispose), which surfaces as `AggregateException` wrapping `TaskCanceledException`.
- `lastTaskId` (host) and `lastCallId` (renderer) are `static volatile int` with non-atomic `++` — concurrent calls can theoretically collide (renderer throws `Call id already exists`).

### Serialization format
Not plain JSON — both sides must stay in sync (`CefGlue.Common.Shared\Serialization` ↔ the stringifier/reviver in `CefGlueGlobalScript.js`):
- One-char type-marker prefixes on strings: `S` = string, `D` = DateTime, `B` = base64 binary (`DataMarkers.cs`).
- Object graphs use `$id`/`$ref`/`$values` (`ReferenceHandler.Preserve`) — cyclic objects round-trip.
- Serializer: System.Text.Json, `IncludeFields=true`, `MaxDepth=int.MaxValue`. Deserializer: hand-written stack machine over `Utf8JsonReader` (`Serialization\State\`), able to deserialize a JSON argument array against a heterogeneous `Type[]` (method dispatch).
- Never swap in a stock JSON serializer on one side only.

## Per-browser customization surface

- **Handler properties — two-tier delegation footgun.** `BaseCefBrowser` exposes 13 settable handler properties backed by public wrapper classes in `CefGlue.Common\Handlers\Handlers.cs`. Two different mechanisms are in play:
  - For `DialogHandler`, `DownloadHandler`, `DragHandler`, `FindHandler`, `FocusHandler`, `KeyboardHandler`, `RequestHandler`, `JSDialogHandler`: `CommonCefClient` hands the user's handler **directly** to CEF — every virtual override works. A null `DownloadHandler` means downloads are disallowed; the `RequestHandler` base deliberately nulls out `GetResourceRequestHandler`, so resource interception requires overriding it.
  - For `ContextMenuHandler`, `DisplayHandler`, `LifeSpanHandler` (and the internal Load/Frame/Render handlers): `CommonCefClient` **always** installs its internal `CommonCef*Handler` and only forwards callbacks that have an explicit `internal Handle*` bridge in `Handlers.cs`. Overriding a non-bridged virtual (e.g. `DisplayHandler.OnCursorChange`) silently does nothing. Supporting a new callback requires adding both a bridge method in `Handlers.cs` and a delegation call in the matching `InternalHandlers\CommonCef*Handler.cs`. Some bridged results are composed with internal behavior (`DoClose`, `OnBeforePopup`, `OnTooltip`).
- **Per-browser `CefRequestContext`.** `BaseCefBrowser` / `AvaloniaCefBrowser` / `WpfCefBrowser` accept an optional `Func<CefRequestContext> cefRequestContextFactory` constructor argument (isolated cache/cookies/session per browser); the resulting `RequestContext` is exposed publicly. This is the main per-browser knob, given that OSR-vs-windowed is process-wide. See the `CustomRequestContext` tests for usage.
- **`DefaultResourceHandler`** (`CefGlue.Common\Handlers\DefaultResourceHandler.cs`) is the intended base class for custom-scheme/resource responses — subclass and set `Response` (stream), `MimeType`, `Status`/`StatusText`, `Headers`, `RedirectUrl` or `ErrorCode`. For async production, override `ProcessRequestAsync` and return `RequestHandlingFashion.ContinueAsync`, completing the `CefCallback` later. Contracts: the `Response` stream may be shared across handler instances (reads are lock-synchronized with a per-handler position), `Response` cannot be set after reading starts, and non-seekable streams fail `Skip`. Writing a raw `CefResourceHandler` by hand is easy to get wrong — prefer this base.
- **Chromium switches via `CefRuntimeLoader.Initialize(flags:)`.** The `KeyValuePair<string,string>[] flags` parameter is the supported way to feed Chromium command-line switches (same channel the FirstPartySets workaround uses). `BrowserCefApp.OnBeforeCommandLineProcessing` appends them **only when `processType` is empty — i.e. the browser process only**; renderer/GPU subprocesses receive just `--custom-scheme` and `--parent-pid`. If a switch "doesn't work", check which process needed it. The DEBUG-only CEF-version guard lives in this same code path.

## CefObjectTracker (native-wrapper disposal)

Fork-specific memory-management subsystem spanning the generator, the core binding, and both processes — relevant to leak investigations (see branch `RDEV-8017-reproduce-memory-leak`):

- `make_interop.py` emits `CefObjectTracker.Track(this)` / `Untrack(this)` into every generated PROXY constructor/`Dispose`, so all generated wrappers participate.
- `CefObjectTracker` is `[ThreadStatic]`: `using (CefObjectTracker.StartTracking()) { ... }` opens a session; everything tracked inside the scope is bulk-disposed when the outermost session ends (nested `StartTracking` returns a no-op session).
- Sessions are opened at callback entry points on both sides: `CefV8Handler.execute` (every V8 function call in the renderer), `RenderProcessHandler` (several sites), `V8BuiltinFunctionHandler`, and `NativeObjectMethodDispatcher` on the host.
- Adding a new callback path that creates CEF proxies without opening a tracking session leaks native references; manually disposing tracked objects interacts with `Untrack`.

## Offscreen rendering (OSR)

- Process-wide mode: `CefSettings.WindowlessRenderingEnabled` at init → `CefRuntimeLoader.IsOSREnabled` → `BaseCefBrowser` picks `CommonOffscreenBrowserAdapter` vs `CommonBrowserAdapter`. OSR also appends `disable-gpu`, `disable-gpu-compositing`, `enable-begin-frame-scheduling`, `disable-smooth-scrolling` (`BrowserCefApp`).
- Paint path: `CommonCefRenderHandler.OnPaint` → `OffScreenRenderSurface.Render` → copy CEF's buffer into a `MemoryMappedFile` view → marshal to UI thread → `CreateBitmap`/`UpdateBitmap` (per-toolkit `WriteableBitmap`). Frames with mismatched size are skipped.
- Workarounds baked into `CommonOffscreenBrowserAdapter`: on becoming visible it fakes a 1px-taller `GetViewRect` for ~50 ms and calls `WasResized` twice (CEF issue 2483 — invalidate doesn't generate a frame); `GetViewRect` clamps to min 1×1; `NotifyScreenInfoChanged` is commented out (old Chromium SurfaceSync crash).
- Avalonia surface: `Bgra8888`, `AllowsTransparency=false` (TODO), fixed 96 DPI, copies the **entire** frame each paint ignoring dirty rects. WPF surface: `Bgra32`, transparency enabled, DPI-scaled, writes only dirty rects. Avalonia hardcodes mouse wheel delta to 100.
- IME composition is **not implemented** (`OnImeCompositionRangeChanged` is an empty override); text input becomes one `CefKeyEventType.Char` event per character — complex-script input behaves poorly in OSR.
- Popup widgets (HTML `<select>` etc.) render into a separate `IOffScreenPopupHost`: Avalonia uses a borderless topmost `Window` positioned in screen coordinates; WPF uses the real `Popup` primitive.

## Windowed rendering platform tricks

- **Windows (Avalonia)**: all browsers parent to a single static, never-disposed hidden Avalonia `Window` — intentional, prevents CEF crashes when a hosting window closes mid-creation (`AvaloniaControl`).
- **Linux (Avalonia)**: a depth-24 X11 window is created via P/Invoke into `libX11.so.6` (`Platform\Linux\XWindow.cs`) because Avalonia's own window is depth-32 and CEF uses a `CopyFromParent` colormap.
- **macOS (Avalonia)**: an `NSView` is allocated through raw `objc_msgSend` P/Invokes (`Platform\MacOS\NSView.cs`); native-view positioning relies on a 500 ms `DispatcherTimer` re-running `TryUpdateNativeControlPosition` (`ExtendedAvaloniaNativeControlHost`) — resizes can visually lag.
- **WPF**: parent HWND from `WindowInteropHelper`; CEF's window embedded via `HwndHost` subclass. `WpfControl.InitializeRender` casts the host to `ContentControl` — any WPF host must derive from it. `DestroyRender` P/Invokes `DestroyWindow` on the calling thread.
- DevTools: `SetAsPopup` with an owner handle only on Windows; other platforms open unparented.

## Known quirks & footguns (inventory)

- `Messages.NativeObjectUnregistrationRequest.FromCefMessage` is declared to return `NativeObjectRegistrationRequest` (the wrong type) and constructs one — long-standing quirk in `Messages.cs`.
- Generated `FromNativeOrNull` in HANDLER classes dereferences the looked-up value before checking `found` — a lookup miss would NRE. This is generator-emitted; fix in `make_interop.py`, not in `.g.cs` files.
- `CefGlue.Common.csproj` carries a vestigial `<Compile Remove="CommonBrowserBehaviors.cs" />` entry for a file that no longer exists — the csproj entry is the leftover. Only `BaseCefBrowser.cs` is the shared-source file.
- `CEF_CALLBACK` uses `CallingConvention.Winapi` (not Cdecl) — historical CEF issue 598; do not "fix" it.
- `Interop\Base\cef_string_t.disabled.cs` is intentionally excluded; the active implementation is `cef_string_t.cs`.
- `CefGlue\Wrapper\**` (MessageRouter) exists but is excluded from compilation — editing it does nothing.
- `CefRuntime.Load(path)` supports an explicit path on Windows only (`LoadLibraryEx` + `LOAD_WITH_ALTERED_SEARCH_PATH`); Linux relies on a recursive `libcef.so` search under the app base directory during the API-hash check.
- `CefGlue.WPF` sets `AllowUnsafeBlocks` only in Debug configurations, not Release; `CefGlue.Avalonia` sets it unconditionally.
- `GenerateAssemblyInfo=false` on `CefGlue.WPF` (AssemblyVersion pinned 1.0.0.0) and `CefGlue.BrowserProcess` — assembly versions do not track the package version.
- The default for every `PackageReference` is `PrivateAssets=compile` (set via `ItemDefinitionGroup` in `Directory.Packages.props`, killing transitive compile assets); CEF redist references that must flow to consumers override with `PrivateAssets="None"`.
- `DefaultResourceHandler` adds `Access-Control-Allow-Origin: *` to custom-scheme responses.
- Custom schemes propagate to subprocesses as `--custom-scheme name|domain|optionsInt;...` plus `--parent-pid` (`CommonBrowserProcessHandler.OnBeforeChildProcessLaunch`); defaults are Standard+Secure+CorsEnabled+FetchEnabled.
- Tooltips: Avalonia uses 0 show-delay attached props with a known hanging-tooltip TODO; WPF uses a dedicated `ToolTip` + 0.5 s timer.
- Drag & drop: WPF sets Text/UnicodeText/Html on the `DataObject`, Avalonia only plain text; `UpdateDragCursor` is a no-op on WPF.
- Both OSR hosts mark Tab/Home/End/arrow `KeyDown` as handled so focus/keyboard navigation can't escape the browser.

## Build & packaging details

- `CefGlue.BrowserProcess`'s `PublishApp` target (AfterBuild) re-invokes MSBuild `Publish` for `win/linux/osx-$(ArchitectureConfig)` with `PublishTrimmed=True;SelfContained=True`, but only when the assembly was actually recompiled — **incremental builds can pack stale or missing BrowserProcess binaries without error** (NU5100/NU5118 are in `NoWarn`). When packaging misbehaves, rebuild `CefGlue.BrowserProcess` explicitly.
- The 8 MiB stack fix (`editbin /STACK:0x800000`, CEF-recommended for renderer recursion, issues #171/#172) runs only on Windows **and** only when `/p:VcvarsFile="...\vcvars64.bat"` is passed — normal builds silently produce a 1 MiB-stack subprocess exe.
- The `ProjectReference` from `CefGlue.Common` to `CefGlue.BrowserProcess` has `ReferenceOutputAssembly=false` — it exists purely for build ordering ahead of the `CopyProjectReferencesToPackage` pack target.
- Consumer-side logic ships inside the `CefGlue.Common` package (`build/` + `buildTransitive/`): copies the subprocess into consumers' output under `CefGlueBrowserProcess\` and copies CEF natives — on Windows via the `ResolveCEFAssets` target filtering `chromiumembeddedframework.runtime.win-*` package assets (+ locale item lists), on macOS/Linux via `@(CefRedistOSX*)`/`@(CefRedistLinux*)` items defined by the `cef.redist.*` packages' own props. In-repo, `CefGlue.CopyLocal.props` + `CefGlue.Common\build\CefGlue.Common.targets` do the same for Tests and the demos.
- `build\setenv.cmd` (legacy) hardcodes VS2019 Professional MSBuild x86 onto PATH — the last survivor of the deleted `build.cmd`/`build.proj` chain.
- Optional NLog logging compiles in only when `DefineConstants` contains `HAS_NLOG` (regex conditions in `Directory.Packages.props` and `CefGlue.Common.csproj`); otherwise a `NullLogger` is used.
- `Directory.Build.rsp` is intentionally blank (avoids accidental response-file imports).
- Sandcastle docs project `Xilium.CefGlue.shfbproj` is legacy; requires SHFBROOT.
- `normalize-line-endings.cmd` runs `tools\xifixeol.exe`: CRLF/UTF-8 for `*.cs`/`*.csproj`, CRLF/ASCII for `*.h`/`*.py`.

## Test infrastructure

- `TestBase.cs`: `[OneTimeSetUp]` initializes CEF once per process (static guard) with a custom `test` scheme, then starts one background thread running Avalonia's `Dispatcher.UIThread.MainLoop` forever (`SetupWithoutStarting`). Each `[SetUp]` creates a real `AvaloniaCefBrowser`, hosts it in a shown 1×1 Avalonia `Window` (created lazily on the fixture's first test and reused until `[OneTimeTearDown]`), and awaits `BrowserInitialized`.
- Test content is loaded via base64 `data:` URLs (`BrowserExtensions.LoadContent`), awaited on `LoadEnd`/`LoadError`.
- ~110 tests in 10 files. Browser-based: Events (14), JavascriptEvaluation (19), NativeObjectInterop (19, re-run by an async-interceptor subclass), Network (4), CustomRequestContext (3, overrides Setup to pass a custom `CefRequestContext` factory), CustomSchemes (1), State (1). Pure unit (no browser): Serialization (41), NativeObjectMethodExecutor (6), NativeObjectIntrospection (2).
- `[assembly: Timeout(30000)]` applies only when `DEBUG` is **not** defined — Debug runs have no per-test timeout and hang forever on a stuck test.
- `SerializationTests`/`NativeObjectMethodExecutorTests` use Newtonsoft.Json **transitively** (via the old Microsoft.NET.Test.Sdk 16.2.0 chain, no direct PackageReference) — upgrading Test.Sdk can break their compilation.
- `CefGlue.Tests\publish.cmd <Configuration>` produces a self-contained win-x64 test drop.
- Despite the name, `-c Debug_WindowlessRender` does **not** make tests run OSR — the `WINDOWLESS` constant is only defined for the two demo projects; `TestBase` passes no `CefSettings`.

## Demos

- `CefGlue.Demo.Avalonia` is the full-featured demo: tabbed browser, unique per-run cache dir `%TEMP%\CefGlue_<guid>` (deleted after shutdown), object binding before load (`boundBeforeLoadObject`) and on demand (`dotNetObject` with async interceptor), `EvaluateJavaScript<T>` round-trips, DevTools, popup re-hosting via a custom `LifeSpanHandler`, macOS `.app` bundling via `Dotnet.Bundle` (`bundle-x64.sh` / `bundle-arm64.sh`).
- `CefGlue.Demo.WPF` is much thinner: `[STAThread] Program.Main` must call `CefRuntimeLoader.Initialize` **before** the WPF `App` starts, and it registers no custom schemes. Its JS-interop menu items exist but their `BrowserView` implementations (`EvaluateJavascript`, `BindJavascriptObject`, the `boundBeforeLoadObject` registration) are entirely commented out — the menu items are no-ops.
- The Avalonia demo also registers a custom `test:` scheme whose handler (`CustomSchemeHandler`) is an unimplemented stub that throws `NotImplementedException`.
- There is no shared demo library — the two demos are copy-paste duplicates structurally.

## Platform notes

- Linux ARM64: dynamic loading of `libcef.so` fails with `cannot allocate memory in static TLS block` (suspected CLR TLS exhaustion). Workarounds in `LINUX.md`: `LD_PRELOAD=/path/to/libHarfBuzzSharp.so:/path/to/libcef.so`, or `patchelf --add-needed` on the published binaries (BrowserProcess needs only `libcef.so`).
- Verified Linux distros (July 2024): Arch x64, Debian 12 x64, Kylin V10 ARM64 (with the workaround).
- macOS: `libEGL`/`libGLESv2`/`libvk_swiftshader` dylibs are additionally placed inside `CefGlueBrowserProcess`; bundling scripts `chmod +x` the subprocess.

## Repo state (as of July 2026)

- Branches in flight: `RDEV-8412-bump-cef-to-134.3.9` (CEF 134 bump — explains the `134.6998.178` nupkgs in local `Nuget\output`), `RDEV-8017-reproduce-memory-leak`.
- Only one git tag exists (`120.6099.2`) — tags are not part of the release flow; releases are "Raise version" PRs + external publishing.
- `origin` and `upstream` both point to `OutSystems/CefGlue`; the original Xilium repo is not configured as a remote.
