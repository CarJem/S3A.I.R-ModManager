using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace AIR_SDK
{
    public class Settings
    {
        public bool FailSafeMode = false;
        public string Sonic3KRomPath = "";
        public bool FixGlitches = false;
        public string FilePath = "";
        private dynamic jsonObj;
        public Version Version;
        public Settings(FileInfo settings, bool JustGetVersion = false)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            Version targetVersion = new Version("19.08.17.0");
            bool isExceptionVersionRelatedForSure = false;

            try
            {
                jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

                string version = jsonObj.GameVersion;
                Version currentVersion = new Version(version);

                if (JustGetVersion)
                {
                    Version = currentVersion;
                }
                else
                {
                    var result = currentVersion.CompareTo(targetVersion);
                    if (result < 0)
                    {
                        MessageBox.Show($"Sonic 3 A.I.R is out of date, please use version {targetVersion.ToString()} or above! (and start it at least once fully)");
                        isExceptionVersionRelatedForSure = true;
                        throw new Exception();
                    }
                    else
                    {
                        Version = currentVersion;
                        FailSafeMode = jsonObj.FailSafeMode;
                        FixGlitches = jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES;
                        Sonic3KRomPath = jsonObj.RomPath;
                    }
                }

            }
            catch (Exception ex)
            {
                if (!isExceptionVersionRelatedForSure) MessageBox.Show("JSON Error, File Not Found, or A.I.R is Outdated! Unable to Load Mod Manager!" + Environment.NewLine + $"If AIR is out of date, please use version {targetVersion.ToString()} or above! (and start it at least once fully)");
                throw ex;
            }





        }

        private void PraseSettings()
        {
            if (FailSafeMode == true)
            {
                jsonObj.FailSafeMode = true;
            }
            else
            {
                jsonObj.FailSafeMode = false;
            }
            if (FixGlitches == true)
            {
                jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = 1;
            }
            else
            {
                jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = 0;
            }
            jsonObj.RomPath = Sonic3KRomPath;
        }

        public void SaveSettings()
        {
            PraseSettings();
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }
    }
}
