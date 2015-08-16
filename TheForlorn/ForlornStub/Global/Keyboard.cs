// https://code.google.com/p/csharpmlib/source/browse/trunk/MLib/InputDevices/Keyboard.cs

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MLib.InputDevices
{
    /// <summary>
    /// Compilation of Keyboard functions
    /// </summary>
    public static class Keyboard
    {

        #region SendString Class
        class SendString
        {


            [DllImport("user32.dll")]
            static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

            [StructLayout(LayoutKind.Explicit)]
            struct INPUT
            {
                [FieldOffset(0)]
                int type;
                [FieldOffset(4)]
                MOUSEINPUT mi;
                [FieldOffset(4)]
                KEYBDINPUT ki;
                [FieldOffset(4)]
                HARDWAREINPUT hi;
            }

            struct MOUSEINPUT
            {
                int dx;
                int dy;
                int mouseData;
                int dwFlags;
                int time;
                IntPtr dwExtraInfo;
            }

            struct KEYBDINPUT
            {
                public char wVk;
                public short wScan;
                public int dwFlags;
                public int time;
                public IntPtr dwExrtaInfo;
            }

            struct HARDWAREINPUT
            {
                int uM;
                short wParamL;
                short wParamH;
            }

            static bool WindowSet = false;
            static public void SetWindow(string Name)
            {
                int i = SendString.FindWindow(Name, null);
                SetForegroundWindow(i);
                WindowSet = true;
            }

            static public void SetHandle(int Handle)
            {
                SetForegroundWindow(Handle);
                WindowSet = true;
            }

            static public void Send(string String)
            {
                for (int j = 0; j < String.Length; j++)
                {
                    PressKey((char)String[j]);
                }
            }

            [DllImport("user32.dll")]
            static extern short VkKeyScan(char ch);

            [DllImport("user32.dll")]
            static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

            [DllImport("user32.dll", EntryPoint = "FindWindow")]
            private static extern int FindWindow(string _ClassName, string _WindowName);

            [DllImport("User32.dll")]
            private static extern Int32 SetForegroundWindow(int hWnd);

            const int KEYEVENTF_KEYUP = 0x2;

            public static void PressKey(char keyCode)
            {
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event((byte)VkKeyScan(keyCode), 0x9e, (uint)0, (UIntPtr)0);
                keybd_event((byte)VkKeyScan(keyCode), 0x9e,
                (uint)KEYEVENTF_KEYUP, (UIntPtr)0);
            }



            public static void HoldKeyHandle(char keyCode)
            {
                keybd_event((byte)VkKeyScan(keyCode), 0x9e, (uint)0, (UIntPtr)0);

            }
            public static void HoldKeyHandle(KeyList keyCode)
            {
                keybd_event((byte)keyCode, 0x9e, (uint)0, (UIntPtr)0);

            }


            public static void ReleaseKeyHandle(char keyCode)
            {
                keybd_event((byte)VkKeyScan(keyCode), 0x9e,
                (uint)KEYEVENTF_KEYUP, (UIntPtr)0);
            }
            public static void ReleaseKeyHandle(KeyList keyCode)
            {
                keybd_event((byte)keyCode, 0x9e,
                (uint)KEYEVENTF_KEYUP, (UIntPtr)0);
            }



            public static void PressKeyHandle(KeyList keyCode)
            {
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event((byte)keyCode, 0x9e, (uint)0, (UIntPtr)0);
                keybd_event((byte)keyCode, 0x9e,
                (uint)KEYEVENTF_KEYUP, (UIntPtr)0);
            }
        }
        #endregion

        #region Win32
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void ReserveKey(IntPtr Window, KeyList KeyOne, KeyList KeyTwo)
        {
            RegisterHotKey(Window, 100, (uint)KeyOne, (uint)KeyOne);
        }

        #endregion

        /// <summary>
        /// Reserves a Key for a specific window
        /// </summary>
        /// <param name="Window">Window that will </param>
        public static void UnreserverKey(IntPtr Window)
        {
            UnregisterHotKey(Window, 100);
        }
        /// <summary>
        /// List of the special keys you that can use
        /// </summary>
        public enum KeyList : ushort
        {
            ALT = 0x01,
            SHIFT = 0x10,
            CONTROL = 0x11,
            MENU = 0x12,
            ESCAPE = 0x1B,
            BACK = 0x08,
            TAB = 0x09,
            RETURN = 0x0D,
            PRIOR = 0x21,
            NEXT = 0x22,
            END = 0x23,
            HOME = 0x24,
            LEFT = 0x25,
            UP = 0x26,
            RIGHT = 0x27,
            DOWN = 0x28,
            SELECT = 0x29,
            PRINT = 0x2A,
            EXECUTE = 0x2B,
            SNAPSHOT = 0x2C,
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,
            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 0x64,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            MULTIPLY = 0x6A,
            ADD = 0x6B,
            SEPARATOR = 0x6C,
            SUBTRACT = 0x6D,
            DECIMAL = 0x6E,
            DIVIDE = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            OEM_1 = 0xBA,   // ',:' for US
            OEM_PLUS = 0xBB,   // '+' any country
            OEM_COMMA = 0xBC,   // ',' any country
            OEM_MINUS = 0xBD,   // '-' any country
            OEM_PERIOD = 0xBE,   // '.' any country
            OEM_2 = 0xBF,   // '/?' for US
            OEM_3 = 0xC0,   // '`~' for US
            MEDIA_NEXT_TRACK = 0xB0,
            MEDIA_PREV_TRACK = 0xB1,
            MEDIA_STOP = 0xB2,
            MEDIA_PLAY_PAUSE = 0xB3,
            LWIN = 0x5B,
            RWIN = 0x5C
        }


        /// <summary>
        /// Simulates a number of keypresses in the current active application
        /// </summary>
        /// <param name="Keys">Keys to be simulated</param>
        public static void SimulateKeyPresses(string Keys)
        {
            SendString.Send(Keys);
        }

        /// <summary>
        /// Simulates a special key press in the current active application
        /// </summary>
        /// <param name="Key">Keypress that will be simulated</param>
        public static void SimulateKeyPresses(KeyList Key)
        {
            SendString.PressKeyHandle(Key);
        }

        public static void ReleaseKey(KeyList Key)
        {
            SendString.ReleaseKeyHandle(Key);
        }

        public static void ReleaseKey(string Keys)
        {
            for (int j = 0; j < Keys.Length; j++)
            {
                SendString.ReleaseKeyHandle((char)Keys[j]);
            }
        }




        public static void HoldKey(KeyList Key)
        {
            SendString.HoldKeyHandle(Key);
        }
        public static void HoldKey(string Keys)
        {
            for (int j = 0; j < Keys.Length; j++)
            {
                SendString.HoldKeyHandle((char)Keys[j]);
            }
        }


        #region SendToApplication


        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern byte VkKeyScan(char ch);
        const int WM_KEYUP = 0x101;
        const uint WM_KEYDOWN = 0x100;

        public static void SendToApplication(Process Proc, IntPtr WindowHandle, KeyList Key)
        {
            PostMessage(WindowHandle, WM_KEYDOWN, (byte)Key, 0);
            Thread.Sleep(50);
            PostMessage(WindowHandle, WM_KEYUP, (byte)Key, 0);
        }

        public static void HoldInApplication(Process Proc, IntPtr WindowHandle, KeyList Key)
        {
            PostMessage(WindowHandle, WM_KEYDOWN, (byte)Key, 0);
        }
        public static void ReleaseInApplication(Process Proc, IntPtr WindowHandle, KeyList Key)
        {
            PostMessage(WindowHandle, WM_KEYUP, (byte)Key, 0);
        }

        public static void HoldInApplication(Process Proc, IntPtr WindowHandle, string Text)
        {
            for (int i = 0; i < Text.Length; i++)
                PostMessage(WindowHandle, WM_KEYDOWN, VkKeyScan(Text[i]), 0);
        }
        public static void ReleaseInApplication(Process Proc, IntPtr WindowHandle, string Text)
        {
            for (int i = 0; i < Text.Length; i++)
                PostMessage(WindowHandle, WM_KEYUP, VkKeyScan(Text[i]), 0);
        }



        public static void SendToApplication(Process Proc, IntPtr WindowHandle, string Text)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                PostMessage(WindowHandle, WM_KEYDOWN, VkKeyScan(Text[i]), 0);
                Thread.Sleep(50);
                PostMessage(WindowHandle, WM_KEYUP, VkKeyScan(Text[i]), 0);
            }
        }
        #endregion
    }
}