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
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace Sonic3AIR_ModManager
{
    public static class ProcessLauncher
    {
        public static bool isGameRunning = false;
        public static Process CurrentGameProcess;
        private static ModManager Instance;
        public static void ForceQuitSonic3AIR()
        {
            // TODO: Add Warning Dialog
            if (CurrentGameProcess != null && !CurrentGameProcess.HasExited) CurrentGameProcess.Kill();
        }
        public static bool UpdateSonic3AIRLocation(bool intended = false)
        {
            if (intended)
            {
                return LocationDialog();
            }
            else
            {
                DialogResult result = MessageBox.Show(Program.LanguageResource.GetString("MissingAIRSetNowAlert"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    return LocationDialog();
                }
                else return false;
            }



            bool LocationDialog()
            {
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = $"{Program.LanguageResource.GetString("EXEFileDialogFilter")} (*.exe)|*.exe",
                    Title = Program.LanguageResource.GetString("EXEFileDialogTitle")
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    ProgramPaths.Sonic3AIRPath = fileDialog.FileName;
                    return true;
                }
                else return false;
            }

        }

        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
        }


        #region Sonic 3 A.I.R. Launcher

        public static void LaunchSonic3AIR()
        {
            bool IsGamePathSet = true;
            if (ProgramPaths.Sonic3AIRPath == null || ProgramPaths.Sonic3AIRPath == "")
            {
                IsGamePathSet = UpdateSonic3AIRLocation();
            }
            if (IsGamePathSet)
            {
                System.Threading.Thread thread = new System.Threading.Thread(ProcessLauncher.RunSonic3AIR);
                thread.Start();
            }
            else
            {
                MessageBox.Show(Program.LanguageResource.GetString("AIRCanNotStartNoPath"));
            }
        }

        public static void RunSonic3AIR()
        {
            try
            {
                GameStartHandler();
                string filename = ProgramPaths.Sonic3AIRPath;
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                CurrentGameProcess = Process.Start(start);
                CurrentGameProcess.WaitForExit();
                GameEndHandler();
            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIR") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        public static void GameStartHandler()
        {
            isGameRunning = true;
            if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Instance.Hide();
                }));

            }
            else
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainDataModel.UpdateInGameButtons(ref Instance);
                }));
            }

        }

        public static void GameEndHandler()
        {
            isGameRunning = false;
            if (!Properties.Settings.Default.KeepOpenOnQuit) Environment.Exit(0);
            else if (!Properties.Settings.Default.KeepOpenOnLaunch)
            {
                Instance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Instance.Show();
                }));
            }
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
                string filePath = MainDataModel.S3AIRSettings.FilePath;
                MainDataModel.S3AIRSettings = new AIR_API.Settings(new FileInfo(filePath));
                MainDataModel.UpdateAIRSettings(ref Instance);
            }));

        }

        #endregion

        #region Game Recording Player

        private static AIR_API.Settings GameRecordingSettings { get; set; }
        private static AIR_API.GameConfig GameRecordingCurrentGameConfig { get; set; }
        private static List<string> Temporary_Settings { get; set; }
        private static List<string> CurrentSettings { get; set; }

        public static void LaunchGameRecording(string file, string viewer_exe)
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(() => ProcessLauncher.RunGameRecordingViewer(file, viewer_exe));
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void RunGameRecordingViewer(string file, string viewer_exe)
        {
            try
            {
                string filename = viewer_exe;
                RecordingStartHandler(file, viewer_exe);
                var start = new ProcessStartInfo() { FileName = filename, WorkingDirectory = Path.GetDirectoryName(filename) };
                CurrentGameProcess = Process.Start(start);
                CurrentGameProcess.WaitForExit();
                RecordingEndHandler(viewer_exe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(UserLanguage.GetOutputString("UnableToStartS3AIRRecordingViewer") + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.Message);
            }

        }

        public static void RecordingStartHandler(string file, string viewer_exe)
        {
            isGameRunning = true;
            BackupSettings(file, viewer_exe);

            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
            }));
        }

        public static void RecordingEndHandler(string viewer_exe)
        {
            RestoreSettings(viewer_exe);
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateAIRSettings(ref Instance);
                MainDataModel.RetriveLaunchOptions(ref Instance);
            }));
            isGameRunning = false;
            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                MainDataModel.UpdateInGameButtons(ref Instance);
            }));
        }

        private static void BackupSettings(string file, string viewer_exe)
        {
            string config_file = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.json");
            string config_file_bak = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.bak.json");

            if (File.Exists(config_file_bak)) File.Delete(config_file_bak);
            File.Copy(config_file, config_file_bak);

            string setting_file = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.json");
            string setting_file_bak = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.bak.json");

            if (File.Exists(setting_file_bak)) File.Delete(setting_file_bak);
            File.Copy(setting_file, setting_file_bak);

            GameRecordingCurrentGameConfig = new AIR_API.GameConfig(new FileInfo(config_file));
            GameRecordingSettings = new AIR_API.Settings(new FileInfo(setting_file));

            RecordingManagement.CopyRecordingToDestination(file, Path.GetDirectoryName(viewer_exe));

            Instance.Dispatcher.BeginInvoke((Action)(() =>
            {
                GameRecordingSettings.FullscreenMode = (int)AIR_API.Settings.FullscreenType.Windowed;
                GameRecordingCurrentGameConfig.StartPhase = 3;
                GameRecordingCurrentGameConfig.GameRecording = 2;

                GameRecordingSettings.Save();
                GameRecordingCurrentGameConfig.Save();
            }));
        }

        private static void RestoreSettings(string viewer_exe)
        {
            string config_file = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.json");
            string config_file_bak = Path.Combine(Path.GetDirectoryName(viewer_exe), "config.bak.json");

            File.Delete(config_file);
            File.Copy(config_file_bak, config_file);
            File.Delete(config_file_bak);

            string setting_file = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.json");
            string setting_file_bak = Path.Combine(ProgramPaths.Sonic3AIRAppDataFolder, "settings.bak.json");

            File.Delete(setting_file);
            File.Copy(setting_file_bak, setting_file);
            File.Delete(setting_file_bak);

            RecordingManagement.DeletePlaybackRecording(Path.GetDirectoryName(viewer_exe));
        }

        #endregion

        #region A.I.R. App Launcher

        public static void OpenEXEFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                string filename = ProgramPaths.Sonic3AIRPath;
                Process.Start(Path.GetDirectoryName(filename));
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    string filename = ProgramPaths.Sonic3AIRPath;
                    Process.Start(Path.GetDirectoryName(filename));
                }
            }
        }

        public static void OpenAppDataFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIRAppDataFolder);
        }

        public static void OpenModsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIRModsFolder);
        }

        public static void OpenSelectedModFolder(ModViewerItem mod)
        {
            Process.Start(mod.Source.FolderPath);
        }

        public static void Open1ClickInstaller()
        {
            string ModLoaderPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string InstallerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "//GameBanana API Installer.exe";
            Process.Start($"\"{InstallerPath}\"", $"\"{ModLoaderPath}\"");
        }

        public static void OpenSettingsFile()
        {
            if (File.Exists(ProgramPaths.Sonic3AIRSettingsFile))
            {
                Process.Start(ProgramPaths.Sonic3AIRSettingsFile);
            }
            else
            {
                //TODO : Add Warning Messages
            }

        }

        public static void OpenConfigFile()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (File.Exists(ProgramPaths.Sonic3AIRConfigFile))
                {
                    Process.Start(ProgramPaths.Sonic3AIRConfigFile);
                }
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    if (File.Exists(ProgramPaths.Sonic3AIRConfigFile))
                    {
                        Process.Start(ProgramPaths.Sonic3AIRConfigFile);
                    }
                }
            }
        }

        public static void OpenLogFile()
        {
            if (File.Exists(ProgramPaths.Sonic3AIRLogFile))
            {
                Process.Start(ProgramPaths.Sonic3AIRLogFile);
            }
            else
            {
                MessageBox.Show($"{Program.LanguageResource.GetString("LogFileNotFound")}: {MainDataModel.nL}{ProgramPaths.Sonic3AIRLogFile}");
            }

        }

        public static void OpenModdingTemplatesFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(ProgramPaths.Sonic3AIRModdingTemplatesFolder);
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    if (ProgramPaths.ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(ProgramPaths.Sonic3AIRModdingTemplatesFolder);
                }
            }
        }

        public static void OpenSampleModsFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRSampleModsFolderPath()) Process.Start(ProgramPaths.Sonic3AIRSampleModsFolder);
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    if (ProgramPaths.ValidateSonic3AIRSampleModsFolderPath()) Process.Start(ProgramPaths.Sonic3AIRSampleModsFolder);
                }
            }
        }

        public static void OpenUserManual()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRUserManualFilePath()) OpenPDFViewer(ProgramPaths.Sonic3AIRUserManualFile);
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    if (ProgramPaths.ValidateSonic3AIRUserManualFilePath()) OpenPDFViewer(ProgramPaths.Sonic3AIRUserManualFile);
                }
            }
        }

        public static void OpenModDocumentation()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRModDocumentationFilePath()) OpenPDFViewer(ProgramPaths.Sonic3AIRModDocumentationFile);
            }
            else
            {
                if (ProcessLauncher.UpdateSonic3AIRLocation())
                {
                    MainDataModel.UpdateAIRSettings(ref Instance);
                    if (ProgramPaths.ValidateSonic3AIRModDocumentationFilePath()) OpenPDFViewer(ProgramPaths.Sonic3AIRModDocumentationFile);
                }
            }
        }

        public static void OpenPDFViewer(string file)
        {
            DocumentationViewer viewer = new DocumentationViewer();
            viewer.ShowDialog(file);
        }

        public static void OpenModURL(string url)
        {
            try
            {
                if (url != "")
                {
                    Process.Start(url);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void OpenGameRecordingsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIRGameRecordingsFolder);
        }

        public static void OpenGlobalSettingsFile()
        {
            Process.Start(ProgramPaths.Sonic3AIRGlobalSettingsFile);
        }

        public static void OpenInputSettingsFile()
        {
            Process.Start(ProgramPaths.Sonic3AIRGlobalInputFile);
        }

        public static void OpenRecordingLocation()
        {
            if (Instance.GameRecordingList.SelectedItem != null)
            {
                AIR_API.Recording item = Instance.GameRecordingList.SelectedItem as AIR_API.Recording;
                if (File.Exists(item.FilePath))
                {
                    Process.Start("explorer.exe", "/select, " + item.FilePath);
                }
            }

        }

        public static void OpenMMDownloadsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIR_MM_DownloadsFolder);
        }

        public static void OpenMMVersionsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
        }

        public static void OpenMMLogsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIR_MM_LogsFolder);
        }

        public static void OpenMMSettingsFile()
        {
            Process.Start(ProgramPaths.Sonic3AIRSettingsFile);
        }

        #endregion

        #region Other Launchers

        public static void LaunchAboutWindow(ref ModManager owner)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = owner;
            about.ShowDialog();
        }

        public enum S3AIRMedia : int
        {
            CarJemTwitter = 0,
            EukaTwitter = 1,
            S3AIRWebsite = 2,
            Gamebanna = 3
        }

        public static void LaunchAIRMediaLink(S3AIRMedia linkType)
        {
            switch (linkType) {
                case S3AIRMedia.CarJemTwitter:
                    Process.Start("https://twitter.com/carter5467_99");
                    break;
                case S3AIRMedia.EukaTwitter:
                    Process.Start("https://twitter.com/eukaryot3k");
                    break;
                case S3AIRMedia.S3AIRWebsite:
                    Process.Start("http://sonic3air.org/");
                    break;
                case S3AIRMedia.Gamebanna:
                    Process.Start("https://gamebanana.com/games/6878");
                    break;
            }
        }

        #endregion





    }
}
