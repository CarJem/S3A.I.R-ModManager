using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    public partial class JoystickInputSelectorDialog : Window
    {
        public int Result = -1;
        public string ResultString = "";
        private bool NoDevicesFound = false;
        public JoystickInputSelectorDialog()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
            RefreshInputsList();
        }


        private void RefreshInputsList()
        {
            listBox1.IsEnabled = true;
            NoDevicesFound = false;

            listBox1.ItemsSource = null;
            listBox1.Items.Clear();
            listBox1.ItemsSource = JoystickReader.GetJoysticks();
            if (JoystickReader.GetJoystickCount() == 0)
            {
                NoDevicesFound = true;
                string no_item_found = Program.LanguageResource.GetString("NoControllerFound");
                if (no_item_found == null) no_item_found = "";
                listBox1.IsEnabled = false;
                listBox1.ItemsSource = null;
                listBox1.Items.Add(no_item_found);
                selectButton.IsEnabled = false;
                listBox1.SelectedIndex = -1;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedItem != null && !NoDevicesFound) selectButton.IsEnabled = true;
            else selectButton.IsEnabled = false;
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            this.Result = listBox1.SelectedIndex;
            this.ResultString = listBox1.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshInputsList();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
