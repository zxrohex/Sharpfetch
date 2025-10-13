using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Sharpfetch.Core.Helpers
{
    public class WindowsInteropHelpers
    {
        // HRESULT DwmGetColorizationColor(DWORD* pcrColorization, BOOL* pfOpaqueBlend);
        [DllImport("dwmapi.dll", ExactSpelling = true)] // <— use function name, not ordinal
        private static extern int DwmGetColorizationColor(out uint pcrColorization, out int pfOpaqueBlend);
        // Alternatively (explicit marshal):
        // private static extern int DwmGetColorizationColor(out uint pcrColorization, [MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

        public static Color GetAccentColor()
        {
            int hr = DwmGetColorizationColor(out uint argb, out int opaqueBlendInt);
            if (hr < 0)  // FAILED(hr)
                Marshal.ThrowExceptionForHR(hr);

            // argb is 0xAARRGGBB
            byte a = (byte)((argb >> 24) & 0xFF);
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
