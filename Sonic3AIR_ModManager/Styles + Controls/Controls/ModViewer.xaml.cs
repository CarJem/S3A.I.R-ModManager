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

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for ModViewer.xaml
    /// </summary>
    public partial class ModViewer : UserControl
    {
        public static Action ItemCheck;
        public ModViewer()
        {
            InitializeComponent();
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
    }



    public class ModViewerItem
    {
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

        public ModViewerItem(AIR_API.Mod _source)
        {
            Source = _source;
        }
    }
}
