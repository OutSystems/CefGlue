# Code Review — PR #208 "RDEV-8412 - Bump to CEF v134.3.9"

Deep review of branch `RDEV-8412-bump-cef-to-134.3.9` (tip `a7bc617`) against `main` (merge-base `98a0655`). PR is a **Draft**; author reports macOS working, **Windows broken** (runtime/DevTools/WebGL/tests all ❌). Reviewer `filipnavara` diagnosed the Windows crash as CEF `.pak` files not copied into a `locales` folder.

**Method.** 8 review dimensions (struct layout, enums, API migration, generator, runtime behavior, build/locales, debug-leftovers, known-bug regression) run in parallel, then **every finding independently verified by two skeptics** tracing the actual branch code + CEF 134 headers. Result: **25 CONFIRMED, 5 PLAUSIBLE (1 of 2 verifiers), 5 REFUTED**. Diff surface: ~271 files (181 regenerated `CefGlue/`, 73 `Interop.Gen/`, ~130 meaningful hand-written/config). Static analysis only — nothing was built or run.

**Original verdict (superseded).** *"NOT mergeable as-is. One critical build break, two critical debug-leftovers shipped in the library, the Windows-crash root cause is real and fixable, and both known criticals persist."* — kept for the record; the build break did not exist and the locales root cause was misdiagnosed. See the correction log.

**Current verdict (2026-08-03): still not mergeable, but the Windows blocker is gone.** The PR's headline failure — Windows unusable — came down to a single missing copy of the locale paks, now fixed and verified by running the app. **Windows windowed mode now works**: page render, DevTools and WebGL all confirmed on `get.webgl.org`. Also fixed: the Linux struct layout, the shipped debug logging, and DevTools opening without window decorations.

What is *not* covered by that: **offscreen rendering has never been run on 134** (see C11 — the demo's OSR switch is dead), the test suite has not been run, and macOS/Linux/`win-arm64` are untested since these fixes landed. See [Priorities](#what-actually-matters-now) for the ranked open list.

---

## ⚠️ Correction log — 2026-08-03

**The original review was static analysis only; nothing was built or run.** When the findings were acted on, three of them did not survive contact with a real build. Corrections are marked inline below; this is the summary.

| Claim | Original rating | Actual | Evidence |
|---|---|---|---|
| **A1/C1** — `CS0208` build break on all OSes | 🔴 CONFIRMED | **Wrong.** Since C# 11 a pointer to a managed struct is *warning* `CS8500`, not an error. The branch tip compiled clean. | Clean rebuild of `a7bc617`: 0 errors. `#error` canary proved the file really is compiled. |
| **A1 second half** — struct layout corrupt | 🔴 CONFIRMED | **Correct.** Managed `planes[]` is an 8-byte reference: `plane_count` sat at offset 24 instead of 136. | Measured both layouts; inline version matches `cef_types_linux.h`. Fixed in `0b9c4bb`. |
| **C7** — `@(CefRuntimeWin64Locales)` "defined nowhere in the repo" | 🔴 CONFIRMED | **Wrong.** Defined in `packages/chromiumembeddedframework.runtime/134.3.9/build/…props`; the paks did copy. | Grep + build output. |
| **A2/P1** — locales go to `<app>\locales\` *instead of* `runtimes\<rid>\native\locales\` | 🟠 PLAUSIBLE | **False dichotomy.** A build needs them in **both**; applying the swap as written just trades one crash for another. | See below. |

**The real Windows defect.** The build output contains **two copies of `libcef.dll`**, and *different processes load different ones*:

| Process | loads `libcef.dll` from | needs locales at |
|---|---|---|
| Browser (the app itself) | `runtimes\win-x64\native\` | `runtimes\win-x64\native\locales\` |
| Subprocess (`CefGlueBrowserProcess\…exe`) | app root — `NativeLibsLoader` resolves `BaseDirectory\..` | `<app>\locales\` |
| Either, after `publish -r win-x64` | app root (the two are flattened into one) | `<app>\locales\` |

CEF resolves `locales\` relative to whichever copy was loaded, so on build **both** directories need the paks. Only the native one had them. Every other CEF resource (`icudtl.dat`, `resources.pak`, the snapshots) was already duplicated into both by the main asset-copy block — locales were the sole exception, which is why nothing else broke.

**Two distinct symptoms, two different missing copies:**

- Missing at `runtimes\…\native\locales\` → the **browser process** aborts instantly: exit code `-2147483645` (`0x80000003` STATUS_BREAKPOINT) inside `libcef.dll`, with **no stdout and no `debug.log`**. The Windows Application event log is the only place the faulting module path appears — that line is what identifies which copy loaded.
- Missing at `<app>\locales\` → the browser starts fine, but every **utility process** fails to init its resource bundle: an endless `ERROR:network_service_instance_impl.cc(612) Network service crashed, restarting service.` loop, with the GPU process going down alongside. **This was the original Windows failure** the PR reported, and it was hidden behind the first symptom until the browser process could start.

Fixed in `f0ad338` (publish) + `bb642a0` (both build locations). Measured: 69 network-service crashes in a 25 s run before, 0 after — at 15 s and 45 s alike, confirming it was a loop and not a shutdown artifact.

**Method lesson.** Every claim above that survived was one about *code semantics*; every claim that failed was about *build and load behavior* — exactly the class that static reading cannot settle. Treat the `CONFIRMED`/`PLAUSIBLE` labels in this document as leads. Reproduce before acting, and re-run the app after acting.

---

## Merge-readiness scorecard

| Area | State |
|---|---|
| Version pins (win/osx/linux + `<Version>`) | ✅ correct & consistent (`134.3.9` / `134.6998.178`) |
| CEF-120 FirstPartySets DEBUG-guard removed | ✅ done (but flag itself dropped — see F2) |
| Interop regeneration + API migration | ✅ largely correct; 2 issues (C13, C17) |
| **Assembly compiles** | ✅ **always did** — C1 was wrong (`CS8500` warning, not `CS0208` error) |
| Linux accelerated-paint struct layout | ✅ **fixed** `0b9c4bb` (the real half of A1) |
| **Windows runtime (locales)** | ✅ **fixed** `f0ad338`+`bb642a0` — startup crash *and* the network-service crash loop both gone (x64; arm64 untested) |
| Production hygiene | ✅ **fixed** `c711f52` — log path + verbose logging + telemetry comment removed |
| **Windows windowed runtime** | ✅ render + DevTools + WebGL confirmed on `get.webgl.org` |
| **Windows DevTools window** | ✅ **fixed** `ab10a9d` — opened as an undecorated child window (P6, found after review) |
| **Offscreen rendering (OSR)** | ❌ **never run on 134** — demo's OSR switch is dead (C11); highest remaining risk |
| Debug scaffolding removed | ❌ leftovers remain (C11, C14, C20, C21, C25); C9/C10 refuted |
| Known criticals from prior audit | ❌ both still present (C4, C5) |
| macOS / Linux runtime | ⚠️ macOS ✅ per PR, but neither retested since these fixes |
| app.manifest / package split / pins | ✅ clean (see Refuted) |

---

## A. Why Windows is broken

### A1 — ~~CRITICAL build break~~ → layout corruption (severity was overstated; ✅ FIXED `0b9c4bb`)

> **Corrected 2026-08-03.** The "build break" half of this finding is **wrong**. The rest is right.

`CefGlue/Interop/Structs/cef_accelerated_paint_info_t.cs:40` — `cef_accelerated_paint_info_t_linux` declared `[MarshalAs(ByValArray, SizeConst=4)] cef_accelerated_paint_native_pixmap_plane_t[] planes;`. A C# array is a reference type, so the struct is a *managed* type and `CefAcceleratedPaintInfoLinuxImpl.cs:9/13` declared a pointer to it.

~~That is `CS0208` and breaks the build on every OS.~~ **It is not.** Since C# 11, a pointer to a managed type is *warning* **`CS8500`**, not an error, and this repo does not treat warnings as errors. A clean rebuild of tip `a7bc617` produced **0 errors**; an `#error` canary in the same file confirmed it genuinely reaches the compiler and is not filtered out by the csproj. Nothing about the build contradicted "macOS build ✅".

**The layout corruption was real**, and is the reason this still needed fixing. Measured:

```
managed array field : sizeof=32   plane_count@24
four inline fields  : sizeof=152  plane_count@136   ← matches cef_types_linux.h
```

At runtime the array field is an 8-byte object reference, not 128 inline bytes — so `plane_count`/`modifier`/`format`/`extra` were 112 bytes off, and `_self->planes` reinterpreted native plane data as a managed reference.

**Severity footnote (added 2026-08-03).** This is currently **unreachable**, not merely latent: `CommonCefRenderHandler.OnAcceleratedPaint` is an empty method body, so CefGlue never consumes accelerated paint on any platform — OSR goes through `OnPaint` with a software buffer. The fix is still correct and worth having before anyone wires accelerated paint up, but it fixed no live defect. See P7.

**Fixed** by spelling the four planes out as inline `plane0..plane3` fields, which also silences `CS8500`.

### A2 — Windows locales (right symptom, wrong mechanism; ✅ FIXED `f0ad338`+`bb642a0`)

> **Corrected 2026-08-03.** filipnavara's symptom-level diagnosis — missing `.pak` files — was right. **Both** mechanisms proposed below were wrong, and the prescribed fix *causes* the crash on `dotnet build`. Do not apply this section as written.

~~1. **(C7, confirmed)** `@(CefRuntimeWin64Locales)` / `@(CefRuntimeWinArm64Locales)` are defined nowhere in the repo → empty ItemGroup → zero locale files copy.~~
**Wrong.** Both items are defined in `packages/chromiumembeddedframework.runtime/134.3.9/build/chromiumembeddedframework.runtime.props:8-9`, and all 55 paks did copy.

~~2. **(P1)** The `<Link>` targets `runtimes\win-x64\native\locales\…` — not `<app>\locales\` — so CEF's `<module dir>\locales` default misses them.~~
**Backwards.** The premise that `<module dir>` is the app root is false: the output carries **two copies of `libcef.dll`**, and the .NET loader picks the RID-native one at `runtimes\win-x64\native\`. So the original `<Link>` already pointed at the correct directory for `dotnet build`, and moving the paks to `<app>\locales\` empties the directory CEF actually reads.

**The actual defect** was that the paks reached only *one* of the two directories CEF reads them from, and were absent from publish entirely. See the correction log for the full table; in short, the browser process and the CefGlue subprocess load different `libcef.dll` copies and each needs its own `locales\` folder. The original block also set no `<CopyToPublishDirectory>`, so `dotnet publish` shipped none at all.

**Fixed** in `f0ad338` + `bb642a0`: both build directories get the paks, publish gets the single flattened copy. Verified by running — the network-service crash loop that this PR was actually suffering from is gone (69 crashes per 25 s run → 0), on build and publish output alike (x64; `win-arm64` uses the same code path but is untested).

**Debugging note for next time.** The two failure modes look nothing alike — a silent instant `STATUS_BREAKPOINT` when the *browser* copy is missing, an endless `network_service_instance_impl.cc(612)` restart loop when the *subprocess* copy is missing — but they have the same cause. In both cases the first question is *which* `libcef.dll` the failing process loaded, which the Windows Application event log answers and static reading does not. This review reasoned from the app-root copy and got both mechanisms wrong.

---

## B. Merge-blockers: production hygiene & debug scaffolding

These ship in the `CefGlue.Common` NuGet (not just the demo) unless noted.

| # | Sev | Finding | Location | Fix |
|---|-----|---------|----------|-----|
| C2/C3 | ✅ | ~~Hardcoded `--log-file=C:\git\CefGlue\cef_main_log.txt` appended unconditionally in the browser process~~ — **fixed `c711f52`** | `BrowserCefApp.cs:40` | removed |
| C6 | ✅ | ~~`--log-severity=verbose` forced on in shipped code~~ — **fixed `c711f52`** | `BrowserCefApp.cs:39` | removed |
| C8/C19 | ✅ | ~~`// Telemetry to debug` + commented-out `disable-features=NetworkServiceSandbox`~~ — **fixed `c711f52`** | `BrowserCefApp.cs:37-38` | removed |
| C9 | ⚪ | ~~`TODO hgo` above the **SharedTexture** getter — "unverified load-bearing interop"~~ **Refuted 2026-08-03: the mapping is correct.** Only the stale comment remains | `Platform/Windows/CefAcceleratedPaintInfoWindowsImpl.cs:26` | delete the comment |
| C10 | ⚪ | Same, Mac. **Also correct** | `Platform/Mac/CefAcceleratedPaintInfoMacImpl.cs:26` | delete the comment |
| C11 | 🟠 | OSR demo silently dead: `#if WINDOWLESS` init removed from `Program.cs`/`MainWindow` but the `WINDOWLESS` define kept in csproj → `Debug_WindowlessRender` builds a windowed demo. **OSR untested on 134.** | `Demo.Avalonia/Program.cs:19` | restore the OSR init, or remove the dead define |
| C14 | 🟡 | Demo default URL changed to `https://get.webgl.org/` + commented aquarium URL (WebGL troubleshooting leftovers) | `Demo.Avalonia/BrowserView.axaml.cs:28` | revert to the intended landing page |
| C20 | ⚪ | Unused `Avalonia.ReactiveUI` PackageReference added to the **demo** (note: the PR body says ReactiveUI is the macOS class-conflict fix — but that belongs in the library/consumers, not an unused demo ref; clarify intent) | `Demo.Avalonia/*.csproj:35` | remove or justify |
| C21 | ⚪ | Unrelated ReSharper wrap-style keys added to `.editorconfig` (personal IDE prefs); `insert_final_newline` is fine | `.editorconfig:18` | drop the `csharp_wrap_*` keys |
| C23 | nit | Leftover C++ codegen marker `/*--cef(optional_param=buffer)--*/` in a hand-written doc comment | `Classes.Proxies/CefV8Value.cs:171` | convert to `<summary>` |
| C25 | nit | `gen-cef3.sh` mode flipped to executable (incidental chmod) | `Interop.Gen/gen-cef3.sh` | revert unless intended |

---

## B2. Found after the review (not in the original 8 dimensions)

### P6 — DevTools opened as an undecorated child window on Windows (✅ FIXED `ab10a9d`)
`CommonBrowserAdapter.ShowDeveloperTools` called `windowInfo.SetAsPopup(BrowserHost.GetWindowHandle(), "DevTools")`. `SetAsPopup` sets `ParentHandle`, and a window info carrying a parent makes CEF host DevTools as a **child widget inside the main window** — no title bar, no close/minimize/maximize. macOS was unaffected because it never called `SetAsPopup`.

Measured by enumerating the Win32 window tree with DevTools open:

| RuntimeStyle | SetAsPopup | top-level windows | DevTools window |
|---|---|---|---|
| Chrome (as committed) | yes | 1 | `Chrome_WidgetWin_1`, has a parent, `0x56000000` = `WS_CHILD` |
| Chrome | **no** | **2** | `parent=0`, `0x16CF0000` = CAPTION\|SYSMENU\|THICKFRAME\|MINIMIZEBOX\|MAXIMIZEBOX |
| default | yes | 1 | child again |

So the **parent handle** is the cause, not `RuntimeStyle` — removing the Chrome style alone does not help, which is why it was left in place. Note this sits one line below the `RuntimeStyle.Chrome` call that the original review examined and cleared (Refuted #1): the verdict on that line was right, but the defect was its neighbour.

### P7 — accelerated paint is never consumed
`CommonCefRenderHandler.OnAcceleratedPaint` is an **empty method body**. CefGlue does not use accelerated paint on any platform; OSR renders through `OnPaint` with a software buffer. Consequences: C9/C10 are cosmetic, A1's struct fix is correct but unreachable, and any future OSR/GPU work starts from "not wired up" rather than "wired up and broken". Not a defect in this PR — recorded so the surface is not mistaken for live code.

### P8 — `dotnet publish` can emit the subprocess without its dependencies (UNRESOLVED)
Observed while testing: repeated `dotnet publish -o <dir>` runs produced an output containing `Xilium.CefGlue.BrowserProcess.exe` at the root but **none of its .NET dependency assemblies** (213 files short of a known-good publish of the same commit). Every CEF subprocess then fails to launch, presenting as a GPU-process crash loop ending in `FATAL:gpu_data_manager_impl_private.cc(420) GPU process isn't usable. Goodbye.` — a symptom that looks nothing like a packaging fault.

An earlier publish of the same commit was complete, so this is intermittent. **Not diagnosed** — unknown whether it is an incremental-build artifact of publishing repeatedly to different `-o` directories, or a real fragility in how `@(CefGlueBrowserProcessFiles)` is gathered. Given this branch already shipped one publish-only defect (A2), a clean-clone publish should be verified before release.

---

## C. Correctness & API-migration issues

### C13 — MEDIUM: new observer handlers are unusable dead code
`CefSettingObserver.OnSettingChanged` (`:30`) and `CefPreferenceObserver.OnPreferenceChanged` are `internal virtual` on `public` classes documented as *client-implemented* — every other handler uses `protected virtual/abstract`. External consumers **cannot override them**, so they can never receive notifications. Worse, the migration removed the observer registration methods from `CefRequestContext.cs` and added **no public `AddSettingObserver`/`AddPreferenceObserver`**, so both types are currently unreachable. The interop glue itself is correct.
**Fix:** make the callbacks `protected virtual`; add the public registration APIs — or drop the classes until wired.

### C17 — LOW: `cef_settings_t` missing the CEF 134 `log_items` field
`Interop/Structs/cef_settings_t.cs:34` — CEF 134 inserts `cef_log_items_t log_items;` between `log_severity` and `javascript_flags`; the mirror omits it. **Not corrupting today** (implicit `Pack=0` padding happens to land `javascript_flags` at the right offset), but `log_items` can never be configured and the coincidental padding would turn into real corruption if another field were inserted there later.
**Fix:** add `log_items` (and the `CefLogItems` enum) to match the header; wire through the `CefSettings` wrapper.

### C18 — LOW: pre-existing `FromNativeOrNull` NRE, unchanged and re-verified
`make_interop.py:556` still emits `value.release(ptr)` before the found-check → NRE when the pointer isn't in `_roots`, replicated to all reversible `.g.cs` (`CefUserData.g.cs:31`, etc.). Not introduced by this PR, but the bump is the natural time to fix it in the template and regenerate.

### Generator: verified sound
The CEF 134 generator adaptations are correct — `get_cef_api_details` parses `cef_api_versions.h` and emits `CEF_API_VERSION=13401` with per-OS hashes matching the header (C24 is only a brittleness nit: it hard-codes the `OS_WIN/OS_MAC/OS_LINUX` macro spellings and would fail silently on a future header reformat — optional hardening: raise if any OS is missing).

---

## D. Known-issues regression status (prior 30-bug audit vs this branch)

The bump touched only 5 of the bug-bearing files; the other 19 are byte-identical to `main`, so those bugs persist by identity. Net: **29 of 30 still present; 1 fixed.**

| KI # | Sev | Status on branch | Evidence |
|------|-----|------------------|----------|
| #1 collections→`{$values}` | 🔴 | **PRESENT** (C4) | `CefGlueGlobalScript.js` identical to main |
| #2 OSR popup-close bricks main | 🔴 | **PRESENT** (C5) | `CommonOffscreenBrowserAdapter.cs` identical |
| #10 detach `VisibilityChanged` NRE | 🟠 | **✅ FIXED** (C12) | line 187 now `VisibilityChanged?.Invoke(false)` |
| #7 dispose-vs-create leak | 🟠 | PRESENT (P2) | lifecycle untouched |
| #13 trackpad wheel→0 | 🟡 | PRESENT (C15) | `OnPointerWheelChanged` unchanged |
| #14 evaluate hangs on nav race | 🟡 | PRESENT (P4) | only a trailing-newline diff |
| #16 `Load` double-init race | 🟡 | PRESENT (P5) | init guard untouched |
| #25 ProcessExit shutdown w/ live browsers | 🟡 | PRESENT (C16) | `CefRuntimeLoader.cs:66` unchanged |
| #30 ARM64 delegate-cache race | ⚪ | PRESENT (C22) | replicated into renamed 134 methods |
| others (19 unchanged files) | — | PRESENT | files byte-identical to main |

The two **criticals** (#1, #2) are unaffected by the bump and remain the highest-value fixes. #10 was correctly fixed with exactly the one-char change the audit recommended.

---

## E. For conscious decision (flagged, not classified as defects)

- **F1 — `NoSandbox=true` now on Windows** (`CefRuntimeLoader.cs:44-48`, Windows folded into the Linux arm). *Refuted as a "defect"* by both verifiers: CefGlue never plumbed the Windows sandbox (`cef_sandbox_info`), so disabling it is arguably required for 134 rather than a regression — **but it is a real security-posture change** that should be an explicit, documented team decision, not an accident of the branch tip. If the sandbox is wanted, that's separate work (sandbox-info plumbing).
- **F2 — FirstPartySets workaround removed without verification** (P3). Dropping the CEF-120 DEBUG guard is correct; dropping the `disable-features=FirstPartySets` flag with it is only safe if CEF #3643 (YouTube crash) is actually fixed in 134. No evidence in the diff that this was retested. **Confirm #3643 is resolved in 134.3.9, or reinstate the flag** (guardless) and note it in the PR.

---

## F. Refuted (reviewed, not issues)

Both verifiers cleared these — recorded so they aren't re-raised:

1. **DevTools `RuntimeStyle.Chrome`** (`CommonBrowserAdapter.cs:257`) — correct/harmless; only the DevTools popup, main browser stays default. *(Still true, and re-confirmed by measurement in P6 — but the `SetAsPopup` call on the very next line **was** a real defect. Clearing a line is not clearing its neighbours.)*
2. **Mixed package family (Windows CefSharp vs mac/linux `cef.redist.*`)** — `CefGlue.Packages.props` unchanged; the split is the existing, working design.
3. **Version pins & macOS dylib names** — complete and consistent (`134.3.9` ×3, `Version 134.6998.178`); dylibs still valid.
4. **`app.manifest` CefSharp identity** — clean; the earlier leftover `assemblyIdentity` was already removed (commit `9f38bef`); only standard UAC/`supportedOS` boilerplate remains (the functional GPU fix).
5. **`NoSandbox` as a "security defect"** — see F1 (surfaced for awareness instead).

Added 2026-08-03, refuted by building rather than reading — see the correction log:

6. **C1 "assembly fails to build on all OSes"** — no; `CS8500` warning, not `CS0208` error.
7. **C7 "`CefRuntimeWin*Locales` defined nowhere"** — no; defined in the runtime meta-package props, and the paks copied.
8. **P1 "locales must go to `<app>\locales\`"** — not for `dotnet build`; that is the wrong directory there, and applying it breaks a working build.

---

## What actually matters now

Ranked after the 2026-08-03 fixes. The original "path to green" is kept below for traceability, but this list supersedes it.

### 1. OSR has never been run on CEF 134 — and it is a primary use case (C11)
`CefGlue.Demo.Avalonia.csproj:14` defines `WINDOWLESS`, but **nothing references it** — no `#if WINDOWLESS` anywhere in the demo, and no `WindowlessRenderingEnabled` either. `Debug_WindowlessRender` therefore builds an ordinary windowed demo. Everything confirmed working on Windows is windowed mode only. After a 14-major Chromium jump, an entire product mode has zero validation, and known-critical #2 (OSR popup-close) lives in it.

**Cheapest unblock on the list**: restore the OSR init, or delete the dead define so the gap stops being invisible.

### 2. `NoSandbox = true` now applies to Windows (F1)
`CefRuntimeLoader.cs:44-48` folds Windows into the Linux arm, so the renderer ships unsandboxed. Arguably required — CefGlue never plumbed `cef_sandbox_info` — but it is currently an accident of the branch rather than a recorded decision. Needs a yes/no from the team, not a code change.

### 3. The FirstPartySets workaround was dropped unverified (F2)
`disable-features=FirstPartySets` is gone from the tree entirely. Correct only if CEF #3643 is genuinely fixed in 134.3.9; otherwise this ships a YouTube crash. One check against the CEF tracker settles it.

### 4. Verification gaps
Test suite not run. macOS and Linux not retested since these fixes. `win-arm64` untested. Clean-clone publish unverified (P8).

### 5. Everything else is hygiene
Demo URL (C14), unused ReactiveUI ref (C20), `.editorconfig` prefs (C21), `chmod` (C25), codegen marker (C23), the two stale `TODO hgo` comments (C9/C10), unusable observer classes (C13), missing `log_items` (C17), pre-existing `FromNativeOrNull` NRE (C18). All real, all trivial, none a risk.

Known-issue criticals #1/#2 are pre-existing on `main`, not bump regressions — schedule alongside, not necessarily blocking this merge.

<details><summary>Original path to green (superseded)</summary>

1. ~~**Fix the build break** (A1)~~ — **done** `0b9c4bb`. There was no build break; the Linux `planes` array was inlined for the layout corruption instead.
2. ~~**Fix Windows locales** (A2 / C7+P1)~~ — **done** `f0ad338`+`bb642a0`, but *not* by the method A2 prescribed.
3. **Strip remaining debug scaffolding** — log path/verbose/telemetry **done** `c711f52`; the rest is hygiene, see above.
4. ~~**Verify the two `SharedTexture` mappings** (C9/C10)~~ — done, both correct; refuted.
5. **Decide F1/F2 consciously** — now items 2 and 3 above.
6. **Wire or drop the observer classes** (C13); add `cef_settings_t.log_items` (C17).
7. **Then** re-run the CEF-UPGRADE.md validation checklist on Windows + macOS + Linux.
8. Known-issue criticals #1/#2 are independent of the bump.

</details>

Cross-references: [CEF-UPGRADE.md](CEF-UPGRADE.md) (bump process & validation), [KNOWN-ISSUES.md](KNOWN-ISSUES.md) (the 30-bug audit), [INTERNALS.md](INTERNALS.md).
