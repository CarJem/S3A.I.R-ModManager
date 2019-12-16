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
        private static ModManager Instance;
        public static InputDevices InputDevices { get; set; }
        public static Dictionary<string, Device> Devices { get => KeyPairListToDictionaryHelper.ToDictionary(InputDevices.Items, x => x.Key, x => x.Value); set => InputDevices.Items = value.ToList(); }

        public static void UpdateInstance(ref ModManager _Instance)
        {
            Instance = _Instance;
        }

        public static void SaveInputs()
        {
            MainDataModel.Input_Settings.InputDevices = InputDevices;
            MainDataModel.Input_Settings.Save();
        }
        public static void UpdateInputDeviceButtons()
        {
            if (Instance.inputMethodsList.SelectedItem != null)
            {
                Instance.moveInputMethodUpButton.IsEnabled = Instance.inputMethodsList.Items.IndexOf(Instance.inputMethodsList.SelectedItem) > 0;
                Instance.moveInputMethodDownButton.IsEnabled = Instance.inputMethodsList.Items.IndexOf(Instance.inputMethodsList.SelectedItem) < Instance.inputMethodsList.Items.Count - 1;
                Instance.moveInputMethodToTopButton.IsEnabled = Instance.inputMethodsList.Items.IndexOf(Instance.inputMethodsList.SelectedItem) > 0;
                Instance.moveInputMethodToBottomButton.IsEnabled = Instance.inputMethodsList.Items.IndexOf(Instance.inputMethodsList.SelectedItem) < Instance.inputMethodsList.Items.Count - 1;

                Instance.removeInputMethodButton.IsEnabled = true;
                Instance.exportConfigButton.IsEnabled = true;
            }
            else
            {
                Instance.removeInputMethodButton.IsEnabled = false;
                Instance.exportConfigButton.IsEnabled = false;
                Instance.moveInputMethodUpButton.IsEnabled = false;
                Instance.moveInputMethodDownButton.IsEnabled = false;
                Instance.moveInputMethodToTopButton.IsEnabled = false;
                Instance.moveInputMethodToBottomButton.IsEnabled = false;
            }
        }

        public static void UpdateInputMappings()
        {
            UpdateInputDeviceButtons();
            Instance.inputDeviceNamesList.Items.Clear();
            if (MainDataModel.Input_Settings != null)
            {
                if (Instance.inputMethodsList.SelectedItem != null)
                {
                    if (Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                    {
                        AIR_API.InputMappings.Device device = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                        Instance.aInputButton.Content = (device.A.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.A.FirstOrDefault());
                        Instance.bInputButton.Content = (device.B.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.B.FirstOrDefault());
                        Instance.xInputButton.Content = (device.X.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.X.FirstOrDefault());
                        Instance.yInputButton.Content = (device.Y.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Y.FirstOrDefault());
                        Instance.upInputButton.Content = (device.Up.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Up.FirstOrDefault());
                        Instance.downInputButton.Content = (device.Down.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Down.FirstOrDefault());
                        Instance.leftInputButton.Content = (device.Left.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Left.FirstOrDefault());
                        Instance.rightInputButton.Content = (device.Right.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Right.FirstOrDefault());
                        Instance.startInputButton.Content = (device.Start.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Start.FirstOrDefault());
                        Instance.backInputButton.Content = (device.Back.Count() > 1 ? Program.LanguageResource.GetString("Input_MULTI") : device.Back.FirstOrDefault());

                        if (Instance.aInputButton.Content == null) Instance.aInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.bInputButton.Content == null) Instance.bInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.xInputButton.Content == null) Instance.xInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.yInputButton.Content == null) Instance.yInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.upInputButton.Content == null) Instance.upInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.downInputButton.Content == null) Instance.downInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.leftInputButton.Content == null) Instance.leftInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.rightInputButton.Content == null) Instance.rightInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.startInputButton.Content == null) Instance.startInputButton.Content = Program.LanguageResource.GetString("Input_NONE");
                        if (Instance.backInputButton.Content == null) Instance.backInputButton.Content = Program.LanguageResource.GetString("Input_NONE");

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
            Instance.inputDeviceNamesList.IsEnabled = enabled;
            Instance.addDeviceNameButton.IsEnabled = enabled;
            Instance.removeDeviceNameButton.IsEnabled = (enabled == true ? Instance.inputDeviceNamesList.SelectedItem != null : enabled);

            if (Instance.inputDeviceNamesList.SelectedItem != null)
            {
                Instance.moveDeviceNameUpButton.IsEnabled = Instance.inputDeviceNamesList.Items.IndexOf(Instance.inputDeviceNamesList.SelectedItem) > 0 && enabled;
                Instance.moveDeviceNameDownButton.IsEnabled = Instance.inputDeviceNamesList.Items.IndexOf(Instance.inputDeviceNamesList.SelectedItem) < Instance.inputDeviceNamesList.Items.Count - 1 && enabled;
                Instance.moveDeviceNameToTopButton.IsEnabled = Instance.inputDeviceNamesList.Items.IndexOf(Instance.inputDeviceNamesList.SelectedItem) > 0 && enabled;
                Instance.moveDeviceNameToBottomButton.IsEnabled = Instance.inputDeviceNamesList.Items.IndexOf(Instance.inputDeviceNamesList.SelectedItem) < Instance.inputDeviceNamesList.Items.Count - 1 && enabled;
            }
            else
            {
                Instance.moveDeviceNameUpButton.IsEnabled = false;
                Instance.moveDeviceNameDownButton.IsEnabled = false;
                Instance.moveDeviceNameToTopButton.IsEnabled = false;
                Instance.moveDeviceNameToBottomButton.IsEnabled = false;
            }
        }

        public static void DisableMappings()
        {
            Instance.inputDeviceNamesList.Items.Clear();
            Instance.aInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.bInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.xInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.yInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.upInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.downInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.leftInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.rightInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.startInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.backInputButton.Content = (Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL"));
            Instance.inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_NULL") == null ? "" : Program.LanguageResource.GetString("Input_NULL")));
        }

        public static void ChangeInputMappings(object sender)
        {
            AIR_API.InputMappings.Device device = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;

            if (sender.Equals(Instance.aInputButton)) ChangeMappings(ref device, "A");
            else if (sender.Equals(Instance.bInputButton)) ChangeMappings(ref device, "B");
            else if (sender.Equals(Instance.xInputButton)) ChangeMappings(ref device, "X");
            else if (sender.Equals(Instance.yInputButton)) ChangeMappings(ref device, "Y");
            else if (sender.Equals(Instance.upInputButton)) ChangeMappings(ref device, "Up");
            else if (sender.Equals(Instance.downInputButton)) ChangeMappings(ref device, "Down");
            else if (sender.Equals(Instance.leftInputButton)) ChangeMappings(ref device, "Left");
            else if (sender.Equals(Instance.rightInputButton)) ChangeMappings(ref device, "Right");
            else if (sender.Equals(Instance.startInputButton)) ChangeMappings(ref device, "Start");
            else if (sender.Equals(Instance.backInputButton)) ChangeMappings(ref device, "Back");

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
            if (Instance.inputMethodsList.SelectedItem != null)
            {
                if (Validation(Instance.inputMethodsList.SelectedIndex) == true)
                {
                    UpdateInputMappings();
                }
            }


            bool Validation(int index)
            {
                string newDevice = Program.LanguageResource.GetString("NewDeviceEntryName");
                DeviceNameDialog deviceNameDialog = new DeviceNameDialog();
                bool? result = deviceNameDialog.ShowDeviceNameDialog(ref newDevice, Program.LanguageResource.GetString("AddNewDeviceTitle"), Program.LanguageResource.GetString("AddNewDeviceDescription"));
                if (result == true)
                {
                    InputDeviceManager.InputDevices.Items[index].Value.DeviceNames.Add(newDevice);
                    return true;
                }
                else return false;
            }

        }

        public static void AddInputDevice()
        {
            if (Validation() == true)
            {
                RefreshInputMappings();
            }


            bool Validation()
            {
                string new_name = Program.LanguageResource.GetString("NewControllerEntryName");
                bool finished = false;
                char[] acceptable_char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890".ToArray();
                bool success = false;

                while (!finished)
                {

                    DialogResult result = ExtraDialog.ShowInputDialog(ref new_name, Program.LanguageResource.GetString("AddInputDeviceDialogTitle"), Program.LanguageResource.GetString("AddInputDeviceDialogCaption"));
                    bool containsKey = InputDeviceManager.Devices.ContainsKey(new_name);
                    bool unacceptable_char = new_name.ContainsOnly(acceptable_char);
                    if (result != System.Windows.Forms.DialogResult.Cancel && !containsKey && unacceptable_char)
                    {
                        finished = true;
                        InputDeviceManager.Devices.Add(new_name, new AIR_API.InputMappings.Device(new_name));
                        success = true;
                    }
                    else if (result != System.Windows.Forms.DialogResult.Cancel)
                    {
                        if (containsKey)
                        {
                            MessageBox.Show(string.Format("\"{0}\" {1}", new_name, Program.LanguageResource.GetString("AddInputDeviceError1")));
                        }
                        else
                        {
                            MessageBox.Show(string.Format("\"{0}\" {1}", new_name, Program.LanguageResource.GetString("AddInputDeviceError2")));
                        }

                    }
                    else
                    {
                        finished = true;
                    }
                }


                return success;


            }
        }

        public static bool RemoveInputDevice()
        {
            if (Instance.inputMethodsList.SelectedItem != null && Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
            {
                var deviceToRemove = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;

                DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(deviceToRemove.EntryName), Program.LanguageResource.GetString("DeleteDeviceTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    InputDeviceManager.Devices.Remove(deviceToRemove.EntryName);
                    return true;
                }
                else return false;

            }
            else return false;
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
                            Instance.inputMethodsList.Items.Add(inputMethod);
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
            Instance.inputPanel.IsEnabled = true;
            Instance.inputErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
        }

        public static void ShowInputMappingErrorPanels()
        {
            Instance.inputPanel.IsEnabled = false;
            Instance.inputErrorMessage.Visibility = System.Windows.Visibility.Visible;
        }

        public static void AIRInputMappingsNullSituation(int situation = 0)
        {
            string hyperLink = MainDataModel.nL + Program.LanguageResource.GetString("ErrorHyperlinkClickMessage");
            if (situation == 0) Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError1") + hyperLink;
            else if (situation == 1) Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError2") + hyperLink;
            else if (situation == 2) Instance.inputErrorMessage.Content = Program.LanguageResource.GetString("InputMappingError3") + hyperLink;

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
            Instance.inputMethodsList.SelectionChanged -= Instance.InputMethodsList_SelectedIndexChanged;
            AIR_API.InputMappings.Device selectedItem = null;
            if (Instance.inputMethodsList.SelectedItem != null)
            {
                selectedItem = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
            }
            Instance.inputMethodsList.ItemsSource = null;
            Instance.inputMethodsList.Items.Refresh();
            if (InputDeviceManager.InputDevices != null)
            {
                if (Instance.inputMethodsList.Items.Count != 0 && Instance.inputMethodsList.ItemsSource == null) Instance.inputMethodsList.Items.Clear();
                Instance.inputMethodsList.ItemsSource = InputDeviceManager.InputDevices.Items.Select(x => x.Value);
                if (selectedItem != null && Instance.inputMethodsList.Items.Contains(selectedItem)) Instance.inputMethodsList.SelectedItem = selectedItem;
            }
            else
            {
                RecollectInputMappings();
            }
            Instance.inputMethodsList.SelectionChanged += Instance.InputMethodsList_SelectedIndexChanged;
        }

        public static void ImportInputDevice()
        {
            if (MainDataModel.Input_Settings != null)
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog()
                {
                    Filter = "Input File | *.json",
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    InputDeviceManager.InputDevices.ImportDevice(ofd.FileName);
                }
                RefreshInputMappings();
            }
        }

        public static void ExportInputDevice()
        {
            if (Instance.inputMethodsList.SelectedItem != null)
            {
                if (Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                {
                    AIR_API.InputMappings.Device device = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog()
                    {
                        FileName = string.Format("{0}", device.EntryName),
                        Filter = "Input File | *.json",
                    };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        device.ExportDevice(sfd.FileName);
                    }
                }
            }
        }

        public static void RemoveInputDeviceName()
        {
            if (Instance.inputMethodsList.SelectedItem != null && Instance.inputDeviceNamesList.SelectedItem != null)
            {
                if (Validate(Instance.inputDeviceNamesList.SelectedItem.ToString(), Instance.inputMethodsList.SelectedIndex, Instance.inputDeviceNamesList.SelectedIndex) == true)
                {
                    UpdateInputMappings();
                }
            }


            bool Validate(string selectedItemToRemove, int inputIndex, int nameIndex)
            {
                DialogResult result = MessageBox.Show(UserLanguage.RemoveInputDevice(selectedItemToRemove), Program.LanguageResource.GetString("DeleteDeviceTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = nameIndex;
                    InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.RemoveAt(index);
                    return true;
                }
                return false;
            }
        }

        public static void UpdateInputDeviceNamesList(bool refreshItems = false)
        {
            if (MainDataModel.Input_Settings != null)
            {
                if (Instance.inputMethodsList.SelectedItem != null)
                {
                    if (Instance.inputMethodsList.SelectedItem is AIR_API.InputMappings.Device)
                    {
                        AIR_API.InputMappings.Device device = Instance.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
                        if (device.HasDeviceNames)
                        {
                            if (refreshItems)
                            {
                                foreach (var name in device.DeviceNames)
                                {
                                    Instance.inputDeviceNamesList.Items.Add(name);
                                }
                            }
                            ToggleDeviceNamesUI(true);
                        }
                        else
                        {
                            Instance.inputDeviceNamesList.Items.Add((Program.LanguageResource.GetString("Input_UNSUPPORTED") == null ? "" : Program.LanguageResource.GetString("Input_UNSUPPORTED")));
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

        public enum MoveListItemDirection : int
        {
            MoveToTop = 0,
            MoveUp = 1,
            MoveDown = 2,
            MoveToBottom = 3
        }

        public static void MoveInputMethod(MoveListItemDirection direction, ref ModManager parent)
        {
            int index = -1;
            switch (direction)
            {
                case MoveListItemDirection.MoveToTop:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, 0);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(0);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveUp:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, index - 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(index - 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveDown:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, index + 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(index + 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
                case MoveListItemDirection.MoveToBottom:
                    index = parent.inputMethodsList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items.Move(index, InputDeviceManager.InputDevices.Items.Count - 1);

                        InputDeviceManager.RefreshInputMappings();
                        parent.inputMethodsList.SelectedItem = InputDeviceManager.InputDevices.Items.ElementAt(InputDeviceManager.InputDevices.Items.Count - 1);
                        InputDeviceManager.UpdateInputMappings();
                    }
                    break;
            }
        }

        public static void MoveDeviceName(MoveListItemDirection direction, ref ModManager parent)
        {
            var selectedItem = parent.inputMethodsList.SelectedItem as AIR_API.InputMappings.Device;
            int index = -1;
            int inputIndex = InputDeviceManager.InputDevices.Items.FindIndex(x => x.Value == selectedItem);
            switch (direction)
            {
                case MoveListItemDirection.MoveToTop:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, 0);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(0);
                    }
                    break;
                case MoveListItemDirection.MoveUp:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != 0)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, index - 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(index - 1);
                    }
                    break;
                case MoveListItemDirection.MoveDown:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, index + 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(index + 1);
                    }
                    break;
                case MoveListItemDirection.MoveToBottom:
                    index = parent.inputDeviceNamesList.SelectedIndex;
                    if (index != InputDeviceManager.InputDevices.Items.Count - 1)
                    {
                        InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Move(index, InputDeviceManager.InputDevices.Items.Count - 1);
                        InputDeviceManager.UpdateInputMappings();
                        parent.inputDeviceNamesList.SelectedItem = InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.ElementAt(InputDeviceManager.InputDevices.Items[inputIndex].Value.DeviceNames.Count - 1);
                    }
                    break;
            }
        }
    }
}
