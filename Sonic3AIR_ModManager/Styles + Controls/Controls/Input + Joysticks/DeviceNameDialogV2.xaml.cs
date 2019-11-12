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
    public partial class DeviceNameDialogV2 : Window
    {
        public DeviceNameDialogV2()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
            var instance = this;
            UserLanguage.ApplyLanguage(ref instance);
        }

        public bool? ShowDeviceNameDialog(ref string input, string caption, string message = "")
        {
            this.Title = caption;
            label1.Text = message;
            textBox1.Text = input;


            this.ShowDialog();
            input = textBox1.Text;
            return this.DialogResult;

        }

        private void detectControllerButton_Click(object sender, RoutedEventArgs e)
        {
            JoystickInputSelectorDialogV2 dlg = new JoystickInputSelectorDialogV2();
            if (dlg.ShowDialog().Value == true)
            {
                textBox1.Text = dlg.ResultString;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
