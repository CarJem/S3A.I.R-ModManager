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
        public string AIREXEPath = "";
        public bool HasEXEPath = true;
        public string FilePath = "";
        private dynamic jsonObj;
        public Version Version = new Version();

        public LoadOptions DefaultLoadOptions;


        public class LoadOptions
        {
            public bool ThrowNoExceptionsForMissingAttributesBesidesVersion = false;
            public bool ThrowNoExceptionsForVersionMismatch = false;
            public Version TargetVersion = new Version("19.08.17.0");

            public LoadOptions(bool _ThrowNoExceptionsForMissingAttributes = false, Version _targetVersion = null, bool _ThrowNoExceptionsForVersionMismatch = false)
            {
                ThrowNoExceptionsForMissingAttributesBesidesVersion = _ThrowNoExceptionsForMissingAttributes;
                if (_targetVersion != null) TargetVersion = _targetVersion;
                ThrowNoExceptionsForVersionMismatch = _ThrowNoExceptionsForVersionMismatch;
            }
        }

        
        public Settings(FileInfo settings, LoadOptions loadOptions = null)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            bool isExceptionVersionRelatedForSure = false;
            if (loadOptions == null) loadOptions = new LoadOptions();

            try
            {
                jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

                try
                {
                    string version = jsonObj.GameVersion;
                    Version = new Version(version);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    var result = Version.CompareTo(loadOptions.TargetVersion);
                    if (result < 0 && loadOptions.ThrowNoExceptionsForVersionMismatch)
                    {
                        MessageBox.Show($"Sonic 3 A.I.R is out of date, please use version {loadOptions.TargetVersion.ToString()} or above! (and start it at least once fully)");
                        isExceptionVersionRelatedForSure = true;
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    if (!loadOptions.ThrowNoExceptionsForMissingAttributesBesidesVersion) throw ex;
                }

                try
                {
                    FailSafeMode = jsonObj.FailSafeMode;
                }
                catch (Exception ex)
                {
                    if (!loadOptions.ThrowNoExceptionsForMissingAttributesBesidesVersion) throw ex;
                }

                try
                {
                    FixGlitches = jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES;
                }
                catch (Exception ex)
                {
                    if (!loadOptions.ThrowNoExceptionsForMissingAttributesBesidesVersion) throw ex;
                }

                try
                {
                    Sonic3KRomPath = jsonObj.RomPath;
                }
                catch (Exception ex)
                {
                    if (!loadOptions.ThrowNoExceptionsForMissingAttributesBesidesVersion) throw ex;
                }

                try
                {
                    AIREXEPath = jsonObj.GameExePath;
                }
                catch
                {
                    AIREXEPath = "";
                    HasEXEPath = false;
                }

            }
            catch (Exception ex)
            {
                if (!isExceptionVersionRelatedForSure) MessageBox.Show("JSON Error, File Not Found, or A.I.R is Outdated! Unable to Load Mod Manager!" + Environment.NewLine + $"If AIR is out of date, please use version {loadOptions.TargetVersion.ToString()} or above! (and start it at least once fully)");
                throw ex;
            }





        }
        

            /*
        public Settings (FileInfo settings, LoadOptions loadOptions = null)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            if (loadOptions == null) loadOptions = new LoadOptions();
            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            string version = (jsonObj.GameVersion == null ? "0.0.0.0" : jsonObj.GameVersion);
            Version = new Version(version);
            FailSafeMode = (jsonObj.FailSafeMode == null ? false : jsonObj.FailSafeMode);
            Sonic3KRomPath = (jsonObj.RomPath == null ? "NULL" : jsonObj.RomPath);
            FixGlitches = (jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES == null ? false : jsonObj.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES);
            AIREXEPath = (jsonObj.GameExePath == null ? "" : jsonObj.GameExePath);
            HasEXEPath = (jsonObj.GameExePath == null ? false : true);
        }*/

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
