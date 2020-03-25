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
using System.Diagnostics;

namespace Sonic3AIR_ModManager
{
    public partial class Updater : Window
    {
        #region Definitions

        private AIR_API.VersionCheck VersionCheckInfo;

        private string VersionCheckFileName = "";
        private string UpdateFileName = "";

        private bool ManuallyTriggered = false;

        private bool DisableUpdater = false;
        private Updater Instance;
        public bool isOnline = false;

        #endregion

        #region Init

        public Updater(bool _manuallyTriggered = false)
        {
            Program.AIRUpdaterState = Program.UpdateState.Checking;
            Program.Log.InfoFormat("Checking for A.I.R. Updates...");
            InitializeComponent();
            try { this.Owner = System.Windows.Application.Current.MainWindow; }
            catch { }
            Instance = this;
            Management.UserLanguage.ApplyLanguage(ref Instance);

            ManuallyTriggered = _manuallyTriggered;

            this.GetUpdates();
        }

        #endregion

        #region Methods

        private void GetUpdates()
        {
            if (DisableUpdater == false) CheckForUpdates();
            else UpdateBypass();
        }
        private void UpdateBypass()
        {
            Program.AIRUpdateResults = Program.UpdateResult.UpToDate;
            Program.AIRUpdaterState = Program.UpdateState.Finished;
            Close();
        }
        private void CheckForUpdates()
        {
            try
            {
                bool isDeveloper = false;
                isOnline = Management.DownloaderAPI.IsNetworkAvailable();

                if (isOnline)
                {
                    string url = "";
                    if (isDeveloper) url = @"http://sonic3air.org/sonic3air_updateinfo_dev.json";
                    else url = @"http://sonic3air.org/sonic3air_updateinfo.json";
                    VersionCheckFileName = Management.DownloaderAPI.DownloadFile(url, Management.ProgramPaths.Sonic3AIR_MM_BaseFolder, DownloadCheckComplete);
                }
                else
                {
                    Program.AIRUpdateResults = Program.UpdateResult.Offline;
                    Program.AIRUpdaterState = Program.UpdateState.Finished;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Program.AIRUpdateResults = Program.UpdateResult.Error;
                Program.AIRUpdaterState = Program.UpdateState.Finished;
                Close();
            }




        }
        private void DownloadUpdate()
        {
            string DownloadURL = VersionCheckInfo.DownloadURL;
            string LatestVersion = VersionCheckInfo.Version.ToString();

            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
            string DownloadsFolder = BaseFolder + "\\downloads";
            string VersionsFolder = BaseFolder + "\\air_versions";

            if (!Directory.Exists(BaseFolder)) Directory.CreateDirectory(BaseFolder);
            if (!Directory.Exists(DownloadsFolder)) Directory.CreateDirectory(DownloadsFolder);
            if (!Directory.Exists(VersionsFolder)) Directory.CreateDirectory(VersionsFolder);

            UpdateFileName = Management.DownloaderAPI.DownloadFile(DownloadURL, DownloadsFolder, UpdateDownloadComplete, false);
        }

        #endregion

        #region Events

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region Download Complete Events

        private void DownloadCheckComplete()
        {
            try
            {
                string destination = Management.ProgramPaths.Sonic3AIR_MM_BaseFolder;
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
                    Version LatestVersion = GetLatestVersion(settingsFile);
                    if (LatestVersion != null)
                    {
                        var result = LatestVersion.CompareTo(VersionCheckInfo.Version);
                        var result2 = CheckFromSelectedVersion(VersionCheckInfo.Version);
                        if (result < 0)
                        {
                            if (result2 < 0)
                            {
                                Program.AIRUpdateResults = Program.UpdateResult.OutOfDate;
                                Program.CheckedForUpdateOnStartup = true;
                            }
                            else
                            {
                                Program.AIRUpdateResults = Program.UpdateResult.UpToDate;
                                Program.CheckedForUpdateOnStartup = true;
                            }

                        }
                        else
                        {
                            Program.AIRUpdateResults = Program.UpdateResult.UpToDate;
                            Program.CheckedForUpdateOnStartup = true;
                        }
                    }
                    else
                    {
                        Program.AIRUpdateResults = Program.UpdateResult.ValueNull;
                        Program.CheckedForUpdateOnStartup = true;
                    }
                }
                else
                {
                    Program.AIRUpdateResults = Program.UpdateResult.FileNotFound;
                    Program.CheckedForUpdateOnStartup = true;
                }

                if (Program.AIRUpdateResults == Program.UpdateResult.OutOfDate || Program.AIRUpdateResults == Program.UpdateResult.ValueNull)
                {
                    updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
                    if (ShowDialog() == true)
                    {
                        DownloadUpdate();
                    }
                    else
                    {
                        Program.AIRUpdaterState = Program.UpdateState.Finished;
                        Close();
                    }
                }
                else if (ManuallyTriggered && (Program.AIRUpdateResults == Program.UpdateResult.UpToDate || Program.AIRUpdateResults == Program.UpdateResult.ValueNull))
                {
                    ManuallyTriggered = false;
                    updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_UpToDate");
                    if (ShowDialog() == true)
                    {
                        DownloadUpdate();
                    }
                    else
                    {
                        Program.AIRUpdaterState = Program.UpdateState.Finished;
                        Close();
                    }
                }
                else
                {
                    Program.AIRUpdaterState = Program.UpdateState.Finished;
                    Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                string version = "";

                if (File.Exists(metaDataFile))
                {
                    FileInfo fileInfo = new FileInfo(metaDataFile);
                    AIR_API.VersionMetadata ver = new AIR_API.VersionMetadata(fileInfo);
                    version = ver.VersionString;
                }
                else
                {
                    string exe = Directory.GetFiles(output, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                    var versInfo = FileVersionInfo.GetVersionInfo(exe);
                    string fileVersionFull = $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}.{versInfo.FilePrivatePart}";
                    if (Version.TryParse(fileVersionFull, out Version result))
                    {
                        version = result.ToString();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }


                //TODO : Add Proper Download Cache for Failed Install of Downloads

                string output2 = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Sonic3AIR_MM\\air_versions\\{version}\\sonic3air_game";

                if (Directory.Exists(output2))
                {
                    // TODO : Add Collision Handling
                    Directory.Delete(output2, true);
                }
                string download_source = Path.Combine(destination, "sonic3air_game");
                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(download_source, output2, true);

                MessageBox.Show($"{Program.LanguageResource.GetString("GameInstalledAt")} \"{output2}\"");

                Program.AIRUpdaterState = Program.UpdateState.Finished;
                try { Management.FileManagement.WipeFolderContents(Management.ProgramPaths.Sonic3AIR_MM_DownloadsFolder); }
                catch (Exception ex) { }
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(Program.LanguageResource.GetString("UpdateFailedError") + Environment.NewLine, ex.Message);
                Program.AIRUpdaterState = Program.UpdateState.Finished;
                Close();
            }




        }

        #endregion

        #region Version Comparision

        private Version GetLatestVersion(FileInfo settingsFile)
        {
            var SettingsFile = new AIR_API.Settings(settingsFile);
            Management.VersionManagement.RefreshVersionsList();
            var LatestVersionStored = Management.VersionManagement.InstalledVersions.Select(s => s.Version).Max();
            if (LatestVersionStored != null && LatestVersionStored.CompareTo(SettingsFile.Version) > 0) return LatestVersionStored;
            else return SettingsFile.Version;
        }
        private int CheckFromSelectedVersion(Version comparision)
        {

            string path = (Management.ProgramPaths.Sonic3AIRPath != null && Management.ProgramPaths.Sonic3AIRPath != "" ? Path.GetDirectoryName(Management.ProgramPaths.Sonic3AIRPath) : "");
            if (path == "" || !Directory.Exists(path)) return 0;
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
                else
                {
                    string exe = Directory.GetFiles(path, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                    var versInfo = FileVersionInfo.GetVersionInfo(exe);
                    string fileVersionFull = $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}.{versInfo.FilePrivatePart}";
                    if (Version.TryParse(fileVersionFull, out Version result))
                    {
                        return result.CompareTo(comparision);
                    }
                    else
                    {
                        return 0;
                    }
                }


            }

        }

        #endregion
    }
}
