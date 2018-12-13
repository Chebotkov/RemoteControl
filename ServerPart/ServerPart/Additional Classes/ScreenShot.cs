using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ServerPart
{ 
    public static class ScreenShot
    {
        public static Image GetScreenShot()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics graphics = Graphics.FromImage(printscreen as Image);

            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            return printscreen;
        }
    }
}
