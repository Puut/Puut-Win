using System;
using System.Drawing;

namespace Puut.Capture
{
    public abstract class Screenshot
    {
        public static Image CaptureScreen()
        {
            Rectangle rect = CaptureHelper.GetScreenBounds();

            return Screenshot.CaptureRectangle(rect);
        }

        public static Image CaptureRectangle(Rectangle rect)
        {
            return Screenshot.CaptureRectangleNative(NativeMethods.GetDesktopWindow(), rect);
        }
        private static Image CaptureRectangleNative(IntPtr handle, Rectangle rectangle)
        {
            // see https://github.com/ShareX/ShareX/blob/master/ScreenCaptureLib/Screenshot.cs#L117
            IntPtr hdcSrc = NativeMethods.GetWindowDC(handle);
            IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, rectangle.Width, rectangle.Height);
            IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, rectangle.Width, rectangle.Height, hdcSrc, rectangle.X, rectangle.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(handle, hdcSrc);
            Image image = Image.FromHbitmap(hBitmap);
            NativeMethods.DeleteDC(hBitmap);

            return image;
        }
    }
}
