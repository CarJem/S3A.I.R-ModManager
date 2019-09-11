using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace AIR_SDK
{
    public class Recording
    {
        public string Name;
        public string FilePath;
        public string FormalName;

        public override string ToString() { return Name; }
        public string GetRAW()
        {
            var binData = File.ReadAllBytes(FilePath);
            var sb = new StringBuilder();
            foreach (var b in binData)
                sb.Append(" " + b.ToString("X2"));
            return sb.ToString();
        }

        public Recording(FileInfo file)
        {
            FilePath = file.FullName;

            string baseString = file.Name.Replace("gamerecording_", "");

            string month = baseString.Substring(2, 2);
            string day = baseString.Substring(4, 2);
            string year = baseString.Substring(0, 2);

            string hour = baseString.Substring(7, 2);
            string minute = baseString.Substring(9, 2);
            string second = baseString.Substring(11, 2);

            string recordingFormat = $"Recording: {month}/{day}/{year} - {hour}.{minute}.{second}";

            Name = recordingFormat;

            FormalName = $"Sonic 3 AIR Recording [{recordingFormat}] ";
        }



    }
}
