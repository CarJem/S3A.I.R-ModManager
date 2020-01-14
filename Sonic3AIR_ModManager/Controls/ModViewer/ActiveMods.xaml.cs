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

namespace Sonic3AIR_ModManager
{
    /// <summary>
    /// Interaction logic for ActiveMods.xaml
    /// </summary>
    public partial class ActiveMods : UserControl
    {
        public ModViewer Source { get; set; }
        public ActiveMods()
        {
            InitializeComponent();
        }

        public ActiveMods(ModViewer _source)
        {
            Source = _source;
            InitializeComponent();
        }

        private void ActiveView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Source.ActiveView_SelectionChanged(sender, e);
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            Source.View_KeyDown(sender, e);
        }
    }
}
