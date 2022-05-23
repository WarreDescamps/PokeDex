using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class TimeSpanExtensions
    {
        public static string ToStringX(this TimeSpan ts)
        {
            return $"{(ts.Minutes > 0 ? $"{ts.Minutes}min{(ts.Minutes > 1 ? "s " : " ")}" : "")}{ts.Seconds}sec{(ts.Seconds > 1 ? "s" : "")}";
        }
    }
}
