# CefGlue — Known Issues (Bug Audit)

Static-analysis audit of the CefGlue codebase, focused on the **Avalonia** browser control (windowed and OSR/windowless modes) and the JS ↔ .NET bridge — the primary OutSystems use case. As of version `120.6099.211` (CEF 120.1.8), `main` branch.

**These are latent defects found by code inspection, not reported field failures.** Each was independently verified by two reviewers who traced the full failing path in the current source; five candidates were refuted and dropped. Severities are the finders' ratings; where a verifier disagreed it is noted inline. Nothing here is a fix — each entry ends with a fix direction.

## Methodology

Five finders swept distinct lenses (threading/races, native-interop lifetime, Avalonia UI layer, JS-interop correctness, lifecycle/disposal) plus eight seed findings from prior documentation passes. Every candidate was then put through two independent skeptics — one instructed to *refute* by tracing the code, one re-deriving from scratch through the threading model — and kept only if **both** confirmed (`CONFIRMED`) with a concrete code trace. Line numbers are from the audited working tree and may drift; treat file + symbol as authoritative.

**Threading vocabulary** (referenced throughout): CEF runs with `MultiThreadedMessageLoop=true` on Windows/Linux, so there is a single CEF **UI thread** driving *all* browsers in the process. The **Avalonia UI thread** is separate. The **renderer main thread** lives in the `CefGlue.BrowserProcess` child process. Renderer→browser `CefProcessMessage`s are delivered on the CEF UI thread.

## Summary

| # | Sev | Bug | Mode |
|---|-----|-----|------|
| 1 | 🔴 Critical | .NET collections returned to JS arrive as `{$values:[…]}`, not arrays | both |
| 2 | 🔴 Critical | OSR: closing DevTools/any popup destroys the *main* browser's state | OSR |
| 3 | 🟠 High | JS→.NET bound calls run on the CEF UI thread → sync-over-async deadlocks every browser | both |
| 4 | 🟠 High | Non-atomic `lastTaskId++` → `EvaluateJavaScript` hangs forever / cross-wired results | both |
| 5 | 🟠 High | `RegisterJavascriptObject` throws on any object with overloaded/case-colliding methods | both |
| 6 | 🟠 High | JS call with wrong arity/type leaves the JS promise pending forever (+ V8 leak) | both |
| 7 | 🟠 High | `Dispose` racing async browser creation leaks browser + renderer process + pipe | both |
| 8 | 🟠 High | OSR drag + teardown: async-void exception crashes the whole app | OSR |
| 9 | 🟠 High | Generated `FromNativeOrNull` NRE on the documented NULL path (fork regression, all reversible handlers) | both |
| 10 | 🟠 High | OSR detach-before-create: unguarded `VisibilityChanged` NRE crashes UI thread | OSR |
| 11 | 🟠 High | Every OSR browser leaks a native top-level window (popup host never closed) | OSR |
| 12 | 🟡 Medium | macOS OSR completely non-functional — browser silently never created *(verifier: High)* | OSR/macOS |
| 13 | 🟡 Medium | Trackpad/precision wheel deltas truncated to 0 → OSR scrolling dead/jerky *(verifier: High)* | OSR |
| 14 | 🟡 Medium | `EvaluateJavaScript` hangs forever if renderer can't enter V8 context (nav race) | both |
| 15 | 🟡 Medium | `NativeObjectRegistry` lock-free dictionary read races writers → intermittent failures | both |
| 16 | 🟡 Medium | `CefRuntimeLoader.Load` check-then-act → concurrent first-browser double-inits CEF | both |
| 17 | 🟡 Medium | OSR visibility toggle racing close → async-void NRE crashes app | OSR |
| 18 | 🟡 Medium | `Bind()` path never disposes `PromiseHolder` → V8 handle leak per bind | both |
| 19 | 🟡 Medium | OSR paint after Dispose resurrects MMF + bitmap, never released again | OSR |
| 20 | 🟡 Medium | `AvaloniaRenderSurface` disposes bitmap outside the render lock → write-after-free | OSR |
| 21 | 🟡 Medium | OSR popup crashes when placement target is detached / not under a `Window` | OSR |
| 22 | 🟡 Medium | Broken `$id`-collision guard: user's `$id` corrupts the reference graph | both |
| 23 | 🟡 Medium | Renderer unregistration needs a live V8 context → registries permanently desync | both |
| 24 | 🟡 Medium | Bound `async Task` methods resolve JS promise with `{}` instead of `null` (timing-dependent) | both |
| 25 | 🟡 Medium | `ProcessExit` shutdown runs with live browsers + can double-`shutdown` libcef | both |
| 26 | 🟡 Medium | `PipeServer` dies permanently after 6 cumulative errors → crash reports lost | both |
| 27 | 🟡 Medium | `EvaluateJavaScript` returns `default(T)` silently on out-of-band cancel *(verifier: Low)* | both |
| 28 | ⚪ Low | `PipeStream.ReadString` ignores partial reads/EOF → corrupt/lost crash reports | both |
| 29 | ⚪ Low | Shared render process: crash-pipe name overwritten → reports routed to dead pipe | both |
| 30 | ⚪ Low | Unsynchronized delegate-cache publication → null-delegate NRE on ARM64 | both/ARM64 |

**Refuted (investigated, not real):** see [Appendix](#appendix-refuted-candidates).

---

## Critical

### 1 — .NET collections returned to JS arrive as `{$values:[…]}` wrapper objects, not arrays
**`CefGlue.BrowserProcess/ObjectBinding/CefGlueGlobalScript.js`** (reviver `revive`, ~line 32) ↔ **`CefGlue.Common.Shared/Serialization/Serializer.cs:24`**

The serializer uses `ReferenceHandler.Preserve`, which emits every `List<T>`/array/collection as `{"$id":"N","$values":[…]}`. On the renderer side, `parseResult`/`revive` handles `$id` (strips it, registers refs) and `$ref`, but **never unwraps `$values`** — the `valuesPropertyName` constant is used only in `objectsStringifier` (the JS→.NET direction). So a bound method returning `List<string>`/`T[]`, or any object with a collection property (nested too), resolves the JS promise with `{$values:[…]}` instead of the array: `Array.isArray(result)` is `false`, `result.length` is `undefined`.

- **Trigger:** any bound `.NET` method returning a collection to JS. Deterministic. `byte[]` is exempt (handled by `BinaryJsonConverter` as a marked base64 string); scalars and plain objects work.
- **Why it survived:** `NativeObjectInteropTests` only round-trips `null`/string/DateTime/`Person` (whose only collection, `byte[] Photo`, bypasses `Preserve`). No test returns a collection to JS. `SerializationTests` pass because they exercise the .NET `Deserializer`, which *does* unwrap `$values` — the JS reviver is the only side missing it.
- **Fix direction:** teach `revive`/`parseResult` in `CefGlueGlobalScript.js` to unwrap a `$values`-bearing object into its array (mirroring `Deserializer`), and add a test returning `List<T>` and a collection-valued property to JS.

### 2 — OSR: closing DevTools or any self-closing popup destroys the main browser's state
**`CefGlue.Common/CommonOffscreenBrowserAdapter.cs:311`** (`OnBrowserClose` override)

In OSR mode, `ShowDeveloperTools` (`CommonBrowserAdapter.cs:265`) creates the DevTools browser with the **same** `CommonCefClient`, so its lifespan callbacks route to the same adapter. The windowed base `OnBrowserClose` (`CommonBrowserAdapter.cs:478-483`) returns early for `browser.IsPopup` (*"popup such as devtools, let it close its window"*), but the **OSR override omits that guard** and unconditionally calls `Cleanup(browser)`. `Cleanup` (`CommonBrowserAdapter.cs:498-510`) disposes the *main* browser's crash `PipeServer`, `JavascriptExecutionEngine`, and `NativeObjectRegistry`, and nulls `BrowserHost`/`_cefClient`/`_browser` while the main browser is still alive.

- **Result:** all input handlers no-op (`BrowserHost` null), `IsInitialized` becomes false (Address setter silently stashes URLs), `EvaluateJavaScript` returns default, registered JS objects vanish. A later `Dispose()` sees `BrowserHost == null` and never calls `CloseBrowser`, so the real `CefBrowser` + renderer subprocess leak until process exit — where `CefRuntime.Shutdown` then runs with a live browser.
- **Trigger:** open+close DevTools once, or any page whose popup closes itself (`window.open`, OAuth popups — allowed by the default `OnBeforePopup` with the shared client).
- **Avalonia impact:** `AvaloniaCefBrowser` in OSR mode (`WindowlessRenderingEnabled=true`) is bricked by everyday interactions.
- **Fix direction:** add the `browser.IsPopup` early-return (delegating to `base.OnBrowserClose`) to the OSR override, matching the windowed adapter.

---

## High

### 3 — JS→.NET bound calls execute synchronously on the CEF UI thread → sync-over-async deadlocks every browser
**`CefGlue.Common/ObjectBinding/NativeObjectMethodDispatcher.cs:34`**

`HandleNativeObjectCall` runs inside `CommonCefClient.OnProcessMessageReceived` on the CEF UI thread and invokes the registered method **inline** (`NativeObject.InnerExecuteMethod` → `NativeMethod.Execute` → `MethodInfo.Invoke`). The former off-thread `ActionBlock` dispatcher was removed in PR #41 with no replacement safeguard. If a bound method blocks — `browser.EvaluateJavaScript<T>(…).Result`, or waiting on the Avalonia dispatcher — it deadlocks *by construction*: the result it waits for can only be delivered via `OnProcessMessageReceived` on the very thread it is blocking. With `MultiThreadedMessageLoop=true` that thread drives **all** browsers, so navigation/paint/input of every browser instance freeze permanently.

- **Avalonia impact:** the primary bridging scenario — one synchronously-waiting bound method hangs every `AvaloniaCefBrowser` in the process; even a merely *slow* method stalls the whole UI.
- **Fix direction:** dispatch user-method execution off the CEF UI thread (restore an off-thread pump or post to a task scheduler), or document a hard "bound methods must not block / must be `async`" contract and detect reentrant `EvaluateJavaScript`.

### 4 — Non-atomic `lastTaskId++` → `EvaluateJavaScript` hangs forever or returns another script's result
**`CefGlue.Common/JavascriptExecution/JavascriptExecutionEngine.cs:15,67`** *(merges two findings on lines 30/67)*

`lastTaskId` is a `static volatile int` shared across **all** engines; `var taskId = lastTaskId++` is a read-modify-write that `volatile` does not make atomic. Two concurrent `Evaluate` calls can get the same id. `_pendingTasks.TryAdd(taskId, tcs)` (line 78) ignores its return value, so the loser's TCS is never stored: with the default `timeout == null`, that `await` hangs forever (not even `Dispose` can cancel it — it isn't in the dictionary). Worse, two `JsEvaluationResult`s share a `TaskId`; `HandleScriptEvaluationResultMessage` `TryRemove`s on first arrival, so **caller A can receive caller B's result** deserialized as A's `T`.

- **Trigger:** `EvaluateJavaScript` called concurrently from more than one thread, or from two browser views (the counter is `static`).
- **Fix direction:** `Interlocked.Increment`; and check the `TryAdd` return / assert uniqueness.

### 5 — `RegisterJavascriptObject` throws `ArgumentException` for any object with overloaded (or case-colliding) methods
**`CefGlue.Common/ObjectBinding/NativeObject.cs:78`**

`GetObjectMembers` does `methods.ToDictionary(m => ToJavascriptMemberName(m.Name), …)` over *all* public instance methods (including inherited). `ToJavascriptMemberName` only lowercases the first char, so any two overloads (`Log(string)` / `Log(string,int)`), a class implementing `IEquatable<T>` (`Equals(T)` + inherited `Equals(object)` → key `equals`), or two methods differing only in first-letter case (`Test`/`test`) collide → `Dictionary.Add` throws synchronously out of `RegisterJavascriptObject`.

- **Avalonia impact:** binding an ordinary .NET object crashes the Avalonia UI thread at registration; only workaround is a hand-crafted facade with unique names. No test covers overloads.
- **Fix direction:** detect duplicates and disambiguate or throw a clear, documented error naming the colliding methods; consider skipping `object`-inherited methods.

### 6 — JS call with wrong argument count/type leaves the JS promise pending forever (and leaks the `PromiseHolder`)
**`CefGlue.Common/ObjectBinding/NativeMethod.cs:42`**

`Execute<T>` evaluates `ConvertArguments(args)` as an argument expression **before** entering its own try/catch. Missing mandatory args (`ValidateMandatoryArguments` → `ArgumentException`) or a deserialization type mismatch (`AssertToken` → `InvalidOperationException`) propagate out of `HandleNativeObjectCall` without `handleResult` ever running — so no `NativeObjectCallResult` is sent. `CommonCefClient.OnProcessMessageReceived`'s catch swallows it; the renderer keeps the `PromiseHolder` (and its entered V8 context ref) until navigation, and the page's `await` never settles. Same hole on the `MakeDelegate` path (`NativeObject.cs:58`).

- **Contract violation:** elsewhere (the `MethodExceptionIsReturned` test) errors are expected to *reject* the promise.
- **Fix direction:** move `ConvertArguments` inside the try/catch so conversion failures flow to `SendResult` as a rejection.

### 7 — `Dispose` racing async browser creation leaks the CEF browser, renderer process, and crash-pipe server
**`CefGlue.Common/CommonBrowserAdapter.cs:431`** (`OnBrowserCreated`) / `Dispose` at lines 64-101

`CefBrowserHost.CreateBrowser` (line 317) is asynchronous; `OnBrowserCreated` fires later on the CEF UI thread. `Dispose` only calls `CloseBrowser` when `BrowserHost` is already set (lines 87-94). If the control is disposed within the creation window (open a view, close its tab within the renderer-spawn latency — tens to hundreds of ms), `Dispose` sees `BrowserHost == null` and closes nothing; `OnBrowserCreated` then runs to completion (it only checks `_browser != null`), sets `BrowserHost`, starts the pipe listener, and calls `InitializeRender` on the disposed control. Nothing ever closes that browser.

- **Avalonia impact:** rapid open/close of views leaks a live `CefBrowser` + renderer subprocess + pipe task each time; in OSR there is no OS window whose destruction would trigger native cleanup, so it persists to app exit.
- **Fix direction:** a shared disposed-state flag checked in `OnBrowserCreated` under the same lock as `Dispose`; if disposed, immediately `CloseBrowser`.

### 8 — OSR drag + teardown: async-void exception crashes the whole application
**`CefGlue.Common/CommonOffscreenBrowserAdapter.cs:413`** (`HandleStartDragging`)

`WithErrorHandling` takes `Action`, so the `async () => { … await Control.StartDrag(…); BrowserHost.DragSourceEndedAt(…); }` lambda compiles to **async void**. Its try/catch covers only up to the first `await`; any exception afterward is rethrown on the ThreadPool with no `SynchronizationContext` → process termination. Concrete trigger: user drags content out of the OSR browser, the browser is disposed mid-drag (`Cleanup` nulls `BrowserHost`), the `await` completes, and `BrowserHost.DragSourceEndedAt(…)` throws `NullReferenceException`. An exception from Avalonia's `DoDragDrop` has the same effect.

- **Fix direction:** add a `Func<Task>` overload of `WithErrorHandling` that awaits inside the try/catch, and null-check `BrowserHost` after the await.

### 9 — Generated `FromNativeOrNull` null-derefs on the documented NULL path (fork regression, all reversible handlers)
**`CefGlue/Classes.g/CefUserData.g.cs:31`** and 6 sibling `.g.cs` files; root cause in **`CefGlue.Interop.Gen/make_interop.py:548-559`**

Commit `9876422` ("memleak: auto release for Handler values") changed the generator template to emit `found = _roots.TryGetValue((IntPtr)ptr, out value); value.release(ptr);` with **no null/found check before dereferencing `value`**. When the native call returns NULL, `TryGetValue(IntPtr.Zero)` fails, `value` is null, and `value.release(ptr)` throws `NullReferenceException` — the `return found ? value : null` contract is unreachable for the null case. Identical code is in `CefClient.g.cs`, `CefV8Handler.g.cs`, `CefUrlRequestClient.g.cs`, `CefRequestContextHandler.g.cs`, `CefExtensionHandler.g.cs`, `CefV8ArrayBufferReleaseCallback.g.cs`.

- **Guaranteed triggers (all documented as returning NULL):** `CefV8Value.GetUserData()` on any object without user data; `GetFunctionHandler()` on a non-CEF-created function; `GetArrayBufferReleaseCallback()` on a foreign ArrayBuffer; `CefRequestContext.GetHandler()`; `CefExtension.GetHandler()`.
- **Impact:** deterministic crash on the "no value" case, on whatever thread calls the getter; in a renderer V8 getter it can take down the render process (blank browser).
- **Fix direction:** fix the template in `make_interop.py` (guard the `release`/deref on `found`) and regenerate all affected `.g.cs` files.

### 10 — OSR detach-before-create: unguarded `VisibilityChanged` invocation NREs on the UI thread
**`CefGlue.Avalonia/Platform/AvaloniaOffScreenControlHost.cs:187`**

`OnDetachedFromVisualTree` calls `VisibilityChanged(false)` directly — no `?.Invoke` (unlike line 192, and unlike the WPF twin at `WpfOffScreenControlHost.cs:333`). The only subscriber is attached in `SetupBrowserView`, which runs only after `CreateBrowser` succeeds on the first layout tick. Add a browser to the tree and remove it before a layout pass completes (fast tab switch, virtualized list, add+remove same frame, or any platform where `GetHostViewHandle` returns null so creation never runs) → `VisibilityChanged` is null at detach → `NullReferenceException` on the Avalonia UI thread (unhandled dispatcher exception terminates the app). The same unguarded call fires for the popup host when the popup window is closed while a `<select>` is open.

- **Fix direction:** use `?.Invoke` (one-char fix), matching the WPF implementation.

### 11 — Every OSR browser leaks a native top-level window (popup host never closed)
**`CefGlue.Avalonia/AvaloniaCefBrowser.cs:37`** / `CefGlue.Avalonia/Platform/AvaloniaPopup.cs:54`

In OSR mode `BaseCefBrowser`'s ctor eagerly calls `CreatePopupHost()`, constructing `new ExtendedAvaloniaPopup { PlacementTarget = this }` — an Avalonia `Window` whose native platform window is allocated immediately. Nothing ever closes it: `IOffScreenPopupHost.Close()` only calls `_popup.Hide()`, and `InnerDispose` disposes only the two render surfaces. After `AvaloniaCefBrowser.Dispose()`, the un-closed `Window` stays alive (its platform impl self-registers in a static list and its delegates close over it), and via `PlacementTarget` it GC-roots the entire disposed control graph.

- **Avalonia impact:** apps that open/close OSR browsers repeatedly (tabs, previews) accumulate one leaked native window handle + one unreclaimable control tree per browser.
- **Fix direction:** `Close()` (not just `Hide()`) and dispose the popup `Window` in the adapter's dispose path; clear `PlacementTarget`.

---

## Medium

### 12 — macOS OSR is completely non-functional (browser silently never created) — *verifier rated High*
**`CefGlue.Avalonia/Platform/AvaloniaControl.cs:58`** (`GetHostWindowPlatformHandle`)

Since commit `0868585` ("Linux support (Final)"), the platform switch handles only Windows and Linux; previously it unconditionally did `new Window().TryGetPlatformHandle()`, yielding the `IMacOSTopLevelPlatformHandle` that `AvaloniaOffScreenControlHost.GetHostViewHandle` still checks for. Now on macOS it returns null → `GetHostViewHandle` returns null → `CreateBrowser` returns false on every size-changed retry, forever. No exception, no log; the dead `is IMacOSTopLevelPlatformHandle` check proves the intent was lost in the refactor.

- **Impact:** `WindowlessRenderingEnabled` on macOS = permanently blank control. Since macOS ships Avalonia-only, OSR is dead on that platform.
- **Fix direction:** restore a macOS case returning the top-level platform handle.

### 13 — Trackpad / precision wheel deltas truncated to zero — *verifier rated High*
**`CefGlue.Avalonia/Platform/AvaloniaOffScreenControlHost.cs:142`**

`OnPointerWheelChanged` computes `(int)e.Delta.X * MouseWheelDelta` — the cast binds tighter than `*`, so the fractional delta is truncated *before* scaling. Avalonia delivers sub-unit deltas for precision touchpads/macOS trackpads (e.g. 0.25), so `(int)0.25 * 100 == 0`: no scroll. Even supra-unit deltas lose their fraction (1.75 → 100 instead of 175). The WPF twin is unaffected (WPF deltas are already ±120 integers).

- **Impact:** on laptops/trackpads (the common desktop case), OSR scrolling is dead for slow scrolling and coarse/jumpy otherwise.
- **Fix direction:** multiply first, then cast: `(int)(e.Delta.Y * MouseWheelDelta)`.

### 14 — `EvaluateJavaScript` hangs forever if the renderer can't enter the V8 context (navigation race)
**`CefGlue.BrowserProcess/JavascriptExecution/JavascriptExecutionEngineRenderSide.cs:18`**

`HandleJavascriptEvaluation` calls `frame.V8Context.EnterOrFail()` before building any response. If the frame's context is gone (message raced a navigation) or `V8Context` is null, this throws; `WithErrorHandling` turns it into an `UnhandledException` message instead of a `JsEvaluationResult`, so the browser-side `_pendingTasks[taskId]` TCS is never completed. With `timeout` defaulting to null, the caller awaits forever; nothing removes pending tasks on `JsContextReleased` either.

- **Trigger:** `EvaluateJavaScript` issued around a navigation/reload — common in startup/refresh logic.
- **Fix direction:** on renderer-side evaluation failure, send a failed `JsEvaluationResult` (carrying the `TaskId`) rather than a generic `UnhandledException`; also drain pending tasks on context release.

### 15 — `NativeObjectRegistry` dictionary read races writers
**`CefGlue.Common/ObjectBinding/NativeObjectRegistry.cs:83`** (`Get`) and the pre-lock `ContainsKey` at line 23

`Get` does `TryGetValue` on a plain `Dictionary` with **no lock**, while `Register`/`Unregister`/`Dispose` mutate it under `_registrationSyncRoot`. `Get` runs on the CEF UI thread for every JS call; register/unregister run on the Avalonia UI thread (late registration is a supported flow). A `TryGetValue` racing an `Add`-triggered resize or a `Remove` is undefined on `Dictionary`: it can throw or return a spurious miss → a JS call fails with "Object named X was not found" or an `UnhandledException`.

- **Fix direction:** switch to `ConcurrentDictionary`, or read under `_registrationSyncRoot`.

### 16 — `CefRuntimeLoader.Load` check-then-act double-initializes CEF
**`CefGlue.Common/CefRuntimeLoader.cs:116`** + `BaseCefBrowser.cs:23-25`

`BaseCefBrowser`'s ctor does `if (!CefRuntimeLoader.IsLoaded) CefRuntimeLoader.Load()` and `Load` does `if (_delayedInitialization != null) { invoke; null; }` — neither synchronized. Two threads constructing the first browsers concurrently both see `IsLoaded == false` and both run `InternalInitialize`: either the second throws `CefRuntimeAlreadyInitialized` out of the ctor, or (interleaving before `_initialized = true`) native `cef_initialize` runs twice concurrently (UB → crash/hang).

- **Trigger:** browsers created on more than one thread during startup (parallel windows, background pre-warm). Typical single-UI-thread creation is safe.
- **Fix direction:** guard `Load`/`Initialize` with a lock or `LazyInitializer`.

### 17 — OSR visibility toggle racing close → async-void NRE crashes app
**`CefGlue.Common/CommonOffscreenBrowserAdapter.cs:202`** (`HandleVisibilityChanged`)

Checks `BrowserHost != null` on the Avalonia UI thread, then posts an `ActionTask` to the CEF UI thread whose body calls `BrowserHost.WasResized()` with no null check (only the post-delay call at 206 is guarded). If the browser closes in between (`Cleanup` nulls `BrowserHost` on the CEF UI thread), the posted task NREs. *(Verifier correction: the posted lambda is async-void via `ActionTask.Run(Action)`, so it crashes via the unobserved-async-void path, not the `CefTask` boundary as originally hypothesized — but the crash is real.)*

- **Fix direction:** null-check `BrowserHost` inside the posted body; give `ActionTask` a `Func<Task>` overload.

### 18 — `Bind()` path never disposes `PromiseHolder` → V8 handle leak per bind
**`CefGlue.BrowserProcess/ObjectBinding/JavascriptHelper.V8BuiltinFunctionHandler.cs:52`**

`CreatePromise` deliberately `Untrack`s the promise/resolve/reject `CefV8Value` wrappers, transferring lifetime to the caller. The native-call path honors this (`using (promiseHolder)` + `HandleContextReleased`), but the `Bind` path's `ContinueWith` calls `ResolveOrReject` and never disposes the holder — the three native V8 refs leak, released only when the GC finalizes the wrappers (off-thread, violating CEF's V8 thread affinity). If the bound object is never registered, the holder plus the entered context leak with no cleanup path at all.

- **Avalonia impact:** `RegisterJavascriptObject` + `cefglue.Bind()` is the core bridge flow; every bind on every page load leaks V8 handles → steady renderer growth in long-lived SPA sessions.
- **Fix direction:** dispose the `PromiseHolder` after `ResolveOrReject`; track Bind promises so context-release can clean them up.

### 19 — OSR paint after Dispose resurrects the MemoryMappedFile + bitmap, never released again
**`CefGlue.Common/Helpers/OffScreenRenderSurface.cs:88`**

`Dispose` releases the MMF and nulls the bitmap, but sets no disposed flag and leaves `_width`/`_height` intact. `CloseBrowser` is async, so CEF keeps delivering `OnPaint`; `HandleViewPaint` has no disposed check. The next `Render` finds `_viewAccessor == null` (indistinguishable from "never allocated") and **recreates** the MMF + view accessor, then the UI callback recreates the `WriteableBitmap` and reassigns `Image.Source`. Nothing ever disposes these resurrected objects.

- **Impact:** every OSR browser close leaks ~2 × W × H × 4 bytes (~16 MB at 1440p) until finalization, or forever if the control instance is retained (cached tab).
- **Fix direction:** add a disposed flag checked in `Render`/`HandleViewPaint`; ignore paints after dispose.

### 20 — `AvaloniaRenderSurface` disposes the bitmap outside the render lock → write-after-free
**`CefGlue.Avalonia/AvaloniaRenderSurface.cs:28`**

`Dispose` runs `base.Dispose()` (which takes `_renderLock` only transiently) then `_bitmap?.Dispose(); _bitmap = null` **without** the lock and with no disposed flag. If `BaseCefBrowser.Dispose()` is called off the UI thread while a paint has resurrected the surface (see #19) and a queued UI callback is mid-`InnerRender` holding `_renderLock` — between `_bitmap.Lock()` and the `Buffer.MemoryCopy` — the disposing thread frees the framebuffer concurrently and the memcpy lands in freed native memory. (Separately, `MemoryCopy`'s destination-size arg is passed as `sourceBufferSize`, not the actual locked-buffer size, so the safety check can never fire.)

- **Impact:** native heap corruption / AccessViolation during OSR teardown, surfacing as a random Skia crash after closing a browser.
- **Fix direction:** dispose `_bitmap` inside `_renderLock` behind a disposed flag; pass the true destination size to `MemoryCopy`.

### 21 — OSR popup crashes when the placement target is detached or not under a `Window`
**`CefGlue.Avalonia/Platform/AvaloniaPopup.cs:48`**

`Open()` posts `_popup.Show(_popup.PlacementTarget.GetVisualRoot() as Window)`. If the control is detached by the time the posted lambda runs (dropdown open racing tab close), `GetVisualRoot()` is null → `Window.Show(null)` throws `ArgumentNullException`. If the browser is hosted under a non-`Window` root (inside an Avalonia flyout/popup, root is `PopupRoot`), the `as Window` cast is null and **every** `<select>` dropdown throws. `MoveAndResize` (line 37) similarly calls `PointToScreen` on a possibly-detached visual. Both surface as unhandled dispatcher exceptions (the posted lambdas run outside `WithErrorHandling`).

- **Fix direction:** null-check the visual root and bail gracefully; support non-`Window` roots.

### 22 — Broken `$id`-collision guard: a user's `$id` corrupts the reference graph
**`CefGlue.BrowserProcess/ObjectBinding/CefGlueGlobalScript.js:124`**

When a JS object passed to a native method carries its own `$id`, the code intends `Object.assign(tmpObj, value, tmpObj)` to let the generated id win — but source 1 (`value`) overwrites `tmpObj.$id`, and source 3 *is* `tmpObj` (a no-op self-copy), so the **user's `$id` survives**. The serialized graph then carries the wrong id; a later `$ref:"N"` fails .NET deserialization (`Deserializer.cs:183` "can't resolve $ref"), or two identical literal `$id`s throw `ArgumentException` in `referencesMap.Add` — and per #6 that arg-conversion failure never rejects the promise.

- **Trigger:** passing data that legitimately contains an `$id` field (e.g. CouchDB/Mongo-style docs echoed from a REST API).
- **Fix direction:** `Object.assign({}, value, tmpObj)` (fresh target).

### 23 — Renderer unregistration needs a live V8 context → registries permanently desync
**`CefGlue.BrowserProcess/ObjectBinding/JavascriptToNativeDispatcherRenderSide.cs:71`**

`HandleNativeObjectUnregistration` unconditionally does `frame.V8Context.EnterOrFail()` (and `frame` may be null from `GetMainFrame` during teardown → NRE). If `UnregisterJavascriptObject` is called before the first page finishes loading or during a navigation window, `EnterOrFail` throws, the message is consumed, and `DeleteNativeObject` never runs — the name stays in the renderer's registry. A subsequent `Register` with the same name is then silently ignored renderer-side (early-return), so the V8 proxy keeps the **old** method list while browser-side dispatch uses the new object. Registration handles this correctly (defers when context is null); unregistration does not.

- **Fix direction:** mirror the registration path — defer/queue unregistration when no context is available.

### 24 — Bound `async Task` methods resolve the JS promise with `{}` instead of `null` (timing-dependent)
**`CefGlue.Common/ObjectBinding/GenericTaskAwaiter.cs:46`**

`GetResultGetter` treats any generic task type as `Task<T>`. A genuinely-asynchronous `async Task` returns `AsyncStateMachineBox<…> : Task<VoidTaskResult>` — `IsGenericType` is true, so the `Result` getter yields a `VoidTaskResult` struct → serialized as `{}`, and JS resolves with `{}`. The *same* method completing synchronously returns the cached non-generic `Task.CompletedTask` (matched by the `_noop` entry) → resolves with `null`. So `await obj.doWorkAsync()` is `null` sometimes and `{}` (truthy!) other times, purely by timing.

- **Avalonia impact:** `async Task` is the idiomatic bound-handler signature; JS branching on the result behaves nondeterministically.
- **Fix direction:** detect `Task<VoidTaskResult>`/non-generic-`Task` equivalence and always resolve void tasks with `null`.

### 25 — `ProcessExit` shutdown runs with live browsers and can double-`shutdown` libcef
**`CefGlue.Common/CefRuntimeLoader.cs:69`**

`InternalInitialize` registers `ProcessExit += { CefRuntime.Shutdown(); }`. Nothing closes live browsers first — `BaseCefBrowser` finalizers run `Dispose(false)`, which deliberately skips `CloseBrowser` — so exiting with any un-disposed browser (the default, since Avalonia doesn't dispose controls) runs `libcef.shutdown()` with live browsers (a documented CEF violation: DCHECK/crash/hang at teardown). Also, `Shutdown()` never resets `_initialized`, so an app that also calls `CefRuntime.Shutdown` itself (exactly the demo's own `ProcessExit` handler, which runs first) invokes `libcef.shutdown()` twice.

- **Fix direction:** close/await all browsers before shutdown; guard `Shutdown` idempotently (reset/consult `_initialized`).

### 26 — `PipeServer` stops listening permanently after 6 cumulative errors → crash reports lost
**`CefGlue.Common.Shared/RendererProcessCommunication/PipeServer.cs:21`**

`errorCount` is declared once before the accept loop and incremented in a bare `catch`; it is **never reset** on a successful connection. After `errorCount > MaxErrorsAllowed` (5) — i.e. the 6th cumulative error over the server's whole lifetime — the loop `break`s permanently, with no log/event/flag. One `PipeServer` lives per browser (can be days), and error sources are realistic (EDR/AV pipe-enumeration probes, any connect-and-drop → see #28). After it dies, renderer crashes never reach the host's `UnhandledException`.

- **Fix direction:** reset `errorCount` on success; log/raise when giving up; distinguish fatal vs transient errors.

### 27 — `EvaluateJavaScript` returns `default(T)` silently on out-of-band cancellation — *verifier rated Low*
**`CefGlue.Common/JavascriptExecution/JavascriptExecutionEngine.cs:134`**

If the engine is disposed (navigation/browser close) while an evaluation is pending, the TCS is `TrySetCanceled` → the continuation observes `AggregateException(TaskCanceledException)`, and the catch filter at line 140 returns `default(T)`. The caller cannot distinguish this from a real null/0/false JS result. (Asymmetry proving the filter is misaimed: a real *timeout* throws a raw `TaskCanceledException`, which is *not* an `AggregateException`, so it rethrows and the caller correctly sees an exception.)

- **Fix direction:** let cancellation propagate as a `TaskCanceledException` (or a distinct result) rather than masquerading as a value.

---

## Low

### 28 — `PipeStream.ReadString` ignores partial reads and EOF → corrupt/lost crash reports
**`CefGlue.Common.Shared/RendererProcessCommunication/PipeStream.cs:22`**

`ReadString` issues a single `Read(buffer, 0, length)` and ignores the return count; a byte-mode pipe returns whatever is buffered, so a crash message larger than one pipe buffer (long stack traces) decodes garbage — and it's deterministic because `PipeServer` disposes the pipe right after the single read. `ReadInt` casts `ReadByte()` to `byte` without checking `-1` (EOF), so a connect-and-drop yields a bogus length → `OverflowException`, swallowed and counted toward the #26 budget.

- **Fix direction:** loop until `length` bytes are read; check `ReadByte() == -1`.

### 29 — Shared render process: crash-pipe name overwritten → reports routed to a dead pipe
**`CefGlue.BrowserProcess/Handlers/RenderProcessHandler.cs:113`**

`RenderProcessHandler` is process-wide, but `OnBrowserCreated` unconditionally replaces `_crashPipeName` and `_browser` with the newest browser's values (and popups arrive with null `extraInfo`, nulling the pipe name). When two host browsers share one render process (same origin, or a popup), disposing the newer one disposes its host-side `PipeServer`; a later crash in that renderer targets the dead pipe → `PipeClient.Connect` blocks up to 10 s then drops the report, so the surviving browser's `UnhandledException` never fires.

- **Fix direction:** key crash-pipe state per browser id; add `OnBrowserDestroyed` handling.

### 30 — Unsynchronized delegate-cache publication → null-delegate NRE on ARM64
**`CefGlue/Interop/Classes.g/*.g.cs`** (every proxy method); root cause in **`make_interop.py:248-263`**

Each generated proxy method caches `(pointer, delegate)` in plain statics: `if (p == _p1) d = _d1; else { …; if (_p1 == IntPtr.Zero) { _d1 = d; _p1 = p; } }`. The `_p1 = p` store has no release ordering after `_d1 = d`, and the reader's `_d1` load sits behind only a control dependency on `_p1` — which ARMv8 does not order. During the first-call window, a concurrent thread can observe `_p1 == p` while `_d1` is still null → `d(self)` NREs inside the refcount path. x64 is unaffected (TSO).

- **Impact:** Apple-Silicon macOS (a shipping target) can crash nondeterministically at browser startup deep in interop refcounting; effectively undiagnosable from dumps. Startup-only, very low probability.
- **Fix direction:** publish the cache with `Volatile.Write`/`Interlocked`, or order the writes (delegate last, with a barrier) in the generator template.

---

## Cross-cutting themes

- **Async-void via `Action`-typed helpers** (`WithErrorHandling`, `ActionTask.Run`) turns recoverable errors into process crashes — bugs #8, #17. A `Func<Task>` overload plus an audit of every `async () =>` lambda passed to an `Action` parameter would close the class.
- **The JS-interop error contract is not honored end-to-end** — #6, #14, #22, #23 all leave a JS promise pending forever instead of rejecting. A single "always settle the promise" invariant (send a failure result on every exception path) addresses them together.
- **Dispose/create races lack a shared disposed-state gate** — #7, #17, #19, #20, and the async-creation window. A single guarded lifecycle flag consulted by `OnBrowserCreated`, `HandleViewPaint`, and the visibility/drag handlers is the common fix.
- **Generator-template bugs replicate widely** — #9 (252 files) and #30 (every proxy). Fix in `make_interop.py` and regenerate; see [CEF-UPGRADE.md](CEF-UPGRADE.md).
- **The crash-reporting pipe is fragile end-to-end** — #26, #28, #29 compound: transient errors kill the listener, partial reads corrupt payloads, and shared render processes misroute. It silently stops doing its one job.
- **OSR mode carries the most defects** (the two criticals, plus #8/#10/#11/#12/#13/#17/#19/#20/#21) — consistent with it being less exercised than windowed mode. Given Avalonia + OSR is the shipping priority, this is where hardening pays off most.

## Appendix — refuted candidates

Investigated and found **not** to be real bugs in the current code (both reviewers concurred):

1. **ABBA deadlock between `FromNativeOrNull` and `add_ref`/`release`** — the lock-order inversion is syntactically present, but the inner `_roots` acquisitions are guarded to the 0→1 / 1→0 refcount transitions, so the two orders can't be in flight simultaneously.
2. **`(string[])` cast of `GetFileNames()` in drag-enter throwing `InvalidCastException`** — guarded by a `Contains(DataFormats.FileNames)` check; on every shipping platform the value under that guard really is `string[]` in the pinned Avalonia 11.0.9.
3. **`NativeObjectUnregistrationRequest.FromCefMessage` returning the wrong type breaking unregistration** — the odd return type is real but harmless; the fields read are compatible and unregistration works end-to-end. (Cosmetic only.)
4. **`CommonCefClient.OnProcessMessageReceived` use-after-dispose** — the `using (message)` does dispose before `base.OnProcessMessageReceived`, but the base implementation doesn't touch the message, so nothing fails.
5. **`AvaloniaRenderSurface` full-frame `MemoryCopy` size mismatch on resize** — the render path drops paints whose size doesn't match the current bitmap (size precheck), so old-size-paint vs new-size-bitmap is structurally impossible. *(Note: the distinct write-after-free on Dispose, #20, is real.)*
