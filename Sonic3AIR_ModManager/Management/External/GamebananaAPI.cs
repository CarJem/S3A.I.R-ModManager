using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Sonic3AIR_ModManager.Management
{
    public static class GamebananaAPI
    {
        #region Definitions

        public static FileSystemWatcher GBAPIWatcher = new FileSystemWatcher(Management.ProgramPaths.Sonic3AIR_MM_GBRequestsFolder);
        public static bool isDownloadAllowed = true;
        public static bool HasAPIChangeBeenDetected = false;

        #endregion

        #region Events/Methods
        public static void GBAPIWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            HasAPIChangeBeenDetected = true;
        }
        public static void GBAPIInstallTrigger()
        {
            if (isDownloadAllowed && HasAPIChangeBeenDetected)
            {
                HasAPIChangeBeenDetected = false;
                isDownloadAllowed = false;
                GetNextAPIPendingInstall();
                isDownloadAllowed = true;
            }
        }
        public static void GetNextAPIPendingInstall()
        {
            string file = Directory.EnumerateFiles(Management.ProgramPaths.Sonic3AIR_MM_GBRequestsFolder).FirstOrDefault();
            if (file != null)
            {
                if (file.Contains("gb_api") && file.Contains(".txt"))
                {
                    string urlData = File.ReadAllText(file);
                    urlData = urlData.Replace("\r\n", "");
                    GamebananaAPI_Install(urlData);
                    File.Delete(file);
                }
            }

        }
        public static void GamebananaAPI_Install(string data)
        {
            Program.Log.InfoFormat("[GBPAPI] Attempting to Download Mod...");
            if (!Directory.Exists(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);
            string url = data.Replace("s3airmm://", "");
            if (url == "") MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else DownloaderAPI.DownloadFile(url, Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder, GamebananaAPI_DownloadFinished, false);

        }
        public static void GamebananaAPI_DownloadFinished()
        {
            Management.ModManagement.DownloadModCompleted();
            GetNextAPIPendingInstall();
        }
        #endregion
    }
}
