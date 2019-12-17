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
        private static ModManager Instance;
        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
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
