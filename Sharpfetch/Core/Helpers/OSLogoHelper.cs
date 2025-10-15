using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Hardware.Info;

using Sharpfetch.Properties;

namespace Sharpfetch.Core.Helpers
{
    public class OSLogoHelper
    {
        public static byte[] GetOSLogo(string distroName = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Build >= 22000)
                {
                    return Resources.OSLogoWinNew;
                }
                else
                {
                    return Resources.OSLogoWinOld;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (string.IsNullOrEmpty(distroName))
                {
                    return Resources.OSLogoLinux;
                }

                switch (distroName)
                {
                    case "Ubuntu":
                        return Resources.OSLogoUbuntu;
                    case "Fedora Linux":
                    case "Fedora":
                        return Resources.OSLogoFedora;
                    case "Debian GNU/Linux":
                    case "Debian":
                        return Resources.OSLogoDebian;
                    case "Manjaro Linux":
                    case "Arch Linux":
                        return Resources.OSLogoArchLinux;
                    case "Gentoo":
                        return Resources.OSLogoGentoo;
                    case "openSUSE":
                    case "openSUSE Leap":
                    case "openSUSE Tumbleweed":
                    case "openSUSE Tumbleweed Kubic":
                        return Resources.OSLogoOpenSUSE;
                    case "Linux Mint":
                        return Resources.OSLogoLinuxMint;
                    default:
                        return Resources.OSLogoLinux;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Resources.OSLogoMacOS;
            }
            else
            {
                return Resources.OSLogoLinux;
            }
        }
    }
}
