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
using System.Windows.Forms;

namespace TDUSCSpeedometerGTAV
{
    public class TDUSCSpeedoGTAV : Script
    {

        private string version = "0.2";
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
        private bool isMainHUDModEnabled = true;
        private bool isSpeedometerBackgroundDemo;
        private bool isRevEngineCurrent;
        private bool isABSHUD = true;
        private bool isTCSHUD = true;

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

        public TDUSCSpeedoGTAV()
        {
            // อ่านไฟล์ .ini
            config = ScriptSettings.Load("scripts\\TDUSCSpeedometerGTAV.ini");
            UIPositionX = config.GetValue<float>("PositionHUD", "PosX", UIPositionX);
            UIPositionY = config.GetValue<float>("PositionHUD", "PosY", UIPositionY);

            UISize = config.GetValue<float>("SizeHUD", "Scale", UISize);

            A_GearColour = config.GetValue<int>("GearColourRGB", "A", A_GearColour);
            R_GearColour = config.GetValue<int>("GearColourRGB", "R", R_GearColour);
            G_GearColour = config.GetValue<int>("GearColourRGB", "G", G_GearColour);
            B_GearColour = config.GetValue<int>("GearColourRGB", "B", B_GearColour);

            rpmMin = config.GetValue<int>("RPMSettings", "RPMMin", rpmMin);
            rpmMax = config.GetValue<int>("RPMSettings", "RPMMax", rpmMax);

            _menuPool = new MenuPool();
            _mainMenu = new UIMenu("TDUSCSpeedoV", "Version " + version + " by " + credits);
            _menuPool.Add(_mainMenu);

            // 2. Create the Checkbox Item (Boolean toggle)
            UIMenuCheckboxItem modEnabledHUD = new UIMenuCheckboxItem("Mod Enabled", true, "Mod Show and Hide HUD for Test Drive Unlimited Solar Crown Speedometer");
            UIMenuCheckboxItem ABSToggle = new UIMenuCheckboxItem("ABS HUD", isABSHUD);
            UIMenuCheckboxItem TCSToggle = new UIMenuCheckboxItem("TCS HUD", isTCSHUD);

            // 3. Add the checkbox to the menu
            _mainMenu.AddItem(modEnabledHUD);
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
                    }
                    else
                    {
                        isMainHUDModEnabled = false;
                    }
                }
                if (item == ABSToggle)
                {
                    if (Checked)
                    {
                        isABSHUD = true;
                    }
                    else
                    {
                        isABSHUD = false;
                    }
                }
                if (item == TCSToggle)
                {
                    if (Checked)
                    {
                        isTCSHUD = true;
                    }
                    else
                    {
                        isTCSHUD = false;
                    }
                }

            };

            Tick += OnTick;
            KeyUp += OnKeyDown;



            BackgroundTextureImage = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict, BackgroundTextureName);

            BackgroundTextureImage2 = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), BackgroundTextureDict2, BackgroundTextureName2);

            ImageKMH = new ScaledTexture(new PointF(UIPositionX, UIPositionY), new SizeF(UISize, UISize), ImageKMHDict, ImageKMHName);

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
            //   SpeedTextDemo();
            TDUSC_HUDDemoTick();
            VehicleSlowMotionFranklin();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Press the F5 key to toggle the menu open and closed
            if (e.KeyCode == Keys.K)
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
                        BackgroundTextureImage2.Draw();
                        BackgroundTextureImage.Draw();
                        ImageKMH.Draw();
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
                        float speedKmh = veh.Speed * 3.6f;

                        // กำหนดเพดานความเร็วไม่ให้เกิน 1000 (ตามชื่อรูปภาพของคุณ)
                        if (speedKmh > 1000f) speedKmh = 1000f;
                        if (speedKmh < 0f) speedKmh = 0f;

                        // แปลงค่าความเร็วเป็นตัวเลข 4 หลัก เช่น 0000, 0120, 1000
                        int speedInt = (int)Math.Round(speedKmh);
                        string textureName = SpeedBaseTextureName + speedInt.ToString("D4");

                        // อัปเดตชื่อ Texture ปัจจุบันตามความเร็วรถ
                        speedTextureImage.Texture = textureName;


                        if(isRevEngineCurrent)
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

                        if(isABSHUD)
                        {
                            ABSTextureImage.Draw();
                        }
                        if(isTCSHUD)
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
