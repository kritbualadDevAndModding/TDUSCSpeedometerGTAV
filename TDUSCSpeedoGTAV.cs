using GTA;
using GTA.Math;
using GTA.Native;
using GTA.NaturalMotion;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace TDUSCSpeedometerGTAV
{
    public class TDUSCSpeedoGTAV : Script
    {

        private string version = "1.0";
        private string credits = "kritbualad";

        public static Color SpriteColor = Color.FromArgb(255, 255, 255, 255);

        private float UIPositionX = 1570f;
        private float UIPositionY = 725f;

        private float UISize = 295f;


        // Gear Colour ARGB
        private int A_GearColour = 255;
        private int R_GearColour = 180;
        private int G_GearColour = 180;
        private int B_GearColour = 180;

        private int A_GearColour_Red = 255;
        private int R_GearColour_Red = 255;
        private int G_GearColour_Red = 0;
        private int B_GearColour_Red = 0;

        private bool gearhudcolour_default;
        private bool gearhudcolour_red;
        private bool gearhudcolour_default_season5;
        private bool gearhudcolour_red_season5;
        // ตัวแปรเก็บสถานะล่าสุดเพื่อไม่ให้ Debug Notification ขึ้นรัวๆ ทุกเฟรม
        private bool wasPhoneVisible = false;

        // Menu UI
        private ObjectPool _menuPool;
        private NativeMenu _mainMenu;

        // Track the last vehicle the player was in to detect when they exit
        private Vehicle _lastVehicle = null;
        private bool _wasInVehicle = false;


        // Drift Status
        private const float DriftAngleThreshold = 10.0f;
        private const float MinSpeedThreshold = 3.0f;

        private bool isKMH;

        private bool isMPH;

        // config ini
        private ScriptSettings config;

        /* สำหรับทำงานผ่าน ytd update\update.rpf\x64\textures\script_txds.rpf */
        private ScaledTexture gearTexture;
        private ScaledTexture gearTexture_Red;
        private ScaledTexture gearTexture_Release;
        private ScaledTexture gearTexture_Release_Red;
        private ScaledTexture gearTexture_Season5;
        private ScaledTexture gearTexture_Season5_Red;
        private int lastGear = -1;
        private bool wasNeutral = false;

        // กำหนดชื่อไฟล์ .ytd ที่เราใส่ไว้ใน script_txds.rpf (ใส่แค่ชื่อ ไม่ต้องระบุนามสกุล .ytd)


        private int SelectVersionHUD;

        private string HUDBackground;
        private string BackgroundTextureName;
        private string HUDHandbrake;
        private string HUDTurnSignalLeft;
        private string HUDTurnSignalRight;
        private string HUDGear;
        private string HUDRev;
        private string HUDSpeed;
        private string HUDABS;
        private string HUDTCS;
        
        private ScaledTexture BackgroundTextureImage;
        private ScaledTexture BackgroundTextureImage2;
        private const string BackgroundTextureName2 = "background2";

        
          private ScaledTexture ImageKMH;
      //  private string ImageKMHName = "";
          private ScaledTexture ImageMPH;
      //  private string ImageMPHName = "mph";




        private ScaledTexture DriftTextureImage;
        private const string DriftTextureName = "Drift";
        private ScaledTexture HandbrakeTextureImage;
        private const string HandbrakeTextureName = "Parking_Brake";


        private ScaledTexture speedTextureImage;
        private const string SpeedBaseTextureName = "speed_";

        private ScaledTexture RevTextureImage;
        private const string RevBaseTextureName = "rev_";

        private ScaledTexture ABSTextureImage;
        private ScaledTexture TCSTextureImage;

        private ScaledTexture TurnSignalLeft;

        private ScaledTexture TurnSignalRight;

        private ScaledTexture RevEngineOffImage;

        private ScaledTexture HeadlightsImage;

        private ScaledTexture ThrottleImageHUD;
        private ScaledTexture BrakeImageHUD;

        private bool TurnSignalLeft_Enabled;

        private bool TurnSignalRight_Enabled;

        // Set LabelRPM
        private ScaledTexture LabelRPMDeafult;
        private const string LabelRPMDefaultName = "rpm_9000";

        private ScaledTexture LabelRPM_2000;
        private ScaledTexture LabelRPM_2200;
        private ScaledTexture LabelRPM_2400;
        private ScaledTexture LabelRPM_2600;
        private ScaledTexture LabelRPM_2800;
        private ScaledTexture LabelRPM_3000;
        private ScaledTexture LabelRPM_4000;
        private ScaledTexture LabelRPM_4500;
        private ScaledTexture LabelRPM_5000;
        private ScaledTexture LabelRPM_5200;
        private ScaledTexture LabelRPM_5400;
        private ScaledTexture LabelRPM_5600;
        private ScaledTexture LabelRPM_5800;
        private ScaledTexture LabelRPM_6000;
        private ScaledTexture LabelRPM_6200;
        private ScaledTexture LabelRPM_6400;
        private ScaledTexture LabelRPM_6600;
        private ScaledTexture LabelRPM_6800;
        private ScaledTexture LabelRPM_7000;
        private ScaledTexture LabelRPM_7200;
        private ScaledTexture LabelRPM_7400;
        private ScaledTexture LabelRPM_7600;
        private ScaledTexture LabelRPM_7800;
        private ScaledTexture LabelRPM_8000;
        private ScaledTexture LabelRPM_8200;
        private ScaledTexture LabelRPM_8400;
        private ScaledTexture LabelRPM_8600;
        private ScaledTexture LabelRPM_8800;
        private ScaledTexture LabelRPM_9000;
        private ScaledTexture LabelRPM_9200;
        private ScaledTexture LabelRPM_9400;
        private ScaledTexture LabelRPM_9600;
        private ScaledTexture LabelRPM_9800;
        private ScaledTexture LabelRPM_10000;
        private ScaledTexture LabelRPM_10500;
        private ScaledTexture LabelRPM_11000;
        private ScaledTexture LabelRPM_11500;
        private ScaledTexture LabelRPM_12000;
        private ScaledTexture LabelRPM_12500;
        private ScaledTexture LabelRPM_13000;
        private ScaledTexture LabelRPM_13500;
        private ScaledTexture LabelRPM_14000;
        private ScaledTexture LabelRPM_14500;
        private ScaledTexture LabelRPM_15000;
        private ScaledTexture LabelRPM_15500;
        private ScaledTexture LabelRPM_16000;
        private ScaledTexture LabelRPM_16500;
        private ScaledTexture LabelRPM_17000;
        private ScaledTexture LabelRPM_18000;
        private ScaledTexture LabelRPM_19000;


        private const string LabelRPM_2000_Name = "rpm_2000";
        private const string LabelRPM_2200_Name = "rpm_2200";
        private const string LabelRPM_2400_Name = "rpm_2400";
        private const string LabelRPM_2600_Name = "rpm_2600";
        private const string LabelRPM_2800_Name = "rpm_2800";
        private const string LabelRPM_3000_Name = "rpm_3000";
        private const string LabelRPM_4000_Name = "rpm_4000";
        private const string LabelRPM_4500_Name = "rpm_4500";
        private const string LabelRPM_5000_Name = "rpm_5000";
        private const string LabelRPM_5200_Name = "rpm_5200";
        private const string LabelRPM_5400_Name = "rpm_5400";
        private const string LabelRPM_5600_Name = "rpm_5600";
        private const string LabelRPM_5800_Name = "rpm_5800";
        private const string LabelRPM_6000_Name = "rpm_6000";
        private const string LabelRPM_6200_Name = "rpm_6200";
        private const string LabelRPM_6400_Name = "rpm_6400";
        private const string LabelRPM_6600_Name = "rpm_6600";
        private const string LabelRPM_6800_Name = "rpm_6800";
        private const string LabelRPM_7000_Name = "rpm_7000";
        private const string LabelRPM_7200_Name = "rpm_7200";
        private const string LabelRPM_7400_Name = "rpm_7400";
        private const string LabelRPM_7600_Name = "rpm_7600";
        private const string LabelRPM_7800_Name = "rpm_7800";
        private const string LabelRPM_8000_Name = "rpm_8000";
        private const string LabelRPM_8200_Name = "rpm_8200";
        private const string LabelRPM_8400_Name = "rpm_8400";
        private const string LabelRPM_8600_Name = "rpm_8600";
        private const string LabelRPM_8800_Name = "rpm_8800";
        private const string LabelRPM_9000_Name = "rpm_9000";
        private const string LabelRPM_9200_Name = "rpm_9200";
        private const string LabelRPM_9400_Name = "rpm_9400";
        private const string LabelRPM_9600_Name = "rpm_9600";
        private const string LabelRPM_9800_Name = "rpm_9800";
        private const string LabelRPM_10000_Name = "rpm_10000";
        private const string LabelRPM_10500_Name = "rpm_10500";
        private const string LabelRPM_11000_Name = "rpm_11000";
        private const string LabelRPM_11500_Name = "rpm_11500";
        private const string LabelRPM_12000_Name = "rpm_12000";
        private const string LabelRPM_12500_Name = "rpm_12500";
        private const string LabelRPM_13000_Name = "rpm_13000";
        private const string LabelRPM_13500_Name = "rpm_13500";
        private const string LabelRPM_14000_Name = "rpm_14000";
        private const string LabelRPM_14500_Name = "rpm_14500";
        private const string LabelRPM_15000_Name = "rpm_15000";
        private const string LabelRPM_15500_Name = "rpm_15500";
        private const string LabelRPM_16000_Name = "rpm_16000";
        private const string LabelRPM_16500_Name = "rpm_16500";
        private const string LabelRPM_17000_Name = "rpm_17000";
        private const string LabelRPM_18000_Name = "rpm_18000";
        private const string LabelRPM_19000_Name = "rpm_19000";

        private int isUnitsLanguage;

        private float isSwitchUnits;
        private float smoothedRpm = 0f;
        private float interpolationSpeed = 0.15f;

        private int rpmMin;
        private int rpmMax;


        private int rpm_2000 = 2000;
        private int rpm_2200 = 2200;
        private int rpm_2400 = 2400;
        private int rpm_2600 = 2600;
        private int rpm_2800 = 2800;
        private int rpm_3000 = 3000;
        private int rpm_4000 = 4000;
        private int rpm_4500 = 4500;
        private int rpm_5000 = 5000;
        private int rpm_5200 = 5200;
        private int rpm_5400 = 5400;
        private int rpm_5600 = 5600;
        private int rpm_5800 = 5800;
        private int rpm_6000 = 6000;
        private int rpm_6200 = 6200;
        private int rpm_6400 = 6400;
        private int rpm_6600 = 6600;
        private int rpm_6800 = 6800;
        private int rpm_7000 = 7000;
        private int rpm_7200 = 7200;
        private int rpm_7400 = 7400;
        private int rpm_7600 = 7600;
        private int rpm_7800 = 7800;
        private int rpm_8000 = 8000;
        private int rpm_8200 = 8200;
        private int rpm_8400 = 8400;
        private int rpm_8600 = 8600;
        private int rpm_8800 = 8800;
        private int rpm_9000 = 9000;
        private int rpm_9200 = 9200;
        private int rpm_9400 = 9400;
        private int rpm_9600 = 9600;
        private int rpm_9800 = 9800;
        private int rpm_10000 = 10000;
        private int rpm_10500 = 10500;
        private int rpm_11000 = 11000;
        private int rpm_11500 = 11500;
        private int rpm_12000 = 12000;
        private int rpm_12500 = 12500;
        private int rpm_13000 = 13000;
        private int rpm_13500 = 13500;
        private int rpm_14000 = 14000;
        private int rpm_14500 = 14500;
        private int rpm_15000 = 15000;
        private int rpm_15500 = 15500;
        private int rpm_16000 = 16000;
        private int rpm_16500 = 16500;
        private int rpm_17000 = 17000;
        private int rpm_18000 = 18000;
        private int rpm_19000 = 19000;


        private bool isEngineOn = false;

        private int UseKMH;

        private float speedKMH = 3.6f;
        private float speedMPH = 2.236936f;

        private Keys keyOpenMenu;

        private string LabelRPMCategory;


        // Set RPM
        private int SetRPM;
        private int currentRPM;

        private readonly string configPath = "TDUSCSpeedometerGTAVSetRPM\\Example.cfg";

        // Dictionary สำหรับเก็บข้อมูลคู่ [ชื่อโมเดลตัวพิมพ์เล็ก, ชื่อแสดงผล] เพื่อเอาไว้ค้นหาตอนขึ้นรถ
        private Dictionary<string, CarData> carNamesDict = new Dictionary<string, CarData>(StringComparer.OrdinalIgnoreCase);
        private int lastVehicleHandle = 0;

        private string activeTextureKMHLanguageName;
        private string activeTextureMPHLanguageName;
        private List<string> ImageKMHLanguage = new List<string> { "kmh" ,"kmh_DenmarkNorway","kmh_Nederlands","kmh_Russian","kmh_Turkish","kmh_Ukriane","kmh_Vietnam"};
        private List<string> ImageMPHLanguage = new List<string> { "mph", "mph", "mph_Nederlands", "mph_Russian", "mph_Turkish", "mph_Ukriane", "mph_Vietnam" };

        // 2. ชุดข้อมูลชื่อไฟล์ .ytd (Texture Dictionary) ที่เรียงลำดับให้ตรงกัน
        private List<string> textureDicts = new List<string> { "tdu_demo_dict", "tdu_release_dict", "tdu_season5_dict" };

        private List<string> ImageUseKMHSettings = new List<string> { "KM/H", "MPH"};

        private string activeUseKMH;

        private bool isDriftHUD;

        private int headlightToggle;

        private bool isHUDThrottleAndBrakeSeason5;
        public TDUSCSpeedoGTAV()
        {
            LoadConfigData();
            // อ่านไฟล์ .ini
            config = ScriptSettings.Load("scripts\\TDUSCSpeedometerGTAV.ini");

            keyOpenMenu = config.GetValue<Keys>("Open_Menu", "MenuKey", keyOpenMenu);
            UIPositionX = config.GetValue<float>("Position_HUD", "PosX", UIPositionX);
            UIPositionY = config.GetValue<float>("Position_HUD", "PosY", UIPositionY);

            isABSHUD = config.GetValue<bool>("HUD_SETTINGS", "ABSHUD", isABSHUD);
            isTCSHUD = config.GetValue<bool>("HUD_SETTINGS", "TCSHUD", isTCSHUD);
            isMainHUDModEnabled = config.GetValue<bool>("HUD_SETTINGS", "Mod_Enabled", isMainHUDModEnabled);
            SelectVersionHUD = config.GetValue<int>("HUD_SETTINGS", "Version_HUD", SelectVersionHUD);

            UISize = config.GetValue<float>("Size_HUD", "Scale", UISize);

            UseKMH = config.GetValue<int>("Units_Settings", "UseKMH", UseKMH);
            isUnitsLanguage = config.GetValue<int>("Units_Settings", "LanguageUnits", isUnitsLanguage);

            _menuPool = new ObjectPool();
            _mainMenu = new NativeMenu("TDUSCSpeedoV", "Version " + version + " by " + credits);
            _menuPool.Add(_mainMenu);

            // 2. Create the Checkbox Item (Boolean toggle)
            NativeCheckboxItem modEnabledHUD = new NativeCheckboxItem("Mod Enabled", "Mod Show and Hide HUD for Test Drive Unlimited Solar Crown Speedometer" , isMainHUDModEnabled);
            NativeCheckboxItem ABSToggle = new NativeCheckboxItem("ABS HUD", isABSHUD);
            NativeCheckboxItem TCSToggle = new NativeCheckboxItem("TCS HUD", isTCSHUD);

            var itemsListUnits = new List<string> { "Imperial", "Metric" };
            NativeListItem<string> UnitsListItem = new NativeListItem<string>("Units","Metric = KM/H\nImperial = MPH",itemsListUnits.ToArray());
            UnitsListItem.SelectedIndex = UseKMH; // ตั้งค่าตำแหน่งเริ่มต้น

            // --- สั่งโหลดไฟล์ .ytd เพียงก้อนเดียวไว้ล่วงหน้าตั้งแต่ม็อดเริ่มทำงาน ---
     //       Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, HUDBackground, false);


            var itemsListVersionHUD = new List<string> { "Demo", "Release", "Season 5 V1" , "Season 5 V2" };
            NativeListItem<string> VersionHUDListItem = new NativeListItem<string>("Version HUD","Select HUD",itemsListVersionHUD.ToArray());
            
            VersionHUDListItem.SelectedIndex = SelectVersionHUD;

            NativeItem selectDebugShowSubtitles = new NativeItem("Show Debug Name Cars");

            var itemsListLanguage = new List<string> { "Default", "Denmark / Norway", "Nederlands", "Russian", "Turkish", "Ukriane", "Vietnamese" };
            NativeListItem<string> LanguageListItem = new NativeListItem<string>("Language Units", "Select Language for KM/H or MPH", itemsListLanguage.ToArray());
            LanguageListItem.SelectedIndex = isUnitsLanguage;

            // 3. Add the checkbox to the menu
            _mainMenu.Add(modEnabledHUD);
            _mainMenu.Add(VersionHUDListItem);
            _mainMenu.Add(UnitsListItem);
            _mainMenu.Add(LanguageListItem);
            _mainMenu.Add(ABSToggle);
            _mainMenu.Add(TCSToggle);
            _mainMenu.Add(selectDebugShowSubtitles);


            VersionHUDListItem.ItemChanged += (sender, e) =>
            {
                if (VersionHUDListItem.SelectedIndex == 0)
                {
                    isDriftHUD = true;
                    config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 0);
                    config.Save();
                    HUDBackground = "!TDUSCDemoHUDBackground";
                    BackgroundTextureName = "background";
                    HUDABS = "ABS";
                    HUDTCS = "TCS";
                    HUDSpeed = "!TDUSCDemoHUDSpeed";
                    HUDGear = "!TDUSCDemoHUDGear";
                    HUDHandbrake = "Parking_Brake";
                    HUDTurnSignalLeft = "TurnLeft";
                    HUDTurnSignalRight = "TurnRight";
                    HUDRev = "!TDUSCDemoHUDRev";
                    headlightToggle = 0;
                    isHUDThrottleAndBrakeSeason5 = false;
                }
                if (VersionHUDListItem.SelectedIndex == 1)
                {
                    isDriftHUD = false;
                    config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 1);
                    config.Save();
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background";
                    HUDABS = "ABS";
                    HUDTCS = "TCS";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGear";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRev";
                    headlightToggle = 0;
                    isHUDThrottleAndBrakeSeason5 = false;
                }
                if (VersionHUDListItem.SelectedIndex == 2)
                {
                    isDriftHUD = false;
                    config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 2);
                    config.Save();
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background_Season5_V1";
                    HUDABS = "ABS_Season5";
                    HUDTCS = "TCS_Season5";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGearSeason5";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRevSeason5";
                    headlightToggle = 255;
                    isHUDThrottleAndBrakeSeason5 = false;
                }
                if (VersionHUDListItem.SelectedIndex == 3)
                {
                    isDriftHUD = false;
                    config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 3);
                    config.Save();
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background_Season5_V2";
                    HUDABS = "ABS_Season5";
                    HUDTCS = "TCS_Season5";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGearSeason5";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRev";
                    headlightToggle = 255;
                    isHUDThrottleAndBrakeSeason5 = true;
                }
            };
            UnitsListItem.ItemChanged += (sender, e) =>
            {
                int currentIndex = UnitsListItem.SelectedIndex;

                activeUseKMH = ImageUseKMHSettings[currentIndex];

                UseKMH = currentIndex;
                switch(currentIndex)
                {
                    case 1:
                        isSwitchUnits = speedKMH;
                        isKMH = true;
                        isMPH = false;
                        config.SetValue<int>("Units_Settings", "UseKMH", 1);
                        config.Save();
                        break;
                    case 0:
                        isSwitchUnits = speedMPH;
                        isKMH = false;
                        isMPH = true;
                        config.SetValue<int>("Units_Settings", "UseKMH", 0);
                        config.Save();
                        break;
                }

            };

            LanguageListItem.ItemChanged += (sender, e) =>
            {
                int currentIndex = LanguageListItem.SelectedIndex;
                isUnitsLanguage = LanguageListItem.SelectedIndex;


                // อัปเดตเปลี่ยนเฉพาะค่า "Texture Name" ไปตามลำดับที่เลือก
                activeTextureKMHLanguageName = ImageKMHLanguage[currentIndex];
                ImageKMH.Texture = activeTextureKMHLanguageName;
                
                activeTextureMPHLanguageName = ImageMPHLanguage[currentIndex];
                ImageMPH.Texture = activeTextureMPHLanguageName;
                
                switch (currentIndex)
                {
                    case 0:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 0);
                        config.Save();
                        break;
                    case 1:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 1);
                        config.Save();
                        break;
                    case 2:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 2);
                        config.Save();
                        break;
                    case 3:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 3);
                        config.Save();
                        break;
                    case 4:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 4);
                        config.Save();
                        break;
                    case 5:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 5);
                        config.Save();
                        break;
                    case 6:
                        config.SetValue<int>("Units_Settings", "LanguageUnits", 6);
                        config.Save();
                        break;
                }
            };
            modEnabledHUD.CheckboxChanged += (sender, e) =>
            {
                bool Enabled = modEnabledHUD.Checked;
                if (Enabled)
                {
                    isMainHUDModEnabled = true;
                    config.SetValue<bool>("HUD_SETTINGS", "Mod_Enabled", true);
                    config.Save();
                }
                else
                {
                    isMainHUDModEnabled = false;
                    config.SetValue<bool>("HUD_SETTINGS", "Mod_Enabled", false);
                    config.Save();
                }
            };
            ABSToggle.CheckboxChanged += (sender, e) =>
            {
                bool Enabled = ABSToggle.Checked;
                if(Enabled)
                {
                    isABSHUD = true;
                    config.SetValue<bool>("HUD_SETTINGS", "ABSHUD", true);
                    config.Save();
                }
                else
                {
                    isABSHUD = false;
                    config.SetValue<bool>("HUD_SETTINGS", "ABSHUD", false);
                    config.Save();
                }
            };
            TCSToggle.CheckboxChanged += (sender, e) =>
            {
                bool Enabled = TCSToggle.Checked;
                if(Enabled)
                {
                    isTCSHUD = true;
                    config.SetValue<bool>("HUD_SETTINGS", "TCSHUD", true);
                    config.Save();
                }
                else
                {
                    isTCSHUD = false;
                    config.SetValue<bool>("HUD_SETTINGS", "TCSHUD", false);
                    config.Save();
                }
            };
            selectDebugShowSubtitles.Activated += (sender, e) =>
            {
                Ped playerPed = Game.Player.Character;

                // 1. ตรวจสอบก่อนว่าตัวละครผู้เล่นมีตัวตนอยู่ในเกมจริงไหม
                if (playerPed.Exists())
                {
                    GTA.UI.Screen.ShowSubtitle("Vehicle Exited");
                    // 2. ตรวจสอบว่าผู้เล่นนั่งอยู่ "ในรถ" เรียบร้อยแล้ว
                    if (playerPed.IsInVehicle())
                    {

                        Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;
                        // 1. ดึงชื่อ Spawn Name แท้ๆ (ตัวพิมพ์เล็ก) ที่เราใช้เสกรถ (เช่น "comet2", "one") จากคลาส Model โดยตรง
                        string modelName = currentVehicle.Model.ToString().Replace(" ", "").ToLower();

                        // 2. ดักทางเผื่อเหนียว: หาก SHVDN3 ดึงไม่ออกแล้วพ่นเลขฐานสิบหก 0x ออกมาแทน ให้สลับไปใช้ Native API
                        if (modelName.StartsWith("0x"))
                        {
                            modelName = Function.Call<string>((Hash)0xB215AAC32D25D019, currentVehicle.Model.Hash).Replace(" ", "").ToLower();
                        }

                        // 3. ค้นหาใน Dictionary เพื่อดึงคลาส CarData ออกมา
                        if (carNamesDict.TryGetValue(modelName, out CarData car))
                        {
                            GTA.UI.Screen.ShowSubtitle($"Vehicle Name: {car.DisplayName}\nVehicle Spawn : {modelName}\nMax RPM : {car.DisplayMaxRPM}", 3000);
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle($"Vehicle Name: Not Found in {configPath}\nVehicle Spawn : {modelName}\nMax RPM : {SetRPM}", 3000);
                        }
                    }
                    else // ❌ เงื่อนไข "else" ตรงนี้จะทำงานเมื่อผู้เล่นเท้าแตะพื้น (ไม่ได้อยู่ในรถ)
                    {
                        // รีเซ็ตค่า Handle เพื่อให้เวลาขึ้นรถคันเดิมซ้ำ ระบบยังโชว์ชื่อรถให้ใหม่
                        lastVehicleHandle = 0;
                    }
                }
            };

            Tick += OnTick;
            KeyUp += OnKeyDown;

            // ระบบอ่านโหลดเซฟ     
            switch (SelectVersionHUD)
            {
                case 0:
                    isDriftHUD = true;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 0);
                    break;
                case 1:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 1);
                    break;
                case 2:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 2);
                    break;
                case 3:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 3);
                    break;
            }

            switch (UseKMH)
            {
                case 1:
                    isSwitchUnits = speedKMH;
                    isKMH = true;
                    isMPH = false;
                    config.GetValue<int>("Units_Settings", "UseKMH", 1);
                    break;

                case 0:
                    isSwitchUnits = speedMPH;
                    isKMH = false;
                    isMPH = true;
                    config.GetValue<int>("Units_Settings", "UseKMH", 0);
                    break;
            }
            
            switch (isABSHUD)
            {
                case true:
                    isABSHUD = true;
                    break;

                case false:
                    isABSHUD = false;
                    break;
            }

            switch (isTCSHUD)
            {
                case true:
                    isTCSHUD = true;
                    break;

                case false:
                    isTCSHUD = false;
                    break;
            }


            switch (SelectVersionHUD)
            {
                case 0:
                    isDriftHUD = true;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 0);
                    HUDBackground = "!TDUSCDemoHUDBackground";
                    BackgroundTextureName = "background";
                    HUDABS = "ABS";
                    HUDTCS = "TCS";
                    HUDSpeed = "!TDUSCDemoHUDSpeed";
                    HUDGear = "!TDUSCDemoHUDGear";
                    HUDHandbrake = "Parking_Brake";
                    HUDTurnSignalLeft = "TurnLeft";
                    HUDTurnSignalRight = "TurnRight";
                    HUDRev = "!TDUSCDemoHUDRev";
                    headlightToggle = 0;
                    isHUDThrottleAndBrakeSeason5 = false;
                    break;
                case 1:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 1);
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background";
                    HUDABS = "ABS";
                    HUDTCS = "TCS";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGear";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRev";
                    headlightToggle = 0;
                    isHUDThrottleAndBrakeSeason5 = false;
                    break;
                case 2:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 2);
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background_Season5_V1";
                    HUDABS = "ABS_Season5";
                    HUDTCS = "TCS_Season5";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGearSeason5";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRevSeason5";
                    headlightToggle = 255;
                    isHUDThrottleAndBrakeSeason5 = false;
                    break;
                case 3:
                    isDriftHUD = false;
                    config.GetValue<int>("HUD_SETTINGS", "Version_HUD", 3);
                    HUDBackground = "!TDUSCReleaseHUDBackground";
                    BackgroundTextureName = "background_Season5_V2";
                    HUDABS = "ABS_Season5";
                    HUDTCS = "TCS_Season5";
                    HUDSpeed = "!TDUSCReleaseHUDSpeed";
                    HUDGear = "!TDUSCReleaseHUDGearSeason5";
                    HUDHandbrake = "handbrake";
                    HUDTurnSignalLeft = "turnleft";
                    HUDTurnSignalRight = "turnright";
                    HUDRev = "!TDUSCReleaseHUDRev";
                    headlightToggle = 255;
                    isHUDThrottleAndBrakeSeason5 = true;
                    break;
            }
            
            switch (isUnitsLanguage)
            {
                case 0:
                    activeTextureKMHLanguageName = "kmh";
                    activeTextureMPHLanguageName = "mph";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 0);
                    break;
                case 1:
                    activeTextureKMHLanguageName = "kmh_DenmarkNorway";
                    activeTextureMPHLanguageName = "mph";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 1);
                    break;
                case 2:
                    activeTextureKMHLanguageName = "kmh_Nederlands";
                    activeTextureMPHLanguageName = "mph_Nederlands";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 2);
                    break;
                case 3:
                    activeTextureKMHLanguageName = "kmh_Russian";
                    activeTextureMPHLanguageName = "mph_Russian";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 3);
                    break;
                case 4:
                    activeTextureKMHLanguageName = "kmh_Turkish";
                    activeTextureMPHLanguageName = "mph_Turkish";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 4);
                    break;
                case 5:
                    activeTextureKMHLanguageName = "kmh_Ukriane";
                    activeTextureMPHLanguageName = "mph_Ukriane";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 5);
                    break;
                case 6:
                    activeTextureKMHLanguageName = "kmh_Vietnam";
                    activeTextureMPHLanguageName = "mph_Vietnam";
                    config.GetValue<int>("Units_Settings", "LanguageUnits", 6);
                    break;
            }

            // Demo HUD
            BackgroundTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, BackgroundTextureName);
            BackgroundTextureImage2 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, BackgroundTextureName2);

            ImageKMH = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, activeTextureKMHLanguageName);
            
            ImageMPH = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, activeTextureMPHLanguageName);

            DriftTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, DriftTextureName);

            HandbrakeTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, HandbrakeTextureName);

            speedTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDSpeed, "speed_0000");

            RevTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDRev, "rev_0000");


            LabelRPMDeafult = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPMDefaultName);
            
            LabelRPM_2000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_2000_Name);
            LabelRPM_2200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_2200_Name);
            LabelRPM_2400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_2400_Name);
            LabelRPM_2600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_2600_Name);
            LabelRPM_2800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_2800_Name);
            LabelRPM_3000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_3000_Name);
            LabelRPM_4000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_4000_Name);
            LabelRPM_4500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_4500_Name);
            LabelRPM_5000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_5000_Name);
            LabelRPM_5200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_5200_Name);
            LabelRPM_5400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_5400_Name);
            LabelRPM_5600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_5600_Name);
            LabelRPM_5800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_5800_Name);
            LabelRPM_6000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_6000_Name);
            LabelRPM_6200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_6200_Name);
            LabelRPM_6400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_6400_Name);
            LabelRPM_6600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_6600_Name);
            LabelRPM_6800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_6800_Name);
            LabelRPM_7000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_7000_Name);
            LabelRPM_7200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_7200_Name);
            LabelRPM_7400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_7400_Name);
            LabelRPM_7600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_7600_Name);
            LabelRPM_7800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_7800_Name);
            LabelRPM_8000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_8000_Name);
            LabelRPM_8200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_8200_Name);
            LabelRPM_8400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_8400_Name);
            LabelRPM_8600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_8600_Name);
            LabelRPM_8800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_8800_Name);
            LabelRPM_9000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_9000_Name);
            LabelRPM_9200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_9200_Name);
            LabelRPM_9400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_9400_Name);
            LabelRPM_9600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_9600_Name);
            LabelRPM_9800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_9800_Name);
            LabelRPM_10000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_10000_Name);
            LabelRPM_10500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_10500_Name);
            LabelRPM_11000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_11000_Name);
            LabelRPM_11500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_11500_Name);
            LabelRPM_12000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_12000_Name);
            LabelRPM_12500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_12500_Name);
            LabelRPM_13000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_13000_Name);
            LabelRPM_13500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_13500_Name);
            LabelRPM_14000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_14000_Name);
            LabelRPM_14500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_14500_Name);
            LabelRPM_15000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_15000_Name);
            LabelRPM_15500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_15500_Name);
            LabelRPM_16000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_16000_Name);
            LabelRPM_16500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_16500_Name);
            LabelRPM_17000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_17000_Name);
            LabelRPM_18000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_18000_Name);
            LabelRPM_19000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, LabelRPM_19000_Name);
            

            ABSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, HUDABS);

            TCSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, HUDTCS);

            TurnSignalLeft = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, HUDTurnSignalLeft);

            TurnSignalRight = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDBackground, HUDTurnSignalRight);

            RevEngineOffImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDRev, "rev_0000");

            gearTexture = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDGear, HUDGear);
            gearTexture_Red = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDGear, HUDGear);

            HeadlightsImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), "!TDUSCReleaseHUDBackground", "Lighting");
            ThrottleImageHUD = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), "!TDUSCReleaseHUDPedalAccelerationSeason5V2", "_000");
            BrakeImageHUD = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), "!TDUSCReleaseHUDPedalBrakeSeason5V2", "_000");

        }
      //  private bool categorySpeedoEnabled;

        private string categoryName;

        private void OnTick(object sender, EventArgs e)
        {

            VehicleEnterExit();
            _menuPool.Process();
            TDUSC_HUDTick();
            VehicleSlowMotionFranklin();

            BackgroundTextureImage.Dictionary = HUDBackground;
            BackgroundTextureImage.Texture = BackgroundTextureName;
            BackgroundTextureImage2.Dictionary = HUDBackground;

            ImageKMH.Dictionary = HUDBackground;
            ImageMPH.Dictionary = HUDBackground;
            ABSTextureImage.Texture = HUDABS;
            TCSTextureImage.Texture = HUDTCS;
            HandbrakeTextureImage.Dictionary = HUDBackground;
            HandbrakeTextureImage.Texture = HUDHandbrake;
            DriftTextureImage.Dictionary = HUDBackground;
            TurnSignalLeft.Texture = HUDTurnSignalLeft;
            TurnSignalRight.Texture = HUDTurnSignalRight;
            TurnSignalLeft.Dictionary = HUDBackground;
            TurnSignalRight.Dictionary = HUDBackground;
            speedTextureImage.Dictionary = HUDSpeed;
            gearTexture.Dictionary = HUDGear;
            gearTexture_Red.Dictionary = HUDGear;
            RevTextureImage.Dictionary = HUDRev;

            LabelRPMDeafult.Dictionary = HUDBackground;
            LabelRPM_2000.Dictionary = HUDBackground;          
            LabelRPM_2200.Dictionary = HUDBackground;          
            LabelRPM_2400.Dictionary = HUDBackground;          
            LabelRPM_2600.Dictionary = HUDBackground;          
            LabelRPM_2800.Dictionary = HUDBackground;          
            LabelRPM_3000.Dictionary = HUDBackground;          
            LabelRPM_4000.Dictionary = HUDBackground;          
            LabelRPM_4500.Dictionary = HUDBackground;          
            LabelRPM_5000.Dictionary = HUDBackground;          
            LabelRPM_5200.Dictionary = HUDBackground;
            LabelRPM_5400.Dictionary = HUDBackground;
            LabelRPM_5600.Dictionary = HUDBackground;
            LabelRPM_5800.Dictionary = HUDBackground;
            LabelRPM_6000.Dictionary = HUDBackground;
            LabelRPM_6200.Dictionary = HUDBackground;          
            LabelRPM_6400.Dictionary = HUDBackground;          
            LabelRPM_6600.Dictionary = HUDBackground;          
            LabelRPM_6800.Dictionary = HUDBackground;
            LabelRPM_7000.Dictionary = HUDBackground;
            LabelRPM_7200.Dictionary = HUDBackground;
            LabelRPM_7400.Dictionary = HUDBackground;
            LabelRPM_7600.Dictionary = HUDBackground;
            LabelRPM_7800.Dictionary = HUDBackground;
            LabelRPM_8000.Dictionary = HUDBackground;
            LabelRPM_8200.Dictionary = HUDBackground;
            LabelRPM_8400.Dictionary = HUDBackground;
            LabelRPM_8600.Dictionary = HUDBackground;
            LabelRPM_8800.Dictionary = HUDBackground;
            LabelRPM_9000.Dictionary = HUDBackground;
            LabelRPM_9200.Dictionary = HUDBackground;
            LabelRPM_9400.Dictionary = HUDBackground;
            LabelRPM_9600.Dictionary = HUDBackground;
            LabelRPM_9800.Dictionary = HUDBackground;
            LabelRPM_10000.Dictionary = HUDBackground;
            LabelRPM_10500.Dictionary = HUDBackground;
            LabelRPM_11000.Dictionary = HUDBackground;
            LabelRPM_11500.Dictionary = HUDBackground;
            LabelRPM_12000.Dictionary = HUDBackground;
            LabelRPM_12500.Dictionary = HUDBackground;
            LabelRPM_13000.Dictionary = HUDBackground;
            LabelRPM_13500.Dictionary = HUDBackground;
            LabelRPM_14000.Dictionary = HUDBackground;
            LabelRPM_14500.Dictionary = HUDBackground;
            LabelRPM_15000.Dictionary = HUDBackground;
            LabelRPM_15500.Dictionary = HUDBackground;
            LabelRPM_16000.Dictionary = HUDBackground;
            LabelRPM_16500.Dictionary = HUDBackground;
            LabelRPM_17000.Dictionary = HUDBackground;
            LabelRPM_18000.Dictionary = HUDBackground;
            LabelRPM_19000.Dictionary = HUDBackground;

            ABSTextureImage.Dictionary = HUDBackground;
            TCSTextureImage.Dictionary = HUDBackground;
            RevEngineOffImage.Dictionary = HUDRev;

            HeadlightsImage.Color = Color.FromArgb(headlightToggle, 255, 255, 255);

            float throttleInput = Game.GetControlValueNormalized(GTA.Control.VehicleAccelerate);
            float brakeInput = Game.GetControlValueNormalized(GTA.Control.VehicleBrake);
            int currentThrottle = (int)Math.Round(throttleInput * 99);
            int currentBrake = (int)Math.Round(brakeInput * 99);

            string textureGasName = "GasPedal_" + currentThrottle.ToString("000");
            ThrottleImageHUD.Texture = textureGasName;

            string textureBrakeName = "BrakePedal_" + currentBrake.ToString("000");
            BrakeImageHUD.Texture = textureBrakeName;

            switch (categoryName)
            {
                case "Compacts":
                    statusHUD = true;
                    break;
                case "Sedans":
                    statusHUD = true;
                    break;
                case "SUV":
                    statusHUD = true;
                    break;
                case "Coupes":
                    statusHUD = true;
                    break;
                case "Muscle":
                    statusHUD = true;
                    break;
                case "SportsClassics":
                    statusHUD = true;
                    break;
                case "Sports":
                    statusHUD = true;
                    break;
                case "Super":
                    statusHUD = true;
                    break;
                case "Motorcycles":
                    statusHUD = true;
                    break;
                case "OffRoad":
                    statusHUD = true;
                    break;
                case "Industrial":
                    statusHUD = true;
                    break;
                case "Utility":
                    statusHUD = false;
                    break;
                case "Vans":
                    statusHUD = true;
                    break;
                case "Cycles":
                    statusHUD = false;
                    break;
                case "Boats":
                    statusHUD = false;
                    break;
                case "Helicopters":
                    statusHUD = false;
                    break;
                case "Planes":
                    statusHUD = false;
                    break;
                case "Service":
                    statusHUD = true;
                    break;
                case "Emergency":
                    statusHUD = true;
                    break;
                case "Military":
                    statusHUD = true;
                    break;
                case "Commercial":
                    statusHUD = true;
                    break;
                case "OpenWheel":
                    statusHUD = true;
                    break;
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyOpenMenu)
            {
                _mainMenu.Visible = !_mainMenu.Visible;
            }
        }

        // TDU Solar Crown Speedometer
        private bool isMainHUDModEnabled;
        private bool isHideHUDForOpenPhoneorCharacterWheel;
        private bool isRevEngineCurrent;
        private bool isABSHUD;
        private bool isTCSHUD;
        private bool statusHUD;

        private bool headlightsSeason5;
        private void TDUSC_HUDTick()
        {
            if (!isMainHUDModEnabled)
            {

            }
            else
            {
                // Speedometer Base
                Ped playerPed = Game.Player.Character;
                Player player = Game.Player;
                bool playerSwitchInProgress = Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS);


                if (statusHUD && playerPed != null && playerPed.IsInVehicle())
                {
                    Vehicle vehicleStatus = playerPed.CurrentVehicle;

                    // Turn on headlights automatically if desired
                    if (vehicleStatus.AreHighBeamsOn)
                    {
                        headlightsSeason5 = true;
                    }
                    else if (vehicleStatus.AreLightsOn)
                    {
                        headlightsSeason5 = true;
                    }
                    else
                    {
                        headlightsSeason5 = false;
                    }

                    // ส่วนนี้จะทำงานทุกๆ 500 มิลลิวินาที (ไม่บล็อกการเช็คค่า bool ด้านบน)
                    if (vehicleStatus.IsLeftIndicatorLightOn)
                    {
                        // ใช้เฉพาะ Game.GameTime หารเอาเศษ เพื่อทำไฟกระพริบสลับทุกๆ 0.5 วินาที
                        if ((Game.GameTime % 1000) < 500)
                        {
                            TurnSignalLeft_Enabled = true;
                        }
                        else
                        {
                            TurnSignalLeft_Enabled = false;
                        }
                    }
                    else
                    {
                        TurnSignalLeft_Enabled = false;
                    }


                    if (vehicleStatus.IsRightIndicatorLightOn)
                    {
                        // ใช้เฉพาะ Game.GameTime หารเอาเศษ เพื่อทำไฟกระพริบสลับทุกๆ 0.5 วินาที
                        if ((Game.GameTime % 1000) < 500)
                        {
                            TurnSignalRight_Enabled = true;
                        }
                        else
                        {
                            TurnSignalRight_Enabled = false;
                        }
                    }
                    else
                    {
                        TurnSignalRight_Enabled = false;
                    }


                    if (isHideHUDForOpenPhoneorCharacterWheel)
                    {
                        Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;
                        // 2. ดึงประเภทของรถ (Vehicle Class) และแปลงเป็นภาษาไทย
                        VehicleClass vehicleClass = currentVehicle.ClassType;
                        string category = GetVehicleClassName(vehicleClass);

                        categoryName = category;

                        // 3. ดึงชื่อรุ่นรถ (รองรับทั้งรถเดิมและรถ Add-on)
                        string displayName = currentVehicle.LocalizedName;
                        if (string.IsNullOrEmpty(displayName) || displayName == "NULL")
                        {
                            displayName = currentVehicle.Model.ToString(); // ใช้ชื่อโมเดลเสกถ้าระบบหาชื่อแปลไม่เจอ 
                        }

                        // 1. ดึงชื่อ Spawn Name แท้ๆ (ตัวพิมพ์เล็ก) ที่เราใช้เสกรถ (เช่น "comet2", "one") จากคลาส Model โดยตรง
                        string modelName = currentVehicle.Model.ToString().Replace(" ", "").ToLower();

                        // 2. ดักทางเผื่อเหนียว: หาก SHVDN3 ดึงไม่ออกแล้วพ่นเลขฐานสิบหก 0x ออกมาแทน ให้สลับไปใช้ Native API
                        if (modelName.StartsWith("0x"))
                        {
                            modelName = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, currentVehicle.Model.Hash).Replace(" ", "").ToLower();
                        }

                        // 3. ค้นหาใน Dictionary เพื่อดึงคลาส CarData ออกมา
                        if (carNamesDict.TryGetValue(modelName, out CarData car))
                        {
                            SetRPM = car.DisplayMaxRPM;
                        }
                        else
                        {
                            SetRPM = 9000;
                        }

                        int add50 = 50;

                        // Set Display REV RPM
                        // Max 3k
                        if (SetRPM >= rpm_2000 && SetRPM < rpm_2200)
                        {
                            // 1202
                            rpmMin = 604;
                            int rpmMaxDefault = 1202;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_2200 && SetRPM < rpm_2400)
                        {
                            // 1323
                            rpmMin = 604;
                            int rpmMaxDefault = 1323;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_2400 && SetRPM < rpm_2600)
                        {
                            // 1447
                            rpmMin = 604;
                            int rpmMaxDefault = 1447;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_2600 && SetRPM < rpm_2800)
                        {
                            // 1566
                            rpmMin = 604;
                            int rpmMaxDefault = 604;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_2800 && SetRPM < rpm_3000)
                        {
                            // 1688
                            rpmMin = 604;
                            int rpmMaxDefault = 1688;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 4k
                        else if (SetRPM >= rpm_3000 && SetRPM < rpm_4000)
                        {
                            // 1351
                            rpmMin = 453;
                            int rpmMaxDefault = 1351;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 6k
                        else if (SetRPM >= rpm_4000 && SetRPM < rpm_4500)
                        {
                            // 1201
                            rpmMin = 304;
                            int rpmMaxDefault = 1201;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_4500 && SetRPM < rpm_5000)
                        {
                            // 1358
                            rpmMin = 304;
                            int rpmMaxDefault = 1358;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_5000 && SetRPM < rpm_5200)
                        {
                            // 1499
                            rpmMin = 304;
                            int rpmMaxDefault = 1499;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_5200 && SetRPM < rpm_5400)
                        {
                            // 1567
                            rpmMin = 304;
                            int rpmMaxDefault = 1567;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_5400 && SetRPM < rpm_5600)
                        {
                            // 1626
                            rpmMin = 304;
                            int rpmMaxDefault = 1626;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 7k
                        else if (SetRPM >= rpm_5600 && SetRPM < rpm_5800)
                        {
                            // 1448
                            rpmMin = 264;
                            int rpmMaxDefault = 1448;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_5800 && SetRPM < rpm_6000)
                        {
                            // 1498
                            rpmMin = 264;
                            int rpmMaxDefault = 1498;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_6000 && SetRPM < rpm_6200)
                        {
                            // 1547
                            rpmMin = 264;
                            int rpmMaxDefault = 1547;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_6200 && SetRPM < rpm_6400)
                        {
                            // 1596
                            rpmMin = 264;
                            int rpmMaxDefault = 1596;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 8k
                        else if (SetRPM >= rpm_6400 && SetRPM < rpm_6600)
                        {
                            // 1448
                            rpmMin = 230;
                            int rpmMaxDefault = 1448;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_6600 && SetRPM < rpm_6800)
                        {
                            // 1495
                            rpmMin = 230;
                            int rpmMaxDefault = 1495;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_6800 && SetRPM < rpm_7000)
                        {
                            // 1536
                            rpmMin = 230;
                            int rpmMaxDefault = 1536;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_7000 && SetRPM < rpm_7200)
                        {
                            // 1579
                            rpmMin = 230;
                            int rpmMaxDefault = 1579;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_7200 && SetRPM < rpm_7400)
                        {
                            // 1625
                            rpmMin = 230;
                            int rpmMaxDefault = 1625;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 9k
                        else if (SetRPM >= rpm_7400 && SetRPM < rpm_7600)
                        {
                            // 1485
                            rpmMin = 207;
                            int rpmMaxDefault = 1485;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_7600 && SetRPM < rpm_7800)
                        {
                            // 1524
                            rpmMin = 207;
                            int rpmMaxDefault = 1524;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_7800 && SetRPM < rpm_8000)
                        {
                            // 1566
                            rpmMin = 207;
                            int rpmMaxDefault = 1566;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_8000 && SetRPM < rpm_8200)
                        {
                            // 1620
                            rpmMin = 207;
                            int rpmMaxDefault = 1620;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 10k
                        else if (SetRPM >= rpm_8200 && SetRPM < rpm_8400)
                        {
                            // 1483
                            rpmMin = 185;
                            int rpmMaxDefault = 1483;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_8400 && SetRPM < rpm_8600)
                        {
                            // 1516
                            rpmMin = 185;
                            int rpmMaxDefault = 1516;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_8600 && SetRPM < rpm_8800)
                        {
                            // 1555
                            rpmMin = 185;
                            int rpmMaxDefault = 1555;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_8800 && SetRPM < rpm_9000)
                        {
                            // 1591
                            rpmMin = 185;
                            int rpmMaxDefault = 1591;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_9000 && SetRPM < rpm_9200)
                        {
                            // 1629
                            rpmMin = 185;
                            int rpmMaxDefault = 1629;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 11k
                        else if (SetRPM >= rpm_9200 && SetRPM < rpm_9400)
                        {
                            // 1512
                            rpmMin = 170;
                            int rpmMaxDefault = 1512;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_9400 && SetRPM < rpm_9600)
                        {
                            // 1544
                            rpmMin = 170;
                            int rpmMaxDefault = 1544;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_9600 && SetRPM < rpm_9800)
                        {
                            // 1578
                            rpmMin = 170;
                            int rpmMaxDefault = 1578;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_9800 && SetRPM < rpm_10000)
                        {
                            // 1612
                            rpmMin = 170;
                            int rpmMaxDefault = 1612;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 12k
                        else if (SetRPM >= rpm_10000 && SetRPM < rpm_10500)
                        {
                            // 1503
                            rpmMin = 153;
                            int rpmMaxDefault = 1503;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_10500 && SetRPM < rpm_11000)
                        {
                            // 1581
                            rpmMin = 153;
                            int rpmMaxDefault = 1581;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_11000 && SetRPM < rpm_11500)
                        {
                            // 1651
                            rpmMin = 153;
                            int rpmMaxDefault = 1651;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 14k
                        else if (SetRPM >= rpm_11500 && SetRPM < rpm_12000)
                        {
                            // 1486
                            rpmMin = 0;
                            int rpmMaxDefault = 1486;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_12000 && SetRPM < rpm_12500)
                        {
                            // 1548
                            rpmMin = 0;
                            int rpmMaxDefault = 1548;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_12500 && SetRPM < rpm_13000)
                        {
                            // 1612
                            rpmMin = 0;
                            int rpmMaxDefault = 1612;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 16k
                        else if (SetRPM >= rpm_13000 && SetRPM < rpm_13500)
                        {
                            // 1466
                            rpmMin = 0;
                            int rpmMaxDefault = 1466;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_13500 && SetRPM < rpm_14000)
                        {
                            // 1522
                            rpmMin = 0;
                            int rpmMaxDefault = 1522;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_14000 && SetRPM < rpm_14500)
                        {
                            // 1581
                            rpmMin = 0;
                            int rpmMaxDefault = 1581;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_14500 && SetRPM < rpm_15000)
                        {
                            // 1637
                            rpmMin = 0;
                            int rpmMaxDefault = 1637;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 18k
                        else if (SetRPM >= rpm_15000 && SetRPM < rpm_15500)
                        {
                            // 1498
                            rpmMin = 0;
                            int rpmMaxDefault = 1498;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_15500 && SetRPM < rpm_16000)
                        {
                            // 1555
                            rpmMin = 0;
                            int rpmMaxDefault = 1555;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_16000 && SetRPM < rpm_16500)
                        {
                            // 1602
                            rpmMin = 0;
                            int rpmMaxDefault = 1602;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        // Max 20k
                        else if (SetRPM >= rpm_16500 && SetRPM < rpm_17000)
                        {
                            // 1487
                            rpmMin = 0;
                            int rpmMaxDefault = 1487;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_17000 && SetRPM < rpm_18000)
                        {
                            // 1535
                            rpmMin = 0;
                            int rpmMaxDefault = 1535;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_18000 && SetRPM < rpm_19000)
                        {
                            // 1613
                            rpmMin = 0;
                            int rpmMaxDefault = 1613;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }
                        else if (SetRPM >= rpm_19000 && SetRPM < 20000)
                        {
                            // 1717
                            rpmMin = 0;
                            int rpmMaxDefault = 1717;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }

                        }
                        else
                        {
                            // 1629
                            rpmMin = 185;
                            int rpmMaxDefault = 1629;
                            rpmMax = rpmMaxDefault + add50;

                            if (currentRPM >= rpmMaxDefault)
                            {
                                gearhudcolour_default = false;
                                gearhudcolour_red = true;
                            }
                            else
                            {
                                gearhudcolour_default = true;
                                gearhudcolour_red = false;
                            }
                        }

                        //   GTA.UI.Screen.ShowSubtitle($"{currentRPM}");
                        // Ensure the player is actually in a vehicle
                        if (currentVehicle != null && currentVehicle.Exists())
                        {
                            // Outline
                            BackgroundTextureImage2.Draw();
                            BackgroundTextureImage.Draw();

                            if (isKMH)
                            {
                                ImageKMH.Draw();
                            }

                            if (isMPH)
                            {
                                ImageMPH.Draw();
                            }


                            speedTextureImage.Draw();


                            if (SetRPM >= rpm_2000 && SetRPM < rpm_2200)
                            {
                                LabelRPM_2000.Draw();
                            }
                            else if (SetRPM >= rpm_2200 && SetRPM < rpm_2400)
                            {
                                LabelRPM_2200.Draw();
                            }
                            else if (SetRPM >= rpm_2400 && SetRPM < rpm_2600)
                            {
                                LabelRPM_2400.Draw();
                            }
                            else if (SetRPM >= rpm_2600 && SetRPM < rpm_2800)
                            {
                                LabelRPM_2600.Draw();
                            }
                            else if (SetRPM >= rpm_2800 && SetRPM < rpm_3000)
                            {
                                LabelRPM_2800.Draw();
                            }
                            else if (SetRPM >= rpm_3000 && SetRPM < rpm_4000)
                            {
                                LabelRPM_3000.Draw();
                            }
                            else if (SetRPM >= rpm_4000 && SetRPM < rpm_4500)
                            {
                                LabelRPM_4000.Draw();
                            }
                            else if (SetRPM >= rpm_4500 && SetRPM < rpm_5000)
                            {
                                LabelRPM_4500.Draw();
                            }
                            else if (SetRPM >= rpm_5000 && SetRPM < rpm_5200)
                            {
                                LabelRPM_5000.Draw();
                            }
                            else if (SetRPM >= rpm_5200 && SetRPM < rpm_5400)
                            {
                                LabelRPM_5200.Draw();
                            }
                            else if (SetRPM >= rpm_5400 && SetRPM < rpm_5600)
                            {
                                LabelRPM_5400.Draw();
                            }
                            else if (SetRPM >= rpm_5600 && SetRPM < rpm_5800)
                            {
                                LabelRPM_5600.Draw();
                            }
                            else if (SetRPM >= rpm_5800 && SetRPM < rpm_6000)
                            {
                                LabelRPM_5800.Draw();
                            }
                            else if (SetRPM >= rpm_6000 && SetRPM < rpm_6200)
                            {
                                LabelRPM_6000.Draw();
                            }
                            else if (SetRPM >= rpm_6200 && SetRPM < rpm_6400)
                            {
                                LabelRPM_6200.Draw();
                            }
                            else if (SetRPM >= rpm_6400 && SetRPM < rpm_6600)
                            {
                                LabelRPM_6400.Draw();
                            }
                            else if (SetRPM >= rpm_6600 && SetRPM < rpm_6800)
                            {
                                LabelRPM_6600.Draw();
                            }
                            else if (SetRPM >= rpm_6800 && SetRPM < rpm_7000)
                            {
                                LabelRPM_6800.Draw();
                            }
                            else if (SetRPM >= rpm_7000 && SetRPM < rpm_7200)
                            {
                                LabelRPM_7000.Draw();
                            }
                            else if (SetRPM >= rpm_7200 && SetRPM < rpm_7400)
                            {
                                LabelRPM_7200.Draw();
                            }
                            else if (SetRPM >= rpm_7400 && SetRPM < rpm_7600)
                            {
                                LabelRPM_7400.Draw();
                            }
                            else if (SetRPM >= rpm_7600 && SetRPM < rpm_7800)
                            {
                                LabelRPM_7600.Draw();
                            }
                            else if (SetRPM >= rpm_7800 && SetRPM < rpm_8000)
                            {
                                LabelRPM_7800.Draw();
                            }
                            else if (SetRPM >= rpm_8000 && SetRPM < rpm_8200)
                            {
                                LabelRPM_8000.Draw();
                            }
                            else if (SetRPM >= rpm_8200 && SetRPM < rpm_8400)
                            {
                                LabelRPM_8200.Draw();
                            }
                            else if (SetRPM >= rpm_8400 && SetRPM < rpm_8600)
                            {
                                LabelRPM_8400.Draw();
                            }
                            else if (SetRPM >= rpm_8600 && SetRPM < rpm_8800)
                            {
                                LabelRPM_8600.Draw();
                            }
                            else if (SetRPM >= rpm_8800 && SetRPM < rpm_9000)
                            {
                                LabelRPM_8800.Draw();
                            }
                            else if (SetRPM >= rpm_9000 && SetRPM < rpm_9200)
                            {
                                LabelRPM_9000.Draw();
                            }
                            else if (SetRPM >= rpm_9200 && SetRPM < rpm_9400)
                            {
                                LabelRPM_9200.Draw();
                            }
                            else if (SetRPM >= rpm_9400 && SetRPM < rpm_9600)
                            {
                                LabelRPM_9400.Draw();
                            }
                            else if (SetRPM >= rpm_9600 && SetRPM < rpm_9800)
                            {
                                LabelRPM_9600.Draw();
                            }
                            else if (SetRPM >= rpm_9800 && SetRPM < rpm_10000)
                            {
                                LabelRPM_9800.Draw();
                            }
                            else if (SetRPM >= rpm_10000 && SetRPM < rpm_10500)
                            {
                                LabelRPM_10000.Draw();
                            }
                            else if (SetRPM >= rpm_10500 && SetRPM < rpm_11000)
                            {
                                LabelRPM_10500.Draw();
                            }
                            else if (SetRPM >= rpm_11000 && SetRPM < rpm_11500)
                            {
                                LabelRPM_11000.Draw();
                            }
                            else if (SetRPM >= rpm_11500 && SetRPM < rpm_12000)
                            {
                                LabelRPM_11500.Draw();
                            }
                            else if (SetRPM >= rpm_12000 && SetRPM < rpm_12500)
                            {
                                LabelRPM_12000.Draw();
                            }
                            else if (SetRPM >= rpm_12500 && SetRPM < rpm_13000)
                            {
                                LabelRPM_12500.Draw();
                            }
                            else if (SetRPM >= rpm_13000 && SetRPM < rpm_13500)
                            {
                                LabelRPM_13000.Draw();
                            }
                            else if (SetRPM >= rpm_13500 && SetRPM < rpm_14000)
                            {
                                LabelRPM_13500.Draw();
                            }
                            else if (SetRPM >= rpm_14000 && SetRPM < rpm_14500)
                            {
                                LabelRPM_14000.Draw();
                            }
                            else if (SetRPM >= rpm_14500 && SetRPM < rpm_15000)
                            {
                                LabelRPM_14500.Draw();
                            }
                            else if (SetRPM >= rpm_15000 && SetRPM < rpm_15500)
                            {
                                LabelRPM_15000.Draw();
                            }
                            else if (SetRPM >= rpm_15500 && SetRPM < rpm_16000)
                            {
                                LabelRPM_15500.Draw();
                            }
                            else if (SetRPM >= rpm_16000 && SetRPM < rpm_16500)
                            {
                                LabelRPM_16000.Draw();
                            }
                            else if (SetRPM >= rpm_16500 && SetRPM < rpm_17000)
                            {
                                LabelRPM_16500.Draw();
                            }
                            else if (SetRPM >= rpm_17000 && SetRPM < rpm_18000)
                            {
                                LabelRPM_17000.Draw();
                            }
                            else if (SetRPM >= rpm_18000 && SetRPM < rpm_19000)
                            {
                                LabelRPM_18000.Draw();
                            }
                            else if (SetRPM >= rpm_19000 && SetRPM < 20000)
                            {
                                LabelRPM_19000.Draw();
                            }
                            else
                            {
                                LabelRPMDeafult.Draw();
                            }

                            if (gearhudcolour_default)
                            {
                                if (gearTexture != null)
                                {
                                    gearTexture.Draw();
                                }
                            }
                            if (gearhudcolour_red)
                            {
                                if (gearTexture_Red != null)
                                {
                                    gearTexture_Red.Draw();
                                }
                            }
                            // Control.VehicleHandbrake corresponds to Spacebar on PC / R1 or RB on controllers
                            if (Game.IsControlPressed(GTA.Control.VehicleHandbrake))
                            {
                                HandbrakeTextureImage.Draw();
                            }

                            Vehicle veh = playerPed.CurrentVehicle;

                            // คำนวณความเร็ว (ความเร็วปกติของเกมเป็น เมตร/วินาที คูณ 3.6 เพื่อแปลงเป็น กม./ชม.)
                            // หากต้องการเป็น ไมล์/ชั่วโมง (MPH) ให้คูณด้วย 2.236936f
                            float speedKmh = veh.Speed * isSwitchUnits;

                            // กำหนดเพดานความเร็วไม่ให้เกิน 1000 (ตามชื่อรูปภาพของคุณ)
                            if (speedKmh > 1799f) speedKmh = 1799f;
                            if (speedKmh < 0f) speedKmh = 0f;

                            // แปลงค่าความเร็วเป็นตัวเลข 4 หลัก เช่น 0000, 0120, 1000
                            int speedInt = (int)Math.Round(speedKmh);
                            string textureName = SpeedBaseTextureName + speedInt.ToString("D4");

                            // อัปเดตชื่อ Texture ปัจจุบันตามความเร็วรถ
                            speedTextureImage.Texture = textureName;


                            if (isRevEngineCurrent)
                            {

                                // ดึงค่า RPM จริงจากรถปัจจุบัน (0.0 ถึง 1.0)
                                float targetRpm = veh.CurrentRPM;

                                // สูตร LERP: ค่อยๆ ขยับ smoothedRpm เข้าหา targetRpm ทีละนิดในทุกเฟรม
                                smoothedRpm = smoothedRpm + (targetRpm - smoothedRpm) * interpolationSpeed;

                                // ส่งค่าสะสมที่นิ่งแล้ว ไปแปลงชื่อภาพ
                                // (ปรับ Min/Max เผื่อไว้ให้ด้วย: เดินเบา 0.2f สูงสุด 1.0f)
                                string RPMtextureName = GetTextureNameFromRange(smoothedRpm, 0.2f, 1.0f);

                                RevTextureImage.Texture = RPMtextureName;
                                RevTextureImage.Draw();
                            }

                            if (isABSHUD)
                            {
                                ABSTextureImage.Draw();
                            }
                            if (isTCSHUD)
                            {
                                TCSTextureImage.Draw();
                            }

                            Vehicle playerVeh = Game.Player.Character.CurrentVehicle;
                            // ตรวจสอบว่าผู้เล่นอยู่ในรถหรือไม่
                            if (playerVeh != null && playerVeh.Exists())
                            {
                                isEngineOn = playerVeh.IsEngineRunning;

                                // อัปเดตสีของไอคอนตามสถานะเครื่องยนต์
                                if (isEngineOn)
                                {
                                    // Engine On
                                    isRevEngineCurrent = true;
                                }
                                else
                                {
                                    // Engine Off
                                    RevEngineOffImage.Draw();
                                    isRevEngineCurrent = false;
                                }
                            }

                            DriftHUD();

                            // Check if the player is alive and inside a vehicle
                            if (playerPed != null && playerPed.IsAlive && playerPed.IsInVehicle())
                            {
                                Vehicle veh2 = playerPed.CurrentVehicle;
                                int currentGear = veh2.CurrentGear;
                                bool isNeutral = false;

                                // ลอจิกจำลองเกียร์ว่าง (Neutral) 
                                // ใน GTA V ค่าเกียร์ 1 จะถูกส่งมาตอนจอดนิ่งๆ ด้วย ดังนั้นเราจะเช็คเพิ่มว่าถ้ารถหยุดนิ่งและรอบเครื่องต่ำ ให้สลับเป็นภาพเกียร์ว่าง
                                if (currentGear == 1 && veh2.Speed < 0.1f && veh2.CurrentRPM < 0.3f)
                                {
                                    isNeutral = true;
                                }

                                // อัปเดตรูปภาพเฉพาะเมื่อเกียร์เปลี่ยน หรือสถานะเกียร์ว่างเปลี่ยน เพื่อลดภาระการทำงาน
                                if (currentGear != lastGear || isNeutral != wasNeutral)
                                {
                                    string textureNameGear = GetGearTextureName(currentGear, isNeutral);

                                    // เรียกใช้ LemonUI ดึงภาพตรงจาก script_txds.rpf
                                    // พิกัดมุมขวาล่างจอ 16:9 (X: 1160, Y: 620) ขนาดรูปภาพ (56x56 พิกเซล)
                                    gearTexture = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDGear, textureNameGear);
                                    gearTexture.Color = Color.FromArgb(A_GearColour, R_GearColour, G_GearColour, B_GearColour);

                                    gearTexture_Red = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HUDGear, textureNameGear);
                                    gearTexture_Red.Color = Color.FromArgb(A_GearColour_Red, R_GearColour_Red, G_GearColour_Red, B_GearColour_Red);

                                    lastGear = currentGear;
                                    wasNeutral = isNeutral;
                                }
                            }
                            else
                            {
                                // ถ้าอยู่นอกรถ ให้เข็มค่อยๆ ตกกลับไปที่ 0 อย่างนุ่มนวล
                                smoothedRpm = smoothedRpm + (0f - smoothedRpm) * interpolationSpeed;

                                // รีเซ็ตสถานะเมื่อลงจากรถ
                                lastGear = -1;
                                wasNeutral = false;
                                gearTexture = null;
                                gearTexture_Red = null;
                            }

                            if (TurnSignalLeft_Enabled)
                            {
                                TurnSignalLeft.Draw();
                            }
                            if (TurnSignalRight_Enabled)
                            {
                                TurnSignalRight.Draw();
                            }
                            if (headlightsSeason5)
                            {
                                HeadlightsImage.Draw();
                            }
                            if (isHUDThrottleAndBrakeSeason5)
                            {
                                ThrottleImageHUD.Draw();
                                BrakeImageHUD.Draw();
                            }
                        }
                    }
                }
                // Phone and Character Wheel
                Vehicle characterWheels = Game.Player.Character.CurrentVehicle;
                if (characterWheels != null && characterWheels.Exists())
                {
                    // Control.VehicleHandbrake corresponds to Spacebar on PC / R1 or RB on controllers
                    if (Game.IsControlPressed(GTA.Control.CharacterWheel))
                    {
                        isHideHUDForOpenPhoneorCharacterWheel = false;
                    }
                    else
                    {
                        isHideHUDForOpenPhoneorCharacterWheel = true;

                        // Ensure the player exists and is alive
                        if (playerPed != null && playerPed.Exists() && playerPed.IsAlive)
                        {
                            // Call the native hash function
                            // Arguments: Hash, Return Type, Input Parameters (the ped's handle)
                            bool isUsingPhone = Function.Call<bool>(Hash.IS_PED_RUNNING_MOBILE_PHONE_TASK, playerPed.Handle);

                            if (isUsingPhone)
                            {
                                isHideHUDForOpenPhoneorCharacterWheel = false;
                            }
                            else
                            {
                                isHideHUDForOpenPhoneorCharacterWheel = true;
                            }
                        }
                    }
                }
            }
        }
        private string GetTextureNameFromRange(float currentValue, float minValue, float maxValue)
        {
            // ใช้ Math.Max/Min สำหรับ .NET 4.8
            currentValue = Math.Max(minValue, Math.Min(currentValue, maxValue));

            float normalized = (currentValue - minValue) / (maxValue - minValue);

            // เปลี่ยนตัวคูณเป็น 1000 เพื่อให้ไล่ลำดับภาพทีละ 1 เฟรม (เช่น speed_0001, speed_0002)
            int textureValue = (int)Math.Round(normalized * rpmMax);
            textureValue = Math.Max(rpmMin, Math.Min(textureValue, rpmMax));

            currentRPM = textureValue;
            return RevBaseTextureName + $"{textureValue:D4}";
        }

        private string GetGearTextureName(int gear, bool isNeutral)
        {
            if (gear == 0) return "gear_r";       // ภาพ r.png (Reverse)
            if (isNeutral) return "gear_n";       // ภาพ n.png (Neutral)

            return $"gear_{gear}";                // ภาพ 1.png ถึง 6.png ตามค่าเกียร์ที่ส่งมา
        }
        private void VehicleEnterExit()
        {
            Player player = Game.Player;

            // Ensure the player character exists and is alive
            if (player == null || player.Character == null || !player.Character.IsAlive)
            {
                return;
            }

            Ped playerPed = player.Character;
            bool isInVehicle = playerPed.IsInVehicle();

            // Detect entry: Player is in a vehicle now, but wasn't in the previous frame
            if (isInVehicle && !_wasInVehicle)
            {
                statusHUD = true;
                _wasInVehicle = true;
            }
            // Detect exit: Player is no longer in a vehicle, but was in one in the previous frame
            else if (!isInVehicle && _wasInVehicle)
            {
                statusHUD = false;
                _lastVehicle = null;
                _wasInVehicle = false;
            }
        }

        private void VehicleSlowMotionFranklin()
        {
            Player player = Game.Player;
            Ped playerPed = player.Character;

            // 1. Check if the player is Franklin
            // Franklin's model hash can be verified via Hash.Michael, Hash.Franklin, Hash.Trevor
            if ((PedHash)playerPed.Model.Hash == PedHash.Franklin)
            {
                // 2. Check if Franklin is currently driving a vehicle
                if (playerPed.IsInVehicle())
                {
                    // 3. Check if the special ability is active
                    // Native.Function.Call returns true if the special ability meter is active/draining
                    bool isAbilityActive = Function.Call<bool>(Hash.IS_SPECIAL_ABILITY_ACTIVE, player.Handle);

                    if (isAbilityActive)
                    {
                        // Franklin Slow Car True
                    }
                    else
                    {
                        // Franklin Slow Car False
                    }
                }
            }
        }

        private void DriftHUD()
        {
            Ped player = Game.Player.Character;
            // 2. Check if the player is alive and inside a vehicle
            if (player.Exists() && player.IsInVehicle())
            {
                Vehicle veh = player.CurrentVehicle;

                // Ensure the vehicle is moving and on its wheels
                if (veh.Exists() && veh.IsAlive && veh.IsOnAllWheels)
                {

                    // 1. Custom Image Size
                    Size imageSize = new Size(200, 200);

                    // 2. Custom Position X , Y
                    PointF imagePosition = new PointF(UIPositionX, UIPositionY);

                    // Get vehicle velocity vector and forward vector
                    Vector3 velocity = veh.Velocity;
                    Vector3 forward = veh.ForwardVector;

                    // Calculate speed in MPH or KPH (velocity.Length() is in meters/sec)
                    float speedKph = velocity.Length() * 3.6f;

                    if (velocity.Length() > MinSpeedThreshold)
                    {
                        // Normalize vectors to calculate the angle between them
                        velocity.Normalize();
                        forward.Normalize();

                        // Calculate the dot product and convert to an angle in degrees
                        double dot = Vector3.Dot(forward, velocity);
                        double angle = Math.Acos(Math.Max(-1.0, Math.Min(1.0, dot))) * (180.0 / Math.PI);

                        // 3. Determine drift status and show subtitle
                        if (angle >= DriftAngleThreshold)
                        {
                            if(isDriftHUD)
                            {
                                DriftTextureImage.Draw();
                            }
                        }
                    }
                }
            }
        }
        private string GetVehicleClassName(VehicleClass vClass)
        {
            switch (vClass)
            {
                case VehicleClass.Compacts:return "Compacts";
                case VehicleClass.Sedans: return "Sedans";
                case VehicleClass.SUVs: return "SUV";
                case VehicleClass.Coupes: return "Coupes";
                case VehicleClass.Muscle: return "Muscle";
                case VehicleClass.SportsClassics: return "SportsClassics";
                case VehicleClass.Sports: return "Sports";
                case VehicleClass.Super: return "Super";
                case VehicleClass.Motorcycles: return "Motorcycles";
                case VehicleClass.OffRoad: return "OffRoad";
                case VehicleClass.Industrial: return "Industrial";
                case VehicleClass.Utility: return "Utility";
                case VehicleClass.Vans: return "Vans";
                case VehicleClass.Cycles: return "Cycles";
                case VehicleClass.Boats: return "Boats";
                case VehicleClass.Helicopters: return "Helicopters";
                case VehicleClass.Planes: return "Planes";
                case VehicleClass.Service: return "Service";
                case VehicleClass.Emergency: return "Emergency";
                case VehicleClass.Military: return "Military";
                case VehicleClass.Commercial: return "Commercial";
                case VehicleClass.OpenWheel: return "OpenWheel";
                default: return "Vehicle General";
            }
        }

        private void LoadConfigData()
        {
            // กำหนดพาธไฟล์ให้ถูกต้องตามมาตรฐาน Windows
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);

            // เคลียร์ข้อมูลเก่าใน Dictionary ออกก่อน (ป้องกันข้อมูลซ้ำซ้อนเวลาโหลดใหม่)
            carNamesDict.Clear();

            if (!File.Exists(fullPath))
            {
                GTA.UI.Screen.ShowSubtitle("~r~[Error] not found file Example.cfg in folder scripts!", 5000);
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(fullPath);
                foreach (string line in lines)
                {
                    // ข้ามบรรทัดว่าง
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // แยกข้อความด้วยเครื่องหมาย Backslash (\)
                    string[] parts = line.Split('\\');

                    // ต้องมีข้อมูลครบ 3 ส่วนพอดี (ชื่อ\โมเดล\ราคา)
                    if (parts.Length == 3)
                    {
                        // [0] ช่องแรกสุด = ชื่อรถที่จะแสดงผล (เช่น Mercedes-AMG AMG ONE)
                        string displayName = parts[0].Trim();

                        // [1] ช่องตรงกลาง = ชื่อ Spawn Name ตัวพิมพ์เล็ก (เช่น one หรือ comet2)
                        string modelName = parts[1].Trim().ToLower();

                        // [2] ช่องสุดท้าย = ตัวเลขราคารถ (ล้างขยะและช่องว่างออกให้หมด)
                        string maxRPM = parts[2].Replace("\r", "").Replace("\n", "").Trim();


                        // แปลงข้อความราคาเป็นตัวเลข int หากผ่านจะนำเข้าคลังข้อมูลทันที
                        if (int.TryParse(maxRPM, out int displaymaxrpms))
                        {
                            if (!carNamesDict.ContainsKey(modelName))
                            {
                                carNamesDict.Add(modelName, new CarData(displayName, displaymaxrpms));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Screen.ShowSubtitle($"~r~[Error] Read file .cfg failed: {ex.Message}", 5000);
            }
        }
    }

    public class CarData
    {
        public string DisplayName { get; set; }
        public int DisplayMaxRPM { get; set; }
        public CarData(string displayName, int displaymaxrpms)
        {
            DisplayName = displayName;
            DisplayMaxRPM = displaymaxrpms;
        }
    }
}
