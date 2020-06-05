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
            if (Management.MainDataModel.Settings.AutoUpdates)
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

            Management.MainDataModel.TimedEvents = new System.Windows.Forms.Timer();
            Management.MainDataModel.TimedEvents.Tick += TimedEvents_Tick;
        }

        private void InitializeUpdaterEventsTimer()
        {
            Management.MainDataModel.TimedUpdaterEvents = new System.Windows.Forms.Timer();
            Management.MainDataModel.TimedUpdaterEvents.Tick += TimedUpdaterEvents_Tick;
            Management.MainDataModel.TimedUpdaterEvents.Enabled = true;
            Management.MainDataModel.TimedUpdaterEvents.Start();
        }

        private void StartModloader(bool autoBoot = false, string gamebanana_api = "", bool isForcedAutoBoot = false)
        {
            AllowUpdate = false;
            InitializeComponent();
            InitializeHostedComponents();
            SetNonDesignerRules();
            AllowUpdate = true;


            if (Management.ProgramPaths.ValidateInstall(ref Management.ModManagement.S3AIRActiveMods, ref Management.MainDataModel.S3AIRSettings, ref Management.MainDataModel.Global_Settings, ref Management.MainDataModel.Input_Settings) == true)
            {
                Program.Log.InfoFormat("[ModManager] Installation Validated Succesfully...");
                Instance = this;
                Management.ModManagement.UpdateInstance(ref Instance);
                Management.ProcessLauncher.UpdateInstance(ref Instance);
                Management.GameHandler.UpdateInstance(ref Instance);
                Management.InputDeviceManager.UpdateInstance(ref Instance);
                Management.MainDataModel.SetTooltips(ref Instance);
                Management.ModManagement.UpdateModsList(true);
                Management.MainDataModel.UpdateSettingsStates(ref Instance);
                Management.MainDataModel.SetInitialWindowSize(ref Instance);
                Management.MainDataModel.TimedEvents.Enabled = true;
                Management.MainDataModel.TimedEvents.Start();
                Management.GamebananaAPI.GBAPIWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
                Management.GamebananaAPI.GBAPIWatcher.EnableRaisingEvents = true;
                Management.GamebananaAPI.GBAPIWatcher.Changed += Management.GamebananaAPI.GBAPIWatcher_Changed;
                Management.UserLanguage.ApplyLanguage(ref Instance);
                if (autoBoot) Management.GameHandler.LaunchSonic3AIR(isForcedAutoBoot);
                if (gamebanana_api != "") Management.GamebananaAPI.GamebananaAPI_Install(gamebanana_api);
                HasInitilizationCompleted = true;
                Program.Log.InfoFormat("[ModManager] Initilization Completed Succesfully...");
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

        }

        private void openMMlogsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMLogsFolder();
        }

        private void openMMSettingFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMSettingsFile();
        }

        private void openGameRecordingsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenGameRecordingsFolder();
        }

        private void openSettingsGlobalFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenGlobalSettingsFile();
        }

        private void openSettingsInputFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenInputSettingsFile();
        }

        private void AddRemoveURLHandlerButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.Open1ClickInstaller();
        }

        private void addInputMethodButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.AddInputDevice();
        }

        private void removeInputMethodButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.RemoveInputDevice();
        }

        private void importConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.ImportInputDevice();
        }

        private void exportConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.ExportInputDevice();
        }

        private void LaunchOptionsUnderstandingButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchOptionsWarning.Visibility = Visibility.Collapsed;
        }

        public void LaunchOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) Management.MainDataModel.UpdateAIRGameConfigLaunchOptions(ref Instance);
        }

        private void CurrentWindowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (Management.MainDataModel.Global_Settings != null)
                {
                    Management.MainDataModel.Global_Settings.Fullscreen = FullscreenTypeComboBox.SelectedIndex;
                    Management.MainDataModel.Global_Settings.Save();
                    Management.MainDataModel.UpdateAIRSettings(ref Instance);
                }

            }
        }

        private void sonic3AIRPathBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            Management.ProgramPaths.Sonic3AIRPath = sonic3AIRPathBox.Text;
            e.Handled = true;
            Management.MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.LaunchAboutWindow(ref Instance);
        }

        private void ForceQuitGame_Click(object sender, RoutedEventArgs e)
        {
            Management.GameHandler.ForceQuitSonic3AIR();
        }

        private void TimedUpdaterEvents_Tick(object sender, EventArgs e)
        {
            Management.MainDataModel.UpdateInUpdateButtons(ref Instance);
        }

        private void TimedEvents_Tick(object sender, EventArgs e)
        {
            Management.GamebananaAPI.GBAPIInstallTrigger();
        }

        public static void UpdateUIFromInvoke()
        {
            Management.ModManagement.UpdateModsList(true);
        }

        public void DownloadButtonTest_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.AddModsFromURL();
        }

        private void LanguageComboBox_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
        {
            if (AllowUpdate) Management.MainDataModel.UpdateCurrentLanguage(ref Instance);
        }
        private void OpenDownloadsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMDownloadsFolder();
        }

        private void OpenVersionsFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenMMVersionsFolder();
        }

        private void airModManagerPlacesButton_Click(object sender, RoutedEventArgs e)
        {
            airModManagerPlacesButton.ContextMenu.IsOpen = true;
        }

        private void OpenConfigFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenConfigFile();
        }

        private void FromSettingsFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.ChangeAIRPathFromSettings(ref Instance);
        }

        private void moveDeviceNameToTopButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveDeviceName(Management.InputDeviceManager.MoveListItemDirection.MoveToTop, ref Instance);
        }

        private void moveDeviceNameUpButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveDeviceName(Management.InputDeviceManager.MoveListItemDirection.MoveUp, ref Instance);
        }

        private void moveDeviceNameDownButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveDeviceName(Management.InputDeviceManager.MoveListItemDirection.MoveDown, ref Instance);
        }

        private void moveDeviceNameToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveDeviceName(Management.InputDeviceManager.MoveListItemDirection.MoveToBottom, ref Instance);
        }

        private void SaveInputsButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.SaveInputs();
        }

        private void ResetInputsButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.ResetInputMappingsToDefault();

        }

        public void InputMethodsList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            Management.InputDeviceManager.UpdateInputMappings();
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputMethodsList.SelectedItem != null) Management.InputDeviceManager.ChangeInputMappings(sender);
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
            Management.ModManagement.MoveModToTop();
        }

        private void MoveToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.MoveModToBottom();
        }

        private void MoreModOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MoreModOptionsButton.ContextMenu.IsOpen = true;
        }

        private void ModCollectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.MMSettingsManagement.CollectModCollectionMenuItemsDictionary(ref Instance);
            ModCollectionsMenuItem.ContextMenu.IsOpen = true;
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.MoveModDown();
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.MoveModUp();
        }

        private void S3AIRWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.LaunchAIRMediaLink(Management.ProcessLauncher.S3AIRMedia.S3AIRWebsite);
        }

        private void GamebannaButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.LaunchAIRMediaLink(Management.ProcessLauncher.S3AIRMedia.Gamebanna);
        }

        private void EukaTwitterButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.LaunchAIRMediaLink(Management.ProcessLauncher.S3AIRMedia.EukaTwitter);
        }

        private void CarJemTwitterButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.LaunchAIRMediaLink(Management.ProcessLauncher.S3AIRMedia.CarJemTwitter);
        }

        private void OpenModdingTemplatesFolder_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModdingTemplatesFolder();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.Save();
        }

        private void OpenSampleModsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenSampleModsFolder();

        }

        private void OpenUserManualButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenUserManual();
        }

        private void OpenModDocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModDocumentation();
        }

        private void OpenModURLToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModURL((ModViewer.SelectedItem as ModViewerItem).Source.URL);
        }

        private void ShowLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenLogFile();
        }

        private void AutoRunCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate) Management.MainDataModel.UpdateSettingsStates(ref Instance);
        }

        private void ModManager_WindowClosing(object sender, CancelEventArgs e)
        {
            if (Management.GameHandler.isGameRunning)
            {
                e.Cancel = true;
            }
            else
            {
                Management.MainDataModel.Settings.WindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
                Management.MainDataModel.Settings.Save();
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Management.MainDataModel.UpdateInGameButtons(ref Instance);
        }

        private void DeleteRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null && GameRecordingList.SelectedItem is AIR_API.Recording)
            {
                if (Management.RecordingManagement.DeleteRecording(GameRecordingList.SelectedItem as AIR_API.Recording) == true)
                {
                    Management.RecordingManagement.CollectGameRecordings(ref Instance);
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
            RemoveButton_Click(sender, e);
        }

        private void OpenModFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModViewer.SelectedItem != null)
            {
                Management.ProcessLauncher.OpenSelectedModFolder(ModViewer.SelectedItem as ModViewerItem);
            }
        }

        private void View_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void View_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) Management.MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void FolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) Management.ModManagement.UpdateModsList();
        }

        private void AddMods_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.AddMods();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.RemoveMods();
        }

        private void TabControl1_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (PrimaryTabControl.SelectedItem == toolsPage)
                {
                    Management.RecordingManagement.CollectGameRecordings(ref Instance);
                }
            }
        }

        private void TabControl3_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (tabControl3.SelectedItem == recordingsPage)
                {
                    Management.RecordingManagement.CollectGameRecordings(ref Instance);
                    Management.RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
                }
                else if (tabControl3.SelectedItem == inputPage)
                {
                    Management.InputDeviceManager.RefreshInputMappings();
                }
            }
        }

        private void CopyRecordingFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                Management.RecordingManagement.CopyRecordingLocationToClipboard(GameRecordingList.SelectedItem as AIR_API.Recording);
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                Management.RecordingManagement.UploadRecordingToFileDotIO(GameRecordingList.SelectedItem as AIR_API.Recording);
            }

        }

        private void GameRecordingList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            Management.RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
        }

        private void OpenRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenRecordingLocation();
        }

        private void RefreshDebugButton_Click(object sender, RoutedEventArgs e)
        {
            Management.RecordingManagement.CollectGameRecordings(ref Instance);
            GameRecordingList_SelectedIndexChanged(null, null);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.UpdateModsList(true);
        }

        private void UpdateSonic3AIRPath_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.UpdateSonic3AIRLocation(true);
            Management.MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void moveInputMethodUpButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveInputMethod(Management.InputDeviceManager.MoveListItemDirection.MoveUp, ref Instance);
        }

        private void moveInputMethodDownButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveInputMethod(Management.InputDeviceManager.MoveListItemDirection.MoveDown, ref Instance);
        }

        private void moveInputMethodToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveInputMethod(Management.InputDeviceManager.MoveListItemDirection.MoveToBottom, ref Instance);
        }

        private void moveInputMethodToTopButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.MoveInputMethod(Management.InputDeviceManager.MoveListItemDirection.MoveToTop, ref Instance);
        }

        private void ChangeSonic3AIRPathButton_Click(object sender, RoutedEventArgs e)
        {
            updateSonic3AIRPathButton.ContextMenu.IsOpen = true;
            Management.MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void ChangeRomPathButton_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.ChangeS3RomPath(ref Instance);
        }

        private void ModsList_SelectedValueChanged(object sender, EventArgs e)
        {
            Management.MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.Save();
            Management.GameHandler.LaunchSonic3AIR();
            Management.MainDataModel.UpdateInGameButtons(ref Instance);
        }

        private void OpenModsFolder_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenModsFolder();
        }

        private void OpenEXEFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenEXEFolder();
        }

        private void OpenAppDataFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenAppDataFolder();
        }

        private void OpenConfigFile_Click(object sender, RoutedEventArgs e)
        {
            Management.ProcessLauncher.OpenSettingsFile();
        }

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Management.MainDataModel.RefreshSelectedModProperties(ref Instance);
        }

        private void FixGlitchesCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.UpdateBoolSettings(Management.MainDataModel.S3AIRSetting.FixGlitches, fixGlitchesCheckbox.IsChecked.Value);
        }

        private void FailSafeModeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.UpdateBoolSettings(Management.MainDataModel.S3AIRSetting.FailSafeMode, failSafeModeCheckbox.IsChecked.Value);
        }

        private void devModeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.UpdateBoolSettings(Management.MainDataModel.S3AIRSetting.EnableDevMode, devModeCheckbox.IsChecked.Value);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Program.Log.InfoFormat("[ModManager] Closing Mod Manager Window...");
            Management.MainDataModel.Settings.Save();
            App.Instance.Shutdown();
        }

        private void OpenGamepadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.LaunchSystemGamepadSettings();
        }

        private void InputDeviceNamesList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            Management.InputDeviceManager.UpdateInputDeviceNamesList();
        }

        private void AddDeviceNameButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.AddInputDeviceName();
        }

        private void RemoveDeviceNameButton_Click(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.RemoveInputDeviceName();
        }

        private void FullDebugOutputCheckBox_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.Settings.Save();
        }

        private void DisableInGameEnhancementsCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.Settings.Save();
        }

        private void RecordingsSelectedLocationCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HasInitilizationCompleted) Management.RecordingManagement.UpdateSelectedFolderPath(ref Instance);
        }

        private void RecordingsSelectedLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Management.ProgramPaths.SetCustomGameRecordingsFolderPath();
            Management.RecordingManagement.CollectGameRecordings(ref Instance);
        }

        private void playbackRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (Management.RecordingManagement.HasPlaybackWarningBeenPresented)
            {
                playbackRecordingButton.ContextMenu.IsOpen = true;
                Management.RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
            }
            else Management.RecordingManagement.UpdatePlayerWarning(ref Instance);
        }

        private void playUsingCurrentVersionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.RecordingManagement.PlayUsingCurrentVersion(ref Instance);
        }

        private void PlaybackContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Management.RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
        }

        private void playUsingMatchingVersionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.RecordingManagement.PlaybackFromVersionThatMatches(ref Instance);
        }

        private void PlaybackContextMenu_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            Management.RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
        }

        private void addNewModSubfolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.AddNewModSubfolder(ModViewer.View.SelectedItem);
        }

        private void moveModToSubFolderMenuItem_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.RefreshMoveToSubfolderList();
        }

        private void RecordingsViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Management.RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
            if (e.ChangedButton == MouseButton.Right)
            {
                if (GameRecordingList.SelectedItem != null && GameRecordingList.SelectedItem is AIR_API.Recording && GameRecordingList.IsMouseOver)
                {
                    recordingsPanel.ContextMenu.IsOpen = !Management.GameHandler.isGameRunning;
                }
            }

        }

        private void VersionsListBox_SelectedValueChanged(object sender, SelectionChangedEventArgs e)
        {
            Management.VersionManagement.RefreshVersionsList(ref Instance);
        }

        private void TabControl2_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (OptionsTabControl.SelectedItem == versionsPage)
                {
                    Management.VersionManagement.RefreshVersionsList(ref Instance, true);
                }
                else if (OptionsTabControl.SelectedItem == gameOptionsPage || OptionsTabControl.SelectedItem == inputPage)
                {
                    Management.MainDataModel.RetriveLaunchOptions(ref Instance);
                    if (OptionsTabControl.SelectedItem == inputPage)
                    {
                        Management.InputDeviceManager.RefreshInputMappings();
                    }
                }
            }
        }

        private void OpenVersionLocationButton_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.OpenVersionFolder(ref Instance);
        }

        private void RemoveVersionButton_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.RemoveVersion(VersionsListView.SelectedItem, ref Instance);
        }

        private void AIRVersionZIPToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.InstallVersionFromZIP();
        }

        private void AIRPathMenuStrip_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void ManageAIRVersionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.GoToAIRVersionManagement(ref Instance);
        }

        private void editModFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.EditModConfig(ref Instance);
        }

        private void FileMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {

        }

        private void LoadLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.LoadLaunchPresets(e);
        }

        private void RenameLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.RenameLaunchPresets(e);
        }

        private void DeleteLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.DeleteLaunchPresets(e);
        }

        private void SaveLaunchPresetAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.SaveLaunchPresetAs(e);
        }

        private void DeleteAllLaunchPresetsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.MMSettingsManagement.DeleteAllLaunchPresets();
        }

        private void SaveLaunchPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.MMSettingsManagement.SaveLaunchPreset();
        }

        private void LoadModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.LoadModCollection(e);
        }

        private void AddFromExistingModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.AppendFromExistingModCollection(e);
        }

        private void RenameModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.RenameModCollection(e);
        }

        private void DeleteModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.DeleteModCollection(e);
        }

        private void SaveModCollectonAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.MMSettingsManagement.SaveModCollectonAs(e);
        }

        private void DeleteAllModCollectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.MMSettingsManagement.DeleteAllModCollections();
        }

        private void SaveModCollectonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.MMSettingsManagement.SaveModCollecton();
        }

        private void RecordingsPlaybackWarningUnderstoodButton_Click(object sender, RoutedEventArgs e)
        {
            Management.RecordingManagement.HasPlaybackWarningBeenPresented = true;
            Management.RecordingManagement.UpdatePlayerWarning(ref Instance);
        }

        private void refreshVersionsButton_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.RefreshVersionsList(ref Instance, true);
        }

        private void HasDeviceNamesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Management.InputDeviceManager.ChangeHasDeviceNamesState(HasDeviceNamesCheckbox.IsChecked.Value);
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

        private void RenderingRadioButton_Click(object sender, RoutedEventArgs e)
        {
            Management.MainDataModel.UpdateBoolSettings(Management.MainDataModel.S3AIRSetting.UseSoftwareRenderer, SoftwareRenderingRadioButton.IsChecked.Value);
        }

        private void selectVersionButton_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.ChangeAIRPathFromSelected(ref Instance);
        }

        private void SetAIRPathButton_Click(object sender, RoutedEventArgs e)
        {
            Management.VersionManagement.GoToAIRVersionManagement(ref Instance);
        }

        private void ThemeComboBox_ThemeChanged(object sender, GenerationsLib.WPF.Themes.Skin e)
        {
            Management.MainDataModel.RefreshTheme(ref Instance, e);
        }


    }
}
