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
using System.Threading;
using System.Resources;

namespace Sonic3AIR_ModLoader
{
    public static class ModFileManagement
    {

        #region File Watcher

        public static FileSystemWatcher GBAPIWatcher = new FileSystemWatcher(ProgramPaths.Sonic3AIR_MM_GBRequestsFolder);
        public static bool isDownloadAllowed = true;
        public static bool HasAPIChangeBeenDetected = false;

        #endregion

        #region File Management

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

        public static void RemoveMod(AIR_SDK.Mod modToRemove)
        {
            if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                WipeFolderContents(modToRemove.FolderPath);
                Directory.Delete(modToRemove.FolderPath);
                new Action(ModManager.UpdateUIFromInvoke).Invoke();
            }


        }

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
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
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
                string meta = FindModRoot(0);
                if (meta != "") AddToModsFolder(meta, file);
                else PrepModAttempt2();

                void PrepModAttempt2()
                {
                    meta = FindModRoot(1);
                    if (meta != "") AddToModsFolder(meta, file);
                    else BadModMessage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something Went Wrong:" + Environment.NewLine + ex.Message);
            }

            CleanUpTempModsFolder();
            new Action(ModManager.UpdateUIFromInvoke).Invoke();

        }

        public static void BadModMessage()
        {
            MessageBox.Show("This is not a valid Sonic 3 A.I.R. Mod. A valid mod requires a mod.json, and either this isn't a mod or it's a legacy mod. If you know for sure that this is a mod, then it's probably a legacy mod. You can't use legacy mods that work without them going forward.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void AddToModsFolder(string meta, string file)
        {
            string ModPath = Path.GetFileNameWithoutExtension(file);

            if (Directory.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath)) ModImportConflictResolve(meta,file);
            else MoveMod();


            void MoveMod() { Directory.Move(System.IO.Path.GetDirectoryName(meta), ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath); }
        }

        private static void ModImportConflictResolve(string meta, string file)
        {
            string ModPath = Path.GetFileNameWithoutExtension(file);
            string existingMeta = ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json";
            AIR_SDK.Mod ExistingMod = null;
            AIR_SDK.Mod NewMod = null;

            if (File.Exists(ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json"))
            {
                ExistingMod = new AIR_SDK.Mod(new FileInfo(existingMeta));
                NewMod = new AIR_SDK.Mod(new FileInfo(meta));
            }
            else
            {
                NewMod = new AIR_SDK.Mod(new FileInfo(meta));
            }



            var result = new ItemConflictDialog().ShowDialog(ExistingMod,NewMod);
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

        #region Downloading

        public static void GBAPIWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            HasAPIChangeBeenDetected = true;
        }

        public static void GBAPIInstallTrigger()
        {
            if (ModFileManagement.isDownloadAllowed && ModFileManagement.HasAPIChangeBeenDetected)
            {
                ModFileManagement.HasAPIChangeBeenDetected = false;
                ModFileManagement.isDownloadAllowed = false;
                ModFileManagement.GetNextAPIPendingInstall();
                ModFileManagement.isDownloadAllowed = true;
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
    }
}
