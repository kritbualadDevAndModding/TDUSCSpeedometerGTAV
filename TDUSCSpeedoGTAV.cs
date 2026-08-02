using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using GTA.Native;
using NativeUI;
using System.Windows.Forms;
using System.Drawing;

namespace TDUSCSpeedometerGTAV
{
    public class TDUSCSpeedoGTAV : Script
    {

        public TDUSCSpeedoGTAV()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            Vehicle speedText = Game.Player.Character.CurrentVehicle;

            if (speedText != null)
            {
                float speedkmh = speedText.Speed * 3.6f;
                float speedmph = speedText.Speed * 2.23694f;

                Function.Call(Hash.SET_TEXT_FONT, 0); // Text Fonts
                Function.Call(Hash.SET_TEXT_SCALE, 1f, 1f); // Text Scale
                Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255); // Text Color
                Function.Call(Hash.SET_TEXT_OUTLINE, 3); // Text Outline
                Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "SPEED" + "\n" + speedkmh.ToString("0") + " KM/H" + "\n" + speedmph.ToString("0") + " MPH"); // Speed Text
                Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, 0.68f, 0.77f); // Position

            }

            Vehicle vehicleRPM = Game.Player.Character.CurrentVehicle;

            if (vehicleRPM != null)
            {
                float rpm = vehicleRPM.CurrentRPM;

                string text = $"RPM : {(rpm * 1000):0}";

                Function.Call(Hash.SET_TEXT_FONT, 0);
                Function.Call(Hash.SET_TEXT_SCALE, 0.8f, 0.8f);
                Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
                Function.Call(Hash.SET_TEXT_OUTLINE, 3);
                Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
                Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, 0.80f, 0.80f);
            }

            Vehicle vehicleGEAR = Game.Player.Character.CurrentVehicle;

            
            if (vehicleRPM != null)
            {
                string gearText;
                if (vehicleGEAR.CurrentGear == 0)
                {
                    gearText = "R";
                }
                else
                {
                    gearText = vehicleGEAR.CurrentGear.ToString();
                }

                Function.Call(Hash.SET_TEXT_FONT, 0);
                Function.Call(Hash.SET_TEXT_SCALE, 0.8f, 0.8f);
                Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
                Function.Call(Hash.SET_TEXT_OUTLINE, 3);
                Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "GEAR : " + gearText);
                Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, 0.80f, 0.75f);
            }
        }
    }
}
