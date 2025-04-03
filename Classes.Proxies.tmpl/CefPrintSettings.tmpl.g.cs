namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing print settings.
    /// </summary>
    public sealed unsafe partial class CefPrintSettings
    {
        /// <summary>
        /// Create a new CefPrintSettings object.
        /// </summary>
        public static cef_print_settings_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.Create
        }
        
        /// <summary>
        /// Returns true if this object is valid. Do not call any other methods if
        /// this function returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.IsValid
        }
        
        /// <summary>
        /// Returns true if the values of this object are read-only. Some APIs may
        /// expose read-only objects.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.IsReadOnly
        }
        
        /// <summary>
        /// Set the page orientation.
        /// </summary>
        public void SetOrientation(int landscape)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetOrientation
        }
        
        /// <summary>
        /// Returns true if the orientation is landscape.
        /// </summary>
        public int IsLandscape()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.IsLandscape
        }
        
        /// <summary>
        /// Set the printer printable area in device units.
        /// Some platforms already provide flipped area. Set |landscape_needs_flip|
        /// to false on those platforms to avoid double flipping.
        /// </summary>
        public void SetPrinterPrintableArea(cef_size_t* physical_size_device_units, cef_rect_t* printable_area_device_units, int landscape_needs_flip)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetPrinterPrintableArea
        }
        
        /// <summary>
        /// Set the device name.
        /// </summary>
        public void SetDeviceName(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetDeviceName
        }
        
        /// <summary>
        /// Get the device name.
        /// </summary>
        public cef_string_userfree* GetDeviceName()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetDeviceName
        }
        
        /// <summary>
        /// Set the DPI (dots per inch).
        /// </summary>
        public void SetDPI(int dpi)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetDPI
        }
        
        /// <summary>
        /// Get the DPI (dots per inch).
        /// </summary>
        public int GetDPI()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetDPI
        }
        
        /// <summary>
        /// Set the page ranges.
        /// </summary>
        public void SetPageRanges(UIntPtr rangesCount, cef_range_t* ranges)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetPageRanges
        }
        
        /// <summary>
        /// Returns the number of page ranges that currently exist.
        /// </summary>
        public UIntPtr GetPageRangesCount()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetPageRangesCount
        }
        
        /// <summary>
        /// Retrieve the page ranges.
        /// </summary>
        public void GetPageRanges(UIntPtr* rangesCount, cef_range_t* ranges)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetPageRanges
        }
        
        /// <summary>
        /// Set whether only the selection will be printed.
        /// </summary>
        public void SetSelectionOnly(int selection_only)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetSelectionOnly
        }
        
        /// <summary>
        /// Returns true if only the selection will be printed.
        /// </summary>
        public int IsSelectionOnly()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.IsSelectionOnly
        }
        
        /// <summary>
        /// Set whether pages will be collated.
        /// </summary>
        public void SetCollate(int collate)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetCollate
        }
        
        /// <summary>
        /// Returns true if pages will be collated.
        /// </summary>
        public int WillCollate()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.WillCollate
        }
        
        /// <summary>
        /// Set the color model.
        /// </summary>
        public void SetColorModel(CefColorModel model)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetColorModel
        }
        
        /// <summary>
        /// Get the color model.
        /// </summary>
        public CefColorModel GetColorModel()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetColorModel
        }
        
        /// <summary>
        /// Set the number of copies.
        /// </summary>
        public void SetCopies(int copies)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetCopies
        }
        
        /// <summary>
        /// Get the number of copies.
        /// </summary>
        public int GetCopies()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetCopies
        }
        
        /// <summary>
        /// Set the duplex mode.
        /// </summary>
        public void SetDuplexMode(CefDuplexMode mode)
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.SetDuplexMode
        }
        
        /// <summary>
        /// Get the duplex mode.
        /// </summary>
        public CefDuplexMode GetDuplexMode()
        {
            throw new NotImplementedException(); // TODO: CefPrintSettings.GetDuplexMode
        }
        
    }
}
