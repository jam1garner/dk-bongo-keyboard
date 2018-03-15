using System;
using System.Linq;
using HidLibrary;
using System.Windows.Forms;

namespace DkBongoKeyboard
{
    class Program
    {
        private const int VendorId = 0x0079;
        private static readonly int[] ProductIds = new[] { MessageFactory.DkBongoId };
        private static int _currentProductId;
        private static HidDevice _device;
        private static bool _attached;

        const string lowercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string uppercase = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*()_-+=[]{};:'\",./<>|\\";

        private static void Press(string key)
        {
            try
            {
                SendKeys.SendWait(key);
            }
            catch { } // Catch random crash and just fail I guess?
        }

        class ButtonsPressed
        {
            public static bool rightBongoTopPressed;
            public static bool rightBongoBottomPressed;
            public static bool leftBongoBottomPressed;
            public static bool leftBongoTopPressed;

            public static bool startPressed;

            public static bool micClap;

            public static DkBongoMessage lastMessage = new DkBongoMessage();

            public static void Update(DkBongoMessage dk)
            {
                rightBongoTopPressed = !lastMessage.rightBongoTopPressed && dk.rightBongoTopPressed;
                rightBongoBottomPressed = !lastMessage.rightBongoBottomPressed && dk.rightBongoBottomPressed;
                leftBongoBottomPressed = !lastMessage.leftBongoBottomPressed && dk.leftBongoBottomPressed;
                leftBongoTopPressed = !lastMessage.leftBongoTopPressed && dk.leftBongoTopPressed;

                startPressed = (lastMessage.startPressed ^ dk.startPressed) && dk.startPressed;

                micClap = (lastMessage.micLevel < 0x40 && dk.micLevel >= 0x40);

                lastMessage = dk;
            }
        }

        class ButtonsDepressed
        {
            public static bool rightBongoTopPressed;
            public static bool rightBongoBottomPressed;
            public static bool leftBongoBottomPressed;
            public static bool leftBongoTopPressed;

            public static bool startPressed;

            public static bool micClap;

            public static DkBongoMessage lastMessage = new DkBongoMessage();

            public static void Update(DkBongoMessage dk)
            {
                rightBongoTopPressed = lastMessage.rightBongoTopPressed && !dk.rightBongoTopPressed;
                rightBongoBottomPressed = lastMessage.rightBongoBottomPressed && !dk.rightBongoBottomPressed;
                leftBongoBottomPressed = lastMessage.leftBongoBottomPressed && !dk.leftBongoBottomPressed;
                leftBongoTopPressed = lastMessage.leftBongoTopPressed && !dk.leftBongoTopPressed;

                startPressed = (lastMessage.startPressed ^ dk.startPressed) && dk.startPressed;

                micClap = (lastMessage.micLevel < 0x40 && dk.micLevel >= 0x40);

                lastMessage = dk;
            }
        }

        static int mode = -1;
        static bool shiftHeld;
        static int currentChar = 0;

        static string GetCharacterSet()
        {
            switch (mode)
            {
                case 0:
                    return lowercase;
                case 1:
                    return numbers;
                case 2:
                    return symbols;
                default:
                    return lowercase;
            }
        }

        static void Main()
        {
            foreach (var productId in ProductIds)
            {
                _device = HidDevices.Enumerate(VendorId, productId).FirstOrDefault();

                if (_device == null) continue;

                _currentProductId = productId;

                _device.OpenDevice();

                _device.Inserted += DeviceAttachedHandler;
                _device.Removed += DeviceRemovedHandler;

                _device.MonitorDeviceEvents = true;

                

                _device.ReadReport(OnReport);
                break;
            }

            if (_device != null)
            {
                Console.WriteLine("Gamepad found, press any key to exit.");
                Console.ReadKey();
                _device.CloseDevice();
            }
            else
            {
                Console.WriteLine("Could not find a gamepad.");
                Console.ReadKey();
            }
        }

        private static void UpdateCharacter()
        {
            Press("{BACKSPACE}" + GetCharacterSet().Substring(currentChar, 1));
            
        }

        private static void OnReport(HidReport report)
        {
            if (_attached == false) { return; }

            if (report.Data.Length >= 4)
            {
                var message = MessageFactory.CreateMessage(_currentProductId, report.Data);

                ButtonsPressed.Update(message);
                ButtonsDepressed.Update(message);

                /*if (ButtonsPressed.leftBongoBottomPressed)
                    Console.WriteLine("Left Bongo Bottom Pressed");
                if (ButtonsPressed.leftBongoTopPressed)
                    Console.WriteLine("Left Bongo Top Pressed");
                if (ButtonsPressed.rightBongoBottomPressed)
                    Console.WriteLine("Right Bongo Bottom Pressed");
                if (ButtonsPressed.rightBongoTopPressed)
                    Console.WriteLine("Right Bongo Top Pressed");
                if (ButtonsPressed.startPressed)
                    Console.WriteLine("Start Pressed");*/
                
                if (mode == -1 && message.startPressed)
                {
                    mode = 0;
                    currentChar = 0;
                    Press("A");
                } 
                else if (mode != -1)
                {
                    string charSet = GetCharacterSet();

                    if (ButtonsPressed.startPressed)
                    {
                        mode++;
                        mode %= 3;
                        currentChar = 0;
                        UpdateCharacter();
                    }

                    if (ButtonsPressed.leftBongoBottomPressed)
                    {
                        Press("{BACKSPACE}");
                    }

                    if (ButtonsPressed.leftBongoTopPressed)
                    {
                        currentChar = 0;
                        Press(charSet.Substring(currentChar, 1));
                    }

                    if (ButtonsPressed.rightBongoBottomPressed)
                    {
                        currentChar = ((currentChar - 1) + charSet.Length) % charSet.Length;
                        UpdateCharacter();
                    }

                    if (ButtonsPressed.rightBongoTopPressed)
                    {
                        currentChar = ((currentChar + 1) + charSet.Length) % charSet.Length;
                        UpdateCharacter();
                    }

                    if (ButtonsPressed.micClap)
                    {
                        Console.WriteLine("Clapped");
                        Press(" ");
                    }
                }
            }

            System.Threading.Thread.Sleep(1000/30);
            _device.ReadReport(OnReport);
        }

        private static void KeyPressed(int value)
        {
            Console.WriteLine("Button {0} pressed.", value);
        }

        private static void KeyDepressed()
        {
            Console.WriteLine("Button depressed.");
        }

        private static void DeviceAttachedHandler()
        {
            _attached = true;
            Console.WriteLine("Gamepad attached.");
            
            _device.ReadReport(OnReport);
        }

        private static void DeviceRemovedHandler()
        {
            _attached = false;
            Console.WriteLine("Gamepad removed.");
        }
    }
}
