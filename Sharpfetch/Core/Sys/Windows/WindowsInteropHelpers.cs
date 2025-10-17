using System.Runtime.InteropServices;
using System.Drawing;

namespace Sharpfetch.Sys.Windows
{
    public class WindowsInteropHelpers
    {
        [DllImport("dwmapi.dll", ExactSpelling = true)]
        private static extern int DwmGetColorizationColor(out uint pcrColorization, out int pfOpaqueBlend);

        public static Color GetAccentColor()
        {
            int hr = DwmGetColorizationColor(out uint argb, out int _);
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);

            byte a = (byte)((argb >> 24) & 0xFF);
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
