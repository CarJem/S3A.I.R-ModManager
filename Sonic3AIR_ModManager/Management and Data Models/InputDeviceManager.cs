using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIR_API;
using System.Windows.Forms;
using Device = AIR_API.InputMappings.Device;

namespace Sonic3AIR_ModManager
{
    public static class InputDeviceManager
    {
        public static InputDevices InputDevices { get; set; }
        public static Dictionary<string, Device> Devices { get => KeyPairListToDictionaryHelper.ToDictionary(InputDevices.Items, x => x.Key, x => x.Value); set => InputDevices.Items = value.ToList(); }



        public static void SaveInputs()
        {
            MainDataModel.Input_Settings.InputDevices = InputDevices;
            MainDataModel.Input_Settings.Save();
        }

        #region Input Mapping
        public static void UpdateInputDeviceButtons()
        {
            if (ModManager.Instance.inputMethodsList.SelectedItem != null)
            {
                ModManager.Instance.moveInputMethodUpButton.IsEnabled = ModManager.Instance.inputMethodsList.Items.IndexOf(ModManager.Instance.inputMethodsList.SelectedItem) > 0;
                ModManager.Instance.moveInputMethodDownButton.IsEnabled = ModManager.Instance.inputMethodsList.Items.IndexOf(ModManager.Instance.inputMethodsList.SelectedItem) < ModManager.Instance.inputMethodsList.Items.Count - 1;
                ModManager.Instance.moveInputMethodToTopButton.IsEnabled = ModManager.Instance.inputMethodsList.Items.IndexOf(ModManager.Instance.inputMethodsList.SelectedItem) > 0;
                ModManager.Instance.moveInputMethodToBottomButton.IsEnabled = ModManager.Instance.inputMethodsList.Items.IndexOf(ModManager.Instance.inputMethodsList.SelectedItem) < ModManager.Instance.inputMethodsList.Items.Count - 1;

                ModManager.Instance.removeInputMethodButton.IsEnabled = true;
                ModManager.Instance.exportConfigButton.IsEnabled = true;
            }
            else
            {
                ModManager.Instance.removeInputMethodButton.IsEnabled = false;
                ModManager.Instance.exportConfigButton.IsEnabled = false;
                ModManager.Instance.moveInputMethodUpButton.IsEnabled = false;
                ModManager.Instance.moveInputMethodDownButton.IsEnabled = false;
                ModManager.Instance.moveInputMethodToTopButton.IsEnabled = false;
                ModManager.Instance.moveInputMethodToBottomButton.IsEnabled = false;
            }
        }

        public static void UpdateInputMappings()
        {
            UpdateInputDeviceButtons();
            ModManager.Instance.inputDeviceNamesList.Items.Clear();
            if (MainDataModel.Input_Settings != null)
            {
                if (ModManager.Instance.inputMethodsList.SelectedItem != null)
                {
                    if (ModManager.Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                    {
                        AIR_API.InputMappings.Device device = ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                        ModManager.Instance.aInputButton.Content = (device.A.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.A.FirstOrDefault());
                        ModManager.Instance.bInputButton.Content = (device.B.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.B.FirstOrDefault());
                        ModManager.Instance.xInputButton.Content = (device.X.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.X.FirstOrDefault());
                        ModManager.Instance.yInputButton.Content = (device.Y.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Y.FirstOrDefault());
                        ModManager.Instance.upInputButton.Content = (device.Up.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Up.FirstOrDefault());
                        ModManager.Instance.downInputButton.Content = (device.Down.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Down.FirstOrDefault());
                        ModManager.Instance.leftInputButton.Content = (device.Left.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Left.FirstOrDefault());
                        ModManager.Instance.rightInputButton.Content = (device.Right.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Right.FirstOrDefault());
                        ModManager.Instance.startInputButton.Content = (device.Start.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Start.FirstOrDefault());
                        ModManager.Instance.backInputButton.Content = (device.Back.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Back.FirstOrDefault());

                        if (ModManager.Instance.aInputButton.Content == null) ModManager.Instance.aInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.bInputButton.Content == null) ModManager.Instance.bInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.xInputButton.Content == null) ModManager.Instance.xInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.yInputButton.Content == null) ModManager.Instance.yInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.upInputButton.Content == null) ModManager.Instance.upInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.downInputButton.Content == null) ModManager.Instance.downInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.leftInputButton.Content == null) ModManager.Instance.leftInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.rightInputButton.Content == null) ModManager.Instance.rightInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.startInputButton.Content == null) ModManager.Instance.startInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (ModManager.Instance.backInputButton.Content == null) ModManager.Instance.backInputButton.Content = Program.LanguageResource.GetString("Input_NONE");

                        UpdateInputDeviceNamesList(true);



                    }
                }
                else
                {

                    DisableMappings();
                }
            }
        }

        public static void ToggleDeviceNamesUI(bool enabled)
        {
            ModManager.Instance.inputDeviceNamesList.IsEnabled = enabled;
            ModManager.Instance.addDeviceNameButton.IsEnabled = enabled;
            ModManager.Instance.removeDeviceNameButton.IsEnabled = (enabled == true ? ModManager.Instance.inputDeviceNamesList.SelectedItem != null : enabled);

            if (ModManager.Instance.inputDeviceNamesList.SelectedItem != null)
            {
                ModManager.Instance.moveDeviceNameUpButton.IsEnabled = ModManager.Instance.inputDeviceNamesList.Items.IndexOf(ModManager.Instance.inputDeviceNamesList.SelectedItem) > 0 && enabled;
                ModManager.Instance.moveDeviceNameDownButton.IsEnabled = ModManager.Instance.inputDeviceNamesList.Items.IndexOf(ModManager.Instance.inputDeviceNamesList.SelectedItem) < ModManager.Instance.inputDeviceNamesList.Items.Count - 1 && enabled;
                ModManager.Instance.moveDeviceNameToTopButton.IsEnabled = ModManager.Instance.inputDeviceNamesList.Items.IndexOf(ModManager.Instance.inputDeviceNamesList.SelectedItem) > 0 && enabled;
                ModManager.Instance.moveDeviceNameToBottomButton.IsEnabled = ModManager.Instance.inputDeviceNamesList.Items.IndexOf(ModManager.Instance.inputDeviceNamesList.SelectedItem) < ModManager.Instance.inputDeviceNamesList.Items.Count - 1 && enabled;
            }
            else
            {
                ModManager.Instance.moveDeviceNameUpButton.IsEnabled = false;
                ModManager.Instance.moveDeviceNameDownButton.IsEnabled = false;
                ModManager.Instance.moveDeviceNameToTopButton.IsEnabled = false;
                ModManager.Instance.moveDeviceNameToBottomButton.IsEnabled = false;
            }
        }

        public static void DisableMappings()
        {
            ModManager.Instance.inputDeviceNamesList.Items.Clear();
            ModManager.Instance.aInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.bInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.xInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.yInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.upInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.downInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.leftInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.rightInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.startInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.backInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            ModManager.Instance.inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL")));
        }

        public static void ChangeInputMappings(object sender)
        {
            AIR_API.InputMappings.Device device = ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;

            if (sender.Equals(ModManager.Instance.aInputButton)) ChangeMappings(ref device, "A");
            else if (sender.Equals(ModManager.Instance.bInputButton)) ChangeMappings(ref device, "B");
            else if (sender.Equals(ModManager.Instance.xInputButton)) ChangeMappings(ref device, "X");
            else if (sender.Equals(ModManager.Instance.yInputButton)) ChangeMappings(ref device, "Y");
            else if (sender.Equals(ModManager.Instance.upInputButton)) ChangeMappings(ref device, "Up");
            else if (sender.Equals(ModManager.Instance.downInputButton)) ChangeMappings(ref device, "Down");
            else if (sender.Equals(ModManager.Instance.leftInputButton)) ChangeMappings(ref device, "Left");
            else if (sender.Equals(ModManager.Instance.rightInputButton)) ChangeMappings(ref device, "Right");
            else if (sender.Equals(ModManager.Instance.startInputButton)) ChangeMappings(ref device, "Start");
            else if (sender.Equals(ModManager.Instance.backInputButton)) ChangeMappings(ref device, "Back");

            void ChangeMappings(ref AIR_API.InputMappings.Device button, string input)
            {
                switch (input)
                {
                    case "A":
                        MappingDialog(ref button.A);
                        break;
                    case "B":
                        MappingDialog(ref button.B);
                        break;
                    case "X":
                        MappingDialog(ref button.X);
                        break;
                    case "Y":
                        MappingDialog(ref button.Y);
                        break;
                    case "Up":
                        MappingDialog(ref button.Up);
                        break;
                    case "Down":
                        MappingDialog(ref button.Down);
                        break;
                    case "Left":
                        MappingDialog(ref button.Left);
                        break;
                    case "Right":
                        MappingDialog(ref button.Right);
                        break;
                    case "Start":
                        MappingDialog(ref button.Start);
                        break;
                    case "Back":
                        MappingDialog(ref button.Back);
                        break;
                }
                UpdateInputMappings();

                void MappingDialog(ref List<string> mappings)
                {
                    var mD = new KeybindingsListDialog(mappings);
                    mD.ShowDialog();
                }

            }
        }

        public static void AddInputDeviceName()
        {
            if (ModManager.Instance.inputMethodsList.SelectedItem != null)
            {
                if (FileManagement.AddInputDeviceName(ModManager.Instance.inputMethodsList.SelectedIndex) == true)
                {
                    UpdateInputMappings();
                }
            }

        }

        public static void AddInputDevice()
        {
            if (FileManagement.AddInputDevice() == true)
            {
                RefreshInputMappings();
            }
        }

        public static void RemoveInputDevice()
        {
            if (ModManager.Instance.inputMethodsList.SelectedItem != null && ModManager.Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
            {
                FileManagement.RemoveInputDevice(ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device);
            }
        }

        public static void RecollectInputMappings()
        {
            HideInputMappingErrorPanels();

            if (ProgramPaths.Sonic3AIRPath != null && ProgramPaths.Sonic3AIRPath != "" && System.IO.File.Exists(ProgramPaths.Sonic3AIRPath))
            {
                if (InputDeviceManager.InputDevices != null)
                {
                    try
                    {
                        foreach (var inputMethod in InputDeviceManager.InputDevices.Items)
                        {
                            ModManager.Instance.inputMethodsList.Items.Add(inputMethod);
                        }
                    }
                    catch
                    {
                        AIRInputMappingsNullSituation(1);
                    }
                }
                else AIRInputMappingsNullSituation(2);

            }
            else AIRInputMappingsNullSituation();



        }

        public static void HideInputMappingErrorPanels()
        {
            ModManager.Instance.inputPanel.IsEnabled = true;
            ModManager.Instance.inputErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
        }

        public static void ShowInputMappingErrorPanels()
        {
            ModManager.Instance.inputPanel.IsEnabled = false;
            ModManager.Instance.inputErrorMessage.Visibility = System.Windows.Visibility.Visible;
        }

        public static void AIRInputMappingsNullSituation(int situation = 0)
        {
            string hyperLink = MainDataModel.nL + Program.LanguageResource.GetString("ErrorHyperlinkClickMessage");
            if (situation == 0) ModManager.Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError1") + hyperLink;
            else if (situation == 1) ModManager.Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError2") + hyperLink;
            else if (situation == 2) ModManager.Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError3") + hyperLink;

            ShowInputMappingErrorPanels();
        }

        public static void ResetInputMappingsToDefault()
        {
            var result = MessageBox.Show(Program.LanguageResource.GetString("ResetInputMappingsDefaultFormMessage"), Program.LanguageResource.GetString("ResetInputMappingsDefaultFormTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                InputDeviceManager.InputDevices.ResetDevicesToDefault();
                InputDeviceManager.RefreshInputMappings();
            }
        }

        public static void RefreshInputMappings()
        {
            InputDeviceManager.InputDevices = MainDataModel.Input_Settings.InputDevices;

            DisableMappings();
            CollectInputMappings();
            UpdateInputDeviceButtons();
        }

        public static void CollectInputMappings()
        {
            ModManager.Instance.inputMethodsList.SelectionChanged -= ModManager.Instance.InputMethodsList_SelectedIndexChanged;
            AIR_API.InputMappings.Device selectedItem = null;
            if (ModManager.Instance.inputMethodsList.SelectedItem != null)
            {
                selectedItem = ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
            }
            ModManager.Instance.inputMethodsList.ItemsSource = null;
            ModManager.Instance.inputMethodsList.Items.Refresh();
            if (InputDeviceManager.InputDevices != null)
            {
                if (ModManager.Instance.inputMethodsList.Items.Count != 0 && ModManager.Instance.inputMethodsList.ItemsSource == null) ModManager.Instance.inputMethodsList.Items.Clear();
                ModManager.Instance.inputMethodsList.ItemsSource = InputDeviceManager.InputDevices.Items.Select(x => x.Value);
                if (selectedItem != null && ModManager.Instance.inputMethodsList.Items.Contains(selectedItem)) ModManager.Instance.inputMethodsList.SelectedItem = selectedItem;
            }
            else
            {
                RecollectInputMappings();
            }
            ModManager.Instance.inputMethodsList.SelectionChanged += ModManager.Instance.InputMethodsList_SelectedIndexChanged;
        }

        public static void ImportInputDevice()
        {
            if (MainDataModel.Input_Settings != null)
            {
                FileManagement.ImportInputMappings();
                RefreshInputMappings();
            }
        }

        public static void ExportInputDevice()
        {
            if (ModManager.Instance.inputMethodsList.SelectedItem != null)
            {
                if (ModManager.Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                {
                    AIR_API.InputMappings.Device device = ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                    FileManagement.ExportInputMappings(device);
                }
            }
        }

        public static void RemoveInputDeviceName()
        {
            if (ModManager.Instance.inputMethodsList.SelectedItem != null && ModManager.Instance.inputDeviceNamesList.SelectedItem != null)
            {
                if (FileManagement.RemoveInputDeviceName(ModManager.Instance.inputDeviceNamesList.SelectedItem.ToString(), ModManager.Instance.inputMethodsList.SelectedIndex, ModManager.Instance.inputDeviceNamesList.SelectedIndex) == true)
                {
                    UpdateInputMappings();
                }
            }
        }

        public static void UpdateInputDeviceNamesList(bool refreshItems = false)
        {
            if (MainDataModel.Input_Settings != null)
            {
                if (ModManager.Instance.inputMethodsList.SelectedItem != null)
                {
                    if (ModManager.Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                    {
                        AIR_API.InputMappings.Device device = ModManager.Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                        if (device.HasDeviceNames)
                        {
                            if (refreshItems)
                            {
                                foreach (var name in device.DeviceNames)
                                {
                                    ModManager.Instance.inputDeviceNamesList.Items.Add(name);
                                }
                            }
                            ToggleDeviceNamesUI(true);
                        }
                        else
                        {
                            ModManager.Instance.inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_UNSUPPORTED") == null ? "" : Program.LanguageResource.GetString("Input_UNSUPPORTED")));
                            ToggleDeviceNamesUI(false);
                        }
                    }
                }
            }
        }

        public static void LaunchSystemGamepadSettings()
        {
            System.Diagnostics.Process.Start("joy.cpl");

        }

        public static void MoveInputMethod(FileManagement.MoveListItemDirection direction)
        {
            FileManagement.MoveInputDevice(ref ModManager.Instance, direction);
        }

        public static void MoveDeviceName(FileManagement.MoveListItemDirection direction)
        {
            FileManagement.MoveInputDeviceIdentifier(ref ModManager.Instance, direction);
        }

        #endregion
    }
}
