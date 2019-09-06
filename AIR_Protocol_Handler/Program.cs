using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace AIR_Protocol_Handler
{
    public static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Sonic 3 A.I.R Mod Manager.exe";
                ManageProtocalHandlerUI(exePath);
            }
            else
            {
                ManageProtocalHandlerUI(args[1]);
            }

        }

        public static string ModLoaderPath = "";
        public enum HandlerOption : int
        {
            Install = 1,
            Uninstall = 2,
            Update = 3
        }

        public static void ManageProtocalHandlerUI(string exePath)
        {
            ModLoaderPath = exePath;
            bool isInstalled = DoesMainEntryExist();
            bool isRightPath = DoesMainEntryMatchInstall();
            HandlerManagementUI ui = new HandlerManagementUI(isInstalled, isRightPath, exePath);
            switch (ui.ShowDialog())
            {
                case DialogResult.Yes:
                    AddProtocalHandler();
                    break;
                case DialogResult.No:
                    RemoveProtocalHandler();
                    break;
                case DialogResult.Retry:
                    UpdateProtocalHandler();
                    break;
            }
        }

        public static void ManageProtocalHandler(HandlerOption option)
        {
            switch (option)
            {
                case HandlerOption.Install:
                    AddProtocalHandler();
                    break;
                case HandlerOption.Uninstall:
                    RemoveProtocalHandler();
                    break;
                case HandlerOption.Update:
                    UpdateProtocalHandler();
                    break;
            }
        }

        public static void AddProtocalHandler()
        {
            CreateKeys();
        }

        public static bool DoesMainEntryExist()
        {
            string Protocol = "S3AIRMM";

            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM*/
            RegistryKey Key = Registry.ClassesRoot.OpenSubKey(Protocol, true);
            if (Key == null)
            {
                return false;
            }
            else
            {
                Key.Close();
                return true;
            }
        }

        public static bool DoesMainEntryMatchInstall()
        {
            string Protocol = "S3AIRMM";

            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM*/
            RegistryKey Key = Registry.ClassesRoot.OpenSubKey(Protocol, true);
            if (Key == null)
            {
                return false;
            }
            else
            {
                if (Key.GetValue("Mod Loader Path") != null)
                {
                    string EntryPath = Key.GetValue("Mod Loader Path").ToString();
                    if (EntryPath == ModLoaderPath) return true;
                    else return false;
                }
                else return false;

            }
        }

        public static void CreateKeys()
        {
            string Protocol = "S3AIRMM";
            string ProtocolName = "Sonic 3 A.I.R Mod Manager Protocol";


            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM*/
            RegistryKey Key = Registry.ClassesRoot.OpenSubKey(Protocol, true);
            if (Key == null) Key = Registry.ClassesRoot.CreateSubKey(Protocol);

            /*URL Protocol*/
            Key.SetValue("", "URL:" + ProtocolName);
            Key.SetValue("URL Protocol", "");
            Key.SetValue("Mod Loader Path", ModLoaderPath);
            RegistryKey PrevKey = Key;

            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM\shell*/
            Key = Key.OpenSubKey("shell", true);
            if (Key == null) Key = PrevKey.CreateSubKey("shell");
            PrevKey = Key;

            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM\shell\open*/
            Key = Key.OpenSubKey("open", true);
            if (Key == null) Key = PrevKey.CreateSubKey("open");
            PrevKey = Key;

            /*Computer\HKEY_CLASSES_ROOT\S3AIRMM\shell\open\command*/
            Key = Key.OpenSubKey("command", true);
            if (Key == null) Key = PrevKey.CreateSubKey("command");

            /*Command for GB API*/
            Key.SetValue("", $"\"{ModLoaderPath}\" \"-g\" \"%1\"");
            Key.Close();
        }

        public static void RemoveProtocalHandler()
        {
            string keyName = @"S3AIRMM";
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(keyName, true);
            if (key != null)
            {
                key.Close();
                Registry.ClassesRoot.DeleteSubKeyTree(keyName, true);
            }
        }

        public static void UpdateProtocalHandler()
        {
            RemoveProtocalHandler();
            CreateKeys();
        }
    }
}
