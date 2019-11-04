using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIR_SDK
{
    public class InputMappings
    {
        public static Device Keyboard1 = new Device(Device.DefaultDeviceType.Keyboard1);
        public static Device Keyboard2 = new Device(Device.DefaultDeviceType.Keyboard2);
        public static Device XBoxController = new Device(Device.DefaultDeviceType.XboxController);
        public static Device PS4Controller = new Device(Device.DefaultDeviceType.PS4Controller);
        public static Device LogitechController = new Device(Device.DefaultDeviceType.LogitechController);
        public static Device CustomController = new Device(Device.DefaultDeviceType.CustomController);

        public class ExportableDevice
        {
            public Device DeviceValues { get; set; }
            public string DeviceName { get; set; }

            public bool HasDeviceNames { get; set; }

            [Newtonsoft.Json.JsonConstructor]
            public ExportableDevice()
            {

            }

            public ExportableDevice(Device _device)
            {
                DeviceValues = _device;
                DeviceName = _device.EntryName;
                HasDeviceNames = _device.HasDeviceNames;
            }
        }
        public class Device
        {
            [JsonIgnore]
            public string EntryName { get; set; }



            public List<string> DeviceNames = new List<string>();
            public List<string> Up = new List<string>();
            public List<string> Down = new List<string>();
            public List<string> Left = new List<string>();
            public List<string> Right = new List<string>();
            public List<string> A = new List<string>();
            public List<string> B = new List<string>();
            public List<string> X = new List<string>();
            public List<string> Y = new List<string>();
            public List<string> Start = new List<string>();
            public List<string> Back = new List<string>();


            [JsonIgnore]
            public bool HasDeviceNames { get; set; } = false;

            public override string ToString()
            {
                return EntryName;
            }


            public void ExportDevice(string filePath)
            {
                var exportable = new ExportableDevice(this);
                File.WriteAllText(filePath, JsonConvert.SerializeObject(exportable, Formatting.Indented));
            }

            [Newtonsoft.Json.JsonConstructor]
            public Device()
            {

            }

            public enum DefaultDeviceType : int
            {
                Keyboard1 = 0,
                Keyboard2 = 1,
                XboxController = 2,
                PS4Controller = 3,
                LogitechController = 4,
                CustomController = 5
            }

            public Device(DefaultDeviceType type)
            {
                if (type == DefaultDeviceType.Keyboard1)
                {
                    Up.Add("Up");
                    Down.Add("Down");
                    Left.Add("Left");
                    Right.Add("Right");
                    A.Add("A");
                    B.Add("S");
                    X.AddRange(new List<string>() { "D", "Q" });
                    Y.Add("W");
                    Start.Add("Enter");
                    Back.Add("Space");

                    EntryName = "Keyboard1";

                    HasDeviceNames = false;

                }
                else if (type == DefaultDeviceType.Keyboard2)
                {
                    Up.Add("");
                    Down.Add("");
                    Left.Add("");
                    Right.Add("");
                    A.Add("");
                    B.Add("");
                    X.Add("");
                    Y.Add("");
                    Start.Add("");
                    Back.Add("");

                    EntryName = "Keyboard2";

                    HasDeviceNames = false;

                }
                if (type == DefaultDeviceType.XboxController)
                {
                    Up.AddRange(new List<string>() { "Axis2", "Pov0" });
                    Down.AddRange(new List<string>() { "Axis3", "Pov2" });
                    Left.AddRange(new List<string>() { "Axis0", "Pov3" });
                    Right.AddRange(new List<string>() { "Axis1", "Pov1" });
                    A.Add("Button0");
                    B.Add("Button1");
                    X.Add("Button2");
                    Y.Add("Button3");
                    Start.Add("Button7");
                    Back.Add("Button6");

                    EntryName = "XBoxController";

                    HasDeviceNames = true;
                    DeviceNames.AddRange(new List<string>() { "Controller (XBOX 360 for Windows)", "*" });

                }
                if (type == DefaultDeviceType.PS4Controller)
                {
                    Up.AddRange(new List<string>() { "Axis2", "Button11" });
                    Down.AddRange(new List<string>() { "Axis3", "Button12" });
                    Left.AddRange(new List<string>() { "Axis0", "Button13" });
                    Right.AddRange(new List<string>() { "Axis1", "Button14" });
                    A.Add("Button0");
                    B.Add("Button1");
                    X.Add("Button2");
                    Y.Add("Button3");
                    Start.AddRange(new List<string>() { "Button5", "Button6" });
                    Back.Add("Button4");

                    EntryName = "PS4Controller";

                    HasDeviceNames = true;
                    DeviceNames.AddRange(new List<string>() { "PS4 Controller" });

                }
                if (type == DefaultDeviceType.LogitechController)
                {
                    Up.AddRange(new List<string>() { "Axis2", "Pov0" });
                    Down.AddRange(new List<string>() { "Axis3", "Pov2" });
                    Left.AddRange(new List<string>() { "Axis0", "Pov3" });
                    Right.AddRange(new List<string>() { "Axis1", "Pov1" });
                    A.Add("Button1");
                    B.Add("Button2");
                    X.Add("Button0");
                    Y.Add("Button3");
                    Start.Add("Button9");
                    Back.Add("Button8");

                    EntryName = "LogitechController";

                    HasDeviceNames = true;
                    DeviceNames.AddRange(new List<string>() { "Logitech Dual Action" });

                }
                if (type == DefaultDeviceType.CustomController)
                {
                    Up.AddRange(new List<string>() { "Axis2", "Pov0" });
                    Down.AddRange(new List<string>() { "Axis3", "Pov2" });
                    Left.AddRange(new List<string>() { "Axis0", "Pov3" });
                    Right.AddRange(new List<string>() { "Axis1", "Pov1" });
                    A.Add("Button0");
                    B.Add("Button1");
                    X.Add("Button2");
                    Y.Add("Button3");
                    Start.Add("Button7");
                    Back.Add("Button6");

                    EntryName = "CustomController";

                    HasDeviceNames = true;
                    DeviceNames.AddRange(new List<string>() { " " });

                }
            }

            public Device(Newtonsoft.Json.Linq.JProperty Property)
            {
                EntryName = Property.Name;
                string message = Property.Name;

                GetButtonMappings(Property, "DeviceNames", ref DeviceNames);

                if (DeviceNames.Count == 0 && (EntryName == "Keyboard1" || EntryName == "Keyboard2")) HasDeviceNames = false;
                else HasDeviceNames = true;

                GetButtonMappings(Property, "Up", ref Up);
                GetButtonMappings(Property, "Down", ref Down);
                GetButtonMappings(Property, "Left", ref Left);
                GetButtonMappings(Property, "Right", ref Right);
                GetButtonMappings(Property, "A", ref A);
                GetButtonMappings(Property, "B", ref B);
                GetButtonMappings(Property, "X", ref X);
                GetButtonMappings(Property, "Y", ref Y);
                GetButtonMappings(Property, "Start", ref Start);
                GetButtonMappings(Property, "Back", ref Back);
            }


            public Device(string controller)
            {
                EntryName = controller;

                HasDeviceNames = true;
            }


            public void GetButtonMappings(Newtonsoft.Json.Linq.JProperty mapping, string name, ref List<string> list)
            {
                foreach (var item in mapping.Children().FirstOrDefault().SelectTokens(name))
                {
                    foreach (var entry in item.Children())
                    {
                        list.Add(entry.ToString());
                    }
                }
            }
        }
    }
}
