using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Device = AIR_SDK.InputMappings.Device;
using ExportableDevice = AIR_SDK.InputMappings.ExportableDevice;

namespace AIR_SDK
{
    public class GameConfig
    {
        [JsonIgnore]
        public string FilePath = "";

        [JsonIgnore]
        private dynamic RawJSON;
        public Dictionary<string, Device> Devices;
        public List<Device> InputDevices
        {
            get => Devices.Values.ToList();
        }


        public GameConfig(FileInfo config)
        {
            FilePath = config.FullName;
            Devices = new Dictionary<string, Device>();
            string data = File.ReadAllText(FilePath);
            RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            foreach (var device in RawJSON.InputDevices)
            {
                if (device is Newtonsoft.Json.Linq.JProperty)
                {
                    Newtonsoft.Json.Linq.JProperty deviceProp = device;
                    Devices.Add(deviceProp.Name,new Device(deviceProp));
                }
            }
        }

        public void ResetDevicesToDefault()
        {
            Devices.Clear();
            Devices.Add("Keyboard1", InputMappings.Keyboard1);
            Devices.Add("Keyboard2", InputMappings.Keyboard2);
            Devices.Add("XBoxController", InputMappings.XBoxController);
            Devices.Add("PS4Controller", InputMappings.PS4Controller);
            Devices.Add("LogitechController", InputMappings.LogitechController);
            Devices.Add("CustomController", InputMappings.CustomController);
        }

        public void ImportDevice(string filePath)
        {
            string data = File.ReadAllText(filePath);
            ExportableDevice deviceImport = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportableDevice>(data);

            string intial_name = deviceImport.DeviceName;
            int copy_number = 0;
            while (Devices.ContainsKey(deviceImport.DeviceName))
            {
                copy_number++;
                deviceImport.DeviceName = string.Format("{0}{1}", intial_name, copy_number);
            }

            if (deviceImport.HasDeviceNames) deviceImport.DeviceValues.HasDeviceNames = deviceImport.HasDeviceNames;
            else if (deviceImport.DeviceValues.DeviceNames.Count > 0) deviceImport.DeviceValues.HasDeviceNames = true;
            deviceImport.DeviceValues.EntryName = deviceImport.DeviceName;
            Devices.Add(deviceImport.DeviceName, deviceImport.DeviceValues);

        }


        public void Save()
        {
            RawJSON.InputDevices = JObject.Parse(JsonConvert.SerializeObject(Devices));
            File.WriteAllText(FilePath, RawJSON.ToString());
        }





        
    }
}
