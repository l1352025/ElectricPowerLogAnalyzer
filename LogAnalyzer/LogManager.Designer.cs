namespace LogAnalyzer
{
    partial class LogManager
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if(this._scom.IsOpen)
            {
                this._scom.Close();
            }

            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grpNetAnalysis = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.treeNwkAnalysis = new System.Windows.Forms.TreeView();
            this.btLoadNetInfo = new System.Windows.Forms.Button();
            this.btSaveNetInfo = new System.Windows.Forms.Button();
            this.btReadNetInfo = new System.Windows.Forms.Button();
            this.grpLogMgr = new System.Windows.Forms.GroupBox();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btLogLoad = new System.Windows.Forms.Button();
            this.combHourEnd = new System.Windows.Forms.ComboBox();
            this.combHourStart = new System.Windows.Forms.ComboBox();
            this.btLogSave = new System.Windows.Forms.Button();
            this.btLogRead = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rbReadByHour = new System.Windows.Forms.RadioButton();
            this.rbReadByDay = new System.Windows.Forms.RadioButton();
            this.rbReadByMth = new System.Windows.Forms.RadioButton();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.grpPortSet = new System.Windows.Forms.GroupBox();
            this.btPortCtrl = new System.Windows.Forms.Button();
            this.combPortChk = new System.Windows.Forms.ComboBox();
            this.combPortBaud = new System.Windows.Forms.ComboBox();
            this.combPortNum = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btShowStationData = new System.Windows.Forms.Button();
            this.combStationList = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpLogMgr2 = new System.Windows.Forms.GroupBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btFolderSelct = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtKeyWord = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.序号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.包号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.页IDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.下一条记录DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.帧长DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eCC错误DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.时间DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.类别DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.源地址DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.目的地址DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.帧类型DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.备注DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctMenuLogList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.保存日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清空ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dsLog = new System.Data.DataSet();
            this.dtLog = new System.Data.DataTable();
            this.序号 = new System.Data.DataColumn();
            this.包号 = new System.Data.DataColumn();
            this.日志页ID = new System.Data.DataColumn();
            this.下一条记录 = new System.Data.DataColumn();
            this.帧长 = new System.Data.DataColumn();
            this.ECC错误 = new System.Data.DataColumn();
            this.时间 = new System.Data.DataColumn();
            this.日志类别 = new System.Data.DataColumn();
            this.日志数据 = new System.Data.DataColumn();
            this.源地址 = new System.Data.DataColumn();
            this.目的地址 = new System.Data.DataColumn();
            this.帧类型 = new System.Data.DataColumn();
            this.备注 = new System.Data.DataColumn();
            this.日志帧 = new System.Data.DataColumn();
            this.btFindKeyWord = new System.Windows.Forms.Button();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tStrpLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tStrpProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.rtbLogText = new System.Windows.Forms.RichTextBox();
            this.treeProtocol = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDlg = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpNetAnalysis.SuspendLayout();
            this.grpLogMgr.SuspendLayout();
            this.grpPortSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.grpLogMgr2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            this.ctMenuLogList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLog)).BeginInit();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grpNetAnalysis);
            this.splitContainer1.Panel1.Controls.Add(this.grpLogMgr);
            this.splitContainer1.Panel1.Controls.Add(this.grpPortSet);
            this.splitContainer1.Panel1MinSize = 228;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1362, 764);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // grpNetAnalysis
            // 
            this.grpNetAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNetAnalysis.Controls.Add(this.label5);
            this.grpNetAnalysis.Controls.Add(this.treeNwkAnalysis);
            this.grpNetAnalysis.Controls.Add(this.btLoadNetInfo);
            this.grpNetAnalysis.Controls.Add(this.btSaveNetInfo);
            this.grpNetAnalysis.Controls.Add(this.btReadNetInfo);
            this.grpNetAnalysis.Location = new System.Drawing.Point(4, 269);
            this.grpNetAnalysis.Margin = new System.Windows.Forms.Padding(0);
            this.grpNetAnalysis.Name = "grpNetAnalysis";
            this.grpNetAnalysis.Padding = new System.Windows.Forms.Padding(4);
            this.grpNetAnalysis.Size = new System.Drawing.Size(222, 493);
            this.grpNetAnalysis.TabIndex = 0;
            this.grpNetAnalysis.TabStop = false;
            this.grpNetAnalysis.Text = "台区分析";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "最近一次组网信息";
            // 
            // treeNwkAnalysis
            // 
            this.treeNwkAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeNwkAnalysis.Location = new System.Drawing.Point(-5, 64);
            this.treeNwkAnalysis.Margin = new System.Windows.Forms.Padding(0);
            this.treeNwkAnalysis.Name = "treeNwkAnalysis";
            this.treeNwkAnalysis.Size = new System.Drawing.Size(227, 429);
            this.treeNwkAnalysis.TabIndex = 0;
            // 
            // btLoadNetInfo
            // 
            this.btLoadNetInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btLoadNetInfo.Location = new System.Drawing.Point(179, 38);
            this.btLoadNetInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btLoadNetInfo.Name = "btLoadNetInfo";
            this.btLoadNetInfo.Size = new System.Drawing.Size(36, 22);
            this.btLoadNetInfo.TabIndex = 2;
            this.btLoadNetInfo.Text = "导入";
            this.btLoadNetInfo.UseVisualStyleBackColor = true;
            this.btLoadNetInfo.Click += new System.EventHandler(this.btLoadNetInfo_Click);
            // 
            // btSaveNetInfo
            // 
            this.btSaveNetInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btSaveNetInfo.Location = new System.Drawing.Point(134, 38);
            this.btSaveNetInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btSaveNetInfo.Name = "btSaveNetInfo";
            this.btSaveNetInfo.Size = new System.Drawing.Size(37, 22);
            this.btSaveNetInfo.TabIndex = 2;
            this.btSaveNetInfo.Text = "保存";
            this.btSaveNetInfo.UseVisualStyleBackColor = true;
            this.btSaveNetInfo.Click += new System.EventHandler(this.btSaveNetInfo_Click);
            // 
            // btReadNetInfo
            // 
            this.btReadNetInfo.Enabled = false;
            this.btReadNetInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btReadNetInfo.Location = new System.Drawing.Point(81, 38);
            this.btReadNetInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btReadNetInfo.Name = "btReadNetInfo";
            this.btReadNetInfo.Size = new System.Drawing.Size(39, 22);
            this.btReadNetInfo.TabIndex = 2;
            this.btReadNetInfo.Text = "读取";
            this.btReadNetInfo.UseVisualStyleBackColor = true;
            this.btReadNetInfo.Click += new System.EventHandler(this.btReadNetInfo_Click);
            // 
            // grpLogMgr
            // 
            this.grpLogMgr.Controls.Add(this.chkAutoSave);
            this.grpLogMgr.Controls.Add(this.label4);
            this.grpLogMgr.Controls.Add(this.btLogLoad);
            this.grpLogMgr.Controls.Add(this.combHourEnd);
            this.grpLogMgr.Controls.Add(this.combHourStart);
            this.grpLogMgr.Controls.Add(this.btLogSave);
            this.grpLogMgr.Controls.Add(this.btLogRead);
            this.grpLogMgr.Controls.Add(this.label2);
            this.grpLogMgr.Controls.Add(this.rbReadByHour);
            this.grpLogMgr.Controls.Add(this.rbReadByDay);
            this.grpLogMgr.Controls.Add(this.rbReadByMth);
            this.grpLogMgr.Controls.Add(this.dtPicker);
            this.grpLogMgr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.grpLogMgr.Location = new System.Drawing.Point(4, 95);
            this.grpLogMgr.Margin = new System.Windows.Forms.Padding(4);
            this.grpLogMgr.Name = "grpLogMgr";
            this.grpLogMgr.Padding = new System.Windows.Forms.Padding(4);
            this.grpLogMgr.Size = new System.Drawing.Size(220, 170);
            this.grpLogMgr.TabIndex = 0;
            this.grpLogMgr.TabStop = false;
            this.grpLogMgr.Text = "日志管理";
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAutoSave.Location = new System.Drawing.Point(83, 146);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(64, 14);
            this.chkAutoSave.TabIndex = 7;
            this.chkAutoSave.Text = "自动保存";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            this.chkAutoSave.CheckedChanged += new System.EventHandler(this.chkAutoSave_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(132, 96);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "->";
            // 
            // btLogLoad
            // 
            this.btLogLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btLogLoad.Location = new System.Drawing.Point(151, 119);
            this.btLogLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btLogLoad.Name = "btLogLoad";
            this.btLogLoad.Size = new System.Drawing.Size(54, 20);
            this.btLogLoad.TabIndex = 2;
            this.btLogLoad.Text = "导入日志";
            this.btLogLoad.UseVisualStyleBackColor = true;
            this.btLogLoad.Click += new System.EventHandler(this.btLogLoad_Click);
            // 
            // combHourEnd
            // 
            this.combHourEnd.FormattingEnabled = true;
            this.combHourEnd.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.combHourEnd.Location = new System.Drawing.Point(155, 91);
            this.combHourEnd.Margin = new System.Windows.Forms.Padding(4);
            this.combHourEnd.Name = "combHourEnd";
            this.combHourEnd.Size = new System.Drawing.Size(49, 20);
            this.combHourEnd.TabIndex = 1;
            // 
            // combHourStart
            // 
            this.combHourStart.FormattingEnabled = true;
            this.combHourStart.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.combHourStart.Location = new System.Drawing.Point(83, 90);
            this.combHourStart.Margin = new System.Windows.Forms.Padding(4);
            this.combHourStart.Name = "combHourStart";
            this.combHourStart.Size = new System.Drawing.Size(48, 20);
            this.combHourStart.TabIndex = 1;
            this.combHourStart.SelectedIndexChanged += new System.EventHandler(this.combHourStart_SelectedIndexChanged);
            // 
            // btLogSave
            // 
            this.btLogSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btLogSave.Location = new System.Drawing.Point(83, 118);
            this.btLogSave.Margin = new System.Windows.Forms.Padding(4);
            this.btLogSave.Name = "btLogSave";
            this.btLogSave.Size = new System.Drawing.Size(59, 21);
            this.btLogSave.TabIndex = 2;
            this.btLogSave.Text = "保存日志";
            this.btLogSave.UseVisualStyleBackColor = true;
            this.btLogSave.Click += new System.EventHandler(this.btLogSave_Click);
            // 
            // btLogRead
            // 
            this.btLogRead.Enabled = false;
            this.btLogRead.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btLogRead.Location = new System.Drawing.Point(21, 118);
            this.btLogRead.Margin = new System.Windows.Forms.Padding(4);
            this.btLogRead.Name = "btLogRead";
            this.btLogRead.Size = new System.Drawing.Size(56, 21);
            this.btLogRead.TabIndex = 2;
            this.btLogRead.Text = "读取日志";
            this.btLogRead.UseVisualStyleBackColor = true;
            this.btLogRead.Click += new System.EventHandler(this.btLogRead_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 29);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "读取规则";
            // 
            // rbReadByHour
            // 
            this.rbReadByHour.AutoSize = true;
            this.rbReadByHour.Checked = true;
            this.rbReadByHour.Location = new System.Drawing.Point(26, 94);
            this.rbReadByHour.Margin = new System.Windows.Forms.Padding(4);
            this.rbReadByHour.Name = "rbReadByHour";
            this.rbReadByHour.Size = new System.Drawing.Size(47, 16);
            this.rbReadByHour.TabIndex = 2;
            this.rbReadByHour.TabStop = true;
            this.rbReadByHour.Text = "按时";
            this.rbReadByHour.UseVisualStyleBackColor = true;
            this.rbReadByHour.CheckedChanged += new System.EventHandler(this.rbReadByHour_CheckedChanged);
            // 
            // rbReadByDay
            // 
            this.rbReadByDay.AutoSize = true;
            this.rbReadByDay.Location = new System.Drawing.Point(26, 73);
            this.rbReadByDay.Margin = new System.Windows.Forms.Padding(4);
            this.rbReadByDay.Name = "rbReadByDay";
            this.rbReadByDay.Size = new System.Drawing.Size(47, 16);
            this.rbReadByDay.TabIndex = 2;
            this.rbReadByDay.Text = "按日";
            this.rbReadByDay.UseVisualStyleBackColor = true;
            // 
            // rbReadByMth
            // 
            this.rbReadByMth.AutoSize = true;
            this.rbReadByMth.Location = new System.Drawing.Point(26, 49);
            this.rbReadByMth.Margin = new System.Windows.Forms.Padding(4);
            this.rbReadByMth.Name = "rbReadByMth";
            this.rbReadByMth.Size = new System.Drawing.Size(47, 16);
            this.rbReadByMth.TabIndex = 2;
            this.rbReadByMth.Text = "按月";
            this.rbReadByMth.UseVisualStyleBackColor = true;
            // 
            // dtPicker
            // 
            this.dtPicker.CustomFormat = "";
            this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtPicker.Location = new System.Drawing.Point(85, 51);
            this.dtPicker.Margin = new System.Windows.Forms.Padding(4);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(119, 21);
            this.dtPicker.TabIndex = 0;
            // 
            // grpPortSet
            // 
            this.grpPortSet.Controls.Add(this.btPortCtrl);
            this.grpPortSet.Controls.Add(this.combPortChk);
            this.grpPortSet.Controls.Add(this.combPortBaud);
            this.grpPortSet.Controls.Add(this.combPortNum);
            this.grpPortSet.Controls.Add(this.label1);
            this.grpPortSet.Location = new System.Drawing.Point(4, 9);
            this.grpPortSet.Margin = new System.Windows.Forms.Padding(4);
            this.grpPortSet.Name = "grpPortSet";
            this.grpPortSet.Padding = new System.Windows.Forms.Padding(4);
            this.grpPortSet.Size = new System.Drawing.Size(222, 78);
            this.grpPortSet.TabIndex = 0;
            this.grpPortSet.TabStop = false;
            this.grpPortSet.Text = "端口设置";
            // 
            // btPortCtrl
            // 
            this.btPortCtrl.BackColor = System.Drawing.Color.Silver;
            this.btPortCtrl.Location = new System.Drawing.Point(138, 52);
            this.btPortCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.btPortCtrl.Name = "btPortCtrl";
            this.btPortCtrl.Size = new System.Drawing.Size(66, 21);
            this.btPortCtrl.TabIndex = 2;
            this.btPortCtrl.Text = "打开";
            this.btPortCtrl.UseVisualStyleBackColor = false;
            this.btPortCtrl.Click += new System.EventHandler(this.btPortCtrl_Click);
            // 
            // combPortChk
            // 
            this.combPortChk.FormattingEnabled = true;
            this.combPortChk.Items.AddRange(new object[] {
            "8N1",
            "8E1",
            "8O1"});
            this.combPortChk.Location = new System.Drawing.Point(138, 24);
            this.combPortChk.Margin = new System.Windows.Forms.Padding(4);
            this.combPortChk.Name = "combPortChk";
            this.combPortChk.Size = new System.Drawing.Size(66, 20);
            this.combPortChk.TabIndex = 1;
            // 
            // combPortBaud
            // 
            this.combPortBaud.FormattingEnabled = true;
            this.combPortBaud.Items.AddRange(new object[] {
            "9600",
            "19200",
            "115200"});
            this.combPortBaud.Location = new System.Drawing.Point(54, 52);
            this.combPortBaud.Margin = new System.Windows.Forms.Padding(4);
            this.combPortBaud.Name = "combPortBaud";
            this.combPortBaud.Size = new System.Drawing.Size(66, 20);
            this.combPortBaud.TabIndex = 1;
            // 
            // combPortNum
            // 
            this.combPortNum.FormattingEnabled = true;
            this.combPortNum.Items.AddRange(new object[] {
            "COM15"});
            this.combPortNum.Location = new System.Drawing.Point(54, 24);
            this.combPortNum.Margin = new System.Windows.Forms.Padding(4);
            this.combPortNum.Name = "combPortNum";
            this.combPortNum.Size = new System.Drawing.Size(66, 20);
            this.combPortNum.TabIndex = 1;
            this.combPortNum.Click += new System.EventHandler(this.combPortNum_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.treeProtocol);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Size = new System.Drawing.Size(1129, 764);
            this.splitContainer2.SplitterDistance = 857;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btShowStationData);
            this.splitContainer3.Panel1.Controls.Add(this.combStationList);
            this.splitContainer3.Panel1.Controls.Add(this.label7);
            this.splitContainer3.Panel1.Controls.Add(this.grpLogMgr2);
            this.splitContainer3.Panel1.Controls.Add(this.txtKeyWord);
            this.splitContainer3.Panel1.Controls.Add(this.label6);
            this.splitContainer3.Panel1.Controls.Add(this.dgvLog);
            this.splitContainer3.Panel1.Controls.Add(this.btFindKeyWord);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.statusBar);
            this.splitContainer3.Panel2.Controls.Add(this.rtbLogText);
            this.splitContainer3.Size = new System.Drawing.Size(857, 764);
            this.splitContainer3.SplitterDistance = 632;
            this.splitContainer3.SplitterWidth = 5;
            this.splitContainer3.TabIndex = 0;
            // 
            // btShowStationData
            // 
            this.btShowStationData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btShowStationData.Location = new System.Drawing.Point(620, 605);
            this.btShowStationData.Name = "btShowStationData";
            this.btShowStationData.Size = new System.Drawing.Size(85, 22);
            this.btShowStationData.TabIndex = 6;
            this.btShowStationData.Text = "近况详情查看";
            this.btShowStationData.UseVisualStyleBackColor = true;
            this.btShowStationData.Click += new System.EventHandler(this.btShowStationData_Click);
            // 
            // combStationList
            // 
            this.combStationList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.combStationList.FormattingEnabled = true;
            this.combStationList.Location = new System.Drawing.Point(503, 606);
            this.combStationList.Margin = new System.Windows.Forms.Padding(4);
            this.combStationList.Name = "combStationList";
            this.combStationList.Size = new System.Drawing.Size(110, 20);
            this.combStationList.TabIndex = 3;
            this.combStationList.SelectedIndexChanged += new System.EventHandler(this.combStationList_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(435, 611);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "集中器地址";
            // 
            // grpLogMgr2
            // 
            this.grpLogMgr2.Controls.Add(this.txtPath);
            this.grpLogMgr2.Controls.Add(this.btFolderSelct);
            this.grpLogMgr2.Controls.Add(this.label8);
            this.grpLogMgr2.Location = new System.Drawing.Point(22, 95);
            this.grpLogMgr2.Name = "grpLogMgr2";
            this.grpLogMgr2.Size = new System.Drawing.Size(222, 170);
            this.grpLogMgr2.TabIndex = 5;
            this.grpLogMgr2.TabStop = false;
            this.grpLogMgr2.Text = "日志管理";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(31, 48);
            this.txtPath.Multiline = true;
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(185, 60);
            this.txtPath.TabIndex = 2;
            // 
            // btFolderSelct
            // 
            this.btFolderSelct.Location = new System.Drawing.Point(31, 116);
            this.btFolderSelct.Name = "btFolderSelct";
            this.btFolderSelct.Size = new System.Drawing.Size(63, 23);
            this.btFolderSelct.TabIndex = 1;
            this.btFolderSelct.Text = "导入日志";
            this.btFolderSelct.UseVisualStyleBackColor = true;
            this.btFolderSelct.Click += new System.EventHandler(this.btFolderSelct_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "文件位置";
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtKeyWord.Location = new System.Drawing.Point(197, 605);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Size = new System.Drawing.Size(128, 21);
            this.txtKeyWord.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(153, 611);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "关键字";
            // 
            // dgvLog
            // 
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.AllowUserToResizeRows = false;
            this.dgvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLog.AutoGenerateColumns = false;
            this.dgvLog.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLog.ColumnHeadersHeight = 35;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.序号DataGridViewTextBoxColumn,
            this.包号DataGridViewTextBoxColumn,
            this.页IDDataGridViewTextBoxColumn,
            this.下一条记录DataGridViewTextBoxColumn,
            this.帧长DataGridViewTextBoxColumn,
            this.eCC错误DataGridViewTextBoxColumn,
            this.时间DataGridViewTextBoxColumn,
            this.类别DataGridViewTextBoxColumn,
            this.源地址DataGridViewTextBoxColumn,
            this.目的地址DataGridViewTextBoxColumn,
            this.帧类型DataGridViewTextBoxColumn,
            this.备注DataGridViewTextBoxColumn});
            this.dgvLog.ContextMenuStrip = this.ctMenuLogList;
            this.dgvLog.DataMember = "T_Log";
            this.dgvLog.DataSource = this.dsLog;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLog.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLog.GridColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvLog.Location = new System.Drawing.Point(3, 0);
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.ReadOnly = true;
            this.dgvLog.RowHeadersVisible = false;
            this.dgvLog.RowHeadersWidth = 20;
            this.dgvLog.RowTemplate.Height = 20;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.Size = new System.Drawing.Size(852, 599);
            this.dgvLog.StandardTab = true;
            this.dgvLog.TabIndex = 0;
            this.dgvLog.SelectionChanged += new System.EventHandler(this.dgvLog_SelectionChanged);
            // 
            // 序号DataGridViewTextBoxColumn
            // 
            this.序号DataGridViewTextBoxColumn.DataPropertyName = "序号";
            this.序号DataGridViewTextBoxColumn.HeaderText = "序号";
            this.序号DataGridViewTextBoxColumn.Name = "序号DataGridViewTextBoxColumn";
            this.序号DataGridViewTextBoxColumn.ReadOnly = true;
            this.序号DataGridViewTextBoxColumn.Width = 55;
            // 
            // 包号DataGridViewTextBoxColumn
            // 
            this.包号DataGridViewTextBoxColumn.DataPropertyName = "包号";
            this.包号DataGridViewTextBoxColumn.HeaderText = "包号";
            this.包号DataGridViewTextBoxColumn.Name = "包号DataGridViewTextBoxColumn";
            this.包号DataGridViewTextBoxColumn.ReadOnly = true;
            this.包号DataGridViewTextBoxColumn.Width = 55;
            // 
            // 页IDDataGridViewTextBoxColumn
            // 
            this.页IDDataGridViewTextBoxColumn.DataPropertyName = "页ID";
            this.页IDDataGridViewTextBoxColumn.HeaderText = "页ID";
            this.页IDDataGridViewTextBoxColumn.Name = "页IDDataGridViewTextBoxColumn";
            this.页IDDataGridViewTextBoxColumn.ReadOnly = true;
            this.页IDDataGridViewTextBoxColumn.Width = 55;
            // 
            // 下一条记录DataGridViewTextBoxColumn
            // 
            this.下一条记录DataGridViewTextBoxColumn.DataPropertyName = "下一条记录";
            this.下一条记录DataGridViewTextBoxColumn.HeaderText = "下一条记录";
            this.下一条记录DataGridViewTextBoxColumn.Name = "下一条记录DataGridViewTextBoxColumn";
            this.下一条记录DataGridViewTextBoxColumn.ReadOnly = true;
            this.下一条记录DataGridViewTextBoxColumn.Width = 70;
            // 
            // 帧长DataGridViewTextBoxColumn
            // 
            this.帧长DataGridViewTextBoxColumn.DataPropertyName = "帧长";
            this.帧长DataGridViewTextBoxColumn.HeaderText = "帧长";
            this.帧长DataGridViewTextBoxColumn.Name = "帧长DataGridViewTextBoxColumn";
            this.帧长DataGridViewTextBoxColumn.ReadOnly = true;
            this.帧长DataGridViewTextBoxColumn.Width = 55;
            // 
            // eCC错误DataGridViewTextBoxColumn
            // 
            this.eCC错误DataGridViewTextBoxColumn.DataPropertyName = "ECC错误";
            this.eCC错误DataGridViewTextBoxColumn.HeaderText = "ECC错误";
            this.eCC错误DataGridViewTextBoxColumn.Name = "eCC错误DataGridViewTextBoxColumn";
            this.eCC错误DataGridViewTextBoxColumn.ReadOnly = true;
            this.eCC错误DataGridViewTextBoxColumn.Width = 55;
            // 
            // 时间DataGridViewTextBoxColumn
            // 
            this.时间DataGridViewTextBoxColumn.DataPropertyName = "时间";
            this.时间DataGridViewTextBoxColumn.HeaderText = "时间";
            this.时间DataGridViewTextBoxColumn.Name = "时间DataGridViewTextBoxColumn";
            this.时间DataGridViewTextBoxColumn.ReadOnly = true;
            this.时间DataGridViewTextBoxColumn.Width = 125;
            // 
            // 类别DataGridViewTextBoxColumn
            // 
            this.类别DataGridViewTextBoxColumn.DataPropertyName = "类别";
            this.类别DataGridViewTextBoxColumn.HeaderText = "类别";
            this.类别DataGridViewTextBoxColumn.Name = "类别DataGridViewTextBoxColumn";
            this.类别DataGridViewTextBoxColumn.ReadOnly = true;
            this.类别DataGridViewTextBoxColumn.Width = 65;
            // 
            // 源地址DataGridViewTextBoxColumn
            // 
            this.源地址DataGridViewTextBoxColumn.DataPropertyName = "源地址";
            this.源地址DataGridViewTextBoxColumn.HeaderText = "源地址";
            this.源地址DataGridViewTextBoxColumn.Name = "源地址DataGridViewTextBoxColumn";
            this.源地址DataGridViewTextBoxColumn.ReadOnly = true;
            this.源地址DataGridViewTextBoxColumn.Width = 85;
            // 
            // 目的地址DataGridViewTextBoxColumn
            // 
            this.目的地址DataGridViewTextBoxColumn.DataPropertyName = "目的地址";
            this.目的地址DataGridViewTextBoxColumn.HeaderText = "目的地址";
            this.目的地址DataGridViewTextBoxColumn.Name = "目的地址DataGridViewTextBoxColumn";
            this.目的地址DataGridViewTextBoxColumn.ReadOnly = true;
            this.目的地址DataGridViewTextBoxColumn.Width = 85;
            // 
            // 帧类型DataGridViewTextBoxColumn
            // 
            this.帧类型DataGridViewTextBoxColumn.DataPropertyName = "帧类型";
            this.帧类型DataGridViewTextBoxColumn.HeaderText = "帧类型";
            this.帧类型DataGridViewTextBoxColumn.Name = "帧类型DataGridViewTextBoxColumn";
            this.帧类型DataGridViewTextBoxColumn.ReadOnly = true;
            this.帧类型DataGridViewTextBoxColumn.Width = 150;
            // 
            // 备注DataGridViewTextBoxColumn
            // 
            this.备注DataGridViewTextBoxColumn.DataPropertyName = "备注";
            this.备注DataGridViewTextBoxColumn.HeaderText = "备注";
            this.备注DataGridViewTextBoxColumn.Name = "备注DataGridViewTextBoxColumn";
            this.备注DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ctMenuLogList
            // 
            this.ctMenuLogList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctMenuLogList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存日志ToolStripMenuItem,
            this.导入ToolStripMenuItem,
            this.清空ToolStripMenuItem});
            this.ctMenuLogList.Name = "ctMenuLogList";
            this.ctMenuLogList.Size = new System.Drawing.Size(101, 70);
            // 
            // 保存日志ToolStripMenuItem
            // 
            this.保存日志ToolStripMenuItem.Name = "保存日志ToolStripMenuItem";
            this.保存日志ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.保存日志ToolStripMenuItem.Text = "保存";
            this.保存日志ToolStripMenuItem.Click += new System.EventHandler(this.保存日志ToolStripMenuItem_Click);
            // 
            // 导入ToolStripMenuItem
            // 
            this.导入ToolStripMenuItem.Name = "导入ToolStripMenuItem";
            this.导入ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.导入ToolStripMenuItem.Text = "导入";
            this.导入ToolStripMenuItem.Click += new System.EventHandler(this.导入ToolStripMenuItem_Click);
            // 
            // 清空ToolStripMenuItem
            // 
            this.清空ToolStripMenuItem.Name = "清空ToolStripMenuItem";
            this.清空ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.清空ToolStripMenuItem.Text = "清空";
            this.清空ToolStripMenuItem.Click += new System.EventHandler(this.清空ToolStripMenuItem_Click);
            // 
            // dsLog
            // 
            this.dsLog.DataSetName = "dataset";
            this.dsLog.Tables.AddRange(new System.Data.DataTable[] {
            this.dtLog});
            // 
            // dtLog
            // 
            this.dtLog.Columns.AddRange(new System.Data.DataColumn[] {
            this.序号,
            this.包号,
            this.日志页ID,
            this.下一条记录,
            this.帧长,
            this.ECC错误,
            this.时间,
            this.日志类别,
            this.日志数据,
            this.源地址,
            this.目的地址,
            this.帧类型,
            this.备注,
            this.日志帧});
            this.dtLog.TableName = "T_Log";
            // 
            // 序号
            // 
            this.序号.AllowDBNull = false;
            this.序号.ColumnName = "序号";
            // 
            // 包号
            // 
            this.包号.ColumnName = "包号";
            // 
            // 日志页ID
            // 
            this.日志页ID.ColumnName = "页ID";
            // 
            // 下一条记录
            // 
            this.下一条记录.ColumnName = "下一条记录";
            // 
            // 帧长
            // 
            this.帧长.ColumnName = "帧长";
            // 
            // ECC错误
            // 
            this.ECC错误.ColumnName = "ECC错误";
            // 
            // 时间
            // 
            this.时间.ColumnName = "时间";
            // 
            // 日志类别
            // 
            this.日志类别.ColumnName = "类别";
            // 
            // 日志数据
            // 
            this.日志数据.ColumnName = "日志数据";
            this.日志数据.DataType = typeof(byte[]);
            // 
            // 源地址
            // 
            this.源地址.ColumnName = "源地址";
            // 
            // 目的地址
            // 
            this.目的地址.ColumnName = "目的地址";
            // 
            // 帧类型
            // 
            this.帧类型.ColumnName = "帧类型";
            // 
            // 备注
            // 
            this.备注.ColumnName = "备注";
            // 
            // 日志帧
            // 
            this.日志帧.ColumnName = "日志帧";
            this.日志帧.DataType = typeof(byte[]);
            // 
            // btFindKeyWord
            // 
            this.btFindKeyWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btFindKeyWord.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btFindKeyWord.Location = new System.Drawing.Point(332, 605);
            this.btFindKeyWord.Margin = new System.Windows.Forms.Padding(4);
            this.btFindKeyWord.Name = "btFindKeyWord";
            this.btFindKeyWord.Size = new System.Drawing.Size(59, 21);
            this.btFindKeyWord.TabIndex = 2;
            this.btFindKeyWord.Text = "查找";
            this.btFindKeyWord.UseVisualStyleBackColor = true;
            this.btFindKeyWord.Click += new System.EventHandler(this.btFindKeyWord_Click);
            // 
            // statusBar
            // 
            this.statusBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusBar.AutoSize = false;
            this.statusBar.Dock = System.Windows.Forms.DockStyle.None;
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tStrpLabel,
            this.tStrpProgress});
            this.statusBar.Location = new System.Drawing.Point(1, 102);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(855, 23);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "状态栏";
            // 
            // tStrpLabel
            // 
            this.tStrpLabel.AutoSize = false;
            this.tStrpLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.tStrpLabel.Name = "tStrpLabel";
            this.tStrpLabel.Size = new System.Drawing.Size(400, 19);
            this.tStrpLabel.Text = "状态：";
            this.tStrpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tStrpProgress
            // 
            this.tStrpProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tStrpProgress.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.tStrpProgress.Name = "tStrpProgress";
            this.tStrpProgress.Size = new System.Drawing.Size(360, 19);
            // 
            // rtbLogText
            // 
            this.rtbLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogText.Location = new System.Drawing.Point(-3, -3);
            this.rtbLogText.Margin = new System.Windows.Forms.Padding(4);
            this.rtbLogText.Name = "rtbLogText";
            this.rtbLogText.Size = new System.Drawing.Size(858, 101);
            this.rtbLogText.TabIndex = 0;
            this.rtbLogText.Text = "";
            // 
            // treeProtocol
            // 
            this.treeProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeProtocol.Location = new System.Drawing.Point(-2, 33);
            this.treeProtocol.Margin = new System.Windows.Forms.Padding(0);
            this.treeProtocol.Name = "treeProtocol";
            this.treeProtocol.Size = new System.Drawing.Size(267, 730);
            this.treeProtocol.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "协议解析";
            // 
            // openFileDlg
            // 
            this.openFileDlg.FileName = "openFileDlg";
            // 
            // LogManager
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "LogManager";
            this.Size = new System.Drawing.Size(1366, 768);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpNetAnalysis.ResumeLayout(false);
            this.grpNetAnalysis.PerformLayout();
            this.grpLogMgr.ResumeLayout(false);
            this.grpLogMgr.PerformLayout();
            this.grpPortSet.ResumeLayout(false);
            this.grpPortSet.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.grpLogMgr2.ResumeLayout(false);
            this.grpLogMgr2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.ctMenuLogList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLog)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpLogMgr;
        private System.Windows.Forms.GroupBox grpPortSet;
        private System.Windows.Forms.GroupBox grpNetAnalysis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbReadByHour;
        private System.Windows.Forms.RadioButton rbReadByDay;
        private System.Windows.Forms.RadioButton rbReadByMth;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.Button btPortCtrl;
        private System.Windows.Forms.ComboBox combPortChk;
        private System.Windows.Forms.ComboBox combPortBaud;
        private System.Windows.Forms.ComboBox combPortNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btLogLoad;
        private System.Windows.Forms.Button btLogSave;
        private System.Windows.Forms.Button btLogRead;
        private System.Windows.Forms.TreeView treeNwkAnalysis;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox rtbLogText;
        private System.Windows.Forms.TreeView treeProtocol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combHourStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox combHourEnd;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.SaveFileDialog saveFileDlg;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Data.DataSet dsLog;
        private System.Data.DataTable dtLog;
        private System.Data.DataColumn 序号;
        private System.Data.DataColumn 包号;
        private System.Data.DataColumn 日志页ID;
        private System.Data.DataColumn 下一条记录;
        private System.Data.DataColumn 帧长;
        private System.Data.DataColumn ECC错误;
        private System.Data.DataColumn 时间;
        private System.Data.DataColumn 日志类别;
        private System.Data.DataColumn 日志数据;
        private System.Data.DataColumn 源地址;
        private System.Data.DataColumn 目的地址;
        private System.Data.DataColumn 帧类型;
        private System.Data.DataColumn 备注;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel tStrpLabel;
        private System.Windows.Forms.ToolStripProgressBar tStrpProgress;
        private System.Windows.Forms.ContextMenuStrip ctMenuLogList;
        private System.Windows.Forms.ToolStripMenuItem 保存日志ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清空ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 包号DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 页IDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 下一条记录DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 帧长DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eCC错误DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 时间DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 类别DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 源地址DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 目的地址DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 帧类型DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 备注DataGridViewTextBoxColumn;
        private System.Data.DataColumn 日志帧;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.Button btLoadNetInfo;
        private System.Windows.Forms.Button btSaveNetInfo;
        private System.Windows.Forms.Button btReadNetInfo;
        private System.Windows.Forms.TextBox txtKeyWord;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btFindKeyWord;
        private System.Windows.Forms.ComboBox combStationList;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpLogMgr2;
        private System.Windows.Forms.Button btFolderSelct;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btShowStationData;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDlg;
    }
}
