using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Puut
{
    public class HotKeyHelper
    {
        // see http://stackoverflow.com/questions/11377977/global-hotkeys-in-wpf-working-from-every-window
        public delegate void HotKeyPressedEventHandler(object sender, EventArgs e);
        public static event HotKeyPressedEventHandler HotKeyPressed;
        private static void OnHotKeyPressed()
        {
            if ( HotKeyHelper.HotKeyPressed != null )
            {
                HotKeyHelper.HotKeyPressed(null, new EventArgs());
            }
        }

        private static HwndSource _source = null;
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;

        #region Key: Add
        /// <summary>
        /// Add a hot key.
        /// </summary>
        /// <param name="window">A window object that is needed to compute the messages.</param>
        /// <param name="vk">
        /// The virtual key code of the key you want to listen to.
        /// See http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx.
        /// </param>
        /// <param name="modifierKeyID">A code representing the modifier keys' state.
        /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms646309(v=vs.85).aspx.
        /// </param>
        public static void AddGlobalKeyHook(Window window, uint vk, uint modifierKeyID)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HotKeyHelper.HwndHook);
            HotKeyHelper.RegisterHotKey(window, vk, modifierKeyID);
        }
        private static void RegisterHotKey(Window window, uint vk, uint mod)
        {
            var helper = new WindowInteropHelper(window);
            /* const uint VK_F10 = 0x79;
            const uint MOD_CTRL = 0x0002; */
            if ( !NativeMethods.RegisterHotKey(helper.Handle, HOTKEY_ID, mod, vk) )
            {
                // handle error
                throw new COMException("Native RegisterHotKey failed.");
            }
        }
        #endregion
        #region Key: Remove
        public static void RemoveGlobalKeyHook(Window window)
        {
            _source.RemoveHook(HotKeyHelper.HwndHook);
            _source = null;

            HotKeyHelper.UnregisterHotKey(window);
        }
        private static void UnregisterHotKey(Window window)
        {
            var helper = new WindowInteropHelper(window);
            NativeMethods.UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }
        #endregion

        #region Events
        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ( msg )
            {
                case WM_HOTKEY:
                    switch ( wParam.ToInt32() )
                    {
                        case HOTKEY_ID:
                            HotKeyHelper.OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}
