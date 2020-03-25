using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Win32;
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
using System.Threading;
using System.Resources;
using MessageBox = System.Windows.Forms.MessageBox;
using DialogResult = System.Windows.Forms.DialogResult;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;

namespace Sonic3AIR_ModManager.Management
{
    public static class RecordingManagement
    {
        public static bool HasPlaybackWarningBeenPresented = false;

        #region Game Recording Management
        public static void UpdateGameRecordingManagerButtons(ref ModManager Instance)
        {
            if (Instance.GameRecordingList.SelectedItem != null)
            {
                Instance.openRecordingButton.IsEnabled = true;
                Instance.copyRecordingFilePath.IsEnabled = true;
                Instance.uploadButton.IsEnabled = true;
                Instance.deleteRecordingButton.IsEnabled = true;
                Instance.playbackRecordingButton.IsEnabled = !Management.GameHandler.isGameRunning;

                Instance.openRecordingMenuItem.IsEnabled = true;
                Instance.copyRecordingFilePathMenuItem.IsEnabled = true;
                Instance.recordingUploadMenuItem.IsEnabled = true;
                Instance.deleteRecordingMenuItem.IsEnabled = true;
                Instance.playbackRecordingMenuItem.IsEnabled = !Management.GameHandler.isGameRunning && Management.RecordingManagement.HasPlaybackWarningBeenPresented;
            }
            else
            {
                Instance.openRecordingButton.IsEnabled = false;
                Instance.copyRecordingFilePath.IsEnabled = false;
                Instance.uploadButton.IsEnabled = false;
                Instance.deleteRecordingButton.IsEnabled = false;
                Instance.playbackRecordingButton.IsEnabled = false;

                Instance.openRecordingMenuItem.IsEnabled = false;
                Instance.copyRecordingFilePathMenuItem.IsEnabled = false;
                Instance.recordingUploadMenuItem.IsEnabled = false;
                Instance.deleteRecordingMenuItem.IsEnabled = false;
                Instance.playbackRecordingMenuItem.IsEnabled = false;
            }
        }

        public static void CollectGameRecordings(ref ModManager Instance)
        {
            Instance.GameRecordingList.Items.Clear();

            if (Management.ProgramPaths.GameRecordingsFolderDesiredPath == Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_Custom)
            {
                if (Directory.Exists(Management.ProgramPaths.CustomGameRecordingsFolderPath))
                {
                    Instance.RecordingsLocationBrowse.Content = $"{Management.UserLanguage.GetOutputString("CustomWordString")} ({Management.ProgramPaths.CustomGameRecordingsFolderPath})";
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

            if (Management.ProgramPaths.DoesSonic3AIRGameRecordingsFolderPathExist())
            {
                Instance.recordingsErrorMessagePanel.Visibility = Visibility.Collapsed;

                string baseDirectory = Management.ProgramPaths.GetSonic3AIRGameRecordingsFolderPath();
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
                Instance.recordingsErrorMessage.Text = Instance.recordingsErrorMessage.Tag.ToString().Replace("{0}", Management.UserLanguage.FolderOrFileDoesNotExist(Management.ProgramPaths.GetSonic3AIRGameRecordingsFolderPath(), false));
                Instance.recordingsErrorMessagePanel.Visibility = Visibility.Visible;
            }

        }

        public static void UpdateSelectedFolderPath(ref ModManager Instance)
        {

            if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationDefault)
            {
                Management.ProgramPaths.GameRecordingsFolderDesiredPath = Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_Default;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationAppData)
            {
                Management.ProgramPaths.GameRecordingsFolderDesiredPath = Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_AppData;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationRecordingsFolder)
            {
                Management.ProgramPaths.GameRecordingsFolderDesiredPath = Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_RecordingsFolder;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationEXEFolder)
            {
                Management.ProgramPaths.GameRecordingsFolderDesiredPath = Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_EXE_Folder;
                CollectGameRecordings(ref Instance);
            }
            else if (Instance.RecordingsSelectedLocationCombobox.SelectedItem == Instance.RecordingsLocationBrowse)
            {
                Management.ProgramPaths.GameRecordingsFolderDesiredPath = Management.ProgramPaths.GameRecordingSearchLocation.S3AIR_Custom;
                if (!Directory.Exists(Management.ProgramPaths.CustomGameRecordingsFolderPath)) SearchForCustomGameRecordingFolder(ref Instance);
                else CollectGameRecordings(ref Instance);
            }
            

        }

        public static void SearchForCustomGameRecordingFolder(ref ModManager Instance)
        {
            Management.ProgramPaths.SetCustomGameRecordingsFolderPath();
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
                Management.GameHandler.LaunchGameRecording(recordingFile.FilePath, Management.ProgramPaths.Sonic3AIRPath);
                Management.MainDataModel.UpdateInGameButtons(ref Instance);
            }
        }

        public static void UpdatePlayerWarning(ref ModManager Instance)
        {
            if (!HasPlaybackWarningBeenPresented)
            {
                Instance.recordingsPlaybackWarning.Visibility = Visibility.Visible;
            }
            else
            {
                Instance.recordingsPlaybackWarning.Visibility = Visibility.Collapsed;
            }
        }

        public static void UpdateAIRVersionsForPlaybackToolstrips(ref ModManager Instance)
        {
            Management.RecordingManagement.UpdateGameRecordingManagerButtons(ref Instance);
            CleanUpInstalledVersionsForPlaybackToolStrip(ref Instance);
            Instance.PlayUsingOtherVersionMenuItem.IsEnabled = false;
            Instance.PlayUsingOtherVersionHoverMenuItem.IsEnabled = false;
            if (Directory.Exists(Management.ProgramPaths.Sonic3AIR_MM_VersionsFolder))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Management.ProgramPaths.Sonic3AIR_MM_VersionsFolder);
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
            item.IsChecked = (filepath == Management.ProgramPaths.Sonic3AIRPath);
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
                        Management.GameHandler.LaunchGameRecording(recordingFile, tag.FilePath);
                        Management.MainDataModel.UpdateInGameButtons(ref tag.Instance);
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
                    Management.GameHandler.LaunchGameRecording(recordingFile.FilePath, exe_path);
                    Management.MainDataModel.UpdateInGameButtons(ref Instance);
                }

            }
        }




        #endregion

        #endregion

        public static async void UploadRecordingToFileDotIO(AIR_API.Recording recording)
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

                    string message = Management.UserLanguage.RecordingUploaded(url);
                    Clipboard.SetText(url);
                    MessageBox.Show(message);

                }
            }
        }

        public static bool DeleteRecording(AIR_API.Recording recording)
        {
            if (MessageBox.Show(Management.UserLanguage.DeleteItem(recording.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    File.Delete(recording.FilePath);
                    return true;
                }
                catch
                {
                    MessageBox.Show(Program.LanguageResource.GetString("UnableToDeleteFile"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            else return false;
        }

        public static void CopyRecordingLocationToClipboard(AIR_API.Recording item)
        {
            Clipboard.SetText(item.FilePath);
            MessageBox.Show(Program.LanguageResource.GetString("RecordingPathCopiedToClipboard"));
        }

        public static void CopyRecordingToDestination(string file, string exe_directory)
        {
            try
            {
                File.Copy(file, Path.Combine(exe_directory, "gamerecording.bin"), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void DeletePlaybackRecording(string exe_directory)
        {
            try
            {
                File.Delete(Path.Combine(exe_directory, "gamerecording.bin"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
