using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace AntiFriendlyFire
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public int GodModeWait;

        public void LoadDefaults()
        {
            Enabled = true;
            GodModeWait = 1000;
        }
    }
}
