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
        public static string mUri = "http://abc.ishadow.online/#free";

        public static void GetPassWordFromNet()
        {
            WebClient http = new WebClient()
            {
                Encoding = Encoding.UTF8
            };

            http.DownloadStringCompleted += http_DownloadStringCompleted;

            http.DownloadStringAsync(new Uri(mUri));

        }

        public static int index = -1;

        public static ConfigForm m_mainform;

        public static ContextMenu contextmenu1;

        public static Action<List<Server>> mAction = null;


        private static void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var html = e.Result;

                if (html == "" || html.Length < 100)
                {
                    if (mAction != null) mAction(null);
                    return;
                }

                ParseHtml(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (mAction != null) mAction(null);
            }

        }

        public static void ParseHtml(string html)
        {

            var key1 = html.Split(new String[] { "<!-- Portfolio Section -->" }, StringSplitOptions.None)[1];

            var mainkey = key1.Split(new String[] { "<!-- Team Section -->" }, StringSplitOptions.None)[0];

            var len = mainkey.Split(new String[] { "IP Address:" }, StringSplitOptions.None).Length - 1;

            if (len == 0)
            {
                if (mAction != null) mAction(null);
                return;
            }


            var list = new List<Server>(len);

            for (int i = 0; i < len; i++)
            {
                list.Add(new Server());
            }

            #region IP
            {
                var result = Regex.Matches(mainkey, "(id=\"ip).{1,20}(</span>)");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].server = item.Split('>')[1].Split('<')[0];
                }

            }
            #endregion

            #region Port
            {
                var result = Regex.Matches(mainkey, "(Port：).{1,20}(</h4>)");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].server_port = int.Parse(item.Split('：')[1].Split('<')[0]);

                }

            }
            #endregion

            #region Password
            {
                var result = Regex.Matches(mainkey, "(id=\"pw).{1,20}(</span>)");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].password = item.Split('>')[1].Split('<')[0];
                }
            }
            #endregion

            #region method
            {
                var result = Regex.Matches(mainkey, "(Method:).{1,20}(</h4>)");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].method = item.Split(':')[1].Split('<')[0];
                }
            }
            #endregion


            if (mAction != null) mAction(list);

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
