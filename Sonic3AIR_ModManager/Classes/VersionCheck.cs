using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sonic3AIR_ModManager.Classes
{
    public class VersionCheck
    {
        public string FilePath = "";
        private dynamic RawJSON;

        public string VersionString;
        public Version Version;
        public string DownloadURL;
        public string Details = "";
        public VersionCheck(FileInfo config)
        {
            FilePath = config.FullName;
            string data = File.ReadAllText(FilePath);
            RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);


            VersionString = RawJSON.Metadata.Version;
            Version = new Version(VersionString);
            DownloadURL = RawJSON.Metadata.DownloadURL;
            Details = RawJSON.Metadata.Details;
        }
    }
}
