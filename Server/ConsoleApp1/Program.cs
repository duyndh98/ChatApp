using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private const int MAX_WIDTH = 1280;
        private const int MAX_HEIGHT = 720;

        static void Main(string[] args)
        {
            var imgPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads\Untitled.png");
            var inpImg = File.ReadAllBytes(imgPath);

            var outImg = PreProcessImage(inpImg, MAX_WIDTH, MAX_HEIGHT, ImageFormat.Jpeg);
            File.WriteAllBytes(imgPath + ".jpeg", outImg);
        }

        public static byte[] PreProcessImage(byte[] inputImgData, int maxWidth, int maxHeight, ImageFormat imgFormat)
        {
            byte[] outputData = null;

            using (MemoryStream inpStream = new MemoryStream())
            {
                inpStream.Write(inputImgData, 0, inputImgData.Length);
                inpStream.Seek(0, SeekOrigin.Begin);

                using (var inputBmp = new Bitmap(inpStream))
                {
                    using (var outputBmp = ScaleImage(inputBmp, maxWidth, maxHeight))
                    {
                        using (var outStream = new MemoryStream())
                        {
                            outputBmp.Save(outStream, imgFormat);
                            outputData = outStream.ToArray();
                        }
                    }
                }
            }

            return outputData;
        }

        public static Bitmap ScaleImage(Bitmap bmp, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / bmp.Width;
            var ratioY = (double)maxHeight / bmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(bmp.Width * ratio);
            var newHeight = (int)(bmp.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(bmp, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}
