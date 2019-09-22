using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Sonic3AIR_ModLoader
{
    public class ProgramPaths
    {
        private static string nL = Environment.NewLine;

        #region File Path Strings

        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Sonic3AIRAppDataFolder = "";
        public static string Sonic3AIRModsFolder = "";
        public static string Sonic3AIRActiveModsList = "";
        public static string Sonic3AIR_MM_TempModsFolder = "";
        public static string Sonic3AIRSettingsFile = "";
        public static string Sonic3AIRGBLinkPath = "";
        public static string Sonic3AIR_MM_BaseFolder = "";
        public static string Sonic3AIR_MM_DownloadsFolder = "";
        public static string Sonic3AIR_MM_VersionsFolder = "";

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

        #endregion

        public static bool ValidateInstall(ref AIR_SDK.ActiveModsList S3AIRActiveMods, ref AIR_SDK.Settings S3AIRSettings)
        {
            Sonic3AIRAppDataFolder = AppDataFolder + "\\Sonic3AIR";
            Sonic3AIRActiveModsList = Sonic3AIRAppDataFolder + "\\mods\\active-mods.json";
            Sonic3AIRModsFolder = Sonic3AIRAppDataFolder + "\\mods";
            Sonic3AIRSettingsFile = Sonic3AIRAppDataFolder + "\\settings.json";

            Sonic3AIR_MM_BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM";
            Sonic3AIR_MM_TempModsFolder = Sonic3AIR_MM_BaseFolder + "\\temp_mod_install";
            Sonic3AIR_MM_DownloadsFolder = Sonic3AIR_MM_BaseFolder + "\\downloads";
            Sonic3AIR_MM_VersionsFolder = Sonic3AIR_MM_BaseFolder + "\\air_versions";


            if (!Directory.Exists(Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_TempModsFolder);
            if (!Directory.Exists(Sonic3AIR_MM_BaseFolder)) Directory.CreateDirectory(Sonic3AIR_MM_BaseFolder);
            if (!Directory.Exists(Sonic3AIR_MM_DownloadsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_DownloadsFolder);
            if (!Directory.Exists(Sonic3AIR_MM_VersionsFolder)) Directory.CreateDirectory(Sonic3AIR_MM_VersionsFolder);

            List<Tuple<string, bool>> MissingFilesState = new List<Tuple<string, bool>>();

            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRAppDataFolder", Directory.Exists(Sonic3AIRAppDataFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRModsFolder", Directory.Exists(Sonic3AIRModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRTempModsFolder", Directory.Exists(Sonic3AIR_MM_TempModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRSettingsFile", File.Exists(Sonic3AIRSettingsFile)));

            if (MissingFilesState.Exists(x => x.Item2.Equals(false)))
            {
                List<Tuple<string, bool>> MissingList = MissingFilesState.Where(x => x.Item2.Equals(false)).ToList();
                string missingItems = UserLanguage.CollectionFilesCouldNotBeFound1;
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRAppDataFolder"))) missingItems += $"{nL}- {Sonic3AIRAppDataFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRModsFolder"))) missingItems += $"{nL}- {Sonic3AIRModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRTempModsFolder"))) missingItems += $"{nL}- {Sonic3AIR_MM_TempModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRSettingsFile"))) missingItems += $"{nL}- {Sonic3AIRSettingsFile}";
                missingItems += UserLanguage.CollectionFilesCouldNotBeFound2;
                missingItems += UserLanguage.CollectionFilesCouldNotBeFound3;
                MessageBox.Show(missingItems);
                return false;
            }
            else
            {
                return ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref S3AIRSettings);
            }
        }


        public static bool ValidateSettingsAndActiveMods(ref AIR_SDK.ActiveModsList S3AIRActiveMods, ref AIR_SDK.Settings S3AIRSettings)
        {
            if (!File.Exists(Sonic3AIRActiveModsList))
            {
                S3AIRActiveMods = new AIR_SDK.ActiveModsList(Sonic3AIRActiveModsList);
            }
            else
            {
                FileInfo list = new FileInfo(Sonic3AIRActiveModsList);
                S3AIRActiveMods = new AIR_SDK.ActiveModsList(list);
            }



            FileInfo file = new FileInfo(Sonic3AIRSettingsFile);
            try
            {
                S3AIRSettings = new AIR_SDK.Settings(file);
                Version target = new Version("19.08.17.0");
                int result = S3AIRSettings.Version.CompareTo(target);
                if (result < 0)
                {
                    MessageBox.Show($"Sonic 3 A.I.R is out of date, please use version 19.08.17.0 or above! (and start it at least once fully)");
                    return false;
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
