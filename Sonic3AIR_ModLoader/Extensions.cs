using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace Sonic3AIR_ModManager
{
    public static class Extensions
    {

        [DllImport("gdi32.dll",
           EntryPoint = "BitBlt",
           CallingConvention = CallingConvention.StdCall)]
    extern public static int BitBlt(
    IntPtr hdcDesc, int nXDest, int nYDest, int nWidth, int nHeight,
    IntPtr hdcSrc, int nXSrc, int nYSrcs, uint dwRop);

    public static System.Drawing.Bitmap GetWhiteBitmap(System.Drawing.Graphics g, System.Drawing.Rectangle r)
    {
        int w = r.Width;
        int h = r.Height;

        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h);
        using (System.Drawing.Graphics gTmp = System.Drawing.Graphics.FromImage(bmp))
        {
            gTmp.Clear(System.Drawing.Color.White);
        }
        return bmp;
    }


    public static void InvertGraphicsArea(System.Drawing.Graphics g, System.Drawing.Rectangle r)
    {
        if (r.Height <= 0) { return; }
        if (r.Width <= 0) { return; }

        using (System.Drawing.Bitmap bmpInvert = GetWhiteBitmap(g, r))
        {
            IntPtr hdcDest = g.GetHdc();
            using (System.Drawing.Graphics src = System.Drawing.Graphics.FromImage(bmpInvert))
            {
                int xDest = r.Left;
                int yDest = r.Top;
                int nWidth = r.Width;
                int nHeight = r.Height;
                IntPtr hdcSrc = src.GetHdc();
                BitBlt(hdcDest, xDest, yDest, nWidth, nHeight,
                       hdcSrc, 0, 0, (uint)System.Drawing.CopyPixelOperation.DestinationInvert);
                src.ReleaseHdc(hdcSrc);
            }
            g.ReleaseHdc(hdcDest);
        }
    }
    public static string EvenColumns(int desiredWidth, IEnumerable<IEnumerable<string>> lists)
        {
            return string.Join(Environment.NewLine, EvenColumns(desiredWidth, true, lists));
        }

        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static bool ContainsOnly(this string haystack, params char[] needles)
        {
            foreach (char hay in haystack)
            {
                bool isAnythingBut = false;
                foreach (char needle in needles)
                {
                    if (hay.Equals(needle))
                    {
                        isAnythingBut = true;
                        break;
                    }
                }
                if (!isAnythingBut) return false;
            }

            return true;
        }

        public static IEnumerable<string> EvenColumns(int desiredWidth, bool rightOrLeft, IEnumerable<IEnumerable<string>> lists)
        {
            return lists.Select(o => EvenColumns(desiredWidth, rightOrLeft, o.ToArray()));
        }

        public static string EvenColumns(int desiredWidth, bool rightOrLeftAlignment, string[] list, bool fitToItems = false)
        {
            // right alignment needs "-X" 'width' vs left alignment which is just "X" in the `string.Format` format string
            int columnWidth = (rightOrLeftAlignment ? -1 : 1) *
                                // fit to actual items? this could screw up "evenness" if
                                // one column is longer than the others
                                // and you use this with multiple rows
                                (fitToItems
                                    ? Math.Max(desiredWidth, list.Select(o => o.Length).Max())
                                    : desiredWidth
                                );

            // make columns for all but the "last" (or first) one
            string format = string.Concat(Enumerable.Range(rightOrLeftAlignment ? 0 : 1, list.Length - 1).Select(i => string.Format("{{{0},{1}}}", i, columnWidth)));

            // then add the "last" one without Alignment
            if (rightOrLeftAlignment)
            {
                format += "{" + (list.Length - 1) + "}";
            }
            else
            {
                format = "{0}" + format;
            }

            return string.Format(format, list);
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {

            // exit if possitions are equal or outside array
            if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) ||
                (newIndex >= list.Count)) return;
            // local variables
            var i = 0;
            T tmp = list[oldIndex];
            // move element down and shift other elements up
            if (oldIndex < newIndex)
            {
                for (i = oldIndex; i < newIndex; i++)
                {
                    list[i] = list[i + 1];
                }
            }
            // move element up and shift other elements down
            else
            {
                for (i = oldIndex; i > newIndex; i--)
                {
                    list[i] = list[i - 1];
                }
            }
            // put element from position 1 to destination
            list[newIndex] = tmp;
        }

        public static IEnumerable<DirectoryInfo> VersionSort(this IEnumerable<DirectoryInfo> list)
        {
            int maxLen = list.Select(s => s.Name.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s.Name, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }

        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {

            // exit if possitions are equal or outside array
            if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) ||
                (newIndex >= list.Count)) return;
            // local variables
            var i = 0;
            T tmp = list[oldIndex];
            // move element down and shift other elements up
            if (oldIndex < newIndex)
            {
                for (i = oldIndex; i < newIndex; i++)
                {
                    list[i] = list[i + 1];
                }
            }
            // move element up and shift other elements down
            else
            {
                for (i = oldIndex; i > newIndex; i--)
                {
                    list[i] = list[i - 1];
                }
            }
            // put element from position 1 to destination
            list[newIndex] = tmp;
        }
    }
}
