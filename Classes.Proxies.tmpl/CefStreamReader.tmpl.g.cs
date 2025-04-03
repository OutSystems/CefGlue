namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to read data from a stream. The methods of this class may be
    /// called on any thread.
    /// </summary>
    public sealed unsafe partial class CefStreamReader
    {
        /// <summary>
        /// Create a new CefStreamReader object from a file.
        /// </summary>
        public static cef_stream_reader_t* CreateForFile(cef_string_t* fileName)
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.CreateForFile
        }
        
        /// <summary>
        /// Create a new CefStreamReader object from data.
        /// </summary>
        public static cef_stream_reader_t* CreateForData(void* data, UIntPtr size)
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.CreateForData
        }
        
        /// <summary>
        /// Create a new CefStreamReader object from a custom handler.
        /// </summary>
        public static cef_stream_reader_t* CreateForHandler(cef_read_handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.CreateForHandler
        }
        
        /// <summary>
        /// Read raw binary data.
        /// </summary>
        public UIntPtr Read(void* ptr, UIntPtr size, UIntPtr n)
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.Read
        }
        
        /// <summary>
        /// Seek to the specified offset position. |whence| may be any one of
        /// SEEK_CUR, SEEK_END or SEEK_SET. Returns zero on success and non-zero on
        /// failure.
        /// </summary>
        public int Seek(long offset, int whence)
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.Seek
        }
        
        /// <summary>
        /// Return the current offset position.
        /// </summary>
        public long Tell()
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.Tell
        }
        
        /// <summary>
        /// Return non-zero if at end of file.
        /// </summary>
        public int Eof()
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.Eof
        }
        
        /// <summary>
        /// Returns true if this reader performs work like accessing the file system
        /// which may block. Used as a hint for determining the thread to access the
        /// reader from.
        /// </summary>
        public int MayBlock()
        {
            throw new NotImplementedException(); // TODO: CefStreamReader.MayBlock
        }
        
    }
}
