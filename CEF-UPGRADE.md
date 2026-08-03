# CEF Upgrade Playbook

Step-by-step process for bumping the CEF version in this repository, written to be executable by an AI agent and auditable by humans. Every step cites the git evidence it was derived from.

**Provenance.** Reconstructed (July 2026) from the actual bump history — commit `6301718` (106→117, which squashed 8 upstream cherry-picks plus a manual 116→117 step), commit `ffa89aa` (117→120, PR #126), the post-120 aftershock commits on `main`, and the unfinished CEF-134 branch `RDEV-8412-bump-cef-to-134.3.9` — cross-checked against the Confluence runbook ["Migrating CEF Step-by-step"](https://outsystemsrd.atlassian.net/wiki/spaces/RKB/pages/3593404436/Migrating+CEF+Step-by-step). Where the wiki disagrees with the repository evidence, the repository wins; see [Wiki vs reality](#wiki-vs-reality).

**Scale expectations.** 117→120 (3 Chromium majors) touched 49 files. 120→134 (14 majors) touched 273 files and needed generator-engine changes plus weeks of runtime debugging. Budget accordingly.

---

## Quick reference

| Thing | Where |
|---|---|
| Version pins | `Directory.Build.props`: `CefRedistVersion` (l.6), `CefRedistOSXVersion` (l.8), `CefRedistLinuxVersion` (l.9), `<Version>` (l.16) |
| Package version scheme | `<chromium major>.<chromium build>.<fork revision>` — fork revision restarts at `.1` per CEF bump, then increments per NuGet release (`117.5938.1` → `117.5938.2` → `120.6099.1` → … → `120.6099.211`) |
| Vendored CEF headers | `CefGlue.Interop.Gen\include\` — 121 tracked files (86 root `*.h`, 24 `internal\`, 10 `wrapper\`, 1 `base\internal\cef_net_error_list.h`) |
| Generator | `CefGlue.Interop.Gen\` — Python 3; entry `cefglue_interop_gen.py`; config `schema.py` + `schema_cef3.py`; emission `make_interop.py`; parser `cef_parser.py` (vendored CEF tool) |
| Generated output (committed) | `CefGlue\Interop\Classes.g\` (126), `CefGlue\Classes.g\` (126), `CefGlue\Interop\libcef.g.cs`, `CefGlue\Interop\version.g.cs` |
| Hand-written, must port manually | `CefGlue\Classes.Proxies\` (68), `Classes.Handlers\` (58), `Enums\` (85), `Structs\` (27), `Interop\Structs\` (24), `Interop\Base\` (8), `Platform\` (7), `libcef.*.cs` partials (7), `Classes.Interfaces\` (3) |
| Runtime version gate | `CefRuntime.Load()` → `CheckVersionByApiHash` (`CefGlue\CefRuntime.cs`) compares `libcef` `cef_api_hash` with `version.g.cs` constants → `CefVersionMismatchException` |

---

## Phase 0 — Version selection & external prerequisites

### 0.1 Choose the target CEF version

Criteria (wiki step 1, still valid except the struck-through upstream check):

1. `chromiumembeddedframework.runtime.win-x64` **and** `.win-arm64` exist on nuget.org at that version (published by the CefSharp project from `cefsharp/cef-binary`).
2. The version is a **stable** release on the [CEF Automated Builds site](https://cef-builds.spotifycdn.com/index.html).
3. ~~Supported by the original Xilium CefGlue repo~~ — obsolete; the upstream GitLab repo is inactive.

### 0.2 Publish the OutSystems-owned redist packages FIRST

Seven packages must exist on nuget.org at the target version before the repo can restore. `Nuget.config` maps pattern `*` to nuget.org only — a missing package is a hard `NU1102` with no fallback feed.

| Package | Publisher | Source |
|---|---|---|
| `chromiumembeddedframework.runtime` (+ `.win-x64`, `.win-arm64`) | CefSharp project | github.com/cefsharp/cef-binary — usually available quickly |
| `cef.redist.linux64`, `cef.redist.linuxarm64` | **OutSystems** | github.com/OutSystems/cef.redist.linux |
| `cef.redist.osx64`, `cef.redist.osx.arm64` | **OutSystems** | github.com/OutSystems/cef.redist.osx — point `make_cefredist.sh` at the new CEF binary URL, publish via the Azure DevOps `ArtifactRepository` pipeline (definitionId 525), setting the version variable (wiki step 2) |

⚠️ **Do not bump only `CefRedistVersion` and leave OSX/Linux pins behind.** It compiles and even runs on Windows, but macOS/Linux throw `CefVersionMismatchException` at first browser creation. The abandoned `cef-121` branch is a fossil of exactly this state (win=121.3.13, osx/linux=120.1.8). The three separate pins exist to decouple publishing cadences, not to allow mixed shipping.

*Automatable: yes (nuget.org API pre-flight); the OutSystems package publishing itself is an external pipeline.*

### 0.3 Reuse prior art

For any target ≤134, branch `RDEV-8412-bump-cef-to-134.3.9` already contains the generator adaptations and API migrations for CEF 121–134 — port from it instead of rediscovering (see [CEF-134 branch: state of play](#cef-134-branch-state-of-play)). The 106→117 bump additionally cherry-picked upstream xilium/CefGlue per-version commits; upstream is now inactive, so treat that strategy as historical.

---

## Phase 1 — Swap the vendored headers

Download the **minimal distribution** for the target version from the CEF builds site (headers are architecture-independent — wiki step 4) and replace the tracked set under `CefGlue.Interop.Gen\include\`:

- Replace the 121 tracked files; `git add`/`git rm` so additions/deletions are tracked.
- Respect `CefGlue.Interop.Gen\.gitignore`: do **not** add `include/base/`, `include/capi/`, `include/test/`, `include/views/`, `cef_pack_resources.h`, `cef_pack_strings.h`. Exception: `include/base/internal/cef_net_error_list.h` is deliberately un-ignored (it is the source for `CefGlue\Enums\CefErrorCode.cs`).
- **Must** update `include\cef_version.h` and `include\cef_api_hash.h` — they feed `version.g.cs` even though `cef_version.h` is excluded from class parsing. CEF ≥126 replaces `cef_api_hash.h` semantics with `include\cef_api_versions.h` (see the ≥126 row in 2.1).
- `include/internal/` and `include/wrapper/` are **never parsed** (`add_directory` is non-recursive, `cef_parser.py:563`) but must be refreshed anyway — the manual struct-mirror porting in Phase 3 diffs against them.

Evidence: 6301718 (55 headers), ffa89aa (16 headers), e05b25c (68 headers). *Automatable: yes.*

---

## Phase 2 — Regenerate the interop layer

### 2.1 Adapt the generator for known CEF breakpoints (before generating)

| CEF version | Required generator change | Evidence |
|---|---|---|
| ~116+ | Fixed-width C types: add `int16_t`…`uint64_t` to `_simpletypes` in `cef_parser.py` | 6301718 (done) |
| 108+ | Class inheritance support in `make_interop.py` (`get_top_base_class_name`, abstract proxies, ctor chaining) — CEF made `cef_request_context_t` derive from `cef_preference_manager_t` | 6301718 (done) |
| ~122+ | `cef_parser.py get_capi_name()`: underscore-insertion condition must be `(lastchr.lower() == lastchr or lastchr.isdigit())` so `V8Value`→`v8_value`, `Base64Encode`→`base64_encode`. **Without this, P/Invokes bind to nonexistent exports and fail at runtime (`EntryPointNotFoundException`), not compile time.** | e05b25c (on 134 branch) |
| ≥126 | `make_interop.py make_version_cs` must parse `include/cef_api_versions.h` (`CEF_API_VERSION_LAST` + per-OS `CEF_API_HASH_<N>`); `CEF_API_HASH_UNIVERSAL` is gone. Hand-written `CefGlue\Interop\libcef.cs`: `api_hash(int entry)` → `api_hash(int version, int entry)`; `CefRuntime.CheckVersion` passes `libcef.CEF_API_VERSION`. | e05b25c (on 134 branch) |
| large jumps | Patch the vendored `cef_parser.py` when it fails to parse new header idioms. It is a **CefGlue-adapted fork** with custom type mappings, only ever patched incrementally (31fd774 smashed in xilium's CEF-106 copy; 6301718 +36 lines for 117; e05b25c +1 line for 134) — do **not** overwrite it wholesale with CEF's `tools/cef_parser.py`, which lacks the CefGlue-specific mappings | 31fd774, 6301718, e05b25c |

*Automatable: no in general — but for targets ≤134, port from the branch (partial).*

### 2.2 Register the new API surface (discovery loop)

For every **new CEF class**: add a `classdef` entry in `schema_cef3.py`:

```python
'CefFoo': { 'role': ROLE_PROXY },                    # native-owned object you call
'CefFooHandler': { 'role': ROLE_HANDLER },           # you implement, native calls you
# options: 'name': 'CefFooBar' (C# name override), 'reversible': True (_roots reverse lookup),
#          'autodispose': True (self-dispose at refcount 0), 'abstract': True (unsealed + private-protected)
```

For every **new enum** in signatures: add `'cef_xxx_t': 'CefXxx'` to `c2cs_enumtypes` in `schema.py`. New POD structs: `c2cs_types` entry + hand-written mirror (Phase 3).

The asymmetry that shapes the loop (verified in `make_interop.py:875` and `schema.py:285`):
- Missing **classdef** → generation **aborts before writing anything**: `ERROR! Class role must be defined. Class name X.` → safe to discover iteratively by just running the generator.
- Missing **enum/struct mapping** → only a stdout **warning** (`Warning! C type "X" is not mapped to C# type`) and non-compiling C# later → **scan generator stdout**; don't rely on the abort.

How to pick the role and lifetime options: see [2.4](#24-choosing-classdef-options--the-decision-rules). Evidence: 6301718 (+2 classes, +6 enums), ffa89aa (+2 enums), e05b25c (+4 classes, +3 enums). *Automatable: mostly — the discovery loop is scriptable and the role is mechanically derivable (2.4); `reversible`/`autodispose` need a signature scan plus light judgment.*

### 2.3 Run the generator

```
cd CefGlue.Interop.Gen          # cwd MATTERS: include, ..\CefGlue and .\Classes.*.tmpl are relative
gen-cef3.cmd                    # = python -B cefglue_interop_gen.py --cpp-header-dir include --cefglue-dir ..\CefGlue\ --no-backup
# POSIX: ./gen-cef3.sh (pins /usr/bin/python3)
```

Python 3 required (the `.cmd` calls bare `python`; its python27 registry comment is stale). Iterate with 2.2 until it prints `Done - Wrote N files.`

Post-run hygiene:
- **Delete `CefGlue\CefGlue.g.props`** — written every run, not gitignored, unused by the SDK-style csproj (committed briefly on main in 2021 — added by 6240c53, removed by d12bf46 — and again by mistake on the abandoned `cef-121` branch, where it still sits).
- **`git rm` stale `.g.cs`** for classes CEF removed — the generator never deletes, and the csproj globs `*.cs`, so leftovers still compile. Remove the matching hand-written `Classes.Proxies/Handlers` files too.
- The regenerated `*.tmpl.g.cs` scaffolds under `Classes.{Proxies,Handlers}.tmpl\` (gitignored) are your porting reference for Phase 3. Regenerate before trusting them — the ones on disk may be stale from a previous local run.
- Verify `CefGlue\Interop\version.g.cs` now matches the new `cef_version.h` (a clean `git status` does **not** prove the generator ran).

*Automatable: yes.*

### 2.4 Choosing classdef options — the decision rules

The correct options are **derivable, not guesswork**. The ground truth is the `/*--cef(...)--*/` annotation directly above each class declaration in the CEF header — the generator only parses declarations carrying these annotations, and `schema_cef3.py` is a manually-maintained mirror of what they imply:

| Option | Rule | Ground truth / precedent |
|---|---|---|
| `role` | `/*--cef(source=library)--*/` → `ROLE_PROXY` (CEF implements it, you call it). `/*--cef(source=client)--*/` → `ROLE_HANDLER` (you implement it, CEF calls you). Purely mechanical. | `cef_browser.h:60` `source=library` → `'CefBrowser': {'role': ROLE_PROXY}`; `cef_client.h:65` `source=client` → handler |
| `name` | Only to fix ugly default C#-ization of acronyms. | 11 existing entries, e.g. `CefDOMNode` → `CefDomNode`, `CefURLRequest` → `CefUrlRequest` |
| `reversible: True` | Handlers only. Needed iff some **library-side API returns this client-side type back** to managed code — the generated `FromNative` must find the original managed instance in `_roots`. Rule: grep the new headers for methods of `source=library` classes returning the class type. | All 7 existing entries follow it: `CefClient` (← `CefBrowserHost::GetClient`), `CefV8Handler` (← `CefV8Value::GetFunctionHandler`), `CefURLRequestClient`, `CefRequestContextHandler`, `CefV8ArrayBufferReleaseCallback`, `CefUserData`, `CefExtensionHandler` |
| `autodispose: True` | Handlers only. One-shot visitor/callback objects that CEF consumes and releases without ever handing back — the managed wrapper disposes itself when the native refcount hits 0. Rule: short-lived, passed in, never stored/returned. | The 9 existing entries are all visitors/callbacks: `CefStringVisitor`, `CefCookieVisitor`, `CefResourceHandler`, `CefSetCookieCallback`, … |
| `abstract: True` | Proxies only. The class is a base of another bound class in CEF's own hierarchy (generator emits an unsealed class with `private protected` members). | Only `CefPreferenceManager` (base of `cef_request_context_t` since CEF 108) |

When still unsure, find the closest analogous class in `schema_cef3.py` (it is organized by CEF release with comments) and copy its shape.

### 2.5 New-feature triage — skip, bind, or integrate?

A new CEF version usually surfaces new features (classes, callbacks, settings). For each one, make an explicit choice between three levels — and **record the decision in the bump PR description** so the next maintainer knows what was deliberately skipped:

**Level 0 — skip entirely (exclusion list).** Add the header to the parse-exclusion list in `cefglue_interop_gen.py` (lines 49–51). Precedent: `cef_thread.h`/`cef_waitable_event.h` are excluded because the BCL covers threading (their stale `classdef` entries are harmless — exclusions win); `cef_application_mac.h` is platform-specific; the whole `include/views/` tree is never parsed by design (`add_directory` is non-recursive) — CefGlue deliberately does not bind the CEF Views framework. **Constraint:** only viable when no *bound* class references the type in a method signature; otherwise you get unmapped-type warnings and broken generated code (the 134 branch had to bind `CefUnresponsiveProcessCallback` purely because the new `OnRenderProcessUnresponsive` callback references it).

**Level 1 — bind at the core layer, don't surface (the sensible default).** Add the `classdef` + minimal hand-written half; do **not** bridge into `CefGlue.Common`. Note the asymmetry:
- New **classes** at Level 1 need only enough hand-written code to compile (proxies can start as a thin file seeded from the `.tmpl.g.cs` scaffold).
- New **callbacks on existing handler classes** cannot be skipped at all: after regeneration the vtable slot exists and `Classes.Handlers` must implement the raw callback (compile error otherwise). Implement it plus a `protected virtual` whose **default preserves the old behavior** — read the callback's doc comment in the CEF header; it states what each return value means, and you pick the one meaning "use CEF default handling" (typically `return false`/`0`, or `callback.Continue(...)`).
- Precedent: this is the normal state of most of the binding — e.g. `CefDisplayHandler.OnCursorChange` exists in core but was never bridged to Common's `DisplayHandler`. Downstream code that needs a Level-1 feature can subclass the core handler directly.

**Level 2 — fully integrate.** Additionally bridge into the Common layer — an `internal Handle*` bridge method in `CefGlue.Common\Handlers\Handlers.cs` plus a delegation call in the matching `InternalHandlers\CommonCef*Handler.cs` (the two-tier pattern in [INTERNALS.md](INTERNALS.md)) — and, if user-facing, surface it on `BaseCefBrowser` (event/property/method). Precedent: `OnBeforeDevToolsPopup` in the 120 bump (ffa89aa, refined by 3a03c28: always call `base` unless fully replacing behavior). Reserve Level 2 for features the fork or its downstream consumers (WebViewControl/ReactView) actually need — everything else stays at Level 1 and can be promoted later.

Deprecated-but-present features: mark the `classdef` entry with a `# (Deprecated)` comment (the 134 branch did this for `CefExtension*`); removed features follow the deletion hygiene of 2.3/3.3.

*Automation note: an agent can default every new feature to Level 1 mechanically (role from the `source=` annotation, old-behavior defaults from the header docs) and escalate only Level-2 candidates and Level-0 opportunities for human sign-off.*

### 2.6 Generator failure modes → what to edit

| Symptom | What to edit |
|---|---|
| `ERROR! Class role must be defined. Class name X.` (aborts pre-write) | Add the `classdef` entry in `schema_cef3.py` (2.2/2.4) |
| `Warning! C type "X" is not mapped to C# type` (stdout only, non-fatal) | Add `c2cs_enumtypes`/`c2cs_types` entry in `schema.py` + hand-written enum/struct mirror (3.1/3.2) |
| `Unknown base class: X.` | New inheritance root — teach `make_interop.py` (`get_base_funcs` etc.) the new base, as done for `cef_preference_manager_t` in 6301718 |
| `Could not find CEF_API_HASH… constant.` | `make_version_cs` regex vs a reformatted `cef_api_hash.h` / the ≥126 `cef_api_versions.h` rework (2.1) |
| Classes/members silently missing from output | Parser can't parse a new header idiom — patch the vendored `cef_parser.py` (a CefGlue-adapted fork; see the "large jumps" row in 2.1) |
| Build passes but P/Invokes throw `EntryPointNotFoundException` at runtime | Wrong C name emission — the ≥122 `get_capi_name()` fix in `cef_parser.py` (2.1) |

---

## Phase 3 — Port the hand-written layers

This is the labor-intensive phase. Compile errors are the intended checklist for 3.3–3.5; **3.1 and 3.2 produce no compile errors when wrong — they produce memory corruption or wrong semantics at runtime.**

### 3.1 Struct mirrors (silent-corruption risk — do first, carefully)

The generator does NOT touch these. Diff old-vs-new `include\internal\cef_types*.h` field-for-field, preserving exact C order:

- `CefGlue\Interop\Structs\` (24 files) — e.g. ffa89aa **removed the mid-struct field** `accept_language_list` from `cef_browser_settings_t.cs` (a missed removal shifts every following field) and appended `chrome_policy_id`/`chrome_app_icon_id` to `cef_settings_t.cs`; 6301718 reshaped `cef_pdf_print_settings_t.cs` entirely.
- `CefGlue\Interop\Base\` (8 files) — rarely changes, still diff it.
- Public wrappers `CefGlue\Structs\` (27) — **including the `ToNative()` marshaling lines**: a forgotten field silently drops a setting.
- `CefGlue\Platform\` (7) — per-platform `cef_window_info_t` layouts.

*Automatable: partial (diff extraction scriptable; edits need review).*

### 3.2 Enums

All 85 files in `CefGlue\Enums\` are manual (header convention: `This file manually written from cef/include/internal/cef_types.h`). One new file per enum registered in 2.2; refresh changed ones; regenerate `CefErrorCode.cs` from `cef_net_error_list.h`.

⚠️ **Bit-flag enums get renumbered upstream** — ffa89aa inserted `Midi` at `1<<11` in `CefPermissionRequestTypes`, shifting everything above, and dropped two members. Never just append; re-derive the full value list.

### 3.3 Partial-class halves (handlers & proxies)

For each new/changed vtable callback: implement in `CefGlue\Classes.Handlers\` the private raw callback (exact generated name, e.g. `on_before_dev_tools_popup`) plus the `protected virtual`/`abstract` managed wrapper. Missing raw callback = compile error; **missing managed wrapper = silent no-op**. For each new proxy method: implement marshaling in `CefGlue\Classes.Proxies\`, seeding brand-new classes from the `.tmpl.g.cs` scaffolds (pattern reference: `Classes.Handlers\CefStringVisitor.cs` vs its tmpl).

Never edit `*.g.cs` — generator-emitted bugs (e.g. the known `FromNativeOrNull` release-before-null-check NRE affecting the 7 `reversible` handlers) are fixed in `make_interop.py` and regenerated.

### 3.4 Global functions

After `libcef.g.cs` regeneration: add/remove public wrappers in `CefGlue\CefRuntime.cs` (6301718: removed `EnableHighDpiSupport`, added `ResolveUrl`); re-home platform-only exports into hand-written partials (`libcef.win.cs` holds `cef_set_osmodal_loop`); grep repo-wide for callers of removed wrappers. **Audit that every new export actually gets a wrapper** — e05b25c generated `cef_get_exit_code` and never wired it.

### 3.5 Fork layers

Propagate API changes through: `CefGlue.Common\Handlers\Handlers.cs` + `InternalHandlers\CommonCef*Handler.cs` (always delegate to `base` unless fully replacing behavior — lesson of 3a03c28), `CommonBrowserAdapter.cs`, `BaseCefBrowser.cs`, `BrowserCefApp.cs`, `CefGlue.BrowserProcess\*`, and the demos. Whether a new CEF callback gets bridged into this layer at all is the Level-1 vs Level-2 triage decision of [2.5](#25-new-feature-triage--skip-bind-or-integrate); the bridging mechanics are the two-tier pattern in [INTERNALS.md](INTERNALS.md).

### 3.6 Version-pinned workarounds

Grep for `Remove this fix` and `ChromeVersion`. As of CEF 120 there is exactly one: `CefGlue.Common\CefRuntimeLoader.cs` (~l.84) appends `disable-features=FirstPartySets` (YouTube crash, CEF issue 3643, commit 0b5abd3) guarded by

```csharp
#if DEBUG
if (CefRuntime.ChromeVersion.Split(".").First() != "120")
    throw new Exception("Remove this fix block after CEF upgrade");
#endif
```

Every Debug run throws after the bump until handled. Re-test whether the workaround is still needed; e05b25c deleted the block for 134. **When adding new temporary workarounds during your bump, reuse this same DEBUG version-guard-throw pattern** so the next upgrade cannot miss them.

---

## Phase 4 — Versions & build

### 4.1 Bump all four pins in the SAME commit

`Directory.Build.props`: `CefRedistVersion`, `CefRedistOSXVersion`, `CefRedistLinuxVersion` = new CEF version; `<Version>` = `<chromium major>.<chromium build>.1`. `Directory.Packages.props` needs no edit (it references these properties). The 134 branch forgot `<Version>` for 19 days (`aa52940 "fix build"`).

### 4.2 Build every shipping configuration

```
dotnet build Xilium.CefGlue.sln -c Debug   -p:Platform=x64
dotnet build Xilium.CefGlue.sln -c Release -p:Platform=x64
dotnet build Xilium.CefGlue.sln -c Release -p:Platform=ARM64
dotnet build Xilium.CefGlue.sln -c ReleaseWPFAvalonia -p:Platform=x64
```

**Always pass `-p:Platform`** — AnyCPU makes the Platform-conditioned ItemGroups match nothing and CEF natives are silently not copied. Iterate compile errors back through Phase 3.

---

## Phase 5 — Validation

Every item below maps to a **real post-bump regression** on `main`; skipping one repeats history.

### 5.1 API-hash gate + regression suite

```
dotnet test CefGlue.Tests\CefGlue.Tests.csproj -c Debug -p:Platform=x64
```

110 NUnit tests; the fixture boots real CEF, so the first browser creation exercises `CheckVersionByApiHash` — a bindings/redist mismatch fails everything immediately with `CefVersionMismatchException`. (`NotSupportedException "Can't find CEF"` instead means natives weren't copied — a Platform/wiring failure, not a version mismatch.) Needs an interactive desktop; sequential only; the 30 s per-test timeout is inactive in Debug.

### 5.2 Demo smoke checklist (historical-regression derived)

Run both demos (`dotnet run --project CefGlue.Demo.Avalonia\... -p:Platform=x64`; likewise WPF):

| Check | Regression it guards against |
|---|---|
| Launch the demo **twice concurrently** | CEF 120+ process-singleton per `root_cache_path` (5749312 #175, 6a82a14 #176) |
| Open a DevTools popup; no NRE; handler base-calls intact | 3a03c28 #127, 37fb0f3 #206 (`extraInfo` can be null) |
| Browse youtube.com / media-heavy site | 0b5abd3 #184 (FirstPartySets crash) |
| WebGL: https://get.webgl.org/ + aquarium sample | GPU-process failure on 134 (6c03af7 — fixed by `app.manifest` with `supportedOS` on the subprocess exe) |
| Deep/complex pages (long sessions) | Subprocess stack overflow — the `editbin /STACK:0x800000` target must fire for both direct builds and `PublishApp` re-invocations (e0a716f #171, e74d0cc #172); requires `/p:VcvarsFile=...` on Windows |
| JS evaluation + bound-object round-trips (Avalonia demo menus) | Renderer IPC / serialization regressions |
| OSR mode: build demos `-c Debug_WindowlessRender` and repeat basics | OSR untested on >120 — the 134 branch removed the **Avalonia** demo's `WINDOWLESS` path during debugging (the WPF demo's path and the configuration itself remain) |

### 5.3 Verify the copied native payload deliberately

The copy logic is wildcard-driven and absorbs file-set drift **without any build error**. Check the output folder against the new distribution: expected 120→134 drift was `+dxcompiler.dll +dxil.dll −snapshot_blob.bin`; `locales\*.pak` present under `runtimes\win-{x64,arm64}\native\locales` (769662f #179); and the **three hardcoded macOS dylib names** in `CefGlue.Common\build\CefGlue.Common.targets` (l.21–41: `libEGL.dylib`, `libGLESv2.dylib`, `libvk_swiftshader.dylib`) still exist in the new distribution. *Automatable: yes — file-list assertion.*

### 5.4 Publish & pack validation

- `CefGlue.Tests\publish.cmd Release` (self-contained win-x64 publish) → confirm `libcef.dll` + locales + `CefGlueBrowserProcess\` land in the output.
- `dotnet build Xilium.CefGlue.sln -c Release -p:Platform=x64` → inspect the `CefGlue.Common` nupkg in `Nuget\output`: its nuspec dependencies must declare the **new** redist versions. Rebuild `CefGlue.BrowserProcess` explicitly first — incremental `PublishApp` can pack stale subprocess binaries without error (NU5100/NU5118 are suppressed).

### 5.5 macOS / Linux

Both need a real run of the Avalonia demo (mixed-pin mistakes only manifest at runtime there). Linux ARM64 additionally needs the `LD_PRELOAD`/`patchelf` workaround from [LINUX.md](LINUX.md). There is no in-repo CI for any of this — validation is manual.

---

## Phase 6 — Release & downstream train

1. Merge to `main`; packages are produced by a Release build (`GeneratePackageOnBuild` → `Nuget\output`); publishing to nuget.org happens outside this repo (release creation is automated per the wiki — validate on the Azure Pipelines ArtifactRepository dashboard). Git tags are not maintained.
2. Downstream, in order (wiki steps 7–10, each: bump package refs → fix API fallout → run sample app + tests → merge & release):
   **WebViewControl** (OutSystems/WebView) → **ReactViewControl** (OutSystems/ReactView) → **UIEditor** validation → **Service Studio** (O11IDE), with a test party against the scenarios doc and **explicit monitoring of `CefGlueBrowserProcess` memory/process counts** — new leaks tend to emerge here.
3. Budget an aftershock window: history shows packaging fixes within days (4b3b227 one day after the 117 bump) and behavior regressions surfacing over 15 months (DevTools popup, stack size, singleton cache, locales, YouTube, null `extraInfo`). Triage new-CEF behavior changes as bump work even when they masquerade as app bugs.

---

## Automation assessment

| Step | Automatable? |
|---|---|
| 0.1 version selection pre-flight (nuget.org + stable check) | ✅ yes |
| 0.2 OutSystems redist publishing | ⚠️ external pipeline (manual trigger today) |
| 1 header swap | ✅ yes |
| 2.1 generator breakpoint adaptations | ❌ judgment (✅ portable from 134 branch for ≤134) |
| 2.2 schema registration | ✅ mostly — discovery loop scriptable; role mechanical from the `source=` annotation (2.4); `reversible`/`autodispose` via signature scan |
| 2.3 generator run + hygiene | ✅ yes |
| 2.5 new-feature triage | 🟡 Level-1 defaults mechanical; Level-0/Level-2 decisions need humans |
| 3.1–3.2 struct mirrors & enums | 🟡 diff extraction scriptable; edits need review (highest-risk step) |
| 3.3–3.5 hand-written halves & fork layers | 🟡 compile-error driven; mechanical for small bumps, judgment for large |
| 3.6 workaround guards | ✅ detection; ❌ keep/drop decision (needs retest) |
| 4 version pins + build matrix | ✅ yes |
| 5.1/5.3/5.4 tests, payload assert, pack inspection | ✅ yes (needs interactive desktop for tests) |
| 5.2 demo smoke checklist | 🟡 scriptable via UI automation; today manual |
| 6 downstream train | ⚠️ separate repos/pipelines |

---

## CEF-134 branch: state of play

Branch `RDEV-8412-bump-cef-to-134.3.9` (11 commits ahead of `main`, tip `a7bc617`, last touched 2025-04-24, **not merged**). Per the maintainers: essentially complete, but with unknown runtime issues to investigate.

**Done (reusable):** full header swap to 134.3.9; all four generator breakpoint fixes from 2.1 (`get_capi_name`, `cef_api_versions.h` parsing, `api_hash(int,int)`, `CEF_API_VERSION=13401`); regeneration; API migrations (Extensions API deleted; `CefFrame.Identifier` `long`→`string`; `GetFrame` split into `GetFrameByName`/`GetFrameByIdentifier`; `OnBeforePopup` +`popupId`; new `OnBeforePopupAborted`/`OnRenderProcessUnresponsive` (`OnBeforeDevToolsPopup` already existed since the 120 bump); `OnAcceleratedPaint` → `CefAcceleratedPaintInfo` with per-platform impls; `CefRuntimeStyle`; `CefSettings` field changes); FirstPartySets guard removed as designed; GPU-process fix via `CefGlue.BrowserProcess\app.manifest` (`supportedOS` section — functional part of 6c03af7); boolean switches migrated to single-arg `AppendSwitch(name)` (727a1c8; Chromium 134 rejects `=1` forms in some paths); DevTools popups pinned to `RuntimeStyle.Chrome` (default runtime style flipped to Chrome in CEF 128+, changing behavior vs 120's Alloy).

**Open / must fix before merging:**
- **The undiagnosed Windows runtime issue**: tip commit forces `settings.NoSandbox = true` on Windows; a commented-out `disable-features=NetworkServiceSandbox` switch and verbose-log telemetry to a hardcoded `C:\git\CefGlue\cef_main_log.txt` remain in `BrowserCefApp.cs` — the suspicion is network-service/sandbox related. Decide: diagnose properly, or accept NoSandbox (CEF 134's Windows sandbox needs `cef_sandbox_info` plumbing CefGlue doesn't implement). Strip the telemetry either way.
- Debug leftovers to revert: demo default URL is `https://get.webgl.org/` (was google.com); the **Avalonia** demo's `WINDOWLESS`/OSR path removed (WPF demo's path remains; OSR untested on 134); stray `Avalonia.ReactiveUI` PackageReference in the Avalonia demo.
- New generated surface not fully wired: `cef_get_exit_code` P/Invoke uncalled; audit `CefTaskManager`/`CefPreferenceObserver`/`CefSettingObserver` for missing hand-written counterparts.
- History is WIP-quality (`"d"`, `"s"`, squashed "tentative" messages) — squash/reword before merge.
- `<Version>` was seeded as `134.6998.178` (equal to the Chromium patch) — decide whether to keep the established `.1`-restart fork-revision scheme instead.

**CEF ≥136 warning:** CEF is moving to a `bootstrap.exe` loading scheme on Windows. The 134 branch deliberately stayed on P/Invoke loading; a 136+ bump may require rearchitecting subprocess startup beyond this playbook.

---

## Wiki vs reality

| Wiki claim | Reality |
|---|---|
| Step 1: pick latest version supported by original CefGlue repo | Obsolete (struck through in wiki) — upstream is inactive; OutSystems now fully owns the interop porting |
| Step 3: create patch of OutSystems changes vs upstream | Obsolete (struck through) — changes were merged upstream before it went inactive |
| Step 5: "smash files from `cefglue_original/<branch>`" | Obsolete in practice — no upstream branches to smash from since CEF 120; the process is now header-swap + regenerate + port (Phases 1–3) |
| Step 4: run `gen-cef3.sh` | On Windows use `gen-cef3.cmd`; must run from inside `CefGlue.Interop.Gen`; Python 3 |
| Step 4 says "regenerate some code" | Understates the work: schema registration, generator breakpoint fixes, and the whole Phase 3 hand-porting are not mentioned |
| Step 6: "build errors … may consume any time" | Correct, but the silent failure modes (struct-mirror drift, enum renumbering, missing managed wrappers) never produce build errors — see Phase 3 warnings |
| Wiki omits | Linux redist packages (`cef.redist.linux*`, OutSystems-published), the version-guard workaround pattern, the Editbin stack step, the validation checklist history, the `.tmpl.g.cs` scaffold workflow |

---

## Open questions (for maintainers)

1. Resolution of the 134 Windows sandbox/NetworkServiceSandbox issue (accept NoSandbox vs diagnose).
2. `<Version>` third component going forward: fork-revision counter (main's history) vs Chromium patch (134 branch seed).
3. Whether FirstPartySets-class workarounds are needed per new CEF version (requires media-site retesting each bump).
4. OSR/windowless support status on CEF >120 (untested since the 134 branch dropped the Avalonia demo's `WINDOWLESS` path).
5. How macOS/Linux validation should be institutionalized (no CI exists; mixed-pin failures only appear at runtime there).
6. Exact build/publish pipeline for `cef.redist.osx*` (nuspec carries no repo URL; only the Azure pipeline link in the wiki).
7. Whether to fix the known `FromNativeOrNull` generator bug during the next bump or keep diff-minimalism.
8. Plan for CEF ≥136 bootstrap-exe subprocess loading.
9. Supported Python version pin for the generator (nothing records the tested 3.x).
10. Whether win-arm64 gets any runtime validation (wired but no execution evidence in history).
