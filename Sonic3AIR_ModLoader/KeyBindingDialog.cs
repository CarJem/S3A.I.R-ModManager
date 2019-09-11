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
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            List<string> functionKeys = new List<string>();

            functionKeys.AddRange(new string[] {
                "Space",
                "Enter",
                "Up",
                "Down",
                "Left",
                "Right"
            });

            List<string> Keys = new List<string>();
            Keys.Add("");
            Keys.AddRange(functionKeys);
            Keys.AddRange(alphabet.Select(c => c.ToString()).ToList());
            //AddUnsupportedKeys(ref Keys);
            return Keys;
        }

        private void AddUnsupportedKeys(ref List<string> Keys)
        {
            for (int i = 1; i <= 12; i++)
            {
                Keys.Add($"F{i}");
            }

            char[] numbers = "12345678910".ToCharArray();
            Keys.AddRange(numbers.Select(c => c.ToString()).ToList());
        }

        public KeyBindingDialog()
        {
            InitializeComponent();
            keyBox.DataSource = KeyBindings;
            RadioButton1_CheckedChanged(null, null);
        }

        public string ShowInputDialog(string keybind)
        {
            OriginalKeybinding = keybind;
            resultText.Text = $"{keybind} (Existing)"; ;
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
                label1.Enabled = enabled;
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
                    else if (axisTypeRadio5.Checked) axisType = axisCustomStringBox.Text;

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
                    resultText.Text = $"{OriginalKeybinding} (Existing)";
                }

            }
            else
            {
                if (ShowExistingString) resultText.Text = $"{OriginalKeybinding} (Existing)";
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
