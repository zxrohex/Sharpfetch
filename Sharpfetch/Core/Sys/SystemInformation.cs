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

        public string MachineName
        {
            get
            {
                try
                {
                    return Environment.MachineName;
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        public string UserName
        {
            get
            {
                try
                {
                    return Environment.UserName;
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        public virtual string OS
        {
            get
            {
                try
                {
                    return RuntimeInformation.OSDescription;
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        // _hardware.OperatingSystem?.Version?.ToString()
        public string OSVersion
        {
            get
            {
                try
                {
                    return _hardware.OperatingSystem.VersionString ?? NotAvailable;
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        public string Architecture
        {
            get
            {
                try
                {
                    return RuntimeInformation.OSArchitecture.ToString();
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }

            }
        }

        public TimeSpan? Uptime
        {
            get
            {
                try
                {
                    return TimeSpan.FromMilliseconds(Environment.TickCount64);
                }
                catch (Exception ex)
                {
                    return null; // will handle it differently
                }
            }
        }

        public string? CPUDescription
        {
            get
            {
                try
                {
                    return _hardware.CpuList?.FirstOrDefault()?.Name?.Trim();
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }


            }
        }

        public int? CPUCores
        {
            get
            {
                try
                {
                    return (int?)_hardware.CpuList?.FirstOrDefault()?.NumberOfCores ?? 0;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public int? CPUThreads
        {
            get
            {
                try
                {
                    return (int?)_hardware.CpuList.FirstOrDefault()?.NumberOfLogicalProcessors ?? 0;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }

        public abstract double CPUSpeed { get; } // GHz

        public string GPUDescription
        {
            get
            {
                try
                {
                    return _hardware.VideoControllerList?.FirstOrDefault()?.Name ?? "N/A";
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }


            }
        }

        public string ResolutionDescription
        {
            get
            {
                try
                {
                    var v = _hardware.VideoControllerList?.FirstOrDefault();

                    return (v?.CurrentHorizontalResolution > 0 && v?.CurrentVerticalResolution > 0)
                        ? $"{v.CurrentHorizontalResolution}x{v.CurrentVerticalResolution}"
                        : "N/A";
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        public string DiskDescription
        {
            get
            {
               

                return FilterVolumes();
            }
        }

        protected abstract string FilterVolumes();

        public string RAMDescription
        {
            get
            {
                try
                {
                    var ms = _hardware.MemoryStatus;
                    if (ms == null) return NotAvailable;
                    double totalMB = ms.TotalPhysical / (1024d * 1024d);
                    double usedMB = (ms.TotalPhysical - ms.AvailablePhysical) / (1024d * 1024d);
                    return $"{Math.Round(usedMB):0} MB / {Math.Round(totalMB):0} MB";
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }
            }
        }

        public string Shell
        {
            get
            {
                try
                {
                    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? (Environment.GetEnvironmentVariable("ComSpec") ?? NotAvailable)
                        : (Environment.GetEnvironmentVariable("SHELL") ?? NotAvailable);
                }
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";
                }

            }
        }

        public string Terminal
        {
            get
            {
                try
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
                catch (Exception ex)
                {
                    return $"Error ({ex.GetType().ToString().EscapeMarkup()})";

                }
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
            return Math.Round(mhz.Value / 1000d, 2, MidpointRounding.ToPositiveInfinity);
        }

        protected virtual Dictionary<string, string> BuildCommonDictionary()
        {
            return new Dictionary<string, string>
            {
                { "OS", OS },
                { "Version", OSVersion },
                { "Architecture", Architecture },
                { "Uptime", Uptime != null ? $"{(int?)Uptime?.TotalHours}h {Uptime?.Minutes}m" : "N/A"},
                { "Shell", Shell },
                { "Terminal", Terminal },
                { "CPU", (CPUDescription != null ? $"{CPUDescription} @ {CPUSpeed:0.##} GHz" : "N/A") },
                { "Cores/Threads", $"{CPUCores ?? -1} Cores / {CPUThreads ?? -1} Threads" },
                { "GPU", GPUDescription ?? "N/A" },
                { "Resolution", ResolutionDescription ?? "N/A" },
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
