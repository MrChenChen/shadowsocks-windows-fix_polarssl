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

        static NetForm nf = null;

        public static void ShowForm(TextBox server, TextBox port, TextBox password, ComboBox encrypt, Button ok)
        {
            if (nf == null)
            {
                nf = new NetForm()
                {
                    ServerTxt = server,
                    PortTxt = port,
                    PasswordTxt = password,
                    EncryptionSelect = encrypt,
                    OkBtn = ok
                };
            }
            nf.StartPosition = FormStartPosition.CenterScreen;
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
                var result = Regex.Matches(mainkey, "(Port).{1,20}(</h4>)");

                for (int i = 0; i < len; i++)
                {
                    var item = result[i].Value;

                    list[i].server_port = int.Parse(item.Split('<')[0].Replace("Port：", "").Replace("Port:", ""));

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

    }

    internal class Server
    {
        public string server;
        public int server_port;
        public string password;
        public string method;

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

}
