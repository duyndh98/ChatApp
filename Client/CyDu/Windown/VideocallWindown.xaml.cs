using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using CyDu.Ultis;
using Microsoft.AspNetCore.SignalR.Client;
using System.Drawing;

namespace CyDu.Windown
{


    /// <summary>
    /// Interaction logic for VideocallWindown.xaml
    /// </summary>
    /// 
    //mode 1 = cam 
    //mode 2 = Sceen
    public partial class VideocallWindown : Window
    {
        HubConnection connection;
        private long ConversationId;
       
        public VideocallWindown(long conversationid,int mode = 1)
        {
            InitializeComponent();
            Title = AppInstance.getInstance().GetUser().FullName;
            this.ConversationId = conversationid;
            LoadHub();
            mode = 1;
            if (mode == 1)
            {
                LoadCam();
                ShareButtonBt.IsEnabled = false;
            }
            else
            {
                LoadSceenCapture();
                ShareScreenBt.IsEnabled = false;
            }

            //chattingpanel.Children.Clear();
            //ChattingPanel chattingPanel = new ChattingPanel(1);
            //chattingpanel.Children.Add(chattingPanel);

        }

        #region Load Connection Hub
        private async void LoadHub()
        {
            string url = Ultils.url + "callhub";

            connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            connection.Reconnecting += error =>
            {

                return Task.CompletedTask;
            };

            connection.On<string>("ReceiveMessage", (message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    string id = message.Split('\0')[0];
                    string mess = message.Split('\0')[1];
                    if (long.Parse(id) != AppInstance.getInstance().GetUser().Id)
                        mainCall.Source = ImageSupportInstance.getInstance().ConvertFromBaseString(mess);
                });
            });

            try
            {
                await connection.StartAsync();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            await connection.InvokeAsync("AddToGroup", ConversationId.ToString());
        }

        #endregion


        #region Record cam
        VideoCaptureDevice videoSource;

        [Obsolete]
        private void LoadCam()
        {


            //List all available video sources. (That can be webcams as well as tv cards, etc)
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            //Check if atleast one video source is available
            if (videosources != null)
            {
                //For example use first video device. You may check if this is your webcam.
                videoSource = new VideoCaptureDevice(videosources[0].MonikerString);

                try
                {
                    //Check if the video device provides a list of supported resolutions
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        string highestSolution = "1280;720";
                        //Search for the lowestt resolution
                        for (int i = 0; i < videoSource.VideoCapabilities.Length; i++)
                        {
                            if (videoSource.VideoCapabilities[i].FrameSize.Width < Convert.ToInt32(highestSolution.Split(';')[0]))
                                highestSolution = videoSource.VideoCapabilities[i].FrameSize.Width.ToString() + ";" + i.ToString();
                        }
                        //Set the lowestt resolution as active
                        videoSource.VideoResolution = videoSource.VideoCapabilities[Convert.ToInt32(highestSolution.Split(';')[1])];


                    }
                }
                catch { }

                //Create NewFrame event handler
                //(This one triggers every time a new frame/image is captured
                videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(videoSource_NewFrame);

                //Start recording
                videoSource.Start();
            }

        }

        delegate void UpdateUserCamCallback(Bitmap img);

        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            //Cast the frame as Bitmap object and don't forget to use ".Clone()" otherwise
            //you'll probably get access violation exceptions
            Bitmap bm = (Bitmap)eventArgs.Frame.Clone();


            Dispatcher.BeginInvoke(new UpdateUserCamCallback(UpdateCameraSource), new object[] { bm });

        }

        private async void UpdateCameraSource(Bitmap bm)
        {
            BitmapImage img = ImageSupportInstance.getInstance().BitmapToImageSource(bm).Clone();
            userCam.Source = img.Clone();

            MemoryStream ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            //byte[] afterreprocess = ImageSupportInstance.getInstance().PreProcessImage(byteImage, 480, 360, System.Drawing.Imaging.ImageFormat.Jpeg);
            string result = Convert.ToBase64String(byteImage);
            result = AppInstance.getInstance().GetUser().Id + "\0" + result;
            byteImage = new byte[1];
            //afterreprocess = new byte[1];
            ms.Close();
            try
            {
                await connection.InvokeAsync("SendMessageToGroup", ConversationId.ToString(),
                   result);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }



        #endregion

        #region Record Sceen
        ScreenCaptureStream CaptureSource;

        private void LoadSceenCapture()
        {

            System.Drawing.Rectangle screenArea = System.Drawing.Rectangle.Empty;
            screenArea.Width = 20 ;
            screenArea.Height = 20;
            CaptureSource = new ScreenCaptureStream(screenArea,5);
            CaptureSource.NewFrame += new AForge.Video.NewFrameEventHandler(screenSource_NewFrame);
            CaptureSource.Start();
        }

        delegate void UpdateUserScreenCallback(Bitmap img);

        void screenSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            //Cast the frame as Bitmap object and don't forget to use ".Clone()" otherwise
            //you'll probably get access violation exceptions
            Bitmap bm = (Bitmap)eventArgs.Frame.Clone();


            Dispatcher.BeginInvoke(new UpdateUserScreenCallback(UpdateSceernSource), new object[] { bm });

        }

        private async void UpdateSceernSource(Bitmap bm)
        {
            //BitmapImage img = ImageSupportInstance.getInstance().BitmapToImageSource(bm).Clone();
            //userCam.Source = img.Clone();

            MemoryStream ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            byte[] afterreprocess = ImageSupportInstance.getInstance().PreProcessImage(byteImage, 100, 50, System.Drawing.Imaging.ImageFormat.Jpeg);
            string result = Convert.ToBase64String(afterreprocess);
            BitmapImage img2 = ImageSupportInstance.getInstance().ConvertFromBaseString(result);
            userCam.Source = img2;
            result = AppInstance.getInstance().GetUser().Id + "\0" + result;
            byteImage = new byte[1];
            afterreprocess = new byte[1];
            ms.Close();


            try
            {
                await connection.InvokeAsync("SendMessageToGroup", ConversationId.ToString(),
                   result);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            //Stop and free the webcam object if application is closing
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
            if (CaptureSource != null)
            {
                CaptureSource.SignalToStop();
                CaptureSource = null; 
            }
        }

        private void ShareScreenBt_Click(object sender, RoutedEventArgs e)
        {
            videoSource.Stop();
            videoSource.SignalToStop();
            ShareScreenBt.IsEnabled = false;
            ShareButtonBt.IsEnabled = true;
            LoadSceenCapture();
        }

        private void ShareButtonBt_Click(object sender, RoutedEventArgs e)
        {
            CaptureSource.Stop();
            CaptureSource.SignalToStop();
            ShareScreenBt.IsEnabled = true;
            ShareButtonBt.IsEnabled = false;
            LoadCam();
        }
    }
}
