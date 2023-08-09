using System;
using System.Text;

namespace DiscordBotCS.Services.Other
{
    public static class Tools
    {
        public static string Base64UrlDecode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "<strong>Message body was not returned from Google</strong>";

            var InputStr = input.Replace("-", "+").Replace("_", "/");
            return Encoding.UTF8.GetString(Convert.FromBase64String(InputStr));
        }

        public static string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        public static string EncodeMessage(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}