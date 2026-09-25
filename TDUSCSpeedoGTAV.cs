using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace TDUSCSpeedometerGTAV
{
    public class TDUSCSpeedoGTAV : Script
    {

        private string version = "0.4";
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

        // ตัวแปรเก็บสถานะล่าสุดเพื่อไม่ให้ Debug Notification ขึ้นรัวๆ ทุกเฟรม
        private bool wasPhoneVisible = false;

        // Menu UI
        private MenuPool _menuPool;
        private UIMenu _mainMenu;

        // Track the last vehicle the player was in to detect when they exit
        private Vehicle _lastVehicle = null;
        private bool _wasInVehicle = false;


        // Drift Status
        private const float DriftAngleThreshold = 10.0f;
        private const float MinSpeedThreshold = 3.0f;

        // TDU Solar Crown Speedometer
        private bool isMainHUDModEnabled;
        private bool isSpeedometerBackgroundDemo;
        private bool isRevEngineCurrent;
        private bool isABSHUD;
        private bool isTCSHUD;

        private bool isKMH;
        private bool isKMH_DNKNOR;
        private bool isKMH_RUS;
        private bool isKMH_TUR;
        private bool isKMH_UKR;
        private bool isKMH_VN;

        private bool isMPH;

        // config ini
        private ScriptSettings config;

        /* สำหรับทำงานผ่าน ytd update\update.rpf\x64\textures\script_txds.rpf */
        private ScaledTexture gearTexture;
        private ScaledTexture gearTexture_Release;
        private int lastGear = -1;
        private bool wasNeutral = false;

        // กำหนดชื่อไฟล์ .ytd ที่เราใส่ไว้ใน script_txds.rpf (ใส่แค่ชื่อ ไม่ต้องระบุนามสกุล .ytd)


        private int SelectVersionHUD;
        // Demo HUD
        private bool HUD_Demo;
        private const string GearTextureDict = "!TDUSCDemoHUDGear";
        private ScaledTexture BackgroundTextureImage;
        private const string BackgroundTextureDict = "!TDUSCDemoHUDBackground";
        private const string BackgroundTextureName = "background";
        private ScaledTexture BackgroundTextureImage2;
        private const string BackgroundTextureDict2 = "!TDUSCDemoHUDBackground";
        private const string BackgroundTextureName2 = "background2";
        private ScaledTexture ImageKMH;
        private const string ImageKMHDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMHName = "kmh";
        private ScaledTexture ImageKMH_DNKNOR;
        private const string ImageKMH_DNKNORDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_DNKNORName = "kmh_DenmarkNorway";
        private ScaledTexture ImageKMH_RUS;
        private const string ImageKMH_RUSDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_RUSName = "kmh_Russian";
        private ScaledTexture ImageKMH_TUR;
        private const string ImageKMH_TURDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_TURName = "kmh_Turkish";
        private ScaledTexture ImageKMH_UKR;
        private const string ImageKMH_UKRDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_UKRName = "kmh_Ukriane";
        private ScaledTexture ImageKMH_VNM;
        private const string ImageKMH_VNDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_VNName = "kmh_Vietnam";
        private ScaledTexture ImageKMH_NLD;
        private const string ImageKMH_NLDDict = "!TDUSCDemoHUDBackground";
        private const string ImageKMH_NLDName = "kmh_Nederlands";
        private ScaledTexture ImageMPH;
        private const string ImageMPHDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPHName = "mph";
        private ScaledTexture ImageMPH_NLD;
        private const string ImageMPH_NLDDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPH_NLDName = "mph_Nederlands";
        private ScaledTexture ImageMPH_RUS;
        private const string ImageMPH_RUSDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPH_RUSName = "mph_Russian";
        private ScaledTexture ImageMPH_TUR;
        private const string ImageMPH_TURDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPH_TURName = "mph_Turkish";
        private ScaledTexture ImageMPH_UKR;
        private const string ImageMPH_UKRDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPH_UKRName = "mph_Ukriane";
        private ScaledTexture ImageMPH_VNM;
        private const string ImageMPH_VNMDict = "!TDUSCDemoHUDBackground";
        private const string ImageMPH_VNMName = "mph_Vietnam";
        private ScaledTexture DriftTextureImage;
        private const string DriftTextureDict = "!TDUSCDemoHUDBackground";
        private const string DriftTextureName = "Drift";
        private ScaledTexture HandbrakeTextureImage;
        private const string HandbrakeTextireDict = "!TDUSCDemoHUDBackground";
        private const string HandbrakeTextureName = "Parking_Brake";
        private ScaledTexture speedTextureImage;
        private const string SpeedTextureDict = "!TDUSCDemoHUDSpeed";
        private const string SpeedBaseTextureName = "speed_";
        private ScaledTexture RevTextureImage;
        private const string RevTextureDict = "!TDUSCDemoHUDRev";
        private const string RevBaseTextureName = "rev_";
        private ScaledTexture ABSTextureImage;
        private const string ABSTextureImageDict = "!TDUSCDemoHUDBackground";
        private const string ABSTextureImageName = "ABS";
        private ScaledTexture TCSTextureImage;
        private const string TCSTextureImageDict = "!TDUSCDemoHUDBackground";
        private const string TCSTextureImageName = "TCS";

        // Set LabelRPM
        private ScaledTexture LabelRPMDeafult;
        private const string LabelRPMDefaultDict = "!TDUSCDemoHUDBackground";
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


        private ScaledTexture TestEngineOff;
        private const string TestEngineOffDict = "!TDUSCDemoHUDBackground";
        private const string TestEngineOffName = "TurnLeft";

        private ScaledTexture TestEngineOn;
        private const string TestEngineOnDict = "!TDUSCDemoHUDBackground";
        private const string TestEngineOnName = "TurnRight";

        private ScaledTexture RevEngineOffImage;
        private const string RevEngineOffImageDict = "!TDUSCDemoHUDRev";
        private const string RevEngineOffImageName = "rev_0000";


        // Release HUD
        private bool HUD_Release;
        private const string GearTextureDict_Release = "!TDUSCReleaseHUDGear";
        private ScaledTexture BackgroundTextureImage_Release;
        private const string BackgroundTextureDict_Release = "!TDUSCReleaseHUDBackground";
        private const string BackgroundTextureName_Release = "background";
        private ScaledTexture BackgroundTextureImage2_Release;
        private const string BackgroundTextureDict2_Release = "!TDUSCReleaseHUDBackground";
        private const string BackgroundTextureName2_Release = "background2";
        private ScaledTexture ImageKMH_Release;
        private const string ImageKMHDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMHName_Release = "kmh";
        private ScaledTexture ImageKMH_DNKNOR_Release;
        private const string ImageKMH_DNKNORDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_DNKNORName_Release = "kmh_DenmarkNorway";
        private ScaledTexture ImageKMH_RUS_Release;
        private const string ImageKMH_RUSDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_RUSName_Release = "kmh_Russian";
        private ScaledTexture ImageKMH_TUR_Release;
        private const string ImageKMH_TURDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_TURName_Release = "kmh_Turkish";
        private ScaledTexture ImageKMH_UKR_Release;
        private const string ImageKMH_UKRDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_UKRName_Release = "kmh_Ukriane";
        private ScaledTexture ImageKMH_VNM_Release;
        private const string ImageKMH_VNDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_VNName_Release = "kmh_Vietnam";
        private ScaledTexture ImageKMH_NLD_Release;
        private const string ImageKMH_NLDDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageKMH_NLDName_Release = "kmh_Nederlands";
        private ScaledTexture ImageMPH_Release;
        private const string ImageMPHDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPHName_Release = "mph";
        private ScaledTexture ImageMPH_NLD_Release;
        private const string ImageMPH_NLDDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPH_NLDName_Release = "mph_Nederlands";
        private ScaledTexture ImageMPH_RUS_Release;
        private const string ImageMPH_RUSDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPH_RUSName_Release = "mph_Russian";
        private ScaledTexture ImageMPH_TUR_Release;
        private const string ImageMPH_TURDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPH_TURName_Release = "mph_Turkish";
        private ScaledTexture ImageMPH_UKR_Release;
        private const string ImageMPH_UKRDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPH_UKRName_Release = "mph_Ukriane";
        private ScaledTexture ImageMPH_VNM_Release;
        private const string ImageMPH_VNMDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ImageMPH_VNMName_Release = "mph_Vietnam";
        private ScaledTexture DriftTextureImage_Release;
        private const string DriftTextureDict_Release = "!TDUSCReleaseHUDBackground";
        private const string DriftTextureName_Release = "Drift";
        private ScaledTexture HandbrakeTextureImage_Release;
        private const string HandbrakeTextireDict_Release = "!TDUSCReleaseHUDBackground";
        private const string HandbrakeTextureName_Release = "handbrake";
        private ScaledTexture speedTextureImage_Release;
        private const string SpeedTextureDict_Release = "!TDUSCReleaseHUDSpeed";
        private const string SpeedBaseTextureName_Release = "speed_";
        private ScaledTexture RevTextureImage_Release;
        private const string RevTextureDict_Release = "!TDUSCReleaseHUDRev";
        private ScaledTexture ABSTextureImage_Release;
        private const string ABSTextureImageDict_Release = "!TDUSCReleaseHUDBackground";
        private const string ABSTextureImageName_Release = "ABS";
        private ScaledTexture TCSTextureImage_Release;
        private const string TCSTextureImageDict_Release = "!TDUSCReleaseHUDBackground";
        private const string TCSTextureImageName_Release = "TCS";


        private ScaledTexture LabelRPMDeafult_Release;
        private const string LabelRPMDefaultDict_Release = "!TDUSCReleaseHUDBackground";
        private const string LabelRPMDefaultName_Release = "rpm_9000";



        private ScaledTexture LabelRPM_2000_Release;
        private ScaledTexture LabelRPM_2200_Release;
        private ScaledTexture LabelRPM_2400_Release;
        private ScaledTexture LabelRPM_2600_Release;
        private ScaledTexture LabelRPM_2800_Release;
        private ScaledTexture LabelRPM_3000_Release;
        private ScaledTexture LabelRPM_4000_Release;
        private ScaledTexture LabelRPM_4500_Release;
        private ScaledTexture LabelRPM_5000_Release;
        private ScaledTexture LabelRPM_5200_Release;
        private ScaledTexture LabelRPM_5400_Release;
        private ScaledTexture LabelRPM_5600_Release;
        private ScaledTexture LabelRPM_5800_Release;
        private ScaledTexture LabelRPM_6000_Release;
        private ScaledTexture LabelRPM_6200_Release;
        private ScaledTexture LabelRPM_6400_Release;
        private ScaledTexture LabelRPM_6600_Release;
        private ScaledTexture LabelRPM_6800_Release;
        private ScaledTexture LabelRPM_7000_Release;
        private ScaledTexture LabelRPM_7200_Release;
        private ScaledTexture LabelRPM_7400_Release;
        private ScaledTexture LabelRPM_7600_Release;
        private ScaledTexture LabelRPM_7800_Release;
        private ScaledTexture LabelRPM_8000_Release;
        private ScaledTexture LabelRPM_8200_Release;
        private ScaledTexture LabelRPM_8400_Release;
        private ScaledTexture LabelRPM_8600_Release;
        private ScaledTexture LabelRPM_8800_Release;
        private ScaledTexture LabelRPM_9000_Release;
        private ScaledTexture LabelRPM_9200_Release;
        private ScaledTexture LabelRPM_9400_Release;
        private ScaledTexture LabelRPM_9600_Release;
        private ScaledTexture LabelRPM_9800_Release;
        private ScaledTexture LabelRPM_10000_Release;
        private ScaledTexture LabelRPM_10500_Release;
        private ScaledTexture LabelRPM_11000_Release;
        private ScaledTexture LabelRPM_11500_Release;
        private ScaledTexture LabelRPM_12000_Release;
        private ScaledTexture LabelRPM_12500_Release;
        private ScaledTexture LabelRPM_13000_Release;
        private ScaledTexture LabelRPM_13500_Release;
        private ScaledTexture LabelRPM_14000_Release;
        private ScaledTexture LabelRPM_14500_Release;
        private ScaledTexture LabelRPM_15000_Release;
        private ScaledTexture LabelRPM_15500_Release;
        private ScaledTexture LabelRPM_16000_Release;
        private ScaledTexture LabelRPM_16500_Release;
        private ScaledTexture LabelRPM_17000_Release;
        private ScaledTexture LabelRPM_18000_Release;
        private ScaledTexture LabelRPM_19000_Release;


        private ScaledTexture RevEngineOffImage_Release;
        private const string RevEngineOffImageDict_Release = "!TDUSCReleaseHUDRev";
        private const string RevEngineOffImageName_Release = "rev_0000";

        private int isUnitsLanguage;

        private float isSwitchUnits;
        private float smoothedRpm = 0f;
        private float interpolationSpeed = 0.15f;

        private int rpmMin = 0;
        private int rpmMax = 1799;

        private bool isEngineOn = false;

        private int UseKMH;

        private float speedKMH = 3.6f;
        private float speedMPH = 2.236936f;

        private Keys keyOpenMenu;

        private string LabelRPMCategory;


        // Set RPM
        private int numberlabelrpm;
        private int SetRPM;
        public TDUSCSpeedoGTAV()
        {
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

            A_GearColour = config.GetValue<int>("Gear_Colour_RGB", "A", A_GearColour);
            R_GearColour = config.GetValue<int>("Gear_Colour_RGB", "R", R_GearColour);
            G_GearColour = config.GetValue<int>("Gear_Colour_RGB", "G", G_GearColour);
            B_GearColour = config.GetValue<int>("Gear_Colour_RGB", "B", B_GearColour);

            rpmMin = config.GetValue<int>("RPM_Settings", "RPMMin", rpmMin);
            rpmMax = config.GetValue<int>("RPM_Settings", "RPMMax", rpmMax);

            UseKMH = config.GetValue<int>("Units_Settings", "UseKMH", UseKMH);
            isUnitsLanguage = config.GetValue<int>("Units_Settings", "LanguageUnits", isUnitsLanguage);

            numberlabelrpm = config.GetValue<int>("CATEGORY", "LabelRPMCategoryText", numberlabelrpm);

            _menuPool = new MenuPool();
            _mainMenu = new UIMenu("TDUSCSpeedoV", "Version " + version + " by " + credits);
            _menuPool.Add(_mainMenu);

            // 2. Create the Checkbox Item (Boolean toggle)
            UIMenuCheckboxItem modEnabledHUD = new UIMenuCheckboxItem("Mod Enabled", isMainHUDModEnabled, "Mod Show and Hide HUD for Test Drive Unlimited Solar Crown Speedometer");
            UIMenuCheckboxItem ABSToggle = new UIMenuCheckboxItem("ABS HUD", isABSHUD);
            UIMenuCheckboxItem TCSToggle = new UIMenuCheckboxItem("TCS HUD", isTCSHUD);

            var itemsListUnits = new List<dynamic> { "Imperial", "Metric" };
            var UnitsListItem = new UIMenuListItem("Units", itemsListUnits, UseKMH, "Metric = KM/H\nImperial = MPH");

            var itemsListLanguage = new List<dynamic> { "Default", "Denmark / Norway", "Nederlands" ,"Russian" , "Turkish", "Ukriane", "Vietnamese" };
            var LanguageListItem = new UIMenuListItem("Language Units", itemsListLanguage, isUnitsLanguage, "Select Language for KM/H or MPH");

            var itemsListVersionHUD = new List<dynamic> { "Demo", "Release" };
            var VersionHUDListItem = new UIMenuListItem("Version HUD", itemsListVersionHUD, SelectVersionHUD, "Select HUD");

            var itemsListTestSelectLabelRPM = new List<dynamic> { "2000" , "2200", "2400", "2600", "2800", "3000", "4000", "4500", "5000", "5200", "5400", "5600", "5800", "6000", "6200", "6400", "6600", "6800", "7000", "7200", "7400", "7600", "7800", "8000", "8200", "8400", "8600", "8800", "9000", "9200", "9400", "9600", "9800", "10000", "10500", "11000", "11500", "12000", "12500", "13000", "13500", "14000", "14500", "15000", "15500", "16000", "16500", "17000", "18000", "19000"};
            var TestSelectLabelRPMItem = new UIMenuListItem("Test Select Label RPM" , itemsListTestSelectLabelRPM, numberlabelrpm);

            // 3. Add the checkbox to the menu
            _mainMenu.AddItem(modEnabledHUD);
            _mainMenu.AddItem(VersionHUDListItem);
            _mainMenu.AddItem(UnitsListItem);
            _mainMenu.AddItem(LanguageListItem);
            _mainMenu.AddItem(ABSToggle);
            _mainMenu.AddItem(TCSToggle);
            _mainMenu.AddItem(TestSelectLabelRPMItem);

            // 4. Hook up the If/Else logic to the event
            _mainMenu.OnCheckboxChange += (sender, item, Checked) =>
            {
                if (item == modEnabledHUD)
                {
                    if (Checked)
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
                }
                if (item == ABSToggle)
                {
                    if (Checked)
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
                }
                if (item == TCSToggle)
                {
                    if (Checked)
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
                }
            };
            // 5. ดักจับเหตุการณ์ (Event) เมื่อมีการเปลี่ยนแปลงหรือกดปุ่ม

            // เมื่อผู้เล่นกดปุ่มซ้าย-ขวา เปลี่ยนค่า (< 1 > เป็น < 2 >)


            _mainMenu.OnListChange += (sender, item, index) =>
            {
                if (item == UnitsListItem)
                {
                    // ทำงานทันทีเมื่อเปลี่ยนตัวเลือก (แสดงข้อความแจ้งเตือนมุมซ้าย)

                    switch (index)
                    {

                        case 0:
                            // MPH
                            isSwitchUnits = speedMPH;
                            isKMH = false;
                            isMPH = true;
                            config.SetValue<int>("Units_Settings", "UseKMH", 0);
                            config.Save();
                            break;
                        case 1:
                            // KM/H
                            isSwitchUnits = speedKMH;
                            isKMH = true;
                            isMPH = false;
                            config.SetValue<int>("Units_Settings", "UseKMH", 1);
                            config.Save();
                            break;
                    }
                }

                if (item == LanguageListItem)
                {
                    isUnitsLanguage = index;

                    switch (index)
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
                }

                if(item == VersionHUDListItem)
                {
                    SelectVersionHUD = index;

                    switch (index)
                    {
                        case 0:
                            HUD_Demo = true;
                            HUD_Release = false;
                            config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 0);
                            config.Save();
                            break;
                        case 1:
                            HUD_Demo = false;
                            HUD_Release = true;
                            config.SetValue<int>("HUD_SETTINGS", "Version_HUD", 1);
                            config.Save();
                            break;
                    }
                }

                if(item == TestSelectLabelRPMItem)
                {
                    numberlabelrpm = index;

                    switch (index)
                    {
                        case 0:
                            SetRPM = 2000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 0);
                            config.Save();
                            break;
                        case 1:
                            SetRPM = 2200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 1);
                            config.Save();
                            break;
                        case 2:
                            SetRPM = 2400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 2);
                            config.Save();
                            break;
                        case 3:
                            SetRPM = 2600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 3);
                            config.Save();
                            break;
                        case 4:
                            SetRPM = 2800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 4);
                            config.Save();
                            break;
                        case 5:
                            SetRPM = 3000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 5);
                            config.Save();
                            break;
                        case 6:
                            SetRPM = 4000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 6);
                            config.Save();
                            break;
                        case 7:
                            SetRPM = 4500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 7);
                            config.Save();
                            break;
                        case 8:
                            SetRPM = 5000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 8);
                            config.Save();
                            break;
                        case 9:
                            SetRPM = 5200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 9);
                            config.Save();
                            break;
                        case 10:
                            SetRPM = 5400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 10);
                            config.Save();
                            break;
                        case 11:
                            SetRPM = 5600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 11);
                            config.Save();
                            break;
                        case 12:
                            SetRPM = 5800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 12);
                            config.Save();
                            break;
                        case 13:
                            SetRPM = 6000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 13);
                            config.Save();
                            break;
                        case 14:
                            SetRPM = 6200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 14);
                            config.Save();
                            break;
                        case 15:
                            SetRPM = 6400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 15);
                            config.Save();
                            break;
                        case 16:
                            SetRPM = 6600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 16);
                            config.Save();
                            break;
                        case 17:
                            SetRPM = 6800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 17);
                            config.Save();
                            break;
                        case 18:
                            SetRPM = 7000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 18);
                            config.Save();
                            break;
                        case 19:
                            SetRPM = 7200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 19);
                            config.Save();
                            break;
                        case 20:
                            SetRPM = 7400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 20);
                            config.Save();
                            break;
                        case 21:
                            SetRPM = 7600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 21);
                            config.Save();
                            break;
                        case 22:
                            SetRPM = 7800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 22);
                            config.Save();
                            break;
                        case 23:
                            SetRPM = 8000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 23);
                            config.Save();
                            break;
                        case 24:
                            SetRPM = 8200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 24);
                            config.Save();
                            break;
                        case 25:
                            SetRPM = 8400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 25);
                            config.Save();
                            break;
                        case 26:
                            SetRPM = 8600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 26);
                            config.Save();
                            break;
                        case 27:
                            SetRPM = 8800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 27);
                            config.Save();
                            break;
                        case 28:
                            SetRPM = 9000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 28);
                            config.Save();
                            break;
                        case 29:
                            SetRPM = 9200;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 29);
                            config.Save();
                            break;
                        case 30:
                            SetRPM = 9400;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 30);
                            config.Save();
                            break;
                        case 31:
                            SetRPM = 9600;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 31);
                            config.Save();
                            break;
                        case 32:
                            SetRPM = 9800;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 32);
                            config.Save();
                            break;
                        case 33:
                            SetRPM = 10000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 33);
                            config.Save();
                            break;
                        case 34:
                            SetRPM = 10500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 34);
                            config.Save();
                            break;
                        case 35:
                            SetRPM = 11000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 35);
                            config.Save();
                            break;
                        case 36:
                            SetRPM = 11500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 36);
                            config.Save();
                            break;
                        case 37:
                            SetRPM = 12000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 37);
                            config.Save();
                            break;
                        case 38:
                            SetRPM = 12500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 38);
                            config.Save();
                            break;
                        case 39:
                            SetRPM = 13000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 39);
                            config.Save();
                            break;
                        case 40:
                            SetRPM = 13500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 40);
                            config.Save();
                            break;
                        case 41:
                            SetRPM = 14000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 41);
                            config.Save();
                            break;
                        case 42:
                            SetRPM = 14500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 42);
                            config.Save();
                            break;
                        case 43:
                            SetRPM = 15000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 43);
                            config.Save();
                            break;
                        case 44:
                            SetRPM = 15500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 44);
                            config.Save();
                            break;
                        case 45:
                            SetRPM = 16000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 45);
                            config.Save();
                            break;
                        case 46:
                            SetRPM = 16500;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 46);
                            config.Save();
                            break;
                        case 47:
                            SetRPM = 17000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 47);
                            config.Save();
                            break;
                        case 48:
                            SetRPM = 18000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 48);
                            config.Save();
                            break;
                        case 49:
                            SetRPM = 19000;
                            config.SetValue<int>("CATEGORY", "LabelRPMCategoryText", 49);
                            config.Save();
                            break;
                    }
                }
            };

            _mainMenu.OnItemSelect += (sender, selectedItem, index) =>
            {
                // Button Only
            };
                Tick += OnTick;
                KeyUp += OnKeyDown;


            switch (UseKMH)
            {
                case 1:
                    isSwitchUnits = 3.6f;
                    isKMH = true;
                    isMPH = false;
                    break;

                case 0:
                    isSwitchUnits = 2.236936f;
                    isKMH = false;
                    isMPH = true;
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
                    HUD_Demo = true;
                    HUD_Release = false;
                    break;
                case 1:
                    HUD_Demo = false;
                    HUD_Release = true;
                    break;
            }

            switch (numberlabelrpm)
            {
                case 0:
                    SetRPM = 2000;
                    break;
                case 1:
                    SetRPM = 2200;
                    break;
                case 2:
                    SetRPM = 2400;
                    break;
                case 3:
                    SetRPM = 2600;
                    break;
                case 4:
                    SetRPM = 2800;
                    break;
                case 5:
                    SetRPM = 3000;
                    break;
                case 6:
                    SetRPM = 4000;
                    break;
                case 7:
                    SetRPM = 4500;
                    break;
                case 8:
                    SetRPM = 5000;
                    break;
                case 9:
                    SetRPM = 5200;
                    break;
                case 10:
                    SetRPM = 5400;
                    break;
                case 11:
                    SetRPM = 5600;
                    break;
                case 12:
                    SetRPM = 5800;
                    break;
                case 13:
                    SetRPM = 6000;
                    break;
                case 14:
                    SetRPM = 6200;
                    break;
                case 15:
                    SetRPM = 6400;
                    break;
                case 16:
                    SetRPM = 6600;
                    break;
                case 17:
                    SetRPM = 6800;
                    break;
                case 18:
                    SetRPM = 7000;
                    break;
                case 19:
                    SetRPM = 7200;
                    break;
                case 20:
                    SetRPM = 7400;
                    break;
                case 21:
                    SetRPM = 7600;
                    break;
                case 22:
                    SetRPM = 7800;
                    break;
                case 23:
                    SetRPM = 8000;
                    break;
                case 24:
                    SetRPM = 8200;
                    break;
                case 25:
                    SetRPM = 8400;
                    break;
                case 26:
                    SetRPM = 8600;
                    break;
                case 27:
                    SetRPM = 8800;
                    break;
                case 28:
                    SetRPM = 9000;
                    break;
                case 29:
                    SetRPM = 9200;
                    break;
                case 30:
                    SetRPM = 9400;
                    break;
                case 31:
                    SetRPM = 9600;
                    break;
                case 32:
                    SetRPM = 9800;
                    break;
                case 33:
                    SetRPM = 10000;
                    break;
                case 34:
                    SetRPM = 10500;
                    break;
                case 35:
                    SetRPM = 11000;
                    break;
                case 36:
                    SetRPM = 11500;
                    break;
                case 37:
                    SetRPM = 12000;
                    break;
                case 38:
                    SetRPM = 12500;
                    break;
                case 39:
                    SetRPM = 13000;
                    break;
                case 40:
                    SetRPM = 13500;
                    break;
                case 41:
                    SetRPM = 14000;
                    break;
                case 42:
                    SetRPM = 14500;
                    break;
                case 43:
                    SetRPM = 15000;
                    break;
                case 44:
                    SetRPM = 15500;
                    break;
                case 45:
                    SetRPM = 16000;
                    break;
                case 46:
                    SetRPM = 16500;
                    break;
                case 47:
                    SetRPM = 17000;
                    break;
                case 48:
                    SetRPM = 18000;
                    break;
                case 49:
                    SetRPM = 19000;
                    break;
            }


            // Demo HUD
            BackgroundTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict, BackgroundTextureName);
            BackgroundTextureImage2 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict2, BackgroundTextureName2);

            ImageKMH = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMHDict, ImageKMHName);
            ImageKMH_DNKNOR = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_DNKNORDict, ImageKMH_DNKNORName);
            ImageKMH_RUS = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_RUSDict, ImageKMH_RUSName);
            ImageKMH_TUR = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_TURDict, ImageKMH_TURName);
            ImageKMH_UKR = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_UKRDict, ImageKMH_UKRName);
            ImageKMH_VNM = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_VNDict, ImageKMH_VNName);
            ImageKMH_NLD = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_NLDDict, ImageKMH_NLDName);

            ImageMPH = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPHDict, ImageMPHName);
            ImageMPH_NLD = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_NLDDict, ImageMPH_NLDName);
            ImageMPH_RUS = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_RUSDict, ImageMPH_RUSName);
            ImageMPH_TUR = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_TURDict, ImageMPH_TURName);
            ImageMPH_UKR = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_UKRDict, ImageMPH_UKRName);
            ImageMPH_VNM = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_VNMDict, ImageMPH_VNMName);

            DriftTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), DriftTextureDict, DriftTextureName);
        
            HandbrakeTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HandbrakeTextireDict, HandbrakeTextureName);

            speedTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), SpeedTextureDict, "speed_0000");

            RevTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), RevTextureDict, "rev_0000");


            LabelRPMDeafult = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPMDefaultName);
            LabelRPM_2000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_2000_Name);
            LabelRPM_2200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_2200_Name);
            LabelRPM_2400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_2400_Name);
            LabelRPM_2600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_2600_Name);
            LabelRPM_2800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_2800_Name);
            LabelRPM_3000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_3000_Name);
            LabelRPM_4000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_4000_Name);
            LabelRPM_4500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_4500_Name);
            LabelRPM_5000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_5000_Name);
            LabelRPM_5200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_5200_Name);
            LabelRPM_5400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_5400_Name);
            LabelRPM_5600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_5600_Name);
            LabelRPM_5800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_5800_Name);
            LabelRPM_6000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_6000_Name);
            LabelRPM_6200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_6200_Name);
            LabelRPM_6400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_6400_Name);
            LabelRPM_6600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_6600_Name);
            LabelRPM_6800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_6800_Name);
            LabelRPM_7000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_7000_Name);
            LabelRPM_7200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_7200_Name);
            LabelRPM_7400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_7400_Name);
            LabelRPM_7600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_7600_Name);
            LabelRPM_7800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_7800_Name);
            LabelRPM_8000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_8000_Name);
            LabelRPM_8200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_8200_Name);
            LabelRPM_8400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_8400_Name);
            LabelRPM_8600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_8600_Name);
            LabelRPM_8800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_8800_Name);
            LabelRPM_9000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_9000_Name);
            LabelRPM_9200 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_9200_Name);
            LabelRPM_9400 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_9400_Name);
            LabelRPM_9600 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_9600_Name);
            LabelRPM_9800 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_9800_Name);
            LabelRPM_10000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_10000_Name);
            LabelRPM_10500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_10500_Name);
            LabelRPM_11000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_11000_Name);
            LabelRPM_11500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_11500_Name);
            LabelRPM_12000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_12000_Name);
            LabelRPM_12500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_12500_Name);
            LabelRPM_13000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_13000_Name);
            LabelRPM_13500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_13500_Name);
            LabelRPM_14000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_14000_Name);
            LabelRPM_14500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_14500_Name);
            LabelRPM_15000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_15000_Name);
            LabelRPM_15500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_15500_Name);
            LabelRPM_16000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_16000_Name);
            LabelRPM_16500 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_16500_Name);
            LabelRPM_17000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_17000_Name);
            LabelRPM_18000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_18000_Name);
            LabelRPM_19000 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict, LabelRPM_19000_Name);


            ABSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ABSTextureImageDict, ABSTextureImageName);

            TCSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TCSTextureImageDict, TCSTextureImageName);

            TestEngineOff = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TestEngineOffDict, TestEngineOffName);

            TestEngineOn = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TestEngineOnDict, TestEngineOnName);

            RevEngineOffImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), RevEngineOffImageDict, RevEngineOffImageName);

            // Release HUD
            BackgroundTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict_Release, BackgroundTextureName_Release);
            BackgroundTextureImage2_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict2_Release, BackgroundTextureName2_Release);

            ImageKMH_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMHDict_Release, ImageKMHName_Release);
            ImageKMH_DNKNOR_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_DNKNORDict_Release, ImageKMH_DNKNORName_Release);
            ImageKMH_RUS_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_RUSDict_Release, ImageKMH_RUSName_Release);
            ImageKMH_TUR_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_TURDict_Release, ImageKMH_TURName_Release);
            ImageKMH_UKR_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_UKRDict_Release, ImageKMH_UKRName_Release);
            ImageKMH_VNM_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_VNDict_Release, ImageKMH_VNName_Release);
            ImageKMH_NLD_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMH_NLDDict_Release, ImageKMH_NLDName_Release);

            ImageMPH_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPHDict_Release, ImageMPHName_Release);
            ImageMPH_NLD_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_NLDDict_Release, ImageMPH_NLDName_Release);
            ImageMPH_RUS_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_RUSDict_Release, ImageMPH_RUSName_Release);
            ImageMPH_TUR_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_TURDict_Release, ImageMPH_TURName_Release);
            ImageMPH_UKR_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_UKRDict_Release, ImageMPH_UKRName_Release);
            ImageMPH_VNM_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageMPH_VNMDict_Release, ImageMPH_VNMName_Release);

            DriftTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), DriftTextureDict_Release, DriftTextureName_Release);

            HandbrakeTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), HandbrakeTextireDict_Release, HandbrakeTextureName_Release);

            speedTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), SpeedTextureDict_Release, "speed_0000");

            RevTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), RevTextureDict_Release, "rev_0000");

            LabelRPMDeafult_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPMDefaultName_Release);
            LabelRPM_2000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_2000_Name);
            LabelRPM_2200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_2200_Name);
            LabelRPM_2400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_2400_Name);
            LabelRPM_2600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_2600_Name);
            LabelRPM_2800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_2800_Name);
            LabelRPM_3000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_3000_Name);
            LabelRPM_4000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_4000_Name);
            LabelRPM_4500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_4500_Name);
            LabelRPM_5000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_5000_Name);
            LabelRPM_5200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_5200_Name);
            LabelRPM_5400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_5400_Name);
            LabelRPM_5600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_5600_Name);
            LabelRPM_5800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_5800_Name);
            LabelRPM_6000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_6000_Name);
            LabelRPM_6200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_6200_Name);
            LabelRPM_6400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_6400_Name);
            LabelRPM_6600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_6600_Name);
            LabelRPM_6800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_6800_Name);
            LabelRPM_7000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_7000_Name);
            LabelRPM_7200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_7200_Name);
            LabelRPM_7400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_7400_Name);
            LabelRPM_7600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_7600_Name);
            LabelRPM_7800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_7800_Name);
            LabelRPM_8000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_8000_Name);
            LabelRPM_8200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_8200_Name);
            LabelRPM_8400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_8400_Name);
            LabelRPM_8600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_8600_Name);
            LabelRPM_8800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_8800_Name);
            LabelRPM_9000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_9000_Name);
            LabelRPM_9200_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_9200_Name);
            LabelRPM_9400_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_9400_Name);
            LabelRPM_9600_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_9600_Name);
            LabelRPM_9800_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_9800_Name);
            LabelRPM_10000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_10000_Name);
            LabelRPM_10500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_10500_Name);
            LabelRPM_11000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_11000_Name);
            LabelRPM_11500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_11500_Name);
            LabelRPM_12000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_12000_Name);
            LabelRPM_12500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_12500_Name);
            LabelRPM_13000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_13000_Name);
            LabelRPM_13500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_13500_Name);
            LabelRPM_14000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_14000_Name);
            LabelRPM_14500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_14500_Name);
            LabelRPM_15000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_15000_Name);
            LabelRPM_15500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_15500_Name);
            LabelRPM_16000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_16000_Name);
            LabelRPM_16500_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_16500_Name);
            LabelRPM_17000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_17000_Name);
            LabelRPM_18000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_18000_Name);
            LabelRPM_19000_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), LabelRPMDefaultDict_Release, LabelRPM_19000_Name);

            ABSTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ABSTextureImageDict_Release, ABSTextureImageName_Release);

            TCSTextureImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TCSTextureImageDict_Release, TCSTextureImageName_Release);

            RevEngineOffImage_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), RevEngineOffImageDict_Release, RevEngineOffImageName_Release);

        }
        private void OnTick(object sender, EventArgs e)
        {

            VehicleEnterExit();
            _menuPool.ProcessMenus();
            TDUSC_HUDDemoTick();
            VehicleSlowMotionFranklin();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyOpenMenu)
            {
                _mainMenu.Visible = !_mainMenu.Visible;
            }
        }
        private void TDUSC_HUDDemoTick()
        {
            if (!isMainHUDModEnabled)
            {

            }
            else
            {
                // Speedometer Base

                Ped playerPed = Game.Player.Character;
                Player player = Game.Player;
                if (isSpeedometerBackgroundDemo && playerPed != null && playerPed.IsInVehicle())
                {
                    // Get the player's current vehicle
                    Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;

                    
                    

                    // Ensure the player is actually in a vehicle
                    if (currentVehicle != null && currentVehicle.Exists())
                    {
                        // 2. ดึงประเภทของรถ (Vehicle Class) และแปลงเป็นภาษาไทย
                        VehicleClass vehicleClass = currentVehicle.ClassType;
                        string category = GetVehicleClassName(vehicleClass);


                        // 3. ดึงชื่อรุ่นรถ (รองรับทั้งรถเดิมและรถ Add-on)
                        string displayName = currentVehicle.LocalizedName;
                        if (string.IsNullOrEmpty(displayName) || displayName == "NULL")
                        {
                            displayName = currentVehicle.Model.ToString(); // ใช้ชื่อโมเดลเสกถ้าระบบหาชื่อแปลไม่เจอ
                        }

                        // 5. แสดงข้อความ Subtitle บอกทั้งประเภทและชื่อรถ
                        //          GTA.UI.Screen.ShowSubtitle("Categories : " + $"{category}", 10);

                        if (HUD_Demo)
                        {
                            // Outline
                            BackgroundTextureImage2.Draw();
                            BackgroundTextureImage.Draw();

                            if (isKMH)
                            {

                                switch (isUnitsLanguage)
                                {

                                    case 0:
                                        ImageKMH.Draw();
                                        break;
                                    case 1:
                                        ImageKMH_DNKNOR.Draw();
                                        break;
                                    case 2:
                                        ImageKMH_NLD.Draw();
                                        break;
                                    case 3:
                                        ImageKMH_RUS.Draw();
                                        break;
                                    case 4:
                                        ImageKMH_TUR.Draw();
                                        break;
                                    case 5:
                                        ImageKMH_UKR.Draw();
                                        break;
                                    case 6:
                                        ImageKMH_VNM.Draw();
                                        break;
                                }
                            }

                            if (isMPH)
                            {
                                switch (isUnitsLanguage)
                                {
                                    case 0:
                                        ImageMPH.Draw();
                                        break;
                                    case 1:
                                        ImageMPH.Draw();
                                        break;
                                    case 2:
                                        ImageMPH_NLD.Draw();
                                        break;
                                    case 3:
                                        ImageMPH_RUS.Draw();
                                        break;
                                    case 4:
                                        ImageMPH_TUR.Draw();
                                        break;
                                    case 5:
                                        ImageMPH_UKR.Draw();
                                        break;
                                    case 6:
                                        ImageMPH_VNM.Draw();
                                        break;
                                }
                            }


                            speedTextureImage.Draw();
                            

                            int rpm_2000 = 2000;
                            int rpm_2200 = 2200;
                            int rpm_2400 = 2400;
                            int rpm_2600 = 2600;
                            int rpm_2800 = 2800;
                            int rpm_3000 = 3000;
                            int rpm_4000 = 4000;
                            int rpm_4500 = 4500;
                            int rpm_5000 = 5000;
                            int rpm_5200 = 5200;
                            int rpm_5400 = 5400;
                            int rpm_5600 = 5600;
                            int rpm_5800 = 5800;
                            int rpm_6000 = 6000;
                            int rpm_6200 = 6200;
                            int rpm_6400 = 6400;
                            int rpm_6600 = 6600;
                            int rpm_6800 = 6800;
                            int rpm_7000 = 7000;
                            int rpm_7200 = 7200;
                            int rpm_7400 = 7400;
                            int rpm_7600 = 7600;
                            int rpm_7800 = 7800;
                            int rpm_8000 = 8000;
                            int rpm_8200 = 8200;
                            int rpm_8400 = 8400;
                            int rpm_8600 = 8600;
                            int rpm_8800 = 8800;
                            int rpm_9000 = 9000;
                            int rpm_9200 = 9200;
                            int rpm_9400 = 9400;
                            int rpm_9600 = 9600;
                            int rpm_9800 = 9800;
                            int rpm_10000 = 10000;
                            int rpm_10500 = 10500;
                            int rpm_11000 = 11000;
                            int rpm_11500 = 11500;
                            int rpm_12000 = 12000;
                            int rpm_12500 = 12500;
                            int rpm_13000 = 13000;
                            int rpm_13500 = 13500;
                            int rpm_14000 = 14000;
                            int rpm_14500 = 14500;
                            int rpm_15000 = 15000;
                            int rpm_15500 = 15500;
                            int rpm_16000 = 16000;
                            int rpm_16500 = 16500;
                            int rpm_17000 = 17000;
                            int rpm_18000 = 18000;
                            int rpm_19000 = 19000;

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

                            if (gearTexture != null)
                            {
                                gearTexture.Draw();
                            }

                            // Control.VehicleHandbrake corresponds to Spacebar on PC / R1 or RB on controllers
                            if (Game.IsControlPressed(GTA.Control.VehicleHandbrake))
                            {
                                HandbrakeTextureImage.Draw();
                            }

                            Vehicle veh = playerPed.CurrentVehicle;

                            /* Speed Text */

                            // คำนวณความเร็ว (ความเร็วปกติของเกมเป็น เมตร/วินาที คูณ 3.6 เพื่อแปลงเป็น กม./ชม.)
                            // หากต้องการเป็น ไมล์/ชั่วโมง (MPH) ให้คูณด้วย 2.236936f
                            float speedKmh = veh.Speed * isSwitchUnits;

                            // กำหนดเพดานความเร็วไม่ให้เกิน 1000 (ตามชื่อรูปภาพของคุณ)
                            if (speedKmh > 1000f) speedKmh = 1000f;
                            if (speedKmh < 0f) speedKmh = 0f;

                            // แปลงค่าความเร็วเป็นตัวเลข 4 หลัก เช่น 0000, 0120, 1000
                            int speedInt = (int)Math.Round(speedKmh);
                            string textureName = SpeedBaseTextureName + speedInt.ToString("D4");

                            // อัปเดตชื่อ Texture ปัจจุบันตามความเร็วรถ
                            speedTextureImage.Texture = textureName;


                            if (isRevEngineCurrent)
                            {
                                /* RPM */

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
                                    gearTexture = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), GearTextureDict, textureNameGear);
                                    gearTexture.Color = Color.FromArgb(A_GearColour, R_GearColour, G_GearColour, B_GearColour);

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
                            }
                        }

                        // Release HUD
                        if (HUD_Release)
                        {
                            // Outline
                            BackgroundTextureImage2_Release.Draw();
                            BackgroundTextureImage_Release.Draw();

                            if (isKMH)
                            {

                                switch (isUnitsLanguage)
                                {

                                    case 0:
                                        ImageKMH_Release.Draw();
                                        break;
                                    case 1:
                                        ImageKMH_DNKNOR_Release.Draw();
                                        break;
                                    case 2:
                                        ImageKMH_NLD_Release.Draw();
                                        break;
                                    case 3:
                                        ImageKMH_RUS_Release.Draw();
                                        break;
                                    case 4:
                                        ImageKMH_TUR_Release.Draw();
                                        break;
                                    case 5:
                                        ImageKMH_UKR_Release.Draw();
                                        break;
                                    case 6:
                                        ImageKMH_VNM_Release.Draw();
                                        break;
                                }
                            }

                            if (isMPH)
                            {
                                switch (isUnitsLanguage)
                                {
                                    case 0:
                                        ImageMPH_Release.Draw();
                                        break;
                                    case 1:
                                        ImageMPH_Release.Draw();
                                        break;
                                    case 2:
                                        ImageMPH_NLD_Release.Draw();
                                        break;
                                    case 3:
                                        ImageMPH_RUS_Release.Draw();
                                        break;
                                    case 4:
                                        ImageMPH_TUR_Release.Draw();
                                        break;
                                    case 5:
                                        ImageMPH_UKR_Release.Draw();
                                        break;
                                    case 6:
                                        ImageMPH_VNM_Release.Draw();
                                        break;
                                }
                            }


                            speedTextureImage_Release.Draw();

                            int rpm_2000 = 2000;
                            int rpm_2200 = 2200;
                            int rpm_2400 = 2400;
                            int rpm_2600 = 2600;
                            int rpm_2800 = 2800;
                            int rpm_3000 = 3000;
                            int rpm_4000 = 4000;
                            int rpm_4500 = 4500;
                            int rpm_5000 = 5000;
                            int rpm_5200 = 5200;
                            int rpm_5400 = 5400;
                            int rpm_5600 = 5600;
                            int rpm_5800 = 5800;
                            int rpm_6000 = 6000;
                            int rpm_6200 = 6200;
                            int rpm_6400 = 6400;
                            int rpm_6600 = 6600;
                            int rpm_6800 = 6800;
                            int rpm_7000 = 7000;
                            int rpm_7200 = 7200;
                            int rpm_7400 = 7400;
                            int rpm_7600 = 7600;
                            int rpm_7800 = 7800;
                            int rpm_8000 = 8000;
                            int rpm_8200 = 8200;
                            int rpm_8400 = 8400;
                            int rpm_8600 = 8600;
                            int rpm_8800 = 8800;
                            int rpm_9000 = 9000;
                            int rpm_9200 = 9200;
                            int rpm_9400 = 9400;
                            int rpm_9600 = 9600;
                            int rpm_9800 = 9800;
                            int rpm_10000 = 10000;
                            int rpm_10500 = 10500;
                            int rpm_11000 = 11000;
                            int rpm_11500 = 11500;
                            int rpm_12000 = 12000;
                            int rpm_12500 = 12500;
                            int rpm_13000 = 13000;
                            int rpm_13500 = 13500;
                            int rpm_14000 = 14000;
                            int rpm_14500 = 14500;
                            int rpm_15000 = 15000;
                            int rpm_15500 = 15500;
                            int rpm_16000 = 16000;
                            int rpm_16500 = 16500;
                            int rpm_17000 = 17000;
                            int rpm_18000 = 18000;
                            int rpm_19000 = 19000;

                            if (SetRPM >= rpm_2000 && SetRPM < rpm_2200)
                            {
                                LabelRPM_2000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_2200 && SetRPM < rpm_2400)
                            {
                                LabelRPM_2200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_2400 && SetRPM < rpm_2600)
                            {
                                LabelRPM_2400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_2600 && SetRPM < rpm_2800)
                            {
                                LabelRPM_2600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_2800 && SetRPM < rpm_3000)
                            {
                                LabelRPM_2800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_3000 && SetRPM < rpm_4000)
                            {
                                LabelRPM_3000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_4000 && SetRPM < rpm_4500)
                            {
                                LabelRPM_4000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_4500 && SetRPM < rpm_5000)
                            {
                                LabelRPM_4500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_5000 && SetRPM < rpm_5200)
                            {
                                LabelRPM_5000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_5200 && SetRPM < rpm_5400)
                            {
                                LabelRPM_5200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_5400 && SetRPM < rpm_5600)
                            {
                                LabelRPM_5400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_5600 && SetRPM < rpm_5800)
                            {
                                LabelRPM_5600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_5800 && SetRPM < rpm_6000)
                            {
                                LabelRPM_5800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_6000 && SetRPM < rpm_6200)
                            {
                                LabelRPM_6000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_6200 && SetRPM < rpm_6400)
                            {
                                LabelRPM_6200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_6400 && SetRPM < rpm_6600)
                            {
                                LabelRPM_6400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_6600 && SetRPM < rpm_6800)
                            {
                                LabelRPM_6600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_6800 && SetRPM < rpm_7000)
                            {
                                LabelRPM_6800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_7000 && SetRPM < rpm_7200)
                            {
                                LabelRPM_7000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_7200 && SetRPM < rpm_7400)
                            {
                                LabelRPM_7200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_7400 && SetRPM < rpm_7600)
                            {
                                LabelRPM_7400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_7600 && SetRPM < rpm_7800)
                            {
                                LabelRPM_7600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_7800 && SetRPM < rpm_8000)
                            {
                                LabelRPM_7800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_8000 && SetRPM < rpm_8200)
                            {
                                LabelRPM_8000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_8200 && SetRPM < rpm_8400)
                            {
                                LabelRPM_8200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_8400 && SetRPM < rpm_8600)
                            {
                                LabelRPM_8400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_8600 && SetRPM < rpm_8800)
                            {
                                LabelRPM_8600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_8800 && SetRPM < rpm_9000)
                            {
                                LabelRPM_8800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_9000 && SetRPM < rpm_9200)
                            {
                                LabelRPM_9000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_9200 && SetRPM < rpm_9400)
                            {
                                LabelRPM_9200_Release.Draw();
                            }
                            else if (SetRPM >= rpm_9400 && SetRPM < rpm_9600)
                            {
                                LabelRPM_9400_Release.Draw();
                            }
                            else if (SetRPM >= rpm_9600 && SetRPM < rpm_9800)
                            {
                                LabelRPM_9600_Release.Draw();
                            }
                            else if (SetRPM >= rpm_9800 && SetRPM < rpm_10000)
                            {
                                LabelRPM_9800_Release.Draw();
                            }
                            else if (SetRPM >= rpm_10000 && SetRPM < rpm_10500)
                            {
                                LabelRPM_10000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_10500 && SetRPM < rpm_11000)
                            {
                                LabelRPM_10500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_11000 && SetRPM < rpm_11500)
                            {
                                LabelRPM_11000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_11500 && SetRPM < rpm_12000)
                            {
                                LabelRPM_11500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_12000 && SetRPM < rpm_12500)
                            {
                                LabelRPM_12000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_12500 && SetRPM < rpm_13000)
                            {
                                LabelRPM_12500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_13000 && SetRPM < rpm_13500)
                            {
                                LabelRPM_13000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_13500 && SetRPM < rpm_14000)
                            {
                                LabelRPM_13500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_14000 && SetRPM < rpm_14500)
                            {
                                LabelRPM_14000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_14500 && SetRPM < rpm_15000)
                            {
                                LabelRPM_14500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_15000 && SetRPM < rpm_15500)
                            {
                                LabelRPM_15000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_15500 && SetRPM < rpm_16000)
                            {
                                LabelRPM_15500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_16000 && SetRPM < rpm_16500)
                            {
                                LabelRPM_16000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_16500 && SetRPM < rpm_17000)
                            {
                                LabelRPM_16500_Release.Draw();
                            }
                            else if (SetRPM >= rpm_17000 && SetRPM < rpm_18000)
                            {
                                LabelRPM_17000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_18000 && SetRPM < rpm_19000)
                            {
                                LabelRPM_18000_Release.Draw();
                            }
                            else if (SetRPM >= rpm_19000 && SetRPM < 20000)
                            {
                                LabelRPM_19000_Release.Draw();
                            }
                            else
                            {
                                LabelRPMDeafult_Release.Draw();
                            }

                            if (gearTexture_Release != null)
                            {
                                gearTexture_Release.Draw();
                            }

                            // Control.VehicleHandbrake corresponds to Spacebar on PC / R1 or RB on controllers
                            if (Game.IsControlPressed(GTA.Control.VehicleHandbrake))
                            {
                                HandbrakeTextureImage_Release.Draw();
                            }

                            Vehicle veh = playerPed.CurrentVehicle;

                            /* Speed Text */

                            // คำนวณความเร็ว (ความเร็วปกติของเกมเป็น เมตร/วินาที คูณ 3.6 เพื่อแปลงเป็น กม./ชม.)
                            // หากต้องการเป็น ไมล์/ชั่วโมง (MPH) ให้คูณด้วย 2.236936f
                            float speedKmh = veh.Speed * isSwitchUnits;

                            // กำหนดเพดานความเร็วไม่ให้เกิน 1000 (ตามชื่อรูปภาพของคุณ)
                            if (speedKmh > 1000f) speedKmh = 1000f;
                            if (speedKmh < 0f) speedKmh = 0f;

                            // แปลงค่าความเร็วเป็นตัวเลข 4 หลัก เช่น 0000, 0120, 1000
                            int speedInt = (int)Math.Round(speedKmh);
                            string textureName = SpeedBaseTextureName_Release + speedInt.ToString("D4");

                            // อัปเดตชื่อ Texture ปัจจุบันตามความเร็วรถ
                            speedTextureImage_Release.Texture = textureName;

                            if (isRevEngineCurrent)
                            {
                                /* RPM */

                                // ดึงค่า RPM จริงจากรถปัจจุบัน (0.0 ถึง 1.0)
                                float targetRpm = veh.CurrentRPM;

                                // สูตร LERP: ค่อยๆ ขยับ smoothedRpm เข้าหา targetRpm ทีละนิดในทุกเฟรม
                                smoothedRpm = smoothedRpm + (targetRpm - smoothedRpm) * interpolationSpeed;

                                // ส่งค่าสะสมที่นิ่งแล้ว ไปแปลงชื่อภาพ
                                // (ปรับ Min/Max เผื่อไว้ให้ด้วย: เดินเบา 0.2f สูงสุด 1.0f)
                                string RPMtextureName = GetTextureNameFromRange(smoothedRpm, 0.2f, 1.0f);

                                RevTextureImage_Release.Texture = RPMtextureName;
                                RevTextureImage_Release.Draw();
                            }

                            if (isABSHUD)
                            {
                                ABSTextureImage_Release.Draw();
                            }
                            if (isTCSHUD)
                            {
                                TCSTextureImage_Release.Draw();
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
                                    RevEngineOffImage_Release.Draw();
                                    isRevEngineCurrent = false;
                                }
                            }

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
                                    gearTexture_Release = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), GearTextureDict_Release, textureNameGear);
                                    gearTexture_Release.Color = Color.FromArgb(A_GearColour, R_GearColour, G_GearColour, B_GearColour);

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
                                gearTexture_Release = null;
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
                            isSpeedometerBackgroundDemo = false;
                        }
                        else
                        {
                            isSpeedometerBackgroundDemo = true;

                            // Ensure the player exists and is alive
                            if (playerPed != null && playerPed.Exists() && playerPed.IsAlive)
                            {
                                // Call the native hash function
                                // Arguments: Hash, Return Type, Input Parameters (the ped's handle)
                                bool isUsingPhone = Function.Call<bool>(Hash.IS_PED_RUNNING_MOBILE_PHONE_TASK, playerPed.Handle);

                                if (isUsingPhone)
                                {
                                    isSpeedometerBackgroundDemo = false;
                                }
                                else
                                {
                                    isSpeedometerBackgroundDemo = true;
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
                isSpeedometerBackgroundDemo = true;
                _wasInVehicle = true;
            }
            // Detect exit: Player is no longer in a vehicle, but was in one in the previous frame
            else if (!isInVehicle && _wasInVehicle)
            {
                isSpeedometerBackgroundDemo = false;
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
                            DriftTextureImage.Draw();
                        }
                    }
                }
            }
        }
        private string GetVehicleClassName(VehicleClass vClass)
        {
            switch (vClass)
            {
                case VehicleClass.Compacts: return "Compacts";
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
                default: return "ยานพาหนะทั่วไป";
            }
        }
    }
}
