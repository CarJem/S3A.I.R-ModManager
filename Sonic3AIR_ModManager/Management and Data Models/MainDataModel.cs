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
    public static class MainDataModel
    {
        public static Settings.ModManagerSettings Settings { get; set; }

        public static Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>> ModCollectionMenuItems { get; set; } = new Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>>();
        public static Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>> LaunchPresetMenuItems { get; set; } = new Dictionary<int, List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>>();

        public static string nL = Environment.NewLine;
        public static AIR_API.Settings S3AIRSettings;

        public static AIR_API.Settings_Global Global_Settings { get; set; }
        public static AIR_API.Settings_Input Input_Settings { get; set; }
        public static AIR_API.GameConfig GameConfig { get; set; }
        public static AIR_API.VersionMetadata CurrentAIRVersion;

        public static System.Windows.Forms.Timer ApiInstallChecker;

        public static System.Windows.Controls.ToolTip AddModTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip RemoveSelectedModTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModUpTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModDownTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModToTopTooltip = new System.Windows.Controls.ToolTip();
        public static System.Windows.Controls.ToolTip MoveModToBottomTooltip = new System.Windows.Controls.ToolTip();

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
            Instance.sonic3AIRPathBox.Text = ProgramPaths.Sonic3AIRPath;
            if (MainDataModel.Global_Settings != null)
            {
                Instance.romPathBox.Text = MainDataModel.Global_Settings.RomPath;
                Instance.fixGlitchesCheckbox.IsChecked = MainDataModel.Global_Settings.FixGlitches;
                Instance.failSafeModeCheckbox.IsChecked = MainDataModel.Global_Settings.FailSafeMode;
                Instance.FullscreenTypeComboBox.SelectedIndex = MainDataModel.Global_Settings.Fullscreen;
                Instance.devModeCheckbox.IsChecked = MainDataModel.Global_Settings.DevMode;
            }

            GetLanguageSelection(ref Instance);
            RetriveLaunchOptions(ref Instance);

            ModManagement.UpdateModsList(true);

            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                if (ValidateSelectedVersion(ref Instance) == false) NullSituation(ref Instance);
            }
            else NullSituation(ref Instance);
            Properties.Settings.Default.Save();

            void NullSituation(ref ModManager SubInstance)
            {
                if (SubInstance.airVersionLabel != null)
                {
                    SubInstance.airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: NULL";
                    if (MainDataModel.S3AIRSettings.Version != null)
                    {
                        SubInstance.airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {MainDataModel.S3AIRSettings.Version.ToString()}";
                    }
                    else SubInstance.airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: NULL";
                }
            }

        }

        private static bool ValidateSelectedVersion(ref ModManager Instance)
        {
            string metaDataFile = Directory.GetFiles(Path.GetDirectoryName(ProgramPaths.Sonic3AIRPath), "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
            if (metaDataFile != null)
            {
                try
                {
                    MainDataModel.CurrentAIRVersion = new AIR_API.VersionMetadata(new FileInfo(metaDataFile));
                    Instance.airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: {MainDataModel.CurrentAIRVersion.VersionString}";
                    Instance.airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {MainDataModel.S3AIRSettings.Version.ToString()}";
                    return true;
                }
                catch
                {
                    return PhaseTwo(ref Instance);
                }

            }
            else
            {
                return PhaseTwo(ref Instance);
            }


            bool PhaseTwo(ref ModManager Parent)
            {
                var versInfo = FileVersionInfo.GetVersionInfo(ProgramPaths.Sonic3AIRPath);
                string fileVersionFull2 = $"{versInfo.FileMajorPart.ToString().PadLeft(2, '0')}.{versInfo.FileMinorPart.ToString().PadLeft(2, '0')}.{versInfo.FileBuildPart.ToString().PadLeft(2, '0')}.{versInfo.FilePrivatePart.ToString()}";
                if (Version.TryParse(fileVersionFull2, out Version result))
                {
                    MainDataModel.CurrentAIRVersion = new AIR_API.VersionMetadata(result, fileVersionFull2);
                    Parent.airVersionLabel.Text = $"{Program.LanguageResource.GetString("AIRVersion")}: {MainDataModel.CurrentAIRVersion.VersionString}";
                    Parent.airVersionLabel.Text += Environment.NewLine + $"{Program.LanguageResource.GetString("SettingsVersionLabel")}: {MainDataModel.S3AIRSettings.Version.ToString()}";
                    return true;
                }
                else
                {
                    return false;
                }
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
                    Content = "English (US)",
                };
                ComboBoxItem GR = new ComboBoxItem()
                {
                    Tag = "GR",
                    Content = "Deutsch"
                };
                ComboBoxItem FR = new ComboBoxItem()
                {
                    Tag = "FR",
                    Content = "Français"
                };
                ComboBoxItem NULL = new ComboBoxItem()
                {
                    Tag = "NULL",
                    Content = "NULL"
                };

                EN_US.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                GR.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                FR.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);
                NULL.SetBinding(FrameworkElement.WidthProperty, LangItemsWidth);

                Instance.languageComboBox.Items.Add(EN_US);
                Instance.languageComboBox.Items.Add(GR);
                Instance.languageComboBox.Items.Add(FR);
                if (Program.isDebug) Instance.languageComboBox.Items.Add(NULL);

                if (UserLanguage.CurrentLanguage == UserLanguage.Language.EN_US) Instance.languageComboBox.SelectedItem = EN_US;
                else if (UserLanguage.CurrentLanguage == UserLanguage.Language.GR) Instance.languageComboBox.SelectedItem = GR;
                else if (UserLanguage.CurrentLanguage == UserLanguage.Language.FR) Instance.languageComboBox.SelectedItem = FR;
                else if (UserLanguage.CurrentLanguage == UserLanguage.Language.NULL) Instance.languageComboBox.SelectedItem = NULL;
                else Instance.languageComboBox.SelectedItem = NULL;

                Instance.AllowUpdate = true;
            }

        }

        public static void SetInitialWindowSize(ref ModManager Instance)
        {
            if (Properties.Settings.Default.WindowSize != null)
            {
                Instance.Width = Properties.Settings.Default.WindowSize.Width;
                Instance.Height = Properties.Settings.Default.WindowSize.Height;
            }
        }

        public static void SetTooltips(ref ModManager Instance)
        {
            MainDataModel.AddModTooltip.Content = Program.LanguageResource.GetString("AddAMod");
            MainDataModel.RemoveSelectedModTooltip.Content = Program.LanguageResource.GetString("RemoveSelectedMod");
            MainDataModel.MoveModUpTooltip.Content = Program.LanguageResource.GetString("IncreaseModPriority");
            MainDataModel.MoveModDownTooltip.Content = Program.LanguageResource.GetString("DecreaseModPriority");
            MainDataModel.MoveModToTopTooltip.Content = Program.LanguageResource.GetString("IncreaseModPriorityToMax");
            MainDataModel.MoveModToBottomTooltip.Content = Program.LanguageResource.GetString("DecreaseModPriorityToMin");

            Instance.removeButton.ToolTip = MainDataModel.RemoveSelectedModTooltip;
            Instance.addMods.ToolTip = MainDataModel.AddModTooltip;
            Instance.moveUpButton.ToolTip = MainDataModel.MoveModUpTooltip;
            Instance.moveDownButton.ToolTip = MainDataModel.MoveModDownTooltip;
            Instance.moveToTopButton.ToolTip = MainDataModel.MoveModToTopTooltip;
            Instance.moveToBottomButton.ToolTip = MainDataModel.MoveModToBottomTooltip;

            Instance.Title = string.Format("{0} {1}", Program.LanguageResource.GetString("ApplicationTitle"), Program.Version);
        }

        public static void UpdateInGameButtons(ref ModManager Instance)
        {
            bool enabled = !ProcessLauncher.isGameRunning;
            Instance.ModManagerButtons.Visibility = (enabled ? Visibility.Visible : Visibility.Hidden);
            Instance.InGameButtons.Visibility = (enabled ? Visibility.Hidden : Visibility.Visible);
            Instance.saveAndLoadButton.IsEnabled = enabled;
            Instance.saveButton.IsEnabled = enabled;
            Instance.exitButton.IsEnabled = enabled;
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
        }



        public static void UpdateSettingsStates(ref ModManager Instance)
        {
            UpdateAIRSettings(ref Instance);
            Instance.autoLaunchDelayLabel.IsEnabled = Properties.Settings.Default.AutoLaunch;
            Instance.AutoLaunchNUD.IsEnabled = Properties.Settings.Default.AutoLaunch;
        }

        public static void ChangeS3RomPath(ref ModManager Instance)
        {
            string inital_folder = "";
            if (File.Exists(MainDataModel.Global_Settings.RomPath)) inital_folder = Path.GetDirectoryName(MainDataModel.Global_Settings.RomPath);

            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("Sonic3KRomFile")} (*.bin)|*.bin",
                InitialDirectory = inital_folder,
                Title = Program.LanguageResource.GetString("SelectSonic3KRomFile")

            };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MainDataModel.Global_Settings.RomPath = fileDialog.FileName;
                MainDataModel.Global_Settings.Save();
            }
            UpdateAIRSettings(ref Instance);
        }

        public enum S3AIRSetting : int
        {
            FailSafeMode = 0,
            FixGlitches = 1,
            EnableDevMode = 2
        }

        public static void UpdateBoolSettings(S3AIRSetting setting, bool isChecked)
        {
            if (setting == S3AIRSetting.FailSafeMode)
            {
                MainDataModel.Global_Settings.FailSafeMode = isChecked;
            }
            else if (setting == S3AIRSetting.FixGlitches)
            {
                MainDataModel.Global_Settings.FixGlitches = isChecked;
            }
            else
            {
                MainDataModel.Global_Settings.DevMode = isChecked;
            }
            MainDataModel.Global_Settings.Save();
        }

        public static void UpdateCurrentLanguage(ref ModManager Instance)
        {
            if (Instance.languageComboBox.SelectedItem != null)
            {
                switch ((Instance.languageComboBox.SelectedItem as ComboBoxItem).Tag.ToString())
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
                ModManagement.UpdateModsList(true);
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
                if (MainDataModel.Global_Settings != null)
                {
                    Instance.SceneComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;

                    if ((Instance.SceneComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        MainDataModel.Global_Settings.LoadLevel = (Instance.SceneComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                    }
                    else MainDataModel.Global_Settings.LoadLevel = null;
                    if ((Instance.PlayerComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        if (int.TryParse((Instance.PlayerComboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out int result))
                        {
                            MainDataModel.Global_Settings.UseCharacters = result;
                        }
                    }
                    else MainDataModel.Global_Settings.UseCharacters = null;
                    if ((Instance.StartPhaseComboBox.SelectedItem as ComboBoxItem).Tag.ToString() != "NONE")
                    {
                        if (int.TryParse((Instance.StartPhaseComboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out int result))
                        {
                            MainDataModel.Global_Settings.StartPhase = result;
                        }
                    }
                    else MainDataModel.Global_Settings.StartPhase = null;

                    MainDataModel.Global_Settings.Save();


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
                if (MainDataModel.Global_Settings != null)
                {
                    Instance.SceneComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.PlayerComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;
                    Instance.StartPhaseComboBox.SelectionChanged -= Instance.LaunchOptions_SelectionChanged;

                    if (MainDataModel.Global_Settings != null)
                    {
                        if (MainDataModel.Global_Settings.LoadLevel != null)
                        {
                            ComboBoxItem item = Instance.SceneComboBox.Items.Cast<ComboBoxItem>().Where(x => x.Tag.ToString() == MainDataModel.Global_Settings.LoadLevel.ToString()).FirstOrDefault();
                            Instance.SceneComboBox.SelectedItem = item;
                        }
                        else Instance.SceneComboBox.SelectedIndex = 0;

                        if (MainDataModel.Global_Settings.UseCharacters != null)
                        {
                            ComboBoxItem item = Instance.PlayerComboBox.Items.Cast<ComboBoxItem>().Where(x => x.Tag.ToString() == MainDataModel.Global_Settings.UseCharacters.ToString()).FirstOrDefault();
                            Instance.PlayerComboBox.SelectedItem = item;
                        }
                        else Instance.PlayerComboBox.SelectedIndex = 0;

                        if (MainDataModel.Global_Settings.StartPhase != null)
                        {
                            string phase = MainDataModel.Global_Settings.StartPhase.ToString();
                            if (MainDataModel.Global_Settings.StartPhase.ToString() == "3" && !Program.isDeveloper) phase = "NONE";
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
                if (Instance.ModViewer.ActiveView.SelectedItem != null && (Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem).IsEnabled && !ModManagement.S3AIRActiveMods.UseLegacyLoading)
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
                if (Instance.ModViewer.ActiveView.Items.Contains(Instance.ModViewer.SelectedItem) && !ModManagement.S3AIRActiveMods.UseLegacyLoading) Instance.moveModToSubFolderMenuItem.IsEnabled = false;
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

            if (ModManagement.S3AIRActiveMods.UseLegacyLoading)
            {
                Instance.moveUpButton.Visibility = Visibility.Collapsed;
                Instance.moveDownButton.Visibility = Visibility.Collapsed;
                Instance.moveToTopButton.Visibility = Visibility.Collapsed;
                Instance.moveToBottomButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                Instance.moveUpButton.Visibility = Visibility.Visible;
                Instance.moveDownButton.Visibility = Visibility.Visible;
                Instance.moveToTopButton.Visibility = Visibility.Visible;
                Instance.moveToBottomButton.Visibility = Visibility.Visible;
            }

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
                    Paragraph description_p = new Paragraph(new Run($"{MainDataModel.nL}{description}"));


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

                    Instance.modInfoTextBox.Document.Blocks.Clear();
                    Instance.modInfoTextBox.Document.Blocks.Add(author_p);
                    Instance.modInfoTextBox.Document.Blocks.Add(version_p);
                    Instance.modInfoTextBox.Document.Blocks.Add(air_version_p);
                    Instance.modInfoTextBox.Document.Blocks.Add(tech_name_p);
                    Instance.modInfoTextBox.Document.Blocks.Add(description_p);
                }
                else Instance.modInfoTextBox.Document.Blocks.Clear();
            }
            else Instance.modInfoTextBox.Document.Blocks.Clear();



        }

        public static void RefreshTheme(ref ModManager Instance)
        {
            if (Properties.Settings.Default.UseDarkTheme)
            {
                App.ChangeSkin(Skin.Dark);
            }
            else
            {
                App.ChangeSkin(Skin.Light);
            }

            Instance.InvalidateVisual();
            foreach (UIElement element in Extensions.FindVisualChildren<UIElement>(Instance))
            {
                element.InvalidateVisual();
            }
        }


        #endregion
    }
}
