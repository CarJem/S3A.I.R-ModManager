using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace Sonic3AIR_ModManager
{
    public static class ModManagement
    {
        public static AIR_API.ActiveModsList S3AIRActiveMods;
        public static IList<ModViewerItem> ModsList = new List<ModViewerItem>();
        public static IList<ModViewerItem> ActiveModsList = new List<ModViewerItem>();

        private static ModManager Instance;

        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
        }

        public static void UpdateModsList(bool FullReload = false)
        {
            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                int PreviousSubFolderIndex = Instance.ModViewer.FolderView.SelectedIndex;
                Instance.modErrorTextPanel.Visibility = Visibility.Collapsed;
                UpdateModListItemCheck(true);
                if (FullReload) FetchMods();
                else UpdateNewModsListItems();
                MainDataModel.RefreshSelectedModProperties(ref Instance);
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
            ProgramPaths.ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref MainDataModel.S3AIRSettings);

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

            string CurrentFolderPath = ProgramPaths.Sonic3AIRModsFolder;
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

            Instance.LegacyLoadingCheckbox.IsChecked = S3AIRActiveMods.UseLegacyLoading;
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

            DirectoryInfo d = new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            itemsCurrent.Add(new SubDirectoryItem(UserLanguage.BaseModFolderString(), ProgramPaths.Sonic3AIRModsFolder));
            itemsCurrent.Add(new SubDirectoryItem("All - Show All Mods (including Subdirectories)","")); //TODO : Add User Translation
            foreach (DirectoryInfo folder in folders)
            {
                var files = folder.GetFiles();
                var isMod = (files.Where(x => x.Name == "mod.json").FirstOrDefault() != null);
                if (!isMod)
                {
                    itemsCurrent.Add(new SubDirectoryItem(UserLanguage.SubModFolderString(folder.Name), folder.FullName));
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
            ModSearch(new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder));
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
                    MessageBox.Show(UserLanguage.LegacyModError1(folder.Name, ex.Message));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(UserLanguage.LegacyModError2(folder.Name, ex.Message));
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
                (Instance.moveModToSubFolderMenuItem.Items[index] as MenuItem).Click -= SubDirectoryMove_Click;
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
            item.Click += SubDirectoryMove_Click;
            return item;
        }

        public static void SubDirectoryMove_Click(object sender, RoutedEventArgs e)
        {
            if (Instance.ModViewer.View.SelectedItem != null && Instance.ModViewer.View.SelectedItem is ModViewerItem)
            {
                FileManagement.MoveMod((Instance.ModViewer.View.SelectedItem as ModViewerItem).Source, (sender as MenuItem).Tag.ToString());
            }
            else if (ModManagement.S3AIRActiveMods.UseLegacyLoading)
            {
                if (Instance.ModViewer.ActiveView.SelectedItem != null && Instance.ModViewer.ActiveView.SelectedItem is ModViewerItem)
                {
                    FileManagement.MoveMod((Instance.ModViewer.ActiveView.SelectedItem as ModViewerItem).Source, (sender as MenuItem).Tag.ToString());
                }
            }



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

        public static void UpdateModListItemCheck(bool shouldEnd)
        {
            if (shouldEnd) ModViewer.ItemCheck = null;
            else ModViewer.ItemCheck = ModsList_ItemCheck;
        }

        private static void ModsList_ItemCheck()
        {
            UpdateModsList();
        }

        public static void DisableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Remove(mod.FolderName);
        }

        public static void EnableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Add(mod.FolderName);
        }

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
                    ModManagement.UpdateModsList(true);
                }
            }
        }

        public static void ToggleLegacyModManagement(bool value)
        {
            ModManagement.S3AIRActiveMods.UseLegacyLoading = value;
            ModManagement.Save();
        }
    }
}
