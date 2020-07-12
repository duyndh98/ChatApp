using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CyDu.Model;
using Microsoft.Win32;

namespace CyDu.Ultis
{
    public class ImageSupportInstance
    {
        private static ImageSupportInstance _instance = null;
        private static Dictionary<long, string> _dirUserImage;

        public static ImageSupportInstance getInstance()
        {
            if (_instance == null)
            {
                _instance = new ImageSupportInstance();
            }
            return _instance;
        }

        protected ImageSupportInstance()
        {
            _dirUserImage = new Dictionary<long, string>();
        }

        public string OpenChooseImageDialogBox()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image file (*.png)|*png";
            if (openFileDialog.ShowDialog() == true)
            {
                return (openFileDialog.FileNames[0]);
            }
            return "";
        }

        public Resource UploadImage(string path,int maxwidth , int maxheight)
        {
            //FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            //int length = Convert.ToInt32(file.Length);
            //byte[] data = new byte[length];
            //file.Read(data, 0, length);
            //file.Close();
            string base64 = GetImageCompressBaseString(path, maxwidth, maxheight);

            Resource resource = new Resource()
            {
                Data = base64,
                Name = "imagename :3",
                Type = 1
            };

            using (HttpClient client = new HttpClient())
            {
                string url = Ultils.getUrl();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.PostAsJsonAsync("/api/Resources", resource).Result;
                if (response.IsSuccessStatusCode)
                {
                    Resource res = response.Content.ReadAsAsync<Resource>().Result;
                    return res;
                }
            }
            return null;
        }

        public string getImageResourceBaseString(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Ultils.getUrl();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                HttpResponseMessage response = client.GetAsync("/api/Resources/"+id).Result;
                if (response.IsSuccessStatusCode)
                {
                    Resource res = response.Content.ReadAsAsync<Resource>().Result;
                    return res.Data;
                }
            }
            return "";
        }

        public string getUserBase64Image(long id)
        {
            if (_dirUserImage.ContainsKey(id))
            {
                return _dirUserImage[id];
            }
            else
            {
                long imgid = 0;
                using (HttpClient client = new HttpClient())
                {
                    string url = Ultils.getUrl();
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppInstance.getInstance().GetUser().Token);
                    HttpResponseMessage response = client.GetAsync("/api/Users/" + id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        User res = response.Content.ReadAsAsync<User>().Result;
                        imgid = res.Avatar;
                    }
                }
                if (imgid != 0)
                {
                    return getImageResourceBaseString(imgid);
                }
                else
                    return "";
            }
        }

        public BitmapImage ConvertFromBaseString(string base64str)
        {
            byte[] data = System.Convert.FromBase64String(base64str);
            MemoryStream memoryStream = new MemoryStream(data);
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = memoryStream;
            imageSource.EndInit();
            return imageSource;
        }

        public BitmapImage GetUserImageFromId(long id)
        {
            string base64str = getUserBase64Image(id);
            if (!base64str.Equals(""))
            {
                return ConvertFromBaseString(base64str);
            }
            return new BitmapImage();
        }


        public string GetImageCompressBaseString(string path,int maxwidth,int maxhight)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            int length = Convert.ToInt32(file.Length);
            byte[] data = new byte[length];
            file.Read(data, 0, length);
            file.Close();
            byte[] outdata = PreProcessImage(data, maxwidth, maxhight, ImageFormat.Png);
            string base64 = Convert.ToBase64String(outdata);
            return base64;
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
