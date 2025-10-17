using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Sharpfetch.Core.Core.Linux;
using Sharpfetch.Core.Helpers;
using Sharpfetch.Core.Helpers.Extensions;

using Spectre.Console;

namespace Sharpfetch.Core.Sys.Linux
{
    public sealed class LinuxSystemInformation : SystemInformation
    {
        public override double CPUSpeed =>
            ToGHz(Hardware.CpuList?.FirstOrDefault()?.CurrentClockSpeed ??
                  Hardware.CpuList?.FirstOrDefault()?.MaxClockSpeed);

        public string DistroName => File.ReadAllLines("/etc/os-release")
            .FirstOrDefault(line => line.StartsWith("NAME="))?
            .Substring("NAME=".Length).Trim('"') ?? "Unknown Linux Distribution";

        public string KernelRelease => File.ReadAllText("/proc/sys/kernel/osrelease").Trim();

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

        protected override Dictionary<string, string> BuildCommonDictionary()
        {
            /*
            Dictionary<string, string> baseDict = base.BuildCommonDictionary();

            Dictionary<string, string> osDictPart = baseDict.Take(2).ToDictionary();

            osDictPart.AddRange(new Dictionary<string, string>
            {
                { "Kernel", KernelRelease }
            });

            Dictionary<string, string> remainderDictPart = baseDict.Skip(2).ToDictionary();

            return osDictPart.Concat(remainderDictPart).ToDictionary();
            */

            Dictionary<string, string> baseDict = base.BuildCommonDictionary();

            baseDict.AddRangeAt(2, new Dictionary<string, string>
            {
                { "Kernel", KernelRelease }
            });

            return baseDict;    
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
