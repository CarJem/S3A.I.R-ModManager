using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonic3AIR_ModManager.Settings
{
    public class ModCollection
    {
        public string Name { get; set; }
        public AIR_API.AIRActiveMods Mods { get; set; }

        public ModCollection(AIR_API.AIRActiveMods _mods, string _name)
        {
            Name = _name;
            Mods = _mods;
        }

    }
}
