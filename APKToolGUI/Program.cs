﻿using APKToolGUI.Languages;
using APKToolGUI.Properties;
using APKToolGUI.Utils;
using Bluegrams.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace APKToolGUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [DllImport("Shcore.dll")]
        static extern int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(String[] arg)
        {
            if (Environment.OSVersion.Version.Major == 6)
            {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (arg.Length == 1)
            {
                switch (arg[0])
                {
                    case "ccm":
                        ExplorerContextMenuMethod(ExplorerContextMenu.Action.Create);
                        break;
                    case "rcm":
                        ExplorerContextMenuMethod(ExplorerContextMenu.Action.Remove);
                        break;
                }
            }
            else
            {
                if (arg.Length == 2)
                {
                    switch (arg[0])
                    {
                        case "comapk":
                            if (!File.Exists(Path.Combine(arg[1], "AndroidManifest.xml")))
                            {
                                MessageBox.Show(Language.NotDecompiledApk, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            break;
                    }
                }
                if (FilesCheck() == true)
                {
                    Directory.CreateDirectory(TEMP_PATH);
                    PortableSettingsProvider.SettingsFileName = "config.xml";
                    PortableSettingsProvider.ApplyProvider(Settings.Default);
                    Application.Run(new FormMain());
                }
            }
        }

        public static void SetLanguage()
        {
            String settingsCulture = Settings.Default.Culture;

            if (settingsCulture.Equals("Auto"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InstalledUICulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InstalledUICulture;
            }
            else
            {
                System.Globalization.CultureInfo _settingsCulture = System.Globalization.CultureInfo.GetCultureInfo(settingsCulture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = _settingsCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = _settingsCulture;
            }
        }

        private static bool FilesCheck()
        {
            // проверка файлов
            List<String> missigFiles = MissingFilesCheck();
            if (missigFiles.Count > 0)
            {
                String files = Environment.NewLine;
                foreach (String file in missigFiles)
                {
                    files += file + Environment.NewLine;
                }
                MessageBox.Show(Language.RequiredFilesMissing + files, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
                return false;
            }
            else
                return true;
        }

        private static List<String> MissingFilesCheck()
        {
            List<String> missingFiles = new List<string>();
            String[] fileList = new String[]{
                APKTOOL_PATH,
                ZIPALIGN_PATH,
                APKSIGNER_PATH,
                BAKSMALI_PATH,
                SMALI_PATH,
                AAPT_PATH,
                AAPT2_PATH,
                SIGNAPK_KEYPRIVATE,
                SIGNAPK_KEYPUBLIC,
                AAPT2JNI_PATH
            };
            for (int i = 0; i < fileList.Length; i++)
                if (!File.Exists(fileList[i]))
                    missingFiles.Add(Path.GetFileName(fileList[i]));
            return missingFiles;
        }

        private static void ExplorerContextMenuMethod(ExplorerContextMenu.Action action)
        {
            ExplorerContextMenu.Status status = null;

            switch (action)
            {
                case ExplorerContextMenu.Action.Create:
                    status = ExplorerContextMenu.Create();
                    break;
                case ExplorerContextMenu.Action.Remove:
                    status = ExplorerContextMenu.Remove();
                    break;
                default:
                    return;
            }

            if (status.Result)
                MessageBox.Show(status.Message, Application.ProductName);
            else
                MessageBox.Show(status.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        public static string GetPortablePath(string path)
        {
            string startupPath = Application.StartupPath + Path.DirectorySeparatorChar;
            if (path.Contains(startupPath))
                return path.Replace(startupPath, String.Empty);
            else
                return path;
        }

        public static string TempDir()
        {
            if (Settings.Default.UseCustomTempDir)
                return Path.Combine(Settings.Default.TempDir, "APKToolGUI" + StringExt.RandStrWithCaps(4));
            else
                return Path.Combine(Path.GetTempPath(), "APKToolGUI" + StringExt.RandStrWithCaps(4));
        }

        public static string LOCAL_APPDATA_PATH = Environment.GetEnvironmentVariable("LocalAppData");
        public static string TEMP_PATH = TempDir();
        public static string APP_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string RES_PATH = Path.Combine(APP_PATH, "Resources");

        public static string APKTOOL_PATH = Path.Combine(RES_PATH, "apktool.jar");
        public static string APKSIGNER_PATH = Path.Combine(RES_PATH, "apksigner.jar");
        public static string BAKSMALI_PATH = Path.Combine(RES_PATH, "baksmali.jar");
        public static string SMALI_PATH = Path.Combine(RES_PATH, "smali.jar");
        public static string SIGNAPK_KEYPRIVATE = Path.Combine(RES_PATH, "testkey.pk8");
        public static string SIGNAPK_KEYPUBLIC = Path.Combine(RES_PATH, "testkey.x509.pem");
        public static string ZIPALIGN_PATH = Path.Combine(RES_PATH, "zipalign.exe");
        public static string AAPT_PATH = Path.Combine(RES_PATH, "aapt.exe");
        public static string AAPT2_PATH = Path.Combine(RES_PATH, "aapt2.exe");
        public static string AAPT2JNI_PATH = Path.Combine(RES_PATH, "libaapt2_jni.dll");
        public static string FRAMEWORK_DIR = Path.Combine(LOCAL_APPDATA_PATH, "apktool", "framework");
    }
}
