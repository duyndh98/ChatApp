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

        public static void UploadData(byte[] data, string code)
        {
            // Build path
            var dataPath = Path.Combine(
                Directory.GetParent(Directory.GetParent(
                    Assembly.GetExecutingAssembly().Location
                    ).FullName).FullName,
                "Data"
            );

            // Validate
            var filePath = Path.Combine(dataPath, code);
            if (File.Exists(filePath))
                throw new Exception("File path already exist");

            // Write
            File.WriteAllBytes(filePath, data);
        }

        public static byte[] DownloadData(string code)
        {
            // Build path
            var dataPath = Path.Combine(
                Directory.GetParent(Directory.GetParent(
                    Assembly.GetExecutingAssembly().Location
                    ).FullName).FullName,
                "Data"
            );

            // Validate
            var filePath = Path.Combine(dataPath, code);
            if (!File.Exists(filePath))
                throw new Exception("File path not found");

            // Read
            var data = File.ReadAllBytes(filePath);

            // Return
            return data;
        }

        public static void DeleteData(string code)
        {
            // Build path
            var dataPath = Path.Combine(
                Directory.GetParent(Directory.GetParent(
                    Assembly.GetExecutingAssembly().Location
                    ).FullName).FullName,
                "Data"
            );

            // Validate
            var filePath = Path.Combine(dataPath, code);
            if (!File.Exists(filePath))
                throw new Exception("File path not found");

            // Delete
            File.Delete(filePath);
        }
    }
}
