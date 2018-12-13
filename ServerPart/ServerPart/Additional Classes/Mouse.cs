using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ServerPart
{
    public static class Mouse
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(MouseEvent dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        public enum MouseEvent
        {
            MOUSEEVENTF_LEFTDOWN = 0x02,
            MOUSEEVENTF_LEFTUP = 0x04,
            MOUSEEVENTF_RIGHTDOWN = 0x08,
            MOUSEEVENTF_RIGHTUP = 0x10,
        }

        public static void Click(Buttons button)
        {
            switch (button)
            {
                case Buttons.leftbutton:
                    {
                        mouse_event(MouseEvent.MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0, 0);
                        mouse_event(MouseEvent.MOUSEEVENTF_LEFTUP, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0, 0);
                        break;
                    }
                case Buttons.rightbutton:
                    {
                        mouse_event(MouseEvent.MOUSEEVENTF_RIGHTDOWN, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0, 0);
                        mouse_event(MouseEvent.MOUSEEVENTF_RIGHTUP, System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 0, 0);
                        break;
                    }
            }
        }
    }
}
