using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for ModViewer.xaml
    /// </summary>
    public partial class ModViewer : UserControl
    {
        public static Action ItemCheck;
        public SelectionChangedEventHandler SelectionChanged;

        #region List Access Variables
        public object SelectedItem { get => GetSelectedItem(); set => SetSelectedItem(value); }
        public object SelectedFolderItem { get => GetSelectedFolderItem(); set => SetSelectedFolderItem(value); }
        public int ActiveSelectedIndex { get => GetSelectedIndex(); set => SetSelectedIndex(value); }

        public object ActiveSelectedItem { get => GetActiveSelectedItem(); set => SetActiveSelectedItem(value); }
        private object GetActiveSelectedItem()
        {
            return ActiveView.SelectedItem;
        }

        private void SetActiveSelectedItem(object value)
        {
            ActiveView.SelectedItem = value;
        }

        private int GetSelectedIndex()
        {
            return ActiveView.SelectedIndex;
        }

        private void SetSelectedIndex(int value)
        {
            ActiveView.SelectedIndex = value;
        }

        private object GetSelectedFolderItem()
        {
            return FolderView.SelectedItem;
        }

        private void SetSelectedFolderItem(object value)
        {
            FolderView.SelectedItem = value;
        }

        private object GetSelectedItem()
        {
            if (ActiveView.SelectedItem != null) return ActiveView.SelectedItem;
            else return View.SelectedItem;
        }

        private void SetSelectedItem(object value)
        {
            if (View.Items.Contains(value)) View.SelectedItem = value;
            else ActiveView.SelectedItem = value;
        }

        #endregion

        public ModViewer()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                // Do runtime stuff
                UpdateSelectedFolderLabel();
                var Instance = this;
                UserLanguage.ApplyLanguage(ref Instance);
            }



        }

        #region List Access Methods

        public void Add(ModViewerItem item)
        {
            if (item.IsEnabled)
            {
                ActiveView.Items.Add(item);
            }
            else
            {
                View.Items.Add(item);
            }

        }

        public void Refresh()
        {
            View.Items.Refresh();
            ActiveView.Items.Refresh();
        }

        public void Clear()
        {
            View.Items.Clear();
            ActiveView.Items.Clear();
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {

            if (View.SelectedItem != null && View.SelectedItem is ModViewerItem)
            {
                var item = View.SelectedItem as ModViewerItem;
                if (e.Key == Key.Enter)
                {
                    item.IsEnabled = !item.IsEnabled;
                }
            }

        }

        private void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveView.SelectedItem != null && View.SelectedItem != null) ActiveView.SelectedItem = null;
            SelectionChanged?.Invoke(sender, e);
        }

        private void ActiveView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveView.SelectedItem != null && View.SelectedItem != null) View.SelectedItem = null;
            SelectionChanged?.Invoke(sender, e);
        }

        private void FolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
            UpdateSelectedFolderLabel();

            if (FolderView.SelectedItem != null)
            {
                if ((FolderView.SelectedItem as SubDirectoryItem).FilePath != ProgramPaths.Sonic3AIRModsFolder)
                {
                    RemoveCurrentFolderMenuItem.IsEnabled = true;
                }
                else
                {
                    RemoveCurrentFolderMenuItem.IsEnabled = false;
                }
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeFolderButton.ContextMenu.IsOpen = true;
            UpdateSelectedFolderLabel();
        }

        private void UpdateSelectedFolderLabel()
        {
            if (FolderView.SelectedItem == null)
            {
                FolderView.SelectedIndex = 0;
            }
        }

        private void RemoveCurrentFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileManagement.RemoveSubFolder((FolderView.SelectedValue as SubDirectoryItem).FilePath);
        }

        private void FolderListHost_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void AddNewSubFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {

            string newFolderName = Program.LanguageResource.GetString("NewSubFolderEntryName");
            System.Windows.Forms.DialogResult result;
            result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption1"));
            while (Directory.Exists(System.IO.Path.Combine(ProgramPaths.Sonic3AIRModsFolder, newFolderName)) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
            {
                result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption2"));
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newDirectoryPath = System.IO.Path.Combine(ProgramPaths.Sonic3AIRModsFolder, newFolderName);
                Directory.CreateDirectory(newDirectoryPath);
                ModManager.Instance.ModManagement.UpdateModsList(true);
            }
        }
    }


    public class SubDirectoryItem
    {
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";

        public override string ToString()
        {
            return FileName;
        }

        public SubDirectoryItem(string name, string path)
        {
            FileName = name;
            FilePath = path;
        }
    }



    public class ModViewerItem
    {
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public bool IsInRootFolder { get; set; } = true;

        public string Name { get => Source.Name; set => Source.Name = value; }
        public string Description { get => Source.Description; set => Source.Description = value; }
        public string Author { get => Source.Author; set => Source.Author = value; }

        public string Version { get => Source.ModVersion; set => Source.ModVersion = value; }

        public string TechName { get => Source.TechnicalName; set => Source.TechnicalName = value; }

        public ImageSource Image { get => GetImage(); }

        private BitmapImage SourceImage;

        public bool IsEnabled { get => GetEnabledState(); set => SetEnabledState(value); }

        private bool GetEnabledState()
        {
            return Source.IsEnabled;
        }

        public void DisposeImage()
        {
            SourceImage = null;
        }

        private ImageSource GetImage()
        {
            if (SourceImage != null)
            {
                SourceImage = null;
            } 
            string ImageLocation = System.IO.Path.Combine(Source.FolderPath, "icon.png");
            if (System.IO.File.Exists(ImageLocation))
            {
                SourceImage = new BitmapImage();
                SourceImage.BeginInit();
                SourceImage.CacheOption = BitmapCacheOption.None;
                SourceImage.UriCachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                SourceImage.CacheOption = BitmapCacheOption.OnLoad;
                SourceImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                SourceImage.UriSource = new Uri(ImageLocation);
                SourceImage.EndInit();

                if (SourceImage.PixelWidth != SourceImage.PixelHeight) return DefaultModImage(Sonic3AIR_ModManager.Properties.Resources.ModIconDefault);
                else return SourceImage;
            }
            else
            {
                return DefaultModImage(Sonic3AIR_ModManager.Properties.Resources.ModIconDefault);
            }

        }

        private BitmapImage DefaultModImage(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void SetEnabledState(bool value)
        {
            Source.IsEnabled = value;
            ModViewer.ItemCheck?.Invoke();
        }

        public AIR_API.Mod Source { get; set; }

        public ModViewerItem(AIR_API.Mod _source, bool root = true)
        {
            Source = _source;
            IsInRootFolder = root;
        }
    }
}
