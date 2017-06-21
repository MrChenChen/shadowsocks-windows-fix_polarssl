namespace GetInfoFromNet
{
    partial class NetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetForm));
            this.textBoxShadowURL = new System.Windows.Forms.TextBox();
            this.labelURL = new System.Windows.Forms.Label();
            this.labelInfos = new System.Windows.Forms.Label();
            this.comboBoxInfos = new System.Windows.Forms.ComboBox();
            this.buttonAcquire = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // textBoxShadowURL
            // 
            this.textBoxShadowURL.Location = new System.Drawing.Point(111, 16);
            this.textBoxShadowURL.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxShadowURL.Name = "textBoxShadowURL";
            this.textBoxShadowURL.Size = new System.Drawing.Size(249, 21);
            this.textBoxShadowURL.TabIndex = 0;
            // 
            // labelURL
            // 
            this.labelURL.AutoSize = true;
            this.labelURL.Location = new System.Drawing.Point(12, 19);
            this.labelURL.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelURL.Name = "labelURL";
            this.labelURL.Size = new System.Drawing.Size(95, 12);
            this.labelURL.TabIndex = 1;
            this.labelURL.Text = "Shadowsock URL:";
            // 
            // labelInfos
            // 
            this.labelInfos.AutoSize = true;
            this.labelInfos.Location = new System.Drawing.Point(12, 73);
            this.labelInfos.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelInfos.Name = "labelInfos";
            this.labelInfos.Size = new System.Drawing.Size(65, 12);
            this.labelInfos.TabIndex = 2;
            this.labelInfos.Text = "信息列表：";
            // 
            // comboBoxInfos
            // 
            this.comboBoxInfos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInfos.Font = new System.Drawing.Font("宋体", 10F);
            this.comboBoxInfos.FormattingEnabled = true;
            this.comboBoxInfos.Location = new System.Drawing.Point(111, 69);
            this.comboBoxInfos.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxInfos.Name = "comboBoxInfos";
            this.comboBoxInfos.Size = new System.Drawing.Size(249, 21);
            this.comboBoxInfos.TabIndex = 4;
            this.comboBoxInfos.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxInfos_DrawItem);
            this.comboBoxInfos.DropDownClosed += new System.EventHandler(this.comboBoxInfos_DropDownClosed);
            this.comboBoxInfos.Leave += new System.EventHandler(this.comboBoxInfos_DropDownClosed);
            this.comboBoxInfos.MouseLeave += new System.EventHandler(this.comboBoxInfos_DropDownClosed);
            // 
            // buttonAcquire
            // 
            this.buttonAcquire.Location = new System.Drawing.Point(62, 110);
            this.buttonAcquire.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAcquire.Name = "buttonAcquire";
            this.buttonAcquire.Size = new System.Drawing.Size(85, 35);
            this.buttonAcquire.TabIndex = 5;
            this.buttonAcquire.Text = "获取";
            this.buttonAcquire.UseVisualStyleBackColor = true;
            this.buttonAcquire.Click += new System.EventHandler(this.buttonAcquire_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(220, 110);
            this.buttonApply.Margin = new System.Windows.Forms.Padding(2);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(85, 35);
            this.buttonApply.TabIndex = 6;
            this.buttonApply.Text = "应用";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // NetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 161);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonAcquire);
            this.Controls.Add(this.comboBoxInfos);
            this.Controls.Add(this.labelInfos);
            this.Controls.Add(this.labelURL);
            this.Controls.Add(this.textBoxShadowURL);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "NetForm";
            this.Text = "NetForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NetForm_FormClosing);
            this.Load += new System.EventHandler(this.NetForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NetForm_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxShadowURL;
        private System.Windows.Forms.Label labelURL;
        private System.Windows.Forms.Label labelInfos;
        private System.Windows.Forms.ComboBox comboBoxInfos;
        private System.Windows.Forms.Button buttonAcquire;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}