using System;
using System.Collections.Generic;
using System.Text;

using Sharpfetch.Core.Helpers;
using Sharpfetch.Sys.Windows;

using Spectre.Console;

namespace Sharpfetch.Core.Sys.Windows
{
    public sealed class WindowsSystemInformation : SystemInformation
    {
        public override string OS => Environment.OSVersion.Version.Build > 22000 ? "Microsoft Windows 11" : base.OS;

        public override double CPUSpeed =>
            ToGHz(Hardware.CpuList?.FirstOrDefault()?.MaxClockSpeed ??
                  Hardware.CpuList?.FirstOrDefault()?.CurrentClockSpeed);

        protected override string FilterVolumes()
        {
            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name == "C:\\" && d.DriveType == DriveType.Fixed && d.IsReady);

            return $"{drive.Name} {(drive.TotalSize / (1024d * 1024d * 1024d)):0.0} GB ({(drive.TotalFreeSpace / (1024d * 1024d * 1024d)):0.0} GB free)";

            /*var drives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed && d.IsReady && d.Name.Length == 3);
     
            return drives.Any()
                ? string.Join("\n", drives.Select(d =>
                {
                    double sizeGB = d.TotalSize / (1024d * 1024d * 1024d);
                    return $"{d.Name.TrimEnd('\\')} ({sizeGB:0.0} GB)";
                }))
                : NotAvailable;*/
        }

        public override void Print()
        {
            var text = MarkupFormatter.FormatAndMarkup(
                $"{UserName}@{MachineName}",
                BuildCommonDictionary(),
                WindowsInteropHelpers.GetAccentColor());

            var osLogoAscii = new ASCIIArtGenerator(" .:/+osydmNM").Generate(OSLogoHelper.GetOSLogo(), (AnsiConsole.Profile.Width + 4) / 3);



            AnsiConsole.Write(MarkupFormatter.CreateDefaultOverview(osLogoAscii, text));
        }
    }
}
