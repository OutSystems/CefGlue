namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle HTTP server requests. A new thread will
    /// be created for each CefServer::CreateServer call (the "dedicated server
    /// thread"), and the methods of this class will be called on that thread. It is
    /// therefore recommended to use a different CefServerHandler instance for each
    /// CefServer::CreateServer call to avoid thread safety issues in the
    /// CefServerHandler implementation.
    /// </summary>
    public abstract unsafe partial class CefServerHandler
    {
        private void on_server_created(cef_server_handler_t* self, cef_server_t* server)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnServerCreated
        }
        
        /// <summary>
        /// Called when |server| is created. If the server was started successfully
        /// then CefServer::IsRunning will return true. The server will continue
        /// running until CefServer::Shutdown is called, after which time
        /// OnServerDestroyed will be called. If the server failed to start then
        /// OnServerDestroyed will be called immediately after this method returns.
        /// </summary>
        // protected abstract void OnServerCreated(cef_server_t* server);
        
        private void on_server_destroyed(cef_server_handler_t* self, cef_server_t* server)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnServerDestroyed
        }
        
        /// <summary>
        /// Called when |server| is destroyed. The server thread will be stopped after
        /// this method returns. The client should release any references to |server|
        /// when this method is called. See OnServerCreated documentation for a
        /// description of server lifespan.
        /// </summary>
        // protected abstract void OnServerDestroyed(cef_server_t* server);
        
        private void on_client_connected(cef_server_handler_t* self, cef_server_t* server, int connection_id)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnClientConnected
        }
        
        /// <summary>
        /// Called when a client connects to |server|. |connection_id| uniquely
        /// identifies the connection. Each call to this method will have a matching
        /// call to OnClientDisconnected.
        /// </summary>
        // protected abstract void OnClientConnected(cef_server_t* server, int connection_id);
        
        private void on_client_disconnected(cef_server_handler_t* self, cef_server_t* server, int connection_id)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnClientDisconnected
        }
        
        /// <summary>
        /// Called when a client disconnects from |server|. |connection_id| uniquely
        /// identifies the connection. The client should release any data associated
        /// with |connection_id| when this method is called and |connection_id| should
        /// no longer be passed to CefServer methods. Disconnects can originate from
        /// either the client or the server. For example, the server will disconnect
        /// automatically after a CefServer::SendHttpXXXResponse method is called.
        /// </summary>
        // protected abstract void OnClientDisconnected(cef_server_t* server, int connection_id);
        
        private void on_http_request(cef_server_handler_t* self, cef_server_t* server, int connection_id, cef_string_t* client_address, cef_request_t* request)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnHttpRequest
        }
        
        /// <summary>
        /// Called when |server| receives an HTTP request. |connection_id| uniquely
        /// identifies the connection, |client_address| is the requesting IPv4 or IPv6
        /// client address including port number, and |request| contains the request
        /// contents (URL, method, headers and optional POST data). Call CefServer
        /// methods either synchronously or asynchronusly to send a response.
        /// </summary>
        // protected abstract void OnHttpRequest(cef_server_t* server, int connection_id, cef_string_t* client_address, cef_request_t* request);
        
        private void on_web_socket_request(cef_server_handler_t* self, cef_server_t* server, int connection_id, cef_string_t* client_address, cef_request_t* request, cef_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnWebSocketRequest
        }
        
        /// <summary>
        /// Called when |server| receives a WebSocket request. |connection_id|
        /// uniquely identifies the connection, |client_address| is the requesting
        /// IPv4 or IPv6 client address including port number, and |request| contains
        /// the request contents (URL, method, headers and optional POST data).
        /// Execute |callback| either synchronously or asynchronously to accept or
        /// decline the WebSocket connection. If the request is accepted then
        /// OnWebSocketConnected will be called after the WebSocket has connected and
        /// incoming messages will be delivered to the OnWebSocketMessage callback. If
        /// the request is declined then the client will be disconnected and
        /// OnClientDisconnected will be called. Call the
        /// CefServer::SendWebSocketMessage method after receiving the
        /// OnWebSocketConnected callback to respond with WebSocket messages.
        /// </summary>
        // protected abstract void OnWebSocketRequest(cef_server_t* server, int connection_id, cef_string_t* client_address, cef_request_t* request, cef_callback_t* callback);
        
        private void on_web_socket_connected(cef_server_handler_t* self, cef_server_t* server, int connection_id)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnWebSocketConnected
        }
        
        /// <summary>
        /// Called after the client has accepted the WebSocket connection for |server|
        /// and |connection_id| via the OnWebSocketRequest callback. See
        /// OnWebSocketRequest documentation for intended usage.
        /// </summary>
        // protected abstract void OnWebSocketConnected(cef_server_t* server, int connection_id);
        
        private void on_web_socket_message(cef_server_handler_t* self, cef_server_t* server, int connection_id, void* data, UIntPtr data_size)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefServerHandler.OnWebSocketMessage
        }
        
        /// <summary>
        /// Called when |server| receives an WebSocket message. |connection_id|
        /// uniquely identifies the connection, |data| is the message content and
        /// |data_size| is the size of |data| in bytes. Do not keep a reference to
        /// |data| outside of this method. See OnWebSocketRequest documentation for
        /// intended usage.
        /// </summary>
        // protected abstract void OnWebSocketMessage(cef_server_t* server, int connection_id, void* data, UIntPtr data_size);
        
    }
}
