namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to write data to a stream. The methods of this class may be
    /// called on any thread.
    /// </summary>
    public sealed unsafe partial class CefStreamWriter
    {
        /// <summary>
        /// Create a new CefStreamWriter object for a file.
        /// </summary>
        public static cef_stream_writer_t* CreateForFile(cef_string_t* fileName)
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.CreateForFile
        }
        
        /// <summary>
        /// Create a new CefStreamWriter object for a custom handler.
        /// </summary>
        public static cef_stream_writer_t* CreateForHandler(cef_write_handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.CreateForHandler
        }
        
        /// <summary>
        /// Write raw binary data.
        /// </summary>
        public UIntPtr Write(void* ptr, UIntPtr size, UIntPtr n)
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.Write
        }
        
        /// <summary>
        /// Seek to the specified offset position. |whence| may be any one of
        /// SEEK_CUR, SEEK_END or SEEK_SET. Returns zero on success and non-zero on
        /// failure.
        /// </summary>
        public int Seek(long offset, int whence)
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.Seek
        }
        
        /// <summary>
        /// Return the current offset position.
        /// </summary>
        public long Tell()
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.Tell
        }
        
        /// <summary>
        /// Flush the stream.
        /// </summary>
        public int Flush()
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.Flush
        }
        
        /// <summary>
        /// Returns true if this writer performs work like accessing the file system
        /// which may block. Used as a hint for determining the thread to access the
        /// writer from.
        /// </summary>
        public int MayBlock()
        {
            throw new NotImplementedException(); // TODO: CefStreamWriter.MayBlock
        }
        
    }
}
