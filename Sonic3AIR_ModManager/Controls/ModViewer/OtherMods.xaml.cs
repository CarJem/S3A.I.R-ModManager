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
    /// Interaction logic for OtherMods.xaml
    /// </summary>
    public partial class OtherMods : UserControl
    {
        public ModViewer Source { get; set; }
        public OtherMods()
        {
            InitializeComponent();
        }

        public OtherMods(ModViewer _source)
        {
            Source = _source;
            InitializeComponent();
        }

        private void RemoveCurrentFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Source.RemoveCurrentFolderMenuItem_Click(sender, e);
        }

        private void AddNewSubFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Source.AddNewSubFolderMenuItem_Click(sender, e);
        }

        private void FolderListHost_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Source.FolderListHost_ContextMenuOpening(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Source.Button_Click(sender, e);
        }

        private void FolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Source.FolderView_SelectionChanged(sender, e);
        }

        private void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Source.View_SelectionChanged(sender, e);
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            Source.View_KeyDown(sender, e);
        }
    }
}
