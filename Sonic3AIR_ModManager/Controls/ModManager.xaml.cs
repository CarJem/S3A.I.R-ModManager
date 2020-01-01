using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net.Http;
using System.Net;
using System.Security.Permissions;
using Microsoft.VisualBasic;
using SharpCompress.Readers;
using SharpCompress.Writers;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Compressors;
using SharpCompress.IO;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Threading;
using System.Resources;
using Path = System.IO.Path;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using DialogResult = System.Windows.Forms.DialogResult;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    // TODO: Backup/Restore A.I.R. Save Data Functionality
    // TODO: Fix URL Handler Not Working While Mod Manager is Open

    public partial class ModManager : Window
    {
        #region Variables

        public System.Windows.Controls.ListView VersionsListView { get => VersionsViewer.View; set => VersionsViewer.View = value; }
        public System.Windows.Controls.ListView GameRecordingList { get => RecordingsViewer.View; set => RecordingsViewer.View = value; }

        public bool AllowUpdate { get; set; } = true;
        public bool HasInitilizationCompleted { get; set; } = false;

        private static ModManager Instance;

        #endregion

        #region Initialize Methods
        public ModManager(bool autoBoot = false, bool isForcedAutoBoot = false)
        {
            InitializeUpdaterEventsTimer();
            if (MainDataModel.Settings.AutoUpdates)
            {
                if (autoBoot == false && Program.AIRUpdaterState == Program.UpdateState.NeverStarted && Program.MMUpdaterState == Program.UpdateState.NeverStarted)
                {
                    new Updater();
                    new ModManagerUpdater();
                }
            }


            StartModloader(autoBoot, "", isForcedAutoBoot);

        }

        public ModManager(string gamebanana_api)
        {
            StartModloader(false, gamebanana_api);
        }

        public void SetNonDesignerRules()
        {
            LaunchOptionsWarning.Visibility = Visibility.Visible;
        }

        private void InitializeHostedComponents()
        {
            ModViewer.View.MouseUp += View_MouseUp;

            ModViewer.SelectionChanged += View_SelectionChanged;
            ModViewer.FolderView.SelectionChanged += FolderView_SelectionChanged;

            MainDataModel.TimedEvents = new System.Windows.Forms.Timer();
            MainDataModel.TimedEvents.Tick += TimedEvents_Tick;
        }

        private void InitializeUpdaterEventsTimer()
        {
            MainDataModel.TimedUpdaterEvents = new System.Windows.Forms.Timer();
            MainDataModel.TimedUpdaterEvents.Tick += TimedUpdaterEvents_Tick;
            MainDataModel.TimedUpdaterEvents.Enabled = true;
            MainDataModel.TimedUpdaterEvents.Start();
        }

        private void StartModloader(bool autoBoot = false, string gamebanana_api = "", bool isForcedAutoBoot = false)
        {
            AllowUpdate = false;
            InitializeComponent();
            InitializeHostedComponents();
            SetNonDesignerRules();
            AllowUpdate = true;


            if (ProgramPaths.ValidateInstall(ref ModManagement.S3AIRActiveMods, ref MainDataModel.S3AIRSettings, ref MainDataModel.Global_Settings, ref MainDataModel.Input_Settings) == true)
            {
                Instance = this;
                ModManagement.UpdateInstance(ref Instance);
                ProcessLauncher.UpdateInstance(ref Instance);
                GameHandler.UpdateInstance(ref Instance);
                InputDeviceManager.UpdateInstance(ref Instance);
                MainDataModel.SetTooltips(ref Instance);
                ModManagement.UpdateModsList(true);
                MainDataModel.UpdateSettingsStates(ref Instance);
                MainDataModel.SetInitialWindowSize(ref Instance);
                MainDataModel.TimedEvents.Enabled = true;
                MainDataModel.TimedEvents.Start();
                FileManagement.GBAPIWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
                FileManagement.GBAPIWatcher.EnableRaisingEvents = true;
                FileManagement.GBAPIWatcher.Changed += FileManagement.GBAPIWatcher_Changed;
                UserLanguage.ApplyLanguage(ref Instance);
                if (autoBoot) GameHandler.LaunchSonic3AIR(isForcedAutoBoot);
                if (gamebanana_api != "") FileManagement.GamebananaAPI_Install(gamebanana_api);
                HasInitilizationCompleted = true;
            }
            else
            {
                this.Close();
            }


        }

        #endregion

        #region Events

        private void useDiscordRPCCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.Settings.Save();
            DiscordRP.UpdateDiscord();
        }

        private void openMMlogsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenMMLogsFolder();
        }

        private void openMMSettingFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenMMSettingsFile();
        }

        private void openGameRecordingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenGameRecordingsFolder();
        }

        private void openSettingsGlobalFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenGlobalSettingsFile();
        }

        private void openSettingsInputFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenInputSettingsFile();
        }

        private void AddRemoveURLHandlerButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.Open1ClickInstaller();
        }

        private void LegacyLoadingCheckbox_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.ToggleLegacyModManagement(LegacyLoadingCheckbox.IsChecked.Value);
        }

        private void addInputMethodButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.AddInputDevice();
        }

        private void removeInputMethodButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.RemoveInputDevice();
        }

        private void importConfigButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.ImportInputDevice();
        }

        private void exportConfigButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.ExportInputDevice();
        }

        private void useDarkModeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.RefreshTheme(ref Instance);
        }

        private void LaunchOptionsUnderstandingButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchOptionsWarning.Visibility = Visibility.Collapsed;
        }

        public void LaunchOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) MainDataModel.UpdateAIRGameConfigLaunchOptions(ref Instance);
        }

        private void CurrentWindowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (MainDataModel.Global_Settings != null)
                {
                    MainDataModel.Global_Settings.Fullscreen = FullscreenTypeComboBox.SelectedIndex;
                    MainDataModel.Global_Settings.Save();
                    MainDataModel.UpdateAIRSettings(ref Instance);
                }

            }
        }

        private void sonic3AIRPathBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            ProgramPaths.Sonic3AIRPath = sonic3AIRPathBox.Text;
            e.Handled = true;
            MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.LaunchAboutWindow(ref Instance);
        }

        private void ForceQuitGame_Click(object sender, RoutedEventArgs e)
        {
            GameHandler.ForceQuitSonic3AIR();
        }

        private void TimedUpdaterEvents_Tick(object sender, EventArgs e)
        {
            MainDataModel.UpdateInUpdateButtons(ref Instance);
        }

        private void TimedEvents_Tick(object sender, EventArgs e)
        {
            FileManagement.GBAPIInstallTrigger();
        }

        public static void UpdateUIFromInvoke()
        {
            ModManagement.UpdateModsList(true);
        }

        public void DownloadButtonTest_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.AddModFromURLLink();
        }

        private void LanguageComboBox_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
        {
            if (AllowUpdate) MainDataModel.UpdateCurrentLanguage(ref Instance);
        }
        private void OpenDownloadsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenMMDownloadsFolder();
        }

        private void OpenVersionsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenMMVersionsFolder();
        }

        private void airModManagerPlacesButton_Click(object sender, RoutedEventArgs e)
        {
            airModManagerPlacesButton.ContextMenu.IsOpen = true;
        }

        private void OpenConfigFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenConfigFile();
        }

        private void FromSettingsFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.ChangeAIRPathFromSettings(ref Instance);
        }

        private void moveDeviceNameToTopButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(InputDeviceManager.MoveListItemDirection.MoveToTop, ref Instance);
        }

        private void moveDeviceNameUpButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(InputDeviceManager.MoveListItemDirection.MoveUp, ref Instance);
        }

        private void moveDeviceNameDownButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(InputDeviceManager.MoveListItemDirection.MoveDown, ref Instance);
        }

        private void moveDeviceNameToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(InputDeviceManager.MoveListItemDirection.MoveToBottom, ref Instance);
        }

        private void SaveInputsButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.SaveInputs();
        }

        private void ResetInputsButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.ResetInputMappingsToDefault();

        }

        public void InputMethodsList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            InputDeviceManager.UpdateInputMappings();
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputMethodsList.SelectedItem != null) InputDeviceManager.ChangeInputMappings(sender);
        }

        private void AirPlacesButton_Click(object sender, RoutedEventArgs e)
        {
            airPlacesButton.ContextMenu.IsOpen = true;

        }

        private void AirMediaButton_Click(object sender, RoutedEventArgs e)
        {
            airMediaButton.ContextMenu.IsOpen = true;

        }

        private void MoveToTopButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.MoveModToTop();
        }

        private void MoveToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.MoveModToBottom();
        }

        private void MoreModOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            moreModOptionsButton.ContextMenu.IsOpen = true;
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.MoveModDown();
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.MoveModUp();
        }

        private void S3AIRWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.LaunchAIRMediaLink(ProcessLauncher.S3AIRMedia.S3AIRWebsite);
        }

        private void GamebannaButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.LaunchAIRMediaLink(ProcessLauncher.S3AIRMedia.Gamebanna);
        }

        private void EukaTwitterButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.LaunchAIRMediaLink(ProcessLauncher.S3AIRMedia.EukaTwitter);
        }

        private void CarJemTwitterButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.LaunchAIRMediaLink(ProcessLauncher.S3AIRMedia.CarJemTwitter);
        }

        private void OpenModdingTemplatesFolder_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenModdingTemplatesFolder();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.Save();
        }

        private void OpenSampleModsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenSampleModsFolder();

        }

        private void OpenUserManualButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenUserManual();
        }

        private void OpenModDocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenModDocumentation();
        }

        private void OpenModURLToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenModURL((ModViewer.SelectedItem as ModViewerItem).Source.URL);
        }

        private void ShowLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenLogFile();
        }

        private void AutoRunCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate) MainDataModel.UpdateSettingsStates(ref Instance);
        }

        private void ModManager_WindowClosing(object sender, CancelEventArgs e)
        {
            if (GameHandler.isGameRunning)
            {
                e.Cancel = true;
            }
            else
            {
                MainDataModel.Settings.WindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
                MainDataModel.Settings.Save();
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainDataModel.UpdateInGameButtons(ref Instance);
        }

        private void DeleteRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null && GameRecordingList.SelectedItem is AIR_API.Recording)
            {
                if (RecordingManagement.DeleteRecording(GameRecordingList.SelectedItem as AIR_API.Recording) == true)
                {
                    RecordingManagement.CollectGameRecordings(ref Instance);
                    GameRecordingList_SelectedIndexChanged(null, null);
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveModToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModViewer.SelectedItem != null)
            {
                FileManagement.RemoveMod((ModViewer.SelectedItem as ModViewerItem).Source);
            }
        }

        private void OpenModFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModViewer.SelectedItem != null)
            {
                ProcessLauncher.OpenSelectedModFolder(ModViewer.SelectedItem as ModViewerItem);
            }
        }

        private void View_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void View_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void FolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) ModManagement.UpdateModsList();
        }

        private void AddMods_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.AddMod();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModViewer.SelectedItem != null)
            {
                FileManagement.RemoveMod((ModViewer.SelectedItem as ModViewerItem).Source);
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (PrimaryTabControl.SelectedItem == toolsPage)
                {
                    RecordingManagement.CollectGameRecordings(ref Instance);
                }
                else if (PrimaryTabControl.SelectedItem == optionsPage)
                {
                    InputDeviceManager.RefreshInputMappings();
                }
            }
        }

        private void TabControl3_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (tabControl3.SelectedItem == recordingsPage)
                {
                    RecordingManagement.CollectGameRecordings(ref Instance);
                    RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
                }
            }
        }

        private void CopyRecordingFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                RecordingManagement.CopyRecordingLocationToClipboard(GameRecordingList.SelectedItem as AIR_API.Recording);
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                RecordingManagement.UploadRecordingToFileDotIO(GameRecordingList.SelectedItem as AIR_API.Recording);
            }

        }

        private void GameRecordingList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
        }

        private void OpenRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenRecordingLocation();
        }

        private void RefreshDebugButton_Click(object sender, RoutedEventArgs e)
        {
            RecordingManagement.CollectGameRecordings(ref Instance);
            GameRecordingList_SelectedIndexChanged(null, null);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.UpdateModsList(true);
        }

        private void UpdateSonic3AIRPath_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.UpdateSonic3AIRLocation(true);
            MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void moveInputMethodUpButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(InputDeviceManager.MoveListItemDirection.MoveUp, ref Instance);
        }

        private void moveInputMethodDownButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(InputDeviceManager.MoveListItemDirection.MoveDown, ref Instance);
        }

        private void moveInputMethodToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(InputDeviceManager.MoveListItemDirection.MoveToBottom, ref Instance);
        }

        private void moveInputMethodToTopButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(InputDeviceManager.MoveListItemDirection.MoveToTop, ref Instance);
        }

        private void ChangeSonic3AIRPathButton_Click(object sender, RoutedEventArgs e)
        {
            updateSonic3AIRPathButton.ContextMenu.IsOpen = true;
            VersionManagement.UpdateAIRVersionsToolstrips(ref Instance);
            MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void ChangeRomPathButton_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.ChangeS3RomPath(ref Instance);
        }

        private void ModsList_SelectedValueChanged(object sender, EventArgs e)
        {
            MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.Save();
            GameHandler.LaunchSonic3AIR();
            MainDataModel.UpdateInGameButtons(ref Instance);
        }

        private void OpenModsFolder_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenModsFolder();
        }

        private void OpenEXEFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenEXEFolder();
        }

        private void OpenAppDataFolderButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenAppDataFolder();
        }

        private void OpenConfigFile_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.OpenSettingsFile();
        }

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void FixGlitchesCheckbox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.UpdateBoolSettings(MainDataModel.S3AIRSetting.FixGlitches, fixGlitchesCheckbox.IsChecked.Value);
        }

        private void FailSafeModeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.UpdateBoolSettings(MainDataModel.S3AIRSetting.FailSafeMode, failSafeModeCheckbox.IsChecked.Value);
        }

        private void devModeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.UpdateBoolSettings(MainDataModel.S3AIRSetting.EnableDevMode, devModeCheckbox.IsChecked.Value);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainDataModel.Settings.Save();
            App.Instance.Shutdown();
        }

        private void OpenGamepadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.LaunchSystemGamepadSettings();
        }

        private void InputDeviceNamesList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            InputDeviceManager.UpdateInputDeviceNamesList();
        }

        private void AddDeviceNameButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.AddInputDeviceName();
        }

        private void RemoveDeviceNameButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.RemoveInputDeviceName();
        }

        private void FullDebugOutputCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.Settings.Save();
        }

        private void DisableInGameEnhancementsCheckbox_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.Settings.Save();
        }

        private void RecordingsSelectedLocationCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) RecordingManagement.UpdateSelectedFolderPath(ref Instance);
        }

        private void RecordingsSelectedLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPaths.SetCustomGameRecordingsFolderPath();
            RecordingManagement.CollectGameRecordings(ref Instance);
        }

        private void playbackRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordingManagement.HasPlaybackWarningBeenPresented)
            {
                playbackRecordingButton.ContextMenu.IsOpen = true;
                RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
            }
            else RecordingManagement.UpdatePlayerWarning(ref Instance);
        }

        private void playUsingCurrentVersionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RecordingManagement.PlayUsingCurrentVersion(ref Instance);
        }

        private void PlaybackContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
        }

        private void playUsingMatchingVersionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RecordingManagement.PlaybackFromVersionThatMatches(ref Instance);
        }

        private void PlaybackContextMenu_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
        }

        private void addNewModSubfolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.AddNewModSubfolder(ModViewer.View.SelectedItem);
        }

        private void moveModToSubFolderMenuItem_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            ModManagement.RefreshMoveToSubfolderList();
        }

        private void RecordingsViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
            if (e.ChangedButton == MouseButton.Right)
            {
                if (GameRecordingList.SelectedItem != null && GameRecordingList.SelectedItem is AIR_API.Recording && GameRecordingList.IsMouseOver)
                {
                    recordingsPanel.ContextMenu.IsOpen = !GameHandler.isGameRunning;
                }
            }

        }

        private void VersionsListBox_SelectedValueChanged(object sender, SelectionChangedEventArgs e)
        {
            VersionManagement.RefreshVersionsList(ref Instance);
        }

        private void TabControl2_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (OptionsTabControl.SelectedItem == versionsPage)
                {
                    VersionManagement.RefreshVersionsList(ref Instance, true);
                }
                else if (OptionsTabControl.SelectedItem == gameOptionsPage || OptionsTabControl.SelectedItem == inputPage)
                {
                    MainDataModel.RetriveLaunchOptions(ref Instance);
                    if (OptionsTabControl.SelectedItem == inputPage)
                    {
                        InputDeviceManager.RefreshInputMappings();
                    }
                }
            }
        }

        private void OpenVersionLocationButton_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.OpenVersionFolder(ref Instance);
        }

        private void RemoveVersionButton_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.RemoveVersion(VersionsListView.SelectedItem, ref Instance);
        }

        private void AIRVersionZIPToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.InstallVersionFromZIP();
        }

        private void AIRPathMenuStrip_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            VersionManagement.UpdateAIRVersionsToolstrips(ref Instance);
        }

        private void ManageAIRVersionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.GoToAIRVersionManagement(ref Instance);
        }

        private void editModFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ModManagement.EditModConfig(ref Instance);
        }

        private void FileMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            VersionManagement.UpdateAIRVersionsToolstrips(ref Instance);
            MMSettingsManagement.CollectModCollectionMenuItemsDictionary(ref Instance);
            MMSettingsManagement.CollectLaunchPresetsMenuItemsDictionary(ref Instance);
        }

        private void LoadLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.LoadLaunchPresets(e);
        }

        private void RenameLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.RenameLaunchPresets(e);
        }

        private void DeleteLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.DeleteLaunchPresets(e);
        }

        private void SaveLaunchPresetAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.SaveLaunchPresetAs(e);
        }

        private void DeleteAllLaunchPresetsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MMSettingsManagement.DeleteAllLaunchPresets();
        }

        private void SaveLaunchPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MMSettingsManagement.SaveLaunchPreset();
        }

        private void LoadModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.LoadModCollection(e);
        }

        private void RenameModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.RenameModCollection(e);
        }

        private void DeleteModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.DeleteModCollection(e);
        }

        private void SaveModCollectonAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            MMSettingsManagement.SaveModCollectonAs(e);
        }

        private void DeleteAllModCollectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MMSettingsManagement.DeleteAllModCollections();
        }

        private void SaveModCollectonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MMSettingsManagement.SaveModCollecton();
        }

        private void RecordingsPlaybackWarningUnderstoodButton_Click(object sender, RoutedEventArgs e)
        {
            RecordingManagement.HasPlaybackWarningBeenPresented = true;
            RecordingManagement.UpdatePlayerWarning(ref Instance);
        }

        private void refreshVersionsButton_Click(object sender, RoutedEventArgs e)
        {
            VersionManagement.RefreshVersionsList(ref Instance, true);
        }

        private void HasDeviceNamesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.ChangeHasDeviceNamesState(HasDeviceNamesCheckbox.IsChecked.Value);
        }

        #endregion

        #region Error Linking to Settings

        private void HyperlinkToGeneralTabAIRPath()
        {
            settingsPage.IsSelected = true;
            PrimaryTabControl.SelectedItem = settingsPage;
            optionsPage.IsSelected = true;
            OptionsTabControl.SelectedItem = optionsPage;

        }

        private void OpenAIRPathSettings(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(LaunchOptionsGroup) && LaunchOptionsFailureMessageBackground.Visibility == Visibility.Visible) HyperlinkToGeneralTabAIRPath();
            else if (!sender.Equals(LaunchOptionsGroup)) HyperlinkToGeneralTabAIRPath();
        }






        #endregion
    }
}
