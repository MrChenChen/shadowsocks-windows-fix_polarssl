using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;

namespace Shadowsocks._3rd
{
    class CreateDesktopShort
    {
        public static readonly string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);


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
    }
}
