namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Container for a single image represented at different scale factors. All
    /// image representations should be the same size in density independent pixel
    /// (DIP) units. For example, if the image at scale factor 1.0 is 100x100 pixels
    /// then the image at scale factor 2.0 should be 200x200 pixels -- both images
    /// will display with a DIP size of 100x100 units. The methods of this class can
    /// be called on any browser process thread.
    /// </summary>
    public sealed unsafe partial class CefImage
    {
        /// <summary>
        /// Create a new CefImage. It will initially be empty. Use the Add*() methods
        /// to add representations at different scale factors.
        /// </summary>
        public static cef_image_t* CreateImage()
        {
            throw new NotImplementedException(); // TODO: CefImage.CreateImage
        }
        
        /// <summary>
        /// Returns true if this Image is empty.
        /// </summary>
        public int IsEmpty()
        {
            throw new NotImplementedException(); // TODO: CefImage.IsEmpty
        }
        
        /// <summary>
        /// Returns true if this Image and |that| Image share the same underlying
        /// storage. Will also return true if both images are empty.
        /// </summary>
        public int IsSame(cef_image_t* that)
        {
            throw new NotImplementedException(); // TODO: CefImage.IsSame
        }
        
        /// <summary>
        /// Add a bitmap image representation for |scale_factor|. Only 32-bit
        /// RGBA/BGRA formats are supported. |pixel_width| and |pixel_height| are the
        /// bitmap representation size in pixel coordinates. |pixel_data| is the array
        /// of pixel data and should be |pixel_width| x |pixel_height| x 4 bytes in
        /// size. |color_type| and |alpha_type| values specify the pixel format.
        /// </summary>
        public int AddBitmap(float scale_factor, int pixel_width, int pixel_height, CefColorType color_type, CefAlphaType alpha_type, void* pixel_data, UIntPtr pixel_data_size)
        {
            throw new NotImplementedException(); // TODO: CefImage.AddBitmap
        }
        
        /// <summary>
        /// Add a PNG image representation for |scale_factor|. |png_data| is the image
        /// data of size |png_data_size|. Any alpha transparency in the PNG data will
        /// be maintained.
        /// </summary>
        public int AddPNG(float scale_factor, void* png_data, UIntPtr png_data_size)
        {
            throw new NotImplementedException(); // TODO: CefImage.AddPNG
        }
        
        /// <summary>
        /// Create a JPEG image representation for |scale_factor|. |jpeg_data| is the
        /// image data of size |jpeg_data_size|. The JPEG format does not support
        /// transparency so the alpha byte will be set to 0xFF for all pixels.
        /// </summary>
        public int AddJPEG(float scale_factor, void* jpeg_data, UIntPtr jpeg_data_size)
        {
            throw new NotImplementedException(); // TODO: CefImage.AddJPEG
        }
        
        /// <summary>
        /// Returns the image width in density independent pixel (DIP) units.
        /// </summary>
        public UIntPtr GetWidth()
        {
            throw new NotImplementedException(); // TODO: CefImage.GetWidth
        }
        
        /// <summary>
        /// Returns the image height in density independent pixel (DIP) units.
        /// </summary>
        public UIntPtr GetHeight()
        {
            throw new NotImplementedException(); // TODO: CefImage.GetHeight
        }
        
        /// <summary>
        /// Returns true if this image contains a representation for |scale_factor|.
        /// </summary>
        public int HasRepresentation(float scale_factor)
        {
            throw new NotImplementedException(); // TODO: CefImage.HasRepresentation
        }
        
        /// <summary>
        /// Removes the representation for |scale_factor|. Returns true on success.
        /// </summary>
        public int RemoveRepresentation(float scale_factor)
        {
            throw new NotImplementedException(); // TODO: CefImage.RemoveRepresentation
        }
        
        /// <summary>
        /// Returns information for the representation that most closely matches
        /// |scale_factor|. |actual_scale_factor| is the actual scale factor for the
        /// representation. |pixel_width| and |pixel_height| are the representation
        /// size in pixel coordinates. Returns true on success.
        /// </summary>
        public int GetRepresentationInfo(float scale_factor, float* actual_scale_factor, int* pixel_width, int* pixel_height)
        {
            throw new NotImplementedException(); // TODO: CefImage.GetRepresentationInfo
        }
        
        /// <summary>
        /// Returns the bitmap representation that most closely matches
        /// |scale_factor|. Only 32-bit RGBA/BGRA formats are supported. |color_type|
        /// and |alpha_type| values specify the desired output pixel format.
        /// |pixel_width| and |pixel_height| are the output representation size in
        /// pixel coordinates. Returns a CefBinaryValue containing the pixel data on
        /// success or NULL on failure.
        /// </summary>
        public cef_binary_value_t* GetAsBitmap(float scale_factor, CefColorType color_type, CefAlphaType alpha_type, int* pixel_width, int* pixel_height)
        {
            throw new NotImplementedException(); // TODO: CefImage.GetAsBitmap
        }
        
        /// <summary>
        /// Returns the PNG representation that most closely matches |scale_factor|.
        /// If |with_transparency| is true any alpha transparency in the image will be
        /// represented in the resulting PNG data. |pixel_width| and |pixel_height|
        /// are the output representation size in pixel coordinates. Returns a
        /// CefBinaryValue containing the PNG image data on success or NULL on
        /// failure.
        /// </summary>
        public cef_binary_value_t* GetAsPNG(float scale_factor, int with_transparency, int* pixel_width, int* pixel_height)
        {
            throw new NotImplementedException(); // TODO: CefImage.GetAsPNG
        }
        
        /// <summary>
        /// Returns the JPEG representation that most closely matches |scale_factor|.
        /// |quality| determines the compression level with 0 == lowest and 100 ==
        /// highest. The JPEG format does not support alpha transparency and the alpha
        /// channel, if any, will be discarded. |pixel_width| and |pixel_height| are
        /// the output representation size in pixel coordinates. Returns a
        /// CefBinaryValue containing the JPEG image data on success or NULL on
        /// failure.
        /// </summary>
        public cef_binary_value_t* GetAsJPEG(float scale_factor, int quality, int* pixel_width, int* pixel_height)
        {
            throw new NotImplementedException(); // TODO: CefImage.GetAsJPEG
        }
        
    }
}
