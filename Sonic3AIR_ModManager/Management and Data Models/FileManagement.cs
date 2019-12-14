﻿using System;
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
using System.Threading;
using System.Resources;

namespace Sonic3AIR_ModManager
{
    public static class FileManagement
    {

        #region Mod File Watcher

        public static FileSystemWatcher GBAPIWatcher = new FileSystemWatcher(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder);
        public static bool isDownloadAllowed = true;
        public static bool HasAPIChangeBeenDetected = false;

        #endregion

        #region Mod File Management

        public static void AddMod()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("ModFileDialogFilter")} (*.zip;*.7z;*.rar)|*.zip;*.7z;*.rar",
                Title = Program.LanguageResource.GetString("ModFileDialogTitle")
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddMod(ofd.FileName);
            }
        }

        public static void AddMod(string file)
        {
            PrepMod(file);
        }

        public static void ExtractRar(string file)
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

        public static void ExtractZip(string file)
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

        public static void Extract7Zip(string file)
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

        public static void RemoveMod(AIR_API.Mod modToRemove)
        {
            if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(modToRemove.FolderPath);
                Directory.Delete(modToRemove.FolderPath);
                new Action(ModManager.UpdateUIFromInvoke).Invoke();
            }


        }

        #region Move Mod
        public static void MoveMod(AIR_API.Mod modToMove, string path)
        {
            string newPath = Path.Combine(path, modToMove.FolderName);
            if (!Directory.Exists(newPath))
            {
                Directory.Move(modToMove.FolderPath, newPath);
            }
            else
            {
                AIR_API.Mod conflictingMod = new AIR_API.Mod();
                if (File.Exists(Path.Combine(newPath, "mod.json")))
                {
                    conflictingMod = new AIR_API.Mod(new FileInfo(Path.Combine(newPath, "mod.json")));
                }
                ModMoveConflictResolve(modToMove, conflictingMod, newPath);

            }

            new Action(ModManager.UpdateUIFromInvoke).Invoke();
        }

        private static void ModMoveConflictResolve(AIR_API.Mod ExistingMod, AIR_API.Mod NewMod, string newPath)
        {

            var result = new ItemConflictDialog().ShowDialog(ExistingMod, NewMod);
            if (result == DialogResult.Yes)
            {
                DeleteOldMod();
                MoveMod();
            }
            else if (result == DialogResult.No)
            {
                MakeModCopy();
            }
            else
            {
                //Don't Import the Mod
            }


            #region Inside Methods

            void MakeModCopy()
            {
                string ModPath = newPath;
                int index = 1;
                string OriginalPath = ExistingMod.FolderName;
                while (Directory.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath))
                {
                    ModPath = $"{ModPath}({index})";
                }
                Directory.Move(ExistingMod.FolderPath, ModPath);
            }

            void MoveMod() { Directory.Move(ExistingMod.FolderPath, newPath); }
            void DeleteOldMod() { Directory.Delete(newPath); }

            #endregion
        }
        #endregion

        #region Remove Sub Folder

        public static void RemoveSubFolder(string subFolderToRemove)
        {
            string item = Path.GetDirectoryName(subFolderToRemove);
            if (MessageBox.Show(string.Format(Program.LanguageResource.GetString("RemoveSubFolderWarning"), item), Program.LanguageResource.GetString("ApplicationTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(subFolderToRemove);
                Directory.Delete(subFolderToRemove);
                new Action(ModManager.UpdateUIFromInvoke).Invoke();
            }


        }

        #endregion

        public static void CleanUpTempModsFolder()
        {
            WipeFolderContents(ProgramPaths.Sonic3AIR_MM_TempModsFolder);
        }

        public static void CleanUpAPIRequests()
        {
            WipeFolderContents(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder);
        }

        public static void WipeFolderContents(string folder)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(folder);

            try
            {
                if (di.Exists)
                {
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        if (file.Exists) file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.EnumerateDirectories())
                    {
                        if (dir.Exists) dir.Delete(true);
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Unable to Wipe Contents of \"{folder}\" clean, this may or may not be an issue.");
            }

        }



        #endregion

        #region Mod Import Validation Chain

        public static void PrepMod(string file)
        {
            try
            {
                ExtractTheMod(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something Went Wrong:" + Environment.NewLine + ex.Message);
            }


            ModSearchLoop();

            while (FindModRoot(0) != "")
            {
                ModSearchLoop();
            }
            CleanUpTempModsFolder();
            new Action(ModManager.UpdateUIFromInvoke).Invoke();

            void ModSearchLoop()
            {
                try
                {
                    string meta = FindModRoot(0);
                    if (meta != "") AddToModsFolder(meta, System.IO.Path.GetDirectoryName(meta));
                    else PrepModAttempt2();

                    void PrepModAttempt2()
                    {
                        meta = FindModRoot(1);
                        if (meta != "") AddToModsFolder(meta, System.IO.Path.GetDirectoryName(meta));
                        else BadModMessage();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something Went Wrong:" + Environment.NewLine + ex.Message);
                }
            }

        }

        public static void BadModMessage()
        {
            MessageBox.Show("This is not a valid Sonic 3 A.I.R. Mod. A valid mod requires a mod.json, and either this isn't a mod or it's a legacy mod. If you know for sure that this is a mod, then it's probably a legacy mod. You can't use legacy mods that work without them going forward.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void AddToModsFolder(string meta, string file)
        {
            string ModPath = Path.GetFileNameWithoutExtension(file);

            if (Directory.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath)) ModImportConflictResolve(meta, file);
            else MoveMod();


            void MoveMod() { Directory.Move(System.IO.Path.GetDirectoryName(meta), ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath); }
        }

        private static void ModImportConflictResolve(string meta, string file)
        {
            string ModPath = Path.GetFileNameWithoutExtension(file);
            string existingMeta = ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json";
            AIR_API.Mod ExistingMod = null;
            AIR_API.Mod NewMod = null;

            if (File.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json"))
            {
                ExistingMod = new AIR_API.Mod(new FileInfo(existingMeta));
                NewMod = new AIR_API.Mod(new FileInfo(meta));
            }
            else
            {
                NewMod = new AIR_API.Mod(new FileInfo(meta));
            }



            var result = new ItemConflictDialog().ShowDialog(ExistingMod, NewMod);
            if (result == DialogResult.Yes)
            {
                DeleteOldMod();
                MoveMod();
            }
            else if (result == DialogResult.No)
            {
                MakeModCopy();
            }
            else
            {
                //Don't Import the Mod
            }


            #region Inside Methods

            void MakeModCopy()
            {
                int index = 1;
                string OriginalPath = ModPath;
                while (Directory.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath))
                {
                    ModPath = $"{ModPath}({index})";
                }
                MoveMod();
            }

            void MoveMod() { Directory.Move(System.IO.Path.GetDirectoryName(meta), ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath); }
            void DeleteOldMod() { Directory.Delete(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath, true); }

            #endregion
        }

        public static void ExtractTheMod(string file)
        {
            if (Path.GetExtension(file) == ".rar") ExtractRar(file);
            else if (Path.GetExtension(file) == ".zip") ExtractZip(file);
            else if (Path.GetExtension(file) == ".7z") Extract7Zip(file);
        }

        public static string FindModRoot(int phase = 0)
        {
            //Find the Root of the Mod in the Zip, Because some people have a folder inside of the zip, others may not
            string foundFile = "";
            if (phase == 0)
            {
                foreach (string d in Directory.GetDirectories(ProgramPaths.Sonic3AIR_MM_TempModsFolder))
                {
                    var item = Directory.GetFiles(d, "mod.json").FirstOrDefault();
                    foundFile = (item != null ? item.ToString() : "");
                }
            }
            else if (phase == 1)
            {
                var item = Directory.GetFiles(ProgramPaths.Sonic3AIR_MM_TempModsFolder, "mod.json").FirstOrDefault();
                foundFile = (item != null ? item.ToString() : "");
            }

            return foundFile;

        }

        #endregion

        #region Mod Downloading

        public static void GBAPIWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            HasAPIChangeBeenDetected = true;
        }

        public static void GBAPIInstallTrigger()
        {
            if (FileManagement.isDownloadAllowed && FileManagement.HasAPIChangeBeenDetected)
            {
                FileManagement.HasAPIChangeBeenDetected = false;
                FileManagement.isDownloadAllowed = false;
                FileManagement.GetNextAPIPendingInstall();
                FileManagement.isDownloadAllowed = true;
            }
        }

        public static void GetNextAPIPendingInstall()
        {
            string file = Directory.EnumerateFiles(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder).FirstOrDefault();
            if (file != null)
            {
                if (file.Contains("gb_api") && file.Contains(".txt"))
                {
                    string urlData = File.ReadAllText(file);
                    urlData = urlData.Replace("\r\n", "");
                    GamebananaAPI_Install(urlData);
                    File.Delete(file);
                }
            }

        }

        public static void GamebananaAPI_Install(string data)
        {
            if (!Directory.Exists(ProgramPaths.Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(ProgramPaths.Sonic3AIR_MM_TempModsFolder);
            string url = data.Replace("s3airmm://", "");
            if (url == "") MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show("Invalid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else DownloadMod(url, true, false);

        }
        public static void DownloadMod(string url, bool isMod = true, bool backgroundDownload = false)
        {
            string baseURL = GetBaseURL(url);
            //MessageBox.Show(baseURL);
            if (baseURL != "") url = baseURL;

            string remote_filename = "";
            if (url != "") remote_filename = GetRemoteFileName(url);
            string filename = "temp.zip";
            if (remote_filename != "") filename = remote_filename;

            if (File.Exists($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}\\{filename}")) File.Delete($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}\\{filename}");
            if (!Directory.Exists(ProgramPaths.Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(ProgramPaths.Sonic3AIR_MM_TempModsFolder);



            DownloadWindow downloadWindow = new DownloadWindow($"{Program.LanguageResource.GetString("Downloading")} \"{filename}\"", url, $"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}\\{filename}");
            Action finishAction = DownloadModCompleted;
            if (isMod) downloadWindow.DownloadCompleted = finishAction;
            if (backgroundDownload) downloadWindow.StartBackground();
            else downloadWindow.Start();
        }

        private static string GetRemoteFileName(string baseURL)
        {
            Uri uri = new Uri(baseURL);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private static string GetBaseURL(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }

        public static void DownloadModCompleted()
        {
            string fileZIP = Directory.GetFiles($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".zip"));
            string file7Z = Directory.GetFiles($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".7z"));
            string fileRAR = Directory.GetFiles($"{ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".rar"));

            if (File.Exists(fileZIP)) AddMod(fileZIP);
            else if (File.Exists(fileRAR)) AddMod(fileRAR);
            else if (File.Exists(file7Z)) AddMod(file7Z);
            else
            {
                MessageBox.Show("Something went Wrong!");
                CleanUpTempModsFolder();
            }
            GetNextAPIPendingInstall();
        }

        public static void AddModFromURLLink()
        {
            string url = "";
            if (ExtraDialog.ShowInputDialog(ref url, Program.LanguageResource.GetString("EnterModURL")) == DialogResult.OK)
            {
                if (url != "") MessageBox.Show(Program.LanguageResource.GetString("InvalidURL"), Program.LanguageResource.GetString("InvalidURL"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show(Program.LanguageResource.GetString("InvalidURL"), Program.LanguageResource.GetString("InvalidURL"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else DownloadMod(url, false);
            }

        }


        #endregion

        #region Mod SubFolder Management

        public static void AddNewModSubfolder(object selectedItem)
        {
            string newFolderName = Program.LanguageResource.GetString("NewSubFolderEntryName");
            DialogResult result;
            result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption1"));
            while (Directory.Exists(Path.Combine(ProgramPaths.Sonic3AIRModsFolder, newFolderName)) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
            {
                result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption2"));
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newDirectoryPath = Path.Combine(ProgramPaths.Sonic3AIRModsFolder, newFolderName);
                Directory.CreateDirectory(newDirectoryPath);
                if (selectedItem != null && selectedItem is ModViewerItem)
                {
                    FileManagement.MoveMod((selectedItem as ModViewerItem).Source, newDirectoryPath);
                }
            }
        }

        #endregion

        #region Version Management

        public static void RemoveVersion(object version)
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
                    ModManager.Instance.RefreshVersionsList(true);
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

        #endregion

        #region Recording Management

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

                    string message = UserLanguage.RecordingUploaded(url);
                    Clipboard.SetText(url);
                    MessageBox.Show(message);

                }
            }
        }

        public static bool DeleteRecording(AIR_API.Recording recording)
        {
            if (MessageBox.Show(UserLanguage.DeleteItem(recording.Name), "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
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

        #endregion

        #region Input Mapping Input Managamenet

        public static bool AddInputDevice()
        {
            string new_name = Program.LanguageResource.GetString("NewControllerEntryName");
            bool finished = false;
            char[] acceptable_char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890".ToArray();
            bool success = false;

            while (!finished)
            {

                DialogResult result = ExtraDialog.ShowInputDialog(ref new_name, Program.LanguageResource.GetString("AddInputDeviceDialogTitle"), Program.LanguageResource.GetString("AddInputDeviceDialogCaption"));
                bool containsKey = InputDeviceManager.Devices.ContainsKey(new_name);
                bool unacceptable_char = new_name.ContainsOnly(acceptable_char);
                if (result != System.Windows.Forms.DialogResult.Cancel && !containsKey && unacceptable_char)
                {
                    finished = true;
                    InputDeviceManager.Devices.Add(new_name, new AIR_API.InputMappings.Device(new_name));
                    success = true;
                }
                else if (result != System.Windows.Forms.DialogResult.Cancel)
                {
                    if (containsKey)
                    {
                        MessageBox.Show(string.Format("\"{0}\" {1}", new_name, Program.LanguageResource.GetString("AddInputDeviceError1")));
                    }
                    else
                    {
                        MessageBox.Show(string.Format("\"{0}\" {1}", new_name, Program.LanguageResource.GetString("AddInputDeviceError2")));
                    }

                }
                else
                {
                    finished = true;
                }
            }


            return success;


        }

        public static bool AddInputDeviceName(int index)
        {
            string newDevice = Program.LanguageResource.GetString("NewDeviceEntryName");
            DeviceNameDialog deviceNameDialog = new DeviceNameDialog();
            bool? result = deviceNameDialog.ShowDeviceNameDialog(ref newDevice, Program.LanguageResource.GetString("AddNewDeviceTitle"), Program.LanguageResource.GetString("AddNewDeviceDescription"));
            if (result == true)
            {
                InputDeviceManager.InputDevices.Items[index].Value.DeviceNames.Add(newDevice);
                return true;
            }
            else return false;
        }

        public static bool RemoveInputDevice(AIR_API.InputMappings.Device deviceToRemove)
        {
            DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(deviceToRemove.EntryName), Program.LanguageResource.GetString("DeleteDeviceTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                InputDeviceManager.Devices.Remove(deviceToRemove.EntryName);
                return true;
            }
            else return false;

        }

        public static bool RemoveInputDeviceName(string selectedItemToRemove, int inputIndex, int nameIndex)
        {
            DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(selectedItemToRemove), Program.LanguageResource.GetString("DeleteDeviceTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                int index = nameIndex;
                InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.RemoveAt(index);
                return true;
            }
            return false;

        }

        public enum MoveListItemDirection : int
        {
            MoveToTop = 0,
            MoveUp = 1,
            MoveDown = 2,
            MoveToBottom = 3
        }

        public static void MoveInputDevice(ref ModManager parent, MoveListItemDirection direction)
        {
            int index = -1;
            switch (direction)
            {
                case MoveListItemDirection.MoveToTop:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, 0);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(0);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveUp:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, index - 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(index - 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveDown:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, index + 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(index + 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveToBottom:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, InputDeviceManager.InputDevices.Items.Count - 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(InputDeviceManager.InputDevices.Items.Count - 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
            }
        }

        public static void MoveInputDeviceIdentifier(ref ModManager parent, MoveListItemDirection direction)
        {
            var selectedItem = parent.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
            int index = -1;
            int inputIndex = InputDeviceManager.InputDevices.Items.FindIndex(x => x.Value == selectedItem);
            switch (direction)
            {
                case MoveListItemDirection.MoveToTop:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, 0);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(0);
                    }
                    break;
                case MoveListItemDirection.MoveUp:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, index - 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(index - 1);
                    }
                    break;
                case MoveListItemDirection.MoveDown:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, index + 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(index + 1);
                    }
                    break;
                case MoveListItemDirection.MoveToBottom:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, InputDeviceManager.InputDevices.Items.Count - 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Count - 1);
                    }
                    break;
            }
        }

        #endregion

        #region Input Mapping Import/Exporting

        public static void ExportInputMappings(AIR_API.InputMappings.Device mappings)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog()
            {
                FileName = string.Format("{0}", mappings.EntryName),
                Filter = "Input File | *.json",
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mappings.ExportDevice(sfd.FileName);
            }
        }

        public static void ImportInputMappings()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Input File | *.json",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InputDeviceManager.InputDevices.ImportDevice(ofd.FileName);
            }
        }


        #endregion

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
                            //TODO : Read EXE For Version (This is Untested)
                            string exe = Directory.GetFiles(destination, "Sonic3AIR.exe", SearchOption.AllDirectories).FirstOrDefault();
                            var versInfo = FileVersionInfo.GetVersionInfo(exe);
                            string fileVersionFull2 = $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}.{versInfo.FilePrivatePart}";
                            if (Version.TryParse(fileVersionFull2, out Version result))
                            {
                                output2 = $"{baseFolder}{result.ToString()}";
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


        #region Full Game Backup

        public static bool BackupEntireGame(string source, string destination)
        {
            if (Directory.Exists(destination)) DeleteFilesFiltered(new DirectoryInfo(destination), new List<string>() { "mods", "gamerecordings" });
            MoveFilesRecursively(new DirectoryInfo(source), new DirectoryInfo(destination), new List<string>() { "mods", "gamerecordings" });
            return true;
        }

        public static void DeleteFilesFiltered(DirectoryInfo source, List<string> folderNamesToIgnore = null)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                if (!(folderNamesToIgnore != null && folderNamesToIgnore.Contains(dir.Name)))
                {
                    DeleteDirectory(dir, true);
                }
            }

            foreach (FileInfo file in source.GetFiles())
            {
                DeleteFile(file);
            }

        }


        public static void MoveFilesRecursively(DirectoryInfo source, DirectoryInfo target, List<string> folderNamesToIgnore = null)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                if (!(folderNamesToIgnore != null && folderNamesToIgnore.Contains(dir.Name)))
                {
                    MoveFilesRecursivelyInternal(dir, target.CreateSubdirectory(dir.Name));
                }
            }

            foreach (FileInfo file in source.GetFiles())
            {
                MoveFile(target, file);
            }

        }

        private static void MoveFilesRecursivelyInternal(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                MoveFilesRecursivelyInternal(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                MoveFile(target, file);
            }
        }


        private static void MoveFile(DirectoryInfo target, FileInfo file)
        {
            DialogResult result = DialogResult.Retry;
            while (result == DialogResult.Retry)
            {
                try
                {
                    file.MoveTo(Path.Combine(target.FullName, file.Name));
                    result = DialogResult.Ignore;
                }
                catch (Exception ex)
                {
                    result = MessageBox.Show(ex.Message + Environment.NewLine + UserLanguage.GetOutputString("TryAgain"), UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }
        }

        private static void DeleteDirectory(DirectoryInfo dir, bool recursive)
        {
            DialogResult result = DialogResult.Retry;
            while (result == DialogResult.Retry)
            {
                try
                {
                    dir.Delete(recursive);
                    result = DialogResult.Ignore;
                }
                catch (Exception ex)
                {
                    result = MessageBox.Show(ex.Message + Environment.NewLine + UserLanguage.GetOutputString("TryAgain"), UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }
        }

        private static void DeleteFile(FileInfo file)
        {
            DialogResult result = DialogResult.Retry;
            while (result == DialogResult.Retry)
            {
                try
                {
                    file.Delete();
                    result = DialogResult.Ignore;
                }
                catch (Exception ex)
                {
                    result = MessageBox.Show(ex.Message + Environment.NewLine + UserLanguage.GetOutputString("TryAgain"), UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }
        }

        #endregion
    }
}