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

namespace Sonic3AIR_ModManager.Management
{
    public static class VersionManagement
    {
        public static void RemoveVersion(object version, ref ModManager Instance)
        {
            if (version != null && version is AIRVersionListItem)
            {
                AIRVersionListItem item = version as AIRVersionListItem;
                if (MessageBox.Show(Management.UserLanguage.RemoveVersion(item.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        Management.FileManagement.WipeFolderContents(item.FilePath);
                        Directory.Delete(item.FilePath);
                    }
                    catch
                    {
                        MessageBox.Show(Program.LanguageResource.GetString("UnableToRemoveVersion"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Management.VersionManagement.RefreshVersionsList(ref Instance, true);
                }

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

        public static List<VersionReader.AIRVersionData> InstalledVersions { get; set; } = new List<VersionReader.AIRVersionData>();

        public static void RefreshVersionsList()
        {
            InstalledVersions.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(Management.ProgramPaths.Sonic3AIR_MM_VersionsFolder);
            var folders = directoryInfo.GetDirectories().ToList();
            if (folders.Count != 0)
            {
                foreach (var folder in folders.VersionSort().Reverse())
                {
                    string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                    if (File.Exists(filePath))
                    {
                        VersionReader.AIRVersionData data = VersionReader.GetVersionData(Path.GetDirectoryName(filePath), false);
                        InstalledVersions.Add(data);
                    }


                }
            }
        }

        public static void RefreshVersionsList(ref ModManager Instance, bool fullRefresh = false)
        {
            if (fullRefresh)
            {
                InstalledVersions.Clear();
                Instance.VersionsListView.Items.Clear();
                DirectoryInfo directoryInfo = new DirectoryInfo(Management.ProgramPaths.Sonic3AIR_MM_VersionsFolder);
                var folders = directoryInfo.GetDirectories().ToList();
                if (folders.Count != 0)
                {
                    foreach (var folder in folders.VersionSort().Reverse())
                    {
                        string filePath = Path.Combine(folder.FullName, "sonic3air_game", "Sonic3AIR.exe");
                        if (File.Exists(filePath))
                        {
                            VersionReader.AIRVersionData data = VersionReader.GetVersionData(Path.GetDirectoryName(filePath), false);
                            Instance.VersionsListView.Items.Add(new AIRVersionListItem(data.ToString(), folder.FullName, (filePath == ProgramPaths.Sonic3AIRPath ? true : false)));
                            InstalledVersions.Add(data);
                        }


                    }
                }
            }

            bool enabled = Instance.VersionsListView.SelectedItem != null;
            Instance.removeVersionButton.IsEnabled = enabled;
            Instance.selectVersionButton.IsEnabled = enabled;
            Instance.openVersionLocationButton.IsEnabled = enabled;
        }

        public static void GoToAIRVersionManagement(ref ModManager Instance)
        {
            Instance.settingsPage.IsSelected = true;
            Instance.PrimaryTabControl.SelectedItem = Instance.settingsPage;
            Instance.versionsPage.IsSelected = true;
            Instance.OptionsTabControl.SelectedItem = Instance.versionsPage;
        }

        public static MenuItem GenerateInstalledVersionsToolstripItem(DirectoryInfo folder, string filepath, ref ModManager Instance)
        {
            MenuItem item = new MenuItem();
            item.Header = $"{folder.Name} ({VersionReader.GetVersionData(folder.FullName).ToString()})";
            item.Tag = new VersionTag(Instance, filepath);
            item.Click += ChangeAIRPathByInstalls;
            item.IsCheckable = false;
            item.IsChecked = (filepath == Management.ProgramPaths.Sonic3AIRPath);
            return item;
        }

        public static void ChangeAIRPathByInstalls(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            VersionTag tag = (item.Tag as VersionTag);
            Management.ProgramPaths.Sonic3AIRPath = tag.Path;
            Management.MainDataModel.Settings.Save();
            Management.MainDataModel.UpdateAIRSettings(ref tag.Instance);
        }

        public static void ChangeAIRPathFromSelected(ref ModManager Instance)
        {
            if (Instance.VersionsViewer.View.SelectedItem != null)
            {
                var entry = (Instance.VersionsViewer.View.SelectedItem as AIRVersionListItem);
                string filePath = Path.Combine(entry.FilePath, "sonic3air_game", "Sonic3AIR.exe");
                if (File.Exists(filePath))
                {
                    Management.ProgramPaths.Sonic3AIRPath = filePath;
                    Management.MainDataModel.Settings.Save();
                    Management.MainDataModel.UpdateAIRSettings(ref Instance);
                    RefreshVersionsList(ref Instance, true);
                }

            }
        }

        public static void ChangeAIRPathFromSettings(ref ModManager Instance)
        {
            if (Management.MainDataModel.S3AIRSettings != null)
            {
                if (Management.MainDataModel.S3AIRSettings.HasEXEPath)
                {
                    if (File.Exists(Management.MainDataModel.S3AIRSettings.AIREXEPath))
                    {
                        Management.ProgramPaths.Sonic3AIRPath = Management.MainDataModel.S3AIRSettings.AIREXEPath;
                        Management.MainDataModel.Settings.Save();
                        Management.MainDataModel.UpdateAIRSettings(ref Instance);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(Program.LanguageResource.GetString("AIRChangePathNoLongerExists"));
                    }
                }
            }
        }

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

                    string exe = Directory.GetFiles(destination, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                    if (exe != null && exe != "")
                    {
                        VersionReader.AIRVersionData version_data = VersionReader.GetVersionData(output, true);
                        string folder_path = $"{Management.ProgramPaths.Sonic3AIR_MM_VersionsFolder}\\{version_data.ToString()}";
                        MoveVersionToFinalLocation(destination, folder_path);
                    }
                    else
                    {
                        //TODO : Proper Message Implemented
                        string message = "";
                        MessageBox.Show(message);
                        CleanUpDownloadsFolder();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    CleanUpDownloadsFolder();
                }

            }



        }

        private static void CleanUpDownloadsFolder()
        {
            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Sonic3AIR_MM\\downloads";
            Management.FileManagement.WipeFolderContents(destination);
        }

        private static void MoveVersionToFinalLocation(string destination, string output2)
        {
            Directory.Move(destination, output2);
            Directory.CreateDirectory(destination);
            MessageBox.Show(Management.UserLanguage.VersionInstalled(output2));
            CleanUpDownloadsFolder();
        }

        public class AIRVersionListItem
        {
            public string Name { get { return _name; } }
            private string _name;

            public bool IsSelected { get; set; }

            public string FilePath { get { return _filePath; } }
            private string _filePath;

            public override string ToString()
            {
                return $"{Program.LanguageResource.GetString("Version")} {Name}";
            }

            public AIRVersionListItem(string name, string filePath, bool isSelected = false)
            {
                _name = name;
                _filePath = filePath;
                IsSelected = isSelected;
            }
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

        public static class VersionReader
        {
            public static Dictionary<string, AIRVersionData> DataRefrence { get; set; } = new Dictionary<string, AIRVersionData>();

            public class AIRVersionData
            {
                public string VersionString { get; set; } = "NULL";
                public Version Version { get; set; }
                public FileVersionInfo FileInfo { get; set; }
                public AIR_API.VersionMetadata Metadata { get; set; }

                public override string ToString()
                {
                    if (FileInfo != null)
                    {
                        return $"{FileInfo.FileMajorPart.ToString().PadLeft(2, '0')}.{FileInfo.FileMinorPart.ToString().PadLeft(2, '0')}.{FileInfo.FileBuildPart.ToString().PadLeft(2, '0')}.{FileInfo.FilePrivatePart.ToString()}";
                    }
                    else if (Version != null)
                    {
                        return $"{Version.Major.ToString().PadLeft(2, '0')}.{Version.Minor.ToString().PadLeft(2, '0')}.{Version.Build.ToString().PadLeft(2, '0')}.{Version.Revision.ToString()}";
                    }
                    else
                    {
                        return VersionString;
                    }
                }


                public AIRVersionData(Version _ver, string _ver_string, AIR_API.VersionMetadata _meta)
                {
                    Version = _ver;
                    VersionString = _ver_string;
                    Metadata = _meta;
                }

                public static AIRVersionData NullableDefault()
                {
                    return new AIRVersionData(null, "NULL", null);
                }
            }

            public static AIRVersionData GetVersionData(string destination, bool isAdding = false)
            {
                if (DataRefrence.ContainsKey(destination)) return DataRefrence[destination];
                else
                {
                    AIR_API.VersionMetadata VersionData = CheckforMetadataFile(destination);
                    if (VersionData == null) return GetDataFromEXE(destination, isAdding);
                    else return ReturnUsingMetadata(destination, VersionData);
                }

            }

            private static AIRVersionData ReturnUsingMetadata(string destination, AIR_API.VersionMetadata meta)
            {
                var data = new AIRVersionData(meta.Version, meta.VersionString, meta);
                DataRefrence.Add(destination, data);
                return data;
            }

            private static AIR_API.VersionMetadata CheckforMetadataFile(string destination)
            {
                if (destination == null || destination == "" || !Directory.Exists(destination)) return null;
                string metaDataFile = Directory.GetFiles(destination, "metadata.json", SearchOption.AllDirectories).FirstOrDefault();
                if (metaDataFile != null && metaDataFile != "")
                {
                    try
                    {
                        AIR_API.VersionMetadata ver = new AIR_API.VersionMetadata(new FileInfo(metaDataFile));
                        return ver;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }

            private static AIRVersionData GetDataFromEXE(string destination, bool isAdding)
            {
                AIRVersionData data;
                if (destination == null || destination == "" || !Directory.Exists(destination)) return LastResortVersionMethod(destination, isAdding);
                string exe = Directory.GetFiles(destination, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                if (exe != null && exe != "")
                {
                    var versInfo = FileVersionInfo.GetVersionInfo(exe);
                    string fileVersionFull2 = $"{versInfo.FileMajorPart.ToString().PadLeft(2, '0')}.{versInfo.FileMinorPart.ToString().PadLeft(2, '0')}.{versInfo.FileBuildPart.ToString().PadLeft(2, '0')}.{versInfo.FilePrivatePart.ToString()}";
                    if (Version.TryParse(fileVersionFull2, out Version result))
                    {
                        data = new AIRVersionData(result, fileVersionFull2, new AIR_API.VersionMetadata(result, fileVersionFull2));
                    }
                    else
                    {
                        data = new AIRVersionData(null, fileVersionFull2, new AIR_API.VersionMetadata(null, fileVersionFull2));
                    }
                    DataRefrence.Add(destination, data);
                    return data;
                }
                else
                {
                    return LastResortVersionMethod(destination, isAdding);
                }

            }

            private static AIRVersionData LastResortVersionMethod(string destination, bool isAdding)
            {
                if (isAdding)
                {
                    string exceptionVersion = "";
                    DialogResult result;
                    result = ExtraDialog.ShowInputDialog(ref exceptionVersion, "", Management.UserLanguage.GetOutputString("VersionSelectCaption1"));
                    while (Directory.Exists($"{exceptionVersion}") && !Uri.IsWellFormedUriString($"{exceptionVersion}", UriKind.Absolute) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
                    {
                        result = ExtraDialog.ShowInputDialog(ref exceptionVersion, "", Management.UserLanguage.GetOutputString("VersionSelectCaption2"));
                    }

                    AIRVersionData data;
                    data = new AIRVersionData(null, exceptionVersion, null);
                    DataRefrence.Add(destination, data);
                    return data;
                }
                else
                {
                    if (destination == null || destination == "" || !Directory.Exists(destination)) destination = "NULL";
                    var data = AIRVersionData.NullableDefault();
                    if (!DataRefrence.ContainsKey(destination)) DataRefrence.Add(destination, data);
                    return data;
                }




            }
        }


    }
}
