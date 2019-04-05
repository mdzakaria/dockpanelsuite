using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WeifenLuo.WinFormsUI.Util
{
    /// <summary>
    /// Provides functions to capture the entire screen,
    /// or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        #region Fields

        public const int Srccopy = 0x00CC0020; // BitBlt dwRop parameter

        #endregion

        #region Methods

        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">
        /// The handle to the window.
        /// (In windows forms, this is obtained by the Handle property)
        /// </param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = GetWindowDC(handle);
            // get the size
            GetWindowRect(handle.ToInt32(), out Rect windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            // create a device context we can copy to
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            // bitblt over
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Srccopy);
            // restore selection
            SelectObject(hdcDest, hOld);
            // clean up
            DeleteDC(hdcDest);
            ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            DeleteObject(hBitmap);
            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        /// <summary>
        /// Scale tooltip image to specified percentage.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Image ScaleImage(Image img, double percent)
        {
            double fractionalPercentage = percent / 100.0;
            var outputWidth = (int) (img.Width * fractionalPercentage);
            var outputHeight = (int) (img.Height * fractionalPercentage);
            return ScaleImage(img, outputWidth, outputHeight);
        }

        /// <summary>
        /// Scale image for specific Width and Height
        /// </summary>
        /// <param name="img"></param>
        /// <param name="outputWidth"></param>
        /// <param name="outputHeight"></param>
        /// <returns></returns>
        public static Image ScaleImage(Image img, int outputWidth, int outputHeight)
        {
            var outputImage = new Bitmap(outputWidth, outputHeight, img.PixelFormat);
            outputImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            Graphics graphics = Graphics.FromImage(outputImage);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(img, new Rectangle(0, 0, outputWidth, outputHeight),
                new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            graphics.Dispose();
            return outputImage;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(int hWnd, out Rect lpRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDc, int nWidth,
            int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        #endregion

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        #endregion
    }
}