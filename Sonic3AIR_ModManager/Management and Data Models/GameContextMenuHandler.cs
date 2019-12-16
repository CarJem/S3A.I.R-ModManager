using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.System.MouseKeyHook.Implementation;
using System.Windows.Controls;
using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;

namespace Sonic3AIR_ModManager
{
    public static class GameContextMenuHandler
    {
        #region Window Detector
        // The GetForegroundWindow function returns a handle to the foreground window
        // (the window  with which the user is currently working).
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // The GetWindowThreadProcessId function retrieves the identifier of the thread
        // that created the specified window and, optionally, the identifier of the
        // process that created the window.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private static bool IsAIRFocused()
        {
            IntPtr hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            if (hwnd == null)
                return false;

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (ProcessLauncher.CurrentGameProcess == null) return false;
                if (p.Id == ProcessLauncher.CurrentGameProcess.Id && pid == ProcessLauncher.CurrentGameProcess.Id)
                    return true;
            }

            return false;
        }

        class NativeMethods
        {
            // http://msdn.microsoft.com/en-us/library/ms633519(VS.85).aspx
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            // http://msdn.microsoft.com/en-us/library/a5ch4fda(VS.80).aspx
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }
        }

        private static System.Windows.Rect GetWindowRect(IntPtr hWnd)
        {
            NativeMethods.RECT rectangle = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(hWnd, ref rectangle);
            //Console.WriteLine("GetRect : {0},{1},{2},{3}", rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            int x = rectangle.Left;
            int y = rectangle.Top;
            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;
            return new System.Windows.Rect(x, y, width, height);
        }


        #endregion

        public static IKeyboardMouseEvents m_GlobalHook;

        public static void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
        }

        private static Controls.InGameContextMenu cm { get; set; }

        public static void CreateContextMenu()
        {
            if (cm == null)
            {
                cm = new Controls.InGameContextMenu();
                cm.StaysOpen = false;
            }
            else
            {
                if (cm.IsOpen) cm.IsOpen = false;
                cm.Reload();
            }

        }

        public static void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (ProcessLauncher.CurrentGameProcess != null && ProcessLauncher.CurrentGameProcess.HasExited == false)
            {
                System.Windows.Rect rectangle = (ProcessLauncher.CurrentGameProcess != null ? GetWindowRect(ProcessLauncher.CurrentGameProcess.MainWindowHandle) : new System.Windows.Rect());
                System.Windows.Point MousePos = new System.Windows.Point(e.Location.X, e.Location.Y);

                bool isWithin = (ProcessLauncher.CurrentGameProcess != null ? isWithinControl(rectangle, MousePos) : false);
                //Console.WriteLine("IsWithin : {0}", isWithin.ToString());
                //Console.WriteLine("Rect : {0}", rectangle.ToString());
                //Console.WriteLine("MousePos : {0}", MousePos.ToString());
                if (isWithin)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right && IsAIRFocused() && ProcessLauncher.isGameRunning)
                    {
                        CreateContextMenu();
                        cm.IsOpen = true;
                        cm.Focus();
                    }
                    else
                    {
                        bool isOutsideOfRange = !(cm != null ? IsWithinContextMenu(new System.Windows.Point(e.Location.X, e.Location.Y)) : true);
                        if (cm != null && cm.IsOpen && isOutsideOfRange)
                        {
                            cm.IsOpen = false;

                        }
                    }
                }
                else
                {
                    if (cm != null && cm.IsOpen)
                    {
                        cm.IsOpen = false;
                    }
                }
            }
            else
            {
                if (cm != null && cm.IsOpen)
                {
                    cm.IsOpen = false;
                }
            }

        }

        #region IsWithin Control Methods

        public static bool IsWithinContextMenu(System.Windows.Point mousePosition)
        {
            List<bool> IsWithin = new List<bool>();
            foreach (System.Windows.Controls.Control item in Extensions.FindVisualChildren<System.Windows.Controls.Control>(cm))
            {
                try
                {
                    if (item is MenuItem)
                    {
                        if (item.IsVisible)
                        {
                            var mItem = item as MenuItem;
                            bool isMainItem = (mItem.Equals(cm.airGuidesItem) || mItem.Equals(cm.airPlacesButton) || mItem.Equals(cm.airModManagerPlacesButton) || mItem.Equals(cm.airTipsItem));
                            IsWithin.Add(mItem.IsHighlighted && !(isMainItem));
                            if (isMainItem)
                            {
                                foreach (System.Windows.Controls.Control subItem in mItem.Items)
                                {
                                    if (subItem.IsVisible)
                                    {
                                        var contextMenuPosition = subItem.PointToScreen(new System.Windows.Point(0, 0));
                                        var contextMenuLW = new System.Windows.Point(subItem.ActualWidth, subItem.ActualHeight);
                                        IsWithin.Add(isWithinControl(contextMenuPosition, mousePosition, contextMenuLW.X, contextMenuLW.Y));
                                    }
                                    else
                                    {
                                        IsWithin.Add(false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            IsWithin.Add(false);
                        }
                    }
                    else
                    {
                        if (item.IsVisible)
                        {
                            var contextMenuPosition = item.PointToScreen(new System.Windows.Point(0, 0));
                            var contextMenuLW = new System.Windows.Point(item.ActualWidth, item.ActualHeight);
                            IsWithin.Add(isWithinControl(contextMenuPosition, mousePosition, contextMenuLW.X, contextMenuLW.Y));
                        }
                        else
                        {
                            IsWithin.Add(false);
                        }
                    }
                }
                catch
                {

                }

            }

            if (IsWithin.Contains(true)) return true;
            else return false;
        }

        public static bool isWithinControl(System.Windows.Point point, System.Windows.Point mousePosition, double width, double height)
        {
            System.Windows.Rect rect = new System.Windows.Rect(point.X, point.Y, width, height);
            if (!rect.Contains(mousePosition))
            {
                // Outside screen bounds.
                return false;
            }
            else return true;
        }

        public static bool isWithinControl(System.Windows.Rect rect, System.Windows.Point mousePosition)
        {
            if (!rect.Contains(mousePosition))
            {
                // Outside screen bounds.
                return false;
            }
            else return true;
        }

        #endregion

        public static void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            //m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }
    }
}
