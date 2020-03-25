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
        #region Definitions

        private Classes.VersionCheck ModManagerVersionCheckInfo;
        private string VersionCheckFileName = "";
        private string UpdateFileName = "";

        private bool ManuallyTriggered = false;

        private bool DisableUpdater = false;
        private ModManagerUpdater Instance;
        public bool isOnline = false;

        #endregion

        #region Init

        public ModManagerUpdater(bool _manuallyTriggered = false)
        {
            Program.MMUpdaterState = Program.UpdateState.Checking;
            Program.Log.InfoFormat("Checking for Mod Manager Updates...");
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
            if (DisableUpdater == false) CheckForModManagerUpdates();
            else UpdateBypass();
        }
        private void UpdateBypass()
        {
            Program.MMUpdateResults = Program.UpdateResult.UpToDate;
            Program.MMUpdaterState = Program.UpdateState.Finished;
            Close();
        }
        public void CheckForModManagerUpdates()
        {
            try
            {
                isOnline = Management.DownloaderAPI.IsNetworkAvailable();

                if (isOnline)
                {
                    string url = "";
                    url = @"https://raw.githubusercontent.com/CarJem/GenerationsLib.Updates/master/UpdateMetadata/AIR_MM_Updates.json";
                    VersionCheckFileName = Management.DownloaderAPI.DownloadFile(url, Management.ProgramPaths.Sonic3AIR_MM_BaseFolder, DownloadCheckComplete);
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
        private void DownloadUpdate()
        {
            string DownloadURL = ModManagerVersionCheckInfo.DownloadURL;
            string LatestVersion = ModManagerVersionCheckInfo.Version.ToString();

            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
            string DownloadsFolder = BaseFolder + "\\downloads";

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
            string destination = Management.ProgramPaths.Sonic3AIR_MM_BaseFolder;
            string path = $"{destination}//{VersionCheckFileName}";
            var file = new FileInfo(path);
            ModManagerVersionCheckInfo = new Classes.VersionCheck(file);

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

        #endregion
    }
}
