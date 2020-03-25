using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sonic3AIR_ModManager.Management
{
    public static class MMSettingsManagement
    {


        public static void CollectModCollectionMenuItemsDictionary(ref ModManager Instance)
        {

            Instance.LoadModCollectionMenuItem.RecentItemsSource = null;
            Instance.RenameModCollectionMenuItem.RecentItemsSource = null;
            Instance.DeleteModCollectionMenuItem.RecentItemsSource = null;
            Instance.SaveModCollectonAsMenuItem.RecentItemsSource = null;
            Instance.AddFromExistingModCollectionMenuItem.RecentItemsSource = null;

            if (Management.MainDataModel.ModCollectionMenuItems.ContainsKey(0)) Management.MainDataModel.ModCollectionMenuItems[0].Clear();
            if (Management.MainDataModel.ModCollectionMenuItems.ContainsKey(1)) Management.MainDataModel.ModCollectionMenuItems[1].Clear();
            if (Management.MainDataModel.ModCollectionMenuItems.ContainsKey(2)) Management.MainDataModel.ModCollectionMenuItems[2].Clear();
            if (Management.MainDataModel.ModCollectionMenuItems.ContainsKey(3)) Management.MainDataModel.ModCollectionMenuItems[3].Clear();
            if (Management.MainDataModel.ModCollectionMenuItems.ContainsKey(4)) Management.MainDataModel.ModCollectionMenuItems[4].Clear();

            Management.MainDataModel.ModCollectionMenuItems.Clear();
            for (int i = 0; i < 5; i++)
            {
                Management.MainDataModel.ModCollectionMenuItems.Add(i, CollectModCollectionsMenuItems());
            }

            Instance.LoadModCollectionMenuItem.RecentItemsSource = Management.MainDataModel.ModCollectionMenuItems[0];
            Instance.RenameModCollectionMenuItem.RecentItemsSource = Management.MainDataModel.ModCollectionMenuItems[1];
            Instance.DeleteModCollectionMenuItem.RecentItemsSource = Management.MainDataModel.ModCollectionMenuItems[2];
            Instance.SaveModCollectonAsMenuItem.RecentItemsSource = Management.MainDataModel.ModCollectionMenuItems[3];
            Instance.AddFromExistingModCollectionMenuItem.RecentItemsSource = Management.MainDataModel.ModCollectionMenuItems[4];


        }

        public static List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectModCollectionsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in Management.MainDataModel.Settings.ModCollections)
            {
                collections.Add(new GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem(collection.Name, collection));
            }
            return collections;
        }

        public static List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> CollectLaunchPresetsMenuItems()
        {
            List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem> collections = new List<GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem>();
            foreach (var collection in Management.MainDataModel.Settings.LaunchPresets)
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

        public static void AppendFromExistingModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            Management.ModManagement.Save();
            var collectionToAppend = (e.Content as Settings.ModCollection).Mods;
            var currentCollection = Management.ModManagement.S3AIRActiveMods;
            currentCollection.ActiveMods.InsertRange(0, collectionToAppend.ActiveMods);
            Management.ModManagement.S3AIRActiveMods.Save(currentCollection.ActiveMods);
            Management.ModManagement.UpdateModsList(true);
        }

        public static void LoadModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            Management.ModManagement.S3AIRActiveMods.Save(collection.Mods);
            Management.ModManagement.UpdateModsList(true);
        }

        public static void RenameModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string name = collection.Name;
            string caption = Management.UserLanguage.GetOutputString("ModCollectionDialog_Caption_Rename");
            string message = Management.UserLanguage.GetOutputString("ModCollectionDialog_Message_Rename");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Management.ModManagement.Save();
                int collectionsIndex = Management.MainDataModel.Settings.ModCollections.IndexOf(collection);
                Management.MainDataModel.Settings.ModCollections[collectionsIndex].Name = name;
                SaveModManagerSettings();
            }
        }

        public static void DeleteModCollection(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            var collection = e.Content as Settings.ModCollection;
            string caption = Management.UserLanguage.GetOutputString("ModCollectionDialog_Caption_Delete");
            string message = string.Format(Management.UserLanguage.GetOutputString("ModCollectionDialog_Message_Delete"), collection.Name);
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Management.MainDataModel.Settings.ModCollections.Remove(collection);
                SaveModManagerSettings();
            }
        }

        public static void SaveModCollectonAs(GenerationsLib.WPF.Controls.RecentsListMenuItem.RecentItem e)
        {
            string caption = Management.UserLanguage.GetOutputString("ModCollectionDialog_Caption_Replace");
            string message = Management.UserLanguage.GetOutputString("ModCollectionDialog_Message_Replace");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Management.ModManagement.Save();
                var collection = e.Content as Settings.ModCollection;
                int collectionsIndex = Management.MainDataModel.Settings.ModCollections.IndexOf(collection);
                Management.MainDataModel.Settings.ModCollections[collectionsIndex] = new Sonic3AIR_ModManager.Settings.ModCollection(Management.ModManagement.S3AIRActiveMods.ActiveClass, collection.Name);
                SaveModManagerSettings();
            }
        }

        public static void DeleteAllModCollections()
        {
            string caption = Management.UserLanguage.GetOutputString("ModCollectionDialog_Caption_DeleteAll");
            string message = Management.UserLanguage.GetOutputString("ModCollectionDialog_Message_DeleteAll");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Management.MainDataModel.Settings.ModCollections.Clear();
                SaveModManagerSettings();
            }
        }

        public static void SaveModCollecton()
        {
            string name = Management.UserLanguage.GetOutputString("ModCollectionDialog_Name_Save");
            string caption = Management.UserLanguage.GetOutputString("ModCollectionDialog_Caption_Save");
            string message = Management.UserLanguage.GetOutputString("ModCollectionDialog_Message_Save");
            var result = ExtraDialog.ShowInputDialog(ref name, caption, message);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Management.ModManagement.Save();
                Management.MainDataModel.Settings.ModCollections.Add(new Sonic3AIR_ModManager.Settings.ModCollection(Management.ModManagement.S3AIRActiveMods.ActiveClass, name));
                SaveModManagerSettings();
            }

        }


        public static void SaveModManagerSettings()
        {
            Management.MainDataModel.Settings.Save();
        }

        public static void LoadModManagerSettings()
        {
            try
            {
                Program.Log.InfoFormat("Loading Personal Settings...");
                if (!System.IO.File.Exists(Management.ProgramPaths.Sonic3AIR_MM_SettingsFile)) System.IO.File.Create(Management.ProgramPaths.Sonic3AIR_MM_SettingsFile).Close();
                Management.MainDataModel.Settings = null;
                Management.MainDataModel.Settings = new Settings.ModManagerSettings(Management.ProgramPaths.Sonic3AIR_MM_SettingsFile);
            }
            catch (Exception ex)
            {
                Program.Log.ErrorFormat("[Fatal Error] {0}", ex.Message);
            }

        }



    }
}
