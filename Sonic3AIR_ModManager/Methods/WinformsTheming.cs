using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Sonic3AIR_ModManager
{
    public class WinformsTheming
    {

        public static Color darkTheme0 = Color.FromArgb(255, 40, 40, 40);
        public static Color darkTheme1 = Color.FromArgb(255, 50, 50, 50);
        public static Color darkTheme2 = Color.FromArgb(255, 70, 70, 70);
        public static Color darkTheme3 = Color.White;
        public static Color darkTheme4 = Color.FromArgb(255, 49, 162, 247);
        public static Color darkTheme5 = Color.FromArgb(255, 80, 80, 80);

        public class SystemColorsUtility
        {
            public SystemColorsUtility()
            {
                // force init color table
                byte unused = SystemColors.Window.R;

                var colorTableField = typeof(Color).Assembly.GetType("System.Drawing.KnownColorTable")
                    .GetField("colorTable", BindingFlags.Static | BindingFlags.NonPublic);

                _colorTable = (int[])colorTableField.GetValue(null);
            }

            public void SetColor(KnownColor knownColor, Color value)
            {
                _colorTable[(int)knownColor] = value.ToArgb();
            }

            private readonly int[] _colorTable;
        }

        public static void UseDarkTheme(bool state = false)
        {
            if (state)
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, darkTheme1);
                systemColors.SetColor(KnownColor.Highlight, Color.Blue);
                systemColors.SetColor(KnownColor.WindowFrame, darkTheme2);
                systemColors.SetColor(KnownColor.GradientActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ControlText, darkTheme3);
                systemColors.SetColor(KnownColor.WindowText, darkTheme3);
                systemColors.SetColor(KnownColor.GrayText, Color.Gray);
                systemColors.SetColor(KnownColor.InfoText, darkTheme3);
                systemColors.SetColor(KnownColor.MenuText, darkTheme3);
                systemColors.SetColor(KnownColor.Control, darkTheme1);
                systemColors.SetColor(KnownColor.ButtonHighlight, darkTheme3);
                systemColors.SetColor(KnownColor.ButtonShadow, darkTheme2);
                systemColors.SetColor(KnownColor.ButtonFace, darkTheme1);
                systemColors.SetColor(KnownColor.Desktop, darkTheme1);
                systemColors.SetColor(KnownColor.ControlLightLight, darkTheme2);
                systemColors.SetColor(KnownColor.ControlLight, darkTheme1);
                systemColors.SetColor(KnownColor.ControlDark, darkTheme3);
                systemColors.SetColor(KnownColor.ControlDarkDark, darkTheme3);
                systemColors.SetColor(KnownColor.ActiveBorder, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaption, darkTheme1);
                systemColors.SetColor(KnownColor.ActiveCaptionText, darkTheme3);
                systemColors.SetColor(KnownColor.InactiveBorder, darkTheme2);
                systemColors.SetColor(KnownColor.MenuBar, darkTheme1);
            }
            else
            {
                SystemColorsUtility systemColors = new SystemColorsUtility();
                systemColors.SetColor(KnownColor.Window, SystemColors.Window);
                systemColors.SetColor(KnownColor.Highlight, SystemColors.Highlight);
                systemColors.SetColor(KnownColor.WindowFrame, SystemColors.WindowFrame);
                systemColors.SetColor(KnownColor.GradientActiveCaption, SystemColors.GradientActiveCaption);
                systemColors.SetColor(KnownColor.GradientInactiveCaption, SystemColors.GradientInactiveCaption);
                systemColors.SetColor(KnownColor.ControlText, SystemColors.ControlText);
                systemColors.SetColor(KnownColor.WindowText, SystemColors.WindowText);
                systemColors.SetColor(KnownColor.GrayText, SystemColors.GrayText);
                systemColors.SetColor(KnownColor.InfoText, SystemColors.InfoText);
                systemColors.SetColor(KnownColor.MenuText, SystemColors.MenuText);
                systemColors.SetColor(KnownColor.Control, SystemColors.Control);
                systemColors.SetColor(KnownColor.ButtonHighlight, SystemColors.ButtonHighlight);
                systemColors.SetColor(KnownColor.ButtonShadow, SystemColors.ButtonShadow);
                systemColors.SetColor(KnownColor.ButtonFace, SystemColors.ButtonFace);
                systemColors.SetColor(KnownColor.Desktop, SystemColors.Desktop);
                systemColors.SetColor(KnownColor.ControlLightLight, SystemColors.ControlLightLight);
                systemColors.SetColor(KnownColor.ControlLight, SystemColors.ControlLight);
                systemColors.SetColor(KnownColor.ControlDark, SystemColors.ControlDark);
                systemColors.SetColor(KnownColor.ControlDarkDark, SystemColors.ControlDarkDark);
                systemColors.SetColor(KnownColor.ActiveBorder, SystemColors.ActiveBorder);
                systemColors.SetColor(KnownColor.ActiveCaption, SystemColors.ActiveCaption);
                systemColors.SetColor(KnownColor.ActiveCaptionText, SystemColors.ActiveCaptionText);
                systemColors.SetColor(KnownColor.InactiveBorder, SystemColors.InactiveBorder);
                systemColors.SetColor(KnownColor.MenuBar, SystemColors.MenuBar);
            }

        }

        public static void InvertGraphicsArea(Graphics g, System.Drawing.Rectangle r)
        {
            if (r.Height <= 0) { return; }
            if (r.Width <= 0) { return; }

            using (Bitmap bmpInvert = GetWhiteBitmap(g, r))
            {
                IntPtr hdcDest = g.GetHdc();
                using (Graphics src = Graphics.FromImage(bmpInvert))
                {
                    int xDest = r.Left;
                    int yDest = r.Top;
                    int nWidth = r.Width;
                    int nHeight = r.Height;
                    IntPtr hdcSrc = src.GetHdc();
                    BitBlt(hdcDest, xDest, yDest, nWidth, nHeight,
                           hdcSrc, 0, 0, (uint)CopyPixelOperation.DestinationInvert);
                    src.ReleaseHdc(hdcSrc);
                }
                g.ReleaseHdc(hdcDest);
            }
        }

        [DllImport("gdi32.dll",EntryPoint = "BitBlt",CallingConvention = CallingConvention.StdCall)]
        extern public static int BitBlt(
        IntPtr hdcDesc, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrcs, uint dwRop);

        private static Bitmap GetWhiteBitmap(Graphics g, Rectangle r)
        {
            int w = r.Width;
            int h = r.Height;

            Bitmap bmp = new Bitmap(w, h);
            using (Graphics gTmp = Graphics.FromImage(bmp))
            {
                gTmp.Clear(Color.White);
            }
            return bmp;
        }
    }
}
