namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface the client can implement to provide a custom stream writer. The
    /// methods of this class may be called on any thread.
    /// </summary>
    public abstract unsafe partial class CefWriteHandler
    {
        private UIntPtr write(cef_write_handler_t* self, void* ptr, UIntPtr size, UIntPtr n)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefWriteHandler.Write
        }
        
        /// <summary>
        /// Write raw binary data.
        /// </summary>
        // protected abstract UIntPtr Write(void* ptr, UIntPtr size, UIntPtr n);
        
        private int seek(cef_write_handler_t* self, long offset, int whence)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefWriteHandler.Seek
        }
        
        /// <summary>
        /// Seek to the specified offset position. |whence| may be any one of
        /// SEEK_CUR, SEEK_END or SEEK_SET. Return zero on success and non-zero on
        /// failure.
        /// </summary>
        // protected abstract int Seek(long offset, int whence);
        
        private long tell(cef_write_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefWriteHandler.Tell
        }
        
        /// <summary>
        /// Return the current offset position.
        /// </summary>
        // protected abstract long Tell();
        
        private int flush(cef_write_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefWriteHandler.Flush
        }
        
        /// <summary>
        /// Flush the stream.
        /// </summary>
        // protected abstract int Flush();
        
        private int may_block(cef_write_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefWriteHandler.MayBlock
        }
        
        /// <summary>
        /// Return true if this handler performs work like accessing the file system
        /// which may block. Used as a hint for determining the thread to access the
        /// handler from.
        /// </summary>
        // protected abstract int MayBlock();
        
    }
}
