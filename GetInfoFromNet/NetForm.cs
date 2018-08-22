using GetInfoFromNet.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace GetInfoFromNet
{

    public partial class NetForm : Form
    {
        private static List<Server> mListServer = null;

        public TextBox ServerTxt;

        public TextBox PortTxt;

        public TextBox PasswordTxt;

        public ComboBox EncryptionSelect;

        public Button OkBtn;


        public static void ShowForm(TextBox server, TextBox port, TextBox password, ComboBox encrypt, Button ok)
        {
            NetForm nf = new NetForm()
            {
                ServerTxt = server,
                PortTxt = port,
                PasswordTxt = password,
                EncryptionSelect = encrypt,
                OkBtn = ok
            };
            nf.StartPosition = FormStartPosition.CenterScreen;
            nf.SetListToCombox();
            nf.Show();
        }

        public NetForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            comboBoxInfos.DrawMode = DrawMode.OwnerDrawFixed;
        }

        private void NetForm_Load(object sender, EventArgs e)
        {
            textBoxShadowURL.Text = Settings.Default.shadowsockurl;
        }

        private void buttonAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                mListServer = AquireInfo();

                SetListToCombox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //var temp = GetFromImage(@"https://en.ss8.fun/images/server01.png");
            //if (temp != null) comboBoxInfos.Items.Add(temp);
            //temp = GetFromImage(@"https://en.ss8.fun/images/server02.png");
            //if (temp != null) comboBoxInfos.Items.Add(temp);
            //temp = GetFromImage(@"https://en.ss8.fun/images/server03.png");
            //if (temp != null) comboBoxInfos.Items.Add(temp);

        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (comboBoxInfos.SelectedIndex != -1)
            {
                var index = comboBoxInfos.SelectedIndex;

                var server = mListServer[index];

                ServerTxt.Text = server.server;
                PortTxt.Text = server.server_port.ToString();
                PasswordTxt.Text = server.password;
                EncryptionSelect.SelectedIndex = EncryptionSelect.Items.IndexOf(server.method);

                OkBtn.PerformClick();

                SaveSetting();

                Close();
            }
        }

        void SaveSetting()
        {
            Settings.Default.shadowsockurl = textBoxShadowURL.Text;
            Settings.Default.Save();
        }

        void SetListToCombox()
        {
            if (mListServer == null) return;

            comboBoxInfos.Items.Clear();

            mListServer.ForEach(p => comboBoxInfos.Items.Add(p));

            comboBoxInfos.SelectedIndex = 0;
        }

        List<Server> AquireInfo()
        {
            var http = new WebClientPro(10000)
            {
                Encoding = Encoding.UTF8
            };

            var html = http.DownloadString(textBoxShadowURL.Text);

            var key1 = html.Split(new string[] { "<!-- Portfolio Section -->" }, StringSplitOptions.None)[1];

            var mainkey = key1.Split(new string[] { "<!-- Team Section -->" }, StringSplitOptions.None)[0];

            var len = mainkey.Split(new string[] { "IP Address:" }, StringSplitOptions.None).Length - 1;

            if (len < 0)
            {
                return null;
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
                var result = Regex.Matches(mainkey, "(Port).{1,50}");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    if (item.Split('>')[1].Trim() == "")
                    {
                        list[i].server_port = 0;
                    }
                    else
                    {
                        list[i].server_port = int.Parse(item.Split('>')[1].Trim());
                    }
                }

            }
            #endregion

            #region Password
            {
                var result = Regex.Matches(mainkey, "(Password:).{1,50}");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].password = item.Split('>')[1].Trim();
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

            return list.Where(p => p.IsVaild()).ToList();

        }

        private void NetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            Hide();
        }

        private void comboBoxInfos_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0)
                e.Graphics.DrawString(comboBoxInfos.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds);
            System.Diagnostics.Debug.WriteLine(e.State);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected
                && e.Index >= 0
                && (e.State & DrawItemState.Selected) != DrawItemState.ComboBoxEdit)
                toolTip1.Show(
                    comboBoxInfos.Items[e.Index].ToString(),
                    comboBoxInfos,
                    e.Bounds.X + e.Bounds.Width,
                    e.Bounds.Y + e.Bounds.Height);

            e.DrawFocusRectangle();
        }

        private void comboBoxInfos_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(comboBoxInfos);
        }

        private void NetForm_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.Hide(comboBoxInfos);
        }

        private Server GetFromImage(string url)
        {
            Server server = null;

            try
            {
                QRCodeDecoder decoder = new QRCodeDecoder();
                var pictureBox1 = new PictureBox();
                pictureBox1.Load(url);
                var input = (Bitmap)pictureBox1.Image;
                string decodedString = decoder.decode(new QRCodeBitmapImage(input));
                var text = Encoding.ASCII.GetString(Convert.FromBase64String(decodedString.Substring(5))).Trim();

                server = new Server();

                var key1 = text.Split(new[] { ":@" }, StringSplitOptions.RemoveEmptyEntries);

                server.method = key1[0];
                server.server = key1[1].Split(':')[0];
                server.password = key1[1].Split(':')[1];
                server.server_port = 443;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return server;

        }

        public static void GetGoogleHtml()
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        WebClient webClient = new WebClient();
                        var text = webClient.DownloadString("https://www.google.com/");
                        Console.WriteLine("Google Html: " + text.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetGoogleHtml: " + ex.Message);
                    }
                    Thread.Sleep(1000);
                }
            });
        }

    }

    internal class Server
    {
        public string server;
        public int server_port;
        public string password;
        public string method;

        public bool IsVaild()
        {
            return !string.IsNullOrWhiteSpace(server)
                && !string.IsNullOrWhiteSpace(password)
                && !string.IsNullOrWhiteSpace(method)
                && server_port != 0;
        }

        public override string ToString()
        {
            return
               "server : " + server + "\r\n" +
               "server_port : " + server_port + "\r\n" +
               "password : " + password + "\r\n" +
               "method : " + method + "\r\n"
                ;
        }
    }

    public class WebClientPro : WebClient
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public int Timeout { get; set; }

        public WebClientPro(int timeout = 30000)
        {//默认30秒
            Timeout = timeout;
        }

        /// <summary>
        /// 重写GetWebRequest,添加WebRequest对象超时时间
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {//WebClient里上传下载的方法很多，但最终应该都是调用了这个方法
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            return request;
        }
    }

}
