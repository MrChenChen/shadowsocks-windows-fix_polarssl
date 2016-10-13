using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Shadowsocks;
using System.Windows.Forms;
using Shadowsocks.Model;
using Shadowsocks.View;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace Shadowsocks
{
    static class GetPassWord
    {
        public static void GetPassWordFromNet(int i)
        {
            WebClient http = new WebClient();

            http.Encoding = Encoding.UTF8;

            index = i;

            http.DownloadStringCompleted += http_DownloadStringCompleted;

            http.DownloadStringAsync(new Uri(@"http://www.ishadowsocks.org/"));

        }

        public static int index = -1;

        public static ConfigForm m_mainform;

        public static ContextMenu contextmenu1;

        public static Action mAction = null;

        public static Button _buttonUS;
        public static Button _buttonHK;
        public static Button _buttonJP;

        private static void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {


            myserverlist[0] = new Model.Server();
            myserverlist[1] = new Model.Server();
            myserverlist[2] = new Model.Server();

            string str = "";
            try
            {
                str = e.Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            //加密方式
            var method = Regex.Matches(str, @":...-...*<");

            myserverlist[0].method = method[0].Value.Replace(":", "").Replace("<", "").Trim();
            myserverlist[1].method = method[1].Value.Replace(":", "").Replace("<", "").Trim();
            myserverlist[2].method = method[2].Value.Replace(":", "").Replace("<", "").Trim();



            //服务器端口
            var port = Regex.Matches(str, @":\d{3,5}<");

            myserverlist[0].server_port = int.Parse(port[0].Value.Replace(":", "").Replace("<", "").Trim());
            myserverlist[1].server_port = int.Parse(port[1].Value.Replace(":", "").Replace("<", "").Trim());
            myserverlist[2].server_port = int.Parse(port[2].Value.Replace(":", "").Replace("<", "").Trim());


            myserverlist[0].local_port = 1080;
            myserverlist[1].local_port = 1080;
            myserverlist[2].local_port = 1080;


            //服务器地址
            var server1 = Regex.Match(str, "(A服务器地址:).{0,16}(</h4>)");
            var server2 = Regex.Match(str, "(B服务器地址:).{0,16}(</h4>)");
            var server3 = Regex.Match(str, "(C服务器地址:).{0,16}(</h4>)");

            myserverlist[0].server = server1.Value.Replace("</h4>", "").Replace("A服务器地址:", "").Trim();
            myserverlist[1].server = server2.Value.Replace("</h4>", "").Replace("B服务器地址:", "").Trim();
            myserverlist[2].server = server3.Value.Replace("</h4>", "").Replace("C服务器地址:", "").Trim();

            //密码
            var password1 = Regex.Match(str, "(A密码:).{0,10}(</h4>)");
            var password2 = Regex.Match(str, "(B密码:).{0,10}(</h4>)");
            var password3 = Regex.Match(str, "(C密码:).{0,10}(</h4>)");

            myserverlist[0].password = password1.Value.Replace("</h4>", "").Replace("A密码:", "");
            myserverlist[1].password = password2.Value.Replace("</h4>", "").Replace("B密码:", "");
            myserverlist[2].password = password3.Value.Replace("</h4>", "").Replace("C密码:", "");


            if (myserverlist[0].password.Length <= 6)
            {
                contextmenu1.MenuItems[0].Enabled = false;
                _buttonUS.Enabled = false;
            }
            else
            {
                contextmenu1.MenuItems[0].Enabled = true;
                _buttonUS.Enabled = true;
            }


            if (myserverlist[1].password.Length <= 6)
            {
                contextmenu1.MenuItems[1].Enabled = false;
                _buttonHK.Enabled = false;
            }
            else
            {
                contextmenu1.MenuItems[1].Enabled = true;
                _buttonHK.Enabled = true;
            }


            if (myserverlist[2].password.Length <= 6)
            {
                contextmenu1.MenuItems[2].Enabled = false;
                _buttonJP.Enabled = false;
            }
            else
            {
                contextmenu1.MenuItems[2].Enabled = true;
                _buttonJP.Enabled = true;
            }

            contextmenu1.MenuItems[0].Checked = false;
            contextmenu1.MenuItems[1].Checked = false;
            contextmenu1.MenuItems[2].Checked = false;


            m_mainform.ProxyPortTextBox.Text = "1080";

            switch (index)
            {
                case 0:
                    {
                        if (_buttonUS.Enabled)
                        {
                            m_mainform.AutoSetPassword(myserverlist[0]);
                            contextmenu1.MenuItems[0].Checked = true;
                        }

                        break;
                    }
                case 1:
                    {
                        if (_buttonHK.Enabled)
                        {
                            m_mainform.AutoSetPassword(myserverlist[1]);
                            contextmenu1.MenuItems[1].Checked = true;
                        }

                        break;
                    }
                case 2:
                    {
                        if (_buttonJP.Enabled)
                        {
                            m_mainform.AutoSetPassword(myserverlist[2]);
                            contextmenu1.MenuItems[2].Checked = true;
                        }

                        break;
                    }

                default:
                    break;
            }

            if (index == -1)
            {
                mAction();
            }


            //m_mainform.OKButton_Click(null, null);
        }




        /// <summary>
        /// 删除字符串中第一个匹配的字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="del">要删掉的字符串</param>
        /// <returns></returns>
        public static string RemoveFirstMatch(this string str, string del)
        {
            if (string.IsNullOrEmpty(del)) return str;

            int i = str.IndexOfAny(del.ToArray());

            return str.Remove(i, del.Length);

        }

        public static Model.Server[] myserverlist = new Model.Server[3];

    }
}
