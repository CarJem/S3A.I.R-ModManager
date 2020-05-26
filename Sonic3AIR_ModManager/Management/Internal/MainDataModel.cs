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

namespace Sonic3AIR_ModManager.Management
{
    public static class MainDataModel
    {
        #region Definitions
        public static Settings.ModManagerSettings Settings { get; set; }

        public static Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>> ModCollectionMenuItems { get; set; } = new Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>>();
        public static Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>> LaunchPresetMenuItems { get; set; } = new Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>>();

        public static string nL = Environment.NewLine;
        public static AIR_API.Settings S3AIRSettings;
        public static bool isDownloading { get; set; } = false;
        public static string LastAIRSettingsVersion { get; set; } = "NULL";
        public static string LastAIREXEVersion { get; set; } = "NULL";

        public static AIR_API.Settings_Global Global_Settings;
        public static AIR_API.Settings_Input Input_Settings;
        public static AIR_API.GameConfig GameConfig { get; set; }
        public static AIR_API.VersionMetadata CurrentAIRVersion;

        public static System.Windows.Forms.Timer TimedEvents;
        public static System.Windows.Forms.Timer TimedUpdaterEvents;

        public static System.Windows.Controls.ToolTip AddModTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip RemoveSelectedModTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModUpTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModDownTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModToTopTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModToBottomTooltip = new System.Windows.Controls.ToolTip();

        #endregion

        #region Refresh Model

        public static void HideLaunchOptionsErrorPanels(ref ModManager Instance)
        {
            Instance.LaunchOptionsFailureMessageBackground.Visibility = Visibility.Collapsed;
            Instance.airLaunchPanel.IsEnabled = true;
        }
        public static void ShowLaunchOptionsErrorPanels(ref ModManager Instance)
        {
            Instance.airLaunchPanel.IsEnabled = false;
            Instance.LaunchOptionsFailureMessageBackground.Visibility = Visibility.Visible;
        }
        public static void UpdateAIRSettings(ref ModManager Instance)
        {
            Instance.sonic3AIRPathBox.Text = Management.ProgramPaths.Sonic3AIRPath;
            Instance.ThemeComboBox.SelectedSkin = App.Skin;
            Instance.ThemeComboBox.UpdateVisual();

            if (Management.MainDataModel.Global_Settings != null)
            {
                Instance.romPathBox.Text = Management.MainDataModel.Global_Settings.RomPath;
                Instance.fixGlitchesCheckbox.IsChecked = Management.MainDataModel.Global_Settings.FixGlitches;
                Instance.failSafeModeCheckbox.IsChecked = Management.MainDataModel.Global_Settings.FailSafeMode;
                Instance.FullscreenTypeComboBox.SelectedIndex = Management.MainDataModel.Global_Settings.Fullscreen;
                Instance.devModeCheckbox.IsChecked = Management.MainDataModel.Global_Settings.DevMode;
                Instance.SoftwareRenderingRadioButton.IsChecked = Management.MainDataModel.Global_Settings.UseSoftwareRenderer;
                Instance.HardwareRenderingRadioButton.IsChecked = !Management.MainDataModel.Global_Settings.UseSoftwareRenderer;
            }

            GetLanguageSelection(ref Instance);
            RetriveLaunchOptions(ref Instance);

            Management.ModManagement.UpdateModsList(true);

            ValidateSelectedVersionLabels(ref Instance);
            Management.MainDataModel.Settings.Save();

        }
        private static void ValidateSelectedVersionLabels(ref ModManager Instance)
        {
            Management.VersionManagement.VersionReader.AIRVersionData fileData = Management.VersionManagement.VersionReader.GetVersionData(Management.ProgramPaths.Sonic3AIR_BaseFolder, false);
            string settingData = (Management.MainDataModel.S3AIRSettings.Version != null ? Management.MainDataModel.S3AIRSettings.Version.ToString() : "NULL");
            if (Instance.airVersionLabel != null)
            {
                LastAIRSettingsVersion = settingData;
                LastAIREXEVersion = fileData.ToString();

                Instance.airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: {fileData.ToString()}";
                Instance.airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {settingData}";
            }
        }
        private static void GetLanguageSelection(ref ModManager Instance)
        {
            if (Instance.languageComboBox != null)
            {
                Instance.languageComboBox.Items.Clear();

                Binding LangItemsWidth = new Binding("ActualWidth") { ElementName = "languageComboBox" };

                Instance.AllowUpdate = false;

                ComboBoxItem EN_US = new ComboBoxItem()
                {
                    Tag = "EN_US",
                    Content = "English (EN_US)",
                };
                ComboBoxItem GR = new ComboBoxItem()
                {
                    Tag = "GR",
                    Content = "Deutsch (GR)"
                };
                ComboBoxItem FR = new ComboBoxItem()
                {
                    Tag = "FR",
                    Content = "Français (FR)"
                };
                ComboBoxItem RU = new ComboBoxItem()
                {
                    Tag = "RU",
                    Content = "Русский (RU)"
                };
                ComboBoxItem ES = new ComboBoxItem()
                {
                    Tag = "ES",
                    Content = "Español (ES)"
                };
                ComboBoxItem NULL = new ComboBoxItem()
                {
                    Tag = "NULL",
                    Content = "NULL (404)"
                };

                EN_US.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                GR.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                FR.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                RU.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                ES.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                NULL.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);

                Instance.languageComboBox.Items.Add(EN_US);
                Instance.languageComboBox.Items.Add(GR);
                Instance.languageComboBox.Items.Add(FR);
                Instance.languageComboBox.Items.Add(RU);
                Instance.languageComboBox.Items.Add(ES);
                if (Program.isDebug) Instance.languageComboBox.Items.Add(NULL);

                if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.EN_US) Instance.languageComboBox.SelectedItem = EN_US;
                else if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.GR) Instance.languageComboBox.SelectedItem = GR;
                else if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.FR) Instance.languageComboBox.SelectedItem = FR;
                else if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.RU) Instance.languageComboBox.SelectedItem = RU;
                else if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.ES) Instance.languageComboBox.SelectedItem = ES;
                else if (Management.UserLanguage.CurrentLanguage == Management.UserLanguage.Language.NULL) Instance.languageComboBox.SelectedItem = NULL;
                else Instance.languageComboBox.SelectedItem = NULL;

                Instance.AllowUpdate = true;
            }

        }
        public static void SetInitialWindowSize(ref ModManager Instance)
        {
            if (Management.MainDataModel.Settings.WindowSize != null)
            {
                Instance.Width = Management.MainDataModel.Settings.WindowSize.Width;
                Instance.Height = Management.MainDataModel.Settings.WindowSize.Height;
            }
        }
        public static void SetTooltips(ref ModManager Instance)
        {
            Management.MainDataModel.AddModTooltip.Content = Program.LanguageResource.GetString("AddAMod");
            Management.MainDataModel.RemoveSelectedModTooltip.Content = Program.LanguageResource.GetString("RemoveSelectedMod");
            Management.MainDataModel.MoveModUpTooltip.Content = Program.LanguageResource.GetString("IncreaseModPriority");
            Management.MainDataModel.MoveModDownTooltip.Content = Program.LanguageResource.GetString("DecreaseModPriority");
            Management.MainDataModel.MoveModToTopTooltip.Content = Program.LanguageResource.GetString("IncreaseModPriorityToMax");
            Management.MainDataModel.MoveModToBottomTooltip.Content = Program.LanguageResource.GetString("DecreaseModPriorityToMin");

            Instance.removeButton.ToolTip = Management.MainDataModel.RemoveSelectedModTooltip;
            Instance.addMods.ToolTip = Management.MainDataModel.AddModTooltip;
            Instance.moveUpButton.ToolTip = Management.MainDataModel.MoveModUpTooltip;
            Instance.moveDownButton.ToolTip = Management.MainDataModel.MoveModDownTooltip;
            Instance.moveToTopButton.ToolTip = Management.MainDataModel.MoveModToTopTooltip;
            Instance.moveToBottomButton.ToolTip = Management.MainDataModel.MoveModToBottomTooltip;

            Instance.Title = string.Format("{0} {1}", Program.LanguageResource.GetString("ApplicationTitle"), Program.Version);
        }
        public static void UpdateInUpdateButtons(ref ModManager Instance)
        {
            bool isAIRUpdaterActive = Program.AIRUpdaterState == Program.UpdateState.Checking || Program.AIRUpdaterState == Program.UpdateState.Running;
            bool isMMUpdaterActive = Program.MMUpdaterState == Program.UpdateState.Checking || Program.MMUpdaterState == Program.UpdateState.Running;


            bool enabled = !isMMUpdaterActive && !isAIRUpdaterActive;
            Instance.IsEnabled = enabled;
        }
        public static void UpdateInGameButtons(ref ModManager Instance)
        {
            bool enabled = !Management.GameHandler.isGameRunning;
            Instance.ModManagerButtons.Visibility = (enabled ? Visibility.Visible : Visibility.Hidden);
            Instance.InGameButtons.Visibility = (enabled ? Visibility.Hidden : Visibility.Visible);

            Instance.saveAndLoadButton.IsEnabled = enabled;
            Instance.saveButton.IsEnabled = enabled;
            Instance.exitButton.IsEnabled = enabled;

            Instance.saveMenuItem.IsEnabled = enabled;
            Instance.LoadMenuItem.IsEnabled = enabled;
            Instance.forceKillMenuItem.IsEnabled = !enabled;

            Instance.keepLoaderOpenCheckBox.IsEnabled = enabled;
            Instance.keepOpenOnQuitCheckBox.IsEnabled = enabled;
            Instance.sonic3AIRPathBox.IsEnabled = enabled;
            Instance.romPathBox.IsEnabled = enabled;
            Instance.fixGlitchesCheckbox.IsEnabled = enabled;
            Instance.failSafeModeCheckbox.IsEnabled = enabled;
            Instance.modPanel.IsEnabled = enabled;
            Instance.autoRunCheckbox.IsEnabled = enabled;
            Instance.inputPanel.IsEnabled = enabled;
            Instance.AboutMenuItem.IsEnabled = enabled;
            Instance.devModeCheckbox.IsEnabled = enabled;
            Instance.OptionsTabControl.IsEnabled = enabled;
            Instance.playbackRecordingButton.IsEnabled = enabled;
            Instance.playbackRecordingMenuItem.IsEnabled = enabled;

            Instance.DisableInGameEnhancementsCheckbox.IsEnabled = enabled;
            Instance.DisableInGameEnhancementsText.IsEnabled = enabled;

            if (Management.MainDataModel.Settings.DisableInGameEnhancements == false)
            {
                if (Management.GameHandler.isGameRunning) Management.GameHandler.InGameContextMenu.Subscribe();
                else Management.GameHandler.InGameContextMenu.Unsubscribe();
            }

        }
        public static void UpdateSettingsStates(ref ModManager Instance)
        {
            UpdateAIRSettings(ref Instance);
            Instance.autoLaunchDelayLabel.IsEnabled = Management.MainDataModel.Settings.AutoLaunch;
            Instance.AutoLaunchNUD.IsEnabled = Management.MainDataModel.Settings.AutoLaunch;
        }
        public static void ChangeS3RomPath(ref ModManager Instance)
        {
            string title = Program.LanguageResource.GetString("ChangeS3KROMPathDialog_Title");
            string text = Program.LanguageResource.GetString("ChangeS3KROMPathDialog_Part1") + nL +
                $"   - {Program.LanguageResource.GetString("ChangeS3KROMPathDialog_Part2")}" + nL +
                $"   - {Program.LanguageResource.GetString("ChangeS3KROMPathDialog_Part3")}" + nL +
                $"   - {Program.LanguageResource.GetString("ChangeS3KROMPathDialog_Part4")}";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                // File Select
                string inital_folder = "";
                if (File.Exists(Management.MainDataModel.Global_Settings.RomPath)) inital_folder = Path.GetDirectoryName(Management.MainDataModel.Global_Settings.RomPath);

                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = $"{Program.LanguageResource.GetString("Sonic3KRomFile")} (*.bin)|*.bin",
                    InitialDirectory = inital_folder,
                    Title = Program.LanguageResource.GetString("SelectSonic3KRomFile")

                };
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Management.MainDataModel.Global_Settings.RomPath = fileDialog.FileName;
                    Management.MainDataModel.Global_Settings.Save();
                }
            }
            else if (result == DialogResult.Yes)
            {
                // Auto Find ROM Path
                string rom_path = AIR_API.SteamROMHandler.TryGetSteamRomPath();
                if (rom_path != "")
                {
                    Management.MainDataModel.Global_Settings.RomPath = rom_path;
                    Management.MainDataModel.Global_Settings.Save();
                }
            }

            UpdateAIRSettings(ref Instance);
        }
        public enum S3AIRSetting : int
        {
            FailSafeMode = 0,
            FixGlitches = 1,
            EnableDevMode = 2,
            UseSoftwareRenderer = 3
        }
        public static void UpdateBoolSettings(S3AIRSetting setting, bool isChecked)
        {
            if (setting == S3AIRSetting.FailSafeMode)
            {
                Management.MainDataModel.Global_Settings.FailSafeMode = isChecked;
            }
            else if (setting == S3AIRSetting.FixGlitches)
            {
                Management.MainDataModel.Global_Settings.FixGlitches = isChecked;
            }
            else if (setting == S3AIRSetting.UseSoftwareRenderer)
            {
                Management.MainDataModel.Global_Settings.UseSoftwareRenderer = isChecked;
            }
            else
            {
                Management.MainDataModel.Global_Settings.DevMode = isChecked;
            }
            Management.MainDataModel.Global_Settings.Save();
        }
        public static void UpdateCurrentLanguage(ref ModManager Instance)
        {
            if (Instance.languageComboBox.SelectedItem != null)
            {
                switch ((Instance.languageComboBox.SelectedItem as ComboBoxItem).Tag.ToString())
                {
                    case "EN_US":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.EN_US;
                        break;
                    case "GR":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.GR;
                        break;
                    case "FR":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.FR;
                        break;
                    case "RU":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.RU;
                        break;
                    case "ES":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.ES;
                        break;
                    case "NULL":
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.NULL;
                        break;
                    default:
                        Management.UserLanguage.CurrentLanguage = Management.UserLanguage.Language.NULL;
                        break;
                }

                Management.UserLanguage.ApplyLanguage(ref Instance);
                Management.ModManagement.UpdateModsList(true);
                UpdateAIRSettings(ref Instance);
            }



        }
        public static void UpdateAIRGameConfigLaunchOptions(ref ModManager Instance)
        {
            SaveLaunchOptions(ref Instance);
            RetriveLaunchOptions(ref Instance);
        }
        private static void SaveLaunchOptions(ref ModManager Instance)
        {
            if (Instance.SceneComboBox != null && Instance.PlayerComboBox != null && Instance.StartPhaseComboBox != null)
            {
                if (Management.MainDataModel.Global_Settings != null)
                {
                    Instance.SceneComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;

                    if ((Instance.SceneComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        Management.MainDataModel.Global_Settings.LoadLevel = (Instance.SceneComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                    }
                    else Management.MainDataModel.Global_Settings.LoadLevel = null;
                    if ((Instance.PlayerComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        if (int.TryParse((Instance.PlayerComboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out int result))
                        {
                            Management.MainDataModel.Global_Settings.UseCharacters = result;
                        }
                    }
                    else Management.MainDataModel.Global_Settings.UseCharacters = null;
                    if ((Instance.StartPhaseComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        if (int.TryParse((Instance.StartPhaseComboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out int result))
                        {
                            Management.MainDataModel.Global_Settings.StartPhase = result;
                        }
                    }
                    else Management.MainDataModel.Global_Settings.StartPhase = null;

                    Management.MainDataModel.Global_Settings.Save();


                    Instance.SceneComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                }


            }
        }
        public static void RetriveLaunchOptions(ref ModManager Instance)
        {
            if (Instance.SceneComboBox != null && Instance.PlayerComboBox != null && Instance.StartPhaseComboBox != null)
            {
                if (Management.MainDataModel.Global_Settings != null)
                {
                    Instance.SceneComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;

                    if (Management.MainDataModel.Global_Settings != null)
                    {
                        if (Management.MainDataModel.Global_Settings.LoadLevel != null)
                        {
                            ComboBoxItem item = Instance.SceneComboBox.Items.Cast<ComboBoxItem>().Where(x => x.Tag.ToString() == Management.MainDataModel.Global_Settings.LoadLevel.ToString()).FirstOrDefault();
                            Instance.SceneComboBox.SelectedItem = item;
                        }
                        else Instance.SceneComboBox.SelectedIndex = 0;

                        if (Management.MainDataModel.Global_Settings.UseCharacters != null)
                        {
                            ComboBoxItem item = Instance.PlayerComboBox.Items.Cast<ComboBoxItem>().Where(x => x.Tag.ToString() == Management.MainDataModel.Global_Settings.UseCharacters.ToString()).FirstOrDefault();
                            Instance.PlayerComboBox.SelectedItem = item;
                        }
                        else Instance.PlayerComboBox.SelectedIndex = 0;

                        if (Management.MainDataModel.Global_Settings.StartPhase != null)
                        {
                            string phase = Management.MainDataModel.Global_Settings.StartPhase.ToString();
                            if (Management.MainDataModel.Global_Settings.StartPhase.ToString() == "3" && !Program.isDeveloper) phase = "NONE";
                            ComboBoxItem item = Instance.StartPhaseComboBox.Items.Cast<ComboBoxItem>().Where(x => x.Tag.ToString() == phase).FirstOrDefault();
                            Instance.StartPhaseComboBox.SelectedItem = item;
                        }
                        else Instance.StartPhaseComboBox.SelectedIndex = 0;


                        if (Instance.SceneComboBox.SelectedIndex == 0) Instance.PlayerComboBox.IsEnabled = false;
                        else Instance.PlayerComboBox.IsEnabled = true;

                        if (!Program.isDeveloper)
                        {
                            Instance.DeveloperOnlyStartPhaseItem.Visibility = Visibility.Collapsed;
                            Instance.DeveloperOnlyStartPhaseItem.IsEnabled = false;

                            if (Instance.SceneComboBox.SelectedIndex != 0) Instance.StartPhaseComboBox.IsEnabled = false;
                            else Instance.StartPhaseComboBox.IsEnabled = true;
                        }
                        else
                        {
                            Instance.StartPhaseComboBox.IsEnabled = true;
                            Instance.DeveloperOnlyStartPhaseItem.Visibility = Visibility.Visible;
                            Instance.DeveloperOnlyStartPhaseItem.IsEnabled = true;
                        }


                    }

                    Instance.SceneComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged += Instance.LaunchOptions_SelectionChanged;
                }
            }
        }
        public static void RefreshModManagementButtons(ref ModManager Instance)
        {
            if (Instance.ModViewer.SelectedItem != null)
            {
                if (Instance.ModViewer.ActiveView.SelectedItem != null && (Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem).IsEnabled)
                {
                    Instance.moveUpButton.IsEnabled = (Instance.ModViewer.ActiveView.Items.IndexOf((Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem)) > 0);
                    Instance.moveDownButton.IsEnabled = (Instance.ModViewer.ActiveView.Items.IndexOf((Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem)) < Instance.ModViewer.ActiveView.Items.Count - 1);
                    Instance.moveToTopButton.IsEnabled = (Instance.ModViewer.ActiveView.Items.IndexOf((Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem)) > 0);
                    Instance.moveToBottomButton.IsEnabled = (Instance.ModViewer.ActiveView.Items.IndexOf((Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem)) < Instance.ModViewer.ActiveView.Items.Count - 1);
                }
                else
                {
                    Instance.moveUpButton.IsEnabled = false;
                    Instance.moveDownButton.IsEnabled = false;
                    Instance.moveToTopButton.IsEnabled = false;
                    Instance.moveToBottomButton.IsEnabled = false;
                }
                Instance.removeButton.IsEnabled = true;
                Instance.removeModToolStripMenuItem.IsEnabled = true;
                Instance.editModFolderToolStripMenuItem.IsEnabled = true;
                Instance.openModFolderToolStripMenuItem.IsEnabled = true;
                Instance.openModURLToolStripMenuItem.IsEnabled = (ValidateURL((Instance.ModViewer.SelectedItem as ModViewerItem).Source.URL));
                if (Instance.ModViewer.ActiveView.Items.Contains(Instance.ModViewer.SelectedItem)) Instance.moveModToSubFolderMenuItem.IsEnabled = false;
                else Instance.moveModToSubFolderMenuItem.IsEnabled = true;
            }
            else
            {
                Instance.moveUpButton.IsEnabled = false;
                Instance.moveDownButton.IsEnabled = false;
                Instance.moveToTopButton.IsEnabled = false;
                Instance.moveToBottomButton.IsEnabled = false;
                Instance.removeButton.IsEnabled = false;
                Instance.editModFolderToolStripMenuItem.IsEnabled = false;
                Instance.removeModToolStripMenuItem.IsEnabled = false;
                Instance.openModFolderToolStripMenuItem.IsEnabled = false;
                Instance.moveModToSubFolderMenuItem.IsEnabled = false;
                Instance.openModURLToolStripMenuItem.IsEnabled = false;
            }

            Instance.moveUpButton.Visibility = Visibility.Visible;
            Instance.moveDownButton.Visibility = Visibility.Visible;
            Instance.moveToTopButton.Visibility = Visibility.Visible;
            Instance.moveToBottomButton.Visibility = Visibility.Visible;

            bool ValidateURL(string value)
            {
                if (value == null) return false;
                else if (value == "" || value == "NULL") return false;
                else if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && ValidateURI(uriResult)) return false;
                else return true;

                bool ValidateURI(Uri uriResult)
                {
                    if (uriResult == null) return false;
                    else return (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                }
            }
        }
        public static void RefreshSelectedModProperties(ref ModManager Instance)
        {
            RefreshModManagementButtons(ref Instance);
            if (Instance.ModViewer.SelectedItem != null)
            {
                AIR_API.Mod item = (Instance.ModViewer.SelectedItem as ModViewerItem).Source;
                if (item != null)
                {

                    string author = $"{Program.LanguageResource.GetString("By")}: {item.Author}";
                    string version = $"{Program.LanguageResource.GetString("Version")}: {item.ModVersion}";
                    string air_version = $"{Program.LanguageResource.GetString("AIRVersion")}: {item.GameVersion}";
                    string tech_name = $"{item.TechnicalName}";

                    string description = item.Description;
                    if (description == "No Description Provided.")
                    {
                        description = Program.LanguageResource.GetString("NoModDescript");
                    }

                    Paragraph author_p = new Paragraph(new Run(author));
                    Paragraph version_p = new Paragraph(new Run(version));
                    Paragraph air_version_p = new Paragraph(new Run(air_version));
                    Paragraph tech_name_p = new Paragraph(new Run(tech_name));
                    Paragraph description_p = new Paragraph(new Run($"{Management.MainDataModel.nL}{description}"));


                    author_p.FontWeight = FontWeights.Normal;
                    version_p.FontWeight = FontWeights.Normal;
                    air_version_p.FontWeight = FontWeights.Normal;
                    tech_name_p.FontWeight = FontWeights.Bold;
                    description_p.FontWeight = FontWeights.Normal;


                    var no_margin = new Thickness(0);
                    author_p.Margin = no_margin;
                    version_p.Margin = no_margin;
                    air_version_p.Margin = no_margin;
                    tech_name_p.Margin = no_margin;
                    description_p.Margin = no_margin;

                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Clear();
                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Add(author_p);
                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Add(version_p);
                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Add(air_version_p);
                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Add(tech_name_p);
                    Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Add(description_p);
                }
                else Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Clear();
            }
            else Instance.ModViewer.PHost.modInfoTextBox.Document.Blocks.Clear();



        }
        public static void RefreshTheme(ref ModManager Instance, GenerationsLib.WPF.Themes.Skin newSkin)
        {
            App.Skin = newSkin;
            GenerationsLib.WPF.Themes.SkinResourceDictionary.ChangeSkin(App.Skin, Sonic3AIR_ModManager.App.Current.Resources.MergedDictionaries);

            Instance.InvalidateVisual();
            foreach (UIElement element in Extensions.FindVisualChildren<UIElement>(Instance))
            {
                element.InvalidateVisual();
            }

            Instance.ThemeComboBox.UpdateVisual();
        }

        #endregion
    }
}
