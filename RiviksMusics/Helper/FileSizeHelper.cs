using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RiviksMusics.Helper
{
   
    public static class FileSizeHelper
    {
        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                }
                max /= scale;
            }
            return "0 Bytes";
        }
        public static string ToShorthand(long number)
        {
            if (number >= 1000000)
                return (number / 1000000D).ToString("0.#") + "M";
            if (number >= 1000)
                return (number / 1000D).ToString("0.#") + "K";

            return number.ToString();
        }
    }
    
}
