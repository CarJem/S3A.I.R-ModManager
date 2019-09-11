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
    public class VersionCheck
    {
        public string FilePath = "";
        private dynamic RawJSON;

        public string VersionString;
        public Version Version;
        public string DownloadURL;
        public VersionCheck(FileInfo config)
        {
            FilePath = config.FullName;
            string data = File.ReadAllText(FilePath);
            RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            VersionString = RawJSON.Stable.Version;
            Version = new Version(VersionString);
            DownloadURL = RawJSON.Stable.URL;
        }
    }
}
