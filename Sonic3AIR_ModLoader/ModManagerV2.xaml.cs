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

namespace Sonic3AIR_ModLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class ModManagerV2 : Window
    {
        public ModManagerV2 ()
        {
            InitializeComponent();
        }
    }

    /*
    public partial class ModManagerV2 : Window
    {
        #region Variables


        #region Winform Leftovers

        System.Windows.Forms.Timer ApiInstallChecker;

        #endregion

        #region Tooltips

        ToolTip AddModTooltip = new ToolTip();
        ToolTip RemoveSelectedModTooltip = new ToolTip();
        ToolTip MoveModUpTooltip = new ToolTip();
        ToolTip MoveModDownTooltip = new ToolTip();
        ToolTip MoveModToTopTooltip = new ToolTip();
        ToolTip MoveModToBottomTooltip = new ToolTip();

        #endregion

        public string nL = Environment.NewLine;
        public static AIR_SDK.Settings S3AIRSettings;
        public static ModManagerV2 Instance;
        public static AIR_SDK.ActiveModsList S3AIRActiveMods;
        public static AIR_SDK.GameConfig GameConfig;
        public static AIR_SDK.VersionMetadata CurrentAIRVersion;
        IList<ModViewerItem> ModsList = new List<ModViewerItem>();
        bool AllowUpdate { get; set; } = true;


        #region Hosted Elements

        public System.Windows.Controls.ListView ModList { get => ModViewer.View; set => ModViewer.View = value; }
        public System.Windows.Controls.ListView VersionsListView { get => VersionsViewer.View; set => VersionsViewer.View = value; }
        public System.Windows.Controls.ListView GameRecordingList { get => RecordingsViewer.View; set => RecordingsViewer.View = value; }


        #endregion

        #endregion

        #region Initialize Methods
        public ModManagerV2(bool autoBoot = false)
        {
            if (Properties.Settings.Default.AutoUpdates)
            {
                if (autoBoot == false && Program.UpdaterState == Updater.UpdateState.NeverStarted) new Updater();
            }


            StartModloader(autoBoot);

        }

        public ModManagerV2(string gamebanana_api)
        {
            StartModloader(false, gamebanana_api);
        }

        #region WPF Definitions
        private void StartupWPFHost()
        {
            ModViewer.View.SelectionChanged += View_SelectionChanged;
            ModViewer.View.MouseUp += View_MouseUp;


            VersionsViewer.View.SelectionChanged += this.VersionsListBox_SelectedValueChanged;

            var VersionsGrid = new AutoSizedGridView();

            VersionsGrid.AllowsColumnReorder = false;

            var VersionColumn = new System.Windows.Controls.GridViewColumn();
            VersionColumn.Header = "Version";
            VersionColumn.Width = 100;
            VersionColumn.DisplayMemberBinding = new System.Windows.Data.Binding("Name");
            VersionsGrid.Columns.Add(VersionColumn);

            var PathColumn = new System.Windows.Controls.GridViewColumn();
            PathColumn.Header = "Path";
            PathColumn.Width = Double.NaN;
            PathColumn.DisplayMemberBinding = new System.Windows.Data.Binding("FilePath");
            VersionsGrid.Columns.Add(PathColumn);

            VersionsViewer.View.View = VersionsGrid;



            var RecordingsGrid = new AutoSizedGridView();

            RecordingsGrid.AllowsColumnReorder = false;

            var TimestampColumn = new System.Windows.Controls.GridViewColumn();
            TimestampColumn.Header = "Item";
            TimestampColumn.Width = 100;
            TimestampColumn.DisplayMemberBinding = new System.Windows.Data.Binding("Name");
            RecordingsGrid.Columns.Add(TimestampColumn);

            var RecVersionColumn = new System.Windows.Controls.GridViewColumn();
            RecVersionColumn.Header = "A.I.R. Version";
            RecVersionColumn.Width = Double.NaN;
            RecVersionColumn.DisplayMemberBinding = new System.Windows.Data.Binding("AIRVersion");
            RecordingsGrid.Columns.Add(RecVersionColumn);

            RecordingsViewer.View.SelectionChanged += GameRecordingList_SelectedIndexChanged;

            RecordingsViewer.View.View = RecordingsGrid;
        }

        private void UpdateWPFListViews(System.Windows.Controls.ListView listView)
        {
            //listView.InvalidateVisual();
            //listView.RenderSize = new System.Windows.Size(listView.RenderSize.Width + 1, listView.RenderSize.Height + 1);
            //listView.RenderSize = new System.Windows.Size(listView.RenderSize.Width - 1, listView.RenderSize.Height - 1);
        }

        #endregion

        private void StartModloader(bool autoBoot = false, string gamebanana_api = "")
        {
            InitializeComponent();
            StartupWPFHost();
            if (ValidateInstall() == true)
            {
                SetTooltips();
                UpdateModsList(true);
                UpdateUI();
                Instance = this;

                ApiInstallChecker.Enabled = true;
                ApiInstallChecker.Start();

                ModFileManagement.GBAPIWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
                ModFileManagement.GBAPIWatcher.EnableRaisingEvents = true;
                ModFileManagement.GBAPIWatcher.Changed += ModFileManagement.GBAPIWatcher_Changed;

                UserLanguage.ApplyLanguage(ref Instance);
                if (autoBoot) GameHandler.LaunchSonic3AIR();
                if (gamebanana_api != "") ModFileManagement.GamebananaAPI_Install(gamebanana_api);
            }
            else
            {
                Environment.Exit(0);
            }

        }

        #endregion

        #region Events

        private void apiInstallChecker_Tick(object sender, EventArgs e)
        {
            ModFileManagement.GBAPIInstallTrigger();
        }

        public static void UpdateUIFromInvoke()
        {
            Instance.UpdateModsList(true);
        }

        public void DownloadButtonTest_Click(object sender, EventArgs e)
        {
            ModFileManagement.AddModFromURLLink();
        }

        private void LanguageComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (AllowUpdate) UpdateCurrentLanguage();
        }
        private void OpenDownloadsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void OpenVersionsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void airModManagerPlacesButton_Click(object sender, EventArgs e)
        {
            modManagerPathMenuStrip.Show(airModManagerPlacesButton, new Point(0, airModManagerPlacesButton.Height));
        }

        private void OpenConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfigFile();
        }

        private void FromSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeAIRPathFromSettings();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Program.UpdaterState == Updater.UpdateState.NeverStarted || Program.UpdaterState == Updater.UpdateState.Finished) new Updater(true);
        }

        private void SaveInputsButton_Click(object sender, EventArgs e)
        {
            GameConfig.Save();
        }

        private void ResetInputsButton_Click(object sender, EventArgs e)
        {

        }

        private void InputMethodsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputMappings();
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            if (inputMethodsList.SelectedItem != null) ChangeInputMappings(sender);
        }

        private void AirPlacesButton_Click(object sender, EventArgs e)
        {
            directoriesMenuStrip.Show(airPlacesButton, new Point(0, airPlacesButton.Height));

        }

        private void AirMediaButton_Click(object sender, EventArgs e)
        {
            mediaLinksMenuStrip.Show(airMediaButton, new Point(0, airMediaButton.Height));

        }
        private void MoveToTopButton_Click(object sender, EventArgs e)
        {
            MoveModToTop();
        }

        private void MoveToBottomButton_Click(object sender, EventArgs e)
        {
            MoveModToBottom();
        }

        private void MoreModOptionsButton_Click(object sender, EventArgs e)
        {
            moreModOptionsMenuStrip.Show(moreModOptionsButton, new Point(0, moreModOptionsButton.Height));
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            MoveModDown();
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            MoveModUp();
        }

        private void ModStackRadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (AllowUpdate)
            {
                AllowUpdate = false;
                if (enableModStackingToolStripMenuItem.Checked)
                {
                    Properties.Settings.Default.EnableNewLoaderMethod = true;
                }
                else
                {
                    Properties.Settings.Default.EnableNewLoaderMethod = false;
                }
                Properties.Settings.Default.Save();
                AllowUpdate = true;
                UpdateModsList(true);
            }
        }

        private void S3AIRWebsiteButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://sonic3air.org/");
        }

        private void GamebannaButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://gamebanana.com/games/6878");
        }

        private void EukaTwitterButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/eukaryot3k");
        }

        private void CarJemTwitterButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/carter5467_99");
        }

        private void OpenModdingTemplatesFolder_Click(object sender, EventArgs e)
        {
            OpenModdingTemplatesFolder();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void OpenSampleModsFolderButton_Click(object sender, EventArgs e)
        {
            OpenSampleModsFolder();

        }

        private void OpenUserManualButton_Click(object sender, EventArgs e)
        {
            OpenUserManual();
        }

        private void OpenModDocumentationButton_Click(object sender, EventArgs e)
        {
            OpenModDocumentation();
        }

        private void OpenModURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenModURL((ModList.SelectedItem as ModViewerItem).Source.URL);
        }

        private void ShowLogFileButton_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void AutoRunCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void ModManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameHandler.isGameRunning)
            {
                e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            }
        }

        private void ModManager_VisibleChanged(object sender, EventArgs e)
        {
            UpdateInGameButtons();
        }

        private void DeleteRecordingButton_Click(object sender, EventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                AIR_SDK.Recording recording = GameRecordingList.SelectedItem as AIR_SDK.Recording;
                if (MessageBox.Show(UserLanguage.DeleteItem(recording.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(recording.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("UnableToDeleteFile"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    CollectGameRecordings();
                    GameRecordingList_SelectedIndexChanged(null, null);
                }
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RemoveModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                ModFileManagement.RemoveMod((ModList.SelectedItem as ModViewerItem).Source);
            }
        }

        private void OpenModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                OpenSelectedModFolder(ModList.SelectedItem as ModViewerItem);
            }
        }

        private void View_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                modContextMenuStrip.Show(ModViewHost, ModViewHost.PointToClient(Cursor.Position));
            }
        }

        private void View_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshSelectedMobProperties();
        }

        private void AddMods_Click(object sender, EventArgs e)
        {
            ModFileManagement.AddMod();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                ModFileManagement.RemoveMod((ModList.SelectedItem as ModViewerItem).Source);
            }
        }
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == toolsPage)
            {
                CollectGameRecordings();
            }
        }
        private void TabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl3.SelectedTab == recordingsPage)
            {
                CollectGameRecordings();
            }
        }

        private void CopyRecordingFilePath_Click(object sender, EventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                var item = GameRecordingList.SelectedItem as AIR_SDK.Recording;
                Clipboard.SetText(item.FilePath);
                MessageBox.Show(Program.LanguageResource.GetString("RecordingPathCopiedToClipboard"));
            }
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                UploadRecordingToFileDotIO(GameRecordingList.SelectedItem as AIR_SDK.Recording);
            }

        }

        private void GameRecordingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameRecordingList.SelectedItem != null)
            {
                openRecordingButton.Enabled = true;
                copyRecordingFilePath.Enabled = true;
                uploadButton.Enabled = true;
                deleteRecordingButton.Enabled = true;
            }
            else
            {
                openRecordingButton.Enabled = false;
                copyRecordingFilePath.Enabled = false;
                uploadButton.Enabled = false;
                deleteRecordingButton.Enabled = false;
            }
        }

        private void OpenRecordingButton_Click(object sender, EventArgs e)
        {
            OpenRecordingLocation();
        }

        private void RefreshDebugButton_Click(object sender, EventArgs e)
        {
            CollectGameRecordings();
            GameRecordingList_SelectedIndexChanged(null, null);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateModsList(true);
        }

        private void UpdateSonic3AIRPath_Click(object sender, EventArgs e)
        {
            GameHandler.UpdateSonic3AIRLocation(true);
            UpdateAIRSettings();
        }

        private void ChangeSonic3AIRPathButton_Click(object sender, EventArgs e)
        {
            AIRPathMenuStrip.Show(updateSonic3AIRPathButton, new Point(0, updateSonic3AIRPathButton.Height));
            UpdateAIRVersionsToolstrips();

        }

        private void ChangeRomPathButton_Click(object sender, EventArgs e)
        {
            ChangeS3RomPath();
        }

        private void ModsList_ItemCheck()
        {
            UpdateModsList();
        }

        private void ModsList_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshSelectedMobProperties();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            Save();
            GameHandler.LaunchSonic3AIR();
            UpdateInGameButtons();
        }

        private void OpenModsFolder_Click(object sender, EventArgs e)
        {
            OpenModsFolder();
        }

        private void OpenEXEFolderButton_Click(object sender, EventArgs e)
        {
            OpenEXEFolder();
        }

        private void OpenAppDataFolderButton_Click(object sender, EventArgs e)
        {
            OpenAppDataFolder();
        }

        private void OpenConfigFile_Click(object sender, EventArgs e)
        {
            OpenSettingsFile();
        }

        private void ModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelectedMobProperties();
        }

        private void FixGlitchesCheckbox_Click(object sender, EventArgs e)
        {
            UpdateBoolSettings(S3AIRSetting.FixGlitches, fixGlitchesCheckbox.Checked);
        }

        private void FailSafeModeCheckbox_Click(object sender, EventArgs e)
        {
            UpdateBoolSettings(S3AIRSetting.FailSafeMode, failSafeModeCheckbox.Checked);
        }

        private void devModeCheckbox_Click(object sender, EventArgs e)
        {
            UpdateBoolSettings(S3AIRSetting.EnableDevMode, devModeCheckbox.Checked);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void OpenGamepadSettingsButton_Click(object sender, EventArgs e)
        {
            LaunchSystemGamepadSettings();
        }

        private void InputDeviceNamesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputDeviceNamesList();
        }

        private void AddDeviceNameButton_Click(object sender, EventArgs e)
        {
            AddInputDeviceName();
        }

        private void RemoveDeviceNameButton_Click(object sender, EventArgs e)
        {
            RemoveInputDeviceName();
        }

        #endregion

        #region Refreshing and Updating

        public void SetTooltips()
        {
            AddModTooltip.SetToolTip(addMods, Program.LanguageResource.GetString("AddAMod"));
            RemoveSelectedModTooltip.SetToolTip(removeButton, Program.LanguageResource.GetString("RemoveSelectedMod"));
            MoveModUpTooltip.SetToolTip(moveUpButton, Program.LanguageResource.GetString("IncreaseModPriority"));
            MoveModDownTooltip.SetToolTip(moveDownButton, Program.LanguageResource.GetString("DecreaseModPriority"));
            MoveModToTopTooltip.SetToolTip(moveToTopButton, Program.LanguageResource.GetString("IncreaseModPriorityToMax"));
            MoveModToBottomTooltip.SetToolTip(moveToBottomButton, Program.LanguageResource.GetString("DecreaseModPriorityToMin"));

            aboutLabel.Text = aboutLabel.Text.Replace("{version}", Program.Version);
            this.Title = this.Title.Replace("{version}", Program.Version);
        }

        public void UpdateInGameButtons()
        {
            bool enabled = !GameHandler.isGameRunning;
            saveAndLoadButton.Enabled = enabled;
            saveButton.Enabled = enabled;
            exitButton.Enabled = enabled;
            keepLoaderOpenCheckBox.Enabled = enabled;
            keepOpenOnQuitCheckBox.Enabled = enabled;
            sonic3AIRPathBox.Enabled = enabled;
            romPathBox.Enabled = enabled;
            fixGlitchesCheckbox.Enabled = enabled;
            failSafeModeCheckbox.Enabled = enabled;
            modPanel.Enabled = enabled;
            autoRunCheckbox.Enabled = enabled;
            inputPanel.Enabled = enabled;
            checkForUpdatesButton.Enabled = enabled;
            devModeCheckbox.Enabled = enabled;
            settingsTabControl.Enabled = enabled;
        }

        private void UpdateUI()
        {
            UpdateAIRSettings();
            UpdateModStackingToggle();
            autoLaunchDelayLabel.Enabled = Properties.Settings.Default.AutoLaunch;
            autoLaunchDelayUpDown.Enabled = Properties.Settings.Default.AutoLaunch;
        }

        private void UpdateModStackingToggle()
        {
            AllowUpdate = false;
            enableModStackingToolStripMenuItem.Checked = Properties.Settings.Default.EnableNewLoaderMethod;
            AllowUpdate = true;
        }

        private void ChangeS3RomPath()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("Sonic3KRomFile")} (*.bin)|*.bin",
                InitialDirectory = Path.GetDirectoryName(S3AIRSettings.Sonic3KRomPath),
                Title = Program.LanguageResource.GetString("SelectSonic3KRomFile")

            };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                S3AIRSettings.Sonic3KRomPath = fileDialog.FileName;
                S3AIRSettings.SaveSettings();
            }
            UpdateAIRSettings();
        }

        private enum S3AIRSetting : int
        {
            FailSafeMode = 0,
            FixGlitches = 1,
            EnableDevMode = 2
        }

        private void UpdateBoolSettings(S3AIRSetting setting, bool isChecked)
        {
            if (setting == S3AIRSetting.FailSafeMode)
            {
                S3AIRSettings.FailSafeMode = isChecked;
            }
            else if (setting == S3AIRSetting.FixGlitches)
            {
                S3AIRSettings.FixGlitches = isChecked;
            }
            else
            {
                S3AIRSettings.EnableDevMode = isChecked;
            }
            S3AIRSettings.SaveSettings();
        }

        private void UpdateCurrentLanguage()
        {
            switch (languageComboBox.SelectedItem)
            {
                case "EN_US":
                    UserLanguage.CurrentLanguage = UserLanguage.Language.EN_US;
                    break;
                case "GR":
                    UserLanguage.CurrentLanguage = UserLanguage.Language.GR;
                    break;
                case "FR":
                    UserLanguage.CurrentLanguage = UserLanguage.Language.FR;
                    break;
                case "NULL":
                    UserLanguage.CurrentLanguage = UserLanguage.Language.NULL;
                    break;
                default:
                    UserLanguage.CurrentLanguage = UserLanguage.Language.NULL;
                    break;
            }

            UserLanguage.ApplyLanguage(ref Instance);
            UpdateAIRSettings();
        }

        private void GetLanguageSelection()
        {
            languageComboBox.Items.Clear();

            AllowUpdate = false;

            languageComboBox.Items.Add("EN_US");
            languageComboBox.Items.Add("GR");
            languageComboBox.Items.Add("FR");
            languageComboBox.Items.Add("NULL");

            if (UserLanguage.CurrentLanguage == UserLanguage.Language.EN_US) languageComboBox.SelectedItem = "EN_US";
            else if (UserLanguage.CurrentLanguage == UserLanguage.Language.GR) languageComboBox.SelectedItem = "GR";
            else if (UserLanguage.CurrentLanguage == UserLanguage.Language.FR) languageComboBox.SelectedItem = "FR";
            else if (UserLanguage.CurrentLanguage == UserLanguage.Language.NULL) languageComboBox.SelectedItem = "NULL";
            else languageComboBox.SelectedItem = "NULL";

            AllowUpdate = true;
        }

        private void UpdateAIRSettings()
        {
            sonic3AIRPathBox.Text = ProgramPaths.Sonic3AIRPath;
            romPathBox.Text = S3AIRSettings.Sonic3KRomPath;
            fixGlitchesCheckbox.Checked = S3AIRSettings.FixGlitches;
            failSafeModeCheckbox.Checked = S3AIRSettings.FailSafeMode;
            devModeCheckbox.Checked = S3AIRSettings.EnableDevMode;
            GetLanguageSelection();

            bool loaderMethodPast = Properties.Settings.Default.EnableNewLoaderMethod;

            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                string metaDataFile = Directory.GetFiles(Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath), "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                if (metaDataFile != null)
                {
                    try
                    {
                        CurrentAIRVersion = new AIR_SDK.VersionMetadata(new FileInfo(metaDataFile));
                        if (CurrentAIRVersion.Version.CompareTo(new Version("19.09.0.0")) >= 0)
                        {
                            Properties.Settings.Default.EnableNewLoaderMethod = true;
                            enableModStackingToolStripMenuItem.Enabled = true;
                            airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: {CurrentAIRVersion.VersionString}";
                            airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {S3AIRSettings.Version.ToString()}";
                        }
                        else
                        {
                            Properties.Settings.Default.EnableNewLoaderMethod = false;
                            enableModStackingToolStripMenuItem.Enabled = false;
                            airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: {CurrentAIRVersion.VersionString}";
                            airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {S3AIRSettings.Version.ToString()}";
                        }
                    }
                    catch
                    {
                        NullSituation();

                    }

                }
                else
                {
                    NullSituation();
                }
            }
            else NullSituation();


            UpdateModStackingToggle();
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.EnableNewLoaderMethod != loaderMethodPast) UpdateModsList(true);

            void NullSituation()
            {
                Properties.Settings.Default.EnableNewLoaderMethod = false;
                enableModStackingToolStripMenuItem.Enabled = false;
                airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: NULL";
                airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {S3AIRSettings.Version.ToString()}";
            }

        }

        public void RefreshSelectedMobProperties()
        {
            if (ModList.SelectedItem != null)
            {
                if (Properties.Settings.Default.EnableNewLoaderMethod)
                {
                    moveUpButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as ModViewerItem)) > 0);
                    moveDownButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as ModViewerItem)) < ModsList.Count - 1);
                    moveToTopButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as ModViewerItem)) > 0);
                    moveToBottomButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as ModViewerItem)) < ModsList.Count - 1);
                }
                else
                {
                    moveUpButton.Enabled = false;
                    moveDownButton.Enabled = false;
                    moveToTopButton.Enabled = false;
                    moveToBottomButton.Enabled = false;
                }
                removeButton.Enabled = true;
                removeModToolStripMenuItem.Enabled = true;
                openModFolderToolStripMenuItem.Enabled = true;
                openModURLToolStripMenuItem.Enabled = ((ModList.SelectedItem as ModViewerItem).Source.URL != null);
            }
            else
            {
                moveUpButton.Enabled = false;
                moveDownButton.Enabled = false;
                removeButton.Enabled = false;
                removeModToolStripMenuItem.Enabled = false;
                openModFolderToolStripMenuItem.Enabled = false;
                openModURLToolStripMenuItem.Enabled = false;
            }



            if (ModList.SelectedItem != null)
            {
                AIR_SDK.Mod item = (ModList.SelectedItem as ModViewerItem).Source;
                if (item != null)
                {

                    modInfoTextBox.Clear();

                    modInfoTextBox.SelectionFont = new Font(modInfoTextBox.Font, FontStyle.Bold);

                    modInfoTextBox.AppendText($"{Program.LanguageResource.GetString("By")}: {item.Author}{nL}{Program.LanguageResource.GetString("Version")}: {item.ModVersion}{nL}{Program.LanguageResource.GetString("AIRVersion")}: {item.GameVersion}{nL}{item.TechnicalName}");

                    modInfoTextBox.SelectionFont = new Font(modInfoTextBox.Font, FontStyle.Regular);


                    string description = item.Description;
                    if (description == "No Description Provided.")
                    {
                        description = Program.LanguageResource.GetString("NoModDescript");
                    }

                    modInfoTextBox.AppendText($"{nL}{nL}{description}");
                }
                else
                {
                    modInfoTextBox.Text = "";
                }
            }
            else
            {
                modInfoTextBox.Text = "";
            }
        }
        #endregion

        #region Information Retriving

        private void CollectGameRecordings()
        {
            GameRecordingList.Items.Clear();
            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                recordingsErrorMessage.SendToBack();
                recordingsErrorMessage.Visible = false;

                string baseDirectory = Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath);
                if (Directory.Exists(baseDirectory))
                {
                    Regex reg = new Regex(@"(gamerecording_)\d{6}(_)\d{6}");
                    DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);
                    FileInfo[] fileInfo = directoryInfo.GetFiles("*.bin").Where(path => reg.IsMatch(path.Name)).ToArray();
                    foreach (var file in fileInfo)
                    {
                        AIR_SDK.Recording recording = new AIR_SDK.Recording(file);
                        GameRecordingList.Items.Add(recording);
                    }
                }
            }
            else
            {
                recordingsErrorMessage.BringToFront();
                recordingsErrorMessage.Visible = true;
                recordingsErrorMessage.Enabled = false;
            }
            UpdateWPFListViews(GameRecordingList);

        }

        private void CollectInputMappings()
        {
            inputMethodsList.Items.Clear();
            if (ProgramPaths.Sonic3AIRPath != null && ProgramPaths.Sonic3AIRPath != "" && File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                string Sonic3AIREXEFolder = Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath);
                FileInfo config = new FileInfo($"{Sonic3AIREXEFolder}//config.json");
                if (config.Exists)
                {
                    inputPanel.Enabled = true;
                    inputErrorMessage.Visible = false;
                    inputErrorMessage.SendToBack();

                    try
                    {
                        GameConfig = new AIR_SDK.GameConfig(config);


                        foreach (var inputMethod in GameConfig.Input.Devices)
                        {
                            inputMethodsList.Items.Add(inputMethod);
                        }
                    }
                    catch
                    {
                        NullSituation(1);
                    }
                }
                else NullSituation(2);
            }
            else NullSituation();

            void NullSituation(int situation = 0)
            {
                if (situation == 0) inputErrorMessage.Text = Program.LanguageResource.GetString("InputMappingError1");
                else if (situation == 1) inputErrorMessage.Text = Program.LanguageResource.GetString("InputMappingError2");
                else if (situation == 2) inputErrorMessage.Text = Program.LanguageResource.GetString("InputMappingError3");

                inputPanel.Enabled = false;
                inputErrorMessage.Parent = inputPanel;
                inputErrorMessage.Location = inputPanel.Location;
                inputErrorMessage.Width = inputPanel.Width;
                inputErrorMessage.Height = inputPanel.Height;
                inputErrorMessage.BringToFront();
                inputErrorMessage.Visible = true;
            }
        }



        private bool ValidateInstall()
        {
            return ProgramPaths.ValidateInstall(ref S3AIRActiveMods, ref S3AIRSettings);
        }

        #endregion

        #region Input Mapping

        private void UpdateInputMappings()
        {
            inputDeviceNamesList.Items.Clear();
            if (GameConfig != null)
            {
                if (inputMethodsList.SelectedItem != null)
                {
                    if (inputMethodsList.SelectedItem is AIR_SDK.GameConfig.InputDevices.Device)
                    {
                        AIR_SDK.GameConfig.InputDevices.Device device = inputMethodsList.SelectedItem as AIR_SDK.GameConfig.InputDevices.Device;
                        aInputButton.Text = (device.A.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.A.FirstOrDefault());
                        bInputButton.Text = (device.B.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.B.FirstOrDefault());
                        xInputButton.Text = (device.X.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.X.FirstOrDefault());
                        yInputButton.Text = (device.Y.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Y.FirstOrDefault());
                        upInputButton.Text = (device.Up.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Up.FirstOrDefault());
                        downInputButton.Text = (device.Down.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Down.FirstOrDefault());
                        leftInputButton.Text = (device.Left.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Left.FirstOrDefault());
                        rightInputButton.Text = (device.Right.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Right.FirstOrDefault());
                        startInputButton.Text = (device.Start.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Start.FirstOrDefault());
                        backInputButton.Text = (device.Back.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Back.FirstOrDefault());

                        if (aInputButton.Text == "") aInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (bInputButton.Text == "") bInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (xInputButton.Text == "") xInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (yInputButton.Text == "") yInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (upInputButton.Text == "") upInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (downInputButton.Text == "") downInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (leftInputButton.Text == "") leftInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (rightInputButton.Text == "") rightInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (startInputButton.Text == "") startInputButton.Text = Program.LanguageResource.GetString("Input_NONE");
                        if (backInputButton.Text == "") backInputButton.Text = Program.LanguageResource.GetString("Input_NONE");

                        UpdateInputDeviceNamesList(true);



                    }
                }
                else
                {
                    DisableMappings();
                }
            }
        }

        private void ToggleDeviceNamesUI(bool enabled)
        {
            inputDeviceNamesList.Enabled = enabled;
            addDeviceNameButton.Enabled = enabled;
            removeDeviceNameButton.Enabled = (enabled == true ? inputDeviceNamesList.SelectedItem != null : enabled);
        }

        private void DisableMappings()
        {
            inputDeviceNamesList.Items.Clear();
            aInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            bInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            xInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            yInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            upInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            downInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            leftInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            rightInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            startInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            backInputButton.Text = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL")));
        }

        private void ChangeInputMappings(object sender)
        {
            AIR_SDK.GameConfig.InputDevices.Device device = inputMethodsList.SelectedItem as AIR_SDK.GameConfig.InputDevices.Device;

            if (sender.Equals(aInputButton)) ChangeMappings(ref device, "A");
            else if (sender.Equals(bInputButton)) ChangeMappings(ref device, "B");
            else if (sender.Equals(xInputButton)) ChangeMappings(ref device, "X");
            else if (sender.Equals(yInputButton)) ChangeMappings(ref device, "Y");
            else if (sender.Equals(upInputButton)) ChangeMappings(ref device, "Up");
            else if (sender.Equals(downInputButton)) ChangeMappings(ref device, "Down");
            else if (sender.Equals(leftInputButton)) ChangeMappings(ref device, "Left");
            else if (sender.Equals(rightInputButton)) ChangeMappings(ref device, "Right");
            else if (sender.Equals(startInputButton)) ChangeMappings(ref device, "Start");
            else if (sender.Equals(backInputButton)) ChangeMappings(ref device, "Back");

            void ChangeMappings(ref AIR_SDK.GameConfig.InputDevices.Device button, string input)
            {
                switch (input)
                {
                    case "A":
                        MappingDialog(ref button.A);
                        break;
                    case "B":
                        MappingDialog(ref button.B);
                        break;
                    case "X":
                        MappingDialog(ref button.X);
                        break;
                    case "Y":
                        MappingDialog(ref button.Y);
                        break;
                    case "Up":
                        MappingDialog(ref button.Up);
                        break;
                    case "Down":
                        MappingDialog(ref button.Down);
                        break;
                    case "Left":
                        MappingDialog(ref button.Left);
                        break;
                    case "Right":
                        MappingDialog(ref button.Right);
                        break;
                    case "Start":
                        MappingDialog(ref button.Start);
                        break;
                    case "Back":
                        MappingDialog(ref button.Back);
                        break;
                }
                UpdateInputMappings();

                void MappingDialog(ref List<string> mappings)
                {
                    var mD = new KeybindingsListDialog(mappings);
                    mD.ShowDialog();
                }

            }
        }

        private void AddInputDeviceName()
        {
            if (inputMethodsList.SelectedItem != null)
            {
                int index = inputMethodsList.SelectedIndex;
                string newDevice = "New Device";
                DialogResult result = ExtraDialog.ShowInputDialog(ref newDevice, Program.LanguageResource.GetString("AddNewDeviceTitle"), Program.LanguageResource.GetString("AddNewDeviceDescription"));
                GameConfig.Input.Devices[inputMethodsList.SelectedIndex].DeviceNames.Add(newDevice);
                UpdateInputMappings();

            }

        }

        private void RemoveInputDeviceName()
        {
            if (inputMethodsList.SelectedItem != null && inputDeviceNamesList.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(inputDeviceNamesList.SelectedItem.ToString()), Program.LanguageResource.GetString("DeleteDeviceTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = inputDeviceNamesList.SelectedIndex;
                    GameConfig.Input.Devices[inputMethodsList.SelectedIndex].DeviceNames.RemoveAt(index);
                    UpdateInputMappings();
                }
            }
        }

        private void UpdateInputDeviceNamesList(bool refreshItems = false)
        {
            if (GameConfig != null)
            {
                if (inputMethodsList.SelectedItem != null)
                {
                    if (inputMethodsList.SelectedItem is AIR_SDK.GameConfig.InputDevices.Device)
                    {
                        AIR_SDK.GameConfig.InputDevices.Device device = inputMethodsList.SelectedItem as AIR_SDK.GameConfig.InputDevices.Device;
                        if (device.HasDeviceNames)
                        {
                            if (refreshItems)
                            {
                                foreach (var name in device.DeviceNames)
                                {
                                    inputDeviceNamesList.Items.Add(name);
                                }
                            }
                            ToggleDeviceNamesUI(true);
                        }
                        else
                        {
                            inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_UNSUPPORTED") == null ? "" : Program.LanguageResource.GetString("Input_UNSUPPORTED")));
                            ToggleDeviceNamesUI(false);
                        }
                    }
                }
            }
        }

        private void LaunchSystemGamepadSettings()
        {
            Process.Start("joy.cpl");

        }

        #endregion

        #region Legacy Mod Management

        private void UpdateModsListLegacy(bool FullReload = false)
        {
            if (FullReload) FetchModsLegacy();
            RefreshSelectedMobProperties();
        }

        private void SaveLegacy()
        {
            foreach (var mod in ModsList)
            {
                UpdateMods(mod.Source);
            }
            UpdateModsList(true);

            void UpdateMods(AIR_SDK.Mod item)
            {
                if (item.IsEnabled != item.EnabledLocal)
                {
                    if (item.IsEnabled == true) EnableModLegacy(item);
                    else DisableModLegacy(item);
                }
            }
        }

        private void FetchModsLegacy()
        {
            ModsList.Clear();
            ModsList = new List<ModViewerItem>();
            GetModsCheckStateLegacy();
            UpdateNewModsListItems();

        }

        private void GetModsCheckStateLegacy()
        {
            DirectoryInfo d = new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            foreach (DirectoryInfo folder in folders)
            {
                DirectoryInfo f = new DirectoryInfo(folder.FullName);
                var root = f.GetFiles("mod.json").FirstOrDefault();
                AIR_SDK.Mod mod;
                if (root != null)
                {
                    try
                    {
                        mod = new AIR_SDK.Mod(root);
                        if (mod != null)
                        {
                            if (folder.Name.Contains("#"))
                            {
                                mod.IsEnabled = false;
                                mod.EnabledLocal = false;
                                ModsList.Add(new ModViewerItem(mod));
                            }
                            else
                            {
                                mod.IsEnabled = true;
                                mod.EnabledLocal = true;
                                ModsList.Add(new ModViewerItem(mod));
                            }
                        }
                    }
                    catch (Newtonsoft.Json.JsonReaderException ex)
                    {
                        MessageBox.Show(UserLanguage.LegacyModError1(folder.Name, ex.Message));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(UserLanguage.LegacyModError2(folder.Name, ex.Message));
                    }


                }



            }
        }

        private void DisableModLegacy(AIR_SDK.Mod mod)
        {
            try
            {
                string result = ProgramPaths.Sonic3AIRModsFolder + "\\" + "#" + mod.FolderName.Replace("#", "");
                Directory.Move(mod.FolderPath, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + Program.LanguageResource.GetString("PleaseRefreshTheModList"));
            }
        }

        private void EnableModLegacy(AIR_SDK.Mod mod)
        {
            try
            {
                string result = ProgramPaths.Sonic3AIRModsFolder + "\\" + mod.FolderName.Replace("#", "");
                Directory.Move(mod.FolderPath, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + Program.LanguageResource.GetString("PleaseRefreshTheModList"));
            }
        }

        #endregion

        #region Modern Mod Management

        private void UpdateModsList(bool FullReload = false)
        {
            ModViewer.ItemCheck = null;
            if (!Properties.Settings.Default.EnableNewLoaderMethod) UpdateModsListLegacy(FullReload);
            else
            {

                if (FullReload) FetchMods();
                else
                {
                    UpdateNewModsListItems();
                }
                RefreshSelectedMobProperties();
            }
            ModViewer.ItemCheck = ModsList_ItemCheck;
        }

        private void UpdateNewModsListItems()
        {
            ProgramPaths.ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref S3AIRSettings);
            ModViewer.View.Items.Clear();
            foreach (ModViewerItem mod in ModsList)
            {
                ModViewer.View.Items.Add(mod);
            }
            ModViewer.View.Items.Refresh();

        }

        private void FetchMods()
        {
            ModsList.Clear();
            ModsList = new List<ModViewerItem>();
            EnableAllLegacyDisabledMods();
            GetModsCheckState();
            UpdateNewModsListItems();
        }

        private void GetModsCheckState()
        {
            DirectoryInfo d = new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            IList<Tuple<AIR_SDK.Mod, int>> ActiveMods = new List<Tuple<AIR_SDK.Mod, int>>();
            foreach (DirectoryInfo folder in folders)
            {
                DirectoryInfo f = new DirectoryInfo(folder.FullName);
                var root = f.GetFiles("mod.json").FirstOrDefault();
                AIR_SDK.Mod mod;
                if (root != null)
                {
                    try
                    {
                        mod = new AIR_SDK.Mod(root);
                        if (S3AIRActiveMods.ActiveMods.Contains(mod.FolderName))
                        {
                            mod.IsEnabled = true;
                            mod.EnabledLocal = true;
                            ActiveMods.Add(new Tuple<AIR_SDK.Mod, int>(mod, S3AIRActiveMods.ActiveMods.IndexOf(mod.FolderName)));
                        }
                        else
                        {
                            mod.IsEnabled = false;
                            mod.EnabledLocal = false;
                            ModsList.Add(new ModViewerItem(mod));
                        }
                    }
                    catch (Newtonsoft.Json.JsonReaderException ex)
                    {
                        MessageBox.Show(UserLanguage.LegacyModError1(folder.Name, ex.Message));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(UserLanguage.LegacyModError2(folder.Name, ex.Message));
                    }


                }
            }
            foreach (var enabledMod in ActiveMods.OrderBy(x => x.Item2).ToList())
            {
                ModsList.Insert(0, new ModViewerItem(enabledMod.Item1));
            }

        }

        private void MoveModToTop()
        {
            int index = ModList.SelectedIndex;
            if (index != 0)
            {
                ModsList.Move(index, 0);
                UpdateModsList();
                ModList.SelectedIndex = 0;
            }
        }

        private void MoveModUp()
        {
            int index = ModList.SelectedIndex;
            if (index != 0)
            {
                ModsList.Move(index, index - 1);
                UpdateModsList();
                ModList.SelectedIndex = index - 1;
            }
        }
        private void MoveModDown()
        {
            int index = ModList.SelectedIndex;
            if (index != ModsList.Count - 1)
            {
                ModsList.Move(index, index + 1);
                UpdateModsList();
                ModList.SelectedIndex = index + 1;
            }
        }

        private void MoveModToBottom()
        {
            int index = ModList.SelectedIndex;
            if (index != ModsList.Count - 1)
            {
                ModsList.Move(index, ModsList.Count - 1);
                UpdateModsList();
                ModList.SelectedIndex = ModsList.Count - 1;
            }
        }

        private void Save()
        {
            if (!Properties.Settings.Default.EnableNewLoaderMethod) SaveLegacy();
            else
            {
                foreach (var mod in ModsList)
                {
                    UpdateMods(mod.Source);
                }
                S3AIRActiveMods.Save(ModsList.Where(x => x.IsEnabled).Select(x => x.Source.FolderName).Reverse().ToList());
                UpdateModsList(true);

                void UpdateMods(AIR_SDK.Mod item)
                {
                    if (item.IsEnabled != item.EnabledLocal)
                    {
                        if (item.IsEnabled == true) EnableMod(item);
                        else DisableMod(item);
                    }
                }
            }
        }

        private void DisableMod(AIR_SDK.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Remove(mod.FolderName);
        }

        private void EnableMod(AIR_SDK.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Add(mod.FolderName);
        }

        private void EnableAllLegacyDisabledMods()
        {
            DirectoryInfo d = new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            List<string> DisabledFolders = new List<string>();
            foreach (DirectoryInfo folder in folders)
            {
                DirectoryInfo f = new DirectoryInfo(folder.FullName);
                var root = f.GetFiles("mod.json").FirstOrDefault();
                if (root != null)
                {
                    if (folder.Name.Contains("#")) DisabledFolders.Add(folder.Name);
                }
            }

            foreach (string folder in DisabledFolders)
            {
                string destination = ProgramPaths.Sonic3AIRModsFolder + "\\" + folder.Replace("#", "");
                string source = ProgramPaths.Sonic3AIRModsFolder + "\\" + folder;
                Directory.Move(source, destination);
            }
        }


        #endregion

        #region Launching Events

        private void AddRemoveURLHandlerButton_Click(object sender, EventArgs e)
        {
            string ModLoaderPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string InstallerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "//GameBanana API Installer.exe";
            Process.Start($"\"{InstallerPath}\"", $"\"{ModLoaderPath}\"");
        }

        private void OpenEXEFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                string filename = ProgramPaths.Sonic3AIRPath;
                Process.Start(Path.GetDirectoryName(filename));
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    string filename = ProgramPaths.Sonic3AIRPath;
                    Process.Start(Path.GetDirectoryName(filename));
                }
            }
        }

        private void OpenAppDataFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIRAppDataFolder);
        }

        private void OpenModsFolder()
        {
            Process.Start(ProgramPaths.Sonic3AIRModsFolder);
        }

        private void OpenSelectedModFolder(ModViewerItem mod)
        {
            Process.Start(mod.Source.FolderPath);
        }

        private void OpenConfigFile()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                string filename = ProgramPaths.Sonic3AIRPath;
                if (File.Exists(Path.GetDirectoryName(filename) + "//config.json"))
                {
                    Process.Start(Path.GetDirectoryName(filename) + "//config.json");
                }
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    string filename = ProgramPaths.Sonic3AIRPath;
                    if (File.Exists(Path.GetDirectoryName(filename) + "//config.json"))
                    {
                        Process.Start(Path.GetDirectoryName(filename) + "//config.json");
                    }
                }
            }
        }

        private void OpenLogFile()
        {
            if (File.Exists(ProgramPaths.Sonic3AIRAppDataFolder + "//logfile.txt"))
            {
                Process.Start(ProgramPaths.Sonic3AIRAppDataFolder + "//logfile.txt");
            }
            else
            {
                MessageBox.Show($"{Program.LanguageResource.GetString("LogFileNotFound")}: {nL}{ProgramPaths.Sonic3AIRAppDataFolder}\\logfile.txt");
            }

        }

        private void OpenModdingTemplatesFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(ProgramPaths.Sonic3AIRModdingTemplatesFolder);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ProgramPaths.ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(ProgramPaths.Sonic3AIRModdingTemplatesFolder);
                }
            }
        }

        private void OpenSampleModsFolder()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRSampleModsFolderPath()) Process.Start(ProgramPaths.Sonic3AIRSampleModsFolder);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ProgramPaths.ValidateSonic3AIRSampleModsFolderPath()) Process.Start(ProgramPaths.Sonic3AIRSampleModsFolder);
                }
            }
        }

        private void OpenUserManual()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRUserManualFilePath()) Process.Start(ProgramPaths.Sonic3AIRUserManualFile);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ProgramPaths.ValidateSonic3AIRUserManualFilePath()) Process.Start(ProgramPaths.Sonic3AIRUserManualFile);
                }
            }
        }

        private void OpenModDocumentation()
        {
            if (ProgramPaths.Sonic3AIRPath != null || ProgramPaths.Sonic3AIRPath != "")
            {
                if (ProgramPaths.ValidateSonic3AIRModDocumentationFilePath()) Process.Start(ProgramPaths.Sonic3AIRModDocumentationFile);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ProgramPaths.ValidateSonic3AIRModDocumentationFilePath()) Process.Start(ProgramPaths.Sonic3AIRModDocumentationFile);
                }
            }
        }

        private void OpenModURL(string url)
        {
            Process.Start(url);
        }

        private void OpenSettingsFile()
        {
            Process.Start(ProgramPaths.Sonic3AIRAppDataFolder + "//settings.json");
        }

        private void OpenRecordingLocation()
        {
            if (GameRecordingList.SelectedItem != null)
            {
                AIR_SDK.Recording item = GameRecordingList.SelectedItem as AIR_SDK.Recording;
                if (File.Exists(item.FilePath))
                {
                    Process.Start("explorer.exe", "/select, " + item.FilePath);
                }
            }

        }

        #endregion

        #region Information Sending

        private async void UploadRecordingToFileDotIO(AIR_SDK.Recording recording)
        {
            string expires = "/?expires=1w";
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://file.io" + expires))
                {
                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(recording.FilePath)), "file", Path.GetFileName(recording.FilePath));
                    request.Content = multipartContent;

                    var response = await httpClient.SendAsync(request);
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    string url = jsonObj.link;

                    string message = UserLanguage.RecordingUploaded(url);
                    Clipboard.SetText(url);
                    MessageBox.Show(message);

                }
            }
        }

        #endregion

        #region Protocol Handler

        public void CreateGameBananaShortcut()
        {
            ProgramPaths.Sonic3AIRGBLinkPath = ProgramPaths.Sonic3AIRAppDataFolder + "//AIRModLoader.lnk";
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = shell.CreateShortcut(ProgramPaths.Sonic3AIRGBLinkPath) as IWshRuntimeLibrary.IWshShortcut;
            shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            shortcut.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            shortcut.Save();
        }




        #endregion

        #region AIR EXE Version Handler Toolstrip / Path Management

        private void AIRVersionZIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstallVersionFromZIP();
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
                            installedVersionsToolStripMenuItem.DropDownItems.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath));
                        }


                    }
                }

            }

        }

        private void CleanUpInstalledVersionsToolStrip()
        {
            foreach (var item in installedVersionsToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Click -= ChangeAIRPathByInstalls;
            }
            installedVersionsToolStripMenuItem.DropDownItems.Clear();
        }

        private ToolStripItem GenerateInstalledVersionsToolstripItem(string name, string filepath)
        {
            ToolStripItem item = new ToolStripMenuItem();
            item.Text = name;
            item.Tag = filepath;
            item.Click += ChangeAIRPathByInstalls;
            return item;
        }

        private void ChangeAIRPathByInstalls(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ProgramPaths.Sonic3AIRPath = item.Tag.ToString();
            Properties.Settings.Default.Save();
            UpdateAIRSettings();
        }

        private void ChangeAIRPathFromSettings()
        {
            if (S3AIRSettings != null)
            {
                if (S3AIRSettings.HasEXEPath)
                {
                    if (File.Exists(S3AIRSettings.AIREXEPath))
                    {
                        ProgramPaths.Sonic3AIRPath = S3AIRSettings.AIREXEPath;
                        Properties.Settings.Default.Save();
                        UpdateAIRSettings();
                    }
                    else
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("AIRChangePathNoLongerExists"));
                    }
                }
            }
        }

        private void InstallVersionFromZIP()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("SonicAIRVersionZIP")} (*.zip)|*.zip",
                Title = Program.LanguageResource.GetString("SelectSonicAIRVersionZIP")
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM\\downloads";
                string output = destination;

                using (var archive = SharpCompress.Archives.Zip.ZipArchive.Open(ofd.FileName))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        entry.WriteToDirectory(output, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }


                string metaDataFile = Directory.GetFiles(destination, "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                AIR_SDK.VersionMetadata ver = new AIR_SDK.VersionMetadata(new FileInfo(metaDataFile));


                string output2 = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Sonic3AIR_MM\\air_versions\\{ver.VersionString}";

                Directory.Move(destination, output2);

                Directory.CreateDirectory(destination);

                MessageBox.Show(UserLanguage.VersionInstalled(output2));
            }


        }



        #endregion

        #region A.I.R. Version Manager List

        private void RefreshVersionsList(bool fullRefresh = false)
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
                            VersionsListView.Items.Add(new AIRVersionListItem(folder.Name, folder.FullName));
                        }


                    }
                }
                UpdateWPFListViews(VersionsListView);
            }

            bool enabled = VersionsListView.SelectedItem != null;
            removeVersionButton.Enabled = enabled;
            openVersionLocationButton.Enabled = enabled;
        }

        private class AIRVersionListItem
        {
            public string Name { get { return _name; } }
            private string _name;

            public string FilePath { get { return _filePath; } }
            private string _filePath;

            public override string ToString()
            {
                return $"{Program.LanguageResource.GetString("Version")} {Name}";
            }

            public AIRVersionListItem(string name, string filePath)
            {
                _name = name;
                _filePath = filePath;
            }
        }

        private void VersionsListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshVersionsList();
        }

        private void TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (settingsTabControl.SelectedTab == versionsPage)
            {
                RefreshVersionsList(true);
            }
            else if (settingsTabControl.SelectedTab == inputPage)
            {
                DisableMappings();
                CollectInputMappings();
            }
        }

        private void OpenVersionLocationButton_Click(object sender, EventArgs e)
        {
            if (VersionsListView.SelectedItem != null && VersionsListView.SelectedItem is AIRVersionListItem)
            {
                AIRVersionListItem item = VersionsListView.SelectedItem as AIRVersionListItem;
                Process.Start(item.FilePath);
            }
        }

        private void RemoveVersionButton_Click(object sender, EventArgs e)
        {
            if (VersionsListView.SelectedItem != null && VersionsListView.SelectedItem is AIRVersionListItem)
            {
                AIRVersionListItem item = VersionsListView.SelectedItem as AIRVersionListItem;
                if (MessageBox.Show(UserLanguage.RemoveVersion(item.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        ModFileManagement.WipeFolderContents(item.FilePath);
                        Directory.Delete(item.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("UnableToRemoveVersion"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshVersionsList(true);
                }

            }
        }




        #endregion

        private void ModManager_Resize(object sender, EventArgs e)
        {

        }
    }
    */
}
