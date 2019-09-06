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



namespace Sonic3AIR_ModLoader
{

    public partial class ModManager : Form
    {
        #region Variables

        #region File Path Strings

        public string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string Sonic3AIRAppDataFolder = "";
        public string Sonic3AIRModsFolder = "";
        public string Sonic3AIRActiveModsList = "";
        public string Sonic3AIRTempModsFolder = "";
        public string Sonic3AIRSettingsFile = "";
        public string Sonic3AIRGBLinkPath = "";


        #region AppData Path Strings
        public string Sonic3AIRSampleModsFolder { get => GetSonic3AIRSampleModsFolderPath(); }
        public string Sonic3AIRModDocumentationFile { get => GetSonic3AIRModDocumentationFilePath(); }
        
        public string Sonic3AIRUserManualFile { get => GetSonic3AIRUserManualFilePath(); }

        public string Sonic3AIRModdingTemplatesFolder { get => GetSonic3AIRModdingTemplatesFolderPath(); }

        #endregion

        #region File Path Checkers
        private string GetSonic3AIRModdingTemplatesFolderPath()
        {
            try
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\modding";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\modding";
            }

        }
        private string GetSonic3AIRSampleModsFolderPath()
        {
            try
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\sample-mods";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\sample-mods";
            }

        }
        private string GetSonic3AIRModDocumentationFilePath()
        {
            try
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\doc\\Modding.pdf";
            }
            catch
            {
                return "%S3AIRPath%" + "\\doc\\Modding.pdf";
            }
        }
        private string GetSonic3AIRUserManualFilePath()
        {
            try
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                return Path.GetDirectoryName(filename) + "\\Manual.pdf";
            }
            catch
            {
                return "%S3AIRPath%" + "\\Manual.pdf";
            }

        }
        #endregion

        #region File Path Validators

        private bool ValidateSonic3AIRModdingTemplatesFolderPath()
        {
            if (!Sonic3AIRModdingTemplatesFolder.Contains("%S3AIRPath%"))
            {
                if (Directory.Exists(Sonic3AIRModdingTemplatesFolder)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRModdingTemplatesFolder);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRModdingTemplatesFolder);
                return false;
            }

        }

        private bool ValidateSonic3AIRSampleModsFolderPath()
        {
            if (!Sonic3AIRSampleModsFolder.Contains("%S3AIRPath%"))
            {
                if (Directory.Exists(Sonic3AIRSampleModsFolder)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRSampleModsFolder);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRSampleModsFolder);
                return false;
            }

        }

        private bool ValidateSonic3AIRModDocumentationFilePath()
        {
            if (!Sonic3AIRModDocumentationFile.Contains("%S3AIRPath%"))
            {
                if (File.Exists(Sonic3AIRModDocumentationFile)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRModDocumentationFile);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRModDocumentationFile);
                return false;
            }
        }

        private bool ValidateSonic3AIRUserManualFilePath()
        {
            if (!Sonic3AIRUserManualFile.Contains("%S3AIRPath%"))
            {
                if (File.Exists(Sonic3AIRUserManualFile)) return true;
                else
                {
                    FileDoesNotExistMessageBox(Sonic3AIRUserManualFile);
                    return false;
                }
            }
            else
            {
                AIRPathNotSetMessageBox(Sonic3AIRUserManualFile);
                return false;
            }
        }

        #endregion

        #region File Path Invalid Messages

        private void AIRPathNotSetMessageBox(string text)
        {
            string message = $"Unable to Validate Path: {nL}{text}{nL}Reason: {nL}Sonic 3 A.I.R's Path is not set!";
            MessageBox.Show(message);
        }

        private void FileDoesNotExistMessageBox(string text)
        {
            string message = $"Unable to Validate Path: {nL}{text}{nL}Reason: {nL}Specified file or directory does not exist!";
            MessageBox.Show(message);
        }

        #endregion

        #endregion

        public string nL = Environment.NewLine;
        Sonic3AIRSettings S3AIRSettings;
        public static ModManager Instance;
        Sonic3AIRActiveMods S3AIRActiveMods;
        IList<Sonic3AIRMod> ModsList = new List<Sonic3AIRMod>();
        bool AuthorizeCheck { get; set; }
        bool AllowUpdate { get; set; } = true; 
        #endregion

        #region Initialize Methods
        public ModManager(bool autoBoot = false)
        {
            Instance = this;
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
            if (InitalCollection() == true)
            {
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
            OpenModURL((ModList.SelectedItem as Sonic3AIRMod).URL);
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
                Sonic3AIRRecording recording = gameRecordingList.SelectedItem as Sonic3AIRRecording;
                if (MessageBox.Show($"Are you sure you want to delete \"{recording.Name}\"?", "", MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(recording.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to Delete File!","",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                OpenSelectedModFolder(ModList.SelectedItem as Sonic3AIRMod);
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
            if (tabControl1.SelectedTab == tabPage3)
            {
                CollectGameRecordings();
            }
        }

        private void CopyRecordingFilePath_Click(object sender, EventArgs e)
        {
            if (gameRecordingList.SelectedItem != null)
            {
                var item = gameRecordingList.SelectedItem as Sonic3AIRRecording;
                Clipboard.SetText(item.FilePath);
                MessageBox.Show("Recording File Path Copied to Clipboard!");
            }
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (gameRecordingList.SelectedItem != null)
            {
                UploadRecordingToFileDotIO(gameRecordingList.SelectedItem as Sonic3AIRRecording);
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

        private void UpdateSonic3AIRPathButton_Click(object sender, EventArgs e)
        {
            GameHandler.UpdateSonic3AIRLocation();
            UpdateAIRSettings();
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
                var item = (ModList.SelectedItem as Sonic3AIRMod);
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

        #endregion

        #region Refreshing and Updating
        private void SetTooltips()
        {
            new ToolTip().SetToolTip(addMods, "Add a Mod...");
            new ToolTip().SetToolTip(removeButton, "Remove Selected Mod...");
            new ToolTip().SetToolTip(moveUpButton, "Increase Selected Mod Priority...");
            new ToolTip().SetToolTip(moveDownButton, "Decrease Selected Mod Priority...");
            new ToolTip().SetToolTip(moveToTopButton, "Increase Selected Mod Priority to Max...");
            new ToolTip().SetToolTip(moveToBottomButton, "Decrease Selected Mod Priority to Min...");
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
                Filter = "Sonic 3K ROM File (*.bin)|*.bin",
                InitialDirectory = Path.GetDirectoryName(S3AIRSettings.Sonic3KRomPath),
                Title = "Select Sonic 3K ROM File..."

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
            sonic3AIRPathBox.Text = Properties.Settings.Default.Sonic3AIRPath;
            romPathBox.Text = S3AIRSettings.Sonic3KRomPath;
            fixGlitchesCheckbox.Checked = S3AIRSettings.FixGlitches;
            failSafeModeCheckbox.Checked = S3AIRSettings.FailSafeMode;
        }

        public void RefreshSelectedMobProperties()
        {
            if (ModList.SelectedItem != null)
            {
                if (Properties.Settings.Default.EnableNewLoaderMethod)
                {
                    moveUpButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as Sonic3AIRMod)) > 0);
                    moveDownButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as Sonic3AIRMod)) < ModsList.Count - 1);
                    moveToTopButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as Sonic3AIRMod)) > 0);
                    moveToBottomButton.Enabled = (ModsList.IndexOf((ModList.SelectedItem as Sonic3AIRMod)) < ModsList.Count - 1);
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
                openModURLToolStripMenuItem.Enabled = ((ModList.SelectedItem as Sonic3AIRMod).URL != null);
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
                Sonic3AIRMod item = ModList.SelectedItem as Sonic3AIRMod;
                if (item != null)
                {

                    modNameLabel.Text = item.Name;
                    modTechnicalNameLabel.Text = item.TechnicalName;

                    modInfoTextBox.Text = "";

                    modInfoTextBox.SelectionFont = new Font(modInfoTextBox.Font, FontStyle.Bold);

                    modInfoTextBox.AppendText($"By: {item.Author}{nL}Version: {item.ModVersion}{nL}A.I.R Version: {item.GameVersion}");

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
            if (File.Exists(Properties.Settings.Default.Sonic3AIRPath))
            {
                string baseDirectory = Path.GetDirectoryName(Properties.Settings.Default.Sonic3AIRPath);
                if (Directory.Exists(baseDirectory))
                {
                    Regex reg = new Regex(@"(gamerecording_)\d{6}(_)\d{6}");
                    DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);
                    FileInfo[] fileInfo = directoryInfo.GetFiles("*.bin").Where(path => reg.IsMatch(path.Name)).ToArray();
                    foreach (var file in fileInfo)
                    {
                        Sonic3AIRRecording recording = new Sonic3AIRRecording(file);
                        gameRecordingList.Items.Add(recording);
                    }
                }
            }

        }

        private bool InitalCollection()
        {
            Sonic3AIRAppDataFolder = AppDataFolder + "\\Sonic3AIR";
            Sonic3AIRActiveModsList = Sonic3AIRAppDataFolder + "\\mods\\active-mods.json";
            Sonic3AIRModsFolder = Sonic3AIRAppDataFolder + "\\mods";
            Sonic3AIRTempModsFolder = Sonic3AIRAppDataFolder + "\\temp_mod_install";
            Sonic3AIRSettingsFile = Sonic3AIRAppDataFolder + "\\settings.json";


            if (!Directory.Exists(Sonic3AIRTempModsFolder))
            {
                Directory.CreateDirectory(Sonic3AIRTempModsFolder);
            }

            List<Tuple<string, bool>> MissingFilesState = new List<Tuple<string, bool>>();

            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRAppDataFolder", Directory.Exists(Sonic3AIRAppDataFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRModsFolder", Directory.Exists(Sonic3AIRModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRTempModsFolder", Directory.Exists(Sonic3AIRTempModsFolder)));
            MissingFilesState.Add(new Tuple<string, bool>("Sonic3AIRSettingsFile", File.Exists(Sonic3AIRSettingsFile)));

            if (MissingFilesState.Exists(x => x.Item2.Equals(false)))
            {
                List<Tuple<string, bool>> MissingList = MissingFilesState.Where(x => x.Item2.Equals(false)).ToList();
                string missingItems = "The following files could not be found: ";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRAppDataFolder"))) missingItems += $"{nL}- {Sonic3AIRAppDataFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRModsFolder"))) missingItems += $"{nL}- {Sonic3AIRModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRTempModsFolder"))) missingItems += $"{nL}- {Sonic3AIRTempModsFolder}";
                if (MissingList.Exists(x => x.Item1.Equals("Sonic3AIRSettingsFile"))) missingItems += $"{nL}- {Sonic3AIRSettingsFile}";
                missingItems += $"{nL}{nL}If you have not run Sonic 3 A.I.R yet, please run Sonic 3 A.I.R once before running the modloader!";
                missingItems += $"{nL}{nL}If you have, make sure these locations exist. The modloader can't run without them";
                MessageBox.Show(missingItems);
                return false;
            }
            else
            {
                //CreateGameBananaShortcut();
                if (!File.Exists(Sonic3AIRActiveModsList))
                {
                    S3AIRActiveMods = new Sonic3AIRActiveMods(Sonic3AIRActiveModsList);
                }
                else
                {
                    FileInfo list = new FileInfo(Sonic3AIRActiveModsList);
                    S3AIRActiveMods = new Sonic3AIRActiveMods(list);
                }


                FileInfo file = new FileInfo(Sonic3AIRSettingsFile);
                S3AIRSettings = new Sonic3AIRSettings(file);
                return true;
            }
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

            foreach (string d in Directory.GetDirectories(Sonic3AIRTempModsFolder))
            {
                foundFile = Directory.GetFiles(d, "mod.json").FirstOrDefault();
            }
            Directory.Move(System.IO.Path.GetDirectoryName(foundFile), Sonic3AIRModsFolder + "\\" + Path.GetFileNameWithoutExtension(file));
            CleanUpTempModsFolder();
            UpdateModsList(true);
        }


        public void ExtractRar(string file)
        {
            using (var archive = SharpCompress.Archives.Rar.RarArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(Sonic3AIRTempModsFolder, new ExtractionOptions()
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
                    entry.WriteToDirectory(Sonic3AIRTempModsFolder, new ExtractionOptions()
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
                    entry.WriteToDirectory(Sonic3AIRTempModsFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        private void RemoveMod()
        {
            var modToRemove = ModList.SelectedItem as Sonic3AIRMod;
            if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(modToRemove.FolderPath);
                Directory.Delete(modToRemove.FolderPath);
                UpdateModsList(true);
                
            }


        }

        private void CleanUpTempModsFolder()
        {
            WipeFolderContents(Sonic3AIRTempModsFolder);
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

            void UpdateMods(Sonic3AIRMod item)
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
            ModsList = new List<Sonic3AIRMod>();
            GetModsCheckStateLegacy();
            ModList.DataSource = ModsList;
            ModList.DisplayMember = "Name";
            ModList.ValueMember = "IsEnabled";
            for (int i = 0; i < ModList.Items.Count; i++)
            {
                Sonic3AIRMod obj = (Sonic3AIRMod)ModList.Items[i];
                ModList.SetItemChecked(i, obj.IsEnabled);
            }
            ModList.ItemCheck += ModsList_ItemCheck;

        }

        private void GetModsCheckStateLegacy()
        {
            DirectoryInfo d = new DirectoryInfo(Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            foreach (DirectoryInfo folder in folders)
            {
                DirectoryInfo f = new DirectoryInfo(folder.FullName);
                var root = f.GetFiles("mod.json").FirstOrDefault();
                Sonic3AIRMod mod;
                if (root != null)
                {
                    try
                    {
                        mod = new Sonic3AIRMod(root);
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
                        MessageBox.Show($"Error with loading {folder.Name}!{Environment.NewLine}(Likely a JSON Error; Make sure the mod.json file is formated correctly!){Environment.NewLine}{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error with loading {folder.Name}!{Environment.NewLine}{ex.Message}");
                    }


                }



            }
        }

        private void DisableModLegacy(Sonic3AIRMod mod)
        {
            try
            {
                string result = Sonic3AIRModsFolder + "\\" + "#" + mod.FolderName.Replace("#", "");
                Directory.Move(mod.FolderPath, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Please Refresh the Mod List!");
            }
        }

        private void EnableModLegacy(Sonic3AIRMod mod)
        {
            try
            {
                string result = Sonic3AIRModsFolder + "\\" + mod.FolderName.Replace("#", "");
                Directory.Move(mod.FolderPath, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Please Refresh the Mod List!");
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
                        Sonic3AIRMod obj = (Sonic3AIRMod)ModList.Items[i];
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
            ModsList = new List<Sonic3AIRMod>();
            EnableAllLegacyDisabledMods();
            GetModsCheckState();
            ModList.DataSource = ModsList;
            ModList.DisplayMember = "Name";
            ModList.ValueMember = "IsEnabled";
            for (int i = 0; i < ModList.Items.Count; i++)
            {
                Sonic3AIRMod obj = (Sonic3AIRMod)ModList.Items[i];
                ModList.SetItemChecked(i, obj.IsEnabled);
            }
            ModList.ItemCheck += ModsList_ItemCheck;

        }

        private void GetModsCheckState()
        {
            DirectoryInfo d = new DirectoryInfo(Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            IList<Tuple<Sonic3AIRMod, int>> ActiveMods = new List<Tuple<Sonic3AIRMod, int>>();
            foreach (DirectoryInfo folder in folders)
            {
                DirectoryInfo f = new DirectoryInfo(folder.FullName);
                var root = f.GetFiles("mod.json").FirstOrDefault();
                Sonic3AIRMod mod;
                if (root != null)
                {
                    try
                    {
                        mod = new Sonic3AIRMod(root);
                        if (S3AIRActiveMods.ActiveMods.Contains(mod.FolderName))
                        {
                            mod.IsEnabled = true;
                            mod.EnabledLocal = true;
                            ActiveMods.Add(new Tuple<Sonic3AIRMod, int>(mod, S3AIRActiveMods.ActiveMods.IndexOf(mod.FolderName)));
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
                        MessageBox.Show($"Error with loading {folder.Name}!{Environment.NewLine}(Likely a JSON Error; Make sure the mod.json file is formated correctly!){Environment.NewLine}{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error with loading {folder.Name}!{Environment.NewLine}{ex.Message}");
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

                void UpdateMods(Sonic3AIRMod item)
                {
                    if (item.IsEnabled != item.EnabledLocal)
                    {
                        if (item.IsEnabled == true) EnableMod(item);
                        else DisableMod(item);
                    }
                }
            }
        }

        private void DisableMod(Sonic3AIRMod mod)
        {
            S3AIRActiveMods.ActiveMods.Remove(mod.FolderName);
        }

        private void EnableMod(Sonic3AIRMod mod)
        {
            S3AIRActiveMods.ActiveMods.Add(mod.FolderName);
        }

        private void EnableAllLegacyDisabledMods()
        {
            DirectoryInfo d = new DirectoryInfo(Sonic3AIRModsFolder);
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
                string destination = Sonic3AIRModsFolder + "\\" + folder.Replace("#", "");
                string source = Sonic3AIRModsFolder + "\\" + folder;
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
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                Process.Start(Path.GetDirectoryName(filename));
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    string filename = Properties.Settings.Default.Sonic3AIRPath;
                    Process.Start(Path.GetDirectoryName(filename));
                }
            }
        }

        private void OpenAppDataFolder()
        {
            Process.Start(Sonic3AIRAppDataFolder);
        }

        private void OpenModsFolder()
        {
            Process.Start(Sonic3AIRModsFolder);
        }

        private void OpenSelectedModFolder(Sonic3AIRMod mod)
        {
            Process.Start(mod.FolderPath);
        }

        private void OpenLogFile()
        {
            if (File.Exists(Sonic3AIRAppDataFolder + "//logfile.txt"))
            {
                Process.Start(Sonic3AIRAppDataFolder + "//logfile.txt");
            }
            else
            {
                MessageBox.Show($"Log file not found: {nL}{Sonic3AIRAppDataFolder}\\logfile.txt");
            }

        }

        private void OpenModdingTemplatesFolder()
        {
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                if (ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(Sonic3AIRModdingTemplatesFolder);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ValidateSonic3AIRModdingTemplatesFolderPath()) Process.Start(Sonic3AIRModdingTemplatesFolder);
                }
            }
        }

        private void OpenSampleModsFolder()
        {
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                if (ValidateSonic3AIRSampleModsFolderPath()) Process.Start(Sonic3AIRSampleModsFolder);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ValidateSonic3AIRSampleModsFolderPath()) Process.Start(Sonic3AIRSampleModsFolder);
                }
            }
        }

        private void OpenUserManual()
        {
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                if (ValidateSonic3AIRUserManualFilePath()) Process.Start(Sonic3AIRUserManualFile);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ValidateSonic3AIRUserManualFilePath()) Process.Start(Sonic3AIRUserManualFile);
                }
            }
        }

        private void OpenModDocumentation()
        {
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                if (ValidateSonic3AIRModDocumentationFilePath()) Process.Start(Sonic3AIRModDocumentationFile);
            }
            else
            {
                if (GameHandler.UpdateSonic3AIRLocation())
                {
                    UpdateAIRSettings();
                    if (ValidateSonic3AIRModDocumentationFilePath()) Process.Start(Sonic3AIRModDocumentationFile);
                }
            }
        }

        private void OpenModURL(string url)
        {
            Process.Start(url);
        }

        private void OpenSettingsFile()
        {
            Process.Start(Sonic3AIRAppDataFolder + "//settings.json");
        }

        private void OpenRecordingLocation()
        {
            if (gameRecordingList.SelectedItem != null)
            {
                Sonic3AIRRecording item = gameRecordingList.SelectedItem as Sonic3AIRRecording;
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

        public void DownloadMod(string url)
        {
            string baseURL = GetBaseURL(url);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (baseURL != "") remote_filename = GetRemoteFileName(baseURL);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            DownloadWindow downloadWindow = new DownloadWindow($"Downloading \"{filename}\"", url, $"{Sonic3AIRTempModsFolder}\\{filename}");
            Action finishAction = DownloadModCompleted;
            downloadWindow.DownloadCompleted = finishAction;
            downloadWindow.Start();
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
            string file = Directory.GetFiles($"{Sonic3AIRTempModsFolder}").FirstOrDefault(x => x.EndsWith(".zip"));
            AddMod(file);
        }

        private void DownloadButtonTest_Click(object sender, EventArgs e)
        {
            //DownloadMod("https://gamebanana.com/dl/430253");

            
            string url = "";
            if (ShowInputDialog(ref url) == DialogResult.OK)
            {
                if (url != "") MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else DownloadMod(url);
            }

        }

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(300, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Enter Mod URL...";
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        #endregion

        #region Information Sending

        private async void UploadRecordingToFileDotIO(Sonic3AIRRecording recording)
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

                    string message = @"File Uploaded to File.io, it will expire in 1 week and has a one time use:" + Environment.NewLine +
                    "(URL Has Been Copied to Clipboard): " + Environment.NewLine +
                    url;
                    Clipboard.SetText(url);
                    MessageBox.Show(message);

                }
            }
        }

        #endregion

        #region Custom Classes
        public class Sonic3AIRMod
        {
            public string Author;
            public string Name { get; set; }
            public string TechnicalName { get; set; }
            public string Description;
            public string FolderName;
            public string FolderPath;
            public string URL;
            public string ModVersion;
            public string GameVersion;
            public bool EnabledLocal { get; set; }
            public bool IsEnabled { get; set; }
            public override string ToString() { return Name; }

            public Sonic3AIRMod(FileInfo mod)
            {
                string data = File.ReadAllText(mod.FullName);
                dynamic stuff = JRaw.Parse(data);
                //Author
                Author = stuff.Metadata.Author;
                if (Author == null) Author = "N/A";
                //Name
                Name = stuff.Metadata.Name;
                if (Name == null) Name = "N/A";
                //Description
                Description = stuff.Metadata.Description;
                if (Description == null) Description = "No Description Provided.";
                //Mod URL
                URL = stuff.Metadata.URL;
                //ModVersion
                ModVersion = stuff.Metadata.ModVersion;
                if (ModVersion == null) ModVersion = "N/A";
                //GameVersion
                GameVersion = stuff.Metadata.GameVersion;
                if (GameVersion == null) GameVersion = "N/A";

                FolderName = mod.Directory.Name;
                FolderPath = mod.Directory.FullName;
                TechnicalName = $"[{FolderName.Replace("#","")}]";

            }
        }
        public class Sonic3AIRSettings
        {
            public bool FailSafeMode = false;
            public string Sonic3KRomPath = "";
            public bool FixGlitches = false;
            public string FilePath = "";
            private dynamic jsonObj;
            public Sonic3AIRSettings(FileInfo settings)
            {
                FilePath = settings.FullName;
                string data = File.ReadAllText(FilePath);
                jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                FailSafeMode = jsonObj.FailSafeMode;
                FixGlitches = jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES;
                Sonic3KRomPath = jsonObj.RomPath;
            }

            private void PraseSettings()
            {
                if (FailSafeMode == true)
                {
                    jsonObj.FailSafeMode = true;
                }
                else
                {
                    jsonObj.FailSafeMode = false;
                }
                if (FixGlitches == true)
                {
                    jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = 1;
                }
                else
                {
                    jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = 0;
                }
                jsonObj.RomPath = Sonic3KRomPath;
            }

            public void SaveSettings()
            {
                PraseSettings();
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(FilePath, output);
            }
        }
        public class Sonic3AIRRecording
        {
            public string Name;
            public string FilePath;
            public string FormalName;

            public override string ToString() { return Name; }
            public string GetRAW()
            {
                var binData = File.ReadAllBytes(FilePath);
                var sb = new StringBuilder();
                foreach (var b in binData)
                    sb.Append(" " + b.ToString("X2"));
                return sb.ToString();
            }

            public Sonic3AIRRecording(FileInfo file)
            {
                FilePath = file.FullName;

                string baseString = file.Name.Replace("gamerecording_", "");

                string month = baseString.Substring(2, 2);
                string day = baseString.Substring(4, 2);
                string year = baseString.Substring(0, 2);

                string hour = baseString.Substring(7, 2);
                string minute = baseString.Substring(9, 2);
                string second = baseString.Substring(11, 2);

                string recordingFormat = $"Recording: {month}/{day}/{year} - {hour}.{minute}.{second}";

                Name = recordingFormat;

                FormalName = $"Sonic 3 AIR Recording [{recordingFormat}] ";
            }



        }
        public class Sonic3AIRActiveMods
        {
            public List<string> ActiveMods = new List<string>();
            public string ConfigPath = "";
            public Sonic3AIRActiveMods(FileInfo config)
            {
                ConfigPath = config.FullName;
                Load();
            }

            public void Load()
            {
                try
                {
                    string data = File.ReadAllText(ConfigPath);
                    JToken stuff = JRaw.Parse(data);
                    foreach (JProperty content in stuff.Children())
                    {
                        if (content.HasValues)
                        {
                            ActiveMods.AddRange(content.Value.ToObject<List<string>>());
                        }
                    }
                }
                catch (Exception ex)
                {
                    CreateFile(ConfigPath);
                    Load();
                }

            }

            public void CreateFile(string filePath)
            {
                var myFile = File.Create(filePath);
                myFile.Close();
                string nL = Environment.NewLine;
                string bracketOpen = "{";
                string bracketClose = "}";
                string standardFile = $"{bracketOpen}{nL}\t\"ActiveMods\": [{nL}{nL}\t]{nL}{bracketClose}";
                using (StreamWriter writetext = new StreamWriter(filePath)) writetext.WriteLine(standardFile);
            }

            public Sonic3AIRActiveMods(string filePath)
            {
                CreateFile(filePath);
                ConfigPath = filePath;
                Load();
            }

            public void Save(List<string> CurrentActiveMods)
            {
                var myFile = File.Create(ConfigPath);
                myFile.Close();
                string nL = Environment.NewLine;
                string bracketOpen = "{";
                string bracketClose = "}";
                string fileHeader = $"{bracketOpen}{nL}\t\"ActiveMods\": [{nL}";
                string fileFooter = $"{nL}\t]{nL}{bracketClose}";
                string fileContents = fileHeader + GetFiles() + fileFooter;
                using (StreamWriter writetext = new StreamWriter(ConfigPath)) writetext.WriteLine(fileContents);

                string GetFiles()
                {
                    string fileList = "";
                    string formatHead = "\t\t\"";
                    string formatFoot = "\",";
                    string formatFootEndofList = $"\"";
                    for (int i = 0; i < CurrentActiveMods.Count; i++)
                    {
                        if (i >= CurrentActiveMods.Count - 1) fileList += $"{nL}{formatHead}{CurrentActiveMods[i]}{formatFootEndofList}";
                        else fileList += $"{nL}{formatHead}{CurrentActiveMods[i]}{formatFoot}";
                    }
                    return fileList;
                }

                ActiveMods = CurrentActiveMods;
            }


        }


        #endregion

        #region Protocol Handler

        public void CreateGameBananaShortcut()
        {
            Sonic3AIRGBLinkPath = Sonic3AIRAppDataFolder + "//AIRModLoader.lnk";
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = shell.CreateShortcut(Sonic3AIRGBLinkPath) as IWshRuntimeLibrary.IWshShortcut;
            shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            shortcut.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            shortcut.Save();
        }



        #endregion


    }
}
