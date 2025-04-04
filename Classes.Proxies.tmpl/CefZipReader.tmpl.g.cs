namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that supports the reading of zip archives via the zlib unzip API.
    /// The methods of this class should only be called on the thread that creates
    /// the object.
    /// </summary>
    public sealed unsafe partial class CefZipReader
    {
        /// <summary>
        /// Create a new CefZipReader object. The returned object's methods can only
        /// be called from the thread that created the object.
        /// </summary>
        public static cef_zip_reader_t* Create(cef_stream_reader_t* stream)
        {
            throw new NotImplementedException(); // TODO: CefZipReader.Create
        }
        
        /// <summary>
        /// Moves the cursor to the first file in the archive. Returns true if the
        /// cursor position was set successfully.
        /// </summary>
        public int MoveToFirstFile()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.MoveToFirstFile
        }
        
        /// <summary>
        /// Moves the cursor to the next file in the archive. Returns true if the
        /// cursor position was set successfully.
        /// </summary>
        public int MoveToNextFile()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.MoveToNextFile
        }
        
        /// <summary>
        /// Moves the cursor to the specified file in the archive. If |caseSensitive|
        /// is true then the search will be case sensitive. Returns true if the cursor
        /// position was set successfully.
        /// </summary>
        public int MoveToFile(cef_string_t* fileName, int caseSensitive)
        {
            throw new NotImplementedException(); // TODO: CefZipReader.MoveToFile
        }
        
        /// <summary>
        /// Closes the archive. This should be called directly to ensure that cleanup
        /// occurs on the correct thread.
        /// </summary>
        public int Close()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.Close
        }
        
        /// <summary>
        /// Returns the name of the file.
        /// </summary>
        public cef_string_userfree* GetFileName()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.GetFileName
        }
        
        /// <summary>
        /// Returns the uncompressed size of the file.
        /// </summary>
        public long GetFileSize()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.GetFileSize
        }
        
        /// <summary>
        /// Returns the last modified timestamp for the file.
        /// </summary>
        public CefBaseTime GetFileLastModified()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.GetFileLastModified
        }
        
        /// <summary>
        /// Opens the file for reading of uncompressed data. A read password may
        /// optionally be specified.
        /// </summary>
        public int OpenFile(cef_string_t* password)
        {
            throw new NotImplementedException(); // TODO: CefZipReader.OpenFile
        }
        
        /// <summary>
        /// Closes the file.
        /// </summary>
        public int CloseFile()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.CloseFile
        }
        
        /// <summary>
        /// Read uncompressed file contents into the specified buffer. Returns &lt; 0 if
        /// an error occurred, 0 if at the end of file, or the number of bytes read.
        /// </summary>
        public int ReadFile(void* buffer, UIntPtr bufferSize)
        {
            throw new NotImplementedException(); // TODO: CefZipReader.ReadFile
        }
        
        /// <summary>
        /// Returns the current offset in the uncompressed file contents.
        /// </summary>
        public long Tell()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.Tell
        }
        
        /// <summary>
        /// Returns true if at end of the file contents.
        /// </summary>
        public int Eof()
        {
            throw new NotImplementedException(); // TODO: CefZipReader.Eof
        }
        
    }
}
