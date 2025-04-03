namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a server that supports HTTP and WebSocket requests.
    /// Server capacity is limited and is intended to handle only a small number of
    /// simultaneous connections (e.g. for communicating between applications on
    /// localhost). The methods of this class are safe to call from any thread in
    /// the brower process unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefServer
    {
        /// <summary>
        /// Create a new server that binds to |address| and |port|. |address| must be
        /// a valid IPv4 or IPv6 address (e.g. 127.0.0.1 or ::1) and |port| must be a
        /// port number outside of the reserved range (e.g. between 1025 and 65535 on
        /// most platforms). |backlog| is the maximum number of pending connections.
        /// A new thread will be created for each CreateServer call (the "dedicated
        /// server thread"). It is therefore recommended to use a different
        /// CefServerHandler instance for each CreateServer call to avoid thread
        /// safety issues in the CefServerHandler implementation. The
        /// CefServerHandler::OnServerCreated method will be called on the dedicated
        /// server thread to report success or failure. See
        /// CefServerHandler::OnServerCreated documentation for a description of
        /// server lifespan.
        /// </summary>
        public static void CreateServer(cef_string_t* address, ushort port, int backlog, cef_server_handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefServer.CreateServer
        }
        
        /// <summary>
        /// Returns the task runner for the dedicated server thread.
        /// </summary>
        public cef_task_runner_t* GetTaskRunner()
        {
            throw new NotImplementedException(); // TODO: CefServer.GetTaskRunner
        }
        
        /// <summary>
        /// Stop the server and shut down the dedicated server thread. See
        /// CefServerHandler::OnServerCreated documentation for a description of
        /// server lifespan.
        /// </summary>
        public void Shutdown()
        {
            throw new NotImplementedException(); // TODO: CefServer.Shutdown
        }
        
        /// <summary>
        /// Returns true if the server is currently running and accepting incoming
        /// connections. See CefServerHandler::OnServerCreated documentation for a
        /// description of server lifespan. This method must be called on the
        /// dedicated server thread.
        /// </summary>
        public int IsRunning()
        {
            throw new NotImplementedException(); // TODO: CefServer.IsRunning
        }
        
        /// <summary>
        /// Returns the server address including the port number.
        /// </summary>
        public cef_string_userfree* GetAddress()
        {
            throw new NotImplementedException(); // TODO: CefServer.GetAddress
        }
        
        /// <summary>
        /// Returns true if the server currently has a connection. This method must be
        /// called on the dedicated server thread.
        /// </summary>
        public int HasConnection()
        {
            throw new NotImplementedException(); // TODO: CefServer.HasConnection
        }
        
        /// <summary>
        /// Returns true if |connection_id| represents a valid connection. This method
        /// must be called on the dedicated server thread.
        /// </summary>
        public int IsValidConnection(int connection_id)
        {
            throw new NotImplementedException(); // TODO: CefServer.IsValidConnection
        }
        
        /// <summary>
        /// Send an HTTP 200 "OK" response to the connection identified by
        /// |connection_id|. |content_type| is the response content type (e.g.
        /// "text/html"), |data| is the response content, and |data_size| is the size
        /// of |data| in bytes. The contents of |data| will be copied. The connection
        /// will be closed automatically after the response is sent.
        /// </summary>
        public void SendHttp200Response(int connection_id, cef_string_t* content_type, void* data, UIntPtr data_size)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendHttp200Response
        }
        
        /// <summary>
        /// Send an HTTP 404 "Not Found" response to the connection identified by
        /// |connection_id|. The connection will be closed automatically after the
        /// response is sent.
        /// </summary>
        public void SendHttp404Response(int connection_id)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendHttp404Response
        }
        
        /// <summary>
        /// Send an HTTP 500 "Internal Server Error" response to the connection
        /// identified by |connection_id|. |error_message| is the associated error
        /// message. The connection will be closed automatically after the response is
        /// sent.
        /// </summary>
        public void SendHttp500Response(int connection_id, cef_string_t* error_message)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendHttp500Response
        }
        
        /// <summary>
        /// Send a custom HTTP response to the connection identified by
        /// |connection_id|. |response_code| is the HTTP response code sent in the
        /// status line (e.g. 200), |content_type| is the response content type sent
        /// as the "Content-Type" header (e.g. "text/html"), |content_length| is the
        /// expected content length, and |extra_headers| is the map of extra response
        /// headers. If |content_length| is &gt;= 0 then the "Content-Length" header will
        /// be sent. If |content_length| is 0 then no content is expected and the
        /// connection will be closed automatically after the response is sent. If
        /// |content_length| is &lt; 0 then no "Content-Length" header will be sent and
        /// the client will continue reading until the connection is closed. Use the
        /// SendRawData method to send the content, if applicable, and call
        /// CloseConnection after all content has been sent.
        /// </summary>
        public void SendHttpResponse(int connection_id, int response_code, cef_string_t* content_type, long content_length, cef_string_multimap* extra_headers)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendHttpResponse
        }
        
        /// <summary>
        /// Send raw data directly to the connection identified by |connection_id|.
        /// |data| is the raw data and |data_size| is the size of |data| in bytes.
        /// The contents of |data| will be copied. No validation of |data| is
        /// performed internally so the client should be careful to send the amount
        /// indicated by the "Content-Length" header, if specified. See
        /// SendHttpResponse documentation for intended usage.
        /// </summary>
        public void SendRawData(int connection_id, void* data, UIntPtr data_size)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendRawData
        }
        
        /// <summary>
        /// Close the connection identified by |connection_id|. See SendHttpResponse
        /// documentation for intended usage.
        /// </summary>
        public void CloseConnection(int connection_id)
        {
            throw new NotImplementedException(); // TODO: CefServer.CloseConnection
        }
        
        /// <summary>
        /// Send a WebSocket message to the connection identified by |connection_id|.
        /// |data| is the response content and |data_size| is the size of |data| in
        /// bytes. The contents of |data| will be copied. See
        /// CefServerHandler::OnWebSocketRequest documentation for intended usage.
        /// </summary>
        public void SendWebSocketMessage(int connection_id, void* data, UIntPtr data_size)
        {
            throw new NotImplementedException(); // TODO: CefServer.SendWebSocketMessage
        }
        
    }
}
