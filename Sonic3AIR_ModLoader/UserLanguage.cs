using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sonic3AIR_ModManager
{
    public class UserLanguage
    {
        private static string nL = Environment.NewLine;

        public static ResourceManager CurrentResource;

        #region Informing


        public enum Language : int
        {
            NULL = 0,
            EN_US = 1,
            GR = 2,
            FR = 3
        }

        private static Language GetLangVar(string value)
        {
            if (value.Equals("NULL")) return Language.NULL;
            else if (value.Equals("EN_US")) return Language.EN_US;
            else if (value.Equals("GR")) return Language.GR;
            else if (value.Equals("FR")) return Language.FR;
            else return Language.EN_US;
        }

        private static string GetLangString(Language value)
        {
            if (value.Equals(Language.NULL)) return "NULL";
            else if (value.Equals(Language.EN_US)) return "EN_US";
            else if (value.Equals(Language.GR)) return "GR";
            else if (value.Equals(Language.FR)) return "FR";
            else return "EN_US";
        }

        public static Language CurrentLanguage { get { return GetCurrentLanguage(); } set { SetCurrentLanguage(value); } }

        private static Language GetCurrentLanguage()
        {
            string currentLang = Properties.Settings.Default.UserLanguage;
            return GetLangVar(currentLang);

        }

        public static void ApplyLanguageResourcePath(Language value)
        {
            if (value.Equals(Language.NULL)) CurrentResource = new ResourceManager("Sonic3AIR_ModLoader.Languages.lang_null", Assembly.GetExecutingAssembly());
            else if (value.Equals(Language.EN_US)) CurrentResource = new ResourceManager("Sonic3AIR_ModLoader.Languages.lang_en", Assembly.GetExecutingAssembly());
            else if (value.Equals(Language.GR)) CurrentResource = new ResourceManager("Sonic3AIR_ModLoader.Languages.lang_gr", Assembly.GetExecutingAssembly());
            else if (value.Equals(Language.FR)) CurrentResource = new ResourceManager("Sonic3AIR_ModLoader.Languages.lang_fr", Assembly.GetExecutingAssembly());
            else CurrentResource = new ResourceManager("Sonic3AIR_ModLoader.Languages.lang_en", Assembly.GetExecutingAssembly());
        }

        private static void SetCurrentLanguage(Language value)
        {
            Properties.Settings.Default.UserLanguage = GetLangString(value);
            Properties.Settings.Default.Save();
            ApplyLanguageResourcePath(value);
        }

        public static string RecordingUploaded(string url)
        {
           return (Program.LanguageResource.GetString("RecordingUploaded1") == null ? "" : Program.LanguageResource.GetString("RecordingUploaded1")) + Environment.NewLine + (Program.LanguageResource.GetString("RecordingUploaded2") == null ? "" : Program.LanguageResource.GetString("RecordingUploaded2")) + Environment.NewLine + url;
        }

        public static string VersionInstalled(string output)
        {
            return $"{(Program.LanguageResource.GetString("GameInstalledAt") == null ? "" : Program.LanguageResource.GetString("GameInstalledAt"))} \"{output}\"";
        }

        #endregion

        #region Confirmation Dialogs / Titles

        public static string GetOutputString(string destintation)
        {
            return (Program.LanguageResource.GetString(destintation) == null ? "" : Program.LanguageResource.GetString(destintation));
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
            form.exitButton.Content = Program.LanguageResource.GetString("Exit");
            form.saveAndLoadButton.Content = Program.LanguageResource.GetString("Save&Load");
            form.saveButton.Content = Program.LanguageResource.GetString("Save");

            //Tab Page Headers
            form.toolsPage.Header = Program.LanguageResource.GetString("ToolsTab");
            form.modPage.Header = Program.LanguageResource.GetString("ModsTab");
            form.settingsPage.Header = Program.LanguageResource.GetString("SettingsTab");

            form.optionsPage.Header = Program.LanguageResource.GetString("GeneralTab");
            form.inputPage.Header = Program.LanguageResource.GetString("InputTab");
            form.versionsPage.Header = Program.LanguageResource.GetString("VersionsTab");
            form.aboutPage.Header = Program.LanguageResource.GetString("AboutTab");

            //Mod Page
            form.groupBox3.Header = Program.LanguageResource.GetString("ModsTab_ModProperties");
            form.refreshButton.Content = Program.LanguageResource.GetString("Reload");
            form.moreModOptionsButton.Content = Program.LanguageResource.GetString("MoreExpandable");

            form.gamebannaURLHandlerOptionsToolStripMenuItem.Header = Program.LanguageResource.GetString("GameBannaURLHandler");
            form.enableModStackingToolStripMenuItem.Header = Program.LanguageResource.GetString("EnableModStacking");
            form.onForAIRVersionUnreleasedToolStripMenuItem.Header = Program.LanguageResource.GetString("EnableModStacking_Note1");
            form.v1909190AndAboveOnlyToolStripMenuItem.Header = Program.LanguageResource.GetString("EnableModStacking_Note2");

            form.openModFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModFolder");
            form.removeModToolStripMenuItem.Header = Program.LanguageResource.GetString("RemoveMod");
            form.openModURLToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModURL");

            //Tools Page
            form.recordingsPage.Header = Program.LanguageResource.GetString("RecordingsTab");
            form.guidesPage.Header = Program.LanguageResource.GetString("GuidesTab");


            //Recordings Page
            form.uploadButton.Content = Program.LanguageResource.GetString("Upload");
            form.copyRecordingFilePath.Content = Program.LanguageResource.GetString("CopyFilePath");
            form.openRecordingButton.Content = Program.LanguageResource.GetString("Open");
            form.deleteRecordingButton.Content = Program.LanguageResource.GetString("Delete");
            form.refreshDebugButton.Content = Program.LanguageResource.GetString("Refresh");



            form.RecordingsViewer.TimestampColumn.Header = Program.LanguageResource.GetString("TimestampColumnHeader");
            form.RecordingsViewer.RecVersionColumn.Header = Program.LanguageResource.GetString("AIRVersionColumnHeader");

            //Guides/Shortcuts Page
            form.groupBox6.Header = Program.LanguageResource.GetString("Delete");
            form.openSampleModsFolderButton.Content = Program.LanguageResource.GetString("OpenSampleModsFolder");
            form.openUserManualButton.Content = Program.LanguageResource.GetString("OpenUserManual");
            form.openModDocumentationButton.Content = Program.LanguageResource.GetString("OpenModInstructions");
            form.openModdingTemplatesFolder.Content = Program.LanguageResource.GetString("OpenModTemplatesFolder");
            form.label5.Text = Program.LanguageResource.GetString("UsefulShortKeys");
            form.airPlacesButton.Content = Program.LanguageResource.GetString("AIRPlaces");
            form.airMediaButton.Content = Program.LanguageResource.GetString("AIRMedia");
            form.airModManagerPlacesButton.Content = Program.LanguageResource.GetString("AIRMMPlaces");

            form.openAppDataFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAppDataFolder");
            form.openEXEFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenEXEFolder");
            form.openSettingsFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenSettingsFile");
            form.openModsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModsFolder");
            form.openConfigFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenConfigFile");

            form.sonic3AIRHomepageToolStripMenuItem.Header = Program.LanguageResource.GetString("S3AIRHomepage");
            form.s3AIRGamebannaToolStripMenuItem.Header = Program.LanguageResource.GetString("S3AIRGB");
            form.eukaryot3KTwitterToolStripMenuItem.Header = Program.LanguageResource.GetString("Eukaryot3KTwitter");
            form.carJemTwitterToolStripMenuItem.Header = Program.LanguageResource.GetString("CarJemTwitter");

            form.openDownloadsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenDownloadsFolder");
            form.openVersionsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenVersionsFolder");

            form.showLogFileButton.Content = Program.LanguageResource.GetString("ShowLogFileButton");

            //General Options Page
            form.groupBox8.Header = Program.LanguageResource.GetString("ModManagerOptions");
            form.label1.Text = Program.LanguageResource.GetString("Sonic3AIRPathLabel");
            form.autoRunCheckbox.Content = Program.LanguageResource.GetString("EnableAutoBootMode");
            form.autoLaunchDelayLabel.Text = Program.LanguageResource.GetString("AutoBootDelay");
            form.keepLoaderOpenCheckBox.Content = Program.LanguageResource.GetString("StayOpenOnLoad");
            form.keepOpenOnQuitCheckBox.Content = Program.LanguageResource.GetString("StayOpenOnExit");
            form.checkBox1.Content = Program.LanguageResource.GetString("CheckforAIRUpdatesOnStart");

            form.groupBox1.Header = Program.LanguageResource.GetString("AIRInternalSettings");
            form.label2.Text = Program.LanguageResource.GetString("S3KROMPathLabel");
            form.fixGlitchesCheckbox.Content = Program.LanguageResource.GetString("FixGlitches");
            form.failSafeModeCheckbox.Content = Program.LanguageResource.GetString("FailSafeMode");
            form.devModeCheckbox.Content = Program.LanguageResource.GetString("AIRDevMode");

            form.languageLabel.Text = Program.LanguageResource.GetString("LanguageLabel");


            //AIR Path Context Menu Strip
            form.setManuallyHeader.Header = Program.LanguageResource.GetString("SetManuallyHeader");
            form.eXEPathToolStripMenuItem.Header = Program.LanguageResource.GetString("SetAIRPathManuallyClassic");
            form.fromSettingsFileToolStripMenuItem.Header = Program.LanguageResource.GetString("SetAIRPathFromSettings");
            form.fromInstalledHeader.Header = Program.LanguageResource.GetString("SetFromInstalledHeader");
            form.aIRVersionZIPToolStripMenuItem.Header = Program.LanguageResource.GetString("SelectInstallFromAIRZIP");
            form.installedVersionsToolStripMenuItem.Header = Program.LanguageResource.GetString("SelectFromInstalledVersions");
            form.noInstalledVersionsToolStripMenuItem.Header = Program.LanguageResource.GetString("NoInstalledVersions");

            //Input Page
            form.groupBox4.Header = Program.LanguageResource.GetString("ButtonMappings");
            form.groupBox7.Header = Program.LanguageResource.GetString("DeviceIdentifierNames");
            form.openGamepadSettingsButton.Content = Program.LanguageResource.GetString("OpenSystemSettingsExpandable");

            form.buttonALabel.Content = Program.LanguageResource.GetString("Buttons_A");
            form.buttonBLabel.Content = Program.LanguageResource.GetString("Buttons_B");
            form.buttonXLabel.Content = Program.LanguageResource.GetString("Buttons_X");
            form.buttonYLabel.Content = Program.LanguageResource.GetString("Buttons_Y");
            form.buttonUpLabel.Content = Program.LanguageResource.GetString("Buttons_Up");
            form.buttonDownLabel.Content = Program.LanguageResource.GetString("Buttons_Down");
            form.buttonLeftLabel.Content = Program.LanguageResource.GetString("Buttons_Left");
            form.buttonRightLabel.Content = Program.LanguageResource.GetString("Buttons_Right");
            form.buttonStartLabel.Content = Program.LanguageResource.GetString("Buttons_Start");
            form.buttonBackLabel.Content = Program.LanguageResource.GetString("Buttons_Back");


            form.importConfigButton.Content = Program.LanguageResource.GetString("ImportExpandable");
            form.exportConfigButton.Content = Program.LanguageResource.GetString("ExportExpanable");

            form.saveInputsButton.Content = Program.LanguageResource.GetString("SaveInputMappingsButton");
            form.resetInputsButton.Content = Program.LanguageResource.GetString("ResetMappingsToDefaultButton");

            //Versions Page
            form.groupBox2.Header = Program.LanguageResource.GetString("InstalledVersions");

            form.removeVersionButton.Content = Program.LanguageResource.GetString("Remove");
            form.openVersionLocationButton.Content = Program.LanguageResource.GetString("OpenLocationExpandable");

            form.VersionsViewer.VersionColumn.Header = Program.LanguageResource.GetString("VersionColumnHeader");
            form.VersionsViewer.PathColumn.Header = Program.LanguageResource.GetString("PathColumnHeader");

            //About Page
            form.checkForUpdatesButton.Content = Program.LanguageResource.GetString("CheckForGameUpdatesExpandable");
            form.checkForModManagerUpdatesButton.Content = Program.LanguageResource.GetString("CheckForModManagerUpdatesButton");

            form.SetTooltips();
        }

        public static void ApplyLanguage(ref AutoBootDialogV2 form)
        {
            form.ForceStartButton.Content = Program.LanguageResource.GetString("ForceStart");
            form.CancelButton.Content = Program.LanguageResource.GetString("Cancel");
        }


        public static void ApplyLanguage(ref Updater form)
        {
            form.yesButton.Text = Program.LanguageResource.GetString("Yes_Button");
            form.noButton.Text = Program.LanguageResource.GetString("No_Button");
            form.updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
        }

        public static void ApplyLanguage(ref KeyBindingDialog form)
        {
            form.inputDeviceRadioButton1.Text = Program.LanguageResource.GetString("KeyboardRadioButton");
            form.inputDeviceRadioButton3.Text = Program.LanguageResource.GetString("CustomAdvancedLabel");

            form.resultLabel.Text = Program.LanguageResource.GetString("ResultLabel");
            form.keyLabel.Text = Program.LanguageResource.GetString("KeyLabel");

            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Text = Program.LanguageResource.GetString("Ok_Button");

            form.Text = Program.LanguageResource.GetString("SelectInputTitleExpandable");
            form.getInputButton.Text = Program.LanguageResource.GetString("DetectGamepadInputExpandable");
        }

        public static void ApplyLanguage(ref KeybindingsListDialog form)
        {
            form.button1.Text = Program.LanguageResource.GetString("Ok_Button");
            form.editButton.Text = Program.LanguageResource.GetString("EditExpandable");
            form.groupBox1.Text = Program.LanguageResource.GetString("KeybindingsLabel");

            form.Text = Program.LanguageResource.GetString("EditKeybindingsTitleExpandable");
        }

        public static void ApplyLanguage(ref DeviceNameDialog form)
        {
            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Text = Program.LanguageResource.GetString("Ok_Button");
            form.detectControllerButton.Text = Program.LanguageResource.GetString("DetectControllerExpandable");
        }

        public static void ApplyLanguage(ref JoystickInputSelectorDialog form)
        {
            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            form.selectButton.Text = Program.LanguageResource.GetString("Select_Button");
            form.refreshButton.Text = Program.LanguageResource.GetString("Refresh");
        }

        public static void ApplyLanguage(ref JoystickReaderDialog form)
        {
            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Text = Program.LanguageResource.GetString("Ok_Button");
            form.reselectInputButton.Text = Program.LanguageResource.GetString("Reselect_Input_Button");
            form.testingForInputLabel.Text = Program.LanguageResource.GetString("WaitingForInputDialogLabel");
            form.testingForInputLabel.Tag = Program.LanguageResource.GetString("WaitingForInputDialogLabelTag");
        }

    }
}
