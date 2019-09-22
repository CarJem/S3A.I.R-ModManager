using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sonic3AIR_ModLoader
{
    public class UserLanguage
    {
        private static string nL = Environment.NewLine;

        #region Informing

        public static string RecordingUploaded(string url)
        {
           return (Program.CurrentLanguage.GetString("RecordingUploaded1") == null ? "" : Program.CurrentLanguage.GetString("RecordingUploaded1")) + Environment.NewLine + (Program.CurrentLanguage.GetString("RecordingUploaded2") == null ? "" : Program.CurrentLanguage.GetString("RecordingUploaded2")) + Environment.NewLine + url;
        }

        public static string VersionInstalled(string output)
        {
            return $"{(Program.CurrentLanguage.GetString("GameInstalledAt") == null ? "" : Program.CurrentLanguage.GetString("GameInstalledAt"))} \"{output}\"";
        }

        #endregion

        #region Confirmation Dialogs / Titles

        public static string GetOutputString(string destintation)
        {
            return (Program.CurrentLanguage.GetString(destintation) == null ? "" : Program.CurrentLanguage.GetString(destintation));
        }

        public static string RemoveInputDevice(string item)
        {
            return $"{GetOutputString("RemoveInputDeviceConfirmation1")} [ {item} ] {GetOutputString("RemoveInputDeviceConfirmation2")}";
        }

        public static string DeleteItem(string item)
        {
            return $"{GetOutputString("DeleteItemConfirmation")} \"{item}\"?";
        }

        public static string RemoveVersion(string name)
        {
            return $"{GetOutputString("RemoveVersionConfirmation")} \"{name}\"?";
        }
        #endregion

        #region Errors
        public static string AIRPathNotSet(string text)
        {
            return $"{GetOutputString("UnableToValididatePath")}: {Environment.NewLine}{text}{Environment.NewLine}{GetOutputString("Reason")}: {Environment.NewLine}{GetOutputString("S3AIRPathNotSet")}";
        }
        public static string FileDoesNotExist(string text)
        {
            return $"{GetOutputString("UnableToValididatePath")}: {Environment.NewLine}{text}{Environment.NewLine}{GetOutputString("Reason")}: {Environment.NewLine}{GetOutputString("SpecifiedFileDirNotExist")}";
        }

        public static string LegacyModError1(string folderName, string exMessage)
        {
            return $"{GetOutputString("ErrorWithLoading")} {folderName}!{Environment.NewLine}{GetOutputString("JSONErrorPossible")}{Environment.NewLine}{exMessage}";
        }

        public static string LegacyModError2(string folderName, string exMessage)
        {
            return $"{GetOutputString("ErrorWithLoading")} {folderName}!{Environment.NewLine}{exMessage}";
        }



        #endregion

        public static void ApplyLanguage(ref ModManager form)
        {
           

            //Main Buttons
            form.exitButton.Text = Program.CurrentLanguage.GetString("Exit");
            form.saveAndLoadButton.Text = Program.CurrentLanguage.GetString("Save&Load");
            form.saveButton.Text = Program.CurrentLanguage.GetString("Save");

            //Tab Page Headers
            form.toolsPage.Text = Program.CurrentLanguage.GetString("ToolsTab");
            form.modPage.Text = Program.CurrentLanguage.GetString("ModsTab");
            form.settingsPage.Text = Program.CurrentLanguage.GetString("SettingsTab");

            form.recordingsPage.Text = Program.CurrentLanguage.GetString("RecordingsTab");
            form.guidesPage.Text = Program.CurrentLanguage.GetString("GuidesTab");

            form.optionsPage.Text = Program.CurrentLanguage.GetString("GeneralTab");
            form.inputPage.Text = Program.CurrentLanguage.GetString("InputTab");
            form.versionsPage.Text = Program.CurrentLanguage.GetString("VersionsTab");
            form.aboutPage.Text = Program.CurrentLanguage.GetString("AboutTab");

            //Mod Page
            form.groupBox3.Text = Program.CurrentLanguage.GetString("ModsTab_ModProperties");
            form.refreshButton.Text = Program.CurrentLanguage.GetString("Reload");
            form.moreModOptionsButton.Text = Program.CurrentLanguage.GetString("MoreExpandable");

            form.gamebannaURLHandlerOptionsToolStripMenuItem.Text = Program.CurrentLanguage.GetString("GameBannaURLHandler");
            form.enableModStackingToolStripMenuItem.Text = Program.CurrentLanguage.GetString("EnableModStacking");
            form.onForAIRVersionUnreleasedToolStripMenuItem.Text = Program.CurrentLanguage.GetString("EnableModStacking_Note1");
            form.v1909190AndAboveOnlyToolStripMenuItem.Text = Program.CurrentLanguage.GetString("EnableModStacking_Note2");

            form.openModFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenModFolder");
            form.removeModToolStripMenuItem.Text = Program.CurrentLanguage.GetString("RemoveMod");
            form.openModURLToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenModURL");

            //Tools Page
            form.recordingsPage.Text = Program.CurrentLanguage.GetString("RecordingsTab");
            form.guidesPage.Text = Program.CurrentLanguage.GetString("GuidesTab");


            //Recordings Page
            form.uploadButton.Text = Program.CurrentLanguage.GetString("Upload");
            form.copyRecordingFilePath.Text = Program.CurrentLanguage.GetString("CopyFilePath");
            form.openRecordingButton.Text = Program.CurrentLanguage.GetString("Open");
            form.deleteRecordingButton.Text = Program.CurrentLanguage.GetString("Delete");
            form.refreshDebugButton.Text = Program.CurrentLanguage.GetString("Refresh");

            //Guides/Shortcuts Page
            form.groupBox6.Text = Program.CurrentLanguage.GetString("Delete");
            form.openSampleModsFolderButton.Text = Program.CurrentLanguage.GetString("OpenSampleModsFolder");
            form.openUserManualButton.Text = Program.CurrentLanguage.GetString("OpenUserManual");
            form.openModDocumentationButton.Text = Program.CurrentLanguage.GetString("OpenModInstructions");
            form.openModdingTemplatesFolder.Text = Program.CurrentLanguage.GetString("OpenModTemplatesFolder");
            form.label5.Text = Program.CurrentLanguage.GetString("UsefulShortKeys");
            form.airPlacesButton.Text = Program.CurrentLanguage.GetString("AIRPlaces");
            form.airMediaButton.Text = Program.CurrentLanguage.GetString("AIRMedia");
            form.airModManagerPlacesButton.Text = Program.CurrentLanguage.GetString("AIRMMPlaces");

            form.openAppDataFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenAppDataFolder");
            form.openEXEFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenEXEFolder");
            form.openSettingsFileToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenSettingsFile");
            form.openModsFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenModsFolder");
            form.openConfigFileToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenConfigFile");

            form.sonic3AIRHomepageToolStripMenuItem.Text = Program.CurrentLanguage.GetString("S3AIRHomepage");
            form.s3AIRGamebannaToolStripMenuItem.Text = Program.CurrentLanguage.GetString("S3AIRGB");
            form.eukaryot3KTwitterToolStripMenuItem.Text = Program.CurrentLanguage.GetString("Eukaryot3KTwitter");
            form.carJemTwitterToolStripMenuItem.Text = Program.CurrentLanguage.GetString("CarJemTwitter");

            form.openDownloadsFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenDownloadsFolder");
            form.openVersionsFolderToolStripMenuItem.Text = Program.CurrentLanguage.GetString("OpenVersionsFolder");

            //General Options Page
            form.groupBox8.Text = Program.CurrentLanguage.GetString("ModManagerOptions");
            form.label1.Text = Program.CurrentLanguage.GetString("Sonic3AIRPathLabel");
            form.autoRunCheckbox.Text = Program.CurrentLanguage.GetString("EnableAutoBootMode");
            form.autoLaunchDelayLabel.Text = Program.CurrentLanguage.GetString("AutoBootDelay");
            form.keepLoaderOpenCheckBox.Text = Program.CurrentLanguage.GetString("StayOpenOnLoad");
            form.keepOpenOnQuitCheckBox.Text = Program.CurrentLanguage.GetString("StayOpenOnExit");
            form.checkBox1.Text = Program.CurrentLanguage.GetString("CheckforAIRUpdatesOnStart");

            form.groupBox1.Text = Program.CurrentLanguage.GetString("AIRInternalSettings");
            form.label2.Text = Program.CurrentLanguage.GetString("S3KROMPathLabel");
            form.fixGlitchesCheckbox.Text = Program.CurrentLanguage.GetString("FixGlitches");
            form.failSafeModeCheckbox.Text = Program.CurrentLanguage.GetString("FailSafeMode");


            //AIR Path Context Menu Strip
            form.setManuallyHeader.Text = Program.CurrentLanguage.GetString("SetManuallyHeader");
            form.eXEPathToolStripMenuItem.Text = Program.CurrentLanguage.GetString("SetAIRPathManuallyClassic");
            form.fromSettingsFileToolStripMenuItem.Text = Program.CurrentLanguage.GetString("SetAIRPathFromSettings");
            form.fromInstalledHeader.Text = Program.CurrentLanguage.GetString("SetFromInstalledHeader");
            form.aIRVersionZIPToolStripMenuItem.Text = Program.CurrentLanguage.GetString("SelectInstallFromAIRZIP");
            form.installedVersionsToolStripMenuItem.Text = Program.CurrentLanguage.GetString("SelectFromInstalledVersions");
            form.noInstalledVersionsToolStripMenuItem.Text = Program.CurrentLanguage.GetString("NoInstalledVersions");

            //Input Page
            form.groupBox4.Text = Program.CurrentLanguage.GetString("ButtonMappings");
            form.groupBox7.Text = Program.CurrentLanguage.GetString("DeviceIdentifierNames");
            form.openGamepadSettingsButton.Text = Program.CurrentLanguage.GetString("OpenSystemSettingsExpandable");

            form.buttonALabel.Text = Program.CurrentLanguage.GetString("Buttons_A");
            form.buttonBLabel.Text = Program.CurrentLanguage.GetString("Buttons_B");
            form.buttonXLabel.Text = Program.CurrentLanguage.GetString("Buttons_X");
            form.buttonYLabel.Text = Program.CurrentLanguage.GetString("Buttons_Y");
            form.buttonUpLabel.Text = Program.CurrentLanguage.GetString("Buttons_Up");
            form.buttonDownLabel.Text = Program.CurrentLanguage.GetString("Buttons_Down");
            form.buttonLeftLabel.Text = Program.CurrentLanguage.GetString("Buttons_Left");
            form.buttonRightLabel.Text = Program.CurrentLanguage.GetString("Buttons_Right");
            form.buttonStartLabel.Text = Program.CurrentLanguage.GetString("Buttons_Start");
            form.buttonBackLabel.Text = Program.CurrentLanguage.GetString("Buttons_Back");

            //Versions Page
            form.groupBox2.Text = Program.CurrentLanguage.GetString("InstalledVersions");

            form.removeVersionButton.Text = Program.CurrentLanguage.GetString("Remove");
            form.openVersionLocationButton.Text = Program.CurrentLanguage.GetString("OpenLocationExpandable");

            //About Page
            form.checkForUpdatesButton.Text = Program.CurrentLanguage.GetString("CheckForGameUpdatesExpandable");



        }

        public static void ApplyLanguage(ref AutoBootDialog form)
        {
            form.forceStartButton.Text = Program.CurrentLanguage.GetString("ForceStart");
            form.cancelButton.Text = Program.CurrentLanguage.GetString("Cancel");

            form.forceStartButton.Refresh();
            form.cancelButton.Refresh();


            form.forceStartButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            form.cancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        }

        }
}
