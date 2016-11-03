using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShadowSocksUpdater
{
    class AutoRunConfig
    {

#if false

        public static void CreateDesktopLnk(string DeskShortPathandName, string TargetPath, string Arguments = "", string Description = "", string WorkingDirectory = "", string IconLocation = "", string HotKey = "", int WindowStyle = 1)
        {

            string DesktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);//得到桌面文件夹
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShellClass();

            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(DeskShortPathandName);
            shortcut.TargetPath = TargetPath;
            shortcut.Arguments = Arguments;// 参数
            shortcut.Description = Description;
            shortcut.WorkingDirectory = WorkingDirectory;//程序所在文件夹，在快捷方式图标点击右键可以看到此属性
            shortcut.IconLocation = IconLocation; // 图标 @"D:\software\cmpc\zy.exe,0"
            shortcut.Hotkey = HotKey;//热键
            shortcut.WindowStyle = WindowStyle;
            shortcut.Save();


        }

#endif

        public static bool GetAutoRunFromRegedit()
        {
            RegistryKey reg = Registry.LocalMachine;
            RegistryKey run = reg.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run");

            return run.GetValue("Shadowsocks") != null;

        }

        public static void EnableAutoRun(string path)
        {
            path = path.Replace("\\\\", @"\");

            if (!GetAutoRunFromRegedit())
            {
                RegistryKey reg = Registry.LocalMachine;
                RegistryKey run = reg.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run");
                run.SetValue("Shadowsocks", path);
            }

        }


        public static void DisableAutoRun()
        {
            if (GetAutoRunFromRegedit())
            {
                RegistryKey reg = Registry.LocalMachine;
                RegistryKey run = reg.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run");
                run.DeleteValue("Shadowsocks");
            }
        }

    }
}
