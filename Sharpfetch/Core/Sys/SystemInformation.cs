using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Hardware.Info;
using Sharpfetch.CLI;
using Sharpfetch.Core.Core.Linux;
using Sharpfetch.Core.Helpers;
using Sharpfetch.Core.Sys.Linux;
using Sharpfetch.Core.Sys.Windows;
using Sharpfetch.Properties;
using SixLabors.ImageSharp.Processing;
using Spectre.Console;

namespace Sharpfetch.Core.Sys
{
    public abstract class SystemInformation : CLIObject
    {
        protected const string NotAvailable = "N/A";

        private readonly HardwareInfo _hardware;

        public string MachineName => Environment.MachineName;
        public string UserName => Environment.UserName;
        public string OS => RuntimeInformation.OSDescription;
        public string OSVersion => _hardware.OperatingSystem?.Version?.ToString() ?? NotAvailable;
        public string Architecture => RuntimeInformation.OSArchitecture.ToString();
        public TimeSpan Uptime => TimeSpan.FromMilliseconds(Environment.TickCount64);

        public string? CPUDescription => _hardware.CpuList?.FirstOrDefault()?.Name?.Trim();
        public int CPUCores => (int?)_hardware.CpuList?.FirstOrDefault()?.NumberOfCores ?? 0;
        public abstract double CPUSpeed { get; } // GHz

        public string? GPUDescription => _hardware.VideoControllerList?.FirstOrDefault()?.Name;

        public string? ResolutionDescription
        {
            get
            {
                var v = _hardware.VideoControllerList?.FirstOrDefault();
                return (v?.CurrentHorizontalResolution > 0 && v?.CurrentVerticalResolution > 0)
                    ? $"{v.CurrentHorizontalResolution}x{v.CurrentVerticalResolution}"
                    : NotAvailable;
            }
        }

        public string DiskDescription => FilterVolumes();

        protected abstract string FilterVolumes();

        public string RAMDescription
        {
            get
            {
                var ms = _hardware.MemoryStatus;
                if (ms == null) return NotAvailable;
                double totalMB = ms.TotalPhysical / (1024d * 1024d);
                double usedMB = (ms.TotalPhysical - ms.AvailablePhysical) / (1024d * 1024d);
                return $"{Math.Round(usedMB):0} MB / {Math.Round(totalMB):0} MB";
            }
        }

        public string Shell =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (Environment.GetEnvironmentVariable("ComSpec") ?? NotAvailable)
                : (Environment.GetEnvironmentVariable("SHELL") ?? NotAvailable);

        public string Terminal
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (Environment.GetEnvironmentVariable("WT_SESSION") != null) return "Windows Terminal";
                    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") is { } tp && !string.IsNullOrWhiteSpace(tp))
                        return tp;
                    return NotAvailable;
                }
                return Environment.GetEnvironmentVariable("TERM") ?? NotAvailable;
            }
        }

        protected SystemInformation()
        {
            _hardware = new HardwareInfo();
            _hardware.RefreshAll();
        }

        protected HardwareInfo Hardware => _hardware;

        protected double ToGHz(uint? mhz)
        {
            if (mhz == null || mhz == 0) return 0;
            // 1000 is closer to reported marketing frequencies than 1024 for MHz->GHz
            return Math.Round(mhz.Value / 1000d, 2, MidpointRounding.AwayFromZero);
        }

        protected Dictionary<string, string> BuildCommonDictionary()
        {
            return new Dictionary<string, string>
            {
                { "OS", OS },
                { "Version", OSVersion },
                { "Architecture", Architecture },
                { "Uptime", $"{(int)Uptime.TotalHours}h {Uptime.Minutes}m" },
                { "Shell", Shell },
                { "Terminal", Terminal },
                { "CPU", (CPUDescription != null ? $"{CPUDescription} @ {CPUSpeed:0.##} GHz" : NotAvailable) },
                { "Cores", CPUCores.ToString() },
                { "GPU", GPUDescription ?? NotAvailable },
                { "Resolution", ResolutionDescription ?? NotAvailable },
                { "RAM", RAMDescription },
                { "Disk", DiskDescription }
            };
        }

        public static SystemInformation Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsSystemInformation();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxSystemInformation();

            // Default to base Windows style if unknown
            return new WindowsSystemInformation();
        }

        public override abstract void Print();
    }
}
