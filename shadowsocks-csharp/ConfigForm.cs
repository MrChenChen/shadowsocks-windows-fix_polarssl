using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

namespace Shadowsocks.View
{

    public partial class ConfigForm : Form
    {

        private ShadowsocksController controller;

        private List<string> m_list_ads = new List<string>();

        private VersionChecker mUpdater = new VersionChecker();

        // this is a copy of configuration that we are working on
        public Configuration _modifiedConfiguration;
        private bool _isFirstRun;
        private List<Server> mListServer = new List<Server>();
        private Settings m_settings;

        public ConfigForm(ShadowsocksController controller)
        {
            InitializeComponent();

            LoadTrayIcon();

            m_settings = Properties.Settings.Default;

            notifyIcon1.ContextMenu = contextMenu1;

            mUpdater.mainForm = this;

            this.controller = controller;
            controller.EnableStatusChanged += controller_EnableStatusChanged;
            controller.ConfigChanged += controller_ConfigChanged;
            controller.PACFileReadyToOpen += controller_PACFileReadyToOpen;
            controller.ShareOverLANStatusChanged += controller_ShareOverLANStatusChanged;

            LoadCurrentConfiguration();

            if (_modifiedConfiguration.ads != null)
            {
                foreach (var item in _modifiedConfiguration.ads)
                {
                    listBoxADs.Items.Add(item);
                }
                checkBoxAutoRun.Checked = _modifiedConfiguration.autorun;
                checkBoxAutoHide.Checked = _modifiedConfiguration.autohide;
                menuItemAutoCheckUpdate.Checked = _modifiedConfiguration.autoupdate;
                tempAutoCheckUpdate = menuItemAutoCheckUpdate.Checked;
            }

            buttonRefresh_Click(null, null);

        }

        private void LoadTrayIcon()
        {
            int dpi;
            Graphics graphics = this.CreateGraphics();
            dpi = (int)graphics.DpiX;
            graphics.Dispose();
            Bitmap icon = null;
            if (dpi < 97)
            {
                // dpi = 96;
                icon = Resources.ss16;
            }
            else if (dpi < 121)
            {
                // dpi = 120;
                icon = Resources.ss20;
            }
            else
            {
                icon = Resources.ss24;
            }
            notifyIcon1.Icon = Icon.FromHandle(icon.GetHicon());
            notifyIcon1.Visible = true;
            //this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());

        }

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            LoadCurrentConfiguration();
        }

        private void controller_EnableStatusChanged(object sender, EventArgs e)
        {
            enableItem.Checked = controller.GetConfiguration().enabled;
        }

        void controller_ShareOverLANStatusChanged(object sender, EventArgs e)
        {
            //ShareOverLANItem.Checked = controller.GetConfiguration().shareOverLan;
        }

        void controller_PACFileReadyToOpen(object sender, ShadowsocksController.PathEventArgs e)
        {
            string argument = @"/select, " + e.Path;

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void ShowWindow()
        {
            this.Opacity = 1;
            this.Show();
            this.Activate();
            OKButton.Focus();
        }

        private bool SaveOldSelectedServer()
        {
            try
            {
                if (_modifiedConfiguration.index == -1 || _modifiedConfiguration.index >= _modifiedConfiguration.configs.Count)
                {
                    return true;
                }
                Server server = new Server
                {
                    server = IPTextBox.Text,
                    server_port = int.Parse(ServerPortTextBox.Text),
                    password = PasswordTextBox.Text,
                    local_port = int.Parse(ProxyPortTextBox.Text),
                    method = EncryptionSelect.Text,
                    remarks = RemarksTextBox.Text
                };
                Configuration.CheckServer(server);
                _modifiedConfiguration.configs[_modifiedConfiguration.index] = server;

                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show("��Ч�Ķ˿ں�");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        private void LoadSelectedServer()
        {
            if (ServersListBox.SelectedIndex >= 0 && ServersListBox.SelectedIndex < _modifiedConfiguration.configs.Count)
            {
                Server server = _modifiedConfiguration.configs[ServersListBox.SelectedIndex];

                IPTextBox.Text = server.server;
                ServerPortTextBox.Text = server.server_port.ToString();
                PasswordTextBox.Text = server.password;
                ProxyPortTextBox.Text = server.local_port.ToString();
                EncryptionSelect.Text = server.method ?? "aes-256-cfb";
                RemarksTextBox.Text = server.remarks;

                //m_list_ads=server.
                //IPTextBox.Focus();
            }
            else
            {
            }
        }

        private void LoadConfiguration(Configuration configuration)
        {
            ServersListBox.Items.Clear();
            foreach (Server server in _modifiedConfiguration.configs)
            {
                ServersListBox.Items.Add(string.IsNullOrEmpty(server.server) ? "New server" : string.IsNullOrEmpty(server.remarks) ? server.server + ":" + server.server_port : server.server + ":" + server.server_port + " (" + server.remarks + ")");
            }

            m_list_ads = _modifiedConfiguration.ads;

        }

        private void LoadCurrentConfiguration()
        {
            _modifiedConfiguration = controller.GetConfiguration();
            LoadConfiguration(_modifiedConfiguration);
            //ServersListBox.SelectedIndex = _modifiedConfiguration.index;
            LoadSelectedServer();

            UpdateServersMenu();
            enableItem.Checked = _modifiedConfiguration.enabled;
            checkBoxEnable.Checked = _modifiedConfiguration.enabled;
            //ShareOverLANItem.Checked = _modifiedConfiguration.shareOverLan;
        }

        private void UpdateServersMenu()
        {
            var items = ServersItem.MenuItems;

            items.Clear();

            Configuration configuration = controller.GetConfiguration();
            for (int i = 0; i < configuration.configs.Count; i++)
            {
                Server server = configuration.configs[i];
                MenuItem item = new MenuItem(string.IsNullOrEmpty(server.remarks) ? server.server + ":" + server.server_port : server.server + ":" + server.server_port + " (" + server.remarks + ")");
                item.Tag = i;
                item.Click += AServerItem_Click;
                items.Add(item);
            }
            items.Add(SeperatorItem);
            items.Add(ConfigItem);

            if (configuration.index >= 0 && configuration.index < configuration.configs.Count)
            {
                items[configuration.index].Checked = true;
            }
        }


        private void ConfigForm_Load(object sender, EventArgs e)
        {
            //��ȡ������������

            CheckForIllegalCrossThreadCalls = false;

            if (!controller.GetConfiguration().isDefault)
            {
                //this.Opacity = 0;
                //BeginInvoke(new MethodInvoker(delegate
                //{
                //    this.Hide();
                //}));
            }
            else
            {
                _isFirstRun = true;
            }
            //updateChecker.CheckUpdate();


            controller.ToggleEnable(true);

            KillTecNews();
        }

        #region WMD PRO

        public const int WM_CLOSE = 0x10;


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // ����һ�����ڵĴ��ں�������һ����Ϣ�����Ǹ����ڡ�������Ϣ������ϣ�����ú������᷵�ء�SendMessageBynum�� SendMessageByString�Ǹú����ġ����Ͱ�ȫ��������ʽ
        [DllImport("user32", EntryPoint = "SendMessage", SetLastError = false,
        CharSet = CharSet.Auto, ExactSpelling = false,
        CallingConvention = CallingConvention.StdCall)]
        private static extern int SendMessage(
            IntPtr hWnd,
            int wMsg,
            int wParam,
            int lParam
        );

        public const int BM_CLICK = 0xF5;

        private const int WM_QUERYENDSESSION = 0x11;

        const int EnabelAutoRun_Return = 2016;

        const int DisabelAutoRun_Return = 2017;

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_QUERYENDSESSION:
                    //string message = string.Format("�յ��Լ���Ϣ�Ĳ���:{0},{1}", m.WParam, m.LParam);
                    //�������� ����MessageBox.Show(message);//��ʾһ����Ϣ��
                    Close();
                    controller.Stop();
                    notifyIcon1.Dispose();
                    Environment.Exit(0);
                    break;
                case EnabelAutoRun_Return:

                    break;
                case DisabelAutoRun_Return:

                    break;
                default:
                    base.DefWndProc(ref m);//һ��Ҫ���û��ຯ�����Ա�ϵͳ����������Ϣ��
                    break;
            }
        }
        #endregion

        void KillTecNews()
        {
            IntPtr temp = IntPtr.Zero;

            new Thread(new ThreadStart(() =>
            {

                while (true)
                {
                    if (checkBoxKillNew.Checked)
                    {
                        foreach (string item in listBoxADs.Items)
                        {
                            CloedTecAds(item);
                        }
                    }

                    timer1_Tick(null, null);

                    Thread.Sleep(500);
                }


            })) { IsBackground = true }.Start();

        }


        void CloedTecAds(string title)
        {
            var temp = FindWindow(null, title);

            if (temp != null)
            {
                SendMessage(temp, WM_CLOSE, 0, 0);
            }
        }


        private void Config_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Close();
            controller.Stop();
            notifyIcon1.Dispose();
            Environment.Exit(0);
        }

        private void ShowFirstTimeBalloon()
        {
            if (_isFirstRun)
            {
                notifyIcon1.BalloonTipTitle = "Shadowsocks is here";
                notifyIcon1.BalloonTipText = "You can turn on/off Shadowsocks in the context menu";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.ShowBalloonTip(0);
                _isFirstRun = false;
            }
        }

        public void OKButton_Click(object sender, EventArgs e)
        {

            if (!SaveOldSelectedServer())
            {
                return;
            }
            if (_modifiedConfiguration.configs.Count == 0)
            {
                MessageBox.Show("���������һ����������");
                return;
            }

            controller.SaveServers(_modifiedConfiguration.configs, this);

            if (checkBoxAutoHide.Checked)
            {
                Hide();
            }

            //ShowFirstTimeBalloon();
        }



        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Stop();

        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            //Process.Start("https://github.com/clowwindy/shadowsocks-csharp");

            MessageBox.Show("Created by clowwindy\r\nModified by MrChenChen", "ShadowSocks " + VersionChecker.GetCurrentVersionNumber());
        }



        private void EnableItem_Click(object sender, EventArgs e)
        {
            enableItem.Checked = !enableItem.Checked;
            checkBoxEnable.Checked = enableItem.Checked;
        }

        private void ShareOverLANItem_Click(object sender, EventArgs e)
        {
            //ShareOverLANItem.Checked = !ShareOverLANItem.Checked;
            //controller.ToggleShareOverLAN(ShareOverLANItem.Checked);
        }

        private void EditPACFileItem_Click(object sender, EventArgs e)
        {
            controller.TouchPACFile();
        }

        private void AServerItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            controller.SelectServerIndex((int)item.Tag);
        }

        private void ShowLogItem_Click(object sender, EventArgs e)
        {
            string argument = Logging.LogFile;

            System.Diagnostics.Process.Start("notepad.exe", argument);
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            IPTextBox.Focus();
        }

        private void QRCodeItem_Click(object sender, EventArgs e)
        {
            QRCodeForm qrCodeForm = new QRCodeForm(controller.GetQRCodeForCurrentServer());
            qrCodeForm.Icon = this.Icon;
            qrCodeForm.Show();
        }

        //�л� Enable ������Ӱ
        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            controller.ToggleEnable(checkBoxEnable.Checked);
            enableItem.Checked = checkBoxEnable.Checked;

        }



        public void AutoSetPassword(Server server)
        {
            if (server != null)
            {

                IPTextBox.Text = server.server;
                ServerPortTextBox.Text = server.server_port.ToString();
                PasswordTextBox.Text = server.password;
                ProxyPortTextBox.Text = server.local_port.ToString();
                EncryptionSelect.Text = server.method ?? "aes-256-cfb";
                RemarksTextBox.Text = server.remarks;

                OKButton_Click(null, null);
            }
            else
            {
                MessageBox.Show("��ȡ�����������");
            }
        }


        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SaveServers(_modifiedConfiguration.configs, this);

            e.Cancel = true;

            Hide();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            int h = DateTime.Now.Hour;      //��ȡ��ǰʱ���Сʱ����

            int m = DateTime.Now.Minute;    //��ȡ��ǰʱ��ķ��Ӳ���

            int s = DateTime.Now.Second;    //��ȡ��ǰʱ����벿��


            if (s == 0 && m == 1)
            {
                if (h == 0 || h == 6 || h == 12 || h == 18)
                {

                    //��ʱ �Զ��л�
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {

                        Action action = () =>
                        {


                        };

                    })) { IsBackground = true }.Start();


                }

            }


        }

        private void menuItem1_Click(object sender, EventArgs e)
        {

            var p = System.Diagnostics.Process.GetProcesses();

            foreach (var item in p)
            {
                if (item.ProcessName.Contains("ThunderPlat"))
                {
                    item.Kill();
                }

            }

            if (checkBoxAutoHide.Checked)
            {
                Hide();
            }

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowWindow();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowWindow();
            }
        }

        //��ӹ��
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var str = Microsoft.VisualBasic.Interaction.InputBox("������Ҫ���εı��⣺", "���Ҫ���εĵ������", "");

            if (str != string.Empty && str != "")
            {
                listBoxADs.Items.Add(str);

                var temp = "";
                foreach (string item in listBoxADs.Items)
                {
                    temp += item + ";";
                }
            }

        }


        //ɾ�����
        private void buttonDel_Click(object sender, EventArgs e)
        {
            if (listBoxADs.SelectedIndex >= 0)
            {
                listBoxADs.Items.RemoveAt(listBoxADs.SelectedIndex);


                var temp = "";
                foreach (string item in listBoxADs.Items)
                {
                    temp += item + ";";
                }

            }
        }




        /// <summary>
        /// �ֶ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemStartCheckUpdate_Click(object sender, EventArgs e)
        {
            mUpdater.VersionCheck();
        }

        public bool tempAutoCheckUpdate = true;

        private void menuItemAutoCheckUpdate_Click(object sender, EventArgs e)
        {
            menuItemAutoCheckUpdate.Checked = !menuItemAutoCheckUpdate.Checked;

            tempAutoCheckUpdate = menuItemAutoCheckUpdate.Checked;

            controller.SaveServers(_modifiedConfiguration.configs, this);
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.ExecutablePath;

            FileInfo file = new FileInfo(path);

            Process.Start("explorer.exe", file.DirectoryName);

        }

        [DllImport("CreateLinkFile.dll", EntryPoint = "CreateLinkFile", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateLinkFile(
            StringBuilder szStartAppPath,
            StringBuilder szAddCmdLine,
            StringBuilder szDestLnkPath,
            StringBuilder szIconPath2);


        private void checkBoxAutoRun_Click(object sender, EventArgs e)
        {
            //���ÿ�������

            var startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Path.GetFileName(Application.ExecutablePath) + ".lnk";

            try
            {
                if (checkBoxAutoRun.Checked)
                {
                    CreateLinkFile(
                        new StringBuilder(Application.ExecutablePath),
                        new StringBuilder(""),
                        new StringBuilder(startup),
                        new StringBuilder(Application.ExecutablePath)
                        );

                }
                else
                {
                    if (File.Exists(startup))
                    {
                        File.Delete(startup);
                    }
                }
            }
            catch (Exception ex)
            {
                checkBoxAutoRun.Checked = !checkBoxAutoRun.Checked;
                MessageBox.Show(ex.Message);
            }

        }

        private void menuItemBase64_Click(object sender, EventArgs e)
        {
            Base64Form f = new Base64Form();
            f.Show();
        }


        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
            {
                GetPassWord.GetPassWordFromNet(_modifiedConfiguration.passworduri, SetListBoxInfo);

            }));
        }

        private void SetListBoxInfo(List<Server> listinfo)
        {
            Invoke(new Action(() =>
            {
                Func<string, string> GetCountry = s =>
                {
                    if (s.Contains("us")) return "����";
                    if (s.Contains("jp")) return " �ձ�";
                    if (s.Contains("sg")) return "�¼���";
                    return "";
                };

                if (listinfo.Count != 0)
                {
                    mListServer = listinfo;

                    listBoxInfo.Items.Clear();

                    foreach (var item in listinfo)
                    {
                        var temp = item.server + "   " + GetCountry(item.server);

                        listBoxInfo.Items.Add(temp);
                    }

                    listBoxInfo.SelectedIndex = _modifiedConfiguration.index;

                    buttonApply_Click(null, null);

                }
                else
                {
                    MessageBox.Show("Cant Find Any Element");
                }

            }));
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (listBoxInfo.SelectedIndex == -1 || mListServer.Count == 0)
            {
                MessageBox.Show("Wrong Config");
            }
            else
            {
                _modifiedConfiguration.index = listBoxInfo.SelectedIndex;
                AutoSetPassword(mListServer[listBoxInfo.SelectedIndex]);
            }
        }


    }

}
