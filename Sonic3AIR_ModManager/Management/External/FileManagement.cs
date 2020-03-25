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

namespace Sonic3AIR_ModManager.Management
{
    public static class FileManagement
    {
        #region Delete

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
                    result = MessageBox.Show(ex.Message + Environment.NewLine + Management.UserLanguage.GetOutputString("TryAgain"), Management.UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }
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
                    result = MessageBox.Show(ex.Message + Environment.NewLine + Management.UserLanguage.GetOutputString("TryAgain"), Management.UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
            }
        }

        #endregion

        #region Move

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
                    result = MessageBox.Show(ex.Message + Environment.NewLine + Management.UserLanguage.GetOutputString("TryAgain"), Management.UserLanguage.GetOutputString("BackupFailed"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                }
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

        #endregion

        #region Extraction

        public static void ExtractRar(string file, string directory)
        {
            using (var archive = SharpCompress.Archives.Rar.RarArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(directory, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        public static void ExtractZip(string file, string directory)
        {
            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(directory, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

        public static void Extract7Zip(string file, string directory)
        {
            using (var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(directory, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }


        #endregion
    }
}
