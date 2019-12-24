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
            public string Sonic3AIRPath { get; set; } = "";
            public bool AutoLaunch { get; set; } = false;
            public bool KeepOpenOnLaunch { get; set; } = false;
            public bool KeepOpenOnQuit { get; set; } = false;
            public int AutoLaunchDelay { get; set; } = 5;
            public bool AutoUpdates { get; set; } = true;
            public string UserLanguage { get; set; } = "EN";
            public bool UseDarkTheme { get; set; } = false;
            public System.Drawing.Size WindowSize { get; set; } = new System.Drawing.Size(0, 0);
            public bool ShowFullDebugOutput { get; set; } = false;
            public bool DisableInGameEnhancements { get; set; } = false;
            public bool ShowDiscordRPC { get; set; } = true;

        }

        public Settings Options { get; set; }
        public string FilePath { get; set; }
        public List<ModCollection> ModCollections
        {
            get { return Options.ModCollections; }
            set { Options.ModCollections = value; Save(); }
        }
        public List<LaunchPreset> LaunchPresets
        {
            get { return Options.LaunchPresets; }
            set { Options.LaunchPresets = value; Save(); }
        }
        public string Sonic3AIRPath 
        { 
            get { return Options.Sonic3AIRPath; } 
            set { Options.Sonic3AIRPath = value; Save(); } 
        }
        public bool AutoLaunch 
        { 
            get { return Options.AutoLaunch; }
            set { Options.AutoLaunch = value; Save(); } 
        }
        public bool KeepOpenOnLaunch 
        { 
            get { return Options.KeepOpenOnLaunch; } 
            set { Options.KeepOpenOnLaunch = value; Save(); } 
        }
        public bool KeepOpenOnQuit 
        { 
            get { return Options.KeepOpenOnQuit; } 
            set { Options.KeepOpenOnQuit = value; Save(); } 
        }
        public int AutoLaunchDelay 
        { 
            get { return Options.AutoLaunchDelay; } 
            set { Options.AutoLaunchDelay = value; Save(); } 
        }
        public bool AutoUpdates 
        { 
            get { return Options.AutoUpdates; } 
            set { Options.AutoUpdates = value; Save(); } 
        }
        public string UserLanguage 
        { 
            get { return Options.UserLanguage; } 
            set { Options.UserLanguage = value; Save(); } 
        }
        public bool UseDarkTheme
        {
            get { return Options.UseDarkTheme; }
            set { Options.UseDarkTheme = value; Save(); }
        }
        public System.Drawing.Size WindowSize
        {
            get { return Options.WindowSize; }
            set { Options.WindowSize = value; Save(); }
        }
        public bool ShowFullDebugOutput
        {
            get { return Options.ShowFullDebugOutput; }
            set { Options.ShowFullDebugOutput = value; Save(); }
        }
        public bool DisableInGameEnhancements
        {
            get { return Options.DisableInGameEnhancements; }
            set { Options.DisableInGameEnhancements = value; Save(); }
        }
        public bool ShowDiscordRPC
        {
            get { return Options.ShowDiscordRPC; }
            set { Options.ShowDiscordRPC = value; Save(); }
        }


        public ModManagerSettings(string path)
        {
            FilePath = path;
            Load();
            Save();
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
