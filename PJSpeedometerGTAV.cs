using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using NativeUI;
using System.Windows.Forms;

namespace ProjectSpeedometerGTAV
{
    public class PJSpeedometerGTAV : Script
    {
        private string yourname;

        public PJSpeedometerGTAV()
        {
            KeyDown += OnKeyDown;

            LoadConfig();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                GTA.UI.Screen.ShowSubtitle("Hello my name is " + yourname , 3000);
            }
        }

        private void LoadConfig()
        {
            ScriptSettings settings = ScriptSettings.Load("scripts\\ProjectSpeedometerGTAV\\player.ini");

            yourname = settings.GetValue("Player","Name","");
        }
    }
}
