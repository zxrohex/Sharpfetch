using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Sharpfetch.Core.Helpers
{
    public class MarkupForrmatter
    {
        public static string FormatAndMarkup(string heading, Dictionary<string, string> keyValuePairs, Color? fgColor = null)
        {
            StringBuilder sb = new StringBuilder();

            string emphasisMarkup = "bold";

            if (fgColor != null)
            {
                emphasisMarkup = $"bold #{fgColor?.R:X2}{fgColor?.G:X2}{fgColor?.B:X2}";
            }

            sb.AppendLine($"[{emphasisMarkup}]{heading}[/]");
            sb.Append('-', heading.Length);
            sb.Append("\n");

            foreach (var kvp in keyValuePairs)
            {
           
                sb.AppendLine($"[{emphasisMarkup}]{kvp.Key}:[/] {kvp.Value}");
            }

            return sb.ToString();
        }
    }
}
