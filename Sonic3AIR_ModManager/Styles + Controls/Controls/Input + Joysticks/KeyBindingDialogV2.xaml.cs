using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using DialogResult = System.Windows.Forms.DialogResult;
using Binding = System.Windows.Data.Binding;

namespace Sonic3AIR_ModManager
{
    public partial class KeyBindingDialogV2 : Window
    {
        List<ComboBoxItem> KeyBindings { get => GetKeys(); }
        string OriginalKeybinding = "";
        bool isInitialized = false;

        private List<ComboBoxItem> GetKeys()
        {
            List<string> functionKeys = new List<string>();

            functionKeys.AddRange(new string[] {
            "Enter","Space","Backspace","Up","Down","Left","Right","A","B","C","D","E","F","G","H","I","J","K","L",
            "M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","Comma","Period","Colon","Semicolon",
            "Quote","Slash","Backslash","Minus","Equals","BracketLeft","BracketRight","0","1","2","3","4","5","6","7","8","9",
            "Numpad0","Numpad1","Numpad2","Numpad3","Numpad4","Numpad5","Numpad6","Numpad7","Numpad8","Numpad9","NumpadPlus",
            "NumpadMinus","NumpadMultiply","NumpadDivide","NumpadPeriod","Insert","Delete","Home","End","PageUp","PageDown"
            });

            List<ComboBoxItem> Keys = new List<ComboBoxItem>();
            functionKeys.Insert(0,"");
            Keys.AddRange(ToComboBoxList(functionKeys));
            return Keys;

            List<ComboBoxItem> ToComboBoxList(List<string> strings)
            {
                List<ComboBoxItem> result = new List<ComboBoxItem>();
                foreach (var item in strings)
                {
                    var result_item = new ComboBoxItem() { Content = item };

                    result.Add(result_item);
                }
                return result;
            }
        }



        private KeyBindingDialogV2 Instance;

        public KeyBindingDialogV2()
        {
            InitializeComponent();
            isInitialized = true;
            this.Owner = System.Windows.Application.Current.MainWindow;
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            keyBox.ItemsSource = KeyBindings;
            RadioButton1_CheckedChanged(null, null);
        }

        public string ShowInputDialog(string keybind)
        {
            OriginalKeybinding = keybind;
            resultText.Text = $"{keybind} {Program.LanguageResource.GetString("KeybindingsExistingNote")}"; ;
            resultText.Tag = keybind;
            if (KeyBindings.Exists(x => x.Content.ToString() == keybind)) keyBox.SelectedIndex = keyBox.Items.IndexOf(OriginalKeybinding);
            if (this.ShowDialog() == true)
            {
                keybind = resultText.Tag.ToString();
            }
            OriginalKeybinding = "";
            return keybind;
        }

        private void Label3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton1_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
            {
                if (inputDeviceRadioButton1.IsChecked.Value)
                {
                    ToggleKeyboardBindingsArea(true);
                    ToggleControllerBindingsArea(false);
                    ToggleCustomBindingsArea(false);
                }
                else if (inputDeviceRadioButton3.IsChecked.Value)
                {
                    ToggleKeyboardBindingsArea(false);
                    ToggleControllerBindingsArea(false);
                    ToggleCustomBindingsArea(true);
                }
            }



            void ToggleKeyboardBindingsArea(bool enabled)
            {
                keyBox.IsEnabled = enabled;
                keyLabel.IsEnabled = enabled;
                UpdateResultText(enabled);
                keyArea.IsEnabled = enabled;
            }

            void ToggleControllerBindingsArea(bool enabled)
            {
                UpdateControllerInputType(enabled);
                UpdateResultText(enabled);
            }

            void ToggleCustomBindingsArea(bool enabled)
            {
                resultLabel.IsEnabled = enabled;
                otherArea.IsEnabled = enabled;
                resultText.IsEnabled = enabled;
                UpdateResultText(!enabled);
            }
        }

        private void UpdateResultText(bool ShowExistingString = true)
        {
            if (keyBox.SelectedIndex != 0 && keyBox.SelectedItem != null && inputDeviceRadioButton1.IsChecked.Value)
            {
                resultText.Text = (keyBox.SelectedItem as ComboBoxItem).Content.ToString();
                resultText.Tag = (keyBox.SelectedItem as ComboBoxItem).Content.ToString();
            }
            else
            {
                if (ShowExistingString)
                {
                    resultText.Text = $"{OriginalKeybinding} {Program.LanguageResource.GetString("KeybindingsExistingNote")}";
                }
                else
                {
                    resultText.Text = OriginalKeybinding;
                    resultText.Tag = OriginalKeybinding;
                }
            }
        }

        private void keyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResultText();
        }

        private void UpdateControllerInputType(bool isSectionEnabled)
        {
            if (isSectionEnabled)
            {

            }
            else
            {
                ToggleButtonIDInput(false);
                ToggleAxisInput(false);


            }

            void ToggleButtonIDInput(bool enabled)
            {

            }

            void ToggleAxisInput(bool enabled)
            {
                UpdateJoyAxisUI(null);
            }

        }

        private void ControllerInputTypeRadio1_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateControllerInputType(true);
            UpdateResultText();
        }

        private void ButtonIDNUD_ValueChanged(object sender, RoutedEventArgs e)
        {
            UpdateResultText();
        }

        private int AxisCurrentDirection = 0;
        private void Label5_Click(object sender, RoutedEventArgs e)
        {
            UpdateJoyAxisUI(sender);
        }

        private void UpdateJoyAxisUI(object sender)
        {
            UpdateAxisDiagram(AxisCurrentDirection);

            void UpdateAxisDiagram(int direction = 0)
            {
                AxisCurrentDirection = direction;
                if (direction == 0)
                {
                    Up(true);
                    Left(false);
                    Down(false);
                    Right(false);
                }
                else if (direction == 1)
                {
                    Up(false);
                    Left(false);
                    Down(false);
                    Right(true);
                }
                else if (direction == 2)
                {
                    Up(false);
                    Left(false);
                    Down(true);
                    Right(false);
                }
                else if (direction == 3)
                {
                    Up(false);
                    Left(true);
                    Down(false);
                    Right(false);
                }


                void Left(bool enabled)
                {

                }


                void Right(bool enabled)
                {

                }


                void Up(bool enabled)
                {

                }


                void Down(bool enabled)
                {

                }
            }
        }

        private void AxisTypeRadio5_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateResultText();
        }

        private void AxisRightRadioButton_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateResultText();
        }

        private void AxisIDNUD_ValueChanged(object sender, RoutedEventArgs e)
        {
            UpdateResultText();
        }

        private void AxisCustomStringBox_TextChanged(object sender, RoutedEventArgs e)
        {
            UpdateResultText();
        }

        private void resultText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputDeviceRadioButton3.IsChecked == true)
            {
                resultText.Tag = resultText.Text;
            }
        }

        private void getInputButton_Click(object sender, RoutedEventArgs e)
        {

            JoystickReaderDialogV2 dlg = new JoystickReaderDialogV2();
            if (dlg.ShowInputDialog() == true)
            {
                inputDeviceRadioButton3.IsChecked = true;
                resultText.Text = dlg.Result;
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

        private void keyBox_DropDownOpened(object sender, EventArgs e)
        {
            DropDownWidth(keyBox);

            void DropDownWidth(ComboBox myCombo)
            {
                foreach (var obj in myCombo.Items)
                {
                    if (obj is ComboBoxItem)
                    {
                        (obj as ComboBoxItem).Width = myCombo.ActualWidth;
                    }
                }
            }
        }
    }
}
