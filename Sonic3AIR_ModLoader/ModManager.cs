using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net.Http;

namespace Sonic3AIR_ModLoader
{

    public partial class ModManager : Form
    {
        string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string Sonic3AIRAppDataFolder = "";
        string Sonic3AIRModsFolder = "";
        string Sonic3AIRTempModsFolder = "";
        Sonic3AIRSettings S3AIRSettings;
        public static ModManager Instance;
        List<Sonic3AIRMod> ModsList = new List<Sonic3AIRMod>();
        /// <summary>
        /// In order to control itemcheck changes (blinds double clicking, among other things)
        /// </summary>
        bool AuthorizeCheck { get; set; }

        public ModManager(bool autoBoot = false)
        {
            Instance = this;
            StartModloader();
            if (autoBoot) GameHandler.LaunchSonic3AIR();

        }
        
        private void StartModloader()
        {
            InitializeComponent();
            InitalCollection();
            UpdateModsList(true);
            UpdateUI();
        }

        #region Events

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (var mod in ModsList)
            {
                UpdateMods(mod);
            }
            UpdateModsList(true);
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
                (ModList.SelectedItem as Sonic3AIRMod).IsEnabled = (e.NewValue == CheckState.Checked);
            }
        }

        private void ModsList_SelectedValueChanged(object sender, EventArgs e)
        {
            RefreshSelectedMobProperties();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
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
        public void UpdateInGameButtons()
        {
            bool enabled = !GameHandler.isGameRunning;
            runButton.Enabled = enabled;
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
            autoLaunchDelayLabel.Enabled = Properties.Settings.Default.AutoLaunch;
            autoLaunchDelayUpDown.Enabled = Properties.Settings.Default.AutoLaunch;
        }

        private void ChangeS3RomPath()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Sonic 3K ROM File (*.bin)|*.bin",
                InitialDirectory = Path.GetDirectoryName(S3AIRSettings.Sonic3KRomPath)
                
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
                removeButton.Enabled = true;
                removeModToolStripMenuItem.Enabled = true;
                openModFolderToolStripMenuItem.Enabled = true;
            }
            else
            {
                removeButton.Enabled = false;
                removeModToolStripMenuItem.Enabled = false;
                openModFolderToolStripMenuItem.Enabled = false;
            }

            
            if (ModList.SelectedItem != null)
            {
                Sonic3AIRMod item = ModList.SelectedItem as Sonic3AIRMod;
                if (item != null)
                {
                    modNameLabel.Text = item.Name;
                    modTechnicalNameLabel.Text = item.TechnicalName;
                    modAuthorLabel.Text = "By: " + item.Author;
                    modDesciptionLabel.Text = item.Description;
                }
                else
                {
                    modNameLabel.Text = "";
                    modTechnicalNameLabel.Text = "";
                    modAuthorLabel.Text = "";
                    modDesciptionLabel.Text = "";
                }
            }
            else
            {
                modNameLabel.Text = "";
                modTechnicalNameLabel.Text = "";
                modAuthorLabel.Text = "";
                modDesciptionLabel.Text = "";
            }
        }

        private void UpdateModsList(bool FullReload = false)
        {
            if (FullReload) FetchModListData();
            RefreshSelectedMobProperties();
        }

        private void FetchModListData()
        {
            ModList.ItemCheck -= ModsList_ItemCheck;
            ModsList.Clear();
            ModsList = new List<Sonic3AIRMod>();
            GetEnabledDisabledMods();
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

        private void UpdateMods(Sonic3AIRMod item)
        {
            if (item.IsEnabled != item.EnabledLocal)
            {
                if (item.IsEnabled == false) DisableMod(item);
                else EnableMod(item);
            }
        }

        #endregion

        #region Information Retriving

        private void CollectGameRecordings()
        {
            gameRecordingList.Items.Clear();
            if (Directory.Exists(Properties.Settings.Default.Sonic3AIRPath))
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

        private void InitalCollection()
        {
            Sonic3AIRAppDataFolder = AppDataFolder + "\\Sonic3AIR";
            Sonic3AIRModsFolder = Sonic3AIRAppDataFolder + "\\mods";
            Sonic3AIRTempModsFolder = Sonic3AIRAppDataFolder + "\\temp_mod_install";
            if (Directory.Exists(Sonic3AIRAppDataFolder) && File.Exists(Sonic3AIRAppDataFolder + "\\settings.json"))
            {
                FileInfo file = new FileInfo(Sonic3AIRAppDataFolder + "\\settings.json");
                S3AIRSettings = new Sonic3AIRSettings(file);
            }
            else
            {
                MessageBox.Show("Please Run Sonic 3 A.I.R Once before Running the Modloader!");
                Application.Exit();
            }

        }

        private void GetEnabledDisabledMods()
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

        #endregion

        #region File Management

        private void AddMod()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Sonic 3 AIR Mod (*.zip)|*.zip"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //Find the Root of the Mod in the Zip, Because some people have a folder inside of the zip, others may not
                string foundFile = "";
                ZipFile.ExtractToDirectory(ofd.FileName, Sonic3AIRTempModsFolder);
                foreach (string d in Directory.GetDirectories(Sonic3AIRTempModsFolder))
                {
                    foundFile = Directory.GetFiles(d, "mod.json").FirstOrDefault();
                }
                Directory.Move(System.IO.Path.GetDirectoryName(foundFile), Sonic3AIRModsFolder + "\\" + Path.GetFileNameWithoutExtension(ofd.FileName));
                CleanUpTempModsFolder();
                UpdateModsList();
            }
        }

        private void RemoveMod()
        {
            var modToRemove = ModList.SelectedItem as Sonic3AIRMod;
            if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(modToRemove.FolderPath);
                Directory.Delete(modToRemove.FolderPath);
                UpdateModsList();
                
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

        private void DisableMod(Sonic3AIRMod mod)
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

        private void EnableMod(Sonic3AIRMod mod)
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

        #region Launching Events

        private void OpenEXEFolder()
        {
            if (Properties.Settings.Default.Sonic3AIRPath != null || Properties.Settings.Default.Sonic3AIRPath != "")
            {
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                Process.Start(Path.GetDirectoryName(filename));
            }
            else
            {
                GameHandler.UpdateSonic3AIRLocation();
                UpdateAIRSettings();
                string filename = Properties.Settings.Default.Sonic3AIRPath;
                Process.Start(Path.GetDirectoryName(filename));
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
            Process.Start(Sonic3AIRAppDataFolder + "//logfile.txt");
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

        #region Helpers

        #endregion

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
                //Enabled = isEnabled;
                string data = File.ReadAllText(mod.FullName);
                data = data.Replace("\r", " ");
                data = data.Replace("\n", " ");
                data = data.Replace("\t", " ");
                data = data.Replace("\"", "\'");
                dynamic stuff = JObject.Parse(data);
                Author = stuff.Metadata.Author;
                Name = stuff.Metadata.Name;
                Description = stuff.Metadata.Description;
                URL = stuff.Metadata.URL;
                ModVersion = stuff.Metadata.ModVersion;
                GameVersion = stuff.Metadata.GameVersion;
                if (Description == null) Description = "No Description Provided.";
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


    }
}
