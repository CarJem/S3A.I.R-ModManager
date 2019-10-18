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

namespace Sonic3AIR_ModLoader
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
    }



    public class ModViewerItem
    {
        public string Name { get => Source.Name; set => Source.Name = value; }
        public string Description { get => Source.Description; set => Source.Description = value; }
        public string Author { get => Source.Author; set => Source.Author = value; }

        public string Version { get => Source.ModVersion; set => Source.ModVersion = value; }

        public string TechName { get => Source.TechnicalName; set => Source.TechnicalName = value; }

        public ImageSource Image { get => GetImage(); }

        public bool IsEnabled { get => GetEnabledState(); set => SetEnabledState(value); }

        private bool GetEnabledState()
        {
            return Source.IsEnabled;
        }

        private ImageSource GetImage()
        {
            string ImageLocation = System.IO.Path.Combine(Source.FolderPath, "icon.png");
            if (System.IO.File.Exists(ImageLocation))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(ImageLocation);
                image.EndInit();

                if (image.PixelWidth != image.PixelHeight) return DefaultModImage(Sonic3AIR_ModLoader.Properties.Resources.ModIconDefault);
                else return image;
            }
            else
            {
                return DefaultModImage(Sonic3AIR_ModLoader.Properties.Resources.ModIconDefault);
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

        public AIR_SDK.Mod Source { get; set; }

        public ModViewerItem(AIR_SDK.Mod _source)
        {
            Source = _source;
        }
    }
}
