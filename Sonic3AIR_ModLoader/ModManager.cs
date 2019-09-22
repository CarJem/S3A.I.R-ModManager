using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
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




namespace Sonic3AIR_ModLoader
{

    public partial class ModManager : Form
    {
        #region Variables




        public string nL = Environment.NewLine;
        public static AIR_SDK.Settings S3AIRSettings;
        public static ModManager Instance;
        public static AIR_SDK.ActiveModsList S3AIRActiveMods;
        public static AIR_SDK.GameConfig GameConfig;
        public static AIR_SDK.VersionMetadata CurrentAIRVersion;
        IList<AIR_SDK.Mod> ModsList = new List<AIR_SDK.Mod>();
        bool AuthorizeCheck { get; set; }
        bool AllowUpdate { get; set; } = true; 
        #endregion

        #region Initialize Methods
        public ModManager(bool autoBoot = false)
        {
            Instance = this;
            if (Properties.Settings.Default.AutoUpdates)
            {
                if (autoBoot == false && Program.UpdaterState == Updater.UpdateState.NeverStarted) new Updater();
            }

            StartModloader(autoBoot);

        }

        public ModManager(string gamebanana_api)
        {
            Instance = this;
            StartModloader(false, gamebanana_api);

        }

        private void StartModloader(bool autoBoot = false, string gamebanana_api = "")
        {
            InitializeComponent();
            if (ValidateInstall() == true)
            {
                UserLanguage.ApplyLanguage(this);
                SetTooltips();
                UpdateModsList(true);
                UpdateUI();
                if (autoBoot) GameHandler.LaunchSonic3AIR();
                if (gamebanana_api != "") GamebananaAPI_Install(gamebanana_api);
            }
            else
            {
                Environment.Exit(0);
            }

        }
        #endregion

        #region Events
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
            OpenModURL((ModList.SelectedItem as AIR_SDK.Mod).URL);
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
            if (gameRecordingList.SelectedItem != null)
            {
                AIR_SDK.Recording recording = gameRecordingList.SelectedItem as AIR_SDK.Recording;
                if (MessageBox.Show(UserLanguage.DeleteItem(recording.Name), "", MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(recording.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(UserLanguage.UnableToDeleteFile, "",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }

                    CollectGameRecordings();
                    GameRecordingList_SelectedIndexChanged(null, null);
                }
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RemoveModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                RemoveMod();
            }
        }

        private void OpenModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                OpenSelectedModFolder(ModList.SelectedItem as AIR_SDK.Mod);
            }
        }

        private void ModsList_MouseClick(object sender, MouseEventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    modContextMenuStrip.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void ModList_MouseDown(object sender, MouseEventArgs e)
        {
            Point loc = this.ModList.PointToClient(Cursor.Position);
            for (int i = 0; i < this.ModList.Items.Count; i++)
            {
                Rectangle rec = this.ModList.GetItemRectangle(i);
                rec.Width = 16; //checkbox itself has a default width of about 16 pixels

                if (rec.Contains(loc))
                {
                    AuthorizeCheck = true;
                    bool newValue = !this.ModList.GetItemChecked(i);
                    this.ModList.SetItemChecked(i, newValue);//check 
                    AuthorizeCheck = false;

                    return;
                }
            }
        }

        private void AddMods_Click(object sender, EventArgs e)
        {
            AddMod();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (ModList.SelectedItem != null)
            {
                RemoveMod();
            }
        }
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            if (gameRecordingList.SelectedItem != null)
            {
                var item = gameRecordingList.SelectedItem as AIR_SDK.Recording;
                Clipboard.SetText(item.FilePath);
                MessageBox.Show(UserLanguage.RecordingPathCopiedToClipboard);
            }
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (gameRecordingList.SelectedItem != null)
            {
                UploadRecordingToFileDotIO(gameRecordingList.SelectedItem as AIR_SDK.Recording);
            }

        }

        private void GameRecordingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameRecordingList.SelectedItem != null)
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

        private void ModsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!AuthorizeCheck)
            {
                e.NewValue = e.CurrentValue; //check state change was not through authorized actions
            }
            else
            {
                var item = (ModList.SelectedItem as AIR_SDK.Mod);
                item.IsEnabled = (e.NewValue == CheckState.Checked);
                UpdateModsList();
            }
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
        private void SetTooltips()
        {
            new ToolTip().SetToolTip(addMods, UserLanguage.AddAMod);
            new ToolTip().SetToolTip(removeButton, UserLanguage.RemoveSelectedMod);
            new ToolTip().SetToolTip(moveUpButton, UserLanguage.IncreaseModPriority);
            new ToolTip().SetToolTip(moveDownButton, UserLanguage.DecreaseModPriority);
            new ToolTip().SetToolTip(moveToTopButton, UserLanguage.IncreaseModPriorityToMax);
            new ToolTip().SetToolTip(moveToBottomButton, UserLanguage.DecreaseModPriorityToMin);

            aboutLabel.Text = aboutLabel.Text.Replace("{version}", Program.Version);
            this.Text = this.Text.Replace("{version}", Program.Version);
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
                Filter = $"{UserLanguage.Sonic3KRomFile} (*.bin)|*.bin",
                InitialDirectory = Path.GetDirectoryName(S3AIRSettings.Sonic3KRomPath),
                Title = UserLanguage.SelectSonic3KRomFile

            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                S3AIRSettings.Sonic3KRomPath = fileDialog.FileName;
                S3AIRSettings.SaveSettings();
            }
            UpdateAIRSettings();
        }

        private enum S3AIRSetting : int
        {
            FailSafeMode = 0,
            FixGlitches = 1 
        }

        private void UpdateBoolSettings(S3AIRSetting setting, bool isChecked)
        {
            if (setting == S3AIRSetting.FailSafeMode)
            {
                S3AIRSettings.FailSafeMode = isChecked;
            }
            else
            {
                S3AIRSettings.FixGlitches = isChecked;
            }
            S3AIRSettings.SaveSettings();
        }

        private void UpdateAIRSettings()
        {
            sonic3AIRPathBox.Text = ProgramPaths.Sonic3AIRPath;
            romPathBox.Text = S3AIRSettings.Sonic3KRomPath;
            fixGlitchesCheckbox.Checked = S3AIRSettings.FixGlitches;
            failSafeModeCheckbox.Checked = S3AIRSettings.FailSafeMode;

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
                            airVersionLabel.Text = $"{UserLanguage.AIRVersion}: {CurrentAIRVersion.VersionString}";
                        }
                        else
                        {
                            Properties.Settings.Default.EnableNewLoaderMethod = false;
                            enableModStackingToolStripMenuItem.Enabled = false;
                            airVersionLabel.Text = $"{UserLanguage.AIRVersion}: {CurrentAIRVersion.VersionString}";
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
                airVersionLabel.Text = $"{UserLanguage.AIRVersion}: NULL";
            }

        }

        public void RefreshSelectedMobProperties()
        {
            if (ModList.SelectedItem != null)
            {
                if (Properties.Settings.Default.EnableNewLoaderMethod)
                {
                    moveUpButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as AIR_SDK.Mod)) > 0);
                    moveDownButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as AIR_SDK.Mod)) < ModsList.Count - 1);
                    moveToTopButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as AIR_SDK.Mod)) > 0);
                    moveToBottomButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as AIR_SDK.Mod)) < ModsList.Count - 1);
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
                openModURLToolStripMenuItem.Enabled = ((ModList.SelectedItem as AIR_SDK.Mod).URL != null);
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
                AIR_SDK.Mod item = ModList.SelectedItem as AIR_SDK.Mod;
                if (item != null)
                {

                    modNameLabel.Text = item.Name;
                    modTechnicalNameLabel.Text = item.TechnicalName;

                    modInfoTextBox.Text = "";

                    modInfoTextBox.SelectionFont = new Font(modInfoTextBox.Font, FontStyle.Bold);

                    modInfoTextBox.AppendText($"{UserLanguage.By}: {item.Author}{nL}{UserLanguage.Version}: {item.ModVersion}{nL}{UserLanguage.AIRVersion}: {item.GameVersion}");

                    modInfoTextBox.SelectionFont = new Font(modInfoTextBox.Font, FontStyle.Regular);

                    modInfoTextBox.AppendText($"{nL}{nL}{item.Description}");
                }
                else
                {
                    modNameLabel.Text = "";
                    modTechnicalNameLabel.Text = "";
                    modInfoTextBox.Text = "";
                }
            }
            else
            {
                modNameLabel.Text = "";
                modTechnicalNameLabel.Text = "";
                modInfoTextBox.Text = "";
            }
        }
        #endregion

        #region Information Retriving

        private void CollectGameRecordings()
        {
            gameRecordingList.Items.Clear();
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
                        gameRecordingList.Items.Add(recording);
                    }
                }
            }
            else
            {
                recordingsErrorMessage.BringToFront();
                recordingsErrorMessage.Visible = true;
                recordingsErrorMessage.Enabled = false;
            }

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
                if (situation == 0) inputErrorMessage.Text = UserLanguage.InputMappingError1;
                else if (situation == 1) inputErrorMessage.Text = UserLanguage.InputMappingError2;
                else if (situation == 2) inputErrorMessage.Text = UserLanguage.InputMappingError3;

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
            return ProgramPaths.ValidateInstall(ref S3AIRActiveMods,ref S3AIRSettings);
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
                        aInputButton.Text = (device.A.Count() > 1 ? UserLanguage.Input_MULTI : device.A.FirstOrDefault());
                        bInputButton.Text = (device.B.Count() > 1 ? UserLanguage.Input_MULTI : device.B.FirstOrDefault());
                        xInputButton.Text = (device.X.Count() > 1 ? UserLanguage.Input_MULTI : device.X.FirstOrDefault());
                        yInputButton.Text = (device.Y.Count() > 1 ? UserLanguage.Input_MULTI : device.Y.FirstOrDefault());
                        upInputButton.Text = (device.Up.Count() > 1 ? UserLanguage.Input_MULTI : device.Up.FirstOrDefault());
                        downInputButton.Text = (device.Down.Count() > 1 ? UserLanguage.Input_MULTI : device.Down.FirstOrDefault());
                        leftInputButton.Text = (device.Left.Count() > 1 ? UserLanguage.Input_MULTI : device.Left.FirstOrDefault());
                        rightInputButton.Text = (device.Right.Count() > 1 ? UserLanguage.Input_MULTI : device.Right.FirstOrDefault());
                        startInputButton.Text = (device.Start.Count() > 1 ? UserLanguage.Input_MULTI : device.Start.FirstOrDefault());
                        backInputButton.Text = (device.Back.Count() > 1 ? UserLanguage.Input_MULTI : device.Back.FirstOrDefault());

                        if (aInputButton.Text == "") aInputButton.Text = UserLanguage.Input_NONE;
                        if (bInputButton.Text == "") bInputButton.Text = UserLanguage.Input_NONE;
                        if (xInputButton.Text == "") xInputButton.Text = UserLanguage.Input_NONE;
                        if (yInputButton.Text == "") yInputButton.Text = UserLanguage.Input_NONE;
                        if (upInputButton.Text == "") upInputButton.Text = UserLanguage.Input_NONE;
                        if (downInputButton.Text == "") downInputButton.Text = UserLanguage.Input_NONE;
                        if (leftInputButton.Text == "") leftInputButton.Text = UserLanguage.Input_NONE;
                        if (rightInputButton.Text == "") rightInputButton.Text = UserLanguage.Input_NONE;
                        if (startInputButton.Text == "") startInputButton.Text = UserLanguage.Input_NONE;
                        if (backInputButton.Text == "") backInputButton.Text = UserLanguage.Input_NONE;

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
            aInputButton.Text = UserLanguage.Input_NULL;
            bInputButton.Text = UserLanguage.Input_NULL;
            xInputButton.Text = UserLanguage.Input_NULL;
            yInputButton.Text = UserLanguage.Input_NULL;
            upInputButton.Text = UserLanguage.Input_NULL;
            downInputButton.Text = UserLanguage.Input_NULL;
            leftInputButton.Text = UserLanguage.Input_NULL;
            rightInputButton.Text = UserLanguage.Input_NULL;
            startInputButton.Text = UserLanguage.Input_NULL;
            backInputButton.Text = UserLanguage.Input_NULL;
            inputDeviceNamesList.Items.Add(UserLanguage.Input_NULL);
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
                DialogResult result = ShowInputDialog(ref newDevice, UserLanguage.AddNewDeviceTitle, UserLanguage.AddNewDeviceDescription);
                GameConfig.Input.Devices[inputMethodsList.SelectedIndex].DeviceNames.Add(newDevice);
                UpdateInputMappings();

            }

        }

        private void RemoveInputDeviceName()
        {
            if (inputMethodsList.SelectedItem != null && inputDeviceNamesList.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(inputDeviceNamesList.SelectedItem.ToString()), UserLanguage.DeleteDeviceTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
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
                            inputDeviceNamesList.Items.Add(UserLanguage.Input_UNSUPPORTED);
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

        #region File Management

        private void AddMod()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Sonic 3 AIR Mod (*.zip;*.7z;*.rar)|*.zip;*.7z;*.rar",
                Title = "Select Compressed Mod File..."
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddMod(ofd.FileName);
            }
        }

        private void AddMod(string file)
        {
            //Find the Root of the Mod in the Zip, Because some people have a folder inside of the zip, others may not
            string foundFile = "";

            if (Path.GetExtension(file) == ".rar") ExtractRar(file);
            else if (Path.GetExtension(file) == ".zip") ExtractZip(file);
            else if (Path.GetExtension(file) == ".7z") Extract7Zip(file);

            foreach (string d in Directory.GetDirectories(ProgramPaths.Sonic3AIR_MM_TempModsFolder))
            {
                var item = Directory.GetFiles(d, "mod.json").FirstOrDefault();
                foundFile = (item != null ? item.ToString() : "");
            }
            if (foundFile != "")
            {
                Directory.Move(System.IO.Path.GetDirectoryName(foundFile), ProgramPaths.Sonic3AIRModsFolder + "\\" + Path.GetFileNameWithoutExtension(file));
            }
            else
            {
                MessageBox.Show("This is not a valid Sonic 3 A.I.R. Mod. A valid mod requires a mod.json, and either this isn't a mod or it's a legacy mod. If you know for sure that this is a mod, then it's probably a legacy mod. You can't use legacy mods that work without them going forward.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CleanUpTempModsFolder();
            UpdateModsList(true);
        }


        public void ExtractRar(string file)
        {
            using (var archive = SharpCompress.Archives.Rar.RarArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(ProgramPaths.Sonic3AIR_MM_TempModsFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        public void ExtractZip(string file)
        {
            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(ProgramPaths.Sonic3AIR_MM_TempModsFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        public void Extract7Zip(string file)
        {
            using (var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(ProgramPaths.Sonic3AIR_MM_TempModsFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        private void RemoveMod()
        {
            var modToRemove = ModList.SelectedItem as AIR_SDK.Mod;
            if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(modToRemove.FolderPath);
                Directory.Delete(modToRemove.FolderPath);
                UpdateModsList(true);
                
            }


        }

        private void CleanUpTempModsFolder()
        {
            WipeFolderContents(ProgramPaths.Sonic3AIR_MM_TempModsFolder);
        }

        private void WipeFolderContents(string folder)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
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
                UpdateMods(mod);
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
            ModList.ItemCheck -= ModsList_ItemCheck;
            ModsList.Clear();
            ModsList = new List<AIR_SDK.Mod>();
            GetModsCheckStateLegacy();
            ModList.DataSource = ModsList;
            ModList.DisplayMember = "Name";
            ModList.ValueMember = "IsEnabled";
            for (int i = 0; i < ModList.Items.Count; i++)
            {
                AIR_SDK.Mod obj = (AIR_SDK.Mod)ModList.Items[i];
                ModList.SetItemChecked(i, obj.IsEnabled);
            }
            ModList.ItemCheck += ModsList_ItemCheck;

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
                                ModsList.Add(mod);
                            }
                            else
                            {
                                mod.IsEnabled = true;
                                mod.EnabledLocal = true;
                                ModsList.Add(mod);
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
                MessageBox.Show(ex.Message + Environment.NewLine + UserLanguage.PleaseRefreshTheModList);
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
                MessageBox.Show(ex.Message + Environment.NewLine + UserLanguage.PleaseRefreshTheModList);
            }
        }

        #endregion

        #region Modern Mod Management

        private void UpdateModsList(bool FullReload = false)
        {
            if (!Properties.Settings.Default.EnableNewLoaderMethod) UpdateModsListLegacy(FullReload);
            else
            {

                if (FullReload) FetchMods();
                else
                {
                    ModList.ItemCheck -= ModsList_ItemCheck;
                    ModList.DataSource = null;
                    ModList.DataSource = ModsList;
                    ModList.DisplayMember = "Name";
                    ModList.ValueMember = "IsEnabled";
                    for (int i = 0; i < ModList.Items.Count; i++)
                    {
                        AIR_SDK.Mod obj = (AIR_SDK.Mod)ModList.Items[i];
                        ModList.SetItemChecked(i, obj.IsEnabled);
                    }
                    ModList.ItemCheck += ModsList_ItemCheck;
                }
                RefreshSelectedMobProperties();
            }
        }

        private void FetchMods()
        {
            ModList.ItemCheck -= ModsList_ItemCheck;
            ModsList.Clear();
            ModsList = new List<AIR_SDK.Mod>();
            EnableAllLegacyDisabledMods();
            GetModsCheckState();
            ModList.DataSource = ModsList;
            ModList.DisplayMember = "Name";
            ModList.ValueMember = "IsEnabled";
            for (int i = 0; i < ModList.Items.Count; i++)
            {
                AIR_SDK.Mod obj = (AIR_SDK.Mod)ModList.Items[i];
                ModList.SetItemChecked(i, obj.IsEnabled);
            }
            ModList.ItemCheck += ModsList_ItemCheck;

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
                            ModsList.Add(mod);
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
                ModsList.Insert(0, enabledMod.Item1);
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
                    UpdateMods(mod);
                }
                S3AIRActiveMods.Save(ModsList.Where(x => x.IsEnabled).Select(x => x.FolderName).Reverse().ToList());
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

        private void OpenSelectedModFolder(AIR_SDK.Mod mod)
        {
            Process.Start(mod.FolderPath);
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
                MessageBox.Show($"{UserLanguage.LogFileNotFound}: {nL}{ProgramPaths.Sonic3AIRAppDataFolder}\\logfile.txt");
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
            if (gameRecordingList.SelectedItem != null)
            {
                AIR_SDK.Recording item = gameRecordingList.SelectedItem as AIR_SDK.Recording;
                if (File.Exists(item.FilePath))
                {
                    Process.Start("explorer.exe", "/select, " + item.FilePath);
                }
            }

        }

        #endregion

        #region Downloading

        public void GamebananaAPI_Install(string data)
        {
            string url = data.Replace("s3airmm://","");
            if (url == "") MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else DownloadMod(url);
        }

        public void DownloadMod(string url, bool isMod = true, bool backgroundDownload = false)
        {
            string baseURL = GetBaseURL(url);
            MessageBox.Show(baseURL);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (url != "") remote_filename = GetRemoteFileName(url);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            DownloadWindow downloadWindow = new DownloadWindow($"{UserLanguage.Downloading} \"{filename}\"", url, $"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}\\{filename}");
            Action finishAction = DownloadModCompleted;
            if (isMod) downloadWindow.DownloadCompleted = finishAction;
            if (backgroundDownload) downloadWindow.StartBackground();
            else downloadWindow.Start();
        }

        private string GetRemoteFileName(string baseURL)
        {
            Uri uri = new Uri(baseURL);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private string GetBaseURL(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;  // IMPORTANT

            webRequest.Timeout = 10000;           // timeout 10s
            webRequest.Method = "HEAD";
            // Get the response ...
            HttpWebResponse webResponse;
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                {
                    string uriString = webResponse.Headers["Location"];
                    return uriString;
                }
            }
            return "";
        }

        private void DownloadModCompleted()
        {
            string file = Directory.GetFiles($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".zip"));
            AddMod(file);
        }

        private void DownloadButtonTest_Click(object sender, EventArgs e)
        {    
            string url = "";
            if (ShowInputDialog(ref url, UserLanguage.EnterModURL) == DialogResult.OK)
            {
                if (url != "") MessageBox.Show(UserLanguage.InvalidURL, UserLanguage.InvalidURL, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show(UserLanguage.InvalidURL, UserLanguage.InvalidURL, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else DownloadMod(url, false);
            }

        }

        private DialogResult ShowInputDialog(ref string input, string caption, string message = "")
        {
            System.Drawing.Size size = new System.Drawing.Size(500, 75);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = caption;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.Label label = new Label();
            label.Size = new System.Drawing.Size(size.Width - 10, 13);
            label.Location = new System.Drawing.Point(5, 5);
            label.Text = message;
            inputBox.Controls.Add(label);

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 25);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = UserLanguage.Ok_Button;
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 49);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = UserLanguage.Cancel_Button;
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 49);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;
            

            inputBox.ShowDialog();
            input = textBox.Text;
            return inputBox.DialogResult;
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
            DirectoryInfo directoryInfo = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
            foreach (var folder in directoryInfo.GetDirectories().ToList().VersionSort().Reverse())
            {
                string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                if (File.Exists(filePath))
                {
                    installedVersionsToolStripMenuItem.DropDownItems.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath));
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
                        MessageBox.Show(UserLanguage.AIRChangePathNoLongerExists);
                    }
                }
            }
        }

        private void InstallVersionFromZIP()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{UserLanguage.SonicAIRVersionZIP} (*.zip)|*.zip",
                Title = UserLanguage.SelectSonicAIRVersionZIP
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
                versionsListBox.Items.Clear();
                DirectoryInfo directoryInfo = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
                foreach (var folder in directoryInfo.GetDirectories().ToList().VersionSort().Reverse())
                {
                    string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                    if (File.Exists(filePath))
                    {
                        versionsListBox.Items.Add(new AIRVersionListItem(folder.Name, folder.FullName));
                    }


                }
            }

            bool enabled = versionsListBox.SelectedItem != null;
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
                return $"{UserLanguage.Version} {Name}";
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
            if (tabControl2.SelectedTab == versionsPage)
            {
                RefreshVersionsList(true);
            }
            else if (tabControl2.SelectedTab == inputPage)
            {
                DisableMappings();
                CollectInputMappings();
            }
        }

        private void OpenVersionLocationButton_Click(object sender, EventArgs e)
        {
            if (versionsListBox.SelectedItem != null && versionsListBox.SelectedItem is AIRVersionListItem)
            {
                AIRVersionListItem item = versionsListBox.SelectedItem as AIRVersionListItem;
                Process.Start(item.FilePath);
            }
        }

        private void RemoveVersionButton_Click(object sender, EventArgs e)
        {
            if (versionsListBox.SelectedItem != null && versionsListBox.SelectedItem is AIRVersionListItem)
            {
                AIRVersionListItem item = versionsListBox.SelectedItem as AIRVersionListItem;
                if (MessageBox.Show(UserLanguage.RemoveVersion(item.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        WipeFolderContents(item.FilePath);
                        Directory.Delete(item.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(UserLanguage.UnableToRemoveVersion, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshVersionsList(true);
                }

            }
        }





        #endregion


    }
}
