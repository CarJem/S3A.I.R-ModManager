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
    public class ProgramPaths
    {
        private static string nL = Environment.NewLine;

        #region File Path Strings

        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Sonic3AIRAppDataFolder = "";
        public static string Sonic3AIRModsFolder = "";
        public static string Sonic3AIRActiveModsList = "";
        public static string Sonic3AIRSettingsFile = "";
        public static string Sonic3AIRGBLinkPath = "";
        public static string Sonic3AIR_MM_BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
        public static string Sonic3AIR_MM_GBRequestsFolder = Sonic3AIR_MM_BaseFolder + "\\gb_api_urls";
        public static string Sonic3AIR_MM_TempModsFolder = Sonic3AIR_MM_BaseFolder + "\\temp_mod_install";
        public static string Sonic3AIR_MM_DownloadsFolder = Sonic3AIR_MM_BaseFolder + "\\downloads";
        public static string Sonic3AIR_MM_VersionsFolder = Sonic3AIR_MM_BaseFolder + "\\air_versions";

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


        public static string CustomGameRecordingsFolderPath { get; set; } = GetCustomGameRecordingsFolderPathUnsetString();

        public static string GetCustomGameRecordingsFolderPathUnsetString()
        {
            return UserLanguage.GetOutputString("UnsetFolderPathString");
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
                    return Directory.Exists(Path.Combine(Sonic3AIRAppDataFolder, "recordings"));
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
                    return Path.Combine(Sonic3AIRAppDataFolder, "recordings");
                case GameRecordingSearchLocation.S3AIR_Custom:
                    return CustomGameRecordingsFolderPath;
                default:
                    return GetDefaultGameRecordingsFolderPath();
            }
        }

        private static bool DoesDefaultGameRecordingsFolderPathExist()
        {
            if (ModManager.S3AIRSettings.RawSettings is AIR_API.AIRSettingsMK2)
            {
                //TODO - Implement this When the Version that adds the Game Recordings Folder Comes Out
                if (ModManager.S3AIRSettings.Version >= new Version("19.12.7.0"))
                {
                    //return Directory.Exists(Path.Combine(Sonic3AIRAppDataFolder, "recordings"));
                    return Directory.Exists(Sonic3AIRAppDataFolder);
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
            if (ModManager.S3AIRSettings.RawSettings is AIR_API.AIRSettingsMK2)
            {
                //TODO - Implement this When the Version that adds the Game Recordings Folder Comes Out
                if (ModManager.S3AIRSettings.Version >= new Version("19.12.7.0"))
                {
                    //return Path.Combine(Sonic3AIRAppDataFolder, "recordings");
                    return Sonic3AIRAppDataFolder;
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
            Sonic3AIRAppDataFolder = AppDataFolder + "\\Sonic3AIR";
            Sonic3AIRActiveModsList = Sonic3AIRAppDataFolder + "\\mods\\active-mods.json";
            Sonic3AIRModsFolder = Sonic3AIRAppDataFolder + "\\mods";
            Sonic3AIRSettingsFile = Sonic3AIRAppDataFolder + "\\settings.json";

            CreateMissingModManagerFolders();

            List<Tuple<string, bool>> MissingFilesState = new List<Tuple<string, bool>>();

            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRAppDataFolder", Directory.Exists(Sonic3AIRAppDataFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRModsFolder", Directory.Exists(Sonic3AIRModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRTempModsFolder", Directory.Exists(Sonic3AIR_MM_TempModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRSettingsFile", File.Exists(Sonic3AIRSettingsFile)));

            if (MissingFilesState.Exists(x => x.Item2.Equals(false)))
            {
                List<Tuple<string, bool>> MissingList = MissingFilesState.Where(x => x.Item2.Equals(false)).ToList();
                string missingItems = Program.LanguageResource.GetString("CollectionFilesCouldNotBeFound1");
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRAppDataFolder"))) missingItems += $"{nL}- {Sonic3AIRAppDataFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRModsFolder"))) missingItems += $"{nL}- {Sonic3AIRModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRTempModsFolder"))) missingItems += $"{nL}- {Sonic3AIR_MM_TempModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRSettingsFile"))) missingItems += $"{nL}- {Sonic3AIRSettingsFile}";
                missingItems += Program.LanguageResource.GetString("CollectionFilesCouldNotBeFound2");
                missingItems += Program.LanguageResource.GetString("CollectionFilesCouldNotBeFound3");
                MessageBox.Show(missingItems);
                return false;
            }
            else
            {
                return ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref S3AIRSettings, true);
            }
        }

        public static void CreateMissingModManagerFolders()
        {
            if (!Directory.Exists(Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_TempModsFolder);
            if (!Directory.Exists(Sonic3AIR_MM_BaseFolder)) Directory.CreateDirectory(Sonic3AIR_MM_BaseFolder);
            if (!Directory.Exists(Sonic3AIR_MM_DownloadsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_DownloadsFolder);
            if (!Directory.Exists(Sonic3AIR_MM_VersionsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_VersionsFolder);
            if (!Directory.Exists(Sonic3AIR_MM_GBRequestsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_GBRequestsFolder);
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
            try
            {
                S3AIRSettings = new AIR_API.Settings(file);
                Version target = new Version("19.08.17.0");
                int result;
                if (S3AIRSettings.Version != null) result = S3AIRSettings.Version.CompareTo(target);
                else result = -1;
                if (result < 0)
                {
                    if (throwVersionMismatchError) MessageBox.Show(Program.LanguageResource.GetString("StartupFailureError"));
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
