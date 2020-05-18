using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Net.Http;
using System.Net;

using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using DialogResult = System.Windows.Forms.DialogResult;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Sonic3AIR_ModManager.Management
{
    public static class ModManagement
    {

        #region Definitions
        public static AIR_API.ActiveModsList S3AIRActiveMods;
        public static IList<ModViewerItem> ModsList = new List<ModViewerItem>();
        public static IList<ModViewerItem> ActiveModsList = new List<ModViewerItem>();



        private static ModManager Instance;
        #endregion

        #region Init

        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
        }
        #endregion

        #region Mod Import Validation Chain

        public static void ImportMod(string file, bool updateUI = true)
        {
            try 
            {
                if (Path.GetExtension(file) == ".rar") Management.FileManagement.ExtractRar(file, Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);
                else if (Path.GetExtension(file) == ".zip") Management.FileManagement.ExtractZip(file, Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);
                else if (Path.GetExtension(file) == ".7z") Management.FileManagement.Extract7Zip(file, Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Something Went Wrong:" + Environment.NewLine + ex.Message);
                return;
            }

            ModSearchLoop();
            while (FindModRoot(0) != "") ModSearchLoop();
            CleanUpTempModsFolder();
            if (updateUI) new Action(ModManager.UpdateUIFromInvoke).Invoke();

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

            if (Directory.Exists(Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath)) ModImportConflictResolve(meta, file);
            else MoveMod();

            if (!Directory.Exists(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder)) Directory.CreateDirectory(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);


            void MoveMod() { Directory.Move(System.IO.Path.GetDirectoryName(meta), Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath); }
        }
        private static void ModImportConflictResolve(string meta, string file)
        {
            string ModPath = Path.GetFileNameWithoutExtension(file);
            string existingMeta = Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json";
            AIR_API.Mod ExistingMod = null;
            AIR_API.Mod NewMod = null;

            if (File.Exists(Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath + "\\" + "mod.json"))
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
                while (Directory.Exists(Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath))
                {
                    ModPath = $"{ModPath}({index})";
                }
                MoveMod();
            }

            void MoveMod() { Directory.Move(System.IO.Path.GetDirectoryName(meta), Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath); }
            void DeleteOldMod() { Directory.Delete(Management.ProgramPaths.Sonic3AIRModsFolder + "\\" + ModPath, true); }

            #endregion
        }
        public static string FindModRoot(int phase = 0)
        {
            //Find the Root of the Mod in the Zip, Because some people have a folder inside of the zip, others may not
            string foundFile = "";
            if (phase == 0)
            {
                foreach (string d in Directory.GetDirectories(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder))
                {
                    var item = Directory.GetFiles(d, "mod.json").FirstOrDefault();
                    foundFile = (item != null ? item.ToString() : "");
                }
            }
            else if (phase == 1)
            {
                var item = Directory.GetFiles(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder, "mod.json").FirstOrDefault();
                foundFile = (item != null ? item.ToString() : "");
            }

            return foundFile;

        }

        #endregion

        #region Mod Downloading





        #endregion

        #region Events
        public static void MoveToSubDirectoryEvent(object sender, RoutedEventArgs e)
        {
            if (Instance.ModViewer.View.SelectedItem != null)
            {
                var collection = Instance.ModViewer.View.SelectedItems;
                for (int i = 0; i < collection.Count; i++)
                {
                    object item = collection[i];
                    if (item is ModViewerItem)
                    {
                        MoveMod((item as ModViewerItem).Source, (sender as MenuItem).Tag.ToString(), false);
                    }
                }
            }

            new Action(ModManager.UpdateUIFromInvoke).Invoke();
        }
        private static void ModsListItemCheckChanged()
        {
            UpdateModsList();
        }
        public static void DownloadModCompleted()
        {
            string fileZIP = Directory.GetFiles($"{Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".zip"));
            string file7Z = Directory.GetFiles($"{Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".7z"));
            string fileRAR = Directory.GetFiles($"{Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder}").FirstOrDefault(x => x.EndsWith(".rar"));

            if (File.Exists(fileZIP)) ImportMod(fileZIP);
            else if (File.Exists(fileRAR)) ImportMod(fileRAR);
            else if (File.Exists(file7Z)) ImportMod(file7Z);
            else
            {
                // Add User Translation
                MessageBox.Show("Something went Wrong!");
                CleanUpTempModsFolder();
            }
        }

        #endregion

        #region Update/Refresh

        public static void UpdateModsList(bool FullReload = false)
        {
            if (File.Exists(Management.ProgramPaths.Sonic3AIRPath))
            {
                int PreviousSubFolderIndex = Instance.ModViewer.FolderView.SelectedIndex;
                Instance.modErrorTextPanel.Visibility = Visibility.Collapsed;
                UpdateModListItemCheck(true);
                if (FullReload) FetchMods();
                else UpdateNewModsListItems();
                Management.MainDataModel.RefreshSelectedModProperties(ref Instance);
                UpdateModListItemCheck(false);
                if (Instance.ModViewer.FolderView.Items.Count > PreviousSubFolderIndex && PreviousSubFolderIndex != -1) Instance.ModViewer.FolderView.SelectedIndex = PreviousSubFolderIndex;
            }
            else
            {
                Instance.modErrorTextPanel.Visibility = Visibility.Visible;
            }

        }
        public static void UpdateActiveAndInactiveModLists()
        {
            IList<ModViewerItem> JustDisabledItems = new List<ModViewerItem>();
            IList<ModViewerItem> JustEnabledItems = new List<ModViewerItem>();

            if (!S3AIRActiveMods.UseLegacyLoading)
            {
                foreach (ModViewerItem mod in ModsList)
                {
                    mod.CheckBoxVisibility = Visibility.Visible;
                    if (mod.IsEnabled) JustEnabledItems.Add(mod);
                }

                foreach (ModViewerItem mod in ActiveModsList)
                {
                    mod.CheckBoxVisibility = Visibility.Visible;
                    if (!mod.IsEnabled) JustDisabledItems.Add(mod);
                }
            }
            else
            {
                foreach (ModViewerItem mod in ModsList)
                {
                    mod.CheckBoxVisibility = Visibility.Collapsed;
                    if (mod.IsInRootFolder) JustEnabledItems.Add(mod);
                }

                foreach (ModViewerItem mod in ActiveModsList)
                {
                    mod.CheckBoxVisibility = Visibility.Collapsed;
                    if (!mod.IsInRootFolder) JustDisabledItems.Add(mod);
                }
            }


            foreach (ModViewerItem mod in JustDisabledItems)
            {
                ActiveModsList.Remove(mod);
                ModsList.Add(mod);
            }

            foreach (ModViewerItem mod in JustEnabledItems)
            {
                ModsList.Remove(mod);
                ActiveModsList.Add(mod);
            }
        }
        public static void UpdateNewModsListItems()
        {
            Management.ProgramPaths.ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref Management.MainDataModel.S3AIRSettings);

            Instance.ModViewer.Clear();

            UpdateActiveAndInactiveModLists();


            foreach (ModViewerItem mod in ModsList)
            {
                Instance.ModViewer.View.Items.Add(mod);
            }

            foreach (ModViewerItem mod in ActiveModsList)
            {
                Instance.ModViewer.ActiveView.Items.Add(mod);
            }

            string CurrentFolderPath = Management.ProgramPaths.Sonic3AIRModsFolder;
            if (Instance.ModViewer.FolderView.SelectedItem != null) CurrentFolderPath = (Instance.ModViewer.FolderView.SelectedItem as SubDirectoryItem).FilePath;

            if (Instance.ModViewer.FolderView.SelectedIndex == 1) UpdateViewerModsShowAll();
            else UpdateViewerMods();

            Instance.ModViewer.Refresh();

            void UpdateViewerModsShowAll()
            {
                foreach (var item in Instance.ModViewer.View.Items)
                {
                    ModViewerItem mod = (ModViewerItem)item;
                    mod.Visibility = Visibility.Visible;
                }
            }

            void UpdateViewerMods()
            {
                foreach (var item in Instance.ModViewer.View.Items)
                {
                    ModViewerItem mod = (ModViewerItem)item;
                    if (CurrentFolderPath != null && (Instance.ModViewer.FolderView.SelectedIndex == 0 || Instance.ModViewer.FolderView.SelectedIndex == -1) && mod.IsInRootFolder)
                    {
                        mod.Visibility = Visibility.Visible;
                    }
                    else if (!(Instance.ModViewer.FolderView.SelectedIndex == 0 || Instance.ModViewer.FolderView.SelectedIndex == -1))
                    {
                        if (CurrentFolderPath != null && mod.Source.FileLocation.Contains(CurrentFolderPath))
                        {
                            mod.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            mod.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        mod.Visibility = Visibility.Collapsed;
                    }


                }
            }

        }
        public static void RefreshMoveToSubfolderList()
        {
            List<MenuItem> ItemsToRemove = new List<MenuItem>();
            foreach (var item in Instance.moveModToSubFolderMenuItem.Items)
            {
                if (item is MenuItem && !item.Equals(Instance.addNewModSubfolderMenuItem))
                {
                    ItemsToRemove.Add(item as MenuItem);
                }
            }
            foreach (var item in ItemsToRemove)
            {
                int index = Instance.moveModToSubFolderMenuItem.Items.IndexOf(item);
                (Instance.moveModToSubFolderMenuItem.Items[index] as MenuItem).Click -= MoveToSubDirectoryEvent;
                Instance.moveModToSubFolderMenuItem.Items.Remove(item);
            }
            ItemsToRemove.Clear();


            foreach (var item in Instance.ModViewer.FolderView.Items)
            {
                SubDirectoryItem realItem;
                if (item is SubDirectoryItem) realItem = item as SubDirectoryItem;
                else realItem = null;

                if (realItem != null)
                {
                    var menuItem = GenerateSubDirectoryToolstripItem(realItem.FileName, realItem.FilePath);
                    Instance.moveModToSubFolderMenuItem.Items.Add(menuItem);
                }
            }

        }
        public static MenuItem GenerateSubDirectoryToolstripItem(string name, string filepath)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = filepath;
            item.Click += MoveToSubDirectoryEvent;
            return item;
        }
        public static void UpdateModListItemCheck(bool shouldEnd)
        {
            if (shouldEnd) ModViewer.ItemCheck = null;
            else ModViewer.ItemCheck = ModsListItemCheckChanged;
        }
        public static void ToggleLegacyModManagement(bool value)
        {
            Management.ModManagement.S3AIRActiveMods.UseLegacyLoading = value;
            Management.ModManagement.Save();
        }
        public static void Save()
        {
            foreach (var mod in ModsList.Concat(ActiveModsList))
            {
                UpdateMods(mod.Source);
            }

            List<string> NewActiveModsList = new List<string>();

            foreach (var mod in ModsList.Concat(ActiveModsList).Where(x => x.IsEnabled).Reverse())
            {
                string filePath = mod.Source.FolderName;
                if (!mod.IsInRootFolder)
                {
                    string subFolder = Directory.GetParent(mod.Source.FolderPath).Name;
                    filePath = string.Format("{0}/{1}", subFolder, filePath);
                }
                NewActiveModsList.Add(filePath);
            }


            S3AIRActiveMods.Save(NewActiveModsList);
            UpdateModsList(true);

            void UpdateMods(AIR_API.Mod item)
            {
                if (item.IsEnabled != item.EnabledLocal)
                {
                    if (item.IsEnabled == true) EnableMod(item);
                    else DisableMod(item);
                }
            }
        }

        #endregion

        #region Collect Mods

        public static void FetchMods()
        {
            for (int i = 0; i < ModsList.Count; i++)
            {
                ModsList[i].DisposeImage();
                ModsList[i] = null;
            }
            ModsList.Clear();
            ModsList = new List<ModViewerItem>();

            for (int i = 0; i < ActiveModsList.Count; i++)
            {
                ActiveModsList[i].DisposeImage();
                ActiveModsList[i] = null;
            }
            ActiveModsList.Clear();
            ActiveModsList = new List<ModViewerItem>();

            GetAllModContainingSubFolders();
            PraseMods();
            UpdateNewModsListItems();
        }
        public static void GetAllModContainingSubFolders()
        {
            Instance.ModViewer.FolderView.Items.Clear();
            List<SubDirectoryItem> itemsCurrent = new List<SubDirectoryItem>();
            List<SubDirectoryItem> itemsOld = new List<SubDirectoryItem>();
            List<SubDirectoryItem> removedItems = new List<SubDirectoryItem>();

            foreach (SubDirectoryItem item in Instance.ModViewer.FolderView.Items)
            {
                itemsOld.Add(item);
            }

            DirectoryInfo d = new DirectoryInfo(Management.ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            itemsCurrent.Add(new SubDirectoryItem(Management.UserLanguage.BaseModFolderString(), Management.ProgramPaths.Sonic3AIRModsFolder));
            itemsCurrent.Add(new SubDirectoryItem("All Disabled Mods","")); //TODO : Add User Translation
            foreach (DirectoryInfo folder in folders)
            {
                var files = folder.GetFiles();
                var isMod = (files.Where(x => x.Name == "mod.json").FirstOrDefault() != null);
                if (!isMod)
                {
                    itemsCurrent.Add(new SubDirectoryItem(Management.UserLanguage.SubModFolderString(folder.Name), folder.FullName));
                }
            }

            foreach (var item in itemsOld)
            {
                if (!itemsCurrent.Contains(item))
                {
                    removedItems.Add(item);
                }
            }

            foreach (var item in itemsCurrent)
            {
                if (removedItems.Contains(item))
                {
                    if (itemsOld.Contains(item)) Instance.ModViewer.FolderView.Items.Remove(item);
                }
                else
                {
                    if (!itemsOld.Contains(item)) Instance.ModViewer.FolderView.Items.Add(item);
                }
            }

        }
        public static void PraseMods()
        {
            IList<Tuple<ModViewerItem, int>> ActiveMods = new List<Tuple<ModViewerItem, int>>();
            ModSearch(new DirectoryInfo(Management.ProgramPaths.Sonic3AIRModsFolder));
            AddActiveMods();

            void ModSearch(DirectoryInfo d, bool isSubFolder = false)
            {
                DirectoryInfo[] folders = d.GetDirectories();
                foreach (DirectoryInfo folder in folders)
                {
                    DirectoryInfo f = new DirectoryInfo(folder.FullName);
                    var root = f.GetFiles("mod.json").FirstOrDefault();
                    if (root != null)
                    {
                        PraseMod(root, folder, isSubFolder, d.Name);
                    }
                    else
                    {
                        if (!isSubFolder) ModSearch(new DirectoryInfo(folder.FullName), true);
                    }
                }
            }

            void AddActiveMods()
            {
                foreach (var enabledMod in ActiveMods.OrderBy(x => x.Item2).ToList())
                {
                    ActiveModsList.Insert(0, enabledMod.Item1);
                }
            }

            void PraseMod(FileInfo root, DirectoryInfo folder, bool isSubFolder = false, string subFolderName = "")
            {
                try
                {
                    var mod = new AIR_API.Mod(root);
                    string modPath = mod.FolderName;
                    if (isSubFolder) modPath = string.Format("{0}/{1}", subFolderName, mod.FolderName);
                    if (S3AIRActiveMods.ActiveMods.Contains(modPath))
                    {
                        mod.IsEnabled = true;
                        mod.EnabledLocal = true;
                        ActiveMods.Add(new Tuple<ModViewerItem, int>(new ModViewerItem(mod, !isSubFolder), S3AIRActiveMods.ActiveMods.IndexOf(modPath)));
                    }
                    else
                    {
                        mod.IsEnabled = false;
                        mod.EnabledLocal = false;
                        ModsList.Add(new ModViewerItem(mod, !isSubFolder));
                    }
                }
                catch (Newtonsoft.Json.JsonReaderException ex)
                {
                    MessageBox.Show(Management.UserLanguage.LegacyModError1(folder.Name, ex.Message));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Management.UserLanguage.LegacyModError2(folder.Name, ex.Message));
                }
            }

        }

        #endregion

        #region Management Methods

        public static void EditModConfig(ref ModManager Instance)
        {
            if (Instance.ModViewer.SelectedItem != null)
            {
                var item = (Instance.ModViewer.SelectedItem as ModViewerItem);
                var parent = Instance as Window;
                ConfigEditorDialog cfg = new ConfigEditorDialog(ref parent);
                if (cfg.ShowConfigEditDialog(item.Source).Value == true)
                {
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.Name = cfg.EditorNameField.Text;
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.Author = cfg.EditorAuthorField.Text;
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.Description = cfg.EditorDescriptionField.Text;
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.URL = cfg.EditorURLField.Text;
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.GameVersion = cfg.EditorGameVersionField.Text;
                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.ModVersion = cfg.EditorModVersionField.Text;

                    (Instance.ModViewer.SelectedItem as ModViewerItem).Source.Save();
                    Management.ModManagement.UpdateModsList(true);
                }
            }
        }
        public static void DisableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Remove(mod.FolderName);
        }
        public static void EnableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Add(mod.FolderName);
        }
        public static void RemoveSubFolder(string subFolderToRemove)
        {
            string item = Path.GetDirectoryName(subFolderToRemove);
            if (MessageBox.Show(string.Format(Program.LanguageResource.GetString("RemoveSubFolderWarning"), item), Program.LanguageResource.GetString("ApplicationTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Management.FileManagement.WipeFolderContents(subFolderToRemove);
                Directory.Delete(subFolderToRemove);
                new Action(ModManager.UpdateUIFromInvoke).Invoke();
            }


        }
        public static void MoveMod(AIR_API.Mod modToMove, string path, bool updateUI = true)
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

            if (updateUI) new Action(ModManager.UpdateUIFromInvoke).Invoke();
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
                string OriginalFolderName = ExistingMod.FolderName;
                string NewFolderName = ExistingMod.FolderName;
                string PathLocation = Directory.GetParent(newPath).FullName;
                int index = 1;
                while (Directory.Exists(PathLocation + "\\" + NewFolderName))
                {
                    NewFolderName = $"{OriginalFolderName}({index})";
                }
                string ModPath = PathLocation + "\\" + NewFolderName;
                Directory.Move(ExistingMod.FolderPath, ModPath);
            }

            void MoveMod()
            {
                Directory.Move(ExistingMod.FolderPath, newPath);
            }
            void DeleteOldMod()
            {
                Management.FileManagement.WipeFolderContents(newPath);
                Directory.Delete(newPath);
            }

            #endregion
        }
        public static void AddNewModSubfolder(object selectedItem)
        {
            string newFolderName = Program.LanguageResource.GetString("NewSubFolderEntryName");
            DialogResult result;
            result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption1"));
            while (Directory.Exists(Path.Combine(Management.ProgramPaths.Sonic3AIRModsFolder, newFolderName)) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
            {
                result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption2"));
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newDirectoryPath = Path.Combine(Management.ProgramPaths.Sonic3AIRModsFolder, newFolderName);
                Directory.CreateDirectory(newDirectoryPath);
                if (selectedItem != null && selectedItem is ModViewerItem)
                {
                    MoveMod((selectedItem as ModViewerItem).Source, newDirectoryPath);
                }
            }
        }
        public static void DownloadMod(string url, bool isBackground = false)
        {
            Management.DownloaderAPI.DownloadFile(url, Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder, DownloadModCompleted, isBackground);
        }

        #endregion

        #region Action Methods
        public static void AddModsFromURL()
        {
            string url = "";
            if (ExtraDialog.ShowInputDialog(ref url, Program.LanguageResource.GetString("EnterModURL")) == DialogResult.OK)
            {
                if (url != "") MessageBox.Show(Program.LanguageResource.GetString("InvalidURL"), Program.LanguageResource.GetString("InvalidURL"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) MessageBox.Show(Program.LanguageResource.GetString("InvalidURL"), Program.LanguageResource.GetString("InvalidURL"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else DownloadMod(url, false);
            }

        }
        public static void AddMods()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{Program.LanguageResource.GetString("ModFileDialogFilter")} (*.zip;*.7z;*.rar)|*.zip;*.7z;*.rar",
                Title = Program.LanguageResource.GetString("ModFileDialogTitle"),
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in ofd.FileNames)
                {
                    ImportMod(filename);
                }
                new Action(ModManager.UpdateUIFromInvoke).Invoke();
            }
        }
        public static void RemoveMods()
        {
            if (Instance.ModViewer.SelectedItem != null)
            {
                if (Instance.ModViewer.SelectedItems.Count > 1)
                {
                    List<AIR_API.Mod> ModsToRemove = new List<AIR_API.Mod>();
                    foreach (var items in Instance.ModViewer.SelectedItems) ModsToRemove.Add((items as ModViewerItem).Source);
                    RemoveMods(ModsToRemove);
                }
                else
                {
                    RemoveMod((Instance.ModViewer.SelectedItem as ModViewerItem).Source);
                }
            }


            void RemoveMods(List<AIR_API.Mod> modsToRemove)
            {
                //TODO : Language Translations
                if (MessageBox.Show($"Are you sure you want to delete {modsToRemove.Count} Mod(s)? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (var modToRemove in modsToRemove)
                    {
                        Management.FileManagement.WipeFolderContents(modToRemove.FolderPath);
                        Directory.Delete(modToRemove.FolderPath);
                    }
                    new Action(ModManager.UpdateUIFromInvoke).Invoke();
                }
            }
            void RemoveMod(AIR_API.Mod modToRemove)
            {
                //TODO : Language Translations
                if (MessageBox.Show($"Are you sure you want to delete {modToRemove.Name}? This cannot be undone!", "Sonic 3 AIR Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Management.FileManagement.WipeFolderContents(modToRemove.FolderPath);
                    Directory.Delete(modToRemove.FolderPath);
                    new Action(ModManager.UpdateUIFromInvoke).Invoke();
                }
            }
        }
        public static void MoveModToTop()
        {
            int index = Instance.ModViewer.ActiveSelectedIndex;
            if (index != 0)
            {
                ActiveModsList.Move(index, 0);
                UpdateModsList();
                Instance.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(0);
            }
        }
        public static void MoveModUp()
        {
            int index = Instance.ModViewer.ActiveSelectedIndex;
            if (index != 0)
            {
                ActiveModsList.Move(index, index - 1);
                UpdateModsList();
                Instance.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(index - 1);
            }
        }
        public static void MoveModDown()
        {
            int index = Instance.ModViewer.ActiveSelectedIndex;
            if (index != ActiveModsList.Count - 1)
            {
                ActiveModsList.Move(index, index + 1);
                UpdateModsList();
                Instance.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(index + 1);
            }
        }
        public static void MoveModToBottom()
        {
            int index = Instance.ModViewer.ActiveSelectedIndex;
            if (index != ActiveModsList.Count - 1)
            {
                ActiveModsList.Move(index, ActiveModsList.Count - 1);
                UpdateModsList();
                Instance.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(ActiveModsList.Count - 1);
            }
        }

        #endregion

        #region Cleanup Methods
        public static void CleanUpTempModsFolder()
        {
            Management.FileManagement.WipeFolderContents(Management.ProgramPaths.Sonic3AIR_MM_TempModsFolder);
        }
        public static void CleanUpAPIRequests()
        {
            Management.FileManagement.WipeFolderContents(Management.ProgramPaths.Sonic3AIR_MM_GBRequestsFolder);
        }

        #endregion

    }
}
