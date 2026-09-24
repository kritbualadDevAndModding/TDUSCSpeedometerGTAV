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

        private string version = "0.3";
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
        private int lastGear = -1;
        private bool wasNeutral = false;

        // กำหนดชื่อไฟล์ .ytd ที่เราใส่ไว้ใน script_txds.rpf (ใส่แค่ชื่อ ไม่ต้องระบุนามสกุล .ytd)
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

        private int isUnitsLanguage;

        private float isSwitchUnits;

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

        private float smoothedRpm = 0f;
        private float interpolationSpeed = 0.15f;

        private ScaledTexture ABSTextureImage;
        private const string ABSTextureImageDict = "!TDUSCDemoHUDBackground";
        private const string ABSTextureImageName = "ABS";

        private ScaledTexture TCSTextureImage;
        private const string TCSTextureImageDict = "!TDUSCDemoHUDBackground";
        private const string TCSTextureImageName = "TCS";

        private int rpmMin = 0;
        private int rpmMax = 1799;

        private ScaledTexture LabelRPMDeafult;
        private const string LabelRPMDefaultDict = "!TDUSCDemoHUDBackground";
        private const string LabelRPMDefaultName = "rpm_9000";

        private ScaledTexture LabelRPMDeafultEV;
        private const string LabelRPMDefaultEVDict = "!TDUSCDemoHUDBackground";
        private const string LabelRPMDefaultEVName = "rpm_19000";


        private bool isEngineOn = false;


        private ScaledTexture TestEngineOff;
        private const string TestEngineOffDict = "!TDUSCDemoHUDBackground";
        private const string TestEngineOffName = "TurnLeft";

        private ScaledTexture TestEngineOn;
        private const string TestEngineOnDict = "!TDUSCDemoHUDBackground";
        private const string TestEngineOnName = "TurnRight";

        private ScaledTexture RevEngineOffImage;
        private const string RevEngineOffImageDict = "!TDUSCDemoHUDRev";
        private const string RevEngineOffImageName = "rev_0000";

        private int UseKMH;

        private float speedKMH = 3.6f;
        private float speedMPH = 2.236936f;

        private Keys keyOpenMenu;
        public TDUSCSpeedoGTAV()
        {
            // อ่านไฟล์ .ini
            config = ScriptSettings.Load("scripts\\TDUSCSpeedometerGTAV.ini");

            keyOpenMenu = config.GetValue<Keys>("Open_Menu", "MenuKey", keyOpenMenu);
            UIPositionX = config.GetValue<float>("Position_HUD", "PosX", UIPositionX);
            UIPositionY = config.GetValue<float>("Position_HUD", "PosY", UIPositionY);

            UISize = config.GetValue<float>("Size_HUD", "Scale", UISize);

            A_GearColour = config.GetValue<int>("Gear_Colour_RGB", "A", A_GearColour);
            R_GearColour = config.GetValue<int>("Gear_Colour_RGB", "R", R_GearColour);
            G_GearColour = config.GetValue<int>("Gear_Colour_RGB", "G", G_GearColour);
            B_GearColour = config.GetValue<int>("Gear_Colour_RGB", "B", B_GearColour);

            rpmMin = config.GetValue<int>("RPM_Settings", "RPMMin", rpmMin);
            rpmMax = config.GetValue<int>("RPM_Settings", "RPMMax", rpmMax);

            UseKMH = config.GetValue<int>("Units_Settings", "UseKMH", UseKMH);
            isUnitsLanguage = config.GetValue<int>("Units_Settings", "LanguageUnits", isUnitsLanguage);

            isABSHUD = config.GetValue<bool>("HUD_SETTINGS", "ABSHUD", isABSHUD);
            isTCSHUD = config.GetValue<bool>("HUD_SETTINGS", "TCSHUD", isTCSHUD);
            isMainHUDModEnabled = config.GetValue<bool>("HUD_SETTINGS", "Mod_Enabled", isMainHUDModEnabled);

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


            //         "kmh_DenmarkNorway" "kmh_Russian" "kmh_Turkish" "kmh_Ukriane" "kmh_Vietnam"
            // 3. Add the checkbox to the menu
            _mainMenu.AddItem(modEnabledHUD);
            _mainMenu.AddItem(UnitsListItem);
            _mainMenu.AddItem(LanguageListItem);
            _mainMenu.AddItem(ABSToggle);
            _mainMenu.AddItem(TCSToggle);

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
        
            ABSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ABSTextureImageDict, ABSTextureImageName);

            TCSTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TCSTextureImageDict, TCSTextureImageName);

            TestEngineOff = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TestEngineOffDict, TestEngineOffName);

            TestEngineOn = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), TestEngineOnDict, TestEngineOnName);

            RevEngineOffImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), RevEngineOffImageDict, RevEngineOffImageName);
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
                        LabelRPMDeafult.Draw();

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

                    else
                    {
                        // ถ้าอยู่นอกรถ ให้เข็มค่อยๆ ตกกลับไปที่ 0 อย่างนุ่มนวล
                        smoothedRpm = smoothedRpm + (0f - smoothedRpm) * interpolationSpeed;

                        //   _wasEngineRunning = false;
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
                                DriftHUD();
                            }
                        }
                    }
                }


                // Check if the player is alive and inside a vehicle
                if (playerPed != null && playerPed.IsAlive && playerPed.IsInVehicle())
                {
                    Vehicle veh = playerPed.CurrentVehicle;
                    int currentGear = veh.CurrentGear;
                    bool isNeutral = false;

                    // ลอจิกจำลองเกียร์ว่าง (Neutral) 
                    // ใน GTA V ค่าเกียร์ 1 จะถูกส่งมาตอนจอดนิ่งๆ ด้วย ดังนั้นเราจะเช็คเพิ่มว่าถ้ารถหยุดนิ่งและรอบเครื่องต่ำ ให้สลับเป็นภาพเกียร์ว่าง
                    if (currentGear == 1 && veh.Speed < 0.1f && veh.CurrentRPM < 0.3f)
                    {
                        isNeutral = true;
                    }

                    // อัปเดตรูปภาพเฉพาะเมื่อเกียร์เปลี่ยน หรือสถานะเกียร์ว่างเปลี่ยน เพื่อลดภาระการทำงาน
                    if (currentGear != lastGear || isNeutral != wasNeutral)
                    {
                        string textureName = GetGearTextureName(currentGear, isNeutral);

                        // เรียกใช้ LemonUI ดึงภาพตรงจาก script_txds.rpf
                        // พิกัดมุมขวาล่างจอ 16:9 (X: 1160, Y: 620) ขนาดรูปภาพ (56x56 พิกเซล)
                        gearTexture = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), GearTextureDict, textureName);
                        gearTexture.Color = Color.FromArgb(A_GearColour, R_GearColour, G_GearColour, B_GearColour);

                        lastGear = currentGear;
                        wasNeutral = isNeutral;
                    }
                }
                else
                {
                    // รีเซ็ตสถานะเมื่อลงจากรถ
                    lastGear = -1;
                    wasNeutral = false;
                    gearTexture = null;
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
    }
}
