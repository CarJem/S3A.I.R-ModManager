using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using MessageBox = System.Windows.Forms.MessageBox;
using GenerationsLib.Core;

namespace Sonic3AIR_ModManager
{
    public partial class ModManagerUpdater : Window
    {
        #region Version Check Class
        public class VersionCheck
        {
            public string FilePath = "";
            private dynamic RawJSON;

            public string VersionString;
            public Version Version;
            public string DownloadURL;
            public string Details = "";
            public VersionCheck(FileInfo config)
            {
                FilePath = config.FullName;
                string data = File.ReadAllText(FilePath);
                RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);


                VersionString = RawJSON.Metadata.Version;
                Version = new Version(VersionString);
                DownloadURL = RawJSON.Metadata.DownloadURL;
                Details = RawJSON.Metadata.Details;
            }
        }
        #endregion

        private VersionCheck ModManagerVersionCheckInfo;
        private string VersionCheckFileName = "";
        private string UpdateFileName = "";

        private bool ManuallyTriggered = false;

        private bool DisableUpdater = false;
        private ModManagerUpdater Instance;
        public bool isOffline = false;

        public ModManagerUpdater(bool _manuallyTriggered = false)
        {
            InitializeComponent();
            try { this.Owner = System.Windows.Application.Current.MainWindow; }
            catch { }
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);

            ManuallyTriggered = _manuallyTriggered;

            this.GetUpdates();
        }

        private void GetUpdates()
        {
            if (DisableUpdater == false) CheckForModManagerUpdates();
            else UpdateBypass();
        }

        private void UpdateBypass()
        {
            Program.MMUpdateResults = Program.UpdateResult.UpToDate;
            Program.MMUpdaterState = Program.UpdateState.Finished;
            Close();
        }

        #region Network Checking

        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(100);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        #endregion

        #region Updating Checking / Downloading Updates

        public void CheckForModManagerUpdates()
        {
            try
            {
                isOffline = IsNetworkAvailable();

                if (isOffline)
                {
                    string url = "";
                    url = @"https://raw.githubusercontent.com/CarJem/GenerationsLib.Updates/master/UpdateMetadata/AIR_MM_Updates.json";
                    VersionCheckFileName = DownloadFromURL(url, ProgramPaths.Sonic3AIR_MM_BaseFolder, DownloadCheckComplete);
                }
                else
                {
                    Program.MMUpdateResults = Program.UpdateResult.Offline;
                    Program.MMUpdaterState = Program.UpdateState.Finished;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Program.MMUpdateResults = Program.UpdateResult.Error;
                Program.MMUpdaterState = Program.UpdateState.Finished;
                Close();
            }




        }

        private string GetRemoteFileName(string baseURL)
        {
            Uri uri = new Uri(baseURL);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private void DownloadCheckComplete()
        {
            string destination = ProgramPaths.Sonic3AIR_MM_BaseFolder;
            string path = $"{destination}//{VersionCheckFileName}";
            var file = new FileInfo(path);
            ModManagerVersionCheckInfo = new VersionCheck(file);

            var block = new Paragraph(new Run(ModManagerVersionCheckInfo.Details));
            richTextBox1.Document.Blocks.Add(block);

            file.Delete();

            var result = Program.InternalVersion.CompareTo(ModManagerVersionCheckInfo.Version);

            if (result < 0)
            {
                Program.MMUpdateResults = Program.UpdateResult.OutOfDate;
                Program.CheckedForUpdateOnStartup = true;
            }
            else
            {
                Program.MMUpdateResults = Program.UpdateResult.UpToDate;
                Program.CheckedForUpdateOnStartup = true;
            }

            if (Program.MMUpdateResults == Program.UpdateResult.OutOfDate)
            {
                updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
                if (ShowDialog() == true)
                {
                    DownloadUpdate();
                }
                else
                {
                    Program.MMUpdaterState = Program.UpdateState.Finished;
                    Close();
                }
            }
            else if (ManuallyTriggered && Program.MMUpdateResults == Program.UpdateResult.UpToDate)
            {
                ManuallyTriggered = false;
                updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_UpToDate");
                if (ShowDialog() == true)
                {
                    DownloadUpdate();
                }
                else
                {
                    Program.MMUpdaterState = Program.UpdateState.Finished;
                    Close();
                }
            }
            else
            {
                Program.MMUpdaterState = Program.UpdateState.Finished;
                Close();
            }





        }

        private void UpdateDownloadComplete()
        {
            //TODO: Proper Implementation
            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM\\downloads\\" + UpdateFileName;
            System.Diagnostics.Process.Start(destination);
            Environment.Exit(0);

        }

        private string GetBaseURL(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;  // IMPORTANT

            webRequest.Timeout = 10000;           // timeout 10s
            webRequest.Method = "HEAD";
            // Get the response ...
            HttpWebResponse webResponse;
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                {
                    string uriString = webResponse.Headers["Location"];
                    return uriString;
                }
            }
            return "";
        }

        private void DownloadUpdate()
        {
            string DownloadURL = ModManagerVersionCheckInfo.DownloadURL;
            string LatestVersion = ModManagerVersionCheckInfo.Version.ToString();

            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
            string DownloadsFolder = BaseFolder + "\\downloads";

            UpdateFileName = DownloadFromURL(DownloadURL, DownloadsFolder, UpdateDownloadComplete, false);
        }

        private string DownloadFromURL(string url, string destination, Action finishAction, bool backgroundDownload = true)
        {
            string baseURL = GetBaseURL(url);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (url != "") remote_filename = GetRemoteFileName(url);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            DownloadWindow downloadWindow = new DownloadWindow($"{Program.LanguageResource.GetString("Downloading")} \"{filename}\"", url, $"{destination}\\{filename}");
            downloadWindow.DownloadCompleted = finishAction;
            if (backgroundDownload) downloadWindow.StartBackground();
            else downloadWindow.Start();
            return filename;
        }

        #endregion

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
