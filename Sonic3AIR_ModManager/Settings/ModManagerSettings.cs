using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sonic3AIR_ModManager.Settings
{
    public class ModManagerSettings
    {
        public class Settings
        {
            public Settings()
            {
                ModCollections = new List<ModCollection>();
                LaunchPresets = new List<LaunchPreset>();
            }
            public List<ModCollection> ModCollections { get; set; }
            public List<LaunchPreset> LaunchPresets { get; set; }
        }

        public Settings Options { get; set; }

        public string FilePath { get; set; }


        public ModManagerSettings(string path)
        {
            FilePath = path;
            Load();
        }

        private void Load()
        {
            try
            {
                string data = File.ReadAllText(FilePath);
                Options = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(data);
                if (Options == null) Options = new Settings();
            }
            catch (Exception ex)
            {
                File.Create(FilePath).Close();
                Load();
            }

        }

        public void Save()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(Options, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }
    }
}
