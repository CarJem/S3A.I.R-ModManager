using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>

    public partial class DownloadWindow : Window
    {
        public string URL;
        public string DestinationPath;
        public WebClient DownloadClient;
        public Action DownloadCompleted;


        public DownloadWindow(string header, string url, string destinationFile)
        {
            InitializeComponent();
            Title = header;
            Header.Text = header;
            URL = url;
            DestinationPath = destinationFile;
        }

        public void StartBackground()
        {
            DownloadClient = new WebClient();
            DownloadClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
            DownloadClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += WebClient_DownloadCompleted;
            DownloadClient.DownloadFileAsync(new Uri(URL), DestinationPath);
        }

        public void Start()
        {
            Show();
            DownloadClient = new WebClient();
            DownloadClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");
            DownloadClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += WebClient_DownloadCompleted;
            DownloadClient.DownloadFileAsync(new Uri(URL), DestinationPath);
        }

        protected void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                Progress.Maximum = args.TotalBytesToReceive;
                Progress.Value = args.BytesReceived;
                TaskbarItemInfo.ProgressValue = (float)args.BytesReceived / (float)args.TotalBytesToReceive;
                ProgressText.Text = $"{(int)ConvertBytesToMegabytes(args.BytesReceived)} MB / {(int)ConvertBytesToMegabytes(args.TotalBytesToReceive)} MB";
            });
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }


        protected void WebClient_DownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            var file = new System.IO.FileInfo(DestinationPath);
            bool cannotProceed = true;
            while (cannotProceed)
            {
                if(GenerationsLib.Core.FileHelpers.IsFileLocked(file))
                {

                    string text = Management.UserLanguage.GetOutputString("FileIsBusyCaption").Replace("{0}", file.FullName);
                    string title = Management.UserLanguage.GetOutputString("FileIsBusyTitle");
                    var result = MessageBox.Show(text, title, MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes) cannotProceed = false;
                }
                else
                {
                    cannotProceed = false;
                }

            }

            Dispatcher.Invoke(() =>
            {
                Close();
                DownloadCompleted?.Invoke();
            });

        }
    }
}
