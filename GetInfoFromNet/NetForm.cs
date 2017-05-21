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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetInfoFromNet
{

    public partial class NetForm : Form
    {
        private List<Server> mListServer = null;

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
            nf.Show();
        }

        public NetForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
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
                EncryptionSelect.SelectedText = server.method;
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
            comboBoxInfos.Items.Clear();

            mListServer.ForEach(p => comboBoxInfos.Items.Add(p));

            comboBoxInfos.SelectedIndex = 0;
        }

        List<Server> AquireInfo()
        {
            WebClient http = new WebClient()
            {
                Encoding = Encoding.UTF8
            };

            var html = http.DownloadString(textBoxShadowURL.Text);

            var key1 = html.Split(new String[] { "<!-- Portfolio Section -->" }, StringSplitOptions.None)[1];

            var mainkey = key1.Split(new String[] { "<!-- Team Section -->" }, StringSplitOptions.None)[0];

            var len = mainkey.Split(new String[] { "IP Address:" }, StringSplitOptions.None).Length - 1;

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

            return list;

        }


    }

    internal class Server
    {
        public string server;
        public int server_port;
        public string password;
        public string method;

        public override string ToString()
        {
            return server;
        }
    }

}
