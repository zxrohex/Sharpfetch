using System;
using System.Collections.Generic;
using System.Text;

namespace Sharpfetch.Core.Helpers.Extensions
{
    public static class TextExtensions
    {
        public static string Bold(this string value)
        {
            return $"[bold]{value}[/]";
        }

        public static string Color(this string value, System.Drawing.Color color)
        {
            return $"[#{color.R:X2}{color.G:X2}{color.B:X2}]{value}[/]";
        }

        public static string Italic(this string value)
        {
            return $"[italic]{value}[/]";
        }

        public static string Underline(this string value)
        {
            return $"[underline]{value}[/]";
        }

        public static string Strikethrough(this string value)
        {
            return $"[strikethrough]{value}[/]";
        }

        public static string Dim(this string value)
        {
            return $"[dim]{value}[/]";
        }

        public static string Invert(this string value)
        {
            return $"[invert]{value}[/]";
        }

        public static string Link(this string value, string url)
        {
            return $"[link={url}]{value}[/]";
        }

    }
}
