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
    public class ModManagement
    {
        public AIR_API.ActiveModsList S3AIRActiveMods;
        public IList<ModViewerItem> ModsList = new List<ModViewerItem>();
        public IList<ModViewerItem> ActiveModsList = new List<ModViewerItem>();

        private ModManager Parent;
        public ModManagement(ModManager _parent)
        {
            Parent = _parent;
        }



        public void UpdateModsList(bool FullReload = false)
        {
            if (File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                int PreviousSubFolderIndex = Parent.ModViewer.FolderView.SelectedIndex;
                Parent.modErrorTextPanel.Visibility = Visibility.Collapsed;
                UpdateModListItemCheck(true);
                if (FullReload) FetchMods();
                else UpdateNewModsListItems();
                MainDataModel.RefreshSelectedModProperties(ref Parent);
                UpdateModListItemCheck(false);
                if (Parent.ModViewer.FolderView.Items.Count > PreviousSubFolderIndex && PreviousSubFolderIndex != -1) Parent.ModViewer.FolderView.SelectedIndex = PreviousSubFolderIndex;
            }
            else
            {
                Parent.modErrorTextPanel.Visibility = Visibility.Visible;
            }

        }

        public void UpdateActiveAndInactiveModLists()
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

        public void UpdateNewModsListItems()
        {
            ProgramPaths.ValidateSettingsAndActiveMods(ref S3AIRActiveMods, ref MainDataModel.S3AIRSettings);

            Parent.ModViewer.Clear();

            UpdateActiveAndInactiveModLists();


            foreach (ModViewerItem mod in ModsList)
            {
                Parent.ModViewer.View.Items.Add(mod);
            }

            foreach (ModViewerItem mod in ActiveModsList)
            {
                Parent.ModViewer.ActiveView.Items.Add(mod);
            }

            string CurrentFolderPath = ProgramPaths.Sonic3AIRModsFolder;
            if (Parent.ModViewer.FolderView.SelectedItem != null) CurrentFolderPath = (Parent.ModViewer.FolderView.SelectedItem as SubDirectoryItem).FilePath;

            foreach (var item in Parent.ModViewer.View.Items)
            {
                ModViewerItem mod = (ModViewerItem)item;
                if (CurrentFolderPath != null && (Parent.ModViewer.FolderView.SelectedIndex == 0 || Parent.ModViewer.FolderView.SelectedIndex == -1) && mod.IsInRootFolder)
                {
                    mod.Visibility = Visibility.Visible;
                }
                else if (!(Parent.ModViewer.FolderView.SelectedIndex == 0 || Parent.ModViewer.FolderView.SelectedIndex == -1))
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

            Parent.ModViewer.Refresh();

        }

        public void FetchMods()
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

            Parent.LegacyLoadingCheckbox.IsChecked = S3AIRActiveMods.UseLegacyLoading;
        }

        public void GetAllModContainingSubFolders()
        {
            Parent.ModViewer.FolderView.Items.Clear();
            List<SubDirectoryItem> itemsCurrent = new List<SubDirectoryItem>();
            List<SubDirectoryItem> itemsOld = new List<SubDirectoryItem>();
            List<SubDirectoryItem> removedItems = new List<SubDirectoryItem>();

            foreach (SubDirectoryItem item in Parent.ModViewer.FolderView.Items)
            {
                itemsOld.Add(item);
            }

            DirectoryInfo d = new DirectoryInfo(ProgramPaths.Sonic3AIRModsFolder);
            DirectoryInfo[] folders = d.GetDirectories();
            itemsCurrent.Add(new SubDirectoryItem(UserLanguage.BaseModFolderString(), ProgramPaths.Sonic3AIRModsFolder));
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
                    if (itemsOld.Contains(item)) Parent.ModViewer.FolderView.Items.Remove(item);
                }
                else
                {
                    if (!itemsOld.Contains(item)) Parent.ModViewer.FolderView.Items.Add(item);
                }
            }

        }

        public void PraseMods()
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
                        ActiveMods.Add(new Tuple<ModViewerItem, int>(new ModViewerItem(this, mod, !isSubFolder), S3AIRActiveMods.ActiveMods.IndexOf(modPath)));
                    }
                    else
                    {
                        mod.IsEnabled = false;
                        mod.EnabledLocal = false;
                        ModsList.Add(new ModViewerItem(this, mod, !isSubFolder));
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

        public void MoveModToTop()
        {
            int index = Parent.ModViewer.ActiveSelectedIndex;
            if (index != 0)
            {
                ActiveModsList.Move(index, 0);
                UpdateModsList();
                Parent.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(0);
            }
        }

        public void MoveModUp()
        {
            int index = Parent.ModViewer.ActiveSelectedIndex;
            if (index != 0)
            {
                ActiveModsList.Move(index, index - 1);
                UpdateModsList();
                Parent.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(index - 1);
            }
        }

        public void MoveModDown()
        {
            int index = Parent.ModViewer.ActiveSelectedIndex;
            if (index != ActiveModsList.Count - 1)
            {
                ActiveModsList.Move(index, index + 1);
                UpdateModsList();
                Parent.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(index + 1);
            }
        }

        public void MoveModToBottom()
        {
            int index = Parent.ModViewer.ActiveSelectedIndex;
            if (index != ActiveModsList.Count - 1)
            {
                ActiveModsList.Move(index, ActiveModsList.Count - 1);
                UpdateModsList();
                Parent.ModViewer.ActiveSelectedItem = ActiveModsList.ElementAt(ActiveModsList.Count - 1);
            }
        }

        public void RefreshMoveToSubfolderList()
        {
            List<MenuItem> ItemsToRemove = new List<MenuItem>();
            foreach (var item in Parent.moveModToSubFolderMenuItem.Items)
            {
                if (item is MenuItem && !item.Equals(Parent.addNewModSubfolderMenuItem))
                {
                    ItemsToRemove.Add(item as MenuItem);
                }
            }
            foreach (var item in ItemsToRemove)
            {
                int index = Parent.moveModToSubFolderMenuItem.Items.IndexOf(item);
                (Parent.moveModToSubFolderMenuItem.Items[index] as MenuItem).Click -= SubDirectoryMove_Click;
                Parent.moveModToSubFolderMenuItem.Items.Remove(item);
            }
            ItemsToRemove.Clear();


            foreach (var item in Parent.ModViewer.FolderView.Items)
            {
                SubDirectoryItem realItem;
                if (item is SubDirectoryItem) realItem = item as SubDirectoryItem;
                else realItem = null;

                if (realItem != null)
                {
                    var menuItem = GenerateSubDirectoryToolstripItem(realItem.FileName, realItem.FilePath);
                    Parent.moveModToSubFolderMenuItem.Items.Add(menuItem);
                }
            }

        }

        public MenuItem GenerateSubDirectoryToolstripItem(string name, string filepath)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = filepath;
            item.Click += SubDirectoryMove_Click;
            return item;
        }

        public void SubDirectoryMove_Click(object sender, RoutedEventArgs e)
        {
            if (Parent.ModViewer.View.SelectedItem != null && Parent.ModViewer.View.SelectedItem is ModViewerItem)
            {
                FileManagement.MoveMod((Parent.ModViewer.View.SelectedItem as ModViewerItem).Source, (sender as MenuItem).Tag.ToString());
            }
            else if (MainDataModel.ModManagement.S3AIRActiveMods.UseLegacyLoading)
            {
                if (Parent.ModViewer.ActiveView.SelectedItem != null && Parent.ModViewer.ActiveView.SelectedItem is ModViewerItem)
                {
                    FileManagement.MoveMod((Parent.ModViewer.ActiveView.SelectedItem as ModViewerItem).Source, (sender as MenuItem).Tag.ToString());
                }
            }



        }

        public void Save()
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

        public void UpdateModListItemCheck(bool shouldEnd)
        {
            if (shouldEnd) ModViewer.ItemCheck = null;
            else ModViewer.ItemCheck = ModsList_ItemCheck;
        }

        private void ModsList_ItemCheck()
        {
            UpdateModsList();
        }

        public void DisableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Remove(mod.FolderName);
        }

        public void EnableMod(AIR_API.Mod mod)
        {
            S3AIRActiveMods.ActiveMods.Add(mod.FolderName);
        }

        public void ToggleLegacyModManagement(bool value)
        {
            this.S3AIRActiveMods.UseLegacyLoading = value;
            this.Save();
        }
    }
}
