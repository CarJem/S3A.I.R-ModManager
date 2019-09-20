using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonic3AIR_ModLoader
{
    public class UserLanguage
    {
        private static string nL = Environment.NewLine;

        #region Version/Section Strings

        public static string AIRVersion = "A.I.R. Version";
        public static string Version = "Version";
        public static string By = "By";
        public static string ModManagerVersion = "Mod Manager Version";

        #endregion

        #region Filters / Open File Dialog Text

        public static string Sonic3KRomFile = "Sonic 3K ROM File";
        public static string SelectSonic3KRomFile = "Select Sonic 3K ROM File...";

        public static string SonicAIRVersionZIP = "Sonic 3 A.I.R. Version ZIP";
        public static string SelectSonicAIRVersionZIP = "Select Sonic 3 A.I.R. Version ZIP...";

        #endregion

        #region Mod List Tooltips
        public static string AddAMod = "Add a Mod...";
        public static string RemoveSelectedMod = "Remove Selected Mod...";
        public static string IncreaseModPriority = "Increase Selected Mod Priority...";
        public static string IncreaseModPriorityToMax = "Increase Selected Mod Priority to Max...";
        public static string DecreaseModPriority = "Decrease Selected Mod Priority...";
        public static string DecreaseModPriorityToMin = "Decrease Selected Mod Priority to Min...";
        #endregion

        #region Informing
        public static string RecordingPathCopiedToClipboard = "Recording File Path Copied to Clipboard!";

        public static string RecordingUploaded(string url)
        {
           return @"File Uploaded to File.io, it will expire in 1 week and has a one time use:" + Environment.NewLine + "(URL Has Been Copied to Clipboard): " + Environment.NewLine + url;
        }

        public static string VersionInstalled(string output)
        {
            return $"The game has been installed at \"{output}\"";
        }

        #endregion

        #region Confirmation Dialogs / Titles

        public static string AddNewDeviceTitle = "Add New Device...";
        public static string AddNewDeviceDescription = "Enter the name of the controller as it appears in the system settings or type * to target all devices.";
        public static string DeleteDeviceTitle = "Delete Device";
        public static string EnterModURL = "Enter Mod URL...";

        public static string RemoveInputDevice(string item)
        {
            return $"Verification: Do you want to remove [ {item} ] from the list of acceptable devices for this input?";
        }

        public static string DeleteItem(string item)
        {
            return $"Are you sure you want to delete \"{item}\"?";
        }

        public static string RemoveVersion(string name)
        {
            return $"Are you sure you want to remove Version \"{name}\"?";
        }
        #endregion

        #region Errors
        public static string InputMappingError1 = "Error Loading Input Configuration File!\r\nMake sure your game is up to date and you have A.I.R.\'s Game Path set in Options.\r\n";
        public static string InputMappingError2 = "There was an error reading the config file in this Version's Folder!\r\n Please make sure nothing is wrong with it or do a clean install of this version of A.I.R.";
        public static string InputMappingError3 = "The config file does not exist in the A.I.R. EXE Folder.\r\n Please make sure the file exists or do a clean install of this version of A.I.R.";

        public static string CollectionFilesCouldNotBeFound1 = "The following files could not be found: ";
        public static string CollectionFilesCouldNotBeFound2 = $"{nL}{nL}If you have not run Sonic 3 A.I.R. yet, please run Sonic 3 A.I.R. once before running the modloader!";
        public static string CollectionFilesCouldNotBeFound3 = $"{nL}{nL}If you have, make sure these locations exist. The modloader can't run without them";

        public static string AIRChangePathNoLongerExists = "The file defined settings does not exist anymore. Launch AIR again outside of the mod loader and try again";

        public static string UnableToRemoveVersion = "Unable to Remove Version!";
        public static string UnableToDeleteFile = "Unable to Delete File!";

        public static string PleaseRefreshTheModList = "Please Refresh the Mod List!";

        public static string LogFileNotFound = "Log file not found";

        public static string InvalidURL = "Invalid URL";
        public static string Downloading = "Downloading";
        public static string AIRPathNotSet(string text)
        {
            return $"Unable to Validate Path: {Environment.NewLine}{text}{Environment.NewLine}Reason: {Environment.NewLine}Sonic 3 A.I.R.'s Path is not set!";
        }
        public static string FileDoesNotExist(string text)
        {
            return $"Unable to Validate Path: {Environment.NewLine}{text}{Environment.NewLine}Reason: {Environment.NewLine}Specified file or directory does not exist!";
        }

        public static string LegacyModError1(string folderName, string exMessage)
        {
            return $"Error with loading {folderName}!{Environment.NewLine}(Likely a JSON Error; Make sure the mod.json file is formated correctly!){Environment.NewLine}{exMessage}";
        }

        public static string LegacyModError2(string folderName, string exMessage)
        {
            return $"Error with loading {folderName}!{Environment.NewLine}{exMessage}";
        }



        #endregion

        #region Misc

        public static string Input_MULTI = "[MULTI]";
        public static string Input_NULL = "[NULL]";
        public static string Input_NONE = "[NONE]";
        public static string Input_INPUT = "[INPUT]";
        public static string Input_UNSUPPORTED = "[UNSUPPORTED]";

        public static string Ok_Button = "&OK";
        public static string Cancel_Button = "&Cancel";

        public static string AutoBoot_Initalizing = "Initializing...";
        public static string AutoBoot_CheckingForUpdates = "Checking for Updates...";
        public static string AutoBoot_LaunchingIn = "Launching in";

        #endregion

        public static void ApplyLanguage(ModManager modManager)
        {

        }



    }
}
