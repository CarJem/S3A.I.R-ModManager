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

        #region View States
        public ViewSetting CurrentView { get; set; } = ViewSetting.Panel;

        public GridLength SpliterALocationStorage { get; set; } = new GridLength(0.5, GridUnitType.Star);
        public GridLength SpliterBLocationStorage { get; set; } = new GridLength(0.5, GridUnitType.Star);

        #endregion

        #region Mod View Hosts

        public ActiveMods AHost;
        public OtherMods MHost;
        public ModProperties PHost;
        

        public ListView View { get => MHost.View; set => MHost.View = value; }
        public ListView ActiveView { get => AHost.ActiveView; set => AHost.ActiveView = value; }
        public ComboBox FolderView { get => MHost.FolderView; set => MHost.FolderView = value; }
        public MenuItem RemoveCurrentFolderMenuItem { get => MHost.RemoveCurrentFolderMenuItem; set => MHost.RemoveCurrentFolderMenuItem = value; }
        public Button ChangeFolderButton { get => MHost.ChangeFolderButton; set => MHost.ChangeFolderButton = value; }

        private void InitializeHostedComponents()
        {
            AHost = new ActiveMods(this);
            MHost = new OtherMods(this);
            PHost = new ModProperties();
        }

        #endregion

        #region List Access Variables



        public object SelectedItem { get => GetSelectedItem(); set => SetSelectedItem(value); }

        public System.Collections.IList SelectedItems { get => GetSelectedItems(); }
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

        private System.Collections.IList GetSelectedItems()
        {
            if (ActiveView.SelectedItems != null && ActiveView.SelectedItems.Count != 0) return ActiveView.SelectedItems;
            else return View.SelectedItems;
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
            InitializeHostedComponents();
            InitializeComponent();
            ChangeView(CurrentView, true);
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                // Do runtime stuff
                UpdateSelectedFolderLabel();
                var Instance = this;
                Management.UserLanguage.ApplyLanguage(ref Instance);
            }



        }

        public enum ViewSetting : int
        {
            Tabbed = 0,
            Panel = 1,
            NoChange = 3
        }

        public void ChangeView(ViewSetting view, bool UpdateProperties = false)
        {
            CurrentView = view;
            ActiveModsTab.Content = null;
            ModsTab.Content = null;

            TabView.Visibility = Visibility.Collapsed;
            PanelView.Visibility = Visibility.Collapsed;

            PanelViewButton.IsChecked = false;
            TabViewButton.IsChecked = false;

            PanelA.Children.Clear();
            PanelB.Children.Clear();

            PropertiesPanelA.Children.Clear();
            PropertiesPanelB.Children.Clear();


            if (view == ViewSetting.Panel)
            {
                PanelA.Children.Add(AHost);
                PanelB.Children.Add(MHost);

                PropertiesPanelB.Children.Add(PHost);

                PanelView.Visibility = Visibility.Visible;

                PanelViewButton.IsChecked = true;
            }
            else if (view == ViewSetting.Tabbed)
            {
                ActiveModsTab.Content = AHost;
                ModsTab.Content = MHost;

                PropertiesPanelA.Children.Add(PHost);

                TabView.Visibility = Visibility.Visible;

                TabViewButton.IsChecked = true;
            }

            if (UpdateProperties)
            {
                if (!ModPropertiesVisibilitySwitch.IsChecked.Value)
                {
                    SpliterALocationStorage = TabView.RowDefinitions[2].Height;
                    TabView.RowDefinitions[2].Height = new GridLength(0);
                    Splitter1.IsEnabled = false;

                    SpliterBLocationStorage = PanelView.RowDefinitions[2].Height;
                    PanelView.RowDefinitions[2].Height = new GridLength(0);
                    Splitter2.IsEnabled = false;
                }
                else
                {
                    TabView.RowDefinitions[2].Height = SpliterALocationStorage;
                    Splitter1.IsEnabled = true;
                    PanelView.RowDefinitions[2].Height = SpliterBLocationStorage;
                    Splitter2.IsEnabled = true;
                }
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

        public void View_KeyDown(object sender, KeyEventArgs e)
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

        public void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveView.SelectedItem != null && View.SelectedItem != null) ActiveView.SelectedItem = null;
            SelectionChanged?.Invoke(sender, e);
        }

        public void ActiveView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveView.SelectedItem != null && View.SelectedItem != null) View.SelectedItem = null;
            SelectionChanged?.Invoke(sender, e);
        }

        public void FolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
            UpdateSelectedFolderLabel();

            if (FolderView.SelectedItem != null)
            {
                if ((FolderView.SelectedItem as SubDirectoryItem).FilePath != Management.ProgramPaths.Sonic3AIRModsFolder)
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

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FolderView.SelectedIndex == 0 || FolderView.SelectedIndex == 1) RemoveCurrentFolderMenuItem.IsEnabled = false;
            else RemoveCurrentFolderMenuItem.IsEnabled = true;

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

        public void RemoveCurrentFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Management.ModManagement.RemoveSubFolder((FolderView.SelectedValue as SubDirectoryItem).FilePath);
        }

        public void FolderListHost_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        public void AddNewSubFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {

            string newFolderName = Program.LanguageResource.GetString("NewSubFolderEntryName");
            System.Windows.Forms.DialogResult result;
            result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption1"));
            while (Directory.Exists(System.IO.Path.Combine(Management.ProgramPaths.Sonic3AIRModsFolder, newFolderName)) && (result != System.Windows.Forms.DialogResult.Cancel || result != System.Windows.Forms.DialogResult.Abort))
            {
                result = ExtraDialog.ShowInputDialog(ref newFolderName, Program.LanguageResource.GetString("CreateSubFolderDialogTitle"), Program.LanguageResource.GetString("CreateSubFolderDialogCaption2"));
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newDirectoryPath = System.IO.Path.Combine(Management.ProgramPaths.Sonic3AIRModsFolder, newFolderName);
                Directory.CreateDirectory(newDirectoryPath);
                Management.ModManagement.UpdateModsList(true);
            }
        }

        private void PanelViewButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(ViewSetting.Panel, false);
        }

        private void TabViewButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(ViewSetting.Tabbed, false);
        }

        private void ModPropertiesVisibilitySwitch_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(CurrentView, true);
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

        public Visibility CheckBoxVisibility { get; set; } = Visibility.Visible;

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
