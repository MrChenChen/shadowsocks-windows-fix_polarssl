using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Shadowsocks.Controller
{
    public class VersionChecker
    {
        private const string UpdateURL = "https://api.github.com/repos/MrChenChen/shadowsocks-windows-fix_polarssl/releases/latest";

        public static string GetCurrentVersionNumber()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void VersionCheck()
        {
            Task.Factory.StartNew(() =>
            {
                WebClient http = new WebClient();

                http.Encoding = Encoding.UTF8;

                http.DownloadStringCompleted += Http_DownloadStringCompleted; ;

                http.DownloadStringAsync(new Uri(UpdateURL));

            });
        }

        private void Http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                JsonObject jsonObject = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(e.Result);


            }
 
        }


    }
}
