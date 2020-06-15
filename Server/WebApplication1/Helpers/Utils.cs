using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public static class Utils
    {
        public static long DateTimeToInt64(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DateTime Int64ToDateTime(long delta)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(delta);
        }
    }
}
