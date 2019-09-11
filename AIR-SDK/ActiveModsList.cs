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
    public class ActiveModsList
    {
        public List<string> ActiveMods = new List<string>();
        public string ConfigPath = "";
        public ActiveModsList(FileInfo config)
        {
            ConfigPath = config.FullName;
            Load();
        }

        public void Load()
        {
            try
            {
                string data = File.ReadAllText(ConfigPath);
                JToken stuff = JRaw.Parse(data);
                foreach (JProperty content in stuff.Children())
                {
                    if (content.HasValues)
                    {
                        ActiveMods.AddRange(content.Value.ToObject<List<string>>());
                    }
                }
            }
            catch (Exception ex)
            {
                CreateFile(ConfigPath);
                Load();
            }

        }

        public void CreateFile(string filePath)
        {
            var myFile = File.Create(filePath);
            myFile.Close();
            string nL = Environment.NewLine;
            string bracketOpen = "{";
            string bracketClose = "}";
            string standardFile = $"{bracketOpen}{nL}\t\"ActiveMods\": [{nL}{nL}\t]{nL}{bracketClose}";
            using (StreamWriter writetext = new StreamWriter(filePath)) writetext.WriteLine(standardFile);
        }

        public ActiveModsList(string filePath)
        {
            CreateFile(filePath);
            ConfigPath = filePath;
            Load();
        }

        public void Save(List<string> CurrentActiveMods)
        {
            var myFile = File.Create(ConfigPath);
            myFile.Close();
            string nL = Environment.NewLine;
            string bracketOpen = "{";
            string bracketClose = "}";
            string fileHeader = $"{bracketOpen}{nL}\t\"ActiveMods\": [{nL}";
            string fileFooter = $"{nL}\t]{nL}{bracketClose}";
            string fileContents = fileHeader + GetFiles() + fileFooter;
            using (StreamWriter writetext = new StreamWriter(ConfigPath)) writetext.WriteLine(fileContents);

            string GetFiles()
            {
                string fileList = "";
                string formatHead = "\t\t\"";
                string formatFoot = "\",";
                string formatFootEndofList = $"\"";
                for (int i = 0; i < CurrentActiveMods.Count; i++)
                {
                    if (i >= CurrentActiveMods.Count - 1) fileList += $"{nL}{formatHead}{CurrentActiveMods[i]}{formatFootEndofList}";
                    else fileList += $"{nL}{formatHead}{CurrentActiveMods[i]}{formatFoot}";
                }
                return fileList;
            }

            ActiveMods = CurrentActiveMods;
        }


    }
}
