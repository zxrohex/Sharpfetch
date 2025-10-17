using System;
using System.Collections.Generic;
using System.Text;

using Hardware.Info;

namespace Sharpfetch.Core.Core.Linux
{
    /*
     * Not sure if this is needed and good coding practice
     * So this will be unused but kept for now
     */

    public class LinuxOSInformation
    {
        public static string GetDistroName()
        {
            if (!OperatingSystem.IsLinux())
            {
                throw new PlatformNotSupportedException("This method is only supported on Linux.");
            }

            try
            {
                string[] osReleaseFile = File.ReadAllLines("/etc/os-release");

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

        public static string GetKernelRelease()
        {
            if (!OperatingSystem.IsLinux())
            {
                throw new PlatformNotSupportedException("This method is only supported on Linux.");

            }

            try
            {
                string kernelRelease = File.ReadAllText("/proc/sys/kernel/osrelease").Trim();

                return kernelRelease;
            }
            catch (Exception ex)
            {
                return $"Error retrieving kernel release: {ex.Message}";
            }
        }
    }
}
