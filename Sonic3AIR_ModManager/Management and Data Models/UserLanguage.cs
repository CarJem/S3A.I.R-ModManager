using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Globalization;

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
            CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en");

            if (value.Equals(Language.NULL))
            {
                CurrentResource = new ResourceManager("Sonic3AIR_ModManager.Languages.lang.null", Assembly.GetExecutingAssembly());
            }
            else
            {
                CurrentResource = new ResourceManager("Sonic3AIR_ModManager.Languages.lang", Assembly.GetExecutingAssembly());

                if (value.Equals(Language.EN_US)) System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                else if (value.Equals(Language.GR)) System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");
                else if (value.Equals(Language.FR)) System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr");
                else System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }

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

        public static string FolderOrFileDoesNotExist(string path, bool isFile = false)
        {
            if (isFile)
            {
                return $"{GetOutputString("FileString")} \"{path}\" {GetOutputString("DoesNotExist")}";
            }
            else
            {
                return $"{GetOutputString("FolderString")} \"{path}\" {GetOutputString("DoesNotExist")}";
            }
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

        #region Other Random Strings

        public static string GenerateUsefulShortKeysString()
        {
            string returnValue = "";
            returnValue += GetOutputString("UsefulShortKeysHeader") + nL;
            returnValue += nL;
            returnValue += GetOutputString("UsefulShortKeysFullscreen") + nL;
            returnValue += GetOutputString("UsefulShortKeysGameRecordingCapture") + nL;
            returnValue += GetOutputString("UsefulShortKeysContextMenu") + nL;
            returnValue += GetOutputString("UsefulShortKeysRescanInputs") + nL;
            returnValue += GetOutputString("UsefulShortKeysSwapInput") + nL;
            returnValue += GetOutputString("UsefulShortKeysAudioAdjust") + nL;
            returnValue += GetOutputString("UsefulShortKeysBackgroundBlur") + nL;
            returnValue += GetOutputString("UsefulShortKeysFilter") + nL;
            returnValue += nL;
            returnValue += GetOutputString("UsefulShortKeysDevHeader") + nL;
            returnValue += nL;
            returnValue += GetOutputString("UsefulShortKeysPerformanceDisplay") + nL;
            returnValue += GetOutputString("UsefulShortKeysPaletteDebug") + nL;
            returnValue += GetOutputString("UsefulShortKeysRefreshAssets");
            return returnValue;
        }

        public static string BaseModFolderString()
        {
            return Program.LanguageResource.GetString("Viewer_BaseFolderItemFormatString");
        }

        public static string SubModFolderString(string folder)
        {
            string result;
            if (Program.LanguageResource.GetString("Viewer_SubFolderItemFormatString") != null) result = string.Format(Program.LanguageResource.GetString("Viewer_SubFolderItemFormatString"), folder);
            else result = "";
            return result;
        }

        #endregion



        public static void ApplyLanguage(ref ModManager form)
        {
            UserLanguage.ApplyLanguage(ref form.ModViewer);


            #region Errors
            string hyperLink = nL + Program.LanguageResource.GetString("ErrorHyperlinkClickMessage");
            form.modErrorText.Text = Program.LanguageResource.GetString("ModsLoadingError") + hyperLink;


            form.recordingsErrorMessage.Tag = Program.LanguageResource.GetString("RecordingsLoadingError") + Environment.NewLine + "{0}";
            form.recordingsErrorMessage.Text = Program.LanguageResource.GetString("RecordingsLoadingError") + Environment.NewLine + "{0}";
            #endregion

            #region Menu Bar

            form.FileMenuItem.Header = Program.LanguageResource.GetString("FileTab");
            form.ViewMenuItem.Header = Program.LanguageResource.GetString("ViewTab");
            form.HelpMenuItem.Header = Program.LanguageResource.GetString("HelpTab");
            form.OtherMenuItem.Header = Program.LanguageResource.GetString("OtherTab");

    


            #region File Section
            form.AddAIRVersionFileMenuItem.Header = GetOutputString("AddAIRVersion");
            form.ChangeAIRVersionFileMenuItem.Header = GetOutputString("ChangeAIRVersion");
            form.ManageAIRVersionsMenuItem.Header = GetOutputString("ManageAIRVersions");
            form.noInstalledVersionsFileToolStripMenuItem.Header = Program.LanguageResource.GetString("NoInstalledVersions");
            form.exitMenuItem.Header = Program.LanguageResource.GetString("Exit");

            #region Mod Collections Section
            form.ModCollectionsMenuItem.Header = Program.LanguageResource.GetString("ModCollectionsMenuItem");
            form.LoadModCollectionMenuItem.Header = Program.LanguageResource.GetString("LoadModCollectionMenuItem");
            form.RenameModCollectionMenuItem.Header = Program.LanguageResource.GetString("RenameModCollectionMenuItem");
            form.DeleteModCollectionMenuItem.Header = Program.LanguageResource.GetString("DeleteModCollectionMenuItem");
            form.SaveModCollectonAsMenuItem.Header = Program.LanguageResource.GetString("SaveModCollectonAsMenuItem");
            form.DeleteAllModCollectionsMenuItem.Header = Program.LanguageResource.GetString("DeleteAllModCollectionsMenuItem");
            form.SaveModCollectonMenuItem.Header = Program.LanguageResource.GetString("SaveModCollectonMenuItem");
            #endregion

            #region Launch Presets Section
            form.LaunchPresetsMenuItem.Header = Program.LanguageResource.GetString("LaunchPresetsMenuItem");
            form.LoadLaunchPresetsMenuItem.Header = Program.LanguageResource.GetString("LoadLaunchPresetsMenuItem");
            form.RenameLaunchPresetsMenuItem.Header = Program.LanguageResource.GetString("RenameLaunchPresetsMenuItem");
            form.DeleteLaunchPresetsMenuItem.Header = Program.LanguageResource.GetString("DeleteLaunchPresetsMenuItem");
            form.SaveLaunchPresetAsMenuItem.Header = Program.LanguageResource.GetString("SaveLaunchPresetAsMenuItem");
            form.DeleteAllLaunchPresetsMenuItem.Header = Program.LanguageResource.GetString("DeleteAllLaunchPresetsMenuItem");
            form.SaveLaunchPresetMenuItem.Header = Program.LanguageResource.GetString("SaveLaunchPresetMenuItem");
            #endregion



            #endregion

            #region View Section

            form.airPlacesButton.Header = Program.LanguageResource.GetString("AIRPlaces");
            form.airModManagerPlacesButton.Header = Program.LanguageResource.GetString("AIRMMPlaces");

            #region AIR Places Section
            form.openAppDataFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAppDataFolder");
            form.openEXEFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenEXEFolder");
            form.openSettingsFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenSettingsFile");
            form.openModsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModsFolder");
            form.openConfigFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenConfigFile");
            form.showLogFileButton.Header = Program.LanguageResource.GetString("ShowLogFileButton");
            form.openSettingsGlobalFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRGlobalSettingsFile");
            form.openSettingsInputFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRInputSettingsFile");
            form.openGameRecordingsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRGamerecordingsFolder");
            #endregion

            #region AIR Mod Manager Places Section
            form.openDownloadsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenDownloadsFolder");
            form.openVersionsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenVersionsFolder");
            form.openMMlogsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModManagerLogFolder");
            form.openMMSettingFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModManagerSettingsFile");
            #endregion




            #endregion

            #region Help Section

            form.airGuidesItem.Header = Program.LanguageResource.GetString("AIRGuides");
            form.airTipsItem.Header = Program.LanguageResource.GetString("AIRTips");

            #region AIR Guides Section
            form.openSampleModsFolderButton.Header = Program.LanguageResource.GetString("OpenSampleModsFolder");
            form.openUserManualButton.Header = Program.LanguageResource.GetString("OpenUserManual");
            form.openModDocumentationButton.Header = Program.LanguageResource.GetString("OpenModInstructions");
            form.openModdingTemplatesFolder.Header = Program.LanguageResource.GetString("OpenModTemplatesFolder");
            #endregion

            #region AIR Tips Section
            form.label5.Text = GenerateUsefulShortKeysString();
            #endregion



            #endregion

            #region Other Section
            form.AboutMenuItem.Header = Program.LanguageResource.GetString("AboutTab");
            form.airMediaButton.Header = Program.LanguageResource.GetString("AIRMedia");

            #region AIR Media Section
            form.sonic3AIRHomepageToolStripMenuItem.Header = Program.LanguageResource.GetString("S3AIRHomepage");
            form.s3AIRGamebannaToolStripMenuItem.Header = Program.LanguageResource.GetString("S3AIRGB");
            form.eukaryot3KTwitterToolStripMenuItem.Header = Program.LanguageResource.GetString("Eukaryot3KTwitter");
            form.carJemTwitterToolStripMenuItem.Header = Program.LanguageResource.GetString("CarJemTwitter");
            #endregion

            #endregion

            #endregion

            #region Main Buttons
            form.exitButton.Content = Program.LanguageResource.GetString("Exit");
            form.saveAndLoadButton.Content = Program.LanguageResource.GetString("Save&Load");
            form.saveButton.Content = Program.LanguageResource.GetString("Save");
            form.ForceQuitGame.Content = Program.LanguageResource.GetString("ForceQuitGame");
            #endregion

            #region Main Tab Control

            form.toolsPage.Header = Program.LanguageResource.GetString("ToolsTab");
            form.modPage.Header = Program.LanguageResource.GetString("ModsTab");
            form.settingsPage.Header = Program.LanguageResource.GetString("SettingsTab");

            #region Mod Page
            form.groupBox3.Header = Program.LanguageResource.GetString("ModsTab_ModProperties");
            form.refreshButton.Content = Program.LanguageResource.GetString("Reload");
            form.moreModOptionsButton.Content = Program.LanguageResource.GetString("MoreExpandable");

            form.gamebannaURLHandlerOptionsToolStripMenuItem.Content = Program.LanguageResource.GetString("GameBannaURLHandler");
            form.LegacyLoadingText.Text = Program.LanguageResource.GetString("LegacyLoadingText_Line1") + Environment.NewLine + Program.LanguageResource.GetString("LegacyLoadingText_Line2");
            //form.enableModStackingToolStripMenuItem.Content = Program.LanguageResource.GetString("EnableModStacking");
            //form.onForAIRVersionUnreleasedToolStripMenuItem.Text = Program.LanguageResource.GetString("EnableModStacking_Note1");
            //form.v1909190AndAboveOnlyToolStripMenuItem.Text = Program.LanguageResource.GetString("EnableModStacking_Note2");

            form.openModFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModFolder");
            form.removeModToolStripMenuItem.Header = Program.LanguageResource.GetString("RemoveMod");
            form.openModURLToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModURL");
            #endregion

            #region Tools Page
            form.recordingsPage.Header = Program.LanguageResource.GetString("RecordingsTab");

            #region Recordings Page
            form.uploadButton.Content = Program.LanguageResource.GetString("Upload");
            form.copyRecordingFilePath.Content = Program.LanguageResource.GetString("CopyFilePath");
            form.openRecordingButton.Content = Program.LanguageResource.GetString("Open");
            form.deleteRecordingButton.Content = Program.LanguageResource.GetString("Delete");
            form.refreshDebugButton.Content = Program.LanguageResource.GetString("Refresh");

            form.RecordingsLocationDefault.Content = GetOutputString("RecordingsLocationDefault");
            form.RecordingsLocationAppData.Content = GetOutputString("RecordingsLocationAppData");
            form.RecordingsLocationEXEFolder.Content = GetOutputString("RecordingsLocationEXEFolder");
            form.RecordingsLocationRecordingsFolder.Content = GetOutputString("RecordingsLocationRecordingsFolder");
            form.RecordingsLocationBrowse.Content = GetOutputString("RecordingsLocationBrowse");
            form.RecordingsLocationBrowse.Tag = GetOutputString("RecordingsLocationBrowse");
            form.RecordingsSelectedLocationLabel.Text = GetOutputString("RecordingsLocationLabel");

            form.RecordingsViewer.TimestampColumn.Header = Program.LanguageResource.GetString("TimestampColumnHeader");
            form.RecordingsViewer.RecVersionColumn.Header = Program.LanguageResource.GetString("AIRVersionColumnHeader");
            #endregion

            #endregion

            #region Settings Page

            form.gameOptionsPage.Header = Program.LanguageResource.GetString("AIROptionsTab");
            form.optionsPage.Header = Program.LanguageResource.GetString("GeneralTab");
            form.inputPage.Header = Program.LanguageResource.GetString("InputTab");
            form.versionsPage.Header = Program.LanguageResource.GetString("VersionsTab");

            #region General Settings Page

            form.groupBox8.Header = Program.LanguageResource.GetString("ModManagerOptions");
            form.label1.Text = Program.LanguageResource.GetString("Sonic3AIRPathLabel");
            form.autoRunCheckbox.Content = Program.LanguageResource.GetString("EnableAutoBootMode");
            form.autoLaunchDelayLabel.Text = Program.LanguageResource.GetString("AutoBootDelay");
            form.keepLoaderOpenCheckBox.Content = Program.LanguageResource.GetString("StayOpenOnLoad");
            form.keepOpenOnQuitCheckBox.Content = Program.LanguageResource.GetString("StayOpenOnExit");
            form.checkBox1.Content = Program.LanguageResource.GetString("CheckforAIRUpdatesOnStart");
            form.languageLabel.Text = Program.LanguageResource.GetString("LanguageLabel");
            form.FullDebugOutputCheckBox.Content = Program.LanguageResource.GetString("FullDebugOutputOption");

            #region AIR Path Context Menu Strip
            form.SetAIRpathManuallyMenuItem.Header = GetOutputString("SetAIRPathManually");
            form.SetAIRPathFromPreviousDataMenuItem.Header = GetOutputString("SetAIRPathFromPreviousData");
            form.AddAIRVersionMenuItem.Header = GetOutputString("AddAIRVersion");
            form.ChangeAIRVersionMenuItem.Header = GetOutputString("ChangeAIRVersion");

            form.noInstalledVersionsToolStripMenuItem.Header = Program.LanguageResource.GetString("NoInstalledVersions");
            #endregion

            #endregion

            #region AIR Internal Settings Page

            form.groupBox1.Header = Program.LanguageResource.GetString("AIRInternalSettings");
            form.label2.Text = Program.LanguageResource.GetString("S3KROMPathLabel");
            form.fixGlitchesCheckbox.Content = Program.LanguageResource.GetString("FixGlitches");
            form.failSafeModeCheckbox.Content = Program.LanguageResource.GetString("FailSafeMode");
            form.devModeCheckbox.Content = Program.LanguageResource.GetString("AIRDevMode");



            form.useDarkModeCheckBox.Content = Program.LanguageResource.GetString("UseDarkTheme");

            form.StartingWindowLabel.Text = Program.LanguageResource.GetString("StartupWindowLabel");
            form.FullscreenOption1.Content = Program.LanguageResource.GetString("FullscreenOption1");
            form.FullscreenOption2.Content = Program.LanguageResource.GetString("FullscreenOption2");
            form.FullscreenOption3.Content = Program.LanguageResource.GetString("FullscreenOption3");

            form.levelSelectLable.Text = Program.LanguageResource.GetString("LaunchOptionsStartingSceneLabel");
            form.StaringPlayer.Text = Program.LanguageResource.GetString("LaunchOptionsStartingPlayerLabel") + "*";
            form.StartPhaseLabel.Text = Program.LanguageResource.GetString("LaunchOptionsStartingPhaseLabel") + "**";

            form.NoteLabel.Text = "*" + Program.LanguageResource.GetString("LaunchOptionsWarning1");
            form.Note2Label.Text = "**" + Program.LanguageResource.GetString("LaunchOptionsWarning2");

            form.LaunchOptionsForewarnMessage.Text = Program.LanguageResource.GetString("LaunchOptionsForewarning");

            form.LaunchOptionsUnderstandingButton.Content = Program.LanguageResource.GetString("UnderstoodButton");

            form.LaunchOptionsGroup.Header = Program.LanguageResource.GetString("AIRLaunchOptionsGroupHeader");

            form.DefaultPlayerItem.Content = Program.LanguageResource.GetString("DefaultLaunchOptionItemText");
            form.DefaultSceneItem.Content = Program.LanguageResource.GetString("DefaultLaunchOptionItemText");
            form.DefaultPhaseItem.Content = Program.LanguageResource.GetString("DefaultLaunchOptionItemText");

            form.LaunchPlayerS.Content = Program.LanguageResource.GetString("LaunchOptionsPlayerS");
            form.LaunchPlayerT.Content = Program.LanguageResource.GetString("LaunchOptionsPlayerT");
            form.LaunchPlayerK.Content = Program.LanguageResource.GetString("LaunchOptionsPlayerK");
            form.LaunchPlayerKT.Content = Program.LanguageResource.GetString("LaunchOptionsPlayerKT");
            form.LaunchPlayerST.Content = Program.LanguageResource.GetString("LaunchOptionsPlayerST");

            #endregion

            #region Input Page
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
            #endregion

            #region Versions Page
            form.groupBox2.Header = Program.LanguageResource.GetString("InstalledVersions");

            form.removeVersionButton.Content = Program.LanguageResource.GetString("Remove");
            form.openVersionLocationButton.Content = Program.LanguageResource.GetString("OpenLocationExpandable");

            form.VersionsViewer.VersionColumn.Header = Program.LanguageResource.GetString("VersionColumnHeader");
            form.VersionsViewer.PathColumn.Header = Program.LanguageResource.GetString("PathColumnHeader");
            #endregion

            #endregion

            #endregion

            MainDataModel.SetTooltips(ref form);
        }

        public static void ApplyLanguage(ref Controls.InGameContextMenu form)
        {
            form.airPlacesButton.Header = Program.LanguageResource.GetString("AIRPlaces");
            form.airModManagerPlacesButton.Header = Program.LanguageResource.GetString("AIRMMPlaces");

            #region AIR Places Section
            form.openAppDataFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAppDataFolder");
            form.openEXEFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenEXEFolder");
            form.openSettingsFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenSettingsFile");
            form.openModsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModsFolder");
            form.openConfigFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenConfigFile");
            form.showLogFileButton.Header = Program.LanguageResource.GetString("ShowLogFileButton");
            form.openSettingsGlobalFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRGlobalSettingsFile");
            form.openSettingsInputFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRInputSettingsFile");
            form.openGameRecordingsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenAIRGamerecordingsFolder");
            #endregion

            #region AIR Mod Manager Places Section
            form.openDownloadsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenDownloadsFolder");
            form.openVersionsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenVersionsFolder");
            form.openMMlogsFolderToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModManagerLogFolder");
            form.openMMSettingFileToolStripMenuItem.Header = Program.LanguageResource.GetString("OpenModManagerSettingsFile");
            #endregion

            form.airGuidesItem.Header = Program.LanguageResource.GetString("AIRGuides");
            form.airTipsItem.Header = Program.LanguageResource.GetString("AIRTips");

            #region AIR Guides Section
            form.openSampleModsFolderButton.Header = Program.LanguageResource.GetString("OpenSampleModsFolder");
            form.openUserManualButton.Header = Program.LanguageResource.GetString("OpenUserManual");
            form.openModDocumentationButton.Header = Program.LanguageResource.GetString("OpenModInstructions");
            form.openModdingTemplatesFolder.Header = Program.LanguageResource.GetString("OpenModTemplatesFolder");
            #endregion

            #region AIR Tips Section
            form.label5.Text = GenerateUsefulShortKeysString();
            #endregion

            form.ForceCloseMenuButton.Header = Program.LanguageResource.GetString("ForceQuitGame");
            form.CloseContextMenuButton.Header = GetOutputString("ContextMenuCloseHeader");
        }

        public static void ApplyLanguage(ref ModViewer form)
        {

            form.AddNewSubFolderMenuItem.Header = Program.LanguageResource.GetString("AddNewSubFolderButton");
            form.RemoveCurrentFolderMenuItem.Header = Program.LanguageResource.GetString("RemoveCurrentFolderButton");           
            form.CurrentFolderLabel.Content = Program.LanguageResource.GetString("CurrentFolderLabel");            
            form.ActiveModsTab.Header = Program.LanguageResource.GetString("Viewer_ActiveModsTab");            
            form.ModsTab.Header = Program.LanguageResource.GetString("Viewer_ModsTab");           
        }

        public static void ApplyLanguage(ref ItemConflictDialog form)
        {
            form.CancelButton.Content = Program.LanguageResource.GetString("Cancel_Button");
            form.OverwriteButton.Content = Program.LanguageResource.GetString("OverwriteButton");
            form.MakeCopyButton.Content = Program.LanguageResource.GetString("MakeCopyButton");
        }

        public static void ApplyLanguage(ref AboutWindow window)
        {
            window.OkButton.Content = Program.LanguageResource.GetString("Ok_Button");
            window.checkForUpdatesButton.Content = Program.LanguageResource.GetString("CheckForGameUpdatesExpandable");
            window.checkForModManagerUpdatesButton.Content = Program.LanguageResource.GetString("CheckForModManagerUpdatesButton");
        }

        public static void ApplyLanguage(ref DocumentationViewer form)
        {
            form.ExternalOpenButton.Content = Program.LanguageResource.GetString("OpenExternally");
        }

        public static void ApplyLanguage(ref AutoBootDialog form)
        {
            form.ForceStartButton.Content = Program.LanguageResource.GetString("ForceStart");
            form.CancelButton.Content = Program.LanguageResource.GetString("Cancel");
        }

        public static void ApplyLanguage(ref Updater form)
        {
            form.yesButton.Content = Program.LanguageResource.GetString("Yes_Button");
            form.noButton.Content = Program.LanguageResource.GetString("No_Button");
            form.updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
        }

        public static void ApplyLanguage(ref ModManagerUpdater form)
        {
            form.yesButton.Content = Program.LanguageResource.GetString("Yes_Button");
            form.noButton.Content = Program.LanguageResource.GetString("No_Button");
            form.updateMessageLabel.Text = Program.LanguageResource.GetString("Updater_Avaliable");
        }

        public static void ApplyLanguage(ref KeyBindingDialog form)
        {
            form.inputDeviceRadioButton1.Content = Program.LanguageResource.GetString("KeyboardRadioButton");
            form.inputDeviceRadioButton3.Content = Program.LanguageResource.GetString("CustomAdvancedLabel");

            form.resultLabel.Text = Program.LanguageResource.GetString("ResultLabel");
            form.keyLabel.Text = Program.LanguageResource.GetString("KeyLabel");

            form.cancelButton.Content = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Content = Program.LanguageResource.GetString("Ok_Button");

            string title = Program.LanguageResource.GetString("SelectInputTitleExpandable");
            form.Title = (title != null ? title : "");
            form.getInputButton.Content = Program.LanguageResource.GetString("DetectGamepadInputExpandable");
        }

        public static void ApplyLanguage(ref KeybindingsListDialog form)
        {
            form.button1.Content = Program.LanguageResource.GetString("Ok_Button");
            form.editButton.Content = Program.LanguageResource.GetString("EditExpandable");
            form.groupBox1.Header = Program.LanguageResource.GetString("KeybindingsLabel");

            string title = Program.LanguageResource.GetString("EditKeybindingsTitleExpandable");

            form.Title = (title != null ? title : "");
        }

        public static void ApplyLanguage(ref DeviceNameDialog form)
        {
            form.cancelButton.Content = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Content = Program.LanguageResource.GetString("Ok_Button");
            form.detectControllerButton.Content = Program.LanguageResource.GetString("DetectControllerExpandable");
        }

        public static void ApplyLanguage(ref JoystickInputSelectorDialog form)
        {
            form.cancelButton.Content = Program.LanguageResource.GetString("Cancel_Button");
            form.selectButton.Content = Program.LanguageResource.GetString("Select_Button");
            form.refreshButton.Content = Program.LanguageResource.GetString("Refresh");
        }

        public static void ApplyLanguage(ref JoystickReaderDialog form)
        {
            form.cancelButton.Content = Program.LanguageResource.GetString("Cancel_Button");
            form.okButton.Content = Program.LanguageResource.GetString("Ok_Button");
            form.reselectInputButton.Content = Program.LanguageResource.GetString("Reselect_Input_Button");
            form.testingForInputLabel.Text = Program.LanguageResource.GetString("WaitingForInputDialogLabel");
            form.testingForInputLabel.Tag = Program.LanguageResource.GetString("WaitingForInputDialogLabelTag");
        }

    }
}
