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
            form.exitButton.Text = Program.LanguageResource.GetString("Exit");
            form.saveAndLoadButton.Text = Program.LanguageResource.GetString("Save&Load");
            form.saveButton.Text = Program.LanguageResource.GetString("Save");

            //Tab Page Headers
            form.toolsPage.Text = Program.LanguageResource.GetString("ToolsTab");
            form.modPage.Text = Program.LanguageResource.GetString("ModsTab");
            form.settingsPage.Text = Program.LanguageResource.GetString("SettingsTab");

            form.recordingsPage.Text = Program.LanguageResource.GetString("RecordingsTab");
            form.guidesPage.Text = Program.LanguageResource.GetString("GuidesTab");

            form.optionsPage.Text = Program.LanguageResource.GetString("GeneralTab");
            form.inputPage.Text = Program.LanguageResource.GetString("InputTab");
            form.versionsPage.Text = Program.LanguageResource.GetString("VersionsTab");
            form.aboutPage.Text = Program.LanguageResource.GetString("AboutTab");

            //Mod Page
            form.groupBox3.Text = Program.LanguageResource.GetString("ModsTab_ModProperties");
            form.refreshButton.Text = Program.LanguageResource.GetString("Reload");
            form.moreModOptionsButton.Text = Program.LanguageResource.GetString("MoreExpandable");

            form.gamebannaURLHandlerOptionsToolStripMenuItem.Text = Program.LanguageResource.GetString("GameBannaURLHandler");
            form.enableModStackingToolStripMenuItem.Text = Program.LanguageResource.GetString("EnableModStacking");
            form.onForAIRVersionUnreleasedToolStripMenuItem.Text = Program.LanguageResource.GetString("EnableModStacking_Note1");
            form.v1909190AndAboveOnlyToolStripMenuItem.Text = Program.LanguageResource.GetString("EnableModStacking_Note2");

            form.openModFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenModFolder");
            form.removeModToolStripMenuItem.Text = Program.LanguageResource.GetString("RemoveMod");
            form.openModURLToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenModURL");

            //Tools Page
            form.recordingsPage.Text = Program.LanguageResource.GetString("RecordingsTab");
            form.guidesPage.Text = Program.LanguageResource.GetString("GuidesTab");


            //Recordings Page
            form.uploadButton.Text = Program.LanguageResource.GetString("Upload");
            form.copyRecordingFilePath.Text = Program.LanguageResource.GetString("CopyFilePath");
            form.openRecordingButton.Text = Program.LanguageResource.GetString("Open");
            form.deleteRecordingButton.Text = Program.LanguageResource.GetString("Delete");
            form.refreshDebugButton.Text = Program.LanguageResource.GetString("Refresh");



            form.TimestampColumn.Header = Program.LanguageResource.GetString("TimestampColumnHeader");
            form.RecVersionColumn.Header = Program.LanguageResource.GetString("AIRVersionColumnHeader");

            //Guides/Shortcuts Page
            form.groupBox6.Text = Program.LanguageResource.GetString("Delete");
            form.openSampleModsFolderButton.Text = Program.LanguageResource.GetString("OpenSampleModsFolder");
            form.openUserManualButton.Text = Program.LanguageResource.GetString("OpenUserManual");
            form.openModDocumentationButton.Text = Program.LanguageResource.GetString("OpenModInstructions");
            form.openModdingTemplatesFolder.Text = Program.LanguageResource.GetString("OpenModTemplatesFolder");
            form.label5.Text = Program.LanguageResource.GetString("UsefulShortKeys");
            form.airPlacesButton.Text = Program.LanguageResource.GetString("AIRPlaces");
            form.airMediaButton.Text = Program.LanguageResource.GetString("AIRMedia");
            form.airModManagerPlacesButton.Text = Program.LanguageResource.GetString("AIRMMPlaces");

            form.openAppDataFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenAppDataFolder");
            form.openEXEFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenEXEFolder");
            form.openSettingsFileToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenSettingsFile");
            form.openModsFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenModsFolder");
            form.openConfigFileToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenConfigFile");

            form.sonic3AIRHomepageToolStripMenuItem.Text = Program.LanguageResource.GetString("S3AIRHomepage");
            form.s3AIRGamebannaToolStripMenuItem.Text = Program.LanguageResource.GetString("S3AIRGB");
            form.eukaryot3KTwitterToolStripMenuItem.Text = Program.LanguageResource.GetString("Eukaryot3KTwitter");
            form.carJemTwitterToolStripMenuItem.Text = Program.LanguageResource.GetString("CarJemTwitter");

            form.openDownloadsFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenDownloadsFolder");
            form.openVersionsFolderToolStripMenuItem.Text = Program.LanguageResource.GetString("OpenVersionsFolder");

            form.showLogFileButton.Text = Program.LanguageResource.GetString("ShowLogFileButton");

            //General Options Page
            form.groupBox8.Text = Program.LanguageResource.GetString("ModManagerOptions");
            form.label1.Text = Program.LanguageResource.GetString("Sonic3AIRPathLabel");
            form.autoRunCheckbox.Text = Program.LanguageResource.GetString("EnableAutoBootMode");
            form.autoLaunchDelayLabel.Text = Program.LanguageResource.GetString("AutoBootDelay");
            form.keepLoaderOpenCheckBox.Text = Program.LanguageResource.GetString("StayOpenOnLoad");
            form.keepOpenOnQuitCheckBox.Text = Program.LanguageResource.GetString("StayOpenOnExit");
            form.checkBox1.Text = Program.LanguageResource.GetString("CheckforAIRUpdatesOnStart");

            form.groupBox1.Text = Program.LanguageResource.GetString("AIRInternalSettings");
            form.label2.Text = Program.LanguageResource.GetString("S3KROMPathLabel");
            form.fixGlitchesCheckbox.Text = Program.LanguageResource.GetString("FixGlitches");
            form.failSafeModeCheckbox.Text = Program.LanguageResource.GetString("FailSafeMode");
            form.devModeCheckbox.Text = Program.LanguageResource.GetString("AIRDevMode");

            form.languageLabel.Text = Program.LanguageResource.GetString("LanguageLabel");


            //AIR Path Context Menu Strip
            form.setManuallyHeader.Text = Program.LanguageResource.GetString("SetManuallyHeader");
            form.eXEPathToolStripMenuItem.Text = Program.LanguageResource.GetString("SetAIRPathManuallyClassic");
            form.fromSettingsFileToolStripMenuItem.Text = Program.LanguageResource.GetString("SetAIRPathFromSettings");
            form.fromInstalledHeader.Text = Program.LanguageResource.GetString("SetFromInstalledHeader");
            form.aIRVersionZIPToolStripMenuItem.Text = Program.LanguageResource.GetString("SelectInstallFromAIRZIP");
            form.installedVersionsToolStripMenuItem.Text = Program.LanguageResource.GetString("SelectFromInstalledVersions");
            form.noInstalledVersionsToolStripMenuItem.Text = Program.LanguageResource.GetString("NoInstalledVersions");

            //Input Page
            form.groupBox4.Text = Program.LanguageResource.GetString("ButtonMappings");
            form.groupBox7.Text = Program.LanguageResource.GetString("DeviceIdentifierNames");
            form.openGamepadSettingsButton.Text = Program.LanguageResource.GetString("OpenSystemSettingsExpandable");

            form.buttonALabel.Text = Program.LanguageResource.GetString("Buttons_A");
            form.buttonBLabel.Text = Program.LanguageResource.GetString("Buttons_B");
            form.buttonXLabel.Text = Program.LanguageResource.GetString("Buttons_X");
            form.buttonYLabel.Text = Program.LanguageResource.GetString("Buttons_Y");
            form.buttonUpLabel.Text = Program.LanguageResource.GetString("Buttons_Up");
            form.buttonDownLabel.Text = Program.LanguageResource.GetString("Buttons_Down");
            form.buttonLeftLabel.Text = Program.LanguageResource.GetString("Buttons_Left");
            form.buttonRightLabel.Text = Program.LanguageResource.GetString("Buttons_Right");
            form.buttonStartLabel.Text = Program.LanguageResource.GetString("Buttons_Start");
            form.buttonBackLabel.Text = Program.LanguageResource.GetString("Buttons_Back");


            form.importConfigButton.Text = Program.LanguageResource.GetString("ImportExpandable");
            form.exportConfigButton.Text = Program.LanguageResource.GetString("ExportExpanable");

            form.saveInputsButton.Text = Program.LanguageResource.GetString("SaveInputMappingsButton");
            form.resetInputsButton.Text = Program.LanguageResource.GetString("ResetMappingsToDefaultButton");

            //Versions Page
            form.groupBox2.Text = Program.LanguageResource.GetString("InstalledVersions");

            form.removeVersionButton.Text = Program.LanguageResource.GetString("Remove");
            form.openVersionLocationButton.Text = Program.LanguageResource.GetString("OpenLocationExpandable");

            form.VersionColumn.Header = Program.LanguageResource.GetString("VersionColumnHeader");
            form.PathColumn.Header = Program.LanguageResource.GetString("PathColumnHeader");

            //About Page
            form.checkForUpdatesButton.Text = Program.LanguageResource.GetString("CheckForGameUpdatesExpandable");

            form.SetTooltips();

        }

        public static void ApplyLanguage(ref ModManagerV2 window)
        {

        }

        public static void ApplyLanguage(ref AutoBootDialog form)
        {
            form.forceStartButton.Text = Program.LanguageResource.GetString("ForceStart");
            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel");

            form.forceStartButton.Refresh();
            form.cancelButton.Refresh();


            form.forceStartButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            form.cancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        }


        public static void ApplyLanguage(ref Updater form)
        {
            form.yesButton.Text = Program.LanguageResource.GetString("Yes_Button");
            form.noButton.Text = Program.LanguageResource.GetString("No_Button");
            form.updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
        }

        public static void ApplyLanguage(ref KeyBindingDialogOriginal form)
        {
            form.inputDeviceRadioButton1.Text = Program.LanguageResource.GetString("KeyboardRadioButton");
            form.inputDeviceRadioButton2.Text = Program.LanguageResource.GetString("ControllerRadioButton");
            form.inputDeviceRadioButton3.Text = Program.LanguageResource.GetString("CustomAdvancedLabel");

            form.controllerInputTypeRadio1.Text = Program.LanguageResource.GetString("ButtonLabel");
            form.controllerInputTypeRadio2.Text = Program.LanguageResource.GetString("AxisPOVLabel");

            form.axisTypeRadio1.Text = Program.LanguageResource.GetString("AxisRadioButton");
            form.axisTypeRadio2.Text = Program.LanguageResource.GetString("POVRadioButton");
            form.axisTypeRadio3.Text = Program.LanguageResource.GetString("CustomStringLabel");
            form.axisTypeRadio4.Text = Program.LanguageResource.GetString("ThumbstickRadioButton");

            form.axisTypeBox.Text = Program.LanguageResource.GetString("TypeGB");
            form.axisDirectionBox.Text = Program.LanguageResource.GetString("DirectionGB");

            form.resultLabel.Text = Program.LanguageResource.GetString("ResultLabel");
            form.keyLabel.Text = Program.LanguageResource.GetString("KeyLabel");
            form.AxisIDLabel.Text = Program.LanguageResource.GetString("IDLabel");

            form.cancelButton.Text = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Text = Program.LanguageResource.GetString("Ok_Button");

            form.Text = Program.LanguageResource.GetString("SelectInputTitleExpandable");
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
