namespace CANsetup
{
    partial class CANsetupForm
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
            this.OK_Btn = new System.Windows.Forms.Button();
            this.CAN4Lbl = new System.Windows.Forms.Label();
            this.CAN_Init_btn = new System.Windows.Forms.Button();
            this.CAN4chnLbl = new System.Windows.Forms.Label();
            this.CAN1Lbl = new System.Windows.Forms.Label();
            this.chnFourSN_comboBox = new System.Windows.Forms.ComboBox();
            this.chnOne_comboBox = new System.Windows.Forms.ComboBox();
            this.chnOneSN_comboBox = new System.Windows.Forms.ComboBox();
            this.chnFour_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN1chnLbl = new System.Windows.Forms.Label();
            this.CAN3Lbl = new System.Windows.Forms.Label();
            this.CAN3chnLbl = new System.Windows.Forms.Label();
            this.chnThreeSN_comboBox = new System.Windows.Forms.ComboBox();
            this.chnThree_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN2chnLbl = new System.Windows.Forms.Label();
            this.chnTwoSN_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN2Lbl = new System.Windows.Forms.Label();
            this.chnTwo_comboBox = new System.Windows.Forms.ComboBox();
            this.applyCANconfigBtn = new System.Windows.Forms.Button();
            this.CANgroupBox2 = new System.Windows.Forms.GroupBox();
            this.chnOneBR_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN5Lbl = new System.Windows.Forms.Label();
            this.CAN5BdLbl = new System.Windows.Forms.Label();
            this.CAN5chnLbl = new System.Windows.Forms.Label();
            this.chnFiveBR_comboBox = new System.Windows.Forms.ComboBox();
            this.chnFiveSN_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN4BdLbl = new System.Windows.Forms.Label();
            this.chnFive_comboBox = new System.Windows.Forms.ComboBox();
            this.chnFourBR_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN1BdLbl = new System.Windows.Forms.Label();
            this.CAN3BdLbl = new System.Windows.Forms.Label();
            this.chnThreeBR_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN2BdLbl = new System.Windows.Forms.Label();
            this.chnTwoBR_comboBox = new System.Windows.Forms.ComboBox();
            this.CAN_HW_tB = new System.Windows.Forms.TextBox();
            this.exit_Btn = new System.Windows.Forms.Button();
            this.cancelLbl = new System.Windows.Forms.Label();
            this.directionLbl = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.refresh_Btn = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.CANgroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // OK_Btn
            // 
            this.OK_Btn.Location = new System.Drawing.Point(597, 483);
            this.OK_Btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.OK_Btn.Name = "OK_Btn";
            this.OK_Btn.Size = new System.Drawing.Size(112, 35);
            this.OK_Btn.TabIndex = 0;
            this.OK_Btn.Text = "OK";
            this.OK_Btn.UseVisualStyleBackColor = true;
            this.OK_Btn.Click += new System.EventHandler(this.OK_Btn_Click);
            // 
            // CAN4Lbl
            // 
            this.CAN4Lbl.AutoSize = true;
            this.CAN4Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN4Lbl.Location = new System.Drawing.Point(15, 237);
            this.CAN4Lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN4Lbl.Name = "CAN4Lbl";
            this.CAN4Lbl.Size = new System.Drawing.Size(228, 17);
            this.CAN4Lbl.TabIndex = 88;
            this.CAN4Lbl.Text = "Select CAN4 by Serial Number";
            // 
            // CAN_Init_btn
            // 
            this.CAN_Init_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN_Init_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CAN_Init_btn.Location = new System.Drawing.Point(42, 426);
            this.CAN_Init_btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CAN_Init_btn.Name = "CAN_Init_btn";
            this.CAN_Init_btn.Size = new System.Drawing.Size(405, 38);
            this.CAN_Init_btn.TabIndex = 82;
            this.CAN_Init_btn.Text = "Initialize CAN HW";
            this.CAN_Init_btn.Visible = false;
            this.CAN_Init_btn.Click += new System.EventHandler(this.CAN_Init_btn_Click);
            // 
            // CAN4chnLbl
            // 
            this.CAN4chnLbl.AutoSize = true;
            this.CAN4chnLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN4chnLbl.Location = new System.Drawing.Point(284, 237);
            this.CAN4chnLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN4chnLbl.Name = "CAN4chnLbl";
            this.CAN4chnLbl.Size = new System.Drawing.Size(198, 17);
            this.CAN4chnLbl.TabIndex = 89;
            this.CAN4chnLbl.Text = "Select CAN4 CAN Channel";
            // 
            // CAN1Lbl
            // 
            this.CAN1Lbl.AutoSize = true;
            this.CAN1Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN1Lbl.Location = new System.Drawing.Point(15, 35);
            this.CAN1Lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN1Lbl.Name = "CAN1Lbl";
            this.CAN1Lbl.Size = new System.Drawing.Size(228, 17);
            this.CAN1Lbl.TabIndex = 75;
            this.CAN1Lbl.Text = "Select CAN1 by Serial Number";
            // 
            // chnFourSN_comboBox
            // 
            this.chnFourSN_comboBox.FormattingEnabled = true;
            this.chnFourSN_comboBox.Location = new System.Drawing.Point(15, 257);
            this.chnFourSN_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFourSN_comboBox.Name = "chnFourSN_comboBox";
            this.chnFourSN_comboBox.Size = new System.Drawing.Size(205, 28);
            this.chnFourSN_comboBox.TabIndex = 87;
            this.chnFourSN_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnFourSN_comboBox_SelectedIndexChanged);
            // 
            // chnOne_comboBox
            // 
            this.chnOne_comboBox.Enabled = false;
            this.chnOne_comboBox.FormattingEnabled = true;
            this.chnOne_comboBox.Location = new System.Drawing.Point(285, 55);
            this.chnOne_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnOne_comboBox.Name = "chnOne_comboBox";
            this.chnOne_comboBox.Size = new System.Drawing.Size(180, 28);
            this.chnOne_comboBox.TabIndex = 73;
            this.chnOne_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnOne_comboBox_SelectedIndexChanged);
            // 
            // chnOneSN_comboBox
            // 
            this.chnOneSN_comboBox.FormattingEnabled = true;
            this.chnOneSN_comboBox.Location = new System.Drawing.Point(15, 55);
            this.chnOneSN_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnOneSN_comboBox.Name = "chnOneSN_comboBox";
            this.chnOneSN_comboBox.Size = new System.Drawing.Size(205, 28);
            this.chnOneSN_comboBox.TabIndex = 74;
            this.chnOneSN_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnOneSN_comboBox_SelectedIndexChanged);
            // 
            // chnFour_comboBox
            // 
            this.chnFour_comboBox.Enabled = false;
            this.chnFour_comboBox.FormattingEnabled = true;
            this.chnFour_comboBox.Location = new System.Drawing.Point(285, 257);
            this.chnFour_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFour_comboBox.Name = "chnFour_comboBox";
            this.chnFour_comboBox.Size = new System.Drawing.Size(180, 28);
            this.chnFour_comboBox.TabIndex = 86;
            this.chnFour_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnFour_comboBox_SelectedIndexChanged);
            // 
            // CAN1chnLbl
            // 
            this.CAN1chnLbl.AutoSize = true;
            this.CAN1chnLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN1chnLbl.Location = new System.Drawing.Point(284, 35);
            this.CAN1chnLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN1chnLbl.Name = "CAN1chnLbl";
            this.CAN1chnLbl.Size = new System.Drawing.Size(198, 17);
            this.CAN1chnLbl.TabIndex = 76;
            this.CAN1chnLbl.Text = "Select CAN1 CAN Channel";
            // 
            // CAN3Lbl
            // 
            this.CAN3Lbl.AutoSize = true;
            this.CAN3Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN3Lbl.Location = new System.Drawing.Point(16, 168);
            this.CAN3Lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN3Lbl.Name = "CAN3Lbl";
            this.CAN3Lbl.Size = new System.Drawing.Size(228, 17);
            this.CAN3Lbl.TabIndex = 84;
            this.CAN3Lbl.Text = "Select CAN3 by Serial Number";
            // 
            // CAN3chnLbl
            // 
            this.CAN3chnLbl.AutoSize = true;
            this.CAN3chnLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN3chnLbl.Location = new System.Drawing.Point(286, 168);
            this.CAN3chnLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN3chnLbl.Name = "CAN3chnLbl";
            this.CAN3chnLbl.Size = new System.Drawing.Size(198, 17);
            this.CAN3chnLbl.TabIndex = 85;
            this.CAN3chnLbl.Text = "Select CAN3 CAN Channel";
            // 
            // chnThreeSN_comboBox
            // 
            this.chnThreeSN_comboBox.FormattingEnabled = true;
            this.chnThreeSN_comboBox.Location = new System.Drawing.Point(15, 188);
            this.chnThreeSN_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnThreeSN_comboBox.Name = "chnThreeSN_comboBox";
            this.chnThreeSN_comboBox.Size = new System.Drawing.Size(205, 28);
            this.chnThreeSN_comboBox.TabIndex = 83;
            this.chnThreeSN_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnThreeSN_comboBox_SelectedIndexChanged);
            // 
            // chnThree_comboBox
            // 
            this.chnThree_comboBox.Enabled = false;
            this.chnThree_comboBox.FormattingEnabled = true;
            this.chnThree_comboBox.Location = new System.Drawing.Point(285, 188);
            this.chnThree_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnThree_comboBox.Name = "chnThree_comboBox";
            this.chnThree_comboBox.Size = new System.Drawing.Size(180, 28);
            this.chnThree_comboBox.TabIndex = 82;
            this.chnThree_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnThree_comboBox_SelectedIndexChanged);
            // 
            // CAN2chnLbl
            // 
            this.CAN2chnLbl.AutoSize = true;
            this.CAN2chnLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN2chnLbl.Location = new System.Drawing.Point(285, 102);
            this.CAN2chnLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN2chnLbl.Name = "CAN2chnLbl";
            this.CAN2chnLbl.Size = new System.Drawing.Size(198, 17);
            this.CAN2chnLbl.TabIndex = 81;
            this.CAN2chnLbl.Text = "Select CAN2 CAN Channel";
            // 
            // chnTwoSN_comboBox
            // 
            this.chnTwoSN_comboBox.FormattingEnabled = true;
            this.chnTwoSN_comboBox.Location = new System.Drawing.Point(15, 122);
            this.chnTwoSN_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnTwoSN_comboBox.Name = "chnTwoSN_comboBox";
            this.chnTwoSN_comboBox.Size = new System.Drawing.Size(205, 28);
            this.chnTwoSN_comboBox.TabIndex = 78;
            this.chnTwoSN_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnTwoSN_comboBox_SelectedIndexChanged);
            // 
            // CAN2Lbl
            // 
            this.CAN2Lbl.AutoSize = true;
            this.CAN2Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN2Lbl.Location = new System.Drawing.Point(16, 102);
            this.CAN2Lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN2Lbl.Name = "CAN2Lbl";
            this.CAN2Lbl.Size = new System.Drawing.Size(228, 17);
            this.CAN2Lbl.TabIndex = 80;
            this.CAN2Lbl.Text = "Select CAN2 by Serial Number";
            // 
            // chnTwo_comboBox
            // 
            this.chnTwo_comboBox.Enabled = false;
            this.chnTwo_comboBox.FormattingEnabled = true;
            this.chnTwo_comboBox.Location = new System.Drawing.Point(285, 122);
            this.chnTwo_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnTwo_comboBox.Name = "chnTwo_comboBox";
            this.chnTwo_comboBox.Size = new System.Drawing.Size(180, 28);
            this.chnTwo_comboBox.TabIndex = 79;
            this.chnTwo_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnTwo_comboBox_SelectedIndexChanged);
            // 
            // applyCANconfigBtn
            // 
            this.applyCANconfigBtn.Enabled = false;
            this.applyCANconfigBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applyCANconfigBtn.Location = new System.Drawing.Point(96, 372);
            this.applyCANconfigBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.applyCANconfigBtn.Name = "applyCANconfigBtn";
            this.applyCANconfigBtn.Size = new System.Drawing.Size(405, 37);
            this.applyCANconfigBtn.TabIndex = 77;
            this.applyCANconfigBtn.Text = "APPLY CAN CONFIGURATION";
            this.applyCANconfigBtn.UseVisualStyleBackColor = true;
            this.applyCANconfigBtn.Click += new System.EventHandler(this.applyCANconfigBtn_Click);
            // 
            // CANgroupBox2
            // 
            this.CANgroupBox2.Controls.Add(this.chnOneBR_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN5Lbl);
            this.CANgroupBox2.Controls.Add(this.CAN5BdLbl);
            this.CANgroupBox2.Controls.Add(this.CAN5chnLbl);
            this.CANgroupBox2.Controls.Add(this.chnFiveBR_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnFiveSN_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN4BdLbl);
            this.CANgroupBox2.Controls.Add(this.chnFive_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnFourBR_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN4Lbl);
            this.CANgroupBox2.Controls.Add(this.CAN1BdLbl);
            this.CANgroupBox2.Controls.Add(this.CAN3BdLbl);
            this.CANgroupBox2.Controls.Add(this.CAN4chnLbl);
            this.CANgroupBox2.Controls.Add(this.chnThreeBR_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN1Lbl);
            this.CANgroupBox2.Controls.Add(this.CAN2BdLbl);
            this.CANgroupBox2.Controls.Add(this.chnFourSN_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnTwoBR_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnOne_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnOneSN_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnFour_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN1chnLbl);
            this.CANgroupBox2.Controls.Add(this.CAN3Lbl);
            this.CANgroupBox2.Controls.Add(this.CAN3chnLbl);
            this.CANgroupBox2.Controls.Add(this.chnThreeSN_comboBox);
            this.CANgroupBox2.Controls.Add(this.chnThree_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN2chnLbl);
            this.CANgroupBox2.Controls.Add(this.chnTwoSN_comboBox);
            this.CANgroupBox2.Controls.Add(this.CAN2Lbl);
            this.CANgroupBox2.Controls.Add(this.chnTwo_comboBox);
            this.CANgroupBox2.Controls.Add(this.applyCANconfigBtn);
            this.CANgroupBox2.Location = new System.Drawing.Point(490, 55);
            this.CANgroupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CANgroupBox2.Name = "CANgroupBox2";
            this.CANgroupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CANgroupBox2.Size = new System.Drawing.Size(700, 423);
            this.CANgroupBox2.TabIndex = 78;
            this.CANgroupBox2.TabStop = false;
            this.CANgroupBox2.Text = "Select CAN HW";
            // 
            // chnOneBR_comboBox
            // 
            this.chnOneBR_comboBox.Enabled = false;
            this.chnOneBR_comboBox.FormattingEnabled = true;
            this.chnOneBR_comboBox.Items.AddRange(new object[] {
            "250 kBd",
            "500 kBd"});
            this.chnOneBR_comboBox.Location = new System.Drawing.Point(522, 55);
            this.chnOneBR_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnOneBR_comboBox.Name = "chnOneBR_comboBox";
            this.chnOneBR_comboBox.Size = new System.Drawing.Size(130, 28);
            this.chnOneBR_comboBox.TabIndex = 114;
            // 
            // CAN5Lbl
            // 
            this.CAN5Lbl.AutoSize = true;
            this.CAN5Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN5Lbl.Location = new System.Drawing.Point(14, 300);
            this.CAN5Lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN5Lbl.Name = "CAN5Lbl";
            this.CAN5Lbl.Size = new System.Drawing.Size(228, 17);
            this.CAN5Lbl.TabIndex = 92;
            this.CAN5Lbl.Text = "Select CAN5 by Serial Number";
            // 
            // CAN5BdLbl
            // 
            this.CAN5BdLbl.AutoSize = true;
            this.CAN5BdLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN5BdLbl.Location = new System.Drawing.Point(518, 300);
            this.CAN5BdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN5BdLbl.Name = "CAN5BdLbl";
            this.CAN5BdLbl.Size = new System.Drawing.Size(151, 17);
            this.CAN5BdLbl.TabIndex = 113;
            this.CAN5BdLbl.Text = "Select CAN5 Bitrate";
            // 
            // CAN5chnLbl
            // 
            this.CAN5chnLbl.AutoSize = true;
            this.CAN5chnLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN5chnLbl.Location = new System.Drawing.Point(284, 300);
            this.CAN5chnLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN5chnLbl.Name = "CAN5chnLbl";
            this.CAN5chnLbl.Size = new System.Drawing.Size(198, 17);
            this.CAN5chnLbl.TabIndex = 93;
            this.CAN5chnLbl.Text = "Select CAN5 CAN Channel";
            // 
            // chnFiveBR_comboBox
            // 
            this.chnFiveBR_comboBox.Enabled = false;
            this.chnFiveBR_comboBox.FormattingEnabled = true;
            this.chnFiveBR_comboBox.Items.AddRange(new object[] {
            "250 kBd",
            "500 kBd"});
            this.chnFiveBR_comboBox.Location = new System.Drawing.Point(522, 320);
            this.chnFiveBR_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFiveBR_comboBox.Name = "chnFiveBR_comboBox";
            this.chnFiveBR_comboBox.Size = new System.Drawing.Size(130, 28);
            this.chnFiveBR_comboBox.TabIndex = 112;
            // 
            // chnFiveSN_comboBox
            // 
            this.chnFiveSN_comboBox.FormattingEnabled = true;
            this.chnFiveSN_comboBox.Location = new System.Drawing.Point(15, 320);
            this.chnFiveSN_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFiveSN_comboBox.Name = "chnFiveSN_comboBox";
            this.chnFiveSN_comboBox.Size = new System.Drawing.Size(205, 28);
            this.chnFiveSN_comboBox.TabIndex = 91;
            this.chnFiveSN_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnFiveSN_comboBox_SelectedIndexChanged);
            // 
            // CAN4BdLbl
            // 
            this.CAN4BdLbl.AutoSize = true;
            this.CAN4BdLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN4BdLbl.Location = new System.Drawing.Point(516, 237);
            this.CAN4BdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN4BdLbl.Name = "CAN4BdLbl";
            this.CAN4BdLbl.Size = new System.Drawing.Size(151, 17);
            this.CAN4BdLbl.TabIndex = 111;
            this.CAN4BdLbl.Text = "Select CAN4 Bitrate";
            // 
            // chnFive_comboBox
            // 
            this.chnFive_comboBox.Enabled = false;
            this.chnFive_comboBox.FormattingEnabled = true;
            this.chnFive_comboBox.Location = new System.Drawing.Point(285, 320);
            this.chnFive_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFive_comboBox.Name = "chnFive_comboBox";
            this.chnFive_comboBox.Size = new System.Drawing.Size(180, 28);
            this.chnFive_comboBox.TabIndex = 90;
            this.chnFive_comboBox.SelectedIndexChanged += new System.EventHandler(this.chnFive_comboBox_SelectedIndexChanged);
            // 
            // chnFourBR_comboBox
            // 
            this.chnFourBR_comboBox.Enabled = false;
            this.chnFourBR_comboBox.FormattingEnabled = true;
            this.chnFourBR_comboBox.Items.AddRange(new object[] {
            "250 kBd",
            "500 kBd"});
            this.chnFourBR_comboBox.Location = new System.Drawing.Point(522, 257);
            this.chnFourBR_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnFourBR_comboBox.Name = "chnFourBR_comboBox";
            this.chnFourBR_comboBox.Size = new System.Drawing.Size(130, 28);
            this.chnFourBR_comboBox.TabIndex = 110;
            // 
            // CAN1BdLbl
            // 
            this.CAN1BdLbl.AutoSize = true;
            this.CAN1BdLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN1BdLbl.Location = new System.Drawing.Point(516, 35);
            this.CAN1BdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN1BdLbl.Name = "CAN1BdLbl";
            this.CAN1BdLbl.Size = new System.Drawing.Size(151, 17);
            this.CAN1BdLbl.TabIndex = 105;
            this.CAN1BdLbl.Text = "Select CAN1 Bitrate";
            // 
            // CAN3BdLbl
            // 
            this.CAN3BdLbl.AutoSize = true;
            this.CAN3BdLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN3BdLbl.Location = new System.Drawing.Point(519, 168);
            this.CAN3BdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN3BdLbl.Name = "CAN3BdLbl";
            this.CAN3BdLbl.Size = new System.Drawing.Size(151, 17);
            this.CAN3BdLbl.TabIndex = 109;
            this.CAN3BdLbl.Text = "Select CAN3 Bitrate";
            // 
            // chnThreeBR_comboBox
            // 
            this.chnThreeBR_comboBox.Enabled = false;
            this.chnThreeBR_comboBox.FormattingEnabled = true;
            this.chnThreeBR_comboBox.Items.AddRange(new object[] {
            "250 kBd",
            "500 kBd"});
            this.chnThreeBR_comboBox.Location = new System.Drawing.Point(522, 188);
            this.chnThreeBR_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnThreeBR_comboBox.Name = "chnThreeBR_comboBox";
            this.chnThreeBR_comboBox.Size = new System.Drawing.Size(130, 28);
            this.chnThreeBR_comboBox.TabIndex = 108;
            // 
            // CAN2BdLbl
            // 
            this.CAN2BdLbl.AutoSize = true;
            this.CAN2BdLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CAN2BdLbl.Location = new System.Drawing.Point(518, 102);
            this.CAN2BdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CAN2BdLbl.Name = "CAN2BdLbl";
            this.CAN2BdLbl.Size = new System.Drawing.Size(151, 17);
            this.CAN2BdLbl.TabIndex = 107;
            this.CAN2BdLbl.Text = "Select CAN2 Bitrate";
            // 
            // chnTwoBR_comboBox
            // 
            this.chnTwoBR_comboBox.Enabled = false;
            this.chnTwoBR_comboBox.FormattingEnabled = true;
            this.chnTwoBR_comboBox.Items.AddRange(new object[] {
            "250 kBd",
            "500 kBd"});
            this.chnTwoBR_comboBox.Location = new System.Drawing.Point(522, 122);
            this.chnTwoBR_comboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chnTwoBR_comboBox.Name = "chnTwoBR_comboBox";
            this.chnTwoBR_comboBox.Size = new System.Drawing.Size(130, 28);
            this.chnTwoBR_comboBox.TabIndex = 106;
            // 
            // CAN_HW_tB
            // 
            this.CAN_HW_tB.Location = new System.Drawing.Point(21, 83);
            this.CAN_HW_tB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CAN_HW_tB.Multiline = true;
            this.CAN_HW_tB.Name = "CAN_HW_tB";
            this.CAN_HW_tB.Size = new System.Drawing.Size(450, 330);
            this.CAN_HW_tB.TabIndex = 79;
            // 
            // exit_Btn
            // 
            this.exit_Btn.Location = new System.Drawing.Point(861, 483);
            this.exit_Btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exit_Btn.Name = "exit_Btn";
            this.exit_Btn.Size = new System.Drawing.Size(112, 35);
            this.exit_Btn.TabIndex = 81;
            this.exit_Btn.Text = "Abort";
            this.exit_Btn.UseVisualStyleBackColor = true;
            this.exit_Btn.Click += new System.EventHandler(this.Exit_Btn_Click);
            // 
            // cancelLbl
            // 
            this.cancelLbl.AutoSize = true;
            this.cancelLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelLbl.Location = new System.Drawing.Point(141, 495);
            this.cancelLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cancelLbl.Name = "cancelLbl";
            this.cancelLbl.Size = new System.Drawing.Size(350, 17);
            this.cancelLbl.TabIndex = 82;
            this.cancelLbl.Text = "To re-interrogate the CAN HW click-on Refresh";
            // 
            // directionLbl
            // 
            this.directionLbl.AutoSize = true;
            this.directionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directionLbl.Location = new System.Drawing.Point(486, 31);
            this.directionLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.directionLbl.Name = "directionLbl";
            this.directionLbl.Size = new System.Drawing.Size(436, 20);
            this.directionLbl.TabIndex = 83;
            this.directionLbl.Text = "Use the comboBoxes to configure the CAN subnet.";
            this.directionLbl.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 57);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(470, 422);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Available CAN HW";
            // 
            // refresh_Btn
            // 
            this.refresh_Btn.Location = new System.Drawing.Point(33, 483);
            this.refresh_Btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.refresh_Btn.Name = "refresh_Btn";
            this.refresh_Btn.Size = new System.Drawing.Size(99, 35);
            this.refresh_Btn.TabIndex = 86;
            this.refresh_Btn.Text = "Refresh";
            this.refresh_Btn.UseVisualStyleBackColor = true;
            this.refresh_Btn.Click += new System.EventHandler(this.refresh_Btn_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CANsetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 534);
            this.Controls.Add(this.refresh_Btn);
            this.Controls.Add(this.directionLbl);
            this.Controls.Add(this.cancelLbl);
            this.Controls.Add(this.exit_Btn);
            this.Controls.Add(this.CAN_HW_tB);
            this.Controls.Add(this.CAN_Init_btn);
            this.Controls.Add(this.CANgroupBox2);
            this.Controls.Add(this.OK_Btn);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "CANsetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kvaser Device Configuration Form";
            this.Load += new System.EventHandler(this.KvCANsetupForm_Load);
            this.CANgroupBox2.ResumeLayout(false);
            this.CANgroupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK_Btn;
        private System.Windows.Forms.Label CAN4Lbl;
        private System.Windows.Forms.Button CAN_Init_btn;
        private System.Windows.Forms.Label CAN4chnLbl;
        private System.Windows.Forms.Label CAN1Lbl;
        private System.Windows.Forms.ComboBox chnFourSN_comboBox;
        private System.Windows.Forms.ComboBox chnOne_comboBox;
        private System.Windows.Forms.ComboBox chnOneSN_comboBox;
        private System.Windows.Forms.ComboBox chnFour_comboBox;
        private System.Windows.Forms.Label CAN1chnLbl;
        private System.Windows.Forms.Label CAN3Lbl;
        private System.Windows.Forms.Label CAN3chnLbl;
        private System.Windows.Forms.ComboBox chnThreeSN_comboBox;
        private System.Windows.Forms.ComboBox chnThree_comboBox;
        private System.Windows.Forms.Label CAN2chnLbl;
        private System.Windows.Forms.ComboBox chnTwoSN_comboBox;
        private System.Windows.Forms.Label CAN2Lbl;
        private System.Windows.Forms.ComboBox chnTwo_comboBox;
        private System.Windows.Forms.Button applyCANconfigBtn;
        private System.Windows.Forms.GroupBox CANgroupBox2;
        private System.Windows.Forms.TextBox CAN_HW_tB;
        private System.Windows.Forms.Button exit_Btn;
        private System.Windows.Forms.Label cancelLbl;
        private System.Windows.Forms.Label CAN5Lbl;
        private System.Windows.Forms.Label CAN5chnLbl;
        private System.Windows.Forms.ComboBox chnFiveSN_comboBox;
        private System.Windows.Forms.ComboBox chnFive_comboBox;
        private System.Windows.Forms.Label directionLbl;
        private System.Windows.Forms.ComboBox chnOneBR_comboBox;
        private System.Windows.Forms.Label CAN5BdLbl;
        private System.Windows.Forms.ComboBox chnFiveBR_comboBox;
        private System.Windows.Forms.Label CAN4BdLbl;
        private System.Windows.Forms.ComboBox chnFourBR_comboBox;
        private System.Windows.Forms.Label CAN1BdLbl;
        private System.Windows.Forms.Label CAN3BdLbl;
        private System.Windows.Forms.ComboBox chnThreeBR_comboBox;
        private System.Windows.Forms.Label CAN2BdLbl;
        private System.Windows.Forms.ComboBox chnTwoBR_comboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button refresh_Btn;
        private System.Windows.Forms.Timer timer1;
    }
}