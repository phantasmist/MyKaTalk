
namespace MyKaTalk
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnOpenServer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnCloseServer = new System.Windows.Forms.ToolStripMenuItem();
            this.sthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnConnect2Server = new System.Windows.Forms.ToolStripMenuItem();
            this.mnExitClass = new System.Windows.Forms.ToolStripMenuItem();
            this.elseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnNetworkConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.출석표조회ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnShowDB = new System.Windows.Forms.ToolStripMenuItem();
            this.mnAddStudent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnCurrTime = new System.Windows.Forms.ToolStripMenuItem();
            this.mnAddDate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnDEBUG = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.sbLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbClientList = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.popInput = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.puSend2Server = new System.Windows.Forms.ToolStripMenuItem();
            this.puSend2Client = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.FileSend = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.popInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.sthToolStripMenuItem,
            this.elseToolStripMenuItem,
            this.출석표조회ToolStripMenuItem,
            this.FileSend});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(368, 28);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnOpenServer,
            this.mnCloseServer});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.menuToolStripMenuItem.Text = "Server";
            // 
            // mnOpenServer
            // 
            this.mnOpenServer.Name = "mnOpenServer";
            this.mnOpenServer.Size = new System.Drawing.Size(176, 26);
            this.mnOpenServer.Text = "Open Server";
            this.mnOpenServer.Click += new System.EventHandler(this.mnOpenServer_Click);
            // 
            // mnCloseServer
            // 
            this.mnCloseServer.Name = "mnCloseServer";
            this.mnCloseServer.Size = new System.Drawing.Size(176, 26);
            this.mnCloseServer.Text = "Close Server";
            this.mnCloseServer.Click += new System.EventHandler(this.mnCloseServer_Click);
            // 
            // sthToolStripMenuItem
            // 
            this.sthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnConnect2Server,
            this.mnExitClass});
            this.sthToolStripMenuItem.Name = "sthToolStripMenuItem";
            this.sthToolStripMenuItem.Size = new System.Drawing.Size(63, 24);
            this.sthToolStripMenuItem.Text = "Client";
            // 
            // mnConnect2Server
            // 
            this.mnConnect2Server.Name = "mnConnect2Server";
            this.mnConnect2Server.Size = new System.Drawing.Size(214, 26);
            this.mnConnect2Server.Text = "Connect to Server";
            this.mnConnect2Server.Click += new System.EventHandler(this.mnConnect2Server_Click);
            // 
            // mnExitClass
            // 
            this.mnExitClass.Name = "mnExitClass";
            this.mnExitClass.Size = new System.Drawing.Size(214, 26);
            this.mnExitClass.Text = "퇴실";
            this.mnExitClass.Click += new System.EventHandler(this.mnExitClass_Click);
            // 
            // elseToolStripMenuItem
            // 
            this.elseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnNetworkConfig});
            this.elseToolStripMenuItem.Name = "elseToolStripMenuItem";
            this.elseToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.elseToolStripMenuItem.Text = "Config";
            // 
            // mnNetworkConfig
            // 
            this.mnNetworkConfig.Name = "mnNetworkConfig";
            this.mnNetworkConfig.Size = new System.Drawing.Size(149, 26);
            this.mnNetworkConfig.Text = "Network";
            this.mnNetworkConfig.Click += new System.EventHandler(this.mnNetworkConfig_Click);
            // 
            // 출석표조회ToolStripMenuItem
            // 
            this.출석표조회ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnShowDB,
            this.mnAddStudent,
            this.mnCurrTime,
            this.mnAddDate,
            this.mnDEBUG});
            this.출석표조회ToolStripMenuItem.Name = "출석표조회ToolStripMenuItem";
            this.출석표조회ToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.출석표조회ToolStripMenuItem.Text = "출석";
            // 
            // mnShowDB
            // 
            this.mnShowDB.Name = "mnShowDB";
            this.mnShowDB.Size = new System.Drawing.Size(224, 26);
            this.mnShowDB.Text = "출석표 조회";
            this.mnShowDB.Click += new System.EventHandler(this.mnShowDB_Click);
            // 
            // mnAddStudent
            // 
            this.mnAddStudent.Name = "mnAddStudent";
            this.mnAddStudent.Size = new System.Drawing.Size(224, 26);
            this.mnAddStudent.Text = "학생 추가";
            this.mnAddStudent.Click += new System.EventHandler(this.mnAddStudent_Click);
            // 
            // mnCurrTime
            // 
            this.mnCurrTime.Name = "mnCurrTime";
            this.mnCurrTime.Size = new System.Drawing.Size(224, 26);
            this.mnCurrTime.Text = "현재 시간";
            this.mnCurrTime.Click += new System.EventHandler(this.mnCurrTime_Click);
            // 
            // mnAddDate
            // 
            this.mnAddDate.Name = "mnAddDate";
            this.mnAddDate.Size = new System.Drawing.Size(224, 26);
            this.mnAddDate.Text = "addDate";
            this.mnAddDate.Click += new System.EventHandler(this.mnAddDate_Click);
            // 
            // mnDEBUG
            // 
            this.mnDEBUG.Name = "mnDEBUG";
            this.mnDEBUG.Size = new System.Drawing.Size(224, 26);
            this.mnDEBUG.Text = "DEBUG";
            this.mnDEBUG.Click += new System.EventHandler(this.mnDEBUG_Click);
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbLabel1,
            this.sbClientList});
            this.statusBar.Location = new System.Drawing.Point(0, 402);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(368, 26);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusStrip1";
            // 
            // sbLabel1
            // 
            this.sbLabel1.Name = "sbLabel1";
            this.sbLabel1.Size = new System.Drawing.Size(65, 20);
            this.sbLabel1.Text = "ClientIP:";
            // 
            // sbClientList
            // 
            this.sbClientList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbClientList.Image = ((System.Drawing.Image)(resources.GetObject("sbClientList.Image")));
            this.sbClientList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbClientList.Name = "sbClientList";
            this.sbClientList.Size = new System.Drawing.Size(85, 24);
            this.sbClientList.Text = "ClientList";
            this.sbClientList.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.sbClientList_DropDownItemClicked);
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.BackColor = System.Drawing.SystemColors.Info;
            this.tbOutput.Location = new System.Drawing.Point(3, 2);
            this.tbOutput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(361, 228);
            this.tbOutput.TabIndex = 2;
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.ContextMenuStrip = this.popInput;
            this.tbInput.Location = new System.Drawing.Point(3, 4);
            this.tbInput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(361, 92);
            this.tbInput.TabIndex = 3;
            this.tbInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbInput_KeyDown);
            this.tbInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbInput_KeyUp);
            // 
            // popInput
            // 
            this.popInput.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.popInput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.puSend2Server,
            this.puSend2Client});
            this.popInput.Name = "popInput";
            this.popInput.Size = new System.Drawing.Size(178, 52);
            // 
            // puSend2Server
            // 
            this.puSend2Server.Name = "puSend2Server";
            this.puSend2Server.Size = new System.Drawing.Size(177, 24);
            this.puSend2Server.Text = "Send to Server";
            this.puSend2Server.Click += new System.EventHandler(this.puSend2Server_Click);
            // 
            // puSend2Client
            // 
            this.puSend2Client.Name = "puSend2Client";
            this.puSend2Client.Size = new System.Drawing.Size(177, 24);
            this.puSend2Client.Text = "Send to Client";
            this.puSend2Client.Click += new System.EventHandler(this.puSend2Client_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 31);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tbOutput);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbInput);
            this.splitContainer1.Size = new System.Drawing.Size(368, 368);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.SplitterWidth = 20;
            this.splitContainer1.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FileSend
            // 
            this.FileSend.Name = "FileSend";
            this.FileSend.Size = new System.Drawing.Size(80, 24);
            this.FileSend.Text = "FileSend";
            this.FileSend.Click += new System.EventHandler(this.FileSend_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 428);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMain";
            this.Text = "MyKaTalk";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.popInput.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elseToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel sbLabel1;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip popInput;
        private System.Windows.Forms.ToolStripMenuItem puSend2Server;
        private System.Windows.Forms.ToolStripMenuItem puSend2Client;
        private System.Windows.Forms.ToolStripMenuItem mnOpenServer;
        private System.Windows.Forms.ToolStripMenuItem mnCloseServer;
        private System.Windows.Forms.ToolStripMenuItem mnConnect2Server;
        private System.Windows.Forms.ToolStripDropDownButton sbClientList;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem mnNetworkConfig;
        private System.Windows.Forms.ToolStripMenuItem 출석표조회ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnAddStudent;
        private System.Windows.Forms.ToolStripMenuItem mnCurrTime;
        private System.Windows.Forms.ToolStripMenuItem mnAddDate;
        private System.Windows.Forms.ToolStripMenuItem mnShowDB;
        private System.Windows.Forms.ToolStripMenuItem mnDEBUG;
        private System.Windows.Forms.ToolStripMenuItem mnExitClass;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem FileSend;
    }
}

