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

        public bool IsEnabled { get => GetEnabledState(); set => SetEnabledState(value); }

        private bool GetEnabledState()
        {
            return Source.IsEnabled;
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
