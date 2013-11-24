using System.Drawing;
using System.Windows.Forms;

namespace Puut.Capture
{
    internal abstract class CaptureHelper
    {
        #region Screens
        public static Rectangle GetScreenBounds()
        {
            // all screens
            return SystemInformation.VirtualScreen;
        }
        public static Rectangle GetActiveScreenBounds()
        {
            return Screen.FromPoint(CaptureHelper.GetCursorPosition()).Bounds;
        }

        private static Point GetCursorPosition()
        {
            NativeMethods.POINT point;
            if ( NativeMethods.GetCursorPos(out point) )
            {
                return (Point) point;
            }

            return Point.Empty;
        }
        #endregion
    }
}
