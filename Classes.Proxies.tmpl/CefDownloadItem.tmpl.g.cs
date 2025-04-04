namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a download item.
    /// </summary>
    public sealed unsafe partial class CefDownloadItem
    {
        /// <summary>
        /// Returns true if this object is valid. Do not call any other methods if
        /// this function returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.IsValid
        }
        
        /// <summary>
        /// Returns true if the download is in progress.
        /// </summary>
        public int IsInProgress()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.IsInProgress
        }
        
        /// <summary>
        /// Returns true if the download is complete.
        /// </summary>
        public int IsComplete()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.IsComplete
        }
        
        /// <summary>
        /// Returns true if the download has been canceled.
        /// </summary>
        public int IsCanceled()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.IsCanceled
        }
        
        /// <summary>
        /// Returns true if the download has been interrupted.
        /// </summary>
        public int IsInterrupted()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.IsInterrupted
        }
        
        /// <summary>
        /// Returns the most recent interrupt reason.
        /// </summary>
        public CefDownloadInterruptReason GetInterruptReason()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetInterruptReason
        }
        
        /// <summary>
        /// Returns a simple speed estimate in bytes/s.
        /// </summary>
        public long GetCurrentSpeed()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetCurrentSpeed
        }
        
        /// <summary>
        /// Returns the rough percent complete or -1 if the receive total size is
        /// unknown.
        /// </summary>
        public int GetPercentComplete()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetPercentComplete
        }
        
        /// <summary>
        /// Returns the total number of bytes.
        /// </summary>
        public long GetTotalBytes()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetTotalBytes
        }
        
        /// <summary>
        /// Returns the number of received bytes.
        /// </summary>
        public long GetReceivedBytes()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetReceivedBytes
        }
        
        /// <summary>
        /// Returns the time that the download started.
        /// </summary>
        public CefBaseTime GetStartTime()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetStartTime
        }
        
        /// <summary>
        /// Returns the time that the download ended.
        /// </summary>
        public CefBaseTime GetEndTime()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetEndTime
        }
        
        /// <summary>
        /// Returns the full path to the downloaded or downloading file.
        /// </summary>
        public cef_string_userfree* GetFullPath()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetFullPath
        }
        
        /// <summary>
        /// Returns the unique identifier for this download.
        /// </summary>
        public uint GetId()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetId
        }
        
        /// <summary>
        /// Returns the URL.
        /// </summary>
        public cef_string_userfree* GetURL()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetURL
        }
        
        /// <summary>
        /// Returns the original URL before any redirections.
        /// </summary>
        public cef_string_userfree* GetOriginalUrl()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetOriginalUrl
        }
        
        /// <summary>
        /// Returns the suggested file name.
        /// </summary>
        public cef_string_userfree* GetSuggestedFileName()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetSuggestedFileName
        }
        
        /// <summary>
        /// Returns the content disposition.
        /// </summary>
        public cef_string_userfree* GetContentDisposition()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetContentDisposition
        }
        
        /// <summary>
        /// Returns the mime type.
        /// </summary>
        public cef_string_userfree* GetMimeType()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItem.GetMimeType
        }
        
    }
}
