using System;
using System.Diagnostics;
using System.Reflection;

namespace Puut
{
    public abstract class Constants
    {
        #region App Constants
        public const String APP_NAME = "Puut";
        public static String APP_VERSION()
        {
            return Constants.GetAppVersion().ToString();
        }
        public static Version GetAppVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            return new Version(fvi.FileVersion);
        }
        #endregion

        #region UI Constants
        public const String TRAYICON_TEXT_FORMAT = "{0} - v{1}";

        public const String SHOW_PREFERENCES = "Show preferences";
        public const String EXIT = "Exit";

        public const int TOOLTIP_TIMEOUT = 3500;
        public const String TOOLTIP_UPLOAD_TITLE = "Screenshot uploaded";
        public const String TOOLTIP_UPLOAD_BODY = "The link was put into your clipboard.";
        public static readonly System.Windows.Forms.ToolTipIcon TOOLTIP_UPLOAD_ICON = System.Windows.Forms.ToolTipIcon.Info;
        #endregion
    }
}
