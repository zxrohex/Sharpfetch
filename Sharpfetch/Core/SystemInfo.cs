using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Hardware.Info;

using Sharpfetch.CLI;

using Spectre.Console;

namespace Sharpfetch.Core
{
    public class SystemInfo : CLIObject
    {
        public string UserName { get; private set; }

        public string ComputerName { get; private set; }

        public OSPlatform OSPlatform => OperatingSystem.IsWindows() ? OSPlatform.Windows : OperatingSystem.IsLinux() ? OSPlatform.Linux : OperatingSystem.IsMacOS() ? OSPlatform.OSX : OSPlatform.FreeBSD;

        public string OSName { get; private set; }

        public string OSVersion { get; private set; }

        public string OSArchitecture { get; private set; }

        public string OSDescription { get; private set; }

        public TimeSpan OSUptime { get; private set; }

        public string Resolution { get; private set; }

        public string Terminal { get; private set; }

        public string CPUDescription { get; private set; }

        public string CPUVendor { get; private set; }

        public string CPUModel { get; private set; }

        public int CPUCores { get; private set; }

        public int CPUSpeed { get; private set; }

        public string GPUDescription { get; private set; }

        public int MemoryTotal { get; private set; }

        public int MemoryUsed { get; private set; }

        public int MemoryFree { get; private set; }

        public SystemInfo()
        {
            HardwareInfo hwInfo = new HardwareInfo();

            hwInfo.RefreshAll();

            UserName = Environment.UserName;

            ComputerName = Environment.MachineName;

            OSName = hwInfo.OperatingSystem.Name ?? "Unknown";

            OSVersion = hwInfo.OperatingSystem.VersionString ?? "Unknown";

            OSArchitecture = RuntimeInformation.OSArchitecture.ToString();

            OSDescription = RuntimeInformation.OSDescription;
         
            OSUptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

            Resolution = (hwInfo.VideoControllerList.FirstOrDefault() != null && hwInfo.VideoControllerList.FirstOrDefault()?.CurrentHorizontalResolution != null && hwInfo.VideoControllerList.FirstOrDefault()?.CurrentVerticalResolution != null) ? $"{hwInfo.VideoControllerList.FirstOrDefault()?.CurrentHorizontalResolution}x{hwInfo.VideoControllerList.FirstOrDefault()?.CurrentVerticalResolution}" : null;



            Terminal = Environment.GetEnvironmentVariable("TERM_PROGRAM") ?? "Unknown";

            CPUDescription = hwInfo.CpuList.FirstOrDefault()?.Description ?? "Unknown";

            CPUVendor = hwInfo.CpuList.FirstOrDefault()?.Manufacturer ?? "Unknown";

            CPUModel = hwInfo.CpuList.FirstOrDefault()?.Name ?? "Unknown";

            CPUCores = (int)(hwInfo.CpuList.FirstOrDefault()?.NumberOfCores ?? 0);

            CPUSpeed = (int)hwInfo.CpuList.FirstOrDefault()?.MaxClockSpeed == 0 ? (int)(hwInfo.CpuList.FirstOrDefault()?.CurrentClockSpeed) : (int)(hwInfo.CpuList.FirstOrDefault()?.MaxClockSpeed);

            GPUDescription = hwInfo.VideoControllerList.FirstOrDefault()?.Description ?? "Unknown";

            MemoryTotal = (int)(hwInfo.MemoryStatus.TotalPhysical / (1024 * 1024));

            MemoryFree = (int)(hwInfo.MemoryStatus.AvailablePhysical / (1024 * 1024));

            MemoryUsed = MemoryTotal - MemoryFree;
        }

        public string ToConsoleText()
        {
            return $"[bold]{UserName}@{ComputerName}[/]\n" +
                new StringBuilder().Append('-', UserName.Length + ComputerName.Length + 1) + "\n" +
                $"[bold]OS: [/] {OSDescription}\n" +
                $"[bold]Version: [/] {OSVersion}\n" +
                $"[bold]Uptime: [/] {OSUptime:d' day(s), 'h' hour(s), 'm} minute(s)\n" +
                (Resolution != null ? $"[bold]Resolution: [/] {Resolution}\n" : "") +
                $"[bold]Terminal: [/] {Terminal}\n" +
                $"[bold]CPU: [/] {CPUModel.TrimEnd()} @ {Math.Round((double)CPUSpeed / 1000, 2, MidpointRounding.ToPositiveInfinity)} GHz\n" +
                $"[bold]GPU: [/] {GPUDescription}\n" +
                $"[bold]Memory: [/] {MemoryUsed} MB / {MemoryTotal} MB ({Math.Round(((double)MemoryUsed / (double)MemoryTotal) * 100, 1)}%)\n";
           
        }

        public override void Print()
        {
            
        }
    }
}
