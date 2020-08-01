using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat
{
    public static class StringExFunc
    {
        public static string ReplacePersianNumbers(this string text)
        {
            if (String.IsNullOrEmpty(text)) return text;

            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < text.Length; ++i)
                if (sb[i] >= '۰' && sb[i] <= '۹')
                    sb[i] = (char)('0' + sb[i] - '۰');

            return sb.ToString();
        }
    }
}
