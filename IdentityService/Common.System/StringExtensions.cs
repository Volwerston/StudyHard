using System;
using System.Text;

namespace Common.System
{
    public static class StringExtensions
    {
        public static string ToBase64(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string FromBase64(this string base64Text)
        {
            var plainTextBytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }
}
