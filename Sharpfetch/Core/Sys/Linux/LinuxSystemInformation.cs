using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Sharpfetch.Core.Core.Linux;
using Sharpfetch.Core.Helpers;

using Spectre.Console;

namespace Sharpfetch.Core.Sys.Linux
{
    public sealed class LinuxSystemInformation : SystemInformation
    {
        public override double CPUSpeed =>
            ToGHz(Hardware.CpuList?.FirstOrDefault()?.CurrentClockSpeed ??
                  Hardware.CpuList?.FirstOrDefault()?.MaxClockSpeed);

        public string DistroName => Hardware.OperatingSystem.GetDistroName();

        protected override string FilterVolumes()
        {
            // Restrict to root and /mnt/<letter> mounts that are fixed (best effort)
            var drives = DriveInfo.GetDrives()
                .Where(d =>
                    d.IsReady &&
                    d.DriveType == DriveType.Fixed &&
                    (d.Name == "/" || Regex.IsMatch(d.Name, @"^/mnt/[a-zA-Z]/?$")));

            return drives.Any()
                ? string.Join("\n", drives.Select(d =>
                {
                    double sizeGB = d.TotalSize / (1024d * 1024d * 1024d);
                    return $"{d.Name} ({sizeGB:0.0} GB)";
                }))
                : NotAvailable;
        }

        public override void Print()
        {
            var text = MarkupFormatter.FormatAndMarkup(
                $"{UserName}@{MachineName}",
                BuildCommonDictionary());

            var osLogoAscii = new ASCIIArtGenerator().Generate(OSLogoHelper.GetOSLogo(DistroName), 4);



            AnsiConsole.Write(MarkupFormatter.CreateDefaultOverview(osLogoAscii, text));
        }
    }
}
