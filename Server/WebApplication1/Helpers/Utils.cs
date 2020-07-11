using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string UploadImage(string base64)
        {
            // Build path
            var dataPath = Path.Combine(
                Directory.GetParent(Directory.GetParent(
                    Assembly.GetExecutingAssembly().Location
                    ).FullName).FullName, 
                "Data"
            );
            var guid = Guid.NewGuid().ToString("B");
            var filePath = Path.Combine(dataPath, guid);

            // Write
            byte[] imgData = Convert.FromBase64String(base64);
            File.WriteAllBytes(filePath, imgData);

            // Return
            return guid;
        }

        public static string DownloadImage(string guid)
        {
            var content = string.Empty;

            return content;
        }
    }
}
