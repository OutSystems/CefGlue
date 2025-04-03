namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface the client can implement to provide a custom stream reader. The
    /// methods of this class may be called on any thread.
    /// </summary>
    public abstract unsafe partial class CefReadHandler
    {
        private UIntPtr read(cef_read_handler_t* self, void* ptr, UIntPtr size, UIntPtr n)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefReadHandler.Read
        }
        
        /// <summary>
        /// Read raw binary data.
        /// </summary>
        // protected abstract UIntPtr Read(void* ptr, UIntPtr size, UIntPtr n);
        
        private int seek(cef_read_handler_t* self, long offset, int whence)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefReadHandler.Seek
        }
        
        /// <summary>
        /// Seek to the specified offset position. |whence| may be any one of
        /// SEEK_CUR, SEEK_END or SEEK_SET. Return zero on success and non-zero on
        /// failure.
        /// </summary>
        // protected abstract int Seek(long offset, int whence);
        
        private long tell(cef_read_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefReadHandler.Tell
        }
        
        /// <summary>
        /// Return the current offset position.
        /// </summary>
        // protected abstract long Tell();
        
        private int eof(cef_read_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefReadHandler.Eof
        }
        
        /// <summary>
        /// Return non-zero if at end of file.
        /// </summary>
        // protected abstract int Eof();
        
        private int may_block(cef_read_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefReadHandler.MayBlock
        }
        
        /// <summary>
        /// Return true if this handler performs work like accessing the file system
        /// which may block. Used as a hint for determining the thread to access the
        /// handler from.
        /// </summary>
        // protected abstract int MayBlock();
        
    }
}
