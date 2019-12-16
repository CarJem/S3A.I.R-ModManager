using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using GenerationsLib.Core;

namespace Sonic3AIR_ModManager
{
    public static class ProgramPaths
    {
        private static string nL = Environment.NewLine;

        #region File Path Strings

        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Sonic3AIRAppDataFolder { get => AppDataFolder + "\\Sonic3AIR"; }
        public static string Sonic3AIRModsFolder { get => Sonic3AIRAppDataFolder + "\\mods"; }
        public static string Sonic3AIRGameRecordingsFolder { get => Sonic3AIRAppDataFolder + "\\gamerecordings"; }
        public static string Sonic3AIRActiveModsList { get => Sonic3AIRAppDataFolder + "\\mods\\active-mods.json"; }
        public static string Sonic3AIRSettingsFile { get => Sonic3AIRAppDataFolder + "\\settings.json"; }
        public static string Sonic3AIRConfigFile { get => Sonic3AIRAppDataFolder + "\\config.json"; }
        public static string Sonic3AIRLogFile { get => Sonic3AIRAppDataFolder + "\\logfile.txt"; }
        public static string Sonic3AIRGlobalSettingsFile { get => Sonic3AIRAppDataFolder + "\\settings_global.json"; }
        public static string Sonic3AIRGlobalInputFile { get => Sonic3AIRAppDataFolder + "\\settings_input.json"; }

        public static string Sonic3AIR_MM_BaseFolder { get => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM"; }
        public static string Sonic3AIR_MM_GBRequestsFolder { get => Sonic3AIR_MM_BaseFolder + "\\gb_api_urls"; }
        public static string Sonic3AIR_MM_TempModsFolder  { get => Sonic3AIR_MM_BaseFolder + "\\temp_mod_install"; }
        public static string Sonic3AIR_MM_DownloadsFolder { get => Sonic3AIR_MM_BaseFolder + "\\downloads"; }
        public static string Sonic3AIR_MM_VersionsFolder { get => Sonic3AIR_MM_BaseFolder + "\\air_versions"; }
        public static string Sonic3AIR_MM_LogsFolder { get => Sonic3AIR_MM_BaseFolder + "\\logs"; }
        public static string Sonic3AIR_MM_SettingsFile { get => Sonic3AIR_MM_BaseFolder + "\\settings.json"; }

        #endregion

        #region Sonic 3 A.I.R. Path
    public static string Sonic3AIRPath { get => GetSonic3AIRPath(); set => SetSonic3AIRPath(value); }
        public static string GetSonic3AIRPath()
        {
            return Properties.Settings.Default.Sonic3AIRPath;

        }
        public static void SetSonic3AIRPath(string value)
        {
            Properties.Settings.Default.Sonic3AIRPath = value;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region AppData Path Strings
        public static string Sonic3AIRSampleModsFolder { get => GetSonic3AIRSampleModsFolderPath(); }
        public static string Sonic3AIRModDocumentationFile { get => GetSonic3AIRModDocumentationFilePath(); }
        public static string Sonic3AIRUserManualFile { get => GetSonic3AIRUserManualFilePath(); }
        public static string Sonic3AIRModdingTemplatesFolder { get => GetSonic3AIRModdingTemplatesFolderPath(); }

        #endregion

        #region File Path Checkers

        private static string GetSonic3AIRModdingTemplatesFolderPath()
        {
            try
            {
                string filename = Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\modding";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\modding";
            }

        }
        private static string GetSonic3AIRSampleModsFolderPath()
        {
            try
            {
                string filename = Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\sample-mods";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\sample-mods";
            }

        }
        private static string GetSonic3AIRModDocumentationFilePath()
        {
            try
            {
                string filename = Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\Modding.pdf";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\Modding.pdf";
            }
        }
        private static string GetSonic3AIRUserManualFilePath()
        {
            try
            {
                string filename = Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\Manual.pdf";
            }
            catch
            {
                return "%S3AIRPath%" + "\\Manual.pdf";
            }

        }
        #endregion

        #region File Path Validators

        public static bool ValidateSonic3AIRModdingTemplatesFolderPath()
        {
            if (!Sonic3AIRModdingTemplatesFolder.Contains("%S3AIRPath%"))
            {
                if (Directory.Exists(Sonic3AIRModdingTemplatesFolder)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRModdingTemplatesFolder);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRModdingTemplatesFolder);
                return false;
            }

        }

        public static bool ValidateSonic3AIRSampleModsFolderPath()
        {
            if (!Sonic3AIRSampleModsFolder.Contains("%S3AIRPath%"))
            {
                if (Directory.Exists(Sonic3AIRSampleModsFolder)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRSampleModsFolder);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRSampleModsFolder);
                return false;
            }

        }

        public static bool ValidateSonic3AIRModDocumentationFilePath()
        {
            if (!Sonic3AIRModDocumentationFile.Contains("%S3AIRPath%"))
            {
                if (File.Exists(Sonic3AIRModDocumentationFile)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRModDocumentationFile);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRModDocumentationFile);
                return false;
            }
        }

        public static bool ValidateSonic3AIRUserManualFilePath()
        {
            if (!Sonic3AIRUserManualFile.Contains("%S3AIRPath%"))
            {
                if (File.Exists(Sonic3AIRUserManualFile)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRUserManualFile);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRUserManualFile);
                return false;
            }
        }

        #endregion

        #region File Path Invalid Messages

        private static void AIRPathNotSetMessageBox(string text)
        {
            string message = UserLanguage.AIRPathNotSet(text);
            System.Windows.Forms.MessageBox.Show(message);
        }

        private static void FileDoesNotExistMessageBox(string text)
        {
            string message = UserLanguage.FileDoesNotExist(text);
            System.Windows.Forms.MessageBox.Show(message);
        }

        #endregion

        #region Recordings Path Validation


        public static string CustomGameRecordingsFolderPath { get; set; } = "UNSET_FOLDER_PATH";

        public static string GetCustomGameRecordingsFolderPathUnsetString()
        {
            if (UserLanguage.GetOutputString("UnsetFolderPathString") == null)
            {
                return "UNSET_FOLDER_PATH";
            }
            else return UserLanguage.GetOutputString("UnsetFolderPathString");
        }

        public static GameRecordingSearchLocation GameRecordingsFolderDesiredPath { get; set; } = GameRecordingSearchLocation.S3AIR_Default;

        public enum GameRecordingSearchLocation : int
        {
            S3AIR_Default = 0,
            S3AIR_AppData = 1,
            S3AIR_EXE_Folder = 2,
            S3AIR_RecordingsFolder = 3,
            S3AIR_Custom = 4

        }

        public static void SetCustomGameRecordingsFolderPath()
        {
            FolderSelectDialog fsd = new FolderSelectDialog()
            {
                Title = UserLanguage.GetOutputString("SetCustomGameRecordingFolderTitle")
            };
            if (fsd.ShowDialog() == true)
            {
                CustomGameRecordingsFolderPath = fsd.FileName;
            }
            else
            {
                CustomGameRecordingsFolderPath = GetCustomGameRecordingsFolderPathUnsetString();
            }
        }

        public static bool DoesSonic3AIRGameRecordingsFolderPathExist()
        {
            switch (GameRecordingsFolderDesiredPath)
            {
                case GameRecordingSearchLocation.S3AIR_Default:
                    return DoesDefaultGameRecordingsFolderPathExist();
                case GameRecordingSearchLocation.S3AIR_AppData:
                    return Directory.Exists(Sonic3AIRAppDataFolder);
                case GameRecordingSearchLocation.S3AIR_EXE_Folder:
                    return Directory.Exists(Path.GetDirectoryName(Sonic3AIRPath));
                case GameRecordingSearchLocation.S3AIR_RecordingsFolder:
                    return Directory.Exists(Path.Combine(Sonic3AIRAppDataFolder, "gamerecordings"));
                case GameRecordingSearchLocation.S3AIR_Custom:
                    return Directory.Exists(CustomGameRecordingsFolderPath);
                default:
                    return DoesDefaultGameRecordingsFolderPathExist();
            }
        }

        public static string GetSonic3AIRGameRecordingsFolderPath()
        {
            switch (GameRecordingsFolderDesiredPath)
            {
                case GameRecordingSearchLocation.S3AIR_Default:
                    return GetDefaultGameRecordingsFolderPath();
                case GameRecordingSearchLocation.S3AIR_AppData:
                    return Sonic3AIRAppDataFolder;
                case GameRecordingSearchLocation.S3AIR_EXE_Folder:
                    return Path.GetDirectoryName(Sonic3AIRPath);
                case GameRecordingSearchLocation.S3AIR_RecordingsFolder:
                    return Path.Combine(Sonic3AIRAppDataFolder, "gamerecordings");
                case GameRecordingSearchLocation.S3AIR_Custom:
                    return CustomGameRecordingsFolderPath;
                default:
                    return GetDefaultGameRecordingsFolderPath();
            }
        }

        private static bool DoesDefaultGameRecordingsFolderPathExist()
        {
            if (MainDataModel.S3AIRSettings != null && MainDataModel.S3AIRSettings.Version != null)
            {
                if (MainDataModel.S3AIRSettings.Version >= new Version("19.12.7.0"))
                {
                    return Directory.Exists(Path.Combine(Sonic3AIRAppDataFolder, "gamerecordings"));
                }
                else
                {
                    return Directory.Exists(Sonic3AIRAppDataFolder);
                }

            }
            else
            {
                return File.Exists(ProgramPaths.Sonic3AIRPath);
            }
        }


        private static string GetDefaultGameRecordingsFolderPath()
        {
            if (MainDataModel.S3AIRSettings != null && MainDataModel.S3AIRSettings.Version != null)
            {
                if (MainDataModel.S3AIRSettings.Version >= new Version("19.12.7.0"))
                {
                    return Path.Combine(Sonic3AIRAppDataFolder, "gamerecordings");
                }
                else
                {
                    return Sonic3AIRAppDataFolder;
                }

            }
            else
            {
                return Path.GetDirectoryName(Sonic3AIRPath);
            }
        }


        #endregion

        public static bool ValidateInstall(ref AIR_API.ActiveModsList S3AIRActiveMods, ref AIR_API.Settings S3AIRSettings)
        {
            CreateMissingModManagerFolders();

            if (!Directory.Exists(Sonic3AIRModsFolder)) Directory.CreateDirectory(Sonic3AIRModsFolder);
            if (!Directory.Exists(Sonic3AIRAppDataFolder)) Directory.CreateDirectory(Sonic3AIRAppDataFolder);
            if (!File.Exists(Sonic3AIRActiveModsList)) File.Create(Sonic3AIRActiveModsList);
            if (!File.Exists(Sonic3AIRSettingsFile)) File.Create(Sonic3AIRSettingsFile);



            return ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref S3AIRSettings, false);
        }

        public static void CreateMissingModManagerFolders()
        {
            try
            {
                if (!Directory.Exists(Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_TempModsFolder);
                if (!Directory.Exists(Sonic3AIR_MM_BaseFolder)) Directory.CreateDirectory(Sonic3AIR_MM_BaseFolder);
                if (!Directory.Exists(Sonic3AIR_MM_DownloadsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_DownloadsFolder);
                if (!Directory.Exists(Sonic3AIR_MM_VersionsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_VersionsFolder);
                if (!Directory.Exists(Sonic3AIR_MM_GBRequestsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_GBRequestsFolder);
                if (!Directory.Exists(Sonic3AIR_MM_LogsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_LogsFolder);
            }
            catch
            {

            }

        }

        public static bool ValidateSettingsAndActiveMods(ref AIR_API.ActiveModsList S3AIRActiveMods, ref AIR_API.Settings S3AIRSettings, bool throwVersionMismatchError = false)
        {
            if (!File.Exists(Sonic3AIRActiveModsList))
            {
                S3AIRActiveMods = new AIR_API.ActiveModsList(Sonic3AIRActiveModsList);
            }
            else
            {
                FileInfo list = new FileInfo(Sonic3AIRActiveModsList);
                S3AIRActiveMods = new AIR_API.ActiveModsList(list);
            }



            FileInfo file = new FileInfo(Sonic3AIRSettingsFile);
            S3AIRSettings = new AIR_API.Settings(file);
            return true;
        }
    }
}
