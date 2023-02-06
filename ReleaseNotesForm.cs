using System;
using System.Drawing;
using System.Windows.Forms;

namespace CANsetup
{
    public partial class ReleaseNotesForm : Form
    {
        public ReleaseNotesForm()
        {
            InitializeComponent();

            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Font = new Font("Lucida Console", 7);
        }

        private void OKbtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ReleseNotesForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "Revision History     Comment describing the changes";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.1  I made this capable for a 5-Port device.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.2  Various updates for re-entry from the FORM1 menu item.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.3  Added Logic so the AP doesn't crash if the CAN HW is different from the CANconfig.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "             You just exit to the main FORM and need to reinitialize using the menu DropDownItem.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.4  Added Logic to support 3 CAN devices in the setupForm and Form1 reads the Engine Speed on all the subnets for test purposes.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.5  Added combo-Boxes for the selection of the Bit-rate on BB1. Others subnets are set by default to 250K or 500K.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.6  Added an over-check in case the configuration file is stale, and it includes a device no longer connected. The APP will catch that and not crash.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.7  Added a view CONFIG drop-down menu item and a start button. I also initialized serialNumber[] with items to prevent a null exception.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.8  CANsetupForm changed to allow the BB1 Baud rate to be adjusted. Various other changes to optimize the code and trap CONFIG errors.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.0.9  CANsetupForm changed slightly to optimize the code and give the user more information when errors occur with the configuration.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.1.0  CANsetupForm changed to work with a single channel device. Kvaser Leaf";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.1.1  For new configurations the user is prompted to enter a CAN subnet name.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.0.1.2  Comments on how to use/include this SW in another Project.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 1.31.20.23  I added a CANmonitor. A rather large SW effort. That displays all the CAN messages on 2 Subnets and supports a Vector style DBC File.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "REV 2.06.20.23  I added a support of the transport protocol. (TPCM/TPDT). Another rather large SW effort. That displays DMx messages found on 2 Subnets.";
            richTextBox1.Text += Environment.NewLine;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
