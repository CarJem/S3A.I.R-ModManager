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


    // TODO: Implement Version Checking to Prevent More Stupid Bug Reports
    // TODO: Fix Unable to Install Version After Download is Complete
    // TODO: Backup/Restore A.I.R. Save Data Functionality


    public partial class ModManager : Window
    {
        #region Variables

        public System.Windows.Controls.ListView VersionsListView { get => VersionsViewer.View; set => VersionsViewer.View = value; }
        public System.Windows.Controls.ListView GameRecordingList { get => RecordingsViewer.View; set => RecordingsViewer.View = value; }

        public bool AllowUpdate { get; set; } = true;
        public bool HasInitilizationCompleted { get; set; } = false;

        public static ModManager Instance;

        #endregion

        #region Initialize Methods
        public ModManager(bool autoBoot = false)
        {
            if (Properties.Settings.Default.AutoUpdates)
            {
                if (autoBoot == false && Program.AIRUpdaterState == Program.UpdateState.NeverStarted && Program.MMUpdaterState == Program.UpdateState.NeverStarted)
                {
                    new Updater();
                    new ModManagerUpdater();
                }
            }


            StartModloader(autoBoot);

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

            MainDataModel.ApiInstallChecker = new System.Windows.Forms.Timer();
            MainDataModel.ApiInstallChecker.Tick += apiInstallChecker_Tick;
        }

        private void StartModloader(bool autoBoot = false, string gamebanana_api = "")
        {
            AllowUpdate = false;
            InitializeComponent();
            InitializeHostedComponents();
            SetNonDesignerRules();
            AllowUpdate = true;
            MainDataModel.ModManagement = new ModManagement(this);


            if (ProgramPaths.ValidateInstall(ref MainDataModel.ModManagement.S3AIRActiveMods, ref MainDataModel.S3AIRSettings) == true)
            {
                Instance = this;
                MainDataModel.Global_Settings = new AIR_API.Settings_Global(new FileInfo(ProgramPaths.Sonic3AIRGlobalSettingsFile));
                MainDataModel.Input_Settings = new AIR_API.Settings_Input(new FileInfo(ProgramPaths.Sonic3AIRGlobalInputFile));
                MainDataModel.SetTooltips(ref Instance);
                MainDataModel.ModManagement.UpdateModsList(true);
                MainDataModel.UpdateSettingsStates(ref Instance);
                MainDataModel.SetInitialWindowSize(ref Instance);
                MainDataModel.Settings = new Settings.ModManagerSettings(ProgramPaths.Sonic3AIR_MM_SettingsFile);
                MainDataModel.ApiInstallChecker.Enabled = true;
                MainDataModel.ApiInstallChecker.Start();
                FileManagement.GBAPIWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
                FileManagement.GBAPIWatcher.EnableRaisingEvents = true;
                FileManagement.GBAPIWatcher.Changed += FileManagement.GBAPIWatcher_Changed;
                UserLanguage.ApplyLanguage(ref Instance);
                if (autoBoot) ProcessLauncher.LaunchSonic3AIR();
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
            MainDataModel.ModManagement.ToggleLegacyModManagement(LegacyLoadingCheckbox.IsChecked.Value);
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
            ProcessLauncher.ForceQuitSonic3AIR();
        }

        private void apiInstallChecker_Tick(object sender, EventArgs e)
        {
            FileManagement.GBAPIInstallTrigger();
        }

        public static void UpdateUIFromInvoke()
        {
            MainDataModel.ModManagement.UpdateModsList(true);
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
            ChangeAIRPathFromSettings();
        }

        private void moveDeviceNameToTopButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(FileManagement.MoveListItemDirection.MoveToTop);
        }

        private void moveDeviceNameUpButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(FileManagement.MoveListItemDirection.MoveUp);
        }

        private void moveDeviceNameDownButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(FileManagement.MoveListItemDirection.MoveDown);
        }

        private void moveDeviceNameToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveDeviceName(FileManagement.MoveListItemDirection.MoveToBottom);
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
            MainDataModel.ModManagement.MoveModToTop();
        }

        private void MoveToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.ModManagement.MoveModToBottom();
        }

        private void MoreModOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            moreModOptionsButton.ContextMenu.IsOpen = true;
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.ModManagement.MoveModDown();
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            MainDataModel.ModManagement.MoveModUp();
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
            MainDataModel.ModManagement.Save();
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
            if (ProcessLauncher.isGameRunning)
            {
                e.Cancel = true;
            }
            else
            {
                Properties.Settings.Default.WindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
                Properties.Settings.Default.Save();
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
                if (FileManagement.DeleteRecording(GameRecordingList.SelectedItem as AIR_API.Recording) == true)
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
            if (HasInitilizationCompleted) MainDataModel.ModManagement.UpdateModsList();
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
                }
            }
        }

        private void CopyRecordingFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                FileManagement.CopyRecordingLocationToClipboard(GameRecordingList.SelectedItem as AIR_API.Recording);
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                FileManagement.UploadRecordingToFileDotIO(GameRecordingList.SelectedItem as AIR_API.Recording);
            }

        }

        private void GameRecordingList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            MainDataModel.UpdateGameRecordingManagerButtons(ref Instance);
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
            MainDataModel.ModManagement.UpdateModsList(true);
        }

        private void UpdateSonic3AIRPath_Click(object sender, RoutedEventArgs e)
        {
            ProcessLauncher.UpdateSonic3AIRLocation(true);
            MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void moveInputMethodUpButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(FileManagement.MoveListItemDirection.MoveUp);
        }

        private void moveInputMethodDownButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(FileManagement.MoveListItemDirection.MoveDown);
        }

        private void moveInputMethodToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(FileManagement.MoveListItemDirection.MoveToBottom);
        }

        private void moveInputMethodToTopButton_Click(object sender, RoutedEventArgs e)
        {
            InputDeviceManager.MoveInputMethod(FileManagement.MoveListItemDirection.MoveToTop);
        }

        private void ChangeSonic3AIRPathButton_Click(object sender, RoutedEventArgs e)
        {
            updateSonic3AIRPathButton.ContextMenu.IsOpen = true;
            UpdateAIRVersionsToolstrips();
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
            MainDataModel.ModManagement.Save();
            ProcessLauncher.LaunchSonic3AIR();
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
            Properties.Settings.Default.Save();
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
            Properties.Settings.Default.Save();
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
            playbackRecordingButton.ContextMenu.IsOpen = true;
            RecordingManagement.UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
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
            MainDataModel.ModManagement.RefreshMoveToSubfolderList();
        }

        private void RecordingsViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (GameRecordingList.SelectedItem != null && GameRecordingList.SelectedItem is AIR_API.Recording && GameRecordingList.IsMouseOver)
                {
                    recordingsPanel.ContextMenu.IsOpen = !ProcessLauncher.isGameRunning;
                }
            }

        }

        #endregion

        #region AIR EXE Version Handler Toolstrip / Path Management

        private void AIRVersionZIPToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.InstallVersionFromZIP();
        }

        private void AIRPathMenuStrip_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            UpdateAIRVersionsToolstrips();
        }

        private void ManageAIRVersionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoToAIRVersionManagement();
        }

        private void GoToAIRVersionManagement()
        {
            settingsPage.IsSelected = true;
            PrimaryTabControl.SelectedItem = settingsPage;
            versionsPage.IsSelected = true;
            OptionsTabControl.SelectedItem = versionsPage;
        }

        private void UpdateAIRVersionsToolstrips()
        {
            CleanUpInstalledVersionsToolStrip();
            if (Directory.Exists(ProgramPaths.Sonic3AIR_MM_VersionsFolder))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
                var folders = directoryInfo.GetDirectories().ToList();
                if (folders.Count != 0)
                {
                    foreach (var folder in folders.VersionSort().Reverse())
                    {
                        string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                        if (File.Exists(filePath))
                        {
                            ChangeAIRVersionMenuItem.Items.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath));
                            ChangeAIRVersionFileMenuItem.Items.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath));
                        }


                    }
                }

            }

        }

        private void CleanUpInstalledVersionsToolStrip()
        {
            foreach (var item in ChangeAIRVersionMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= ChangeAIRPathByInstalls;
            }
            foreach (var item in ChangeAIRVersionFileMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= ChangeAIRPathByInstalls;
            }
            ChangeAIRVersionMenuItem.Items.Clear();
            ChangeAIRVersionFileMenuItem.Items.Clear();
        }

        private MenuItem GenerateInstalledVersionsToolstripItem(string name, string filepath)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = filepath;
            item.Click += ChangeAIRPathByInstalls;
            item.IsCheckable = false;
            item.IsChecked = (filepath == ProgramPaths.Sonic3AIRPath);
            return item;
        }

        private void ChangeAIRPathByInstalls(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ProgramPaths.Sonic3AIRPath = item.Tag.ToString();
            Properties.Settings.Default.Save();
            MainDataModel.UpdateAIRSettings(ref Instance);
        }

        private void ChangeAIRPathFromSettings()
        {
            if (MainDataModel.S3AIRSettings != null)
            {
                if (MainDataModel.S3AIRSettings.HasEXEPath)
                {
                    if (File.Exists(MainDataModel.S3AIRSettings.AIREXEPath))
                    {
                        ProgramPaths.Sonic3AIRPath = MainDataModel.S3AIRSettings.AIREXEPath;
                        Properties.Settings.Default.Save();
                        MainDataModel.UpdateAIRSettings(ref Instance);
                    }
                    else
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("AIRChangePathNoLongerExists"));
                    }
                }
            }
        }
        #endregion

        #region A.I.R. Version Manager List

        public void RefreshVersionsList(bool fullRefresh = false)
        {
            if (fullRefresh)
            {
                VersionsListView.Items.Clear();
                DirectoryInfo directoryInfo = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
                var folders = directoryInfo.GetDirectories().ToList();
                if (folders.Count != 0)
                {
                    foreach (var folder in folders.VersionSort().Reverse())
                    {
                        string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                        if (File.Exists(filePath))
                        {
                            VersionsListView.Items.Add(new FileManagement.AIRVersionListItem(folder.Name, folder.FullName));
                        }


                    }
                }
            }

            bool enabled = VersionsListView.SelectedItem != null;
            removeVersionButton.IsEnabled = enabled;
            openVersionLocationButton.IsEnabled = enabled;
        }
        private void VersionsListBox_SelectedValueChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVersionsList();
        }

        private void TabControl2_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (OptionsTabControl.SelectedItem == versionsPage)
                {
                    RefreshVersionsList(true);
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
            if (VersionsListView.SelectedItem != null && VersionsListView.SelectedItem is FileManagement.AIRVersionListItem)
            {
                FileManagement.AIRVersionListItem item = VersionsListView.SelectedItem as FileManagement.AIRVersionListItem;
                Process.Start(item.FilePath);
            }
        }

        private void RemoveVersionButton_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.RemoveVersion(VersionsListView.SelectedItem);
        }

        #endregion

        #region Selected Mod Modification

        private void editModFolderToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModViewer.SelectedItem != null)
            {
                var item = (ModViewer.SelectedItem as ModViewerItem);
                var parent = this as Window;
                ConfigEditorDialog cfg = new ConfigEditorDialog(ref parent);
                if(cfg.ShowConfigEditDialog(item.Source).Value == true)
                {
                    (ModViewer.SelectedItem as ModViewerItem).Source.Name = cfg.EditorNameField.Text;
                    (ModViewer.SelectedItem as ModViewerItem).Source.Author = cfg.EditorAuthorField.Text;
                    (ModViewer.SelectedItem as ModViewerItem).Source.Description = cfg.EditorDescriptionField.Text;
                    (ModViewer.SelectedItem as ModViewerItem).Source.URL = cfg.EditorURLField.Text;
                    (ModViewer.SelectedItem as ModViewerItem).Source.GameVersion = cfg.EditorGameVersionField.Text;
                    (ModViewer.SelectedItem as ModViewerItem).Source.ModVersion = cfg.EditorModVersionField.Text;

                    (ModViewer.SelectedItem as ModViewerItem).Source.Save();
                    MainDataModel.ModManagement.UpdateModsList(true);
                }
            }
        }

        #endregion

        #region Error Message Helpers

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

        #region Mod Manager Settings Management

        #region Mod Collections / Launch Presets Mananagement
        private void FileMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            UpdateAIRVersionsToolstrips();
            CollectModCollectionMenuItemsDictionary();
            CollectLaunchPresetsMenuItemsDictionary();
        }

        private void CollectModCollectionMenuItemsDictionary()
        {

            LoadModCollectionMenuItem.RecentItemsSource = null;
            RenameModCollectionMenuItem.RecentItemsSource = null;
            DeleteModCollectionMenuItem.RecentItemsSource = null;
            SaveModCollectonAsMenuItem.RecentItemsSource = null;

            if ( MainDataModel.ModCollectionMenuItems.ContainsKey(0))  MainDataModel.ModCollectionMenuItems[0].Clear();
            if ( MainDataModel.ModCollectionMenuItems.ContainsKey(1))  MainDataModel.ModCollectionMenuItems[1].Clear();
            if ( MainDataModel.ModCollectionMenuItems.ContainsKey(2))  MainDataModel.ModCollectionMenuItems[2].Clear();
            if ( MainDataModel.ModCollectionMenuItems.ContainsKey(3))  MainDataModel.ModCollectionMenuItems[3].Clear();

             MainDataModel.ModCollectionMenuItems.Clear();
            for (int i = 0; i < 4; i++)
            {
                 MainDataModel.ModCollectionMenuItems.Add(i, CollectModCollectionsMenuItems());
            }

            LoadModCollectionMenuItem.RecentItemsSource =  MainDataModel.ModCollectionMenuItems[0];
            RenameModCollectionMenuItem.RecentItemsSource =  MainDataModel.ModCollectionMenuItems[1];
            DeleteModCollectionMenuItem.RecentItemsSource =  MainDataModel.ModCollectionMenuItems[2];
            SaveModCollectonAsMenuItem.RecentItemsSource =  MainDataModel.ModCollectionMenuItems[3];


        }

        private List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectModCollectionsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in MainDataModel.Settings.Options.ModCollections)
            {
                collections.Add(new GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem(collection.Name, collection));
            }
            return collections;
        }

        private void CollectLaunchPresetsMenuItemsDictionary()
        {
            LoadLaunchPresetsMenuItem.RecentItemsSource.Clear();
            RenameLaunchPresetsMenuItem.RecentItemsSource.Clear();
            DeleteLaunchPresetsMenuItem.RecentItemsSource.Clear();
            SaveLaunchPresetAsMenuItem.RecentItemsSource.Clear();

            LoadLaunchPresetsMenuItem.RecentItemsSource = null;
            RenameLaunchPresetsMenuItem.RecentItemsSource = null;
            DeleteLaunchPresetsMenuItem.RecentItemsSource = null;
            SaveLaunchPresetAsMenuItem.RecentItemsSource = null;

            LoadLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            RenameLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            DeleteLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            SaveLaunchPresetAsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();

        }

        private List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectLaunchPresetsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in MainDataModel.Settings.Options.LaunchPresets)
            {
                collections.Add(new GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem(collection.Name, collection));
            }
            return collections;
        }

        private void LoadLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            
        }

        private void RenameLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        private void DeleteLaunchPresetsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        private void SaveLaunchPresetAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        private void DeleteAllLaunchPresetsMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveLaunchPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            MainDataModel.ModManagement.S3AIRActiveMods.Save(collection.Mods);
            MainDataModel.ModManagement.UpdateModsList(true);
        }

        private void RenameModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string name = collection.Name;
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Rename");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Rename");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                MainDataModel.ModManagement.Save();
                int collectionsIndex = MainDataModel.Settings.Options.ModCollections.IndexOf(collection);
                MainDataModel.Settings.Options.ModCollections[collectionsIndex].Name = name;
                SaveModManagerSettings();
            }
        }

        private void DeleteModCollectionMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Delete");
            string message = string.Format(UserLanguage.GetOutputString("ModCollectionDialog_Message_Delete"), collection.Name);
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                MainDataModel.Settings.Options.ModCollections.Remove(collection);
                SaveModManagerSettings();
            }
        }

        private void SaveModCollectonAsMenuItem_RecentItemSelected(object sender, GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Replace");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Replace");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                MainDataModel.ModManagement.Save();
                var collection = e.Content as Settings.ModCollection;
                int collectionsIndex = MainDataModel.Settings.Options.ModCollections.IndexOf(collection);
                MainDataModel.Settings.Options.ModCollections[collectionsIndex] = new Sonic3AIR_ModManager.Settings.ModCollection(MainDataModel.ModManagement.S3AIRActiveMods.ActiveClass, collection.Name);
                SaveModManagerSettings();
            }
        }

        private void DeleteAllModCollectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_DeleteAll");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_DeleteAll");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                MainDataModel.Settings.Options.ModCollections.Clear();
                SaveModManagerSettings();
            }
        }

        private void SaveModCollectonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string name = UserLanguage.GetOutputString("ModCollectionDialog_Name_Save");
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Save");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Save");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                MainDataModel.ModManagement.Save();
                MainDataModel.Settings.Options.ModCollections.Add(new Sonic3AIR_ModManager.Settings.ModCollection(MainDataModel.ModManagement.S3AIRActiveMods.ActiveClass, name));
                SaveModManagerSettings();
            }

        }

        #endregion

        private void SaveModManagerSettings()
        {
            MainDataModel.Settings.Save();
        }

        private void LoadModManagerSettings()
        {
            MainDataModel.Settings = null;
            MainDataModel.Settings = new Settings.ModManagerSettings(ProgramPaths.Sonic3AIR_MM_SettingsFile);
        }




        #endregion


    }
}
