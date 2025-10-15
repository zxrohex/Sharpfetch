using System;
using System.Collections.Generic;
using System.Text;

using Hardware.Info;

namespace Sharpfetch.Core.Core.Linux
{
    public static class OSExtensions
    {
        public static string GetDistroName(this OS os)
        {
            if (!OperatingSystem.IsLinux())
            {
                throw new PlatformNotSupportedException("This method is only supported on Linux.");
            }

            try
            {
                string[] osReleaseFile = System.IO.File.ReadAllLines("/etc/os-release");

                foreach (var line in osReleaseFile)
                {
                    if (line.StartsWith("NAME="))
                    {
                        // Remove the key and any surrounding quotes
                        return line.Substring("NAME=".Length).Trim('"');
                    }
                }

                return "Unknown Linux Distribution";
            }
            catch (Exception ex)
            {
                return $"Error retrieving distro name: {ex.Message}";
            }
        }
    }
}
