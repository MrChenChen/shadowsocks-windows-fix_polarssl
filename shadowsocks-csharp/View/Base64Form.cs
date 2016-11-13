using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Shadowsocks.View
{
    public partial class Base64Form : Form
    {
        public Base64Form()
        {
            InitializeComponent();
        }

        private void buttonEncryption_Click(object sender, EventArgs e)
        {
            if (textBoxSrc.Text == "")
            {
                textBoxDst.Text = "";
            }
            else
            {
                System.Text.Encoding encode = System.Text.Encoding.UTF8;
                byte[] bytedata = encode.GetBytes(textBoxSrc.Text);
                textBoxDst.Text = Convert.ToBase64String(bytedata, 0, bytedata.Length);
                Clipboard.SetText(textBoxDst.Text);

                labelExplain.Text = "已复制密文至剪切板";
                new Thread(() =>
                {
                    Thread.Sleep(3000);
                    Invoke(new Action(() =>
                    {
                        labelExplain.Text = "";
                    }));

                }).Start();
            }
        }

        private void buttonDecryption_Click(object sender, EventArgs e)
        {
            if (textBoxDst.Text == "")
            {
                textBoxSrc.Text = "";
            }
            else
            {
                try
                {
                    byte[] bpath = Convert.FromBase64String(textBoxDst.Text);
                    textBoxSrc.Text = System.Text.ASCIIEncoding.UTF8.GetString(bpath);

                    Clipboard.SetText(textBoxSrc.Text);

                    labelExplain.Text = "已复制原文至剪切板";
                    new Thread(() =>
                    {
                        Thread.Sleep(3000);
                        Invoke(new Action(() =>
                        {
                            labelExplain.Text = "";
                        }));

                    }).Start();
                }
                catch (Exception ex)
                {
                    textBoxSrc.Text = "";
                    labelExplain.Text = ex.Message;
                    new Thread(() =>
                    {
                        Thread.Sleep(3000);
                        Invoke(new Action(() =>
                        {
                            labelExplain.Text = "";
                        }));

                    }).Start();
                }

            }
        }
    }
}
