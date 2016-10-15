using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShadowSocksUpdater
{
    class Program
    {
        const int EnabelAutoRun = 0;
        const int DisabelAutoRun = 1;
        const int Update = 2;


        const int EnabelAutoRun_Return = 2016;
        const int DisabelAutoRun_Return = 2017;


        // 调用一个窗口的窗口函数，将一条消息发给那个窗口。除非消息处理完毕，否则该函数不会返回。SendMessageBynum， SendMessageByString是该函数的“类型安全”声明形式
        [DllImport("user32", EntryPoint = "SendMessage", SetLastError = false,
        CharSet = CharSet.Auto, ExactSpelling = false,
        CallingConvention = CallingConvention.StdCall)]
        private static extern int SendMessage(
            IntPtr hWnd,
            int wMsg,
            int wParam,
            int lParam
        );

        private const int WM_QUERYENDSESSION = 0x11;


        static void Main(string[] args)
        {
            if (args.Length == 0) return;

            string cmdline = "";

            foreach (var item in args)
            {
                cmdline += item + " ";
            }

            var arg = cmdline.Trim().Split('|');

            int type = -1;

            IntPtr mainIntptr = IntPtr.Zero;

            try
            {
                type = int.Parse(arg[0]);

                if (arg.Length >= 2) mainIntptr = (IntPtr)int.Parse(arg[1]);

            }
            catch (Exception)
            {
                return;
            }

            switch (type)
            {
                case EnabelAutoRun:
                    {
                        //0: Type
                        //1: MainForm Hwnd
                        //2: Executable Path

                        if (arg.Length == 3 &&
                            arg[2].EndsWith(".exe", StringComparison.OrdinalIgnoreCase) &&
                            File.Exists(arg[2])
                            )
                        {
                            AutoRunConfig.EnableAutoRun(arg[2]);
                        }

                        SendMessage(mainIntptr, DisabelAutoRun_Return, 0, 0);

                        break;
                    }
                case DisabelAutoRun:
                    {
                        //0: Type
                        //1: MainForm Hwnd

                        AutoRunConfig.DisableAutoRun();

                        SendMessage(mainIntptr, EnabelAutoRun_Return, 0, 0);

                        break;
                    }
                case Update:
                    {
                        //0: Type
                        //1: MainForm Hwnd
                        //2: Old EXE Path
                        //3: New EXE Path
                        MessageBox.Show("Update");

                        if (arg.Length != 4) return;

                        SendMessage(mainIntptr, WM_QUERYENDSESSION, 0, 0);

                        string oldPath = arg[2]; string newPath = arg[3];

                        Thread.Sleep(15);

                        File.Delete(oldPath);

                        File.Move(oldPath, newPath);

                        Process.Start(newPath);

                        break;
                    }

                default:
                    break;
            }


        }
    }
}
