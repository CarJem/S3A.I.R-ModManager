using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonic3AIR_ModManager.Settings
{
    public class LaunchPreset
    {
        public string Name { get; set; }

        public LaunchPreset(string _name)
        {
            Name = _name;
        }

    }
}
