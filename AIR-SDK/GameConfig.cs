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
    public class GameConfig
    {
        public string FilePath = "";
        private dynamic RawJSON;
        public InputDevices Input;
        public GameConfig(FileInfo config)
        {
            FilePath = config.FullName;
            string data = File.ReadAllText(FilePath);
            RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            Input = new InputDevices(RawJSON);
        }
        public void Save()
        {
            string json = Input.Save();
            RawJSON.InputDevices = JObject.Parse(json);
            File.WriteAllText(FilePath, RawJSON.ToString());
        }
        public class InputDevices
        {
            public Device Keyboard1;
            public Device Keyboard2;
            public Device XBoxController;
            public Device PS4Controller;
            public Device LogitechController;
            public Device CustomController;

            [JsonIgnore]
            public List<Device> Devices
            {
                get
                {
                    return new List<Device>() { Keyboard1, Keyboard2, XBoxController, PS4Controller, LogitechController, CustomController };
                }
            }

            [JsonIgnore]
            public dynamic RawJSON;
            [JsonIgnore]
            public Newtonsoft.Json.Linq.JProperty Property;

            public InputDevices(dynamic obj)
            {
                RawJSON = obj;
                foreach (var device in RawJSON.InputDevices)
                {
                    if (device is Newtonsoft.Json.Linq.JProperty)
                    {
                        Newtonsoft.Json.Linq.JProperty deviceProp = device;
                        string name = deviceProp.Name;
                        if (name == "Keyboard1") Keyboard1 = new Device(deviceProp);
                        else if (name == "Keyboard2") Keyboard2 = new Device(deviceProp);
                        else if (name == "XBoxController") XBoxController = new Device(deviceProp);
                        else if (name == "PS4Controller") PS4Controller = new Device(deviceProp);
                        else if (name == "LogitechController") LogitechController = new Device(deviceProp);
                        else if (name == "CustomController") CustomController = new Device(deviceProp);
                    }
                }
            }

            public string Save()
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                return json;
            }

            public class Device
            {
                [JsonIgnore]
                public string EntryName { get; set; }

                Newtonsoft.Json.Linq.JProperty Property;

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
                public bool HasDeviceNames = false;

                public override string ToString()
                {
                    return EntryName;
                }
                public Device(Newtonsoft.Json.Linq.JProperty mapping)
                {
                    Property = mapping;
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
}
