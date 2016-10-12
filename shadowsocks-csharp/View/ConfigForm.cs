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
        private Configuration _modifiedConfiguration;
        private int _oldSelectedIndex = -1;
        private bool _isFirstRun;

        private Settings m_settings;

        public ConfigForm(ShadowsocksController controller)
        {
            InitializeComponent();

            LoadTrayIcon();

            m_settings = Properties.Settings.Default;

            notifyIcon1.ContextMenu = contextMenu1;

            this.controller = controller;
            controller.EnableStatusChanged += controller_EnableStatusChanged;
            controller.ConfigChanged += controller_ConfigChanged;
            controller.PACFileReadyToOpen += controller_PACFileReadyToOpen;
            controller.ShareOverLANStatusChanged += controller_ShareOverLANStatusChanged;


            LoadCurrentConfiguration();

            GetPassWord.contextmenu1 = contextMenu1;
            GetPassWord._buttonUS = buttonUS;
            GetPassWord._buttonHK = buttonHK;
            GetPassWord._buttonJP = buttonJP;

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


            //if (tempAutoCheckUpdate)
            //CheckUpdate();

            //mUpdater.VersionCheck();

 

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
            ShareOverLANItem.Checked = controller.GetConfiguration().shareOverLan;
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
                if (_oldSelectedIndex == -1 || _oldSelectedIndex >= _modifiedConfiguration.configs.Count)
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
                _modifiedConfiguration.configs[_oldSelectedIndex] = server;

                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show("无效的端口号");
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
                ServerGroupBox.Visible = true;

                //m_list_ads=server.
                //IPTextBox.Focus();
            }
            else
            {
                ServerGroupBox.Visible = false;
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
            _oldSelectedIndex = _modifiedConfiguration.index;
            ServersListBox.SelectedIndex = _modifiedConfiguration.index;
            LoadSelectedServer();

            UpdateServersMenu();
            enableItem.Checked = _modifiedConfiguration.enabled;
            checkBoxEnable.Checked = _modifiedConfiguration.enabled;
            ShareOverLANItem.Checked = _modifiedConfiguration.shareOverLan;
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
        string Startuplnkname;

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            Startuplnkname = Shadowsocks._3rd.CreateDesktopShort.StartupPath + "\\" + System.IO.Path.GetFileName(Application.ExecutablePath).Replace(".exe", ".lnk").Replace(".EXE", ".lnk");

            checkBoxAutoRun.Checked = File.Exists(Startuplnkname);

            GetPassWord.m_mainform = this;

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
            GetPassWorsFunc(null);

            //timer1.Start();

            controller.ToggleEnable(true);

            KillTecNews();
        }

        public const int WM_CLOSE = 0x10;


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_QUERYENDSESSION:
                    //string message = string.Format("收到自己消息的参数:{0},{1}", m.WParam, m.LParam);
                    //处理启动 函数MessageBox.Show(message);//显示一个消息框
                    Close();
                    notifyIcon1.Dispose();
                    Environment.Exit(0);
                    break;
                default:
                    base.DefWndProc(ref m);//一定要调用基类函数，以便系统处理其它消息。
                    break;
            }
        }

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


            }))
            { IsBackground = true }.Start();

        }


        void CloedTecAds(string title)
        {
            var temp = FindWindow(null, title);

            if (temp != null)
            {
                SendMessage(temp, WM_CLOSE, 0, 0);
            }
        }



        private void ServersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_oldSelectedIndex == ServersListBox.SelectedIndex)
            {
                // we are moving back to oldSelectedIndex or doing a force move
                return;
            }
            if (!SaveOldSelectedServer())
            {
                // why this won't cause stack overflow?
                ServersListBox.SelectedIndex = _oldSelectedIndex;
                return;
            }
            LoadSelectedServer();
            _oldSelectedIndex = ServersListBox.SelectedIndex;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!SaveOldSelectedServer())
            {
                return;
            }
            Server server = Configuration.GetDefaultServer();
            _modifiedConfiguration.configs.Add(server);
            LoadConfiguration(_modifiedConfiguration);
            ServersListBox.SelectedIndex = _modifiedConfiguration.configs.Count - 1;
            _oldSelectedIndex = ServersListBox.SelectedIndex;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            _oldSelectedIndex = ServersListBox.SelectedIndex;
            if (_oldSelectedIndex >= 0 && _oldSelectedIndex < _modifiedConfiguration.configs.Count)
            {
                _modifiedConfiguration.configs.RemoveAt(_oldSelectedIndex);
            }
            if (_oldSelectedIndex >= _modifiedConfiguration.configs.Count)
            {
                // can be -1
                _oldSelectedIndex = _modifiedConfiguration.configs.Count - 1;
            }
            ServersListBox.SelectedIndex = _oldSelectedIndex;
            LoadConfiguration(_modifiedConfiguration);
            ServersListBox.SelectedIndex = _oldSelectedIndex;
            LoadSelectedServer();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Close();
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
                MessageBox.Show("请至少添加一个服务器！");
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
            Process.Start("https://github.com/clowwindy/shadowsocks-csharp");
        }



        private void EnableItem_Click(object sender, EventArgs e)
        {
            enableItem.Checked = !enableItem.Checked;
            controller.ToggleEnable(enableItem.Checked);
        }

        private void ShareOverLANItem_Click(object sender, EventArgs e)
        {
            ShareOverLANItem.Checked = !ShareOverLANItem.Checked;
            controller.ToggleShareOverLAN(ShareOverLANItem.Checked);
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

        //切换 Enable 开启梭影
        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            controller.ToggleEnable(checkBoxEnable.Checked);
            enableItem.Checked = checkBoxEnable.Checked;
        }




        void GetPassWorsFunc(Action action)
        {
            for (int i = 0; i < ServersListBox.Items.Count; i++)
            {
                DeleteButton_Click(null, null);
            }


            AddButton_Click(null, null);

            Action GetPassWord_Completed = () =>
            {



                Task.Factory.StartNew(() =>
                {

                    if (GetPassWord.myserverlist[0].password.Length >= 6)
                    {
                        this.Invoke(new Action(() =>
                        {
                            AutoSetPassword(GetPassWord.myserverlist[0]);
                            contextMenu1.MenuItems[0].Checked = true;
                            contextMenu1.MenuItems[1].Checked = false;
                            contextMenu1.MenuItems[2].Checked = false;
                        }));
                        return;
                    }

                    if (GetPassWord.myserverlist[1].password.Length >= 6)
                    {
                        this.Invoke(new Action(() =>
                        {
                            AutoSetPassword(GetPassWord.myserverlist[1]);
                            contextMenu1.MenuItems[0].Checked = false;
                            contextMenu1.MenuItems[1].Checked = true;
                            contextMenu1.MenuItems[2].Checked = false;
                        }));
                        return;
                    }

                    if (GetPassWord.myserverlist[2].password.Length >= 6)
                    {
                        this.Invoke(new Action(() =>
                        {
                            AutoSetPassword(GetPassWord.myserverlist[2]);
                            contextMenu1.MenuItems[0].Checked = false;
                            contextMenu1.MenuItems[1].Checked = false;
                            contextMenu1.MenuItems[2].Checked = true;
                        }));
                        return;
                    }




                });


            };

            GetPassWord.mAction = GetPassWord_Completed;
            GetPassWord.GetPassWordFromNet(-1);

            action?.Invoke();

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
                ServerGroupBox.Visible = true;

                OKButton_Click(null, null);
            }
            else
            {
                MessageBox.Show("获取网络密码错误！");
            }
        }


        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SaveServers(_modifiedConfiguration.configs, this);
            e.Cancel = true;

            Hide();
        }



        private void buttonUS_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(0);
        }

        private void buttonHK_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(1);
        }

        private void buttonJP_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(2);
        }

        private void menuItemUS_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(0);
        }

        private void menuItemHK_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(1);
        }

        private void menuItemJP_Click(object sender, EventArgs e)
        {
            GetPassWord.GetPassWordFromNet(2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int h = DateTime.Now.Hour;      //获取当前时间的小时部分

            int m = DateTime.Now.Minute;    //获取当前时间的分钟部分

            int s = DateTime.Now.Second;    //获取当前时间的秒部分


            if (s == 0 && m == 1)
            {
                if (h == 0 || h == 6 || h == 12 || h == 18)
                {

                    //定时 自动切换
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {

                        Action action = () =>
                        {

                            Invoke(new Action(() =>
                                                    {
                                                        if (buttonUS.Enabled)
                                                        {
                                                            buttonUS_Click(null, null);

                                                            return;
                                                        }

                                                        if (buttonHK.Enabled)
                                                        {
                                                            buttonHK_Click(null, null);

                                                            return;
                                                        }

                                                        if (buttonJP.Enabled)
                                                        {
                                                            buttonJP_Click(null, null);

                                                            return;
                                                        }

                                                    }));

                        };

                        GetPassWorsFunc(action);

                    }))
                    { IsBackground = true }.Start();


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

        //添加广告
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var str = Microsoft.VisualBasic.Interaction.InputBox("请输入要屏蔽的标题：", "添加要屏蔽的弹出广告", "");

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


        //删除广告
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
        /// 手动更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemStartCheckUpdate_Click(object sender, EventArgs e)
        {
            //CheckUpdate();
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

    }

}