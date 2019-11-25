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

namespace Sonic3AIR_ModManager
{
    public partial class Updater : Window
    {
        public enum UpdateResult : int
        {
            OutOfDate,
            UpToDate,
            Offline,
            FileNotFound,
            ValueNull,
            Null,
            Error
        }

        public enum UpdateState : int
        {
            Running,
            Finished,
            NeverStarted,
        }

        public AIR_API.VersionCheck VersionCheckInfo;
        private AIR_API.Settings SettingsFile;

        private string VersionCheckFileName = "";
        private string UpdateFileName = "";

        private bool ManuallyTriggered = false;

        private bool DisableUpdater = false;
        private Updater Instance;

        public Updater(bool _manuallyTriggered = false)
        {
            InitializeComponent();
            try { this.Owner = System.Windows.Application.Current.MainWindow; }
            catch { }
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);

            ManuallyTriggered = _manuallyTriggered;

            if (DisableUpdater == false) CheckForUpdates();
            else UpdateBypass();
        }

        private void UpdateBypass()
        {
            Program.UpdateResult = UpdateResult.UpToDate;
            Program.UpdaterState = UpdateState.Finished;
            Close();
        }



        #region Updating Checking / Downloading Updates

        public void CheckForUpdates()
        {
            try
            {
                bool isDeveloper = false;
                bool isNetAccessable = IsNetworkAvailable();

                if (isNetAccessable)
                {
                    string url = "";
                    if (isDeveloper) url = @"http://sonic3air.org/sonic3air_updateinfo_dev.json";
                    else url = @"http://sonic3air.org/sonic3air_updateinfo.json";
                    VersionCheckFileName = DownloadFromURL(url, ProgramPaths.Sonic3AIR_MM_BaseFolder, DownloadCheckComplete);
                }
                else
                {
                    Program.UpdateResult = UpdateResult.Offline;
                    Program.UpdaterState = UpdateState.Finished;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Program.UpdateResult = UpdateResult.Error;
                Program.UpdaterState = UpdateState.Finished;
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
            FileInfo file = new FileInfo(path);
            VersionCheckInfo = new AIR_API.VersionCheck(file);

            var block = new Paragraph(new Run(VersionCheckInfo.Details));
            richTextBox1.Document.Blocks.Add(block);

            string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR" + "\\settings.json";
            FileInfo settingsFile = new FileInfo(settingsPath);


            file.Delete();

            if (File.Exists(settingsPath))
            {
                SettingsFile = new AIR_API.Settings(settingsFile);
                if (SettingsFile.Version != null)
                {
                    var result = SettingsFile.Version.CompareTo(VersionCheckInfo.Version);
                    var result2 = CheckFromSelectedVersion(VersionCheckInfo.Version);
                    if (result < 0)
                    {
                        if (result2 < 0)
                        {
                            Program.UpdateResult = UpdateResult.OutOfDate;
                            Program.CheckedForUpdateOnStartup = true;
                        }
                        else
                        {
                            Program.UpdateResult = UpdateResult.UpToDate;
                            Program.CheckedForUpdateOnStartup = true;
                        }

                    }
                    else
                    {
                        Program.UpdateResult = UpdateResult.UpToDate;
                        Program.CheckedForUpdateOnStartup = true;
                    }
                } 
                else
                {
                    Program.UpdateResult = UpdateResult.ValueNull;
                    Program.CheckedForUpdateOnStartup = true;
                }
            }
            else
            {
                Program.UpdateResult = UpdateResult.FileNotFound;
                Program.CheckedForUpdateOnStartup = true;
            }

            if (Program.UpdateResult == UpdateResult.OutOfDate)
            {
                updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
                if (ShowDialog() == true)
                    if (ShowDialog() == true)
                    {
                        DownloadUpdate();
                    }
                    else
                    {
                        Program.UpdaterState = UpdateState.Finished;
                        Close();
                    }
            }
            else if (ManuallyTriggered && Program.UpdateResult == UpdateResult.UpToDate)
            {
                ManuallyTriggered = false;
                updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_UpToDate");
                if (ShowDialog() == true)
                {
                    DownloadUpdate();
                }
                else
                {
                    Program.UpdaterState = UpdateState.Finished;
                    Close();
                }
            }
            else
            {
                Program.UpdaterState = UpdateState.Finished;
                Close();
            }





        }

        private int CheckFromSelectedVersion(Version comparision)
        {

            string path = (ProgramPaths.Sonic3AIRPath != null && ProgramPaths.Sonic3AIRPath != "" ? Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath) : "");
            if (path == "") return 0;
            else
            {
                string metaDataFile = Directory.GetFiles(path, "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                if (metaDataFile != null)
                {
                    try
                    {
                        var metadata = new AIR_API.VersionMetadata(new FileInfo(metaDataFile));
                        return metadata.Version.CompareTo(comparision);
                    }
                    catch
                    {
                        return 0;
                    }
                }
                else return 0;
            }

        }

        private void UpdateDownloadComplete()
        {
            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM\\downloads";
            string file = $"{destination}\\{UpdateFileName}";
            string output = destination;

            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(output, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }

            try
            {
                string metaDataFile = Directory.GetFiles(output, "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                FileInfo fileInfo = new FileInfo(metaDataFile);
                AIR_API.VersionMetadata ver = new AIR_API.VersionMetadata(fileInfo);


                string output2 = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Sonic3AIR_MM\\air_versions\\{ver.VersionString}\\sonic3air_game";

                if (Directory.Exists(output2)) Directory.Delete(output2, true);

                Directory.Move(Path.Combine(destination, "sonic3air_game"), output2);

                MessageBox.Show($"{Program.LanguageResource.GetString("GameInstalledAt")} \"{output2}\"");

                Program.UpdaterState = UpdateState.Finished;
                Close();
            }
            catch
            {
                MessageBox.Show(Program.LanguageResource.GetString("UpdateFailedError"));

                Program.UpdaterState = UpdateState.Finished;
                Close();
            }




        }

        private void WipeFolderContents(string folder)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
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
            string AIR_Path = SettingsFile.AIREXEPath;
            string DownloadURL = VersionCheckInfo.DownloadURL;
            string CurrentVersion = SettingsFile.Version.ToString();
            string LatestVersion = VersionCheckInfo.Version.ToString();

            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
            string DownloadsFolder = BaseFolder + "\\downloads";
            string VersionsFolder = BaseFolder + "\\air_versions";

            if (!Directory.Exists(BaseFolder)) Directory.CreateDirectory(BaseFolder);
            if (!Directory.Exists(DownloadsFolder)) Directory.CreateDirectory(DownloadsFolder);
            if (!Directory.Exists(VersionsFolder)) Directory.CreateDirectory(VersionsFolder);

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
            return IsNetworkAvailable(0);
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

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //this.DialogResult = false;
        }
    }
}
