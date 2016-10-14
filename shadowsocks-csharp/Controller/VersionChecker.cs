using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Shadowsocks.Controller
{

    public class VersionChecker
    {
        private const string UpdateURL = "http://api.github.com/repos/MrChenChen/shadowsocks-windows-fix_polarssl/releases/latest";

        public Form mainForm = null;

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
                http.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)");
                http.DownloadStringCompleted += Http_DownloadStringCompleted;
                http.DownloadStringAsync(new Uri(UpdateURL), null);

            });
        }

        private void Http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string curr_version = GetCurrentVersionNumber();

                try
                {
                    JsonObject jsonObject = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(e.Result);

                    if (jsonObject == null || jsonObject.Count == 0) throw new Exception("JsonObject is empty");

                    var newest_version = GetJsonValue(jsonObject, "tag_name");

                    if (newest_version == curr_version)
                    {
                        MessageBox.Show("当前已是最新版本！", "Shadowsock " + newest_version);
                    }
                    else
                    {
                        var currTemp = curr_version.Split('.');

                        var newTemp = newest_version.Split('.');

                        var index = Math.Min(currTemp.Length, newTemp.Length);

                        for (int i = 0; i < index; i++)
                        {
                            if (int.Parse(currTemp[i]) < int.Parse(newTemp[i]))
                            {

                                var result = MessageBox.Show("Shadowsock 检测到新版本!\r\n\r\n" + GetJsonValue(jsonObject, "body") + "\r\n\r\n当前版本号： " + curr_version + "\r\n" + "最新版本号： " + newest_version, "是否开始更新？", MessageBoxButtons.YesNo);

                                if (result == DialogResult.Yes)
                                {
                                    object temp;

                                    jsonObject.TryGetValue("assets", out temp);

                                    JsonArray jsonArr = (JsonArray)temp;

                                    List<string> list_files = new List<string>();

                                    foreach (var item in jsonArr)
                                    {
                                        JsonObject tempJson = (JsonObject)item;

                                        list_files.Add(GetJsonValue(tempJson, "browser_download_url"));
                                    }

                                    string oldPath = Application.ExecutablePath;

                                    string newPath = Path.GetTempPath() + "Shadowsocks.exe";

                                    string uri = list_files[0];

                                    WebClient http = new WebClient();

                                    http.DownloadFileCompleted += (o, e1) =>
                                    {
                                        if (e.Error == null)
                                        {
                                            string updater = Path.GetTempPath() + "Shadowsocks Update.exe";

                                            ReleaseUpdater(updater);

                                            Process.Start(updater, "2 " + mainForm.Handle + " " + oldPath + " " + newPath);

                                        }
                                    };

                                    File.Delete(newPath);

                                    http.DownloadFile(new Uri(uri), newPath);

                                }

                                return;
                            }
                        }

                        MessageBox.Show("当前已是最新版本！");
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Version Check");
                }



            }

        }

        public static string GetJsonValue(JsonObject json, string key)
        {
            object name;

            json.TryGetValue(key, out name);

            return (string)name;
        }

        public static void ReleaseUpdater(string uri)
        {
            File.WriteAllBytes(uri, Properties.Resources.Shadowsocks_Update);
        }


    }


}
