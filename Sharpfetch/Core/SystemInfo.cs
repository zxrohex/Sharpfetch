using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using Hardware.Info;

using Sharpfetch.CLI;
using Sharpfetch.Core.Helpers;
using Sharpfetch.Properties;

using SixLabors.ImageSharp.Processing;

using Spectre.Console;

namespace Sharpfetch.Core
{
    public abstract class SystemInformation
    {
        public string MachineName => Environment.MachineName;

        public string UserName => Environment.UserName;

        public string OS => RuntimeInformation.OSDescription;

        public string OSVersion => hardwareInformation.OperatingSystem.Version.ToString();

        public string Architecture => RuntimeInformation.OSArchitecture.ToString();

        public TimeSpan Uptime => TimeSpan.FromMilliseconds(Environment.TickCount64);

        public string? CPUDescription => hardwareInformation.CpuList?.FirstOrDefault()?.Name.TrimEnd();

        public abstract double CPUSpeed { get; }

        public int CPUCores => (int?)hardwareInformation.CpuList?.FirstOrDefault()?.NumberOfCores ?? 0;

        public string? ResolutionDescription => (hardwareInformation.VideoControllerList.FirstOrDefault() is VideoController v ? v.CurrentHorizontalResolution + "x" + v.CurrentVerticalResolution : "N/A");

        public string? GPUDescription => hardwareInformation.VideoControllerList?.FirstOrDefault()?.Name;


        public string DiskDescription
        {

            get
            {
                string a = "";

                if (hardwareInformation.DriveList != null && hardwareInformation.DriveList.Count > 0)
                {
                    foreach (var drive in hardwareInformation.DriveList)
                    {
                        foreach (var partition in drive.PartitionList)
                        {
                            

                            foreach 
                                (var volume in  partition.VolumeList)
                            {

                                if (volume.Name == "/" || volume.Name == @"C:" | (volume.Name.StartsWith("/mnt/") && volume.Name.Split("/").Length == 2))
                                {
                                    Console.WriteLine(volume.Name);


                                    a += $"{volume.Name} {Math.Round((double)volume.FreeSpace / (1024 * 1024 * 1024), 2)} GB / {Math.Round((double)volume.Size / (1024 * 1024 * 1024), 2)} GB, ";
                                }
                            }
                        }
                    }
       
                }
                else
                {
                    return "N/A";
                }

                return !string.IsNullOrEmpty(a) ? a : "N/A";
            }
        }

        public string? RAMDescription
        {
            get
            {
                if (hardwareInformation.MemoryStatus != null)
                {
                    double totalRamInGB = Math.Round((double)hardwareInformation.MemoryStatus.TotalPhysical / (1024 * 1024), 0);
                    double usedRamInGB = Math.Round((double)(hardwareInformation.MemoryStatus.TotalPhysical - hardwareInformation.MemoryStatus.AvailablePhysical) / (1024 * 1024), 0);
                    return $"{usedRamInGB} MB / {totalRamInGB} MB";
                }
                else
                {
                    return "N/A";
                }
            }
        }

        public string? Shell
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetEnvironmentVariable("ComSpec") ?? "N/A";
                }
                else
                {
                    return Environment.GetEnvironmentVariable("SHELL") ?? "N/A";
                }
            }
        }
        public string? Terminal
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetEnvironmentVariable("WT_SESSION") != null ? "Windows Terminal" : "N/A";
                }
                else
                {
                    return Environment.GetEnvironmentVariable("TERM") ?? "N/A";
                }
            }
        }


        protected HardwareInfo hardwareInformation;

        protected SystemInformation()
        {
            hardwareInformation = new HardwareInfo();

            hardwareInformation.RefreshAll();
        }

        public abstract void Print();
    }

    public class WindowsSystemInformation : SystemInformation
    {
        public override double CPUSpeed => (Math.Round((double)hardwareInformation.CpuList?.FirstOrDefault()?.MaxClockSpeed / 1024, 2, MidpointRounding.ToPositiveInfinity));

        public WindowsSystemInformation() : base()
        {

        }



        public override void Print()
        {
            string text = MarkupForrmatter.FormatAndMarkup($"{UserName}@{MachineName}",
                new System.Collections.Generic.Dictionary<string, string>
                {
                    { "OS", OS },
                    { "Version", OSVersion },
                    { "Architecture", Architecture },
                    { "Uptime", $"{(int)Uptime.TotalHours}h {Uptime.Minutes}m" },
                    { "Shell", Shell ?? "N/A" },
                    { "Terminal", Terminal ?? "N/A" },
                    { "CPU", CPUDescription + " @ " + CPUSpeed + " GHz" ?? "N/A" },
                    { "Cores", CPUCores.ToString() },
                    { "GPU", GPUDescription ?? "N/A" },
                    { "Resolution", ResolutionDescription ?? "N/A" },
                    { "RAM", RAMDescription ?? "N/A" },
                                        { "Disk", DiskDescription ?? "N/A" }
                }, WindowsInteropHelpers.GetAccentColor());

            Panel logoPanel = new Panel(new CanvasImage(Resources.WindowsLogo64px)
               .Mutate(m => m.Resize(20, 20, KnownResamplers.NearestNeighbor)))
               .NoBorder();

            Panel contentPanel = new Panel(text).NoBorder().Expand();

            var columns = new Columns(logoPanel, contentPanel);

            columns.Collapse();

            AnsiConsole.Write(columns);
        }
    }

    public class LinuxSystemInformation : SystemInformation
    {
        public override double CPUSpeed => (Math.Round((double)hardwareInformation.CpuList?.FirstOrDefault()?.CurrentClockSpeed / 1024, 2, MidpointRounding.ToPositiveInfinity));
        public LinuxSystemInformation() : base()
        {

        }
        private List<string> ExtractRealDrives(string input)
        {
            List<string> result = new List<string>();

            // Regex: match ONLY "/mnt/[a-z]" (Windows drive mounts)
            Regex drivePattern = new Regex(@"/mnt/[a-zA-Z]\b\s+[0-9\.,]+\s+GB");

            var matches = drivePattern.Matches(input);
            foreach (Match match in matches)
            {
                // Filter out things like /mnt/wsl or /mnt/wslg or longer paths
                if (Regex.IsMatch(match.Value, @"^/mnt/[a-zA-Z]\b"))
                    result.Add(match.Value);
            }

            return result;
        }
        public override void Print()
        {
            string text = MarkupForrmatter.FormatAndMarkup($"{UserName}@{MachineName}",
                new System.Collections.Generic.Dictionary<string, string>
                {
                    { "OS", OS },
                    { "Version", OSVersion },
                    { "Architecture", Architecture },
                    { "Uptime", $"{(int)Uptime.TotalHours}h {Uptime.Minutes}m" },
                    { "Shell", Shell ?? "N/A" },
                    { "Terminal", Terminal ?? "N/A" },
                    { "CPU", CPUDescription + " @ " + CPUSpeed + " GHz" ?? "N/A" },
                    { "Cores", CPUCores.ToString() },
                    { "GPU", GPUDescription ?? "N/A" },
                    { "Resolution", ResolutionDescription ?? "N/A" },
                    { "RAM", RAMDescription ?? "N/A" },
                                        { "Disk", DiskDescription ?? "N/A" }
                });
            Panel logoPanel = new Panel(new CanvasImage(Resources.LinuxLogo64px)
               .Mutate(m => m.Resize(20, 20, KnownResamplers.NearestNeighbor)))
               .NoBorder();

            Panel contentPanel = new Panel(text).NoBorder().Expand();

            var columns = new Columns(logoPanel, contentPanel);

            columns.Collapse();

            AnsiConsole.Write(columns);
        }
    }
}
