using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sonic3AIR_ModLoader
{
    public partial class KeyBindingDialog : Form
    {
        List<string> KeyBindings { get => GetKeys(); }
        string OriginalKeybinding = "";

        private List<string> GetKeys()
        {
            List<string> functionKeys = new List<string>();

            functionKeys.AddRange(new string[] {
            "Enter","Space","Backspace","Up","Down","Left","Right","A","B","C","D","E","F","G","H","I","J","K","L",
            "M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","Comma","Period","Colon","Semicolon",
            "Quote","Slash","Backslash","Minus","Equals","BracketLeft","BracketRight","0","1","2","3","4","5","6","7","8","9",
            "Numpad0","Numpad1","Numpad2","Numpad3","Numpad4","Numpad5","Numpad6","Numpad7","Numpad8","Numpad9","NumpadPlus",
            "NumpadMinus","NumpadMultiply","NumpadDivide","NumpadPeriod","Insert","Delete","Home","End","PageUp","PageDown"
            });

            List<string> Keys = new List<string>();
            Keys.Add("");
            Keys.AddRange(functionKeys);
            return Keys;
        }

        private KeyBindingDialog Instance;

        public KeyBindingDialog()
        {
            InitializeComponent();
            Instance = this;
            UserLanguage.ApplyLanguage(ref Instance);
            keyBox.DataSource = KeyBindings;
            RadioButton1_CheckedChanged(null, null);
        }

        public string ShowInputDialog(string keybind)
        {
            OriginalKeybinding = keybind;
            resultText.Text = $"{keybind} {Program.LanguageResource.GetString("KeybindingsExistingNote")}"; ;
            resultText.Tag = keybind;
            if (this.ShowDialog() == DialogResult.OK)
            {
                keybind = resultText.Tag.ToString();
            }
            OriginalKeybinding = "";
            return keybind;
        }

        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (inputDeviceRadioButton1.Checked)
            {
                ToggleKeyboardBindingsArea(true);
                ToggleControllerBindingsArea(false);
                ToggleCustomBindingsArea(false);
            }
            else if (inputDeviceRadioButton2.Checked)
            {
                ToggleKeyboardBindingsArea(false);
                ToggleControllerBindingsArea(true);
                ToggleCustomBindingsArea(false);
            }
            else if (inputDeviceRadioButton3.Checked)
            {
                ToggleKeyboardBindingsArea(false);
                ToggleControllerBindingsArea(false);
                ToggleCustomBindingsArea(true);
            }


            void ToggleKeyboardBindingsArea(bool enabled)
            {
                keyBox.Enabled = enabled;
                keyLabel.Enabled = enabled;
                UpdateResultText(enabled);
            }

            void ToggleControllerBindingsArea(bool enabled)
            {
                UpdateControllerInputType(enabled);
                UpdateResultText(enabled);
            }

            void ToggleCustomBindingsArea(bool enabled)
            {
                resultLabel.Enabled = enabled;
                resultText.Enabled = enabled;
                UpdateResultText(!enabled);
            }
        }

        private void UpdateResultText(bool ShowExistingString = true)
        {
            if (keyBox.SelectedIndex != 0 && inputDeviceRadioButton1.Checked)
            {
                resultText.Text = keyBox.SelectedItem.ToString();
                resultText.Tag = keyBox.SelectedItem.ToString();
            }
            else if (inputDeviceRadioButton2.Checked)
            {
                if (controllerInputTypeRadio1.Checked)
                {
                    resultText.Text = $"Button{(int)buttonIDNUD.Value}";
                    resultText.Tag = $"Button{(int)buttonIDNUD.Value}";
                }
                else if (controllerInputTypeRadio2.Checked)
                {
                    int axisBase = (int)AxisIDNUD.Value * 4;
                    string axisType = "";


                    if (axisTypeRadio1.Checked) axisType = "Axis";
                    else if (axisTypeRadio2.Checked) axisType = "Pov";
                    else if (axisTypeRadio4.Checked) axisType = "Thumb";
                    else if (axisTypeRadio3.Checked) axisType = axisCustomStringBox.Text;

                    bool left = AxisLeftRadioButton.Checked;
                    bool right = AxisRightRadioButton.Checked;
                    bool up = AxisUpRadioButton.Checked;
                    bool down = AxisDownRadioButton.Checked;

                    if (axisType == "Pov")
                    {
                        if (up)
                        {
                            resultText.Text = $"{axisType}{axisBase}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (right)
                        {
                            resultText.Text = $"{axisType}{axisBase + 1}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (down)
                        {
                            resultText.Text = $"{axisType}{axisBase + 2}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (left)
                        {
                            resultText.Text = $"{axisType}{axisBase + 3}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }

                    }
                    else
                    {
                        if (left)
                        {
                            resultText.Text = $"{axisType}{axisBase}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (right)
                        {
                            resultText.Text = $"{axisType}{axisBase + 1}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (up)
                        {
                            resultText.Text = $"{axisType}{axisBase + 2}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                        else if (down)
                        {
                            resultText.Text = $"{axisType}{axisBase + 3}";
                            resultText.Tag = keyBox.SelectedItem.ToString();
                        }
                    }





                }

                if (ShowExistingString && resultText.Tag.ToString() == OriginalKeybinding)
                {
                    resultText.Text = $"{OriginalKeybinding} {Program.LanguageResource.GetString("KeybindingsExistingNote")}";
                }

            }
            else
            {
                if (ShowExistingString) resultText.Text = $"{OriginalKeybinding} {Program.LanguageResource.GetString("KeybindingsExistingNote")}";
                else resultText.Text = OriginalKeybinding;
                resultText.Tag = OriginalKeybinding;
            }
        }

        private void KeyBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }

        private void UpdateControllerInputType(bool isSectionEnabled)
        {
            if (isSectionEnabled)
            {
                controllerInputTypeRadio1.Enabled = true;
                controllerInputTypeRadio2.Enabled = true;

                if (controllerInputTypeRadio1.Checked)
                {
                    ToggleButtonIDInput(true);
                    ToggleAxisInput(false);
                }
                else if (controllerInputTypeRadio2.Checked)
                {
                    ToggleButtonIDInput(false);
                    ToggleAxisInput(true);
                }
            }
            else
            {
                ToggleButtonIDInput(false);
                ToggleAxisInput(false);

                controllerInputTypeRadio1.Enabled = false;
                controllerInputTypeRadio2.Enabled = false;
            }

            void ToggleButtonIDInput(bool enabled)
            {
                buttonIDNUD.Enabled = enabled;
            }

            void ToggleAxisInput(bool enabled)
            {
                axisPOVBox.Enabled = enabled;
                axisTypeBox.Enabled = enabled;
                UpdateJoyAxisUI(null);
            }

        }

        private void ControllerInputTypeRadio1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControllerInputType(true);
            UpdateResultText();
        }

        private void ButtonIDNUD_ValueChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }

        private int AxisCurrentDirection = 0;
        private void Label5_Click(object sender, EventArgs e)
        {
            UpdateJoyAxisUI(sender);
        }

        private void UpdateJoyAxisUI(object sender)
        {
            if (sender == UpAxisButton) UpdateAxisDiagram(0);
            else if (sender == RightAxisButton) UpdateAxisDiagram(1);
            else if (sender == DownAxisButton) UpdateAxisDiagram(2);
            else if (sender == LeftAxisButton) UpdateAxisDiagram(3);
            else UpdateAxisDiagram(AxisCurrentDirection);

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
                    LeftAxisButton.ForeColor = (enabled ? SystemColors.Highlight : SystemColors.WindowText);
                    AxisLeftRadioButton.Checked = enabled;
                }


                void Right(bool enabled)
                {
                    RightAxisButton.ForeColor = (enabled ? SystemColors.Highlight : SystemColors.WindowText);
                    AxisRightRadioButton.Checked = enabled;
                }


                void Up(bool enabled)
                {
                    UpAxisButton.ForeColor = (enabled ? SystemColors.Highlight : SystemColors.WindowText);
                    AxisUpRadioButton.Checked = enabled;
                }


                void Down(bool enabled)
                {
                    DownAxisButton.ForeColor = (enabled ? SystemColors.Highlight : SystemColors.WindowText);
                    AxisDownRadioButton.Checked = enabled;
                }
            }
        }

        private void AxisTypeRadio5_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }

        private void AxisRightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }

        private void AxisIDNUD_ValueChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }

        private void AxisCustomStringBox_TextChanged(object sender, EventArgs e)
        {
            UpdateResultText();
        }
    }
}
