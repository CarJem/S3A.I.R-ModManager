using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Windows.Controls;
using MenuItem = System.Windows.Controls.MenuItem;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Sonic3AIR_ModManager
{
    public static class VersionManagement
    {
        public static void RemoveVersion(object version, ref ModManager Instance)
        {
            if (version != null && version is AIRVersionListItem)
            {
                AIRVersionListItem item = version as AIRVersionListItem;
                if (MessageBox.Show(UserLanguage.RemoveVersion(item.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        FileManagement.WipeFolderContents(item.FilePath);
                        Directory.Delete(item.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("UnableToRemoveVersion"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    VersionManagement.RefreshVersionsList(ref Instance, true);
                }

            }
        }

        public class AIRVersionListItem
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

        public static void OpenVersionFolder(ref ModManager Instance)
        {
            if (Instance.VersionsListView.SelectedItem != null && Instance.VersionsListView.SelectedItem is AIRVersionListItem)
            {
                AIRVersionListItem item = Instance.VersionsListView.SelectedItem as AIRVersionListItem;
                Process.Start(item.FilePath);
            }
        }

        public static void RefreshVersionsList(ref ModManager Instance, bool fullRefresh = false)
        {
            if (fullRefresh)
            {
                Instance.VersionsListView.Items.Clear();
                DirectoryInfo directoryInfo = new DirectoryInfo(ProgramPaths.Sonic3AIR_MM_VersionsFolder);
                var folders = directoryInfo.GetDirectories().ToList();
                if (folders.Count != 0)
                {
                    foreach (var folder in folders.VersionSort().Reverse())
                    {
                        string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                        if (File.Exists(filePath))
                        {
                            var versInfo = FileVersionInfo.GetVersionInfo(filePath);
                            string fileVersionFull2 = $"{versInfo.FileMajorPart.ToString().PadLeft(2, '0')}.{versInfo.FileMinorPart.ToString().PadLeft(2, '0')}.{versInfo.FileBuildPart.ToString().PadLeft(2, '0')}.{versInfo.FilePrivatePart}";
                            if (Version.TryParse(fileVersionFull2, out Version result))
                            {
                                Instance.VersionsListView.Items.Add(new AIRVersionListItem(fileVersionFull2, folder.FullName));
                            }
                            else
                            {
                                Instance.VersionsListView.Items.Add(new AIRVersionListItem(folder.Name, folder.FullName));
                            }
                        }


                    }
                }
            }

            bool enabled = Instance.VersionsListView.SelectedItem != null;
            Instance.removeVersionButton.IsEnabled = enabled;
            Instance.openVersionLocationButton.IsEnabled = enabled;
        }

        public static void GoToAIRVersionManagement(ref ModManager Instance)
        {
            Instance.settingsPage.IsSelected = true;
            Instance.PrimaryTabControl.SelectedItem = Instance.settingsPage;
            Instance.versionsPage.IsSelected = true;
            Instance.OptionsTabControl.SelectedItem = Instance.versionsPage;
        }

        public static void UpdateAIRVersionsToolstrips(ref ModManager Instance)
        {
            CleanUpInstalledVersionsToolStrip(ref Instance);
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
                            Instance.ChangeAIRVersionMenuItem.Items.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath, ref Instance));
                            Instance.ChangeAIRVersionFileMenuItem.Items.Add(GenerateInstalledVersionsToolstripItem(folder.Name, filePath, ref Instance));
                        }


                    }
                }

            }

        }

        public static void CleanUpInstalledVersionsToolStrip(ref ModManager Instance)
        {
            foreach (var item in Instance.ChangeAIRVersionMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= ChangeAIRPathByInstalls;
            }
            foreach (var item in Instance.ChangeAIRVersionFileMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= ChangeAIRPathByInstalls;
            }
            Instance.ChangeAIRVersionMenuItem.Items.Clear();
            Instance.ChangeAIRVersionFileMenuItem.Items.Clear();
        }

        public class VersionTag
        {
            public string Path { get; set; }
            public ModManager Instance;

            public VersionTag(ModManager _instance, string _path)
            {
                Instance = _instance;
                Path = _path;
            }
        }

        public static MenuItem GenerateInstalledVersionsToolstripItem(string name, string filepath, ref ModManager Instance)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = new VersionTag(Instance, filepath);
            item.Click += ChangeAIRPathByInstalls;
            item.IsCheckable = false;
            item.IsChecked = (filepath == ProgramPaths.Sonic3AIRPath);
            return item;
        }

        public static void ChangeAIRPathByInstalls(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            VersionTag tag = (item.Tag as VersionTag);
            ProgramPaths.Sonic3AIRPath = tag.Path;
            Properties.Settings.Default.Save();
            MainDataModel.UpdateAIRSettings(ref tag.Instance);
        }

        public static void ChangeAIRPathFromSettings(ref ModManager Instance)
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
                        System.Windows.MessageBox.Show(Program.LanguageResource.GetString("AIRChangePathNoLongerExists"));
                    }
                }
            }
        }


        #region A.I.R. Version Installation 

        public static void InstallVersionFromZIP()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("SonicAIRVersionZIP")} (*.zip)|*.zip",
                Title = Program.LanguageResource.GetString("SelectSonicAIRVersionZIP")
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
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
                    AIR_API.VersionMetadata ver;
                    string output2 = "";
                    string baseFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Sonic3AIR_MM\\air_versions\\";

                    try
                    {
                        ver = new AIR_API.VersionMetadata(new FileInfo(metaDataFile));
                        output2 = $"{baseFolder}{ver.VersionString}";
                        if (Directory.Exists(output2)) throw new Exception();
                        try
                        {
                            AddVersion(destination, output2);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    catch
                    {
                        try
                        {
                            string exe = Directory.GetFiles(destination, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                            var versInfo = FileVersionInfo.GetVersionInfo(exe);
                            string fileVersionFull2 = $"{versInfo.FileMajorPart.ToString().PadLeft(2, '0')}.{versInfo.FileMinorPart.ToString().PadLeft(2, '0')}.{versInfo.FileBuildPart.ToString().PadLeft(2, '0')}.{versInfo.FilePrivatePart.ToString()}";
                            if (Version.TryParse(fileVersionFull2, out Version result))
                            {
                                output2 = $"{baseFolder}{fileVersionFull2}";
                                AddVersion(destination, output2);
                            }
                            else
                            {
                                output2 = $"{baseFolder}{fileVersionFull2}";
                                AddVersion(destination, output2);
                            }

                        }
                        catch
                        {
                            VersionException(baseFolder, output2, destination);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

            void AddVersion(string destination, string output2)
            {
                Directory.Move(destination, output2);

                Directory.CreateDirectory(destination);

                MessageBox.Show(UserLanguage.VersionInstalled(output2));
            }

        }

        private static void VersionException(string baseFolder, string output2, string destination)
        {
            string exceptionVersion = "";
            DialogResult result;
            result = ExtraDialog.ShowInputDialog(ref exceptionVersion, "", UserLanguage.GetOutputString("VersionSelectCaption1"));
            while (Directory.Exists($"{baseFolder}{exceptionVersion}") && !Uri.IsWellFormedUriString($"{baseFolder}{exceptionVersion}", UriKind.Absolute) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
            {
                result = ExtraDialog.ShowInputDialog(ref exceptionVersion, "", UserLanguage.GetOutputString("VersionSelectCaption2"));
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                output2 = exceptionVersion;
                AddVersion();
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(destination);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            void AddVersion()
            {
                Directory.Move(destination, output2);

                Directory.CreateDirectory(destination);

                MessageBox.Show(UserLanguage.VersionInstalled(output2));
            }
        }

        #endregion
    }
}
