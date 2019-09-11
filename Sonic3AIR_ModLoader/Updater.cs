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

namespace Sonic3AIR_ModLoader
{
    public partial class Updater : Form
    {
        public AIR_SDK.VersionCheck VersionCheckInfo;
        public Updater()
        {
            CheckForUpdates();
        }

        public void CheckForUpdates()
        {
            string url = @"http://sonic3air.org/sonic3air_updateinfo_2.json";

            string baseURL = GetBaseURL(url);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (url != "") remote_filename = GetRemoteFileName(url);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            string destination = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            DownloadWindow downloadWindow = new DownloadWindow($"Downloading \"{filename}\"", url, $"{destination}\\{filename}");
            Action finishAction = DownloadUpdateInfoComplete;
            downloadWindow.DownloadCompleted = finishAction;
            downloadWindow.StartBackground();

        }

        private string GetRemoteFileName(string baseURL)
        {
            Uri uri = new Uri(baseURL);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private void DownloadUpdateInfoComplete()
        {
            string destination = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string path = $"{destination}//sonic3air_updateinfo_2.json";
            FileInfo file = new FileInfo(path);
            VersionCheckInfo = new AIR_SDK.VersionCheck(file);

            FileInfo settingsFile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR" + "\\settings.json");

            if (settingsFile.Exists)
            {
                AIR_SDK.Settings Settings = new AIR_SDK.Settings(settingsFile, true);
                var result = Settings.Version.CompareTo(VersionCheckInfo.Version);
                if (result < 0)
                {
                    ShowDialog();
                }
            }
            Program.CanUpdaterRun = true;
            Close();


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

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
