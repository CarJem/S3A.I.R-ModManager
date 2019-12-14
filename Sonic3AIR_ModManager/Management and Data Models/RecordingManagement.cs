using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Sonic3AIR_ModManager
{
    public static class RecordingManagement
    {
        #region Game Recording Management

        public static void CollectGameRecordings(ref ModManager Instance)
        {
            Instance.GameRecordingList.Items.Clear();

            if (ProgramPaths.GameRecordingsFolderDesiredPath == ProgramPaths.GameRecordingSearchLocation.S3AIR_Custom)
            {
                if (Directory.Exists(ProgramPaths.CustomGameRecordingsFolderPath))
                {
                    Instance.RecordingsLocationBrowse.Content = $"{UserLanguage.GetOutputString("CustomWordString")} ({ProgramPaths.CustomGameRecordingsFolderPath})";
                }
                else
                {
                    Instance.RecordingsLocationBrowse.Content = Instance.RecordingsLocationBrowse.Tag.ToString();
                }
                Instance.RecordingsSelectedLocationBrowseButton.IsEnabled = true;
            }
            else
            {
                Instance.RecordingsSelectedLocationBrowseButton.IsEnabled = false;
            }

            if (ProgramPaths.DoesSonic3AIRGameRecordingsFolderPathExist())
            {
                Instance.recordingsErrorMessagePanel.Visibility = Visibility.Collapsed;

                string baseDirectory = ProgramPaths.GetSonic3AIRGameRecordingsFolderPath();
                if (Directory.Exists(baseDirectory))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);
                    FileInfo[] fileInfo = directoryInfo.GetFiles("*.bin").ToArray();
                    foreach (var file in fileInfo)
                    {
                        try
                        {
                            AIR_API.Recording recording = new AIR_API.Recording(file);
                            Instance.GameRecordingList.Items.Add(recording);
                        }
                        catch
                        {
                            //TODO : Add A Valid Catch Statement
                        }

                    }
                }
            }
            else
            {
                Instance.recordingsErrorMessage.Text = Instance.recordingsErrorMessage.Tag.ToString().Replace("{0}", UserLanguage.FolderOrFileDoesNotExist(ProgramPaths.GetSonic3AIRGameRecordingsFolderPath(), false));
                Instance.recordingsErrorMessagePanel.Visibility = Visibility.Visible;
            }

        }

        public static void UpdateSelectedFolderPath(ref ModManager Instance)
        {

            if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationDefault)
            {
                ProgramPaths.GameRecordingsFolderDesiredPath = ProgramPaths.GameRecordingSearchLocation.S3AIR_Default;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationAppData)
            {
                ProgramPaths.GameRecordingsFolderDesiredPath = ProgramPaths.GameRecordingSearchLocation.S3AIR_AppData;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationRecordingsFolder)
            {
                ProgramPaths.GameRecordingsFolderDesiredPath = ProgramPaths.GameRecordingSearchLocation.S3AIR_RecordingsFolder;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationEXEFolder)
            {
                ProgramPaths.GameRecordingsFolderDesiredPath = ProgramPaths.GameRecordingSearchLocation.S3AIR_EXE_Folder;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationBrowse)
            {
                ProgramPaths.GameRecordingsFolderDesiredPath = ProgramPaths.GameRecordingSearchLocation.S3AIR_Custom;
                if (!Directory.Exists(ProgramPaths.CustomGameRecordingsFolderPath)) SearchForCustomGameRecordingFolder(ref Instance);
                else CollectGameRecordings(ref Instance);
            }
            

        }

        public static void SearchForCustomGameRecordingFolder(ref ModManager Instance)
        {
            ProgramPaths.SetCustomGameRecordingsFolderPath();
            CollectGameRecordings(ref Instance);
        }

        private static Dictionary<string, string> RecordingVersions = new Dictionary<string, string>();

        #region AIR Gamerecording Playback Feature
        public static void OpenPlaybackContextMenu(ref ModManager Instance)
        {
            Instance.playbackRecordingButton.ContextMenu.IsOpen = true;
            UpdateAIRVersionsForPlaybackToolstrips(ref Instance);
        }

        public static void PlayUsingCurrentVersion(ref ModManager Instance)
        {
            if (Instance.GameRecordingList.SelectedItem != null && Instance.GameRecordingList.SelectedItem is AIR_API.Recording)
            {
                var recordingFile = Instance.GameRecordingList.SelectedItem as AIR_API.Recording;
                ProcessLauncher.LaunchGameRecording(recordingFile.FilePath, ProgramPaths.Sonic3AIRPath);
                MainDataModel.UpdateInGameButtons(ref Instance);
            }
        }

        public static void UpdateAIRVersionsForPlaybackToolstrips(ref ModManager Instance)
        {
            MainDataModel.UpdateGameRecordingManagerButtons(ref Instance);
            CleanUpInstalledVersionsForPlaybackToolStrip(ref Instance);
            Instance.PlayUsingOtherVersionMenuItem.IsEnabled = false;
            Instance.PlayUsingOtherVersionHoverMenuItem.IsEnabled = false;
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
                            Instance.PlayUsingOtherVersionMenuItem.Items.Add(GenerateInstalledVersionsForPlaybackToolstripItem(folder.Name, filePath, ref Instance));
                            Instance.PlayUsingOtherVersionHoverMenuItem.Items.Add(GenerateInstalledVersionsForPlaybackToolstripItem(folder.Name, filePath, ref Instance));
                            string versionID = folder.Name;
                            if (File.Exists(Path.Combine(folder.FullName, "sonic3air_game", "data", "metadata.json")))
                            {
                                AIR_API.VersionMetadata meta = new AIR_API.VersionMetadata(new FileInfo(Path.Combine(folder.FullName, "sonic3air_game", "data", "metadata.json")));
                                versionID = meta.VersionString;

                                Instance.PlayUsingOtherVersionMenuItem.IsEnabled = true;
                                Instance.PlayUsingOtherVersionHoverMenuItem.IsEnabled = true;
                            }
                            RecordingVersions.Add(versionID, filePath);
                        }


                    }
                }

            }

        }

        public static void CleanUpInstalledVersionsForPlaybackToolStrip(ref ModManager Instance)
        {
            foreach (var item in Instance.PlayUsingOtherVersionMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= LaunchPlaybackOnThisVersion;
            }
            foreach (var item in Instance.PlayUsingOtherVersionHoverMenuItem.Items.Cast<MenuItem>())
            {
                item.Click -= LaunchPlaybackOnThisVersion;
            }
            Instance.PlayUsingOtherVersionMenuItem.Items.Clear();
            Instance.PlayUsingOtherVersionHoverMenuItem.Items.Clear();
            RecordingVersions.Clear();
        }

        public class GameRecordingTag
        {
            public string FilePath { get; set; }
            public ModManager Instance;
        }

        public static MenuItem GenerateInstalledVersionsForPlaybackToolstripItem(string name, string filepath, ref ModManager Instance)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = new GameRecordingTag() { FilePath = filepath, Instance = Instance };
            item.Click += LaunchPlaybackOnThisVersion;
            item.IsCheckable = false;
            item.IsChecked = (filepath == ProgramPaths.Sonic3AIRPath);
            return item;
        }

        public static void LaunchPlaybackOnThisVersion(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                GameRecordingTag tag = item.Tag as GameRecordingTag;
                if (tag != null)
                {
                    if (tag.Instance.GameRecordingList.SelectedItem != null && tag.Instance.GameRecordingList.SelectedItem is AIR_API.Recording)
                    {
                        var recordingFile = tag.FilePath;
                        ProcessLauncher.LaunchGameRecording(recordingFile, tag.FilePath);
                        MainDataModel.UpdateInGameButtons(ref tag.Instance);
                    }
                }

            }


        }

        public static void PlaybackFromVersionThatMatches(ref ModManager Instance)
        {
            if (Instance.GameRecordingList.SelectedItem != null && Instance.GameRecordingList.SelectedItem is AIR_API.Recording)
            {
                var recordingFile = Instance.GameRecordingList.SelectedItem as AIR_API.Recording;
                if (RecordingVersions.Keys.ToList().Contains(recordingFile.AIRVersion))
                {
                    var exe_path = RecordingVersions.Where(x => x.Key == recordingFile.AIRVersion).FirstOrDefault().Value;
                    ProcessLauncher.LaunchGameRecording(recordingFile.FilePath, exe_path);
                    MainDataModel.UpdateInGameButtons(ref Instance);
                }

            }
        }




        #endregion

        #endregion
    }
}
