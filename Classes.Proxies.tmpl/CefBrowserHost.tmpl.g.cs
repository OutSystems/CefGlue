namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent the browser process aspects of a browser. The
    /// methods of this class can only be called in the browser process. They may be
    /// called on any thread in that process unless otherwise indicated in the
    /// comments.
    /// </summary>
    public sealed unsafe partial class CefBrowserHost
    {
        /// <summary>
        /// Create a new browser using the window parameters specified by
        /// |windowInfo|. All values will be copied internally and the actual window
        /// (if any) will be created on the UI thread. If |request_context| is empty
        /// the global request context will be used. This method can be called on any
        /// browser process thread and will not block. The optional |extra_info|
        /// parameter provides an opportunity to specify extra information specific to
        /// the created browser that will be passed to
        /// CefRenderProcessHandler::OnBrowserCreated() in the render process.
        /// </summary>
        public static int CreateBrowser(cef_window_info_t* windowInfo, cef_client_t* client, cef_string_t* url, cef_browser_settings_t* settings, cef_dictionary_value_t* extra_info, cef_request_context_t* request_context)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CreateBrowser
        }
        
        /// <summary>
        /// Create a new browser using the window parameters specified by
        /// |windowInfo|. If |request_context| is empty the global request context
        /// will be used. This method can only be called on the browser process UI
        /// thread. The optional |extra_info| parameter provides an opportunity to
        /// specify extra information specific to the created browser that will be
        /// passed to CefRenderProcessHandler::OnBrowserCreated() in the render
        /// process.
        /// </summary>
        public static cef_browser_t* CreateBrowserSync(cef_window_info_t* windowInfo, cef_client_t* client, cef_string_t* url, cef_browser_settings_t* settings, cef_dictionary_value_t* extra_info, cef_request_context_t* request_context)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CreateBrowserSync
        }
        
        /// <summary>
        /// Returns the browser (if any) with the specified identifier.
        /// </summary>
        public static cef_browser_t* GetBrowserByIdentifier(int browser_id)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetBrowserByIdentifier
        }
        
        /// <summary>
        /// Returns the hosted browser object.
        /// </summary>
        public cef_browser_t* GetBrowser()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetBrowser
        }
        
        /// <summary>
        /// Request that the browser close. Closing a browser is a multi-stage process
        /// that may complete either synchronously or asynchronously, and involves
        /// callbacks such as CefLifeSpanHandler::DoClose (Alloy style only),
        /// CefLifeSpanHandler::OnBeforeClose, and a top-level window close handler
        /// such as CefWindowDelegate::CanClose (or platform-specific equivalent). In
        /// some cases a close request may be delayed or canceled by the user. Using
        /// TryCloseBrowser() instead of CloseBrowser() is recommended for most use
        /// cases. See CefLifeSpanHandler::DoClose() documentation for detailed usage
        /// and examples.
        /// If |force_close| is false then JavaScript unload handlers, if any, may be
        /// fired and the close may be delayed or canceled by the user. If
        /// |force_close| is true then the user will not be prompted and the close
        /// will proceed immediately (possibly asynchronously). If browser close is
        /// delayed and not canceled the default behavior is to call the top-level
        /// window close handler once the browser is ready to be closed. This default
        /// behavior can be changed for Alloy style browsers by implementing
        /// CefLifeSpanHandler::DoClose(). IsReadyToBeClosed() can be used to detect
        /// mandatory browser close events when customizing close behavior on the
        /// browser process UI thread.
        /// </summary>
        public void CloseBrowser(int force_close)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CloseBrowser
        }
        
        /// <summary>
        /// Helper for closing a browser. This is similar in behavior to
        /// CLoseBrowser(false) but returns a boolean to reflect the immediate close
        /// status. Call this method from a top-level window close handler such as
        /// CefWindowDelegate::CanClose (or platform-specific equivalent) to request
        /// that the browser close, and return the result to indicate if the window
        /// close should proceed. Returns false if the close will be delayed
        /// (JavaScript unload handlers triggered but still pending) or true if the
        /// close will proceed immediately (possibly asynchronously). See
        /// CloseBrowser() documentation for additional usage information. This method
        /// must be called on the browser process UI thread.
        /// </summary>
        public int TryCloseBrowser()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.TryCloseBrowser
        }
        
        /// <summary>
        /// Returns true if the browser is ready to be closed, meaning that the close
        /// has already been initiated and that JavaScript unload handlers have
        /// already executed or should be ignored. This can be used from a top-level
        /// window close handler such as CefWindowDelegate::CanClose (or
        /// platform-specific equivalent) to distringuish between potentially
        /// cancelable browser close events (like the user clicking the top-level
        /// window close button before browser close has started) and mandatory
        /// browser close events (like JavaScript `window.close()` or after browser
        /// close has started in response to [Try]CloseBrowser()). Not completing the
        /// browser close for mandatory close events (when this method returns true)
        /// will leave the browser in a partially closed state that interferes with
        /// proper functioning. See CloseBrowser() documentation for additional usage
        /// information. This method must be called on the browser process UI thread.
        /// </summary>
        public int IsReadyToBeClosed()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.IsReadyToBeClosed
        }
        
        /// <summary>
        /// Set whether the browser is focused.
        /// </summary>
        public void SetFocus(int focus)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetFocus
        }
        
        /// <summary>
        /// Retrieve the window handle (if any) for this browser. If this browser is
        /// wrapped in a CefBrowserView this method should be called on the browser
        /// process UI thread and it will return the handle for the top-level native
        /// window.
        /// </summary>
        public IntPtr GetWindowHandle()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetWindowHandle
        }
        
        /// <summary>
        /// Retrieve the window handle (if any) of the browser that opened this
        /// browser. Will return NULL for non-popup browsers or if this browser is
        /// wrapped in a CefBrowserView. This method can be used in combination with
        /// custom handling of modal windows.
        /// </summary>
        public IntPtr GetOpenerWindowHandle()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetOpenerWindowHandle
        }
        
        /// <summary>
        /// Retrieve the unique identifier of the browser that opened this browser.
        /// Will return 0 for non-popup browsers.
        /// </summary>
        public int GetOpenerIdentifier()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetOpenerIdentifier
        }
        
        /// <summary>
        /// Returns true if this browser is wrapped in a CefBrowserView.
        /// </summary>
        public int HasView()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.HasView
        }
        
        /// <summary>
        /// Returns the client for this browser.
        /// </summary>
        public cef_client_t* GetClient()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetClient
        }
        
        /// <summary>
        /// Returns the request context for this browser.
        /// </summary>
        public cef_request_context_t* GetRequestContext()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetRequestContext
        }
        
        /// <summary>
        /// Returns true if this browser can execute the specified zoom command. This
        /// method can only be called on the UI thread.
        /// </summary>
        public int CanZoom(CefZoomCommand command)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CanZoom
        }
        
        /// <summary>
        /// Execute a zoom command in this browser. If called on the UI thread the
        /// change will be applied immediately. Otherwise, the change will be applied
        /// asynchronously on the UI thread.
        /// </summary>
        public void Zoom(CefZoomCommand command)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.Zoom
        }
        
        /// <summary>
        /// Get the default zoom level. This value will be 0.0 by default but can be
        /// configured. This method can only be called on the UI thread.
        /// </summary>
        public double GetDefaultZoomLevel()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetDefaultZoomLevel
        }
        
        /// <summary>
        /// Get the current zoom level. This method can only be called on the UI
        /// thread.
        /// </summary>
        public double GetZoomLevel()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetZoomLevel
        }
        
        /// <summary>
        /// Change the zoom level to the specified value. Specify 0.0 to reset the
        /// zoom level to the default. If called on the UI thread the change will be
        /// applied immediately. Otherwise, the change will be applied asynchronously
        /// on the UI thread.
        /// </summary>
        public void SetZoomLevel(double zoomLevel)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetZoomLevel
        }
        
        /// <summary>
        /// Call to run a file chooser dialog. Only a single file chooser dialog may
        /// be pending at any given time. |mode| represents the type of dialog to
        /// display. |title| to the title to be used for the dialog and may be empty
        /// to show the default title ("Open" or "Save" depending on the mode).
        /// |default_file_path| is the path with optional directory and/or file name
        /// component that will be initially selected in the dialog. |accept_filters|
        /// are used to restrict the selectable file types and may any combination of
        /// (a) valid lower-cased MIME types (e.g. "text/*" or "image/*"), (b)
        /// individual file extensions (e.g.
        /// ".txt" or ".png"), or (c) combined description and file extension
        /// delimited using "|" and ";" (e.g. "Image Types|.png;.gif;.jpg").
        /// |callback| will be executed after the dialog is dismissed or immediately
        /// if another dialog is already pending. The dialog will be initiated
        /// asynchronously on the UI thread.
        /// </summary>
        public void RunFileDialog(CefFileDialogMode mode, cef_string_t* title, cef_string_t* default_file_path, cef_string_list* accept_filters, cef_run_file_dialog_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.RunFileDialog
        }
        
        /// <summary>
        /// Download the file at |url| using CefDownloadHandler.
        /// </summary>
        public void StartDownload(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.StartDownload
        }
        
        /// <summary>
        /// Download |image_url| and execute |callback| on completion with the images
        /// received from the renderer. If |is_favicon| is true then cookies are not
        /// sent and not accepted during download. Images with density independent
        /// pixel (DIP) sizes larger than |max_image_size| are filtered out from the
        /// image results. Versions of the image at different scale factors may be
        /// downloaded up to the maximum scale factor supported by the system. If
        /// there are no image results &lt;= |max_image_size| then the smallest image is
        /// resized to |max_image_size| and is the only result. A |max_image_size| of
        /// 0 means unlimited. If |bypass_cache| is true then |image_url| is requested
        /// from the server even if it is present in the browser cache.
        /// </summary>
        public void DownloadImage(cef_string_t* image_url, int is_favicon, uint max_image_size, int bypass_cache, cef_download_image_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DownloadImage
        }
        
        /// <summary>
        /// Print the current browser contents.
        /// </summary>
        public void Print()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.Print
        }
        
        /// <summary>
        /// Print the current browser contents to the PDF file specified by |path| and
        /// execute |callback| on completion. The caller is responsible for deleting
        /// |path| when done. For PDF printing to work on Linux you must implement the
        /// CefPrintHandler::GetPdfPaperSize method.
        /// </summary>
        public void PrintToPDF(cef_string_t* path, cef_pdf_print_settings_t* settings, cef_pdf_print_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.PrintToPDF
        }
        
        /// <summary>
        /// Search for |searchText|. |forward| indicates whether to search forward or
        /// backward within the page. |matchCase| indicates whether the search should
        /// be case-sensitive. |findNext| indicates whether this is the first request
        /// or a follow-up. The search will be restarted if |searchText| or
        /// |matchCase| change. The search will be stopped if |searchText| is empty.
        /// The CefFindHandler instance, if any, returned via
        /// CefClient::GetFindHandler will be called to report find results.
        /// </summary>
        public void Find(cef_string_t* searchText, int forward, int matchCase, int findNext)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.Find
        }
        
        /// <summary>
        /// Cancel all searches that are currently going on.
        /// </summary>
        public void StopFinding(int clearSelection)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.StopFinding
        }
        
        /// <summary>
        /// Open developer tools (DevTools) in its own browser. The DevTools browser
        /// will remain associated with this browser. If the DevTools browser is
        /// already open then it will be focused, in which case the |windowInfo|,
        /// |client| and |settings| parameters will be ignored. If
        /// |inspect_element_at| is non-empty then the element at the specified (x,y)
        /// location will be inspected. The |windowInfo| parameter will be ignored if
        /// this browser is wrapped in a CefBrowserView.
        /// </summary>
        public void ShowDevTools(cef_window_info_t* windowInfo, cef_client_t* client, cef_browser_settings_t* settings, cef_point_t* inspect_element_at)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ShowDevTools
        }
        
        /// <summary>
        /// Explicitly close the associated DevTools browser, if any.
        /// </summary>
        public void CloseDevTools()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CloseDevTools
        }
        
        /// <summary>
        /// Returns true if this browser currently has an associated DevTools browser.
        /// Must be called on the browser process UI thread.
        /// </summary>
        public int HasDevTools()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.HasDevTools
        }
        
        /// <summary>
        /// Send a method call message over the DevTools protocol. |message| must be a
        /// UTF8-encoded JSON dictionary that contains "id" (int), "method" (string)
        /// and "params" (dictionary, optional) values. See the DevTools protocol
        /// documentation at https://chromedevtools.github.io/devtools-protocol/ for
        /// details of supported methods and the expected "params" dictionary
        /// contents. |message| will be copied if necessary. This method will return
        /// true if called on the UI thread and the message was successfully submitted
        /// for validation, otherwise false. Validation will be applied asynchronously
        /// and any messages that fail due to formatting errors or missing parameters
        /// may be discarded without notification. Prefer ExecuteDevToolsMethod if a
        /// more structured approach to message formatting is desired.
        /// Every valid method call will result in an asynchronous method result or
        /// error message that references the sent message "id". Event messages are
        /// received while notifications are enabled (for example, between method
        /// calls for "Page.enable" and "Page.disable"). All received messages will be
        /// delivered to the observer(s) registered with AddDevToolsMessageObserver.
        /// See CefDevToolsMessageObserver::OnDevToolsMessage documentation for
        /// details of received message contents.
        /// Usage of the SendDevToolsMessage, ExecuteDevToolsMethod and
        /// AddDevToolsMessageObserver methods does not require an active DevTools
        /// front-end or remote-debugging session. Other active DevTools sessions will
        /// continue to function independently. However, any modification of global
        /// browser state by one session may not be reflected in the UI of other
        /// sessions.
        /// Communication with the DevTools front-end (when displayed) can be logged
        /// for development purposes by passing the
        /// `--devtools-protocol-log-file=&lt;path&gt;` command-line flag.
        /// </summary>
        public int SendDevToolsMessage(void* message, UIntPtr message_size)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendDevToolsMessage
        }
        
        /// <summary>
        /// Execute a method call over the DevTools protocol. This is a more
        /// structured version of SendDevToolsMessage. |message_id| is an incremental
        /// number that uniquely identifies the message (pass 0 to have the next
        /// number assigned automatically based on previous values). |method| is the
        /// method name. |params| are the method parameters, which may be empty. See
        /// the DevTools protocol documentation (linked above) for details of
        /// supported methods and the expected |params| dictionary contents. This
        /// method will return the assigned message ID if called on the UI thread and
        /// the message was successfully submitted for validation, otherwise 0. See
        /// the SendDevToolsMessage documentation for additional usage information.
        /// </summary>
        public int ExecuteDevToolsMethod(int message_id, cef_string_t* method, cef_dictionary_value_t* @params)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ExecuteDevToolsMethod
        }
        
        /// <summary>
        /// Add an observer for DevTools protocol messages (method results and
        /// events). The observer will remain registered until the returned
        /// Registration object is destroyed. See the SendDevToolsMessage
        /// documentation for additional usage information.
        /// </summary>
        public cef_registration_t* AddDevToolsMessageObserver(cef_dev_tools_message_observer_t* observer)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.AddDevToolsMessageObserver
        }
        
        /// <summary>
        /// Retrieve a snapshot of current navigation entries as values sent to the
        /// specified visitor. If |current_only| is true only the current navigation
        /// entry will be sent, otherwise all navigation entries will be sent.
        /// </summary>
        public void GetNavigationEntries(cef_navigation_entry_visitor_t* visitor, int current_only)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetNavigationEntries
        }
        
        /// <summary>
        /// If a misspelled word is currently selected in an editable node calling
        /// this method will replace it with the specified |word|.
        /// </summary>
        public void ReplaceMisspelling(cef_string_t* word)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ReplaceMisspelling
        }
        
        /// <summary>
        /// Add the specified |word| to the spelling dictionary.
        /// </summary>
        public void AddWordToDictionary(cef_string_t* word)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.AddWordToDictionary
        }
        
        /// <summary>
        /// Returns true if window rendering is disabled.
        /// </summary>
        public int IsWindowRenderingDisabled()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.IsWindowRenderingDisabled
        }
        
        /// <summary>
        /// Notify the browser that the widget has been resized. The browser will
        /// first call CefRenderHandler::GetViewRect to get the new size and then call
        /// CefRenderHandler::OnPaint asynchronously with the updated regions. This
        /// method is only used when window rendering is disabled.
        /// </summary>
        public void WasResized()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.WasResized
        }
        
        /// <summary>
        /// Notify the browser that it has been hidden or shown. Layouting and
        /// CefRenderHandler::OnPaint notification will stop when the browser is
        /// hidden. This method is only used when window rendering is disabled.
        /// </summary>
        public void WasHidden(int hidden)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.WasHidden
        }
        
        /// <summary>
        /// Send a notification to the browser that the screen info has changed. The
        /// browser will then call CefRenderHandler::GetScreenInfo to update the
        /// screen information with the new values. This simulates moving the webview
        /// window from one display to another, or changing the properties of the
        /// current display. This method is only used when window rendering is
        /// disabled.
        /// </summary>
        public void NotifyScreenInfoChanged()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.NotifyScreenInfoChanged
        }
        
        /// <summary>
        /// Invalidate the view. The browser will call CefRenderHandler::OnPaint
        /// asynchronously. This method is only used when window rendering is
        /// disabled.
        /// </summary>
        public void Invalidate(CefPaintElementType type)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.Invalidate
        }
        
        /// <summary>
        /// Issue a BeginFrame request to Chromium.  Only valid when
        /// CefWindowInfo::external_begin_frame_enabled is set to true.
        /// </summary>
        public void SendExternalBeginFrame()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendExternalBeginFrame
        }
        
        /// <summary>
        /// Send a key event to the browser.
        /// </summary>
        public void SendKeyEvent(cef_key_event_t* @event)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendKeyEvent
        }
        
        /// <summary>
        /// Send a mouse click event to the browser. The |x| and |y| coordinates are
        /// relative to the upper-left corner of the view.
        /// </summary>
        public void SendMouseClickEvent(cef_mouse_event_t* @event, CefMouseButtonType type, int mouseUp, int clickCount)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendMouseClickEvent
        }
        
        /// <summary>
        /// Send a mouse move event to the browser. The |x| and |y| coordinates are
        /// relative to the upper-left corner of the view.
        /// </summary>
        public void SendMouseMoveEvent(cef_mouse_event_t* @event, int mouseLeave)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendMouseMoveEvent
        }
        
        /// <summary>
        /// Send a mouse wheel event to the browser. The |x| and |y| coordinates are
        /// relative to the upper-left corner of the view. The |deltaX| and |deltaY|
        /// values represent the movement delta in the X and Y directions
        /// respectively. In order to scroll inside select popups with window
        /// rendering disabled CefRenderHandler::GetScreenPoint should be implemented
        /// properly.
        /// </summary>
        public void SendMouseWheelEvent(cef_mouse_event_t* @event, int deltaX, int deltaY)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendMouseWheelEvent
        }
        
        /// <summary>
        /// Send a touch event to the browser for a windowless browser.
        /// </summary>
        public void SendTouchEvent(cef_touch_event_t* @event)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendTouchEvent
        }
        
        /// <summary>
        /// Send a capture lost event to the browser.
        /// </summary>
        public void SendCaptureLostEvent()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SendCaptureLostEvent
        }
        
        /// <summary>
        /// Notify the browser that the window hosting it is about to be moved or
        /// resized. This method is only used on Windows and Linux.
        /// </summary>
        public void NotifyMoveOrResizeStarted()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.NotifyMoveOrResizeStarted
        }
        
        /// <summary>
        /// Returns the maximum rate in frames per second (fps) that
        /// CefRenderHandler::OnPaint will be called for a windowless browser. The
        /// actual fps may be lower if the browser cannot generate frames at the
        /// requested rate. The minimum value is 1 and the maximum value is 60
        /// (default 30). This method can only be called on the UI thread.
        /// </summary>
        public int GetWindowlessFrameRate()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetWindowlessFrameRate
        }
        
        /// <summary>
        /// Set the maximum rate in frames per second (fps) that CefRenderHandler::
        /// OnPaint will be called for a windowless browser. The actual fps may be
        /// lower if the browser cannot generate frames at the requested rate. The
        /// minimum value is 1 and the maximum value is 60 (default 30). Can also be
        /// set at browser creation via CefBrowserSettings.windowless_frame_rate.
        /// </summary>
        public void SetWindowlessFrameRate(int frame_rate)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetWindowlessFrameRate
        }
        
        /// <summary>
        /// Begins a new composition or updates the existing composition. Blink has a
        /// special node (a composition node) that allows the input method to change
        /// text without affecting other DOM nodes. |text| is the optional text that
        /// will be inserted into the composition node. |underlines| is an optional
        /// set of ranges that will be underlined in the resulting text.
        /// |replacement_range| is an optional range of the existing text that will be
        /// replaced. |selection_range| is an optional range of the resulting text
        /// that will be selected after insertion or replacement. The
        /// |replacement_range| value is only used on OS X.
        /// This method may be called multiple times as the composition changes. When
        /// the client is done making changes the composition should either be
        /// canceled or completed. To cancel the composition call
        /// ImeCancelComposition. To complete the composition call either
        /// ImeCommitText or ImeFinishComposingText. Completion is usually signaled
        /// when:
        /// 1. The client receives a WM_IME_COMPOSITION message with a GCS_RESULTSTR
        /// flag (on Windows), or;
        /// 2. The client receives a "commit" signal of GtkIMContext (on Linux), or;
        /// 3. insertText of NSTextInput is called (on Mac).
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void ImeSetComposition(cef_string_t* text, UIntPtr underlinesCount, cef_composition_underline_t* underlines, cef_range_t* replacement_range, cef_range_t* selection_range)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ImeSetComposition
        }
        
        /// <summary>
        /// Completes the existing composition by optionally inserting the specified
        /// |text| into the composition node. |replacement_range| is an optional range
        /// of the existing text that will be replaced. |relative_cursor_pos| is where
        /// the cursor will be positioned relative to the current cursor position. See
        /// comments on ImeSetComposition for usage. The |replacement_range| and
        /// |relative_cursor_pos| values are only used on OS X.
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void ImeCommitText(cef_string_t* text, cef_range_t* replacement_range, int relative_cursor_pos)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ImeCommitText
        }
        
        /// <summary>
        /// Completes the existing composition by applying the current composition
        /// node contents. If |keep_selection| is false the current selection, if any,
        /// will be discarded. See comments on ImeSetComposition for usage. This
        /// method is only used when window rendering is disabled.
        /// </summary>
        public void ImeFinishComposingText(int keep_selection)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ImeFinishComposingText
        }
        
        /// <summary>
        /// Cancels the existing composition and discards the composition node
        /// contents without applying them. See comments on ImeSetComposition for
        /// usage.
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void ImeCancelComposition()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ImeCancelComposition
        }
        
        /// <summary>
        /// Call this method when the user drags the mouse into the web view (before
        /// calling DragTargetDragOver/DragTargetLeave/DragTargetDrop).
        /// |drag_data| should not contain file contents as this type of data is not
        /// allowed to be dragged into the web view. File contents can be removed
        /// using CefDragData::ResetFileContents (for example, if |drag_data| comes
        /// from CefRenderHandler::StartDragging). This method is only used when
        /// window rendering is disabled.
        /// </summary>
        public void DragTargetDragEnter(cef_drag_data_t* drag_data, cef_mouse_event_t* @event, CefDragOperationsMask allowed_ops)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragTargetDragEnter
        }
        
        /// <summary>
        /// Call this method each time the mouse is moved across the web view during
        /// a drag operation (after calling DragTargetDragEnter and before calling
        /// DragTargetDragLeave/DragTargetDrop).
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void DragTargetDragOver(cef_mouse_event_t* @event, CefDragOperationsMask allowed_ops)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragTargetDragOver
        }
        
        /// <summary>
        /// Call this method when the user drags the mouse out of the web view (after
        /// calling DragTargetDragEnter).
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void DragTargetDragLeave()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragTargetDragLeave
        }
        
        /// <summary>
        /// Call this method when the user completes the drag operation by dropping
        /// the object onto the web view (after calling DragTargetDragEnter).
        /// The object being dropped is |drag_data|, given as an argument to
        /// the previous DragTargetDragEnter call.
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void DragTargetDrop(cef_mouse_event_t* @event)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragTargetDrop
        }
        
        /// <summary>
        /// Call this method when the drag operation started by a
        /// CefRenderHandler::StartDragging call has ended either in a drop or
        /// by being cancelled. |x| and |y| are mouse coordinates relative to the
        /// upper-left corner of the view. If the web view is both the drag source
        /// and the drag target then all DragTarget* methods should be called before
        /// DragSource* mthods.
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void DragSourceEndedAt(int x, int y, CefDragOperationsMask op)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragSourceEndedAt
        }
        
        /// <summary>
        /// Call this method when the drag operation started by a
        /// CefRenderHandler::StartDragging call has completed. This method may be
        /// called immediately without first calling DragSourceEndedAt to cancel a
        /// drag operation. If the web view is both the drag source and the drag
        /// target then all DragTarget* methods should be called before DragSource*
        /// mthods.
        /// This method is only used when window rendering is disabled.
        /// </summary>
        public void DragSourceSystemDragEnded()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.DragSourceSystemDragEnded
        }
        
        /// <summary>
        /// Returns the current visible navigation entry for this browser. This method
        /// can only be called on the UI thread.
        /// </summary>
        public cef_navigation_entry_t* GetVisibleNavigationEntry()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetVisibleNavigationEntry
        }
        
        /// <summary>
        /// Set accessibility state for all frames. |accessibility_state| may be
        /// default, enabled or disabled. If |accessibility_state| is STATE_DEFAULT
        /// then accessibility will be disabled by default and the state may be
        /// further controlled with the "force-renderer-accessibility" and
        /// "disable-renderer-accessibility" command-line switches. If
        /// |accessibility_state| is STATE_ENABLED then accessibility will be enabled.
        /// If |accessibility_state| is STATE_DISABLED then accessibility will be
        /// completely disabled.
        /// For windowed browsers accessibility will be enabled in Complete mode
        /// (which corresponds to kAccessibilityModeComplete in Chromium). In this
        /// mode all platform accessibility objects will be created and managed by
        /// Chromium's internal implementation. The client needs only to detect the
        /// screen reader and call this method appropriately. For example, on macOS
        /// the client can handle the @"AXEnhancedUserInterface" accessibility
        /// attribute to detect VoiceOver state changes and on Windows the client can
        /// handle WM_GETOBJECT with OBJID_CLIENT to detect accessibility readers.
        /// For windowless browsers accessibility will be enabled in TreeOnly mode
        /// (which corresponds to kAccessibilityModeWebContentsOnly in Chromium). In
        /// this mode renderer accessibility is enabled, the full tree is computed,
        /// and events are passed to CefAccessibiltyHandler, but platform
        /// accessibility objects are not created. The client may implement platform
        /// accessibility objects using CefAccessibiltyHandler callbacks if desired.
        /// </summary>
        public void SetAccessibilityState(CefState accessibility_state)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetAccessibilityState
        }
        
        /// <summary>
        /// Enable notifications of auto resize via CefDisplayHandler::OnAutoResize.
        /// Notifications are disabled by default. |min_size| and |max_size| define
        /// the range of allowed sizes.
        /// </summary>
        public void SetAutoResizeEnabled(int enabled, cef_size_t* min_size, cef_size_t* max_size)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetAutoResizeEnabled
        }
        
        /// <summary>
        /// Set whether the browser's audio is muted.
        /// </summary>
        public void SetAudioMuted(int mute)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.SetAudioMuted
        }
        
        /// <summary>
        /// Returns true if the browser's audio is muted.  This method can only be
        /// called on the UI thread.
        /// </summary>
        public int IsAudioMuted()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.IsAudioMuted
        }
        
        /// <summary>
        /// Returns true if the renderer is currently in browser fullscreen. This
        /// differs from window fullscreen in that browser fullscreen is entered using
        /// the JavaScript Fullscreen API and modifies CSS attributes such as the
        /// ::backdrop pseudo-element and :fullscreen pseudo-class. This method can
        /// only be called on the UI thread.
        /// </summary>
        public int IsFullscreen()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.IsFullscreen
        }
        
        /// <summary>
        /// Requests the renderer to exit browser fullscreen. In most cases exiting
        /// window fullscreen should also exit browser fullscreen. With Alloy
        /// style this method should be called in response to a user action such as
        /// clicking the green traffic light button on MacOS
        /// (CefWindowDelegate::OnWindowFullscreenTransition callback) or pressing the
        /// "ESC" key (CefKeyboardHandler::OnPreKeyEvent callback). With Chrome
        /// style these standard exit actions are handled internally but
        /// new/additional user actions can use this method. Set |will_cause_resize|
        /// to true if exiting browser fullscreen will cause a view resize.
        /// </summary>
        public void ExitFullscreen(int will_cause_resize)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ExitFullscreen
        }
        
        /// <summary>
        /// Returns true if a Chrome command is supported and enabled. Use the
        /// cef_id_for_command_id_name() function for version-safe mapping of command
        /// IDC names from cef_command_ids.h to version-specific numerical
        /// |command_id| values. This method can only be called on the UI thread. Only
        /// used with Chrome style.
        /// </summary>
        public int CanExecuteChromeCommand(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.CanExecuteChromeCommand
        }
        
        /// <summary>
        /// Execute a Chrome command. Use the cef_id_for_command_id_name()
        /// function for version-safe mapping of command IDC names from
        /// cef_command_ids.h to version-specific numerical |command_id| values.
        /// |disposition| provides information about the intended command target. Only
        /// used with Chrome style.
        /// </summary>
        public void ExecuteChromeCommand(int command_id, CefWindowOpenDisposition disposition)
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.ExecuteChromeCommand
        }
        
        /// <summary>
        /// Returns true if the render process associated with this browser is
        /// currently unresponsive as indicated by a lack of input event processing
        /// for at least 15 seconds. To receive associated state change notifications
        /// and optionally handle an unresponsive render process implement
        /// CefRequestHandler::OnRenderProcessUnresponsive. This method can only be
        /// called on the UI thread.
        /// </summary>
        public int IsRenderProcessUnresponsive()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.IsRenderProcessUnresponsive
        }
        
        /// <summary>
        /// Returns the runtime style for this browser (ALLOY or CHROME). See
        /// cef_runtime_style_t documentation for details.
        /// </summary>
        public cef_runtime_style_t GetRuntimeStyle()
        {
            throw new NotImplementedException(); // TODO: CefBrowserHost.GetRuntimeStyle
        }
        
    }
}
