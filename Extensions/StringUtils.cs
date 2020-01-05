using System;
namespace AfyaHMIS.Extensions {
    public static class StringUtils {
        public static string ToValidString(this string str) {
            return string.IsNullOrEmpty(str) ? "" : str.Trim().Replace("'", "`");
        }
    }
}
