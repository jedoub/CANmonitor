namespace CANmonitor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.kvaserSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createANewConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCANMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.associateDBCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTPMsgsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveEMSnACMFaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sWVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kvaserDriverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CANstatus_cB = new System.Windows.Forms.CheckBox();
            this.SWversLbl = new System.Windows.Forms.Label();
            this.GUItimer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.updateIntervalLbl = new System.Windows.Forms.Label();
            this.openFD = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.BB1btn = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.ESNbtn = new System.Windows.Forms.Button();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.TDlbl = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kvaserSetupToolStripMenuItem,
            this.associateDBCToolStripMenuItem,
            this.viewTPMsgsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1281, 33);
            this.menuStrip1.TabIndex = 51;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // kvaserSetupToolStripMenuItem
            // 
            this.kvaserSetupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createANewConfigToolStripMenuItem,
            this.viewCANMappingToolStripMenuItem});
            this.kvaserSetupToolStripMenuItem.Name = "kvaserSetupToolStripMenuItem";
            this.kvaserSetupToolStripMenuItem.Size = new System.Drawing.Size(122, 29);
            this.kvaserSetupToolStripMenuItem.Text = "CAN Config";
            // 
            // createANewConfigToolStripMenuItem
            // 
            this.createANewConfigToolStripMenuItem.Name = "createANewConfigToolStripMenuItem";
            this.createANewConfigToolStripMenuItem.Size = new System.Drawing.Size(276, 34);
            this.createANewConfigToolStripMenuItem.Text = "Create a New Config";
            this.createANewConfigToolStripMenuItem.Click += new System.EventHandler(this.createANewConfigToolStripMenuItem_Click);
            // 
            // viewCANMappingToolStripMenuItem
            // 
            this.viewCANMappingToolStripMenuItem.Name = "viewCANMappingToolStripMenuItem";
            this.viewCANMappingToolStripMenuItem.Size = new System.Drawing.Size(276, 34);
            this.viewCANMappingToolStripMenuItem.Text = "View CAN Mapping";
            this.viewCANMappingToolStripMenuItem.Click += new System.EventHandler(this.viewCANMappingToolStripMenuItem_Click);
            // 
            // associateDBCToolStripMenuItem
            // 
            this.associateDBCToolStripMenuItem.Name = "associateDBCToolStripMenuItem";
            this.associateDBCToolStripMenuItem.Size = new System.Drawing.Size(142, 29);
            this.associateDBCToolStripMenuItem.Text = "Associate DBC";
            this.associateDBCToolStripMenuItem.Click += new System.EventHandler(this.associateDBCToolStripMenuItem_Click);
            // 
            // viewTPMsgsToolStripMenuItem
            // 
            this.viewTPMsgsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveEMSnACMFaultsToolStripMenuItem});
            this.viewTPMsgsToolStripMenuItem.Name = "viewTPMsgsToolStripMenuItem";
            this.viewTPMsgsToolStripMenuItem.Size = new System.Drawing.Size(149, 29);
            this.viewTPMsgsToolStripMenuItem.Text = "View DTC Msgs";
            this.viewTPMsgsToolStripMenuItem.Click += new System.EventHandler(this.viewTPMsgsToolStripMenuItem_Click);
            // 
            // saveEMSnACMFaultsToolStripMenuItem
            // 
            this.saveEMSnACMFaultsToolStripMenuItem.Enabled = false;
            this.saveEMSnACMFaultsToolStripMenuItem.Name = "saveEMSnACMFaultsToolStripMenuItem";
            this.saveEMSnACMFaultsToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.saveEMSnACMFaultsToolStripMenuItem.Text = "Save File";
            this.saveEMSnACMFaultsToolStripMenuItem.Click += new System.EventHandler(this.saveEMSnACMFaultsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sWVersionToolStripMenuItem,
            this.kvaserDriverToolStripMenuItem,
            this.configHintsToolStripMenuItem,
            this.releaseNotesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // sWVersionToolStripMenuItem
            // 
            this.sWVersionToolStripMenuItem.Name = "sWVersionToolStripMenuItem";
            this.sWVersionToolStripMenuItem.Size = new System.Drawing.Size(224, 34);
            this.sWVersionToolStripMenuItem.Text = "SW Version";
            this.sWVersionToolStripMenuItem.Click += new System.EventHandler(this.sWVersionToolStripMenuItem_Click_1);
            // 
            // kvaserDriverToolStripMenuItem
            // 
            this.kvaserDriverToolStripMenuItem.Name = "kvaserDriverToolStripMenuItem";
            this.kvaserDriverToolStripMenuItem.Size = new System.Drawing.Size(224, 34);
            this.kvaserDriverToolStripMenuItem.Text = "Kvaser Driver";
            this.kvaserDriverToolStripMenuItem.Click += new System.EventHandler(this.kvaserDriverToolStripMenuItem_Click);
            // 
            // configHintsToolStripMenuItem
            // 
            this.configHintsToolStripMenuItem.Name = "configHintsToolStripMenuItem";
            this.configHintsToolStripMenuItem.Size = new System.Drawing.Size(224, 34);
            this.configHintsToolStripMenuItem.Text = "Config Hints";
            this.configHintsToolStripMenuItem.Click += new System.EventHandler(this.configHintsToolStripMenuItem_Click_1);
            // 
            // releaseNotesToolStripMenuItem
            // 
            this.releaseNotesToolStripMenuItem.Name = "releaseNotesToolStripMenuItem";
            this.releaseNotesToolStripMenuItem.Size = new System.Drawing.Size(224, 34);
            this.releaseNotesToolStripMenuItem.Text = "Release Notes";
            this.releaseNotesToolStripMenuItem.Click += new System.EventHandler(this.releaseNotesToolStripMenuItem_Click);
            // 
            // CANstatus_cB
            // 
            this.CANstatus_cB.AutoSize = true;
            this.CANstatus_cB.Enabled = false;
            this.CANstatus_cB.Font = new System.Drawing.Font("Lucida Console", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CANstatus_cB.Location = new System.Drawing.Point(507, 47);
            this.CANstatus_cB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CANstatus_cB.Name = "CANstatus_cB";
            this.CANstatus_cB.Size = new System.Drawing.Size(137, 21);
            this.CANstatus_cB.TabIndex = 55;
            this.CANstatus_cB.Text = "CAN HW Status";
            this.CANstatus_cB.UseVisualStyleBackColor = true;
            // 
            // SWversLbl
            // 
            this.SWversLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SWversLbl.AutoSize = true;
            this.SWversLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SWversLbl.Location = new System.Drawing.Point(18, 1383);
            this.SWversLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SWversLbl.Name = "SWversLbl";
            this.SWversLbl.Size = new System.Drawing.Size(51, 15);
            this.SWversLbl.TabIndex = 56;
            this.SWversLbl.Text = "SWvers:";
            // 
            // GUItimer
            // 
            this.GUItimer.Interval = 50;
            this.GUItimer.Tick += new System.EventHandler(this.GUItimer_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 62);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 46);
            this.button1.TabIndex = 57;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // updateIntervalLbl
            // 
            this.updateIntervalLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateIntervalLbl.AutoSize = true;
            this.updateIntervalLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateIntervalLbl.Location = new System.Drawing.Point(213, 1383);
            this.updateIntervalLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.updateIntervalLbl.Name = "updateIntervalLbl";
            this.updateIntervalLbl.Size = new System.Drawing.Size(153, 15);
            this.updateIntervalLbl.TabIndex = 58;
            this.updateIntervalLbl.Text = "multi-media Timer Interval:";
            // 
            // openFD
            // 
            this.openFD.FileName = "vtc1939_r10.dbc";
            this.openFD.Filter = "Database|*.dbc";
            this.openFD.InitialDirectory = "Environment.GetFolderPath(Environment.SpecialFolder.Personal) + \"\\\\CANmonitor\"";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Controls.Add(this.BB1btn);
            this.panel1.Controls.Add(this.richTextBox2);
            this.panel1.Controls.Add(this.ESNbtn);
            this.panel1.Controls.Add(this.richTextBox3);
            this.panel1.Location = new System.Drawing.Point(21, 116);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1234, 1241);
            this.panel1.TabIndex = 62;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Info;
            this.richTextBox1.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(20, 48);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1195, 1182);
            this.richTextBox1.TabIndex = 62;
            this.richTextBox1.Text = "BB1 DISPLAY";
            this.richTextBox1.WordWrap = false;
            this.richTextBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseUp);
            // 
            // BB1btn
            // 
            this.BB1btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BB1btn.Location = new System.Drawing.Point(981, 8);
            this.BB1btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BB1btn.Name = "BB1btn";
            this.BB1btn.Size = new System.Drawing.Size(94, 36);
            this.BB1btn.TabIndex = 66;
            this.BB1btn.Text = "CAN1";
            this.BB1btn.UseVisualStyleBackColor = true;
            this.BB1btn.Click += new System.EventHandler(this.BB1btn_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(20, 11);
            this.richTextBox2.Multiline = false;
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox2.Size = new System.Drawing.Size(913, 29);
            this.richTextBox2.TabIndex = 63;
            this.richTextBox2.Text = "";
            // 
            // ESNbtn
            // 
            this.ESNbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ESNbtn.Location = new System.Drawing.Point(1113, 8);
            this.ESNbtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ESNbtn.Name = "ESNbtn";
            this.ESNbtn.Size = new System.Drawing.Size(94, 36);
            this.ESNbtn.TabIndex = 65;
            this.ESNbtn.Text = "CAN2";
            this.ESNbtn.UseVisualStyleBackColor = true;
            this.ESNbtn.Click += new System.EventHandler(this.ESNbtn_Click);
            // 
            // richTextBox3
            // 
            this.richTextBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox3.BackColor = System.Drawing.SystemColors.Info;
            this.richTextBox3.Location = new System.Drawing.Point(28, 48);
            this.richTextBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.Size = new System.Drawing.Size(1179, 1183);
            this.richTextBox3.TabIndex = 64;
            this.richTextBox3.Text = "ESN DISPLAY";
            this.richTextBox3.Visible = false;
            this.richTextBox3.WordWrap = false;
            this.richTextBox3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBox3_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(167, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(291, 51);
            this.label1.TabIndex = 63;
            this.label1.Text = "To view the SPNs and their values of a PGN.\r\nStop the trace and double click the " +
    "J1939 ID.\r\nThen restart the CAN Trace.";
            // 
            // TDlbl
            // 
            this.TDlbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TDlbl.AutoSize = true;
            this.TDlbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TDlbl.Location = new System.Drawing.Point(1088, 1383);
            this.TDlbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TDlbl.Name = "TDlbl";
            this.TDlbl.Size = new System.Drawing.Size(36, 15);
            this.TDlbl.TabIndex = 64;
            this.TDlbl.Text = "TDlbl";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 1403);
            this.Controls.Add(this.TDlbl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.updateIntervalLbl);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SWversLbl);
            this.Controls.Add(this.CANstatus_cB);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CAN Trace";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem kvaserSetupToolStripMenuItem;
        private System.Windows.Forms.CheckBox CANstatus_cB;
        private System.Windows.Forms.Label SWversLbl;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sWVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kvaserDriverToolStripMenuItem;
        private System.Windows.Forms.Timer GUItimer;
        private System.Windows.Forms.ToolStripMenuItem createANewConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewCANMappingToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label updateIntervalLbl;
        private System.Windows.Forms.ToolStripMenuItem configHintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releaseNotesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem associateDBCToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFD;
        private System.Windows.Forms.ToolStripMenuItem viewTPMsgsToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button ESNbtn;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.Button BB1btn;
        private System.Windows.Forms.ToolStripMenuItem saveEMSnACMFaultsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TDlbl;
    }
}

