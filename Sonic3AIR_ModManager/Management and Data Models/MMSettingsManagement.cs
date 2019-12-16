using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sonic3AIR_ModManager
{
    public static class MMSettingsManagement
    {


        public static void CollectModCollectionMenuItemsDictionary(ref ModManager Instance)
        {

            Instance.LoadModCollectionMenuItem.RecentItemsSource = null;
            Instance.RenameModCollectionMenuItem.RecentItemsSource = null;
            Instance.DeleteModCollectionMenuItem.RecentItemsSource = null;
            Instance.SaveModCollectonAsMenuItem.RecentItemsSource = null;

            if (MainDataModel.ModCollectionMenuItems.ContainsKey(0)) MainDataModel.ModCollectionMenuItems[0].Clear();
            if (MainDataModel.ModCollectionMenuItems.ContainsKey(1)) MainDataModel.ModCollectionMenuItems[1].Clear();
            if (MainDataModel.ModCollectionMenuItems.ContainsKey(2)) MainDataModel.ModCollectionMenuItems[2].Clear();
            if (MainDataModel.ModCollectionMenuItems.ContainsKey(3)) MainDataModel.ModCollectionMenuItems[3].Clear();

            MainDataModel.ModCollectionMenuItems.Clear();
            for (int i = 0; i < 4; i++)
            {
                MainDataModel.ModCollectionMenuItems.Add(i, CollectModCollectionsMenuItems());
            }

            Instance.LoadModCollectionMenuItem.RecentItemsSource = MainDataModel.ModCollectionMenuItems[0];
            Instance.RenameModCollectionMenuItem.RecentItemsSource = MainDataModel.ModCollectionMenuItems[1];
            Instance.DeleteModCollectionMenuItem.RecentItemsSource = MainDataModel.ModCollectionMenuItems[2];
            Instance.SaveModCollectonAsMenuItem.RecentItemsSource = MainDataModel.ModCollectionMenuItems[3];


        }

        public static List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectModCollectionsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in MainDataModel.Settings.Options.ModCollections)
            {
                collections.Add(new GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem(collection.Name, collection));
            }
            return collections;
        }

        public static void CollectLaunchPresetsMenuItemsDictionary(ref ModManager Instance)
        {
            Instance.LoadLaunchPresetsMenuItem.RecentItemsSource.Clear();
            Instance.RenameLaunchPresetsMenuItem.RecentItemsSource.Clear();
            Instance.DeleteLaunchPresetsMenuItem.RecentItemsSource.Clear();
            Instance.SaveLaunchPresetAsMenuItem.RecentItemsSource.Clear();

            Instance.LoadLaunchPresetsMenuItem.RecentItemsSource = null;
            Instance.RenameLaunchPresetsMenuItem.RecentItemsSource = null;
            Instance.DeleteLaunchPresetsMenuItem.RecentItemsSource = null;
            Instance.SaveLaunchPresetAsMenuItem.RecentItemsSource = null;

            Instance.LoadLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            Instance.RenameLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            Instance.DeleteLaunchPresetsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();
            Instance.SaveLaunchPresetAsMenuItem.RecentItemsSource = CollectLaunchPresetsMenuItems();

        }

        public static List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectLaunchPresetsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in MainDataModel.Settings.Options.LaunchPresets)
            {
                collections.Add(new GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem(collection.Name, collection));
            }
            return collections;
        }

        public static void LoadLaunchPresets(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        public static void RenameLaunchPresets(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        public static void DeleteLaunchPresets(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        public static void SaveLaunchPresetAs(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {

        }

        public static void DeleteAllLaunchPresets()
        {

        }

        public static void SaveLaunchPreset()
        {

        }

        public static void LoadModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            ModManagement.S3AIRActiveMods.Save(collection.Mods);
            ModManagement.UpdateModsList(true);
        }

        public static void RenameModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string name = collection.Name;
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Rename");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Rename");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ModManagement.Save();
                int collectionsIndex = MainDataModel.Settings.Options.ModCollections.IndexOf(collection);
                MainDataModel.Settings.Options.ModCollections[collectionsIndex].Name = name;
                SaveModManagerSettings();
            }
        }

        public static void DeleteModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Delete");
            string message = string.Format(UserLanguage.GetOutputString("ModCollectionDialog_Message_Delete"), collection.Name);
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                MainDataModel.Settings.Options.ModCollections.Remove(collection);
                SaveModManagerSettings();
            }
        }

        public static void SaveModCollectonAs(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Replace");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Replace");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                ModManagement.Save();
                var collection = e.Content as Settings.ModCollection;
                int collectionsIndex = MainDataModel.Settings.Options.ModCollections.IndexOf(collection);
                MainDataModel.Settings.Options.ModCollections[collectionsIndex] = new Sonic3AIR_ModManager.Settings.ModCollection(ModManagement.S3AIRActiveMods.ActiveClass, collection.Name);
                SaveModManagerSettings();
            }
        }

        public static void DeleteAllModCollections()
        {
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_DeleteAll");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_DeleteAll");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                MainDataModel.Settings.Options.ModCollections.Clear();
                SaveModManagerSettings();
            }
        }

        public static void SaveModCollecton()
        {
            string name = UserLanguage.GetOutputString("ModCollectionDialog_Name_Save");
            string caption = UserLanguage.GetOutputString("ModCollectionDialog_Caption_Save");
            string message = UserLanguage.GetOutputString("ModCollectionDialog_Message_Save");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ModManagement.Save();
                MainDataModel.Settings.Options.ModCollections.Add(new Sonic3AIR_ModManager.Settings.ModCollection(ModManagement.S3AIRActiveMods.ActiveClass, name));
                SaveModManagerSettings();
            }

        }


        public static void SaveModManagerSettings()
        {
            MainDataModel.Settings.Save();
        }

        public static void LoadModManagerSettings()
        {
            MainDataModel.Settings = null;
            MainDataModel.Settings = new Settings.ModManagerSettings(ProgramPaths.Sonic3AIR_MM_SettingsFile);
        }



    }
}
