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
    public class Mod
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string TechnicalName { get; set; }
        public string Description { get; set; }
        public string FolderName;
        public string FolderPath;
        public string URL;
        public string ModVersion;
        public string GameVersion;
        public bool EnabledLocal { get; set; }
        public bool IsEnabled { get; set; }
        public override string ToString() { return Name; }


        public Mod()
        {
            Author = "N/A";
            Name = "N/A";
            URL = "NULL";
            Description = "No Description Provided.";
            GameVersion = "N/A";
            ModVersion = "N/A";

            FolderName = "N/A";
            FolderPath = "N/A";
            TechnicalName = $"[{FolderName.Replace("#", "")}]";

        }

        public Mod(FileInfo mod)
        {
            string data = File.ReadAllText(mod.FullName);
            dynamic stuff = JRaw.Parse(data);
            //Author
            Author = stuff.Metadata.Author;
            if (Author == null) Author = "N/A";
            //Name
            Name = stuff.Metadata.Name;
            if (Name == null) Name = "N/A";
            //Description
            Description = stuff.Metadata.Description;
            if (Description == null) Description = "No Description Provided.";
            //Mod URL
            URL = stuff.Metadata.URL;
            if (URL == null) URL = "NULL";
            //ModVersion
            ModVersion = stuff.Metadata.ModVersion;
            if (ModVersion == null) ModVersion = "N/A";
            //GameVersion
            GameVersion = stuff.Metadata.GameVersion;
            if (GameVersion == null) GameVersion = "N/A";

            FolderName = mod.Directory.Name;
            FolderPath = mod.Directory.FullName;
            TechnicalName = $"[{FolderName.Replace("#", "")}]";

        }
    }
}
