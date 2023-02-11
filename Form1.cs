using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using CANsetup;
using Multimedia;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Kvaser.CanLib;




// REVISION NOTES
// SEE the ReleaseNotesForm.

namespace CANmonitor
{
    public partial class Form1 : Form
    {
        private static string SWvers = "2.06.20.23";

        CANsetupForm idNselcCAN_HW = new CANsetupForm();
        MultiPacketForm tpForm = new MultiPacketForm();
        ReleaseNotesForm versionHistory = new ReleaseNotesForm();
        
        private SPNattrib currSPN;
        private CanMsgProc mpMessage = new CanMsgProc();
        private CanMsgProc stdMessage = new CanMsgProc();
        private MultimediaTimer mmTimer;

        
        private static Canlib.canStatus can_status, canTimer_status;   //A KVASER CANLIBRARY OBJECT        
        private static long time, GUIinterval = 0, GUIintervalPV = 0, execTime = 0, msgTime = 0, msgDeltaTime = 0;
        public static long zeroTime = 0;
        private static long WaitTimeLimit, timeOutDelta = 75, CurrentTime;

        private static string PersonalDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CANsetup";
        private static string dbcDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CANmonitor";
        private static string PGNfilename = "", SPNfilename = "", SRCfilename = "";
        private static string DBCfilename = "";


        public static List<string> SelcetedIDs = new List<string>();
        
        public static Hashtable dbcMsgInfo = new Hashtable();
        public static Hashtable dbcNodes = new Hashtable();

        private static List<string> viewedSPNinfo = new List<string>();
        private static List<string> SPNinfo = new List<string>();

        private static string[] SPNlines = new string[50];

        private static bool pwtTPCM_E_active = false, CANrunningStat = false, assocDBCfile_S = false;

        public static byte[] VP26_Emsg = new byte[64];
        public static byte[] DM1Etpmsg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public const int TPCM_E = 0x18ECFF00;
        public const int TPCM_A = 0x18ECFF3D;
        public const int TPCM_R = 0x18ECFF10;
        public const int TPCM_ER = 0x18ECFF0F;
        public const int TPDT_E = 0x18EBFF00;
        public const int TPDT_A = 0x18EBFF3D;
        public const int TPDT_R = 0x18EBFF10;
        public const int TPDT_ER = 0x18EBFF0F;
        
        public const int DM1_E = 0x18FECA00;
        public const int DM1_A = 0x18FECA3D;
        public const int DM1_R = 0x18FECA10;
        public const int DM1_T = 0x18FECA03;
        public const int DM2_R = 0x18FECB10;
        public const int EC1_E = 0x18FEE300;
        public const int RC_ER = 0x18FEE10F;
        public const int SI_E = 0x18FEDA00;
        public const int CI_E = 0x18FEEB00;
        public const int VIN_E = 0x18FEEC00;
        public const int RC_R = 0x18FEE110;
        
        private static uint tictoc = 0;                                     //Schedules J1939 Messages
        private static int numOfSPNsFound = 0;
                                                                            // I made the array large enough to hold a msgID embedded in a TPCM from any node. Only One msgID is active at a time.
        private static int[] msgIDsInTPCM = new int[256];                   // This array holds the "virtual" MessageID of an Active multi-packet message. This provides an Index/link for the data.

        private static List<int> PWTmsgIDs = new List<int>();
        private static List<int> PWTmsgDLCs = new List<int>();
        private static List<byte[]> PWTmsgData = new List<byte[]>();
        private static List<long> PWTmsgTimeStamp = new List<long>();
        private static List<long> PWTmsgTimeStamp_pv = new List<long>();
        private static long PWTmsgDeltaTime = 0;

        private static List<int> BB2msgIDs = new List<int>();
        private static List<int> BB2msgDLCs = new List<int>(); 
        private static List<byte[]> BB2msgData = new List<byte[]>();
        private static List<long> BB2msgTimeStamp = new List<long>();
        private static List<long> BB2msgTimeStamp_pv = new List<long>();
        private static long BB2msgDeltaTime = 0;

        public Form1()
        {
            //
            // Required for Windows Form Designer support
            // CALL to link the FORM1 REFERENCE to OBJECTS by CREATING an instance of the FORM1
            InitializeComponent();

            // I found this solution on StackOverflow. This triggers the form to close when clicking on the Window's upper-right corner [X].
            // Delegate the event to a handler using this style of statement
            FormClosing += MainWin_FormClosing;
        }

        /// <summary>
        /// This main form is used to "call" the CANsetup Form, View TPDT Form, and display the J1939 Messages (PGN+SPNs) used by the ECUs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            string textline = "";

            // Class-level declaration HERE
            // Create an object of microTimer CLASS
            mmTimer = new MultimediaTimer(components);
            mmTimer.Stop();
            // Example of Setting the properties.
            mmTimer.Period = 10;
            mmTimer.Resolution = 1;
            mmTimer.Mode = TimerMode.Periodic;
            mmTimer.SynchronizingObject = this;

            // Hook up the Elapsed event for the timer.
            mmTimer.Tick += new EventHandler(mmTimerTick);

            richTextBox1.ForeColor = System.Drawing.Color.DarkBlue;
            richTextBox1.Font = new Font("Lucida Console", 8);

            richTextBox2.ForeColor = Color.DarkBlue;
            
            richTextBox3.ForeColor = System.Drawing.Color.DarkBlue;
            richTextBox3.Font = new Font("Lucida Console", 8);

            SWversLbl.Text = "SWvers: " + SWvers;

            // Initialize all the array elements to Zero
            for (int w = 0; w < 256; w++)
                msgIDsInTPCM[w] = 0;

            // This statement calls the CANsetup Form.
            if (idNselcCAN_HW.ShowDialog() == DialogResult.OK)
            {
                // Depending on the button used to return from the CANsetup do the following in the main Form
                CANstatus_cB.Enabled = true;
                CANstatus_cB.Checked = true;
                CANstatus_cB.ForeColor = Color.Green;
                CANstatus_cB.Font = new Font("Lucida Console", 7);
                CANstatus_cB.Text = CANsetupForm.txtCAN_HW;
            }
            else //if (DialogResult == DialogResult.Abort)
            {
                MainWin_FormClosing(null, null);
            }
            openFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CANmonitor";

            // READ the DBCconfig File
            if (File.Exists(dbcDirPath + "\\DBCconfig.txt"))
            {
                using (FileStream fs = new FileStream(dbcDirPath + "\\DBCconfig.txt", FileMode.Open, FileAccess.Read, share: FileShare.Read))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    DBCfilename = sr.ReadLine();

                    PGNfilename = DBCfilename.Remove((DBCfilename.Length - 4), 4);
                    PGNfilename += "_IDs.txt";

                    SPNfilename = DBCfilename.Remove((DBCfilename.Length - 4), 4);
                    SPNfilename += "_SPNs.txt";

                    SRCfilename = DBCfilename.Remove((DBCfilename.Length - 4), 4);
                    SRCfilename += "_SRCs.txt";
                }
            }
            else
            {
                // The File is missing                            
                MessageBox.Show("Error: " + dbcDirPath + "\\DBCconfig.txt" + " FILE NOT FOUND\n" + "\nYou must Associate a DBC file.", "Check Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Virtual Menu click
                associateDBCToolStripMenuItem_Click(null, null);
            }

            // After the Associate DBC is called these files exist.
            if (!File.Exists(PGNfilename))
            {
                // The File is missing                            
                MessageBox.Show("Error: missing file " + PGNfilename + " FILE NOT FOUND\n" + "\nYou must Associate a conforming DBCfile.", "Check Information");
                //associateDBCToolStripMenuItem_Click(null, null);
                File.Delete(dbcDirPath + "\\DBCconfig.txt");
                MainWin_FormClosing(null, null);
            }
            else if (!File.Exists(SPNfilename))
            {
                // The File is missing                            
                MessageBox.Show("Error: missing file " + SPNfilename + " FILE NOT FOUND\n" + "\nYou must Associate a conforming DBCfile.", "Check Information");
                //associateDBCToolStripMenuItem_Click(null, null);
                File.Delete(dbcDirPath + "\\DBCconfig.txt");
                MainWin_FormClosing(null, null);
            }
            else if (!File.Exists(SRCfilename))
            {
                // The File is missing                            
                MessageBox.Show("Error: missing file " + SRCfilename + " FILE NOT FOUND\n" + "\nYou must Associate a conforming DBCfile.", "Check Information");
                //associateDBCToolStripMenuItem_Click(null, null);
                File.Delete(dbcDirPath + "\\DBCconfig.txt");
                MainWin_FormClosing(null, null);
            }

            if (assocDBCfile_S == false)
            {
                // All three of the files exist.
                if (MessageBox.Show(DBCfilename + "\nis referenced in the DBCconfig file save in the CANmonitor Folder" + "\n\nDo you want to use it?", "Check Information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    associateDBCToolStripMenuItem.Visible = false;
                    // I broke the DBC file into two files
                    // Read in the file (PGN ID and descriptive short name) line by line
                    // Save the file into a hashtable
                    using (FileStream fs = new FileStream(PGNfilename, FileMode.Open, FileAccess.Read, share: FileShare.Read))
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        // Cycle through all the CAN MSG IDs and populate the MSG info List
                        do
                        {
                            textline = sr.ReadLine();
                            if (textline.Length > 0)
                                dbcMsgInfo.Add(textline.Substring(0, 8), textline.Substring(9, textline.Length - 9));

                        } while (sr.Peek() != -1);
                    }

                    // Read in the SRC file line by line
                    // Read the file into memory
                    using (FileStream fs = new FileStream(SRCfilename, FileMode.Open, FileAccess.Read, share: FileShare.Read))
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        // Cycle through all the ECUs that are on the Subnet add all the Info HASH TABLE
                        do
                        {
                            textline = sr.ReadLine();
                            if (textline.Length > 0)
                            {
                                // the endpt is the space character
                                int endpt = textline.IndexOf(" ");
                                // Get the number first
                                string Nmbr = textline.Substring(endpt + 1, textline.Length - (endpt + 1));
                                // strip off the number
                                string SrcName = textline.Substring(0, endpt);

                                dbcNodes.Add(SrcName, Nmbr);
                            }
                        } while (sr.Peek() != -1);
                    }

                    // Read in the SPN file line by line
                    // Read the file into memory
                    using (FileStream fs = new FileStream(SPNfilename, FileMode.Open, FileAccess.Read, share: FileShare.Read))
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        // Cycle through all the SPNs and add all the Info into a List
                        do
                        {
                            textline = sr.ReadLine();
                            if (textline.Length > 0)
                                SPNinfo.Add(textline);

                        } while (sr.Peek() != -1);
                    }
                }
                else
                {
                    // Virtual Menu click to create a new set of DBC files
                    associateDBCToolStripMenuItem_Click(null, null);
                }
            }

            richTextBox1.BringToFront();
            richTextBox1.Clear();
            richTextBox3.Clear();
            richTextBox1.Enabled = false;
            richTextBox3.Enabled = false;
            richTextBox2.Text = "Time ms       \t\u0394T ms\tJ1939 ID\t\tNAME\t\tDATA BYTES\t\t\tChannel";

            if (CANsetupForm.txtCAN_HW.Contains(CANsetupForm.subNetName[1]))
                Canlib.canFlushReceiveQueue(CANsetupForm.CAN1hnd);
            if (CANsetupForm.txtCAN_HW.Contains(CANsetupForm.subNetName[2]))
                Canlib.canFlushReceiveQueue(CANsetupForm.CAN2hnd);
            if (CANsetupForm.txtCAN_HW.Contains(CANsetupForm.subNetName[3]))
                Canlib.canFlushReceiveQueue(CANsetupForm.CAN3hnd);
            if (CANsetupForm.txtCAN_HW.Contains(CANsetupForm.subNetName[4]))
                Canlib.canFlushReceiveQueue(CANsetupForm.CAN4hnd);
            if (CANsetupForm.txtCAN_HW.Contains(CANsetupForm.subNetName[5]))
                Canlib.canFlushReceiveQueue(CANsetupForm.CAN5hnd);

            CanMsgProc.doOnce = false;

            viewedSPNinfo.Clear();
            
            CanMsgProc.msgSubnet.Clear();
            CanMsgProc.msgIDs.Clear();
            CanMsgProc.msgDLCs.Clear();
            CanMsgProc.msgData.Clear();
            CanMsgProc.msgTimeStamp.Clear();
            CanMsgProc.msgTimeStamp_pv.Clear();
            CanMsgProc.msgNodes.Clear();
        }
        private void associateDBCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int strtpt, endpt;
            string idHexString = null;
            
            if (openFD.ShowDialog() != DialogResult.Cancel)
            {

                StreamReader objReader;
                objReader = new StreamReader(openFD.FileName);

                objReader.Close();

                // Get the window to the front and then allow it to be displaced.
                TopMost = true;
                TopMost = false;

                // 'Steal' the focus.
                Activate();

                Show();

                Application.DoEvents();

                // Read the DBC file into memory
                using (FileStream fs = new FileStream(openFD.FileName, FileMode.Open, FileAccess.Read, share: FileShare.Read))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string textline = sr.ReadLine();

                    //Cycle through all the lines in the J1939 DBC file
                    do
                    {
                        if (textline.Contains("BO_ "))
                        {
                            richTextBox2.Text = "READING the DBC FILE";

                            // 'Steal' the focus.
                            Activate();

                            Show();

                            Application.DoEvents();

                            string BO_txt = textline.Substring(0, 4);

                            if (BO_txt.Contains("BO_ "))
                            {
                                // Locate the starting and ending point for the Message ID HEX Value (" " " ")
                                strtpt = textline.IndexOf(" ") + 1;

                                // eliminate the "BO_ " from the string so the next " " is the delimiter for the HEX value
                                string IDnDesc = textline.Substring(strtpt, textline.Length - strtpt);

                                // the endpt is the length of the CAN ID in decimal
                                endpt = IDnDesc.IndexOf(" ");

                                string textCANID = GetNumbers(IDnDesc.Substring(0, endpt));

                                ulong idValue = Convert.ToUInt64(textCANID) - 0x80000000;
                                idHexString = idValue.ToString("X8");  //BASE 16

                                // Locate the starting and ending point for the short name of the message (" " ":")
                                strtpt = textline.IndexOf(" ") + 1;
                                strtpt = textline.IndexOf(" ", strtpt, textline.Length - strtpt) + 1; // find the second space character plus 1

                                string shortName = textline.Substring(strtpt, textline.IndexOf(":") - strtpt);
                                // Limit the Description to 12 Characters
                                if (shortName.Length > 12)
                                    shortName = shortName.Substring(0, 9) + "...";

                                dbcMsgInfo.Add(idHexString, shortName);
                            }

                            textline = sr.ReadLine();   // READ the Blank Line after a BO_ with out any signal definitions OR if BO_ is hit Later in the textline OR it is a SG_ line
                        }
                        else if (textline.Contains(" SG_ "))
                        {
                            string SG_txt = textline.Substring(0, 5);
                            // Make sure the SG_ is at the beginning of the line
                            if (SG_txt.Contains(" SG_ "))
                            {
                                endpt = textline.IndexOf("\"");                                     // Find the openingQuotation mark
                                endpt += 1;                                                         // Advance index by one
                                endpt = textline.IndexOf("\"", endpt, textline.Length - endpt) + 1; // Find the endQuotation mark for the units

                                string SPNdetails = idHexString + " " + textline.Substring(5, endpt - 5);

                                SPNinfo.Add(SPNdetails);
                            }

                            textline = sr.ReadLine();   // Get all the associated Signals with the the PGN
                        }
                        else if (textline.Contains("BA_ \"NmStationAddress\" BU_"))
                        {
                            // Locate the starting and ending point
                            strtpt = textline.IndexOf("_") + 1;
                            strtpt = textline.IndexOf("_", strtpt, textline.Length - strtpt) + 2;   // Find the second underscore character plus (SP char)
                            
                            string SrcName = textline.Substring(strtpt, textline.Length - strtpt);  // At this point the SrcName string has the Name + number

                            // The strtpt is the space character after the name
                            strtpt = SrcName.IndexOf(" ");
                            // Get the number first
                            string Nmbr = SrcName.Substring(strtpt + 1, SrcName.Length - 1 - (strtpt + 1));
                            // The Node name is from the start of the string to the SP CHAR
                            SrcName = SrcName.Substring(0, strtpt);
                            
                            dbcNodes.Add(SrcName, Nmbr);

                            textline = sr.ReadLine();   // Get all the NmStationAddresses in the DBC
                        }
                        else
                            textline = sr.ReadLine();       // READ next Line of the DBC

                    } while (sr.Peek() != -1);
                }

                richTextBox1.Clear();
                richTextBox2.Clear();

                foreach (DictionaryEntry idNdesc in dbcMsgInfo)
                {
                    richTextBox1.Text += idNdesc.Key + " " + idNdesc.Value + "\r\n";
                }

                // Function call to Sort the lines in the richTextBox
                SortRTBlines();
                
                // I store the MSG IDs and short names in one file and the SPNInfo (startbit length scaling etc.) in another file
                PGNfilename = openFD.FileName.Remove((openFD.FileName.Length - 4), 4);
                PGNfilename += "_IDs.txt";

                File.WriteAllLines(PGNfilename, richTextBox1.Lines);

                richTextBox1.Clear();

                foreach (string item in SPNinfo)
                {
                    richTextBox1.Text += item + "\n";
                    //Display a make shift progress bar using richTextBox2, use a CHAR per every xxLine.
                    if (++tictoc == 37)
                    {
                        richTextBox2.Text += "\u039E";  // this CHAR fills the space well
                        tictoc = 0;
                    }

                    // 'Steal' the focus.
                    Activate();

                    Show();

                    Application.DoEvents();
                }

                // Function call to Sort the PGN lines in the richTextBox
                SortRTBlines();

                // I store the MSG IDs in one file and the SPNInfo in another file, and the ECU Nodes and name in an other
                SPNfilename = openFD.FileName.Remove((openFD.FileName.Length - 4), 4);
                SPNfilename += "_SPNs.txt";

                File.WriteAllLines(SPNfilename, SPNinfo);

                richTextBox1.Clear();

                foreach (DictionaryEntry nodeNdesc in dbcNodes)
                {
                    richTextBox1.Text += nodeNdesc.Key + " " + nodeNdesc.Value + "\r\n";
                }
                // Function call to Sort the SPN lines in the richTextBox
                SortRTBlines();

                // I store the MSG IDs in one file and the SPNInfo in another file, and the ECU Nodes and name in an other
                SRCfilename = openFD.FileName.Remove((openFD.FileName.Length - 4), 4);
                SRCfilename += "_SRCs.txt";

                if (richTextBox1.TextLength != 0)
                    File.WriteAllLines(SRCfilename, richTextBox1.Lines);
                else
                {
                    MessageBox.Show("Error: DBC FILE doesn't contain ECU Station Numbers.\n" + "\nDBC file not compatible.", "Check Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                richTextBox1.Clear();

                System.IO.StreamWriter objWriter;
                objWriter = new System.IO.StreamWriter(dbcDirPath + "\\DBCconfig.txt");
                DBCfilename = dbcDirPath + "\\DBCconfig.txt";
                
                // Update to the Newly selected DBC File in the DBCconfig.txt file. 
                objWriter.Write(openFD.FileName);
                objWriter.Close();

                assocDBCfile_S = true;

                richTextBox2.Text = "Time ms       \t\u0394T ms\t\tJ1939 ID\t\tNAME\t\tDATA BYTES\t\tChannel";
                
                TDlbl.Text = DateTime.Now.ToString("u").TrimEnd('Z');
                
                tictoc = 0;
            }
        }
        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Contains("Start"))
            {                                
                button1.Text = "OnLine";
                button1.BackColor = Color.LightGreen;

                // This is an over check on the consistencay of the data stream on a given Subnet.
                canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out CurrentTime);    // Update the wait time limit every CANmsg.
                if (canTimer_status == Canlib.canStatus.canOK)
                {
                        WaitTimeLimit = CurrentTime;
                }

                if (!GUItimer.Enabled == true)
                {
                    GUItimer.Enabled = true;
                    GUItimer.Start();
                }
                BB1btn_Click(null, null);   // Default the Trace to start on BB1
                // Start the Timer that schedules the CAN msgs reads
                mmTimer.Start();
            }
            else
            {
                mmTimer.Stop();
                button1.Text = "Start";
                button1.BackColor = Color.White;
                if (GUItimer.Enabled == true)
                {
                    GUItimer.Stop();
                    GUItimer.Enabled = false;
                }
                richTextBox1.Enabled = true;
                richTextBox3.Enabled = true;
            }
        }
        private void BB1btn_Click(object sender, EventArgs e)
        {
            BB1btn.BackColor = Color.LightGreen;
            viewedSPNinfo.Clear();
            if (!richTextBox1.Visible) 
            {
                richTextBox1.Visible = true;
                ESNbtn.BackColor = Color.White;
            }

            if (panel1.Contains(richTextBox1))
            {
                richTextBox3.Visible = false;
                richTextBox1.BringToFront();
            }
        }
        private void ESNbtn_Click(object sender, EventArgs e)
        {
            ESNbtn.BackColor = Color.LightGreen;

            viewedSPNinfo.Clear();
            if (!richTextBox3.Visible)
            {
                richTextBox3.Visible = true;
                BB1btn.BackColor = Color.White;
            }

            if (panel1.Contains(richTextBox2))
            {
                richTextBox1.Visible = false;
                richTextBox3.BringToFront();
            }
        }
        private void mmTimerTick(object sender, EventArgs timerEventArgs)
        {
            byte[] CANmsg = new byte[8];
            int dlc;
            int flag;
            int CANmsgID;
            bool msgStatus = false;

            GUIintervalPV = GUIinterval;

            //GUIinterval = Canlib.canReadTimer(CANsetupForm.CAN1hnd);
            canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out CurrentTime);    // Update the wait time limit every CANmsg.
            GUIinterval = CurrentTime;

            if (CurrentTime > WaitTimeLimit)
            {
                CanMsgProc.msgSubnet.Clear();
                CanMsgProc.msgIDs.Clear();
                CanMsgProc.msgDLCs.Clear();
                CanMsgProc.msgData.Clear();
                CanMsgProc.msgTimeStamp.Clear();
                CanMsgProc.msgTimeStamp_pv.Clear();
                CanMsgProc.msgNodes.Clear();
                CanMsgProc.doOnce = false;
                CANrunningStat = false;
                for (int elm = 0; elm < 256; elm++)
                    msgIDsInTPCM[elm] = 0;
            }
            else
                CANrunningStat = true;

            // READ THE BB1 SUBNET MESSAGES BELOW
            do
            {
                // By convention. I always connect CAN1 to the BB1 subnet
                can_status = Canlib.canRead(CANsetupForm.CAN1hnd, out int id, CANmsg, out dlc, out flag, out time);
                
                if (can_status == Canlib.canStatus.canOK)
                {
                    if (CanMsgProc.doOnce == false)
                    {
                        zeroTime = time;    // Once the TRACE is started from the GUI Start button use the initial msg time as the base "ZERO".
                        CanMsgProc.doOnce = true;
                    }

                    // This is an over check on the consistencay of the data stream on a given Subnet.
                    canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out CurrentTime);    // Update the wait time limit every CANmsg.

                    if (canTimer_status == Canlib.canStatus.canOK)
                        WaitTimeLimit = CurrentTime + timeOutDelta;

                    msgTime = time - zeroTime;

                    CANmsgID = id;

                    // Found I needed to double buffer the Signals. "Unrolled" this activity; at one point this technique is fastest method.
                    byte[] msg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    msg[0] = CANmsg[0];
                    msg[1] = CANmsg[1];
                    msg[2] = CANmsg[2];
                    msg[3] = CANmsg[3];
                    msg[4] = CANmsg[4];
                    msg[5] = CANmsg[5];
                    msg[6] = CANmsg[6];
                    msg[7] = CANmsg[7];

                    // Read the messages on BB1.
                    msgStatus = stdMessage.StandardMsgInfo(CANsetupForm.CAN1hnd, CANmsgID, msg, dlc, msgTime);

                    // Handle the MULTI-PACKET msgs (TPCM/TPDT) messages HERE
                    // The PGN contained in the BAM is added to the Message List with a dlc length equal to the totalNumOfByte
                    #region "Dedicated TPCM/TPDT Processing of Multi-packet Messages"

                    // TPCM_E is active 5 times, but the TPDT multi-packets don't overlap from a given ECU so a single ACTIVE Flag is necessary.
                    // But what happens if the EMS is sending a DM1 and the ACM is sending a DM1 at the same time?
                    // Set Both active Flags to true
                    // and save each msgIndx returned from the TPCMinfo Function so you can link the TPDT messages frome any source in the MSG List.

                    // TRAP on TPCMs for any NODE and also Point-to-PointTPCMs
                    if (((0x18FFFF00 & CANmsgID) >> 8) == 0x18ECFF)
                    {
                        int Node = 0xFF & CANmsgID;

                        // Use the Source ADDR of the ECU to index into an array and save the extended msg ID found in the TPCM.
                        // This "virtual message ID" will be used to properly associate the TPDT data packets.

                        msgIDsInTPCM[Node] = mpMessage.TPCMinfo(CANsetupForm.CAN1hnd, CANmsgID, msg);
                    }
                    else if (((0x1CFFFF00 & CANmsgID) >> 16) == 0x1CEC)
                    {
                        int Node = 0xFF & CANmsgID;

                        // Use the Source ADDR of the ECU to index into an array and save the extended msg ID found in the TPCM.
                        // This "virtual message ID" will be used to properly associate the TPDT data packets.

                        msgIDsInTPCM[Node] = mpMessage.TPCMinfo(CANsetupForm.CAN1hnd, CANmsgID, msg);
                    }

                    // TPDTs don't overlap from a GIVEN ECU. However Different ECUs can Communicate TPDTS asychronously.
                    // So the MSG QUE may hve sequencial TPDTs that are not associated.
                    // To sort this out I use an array that holds the virtual" msgID. This non-Zero value also denotes when a multi-packet message is active.
                    // The array is accessed using the ECU node Number. If the array item is non-Zero there is a multi-packet message in progress.

                    // Filter for any TPDTs
                    else if (((0x18FFFF00 & CANmsgID) >> 8) == 0x18EBFF)
                    {
                        int Node = 0xFF & CANmsgID;
                        
                        // Check before calling the TPDT Handle that the TPCM has been recv'd and is in the LIST
                        if (msgIDsInTPCM[Node] > 0)
                        {
                            msgIDsInTPCM[Node] = mpMessage.TPDTinfo(msgIDsInTPCM[Node], msg);
                        }
                    }
                    else if (((0x1CFFFF00 & CANmsgID) >> 16) == 0x1CEB)
                    {
                        int Node = 0xFF & CANmsgID;

                        if (msgIDsInTPCM[Node] > 0)
                        {
                            msgIDsInTPCM[Node] = mpMessage.TPDTinfo(msgIDsInTPCM[Node], msg);
                        }
                    }
                    #endregion
                }
            } while (can_status == Canlib.canStatus.canOK && msgStatus == true);
            
            // READ THE ESN SUBNET MESSAGES BELOW
            do
            {
                // By convention. I always connect CAN2 to the ESN subnet
                can_status = Canlib.canRead(CANsetupForm.CAN2hnd, out int id, CANmsg, out dlc, out flag, out time);
                if (can_status == Canlib.canStatus.canOK)
                {
                    msgTime = time - zeroTime;

                    // To make the message ID different for the same message (ie EEC1) on both the CAN1 and CAN2 subnets I set bit 32
                    // This is only in the msgLIST and not displayed. Later when the CAN screen is built.
                    CANmsgID = (int)(id | 0x80000000);

                    // Found I needed to double buffer the Signals. "Unrolled" this activity; at one point this technique is fastest method.
                    byte[] msg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    msg[0] = CANmsg[0];
                    msg[1] = CANmsg[1];
                    msg[2] = CANmsg[2];
                    msg[3] = CANmsg[3];
                    msg[4] = CANmsg[4];
                    msg[5] = CANmsg[5];
                    msg[6] = CANmsg[6];
                    msg[7] = CANmsg[7];

                    // HANDLE standard 8-Byte messages here
                    msgStatus = stdMessage.StandardMsgInfo(CANsetupForm.CAN2hnd, CANmsgID, msg, dlc, time);                    

                    // Handle the MULTI-PACKET msgs (TPCM/TPDT) messages HERE
                    // The PGN contained in the BAM is added to the Message List with a dlc length equal to the totalNumOfByte
                    #region "Dedicated TPCM/TPDT Processing of Multi-packet Messages"

                    // TPCM_E is active 5 times, but the TPDT multi-packets don't overlap from a given ECU so a single ACTIVE Flag is necessary.
                    // But what happens if the EMS is sending a DM1 and the ACM is sending a DM1 at the same time?
                    // Set Both active Flags to true
                    // and save each msgIndx returned from the TPCMinfo Function so you can link the TPDT messages frome any source in the MSG List.

                    // Filter for any TPCMs 0xECFF is a BAM to any ECU; 0xECxx where xx is the target NODE needs to also be included for Point to Point
                    if (((0xFFFF00 & CANmsgID) >> 8) == 0xECFF)
                    {
                        int Node = 0xFF & CANmsgID;

                        // Use the Source ADDR of the ECU to index into an array and save the extended msg ID found in the TPCM.
                        // This "virtual message ID" will be used to properly associate the TPDT data packets.

                        msgIDsInTPCM[Node] = mpMessage.TPCMinfo(CANsetupForm.CAN2hnd, CANmsgID, msg);
                    }
                    // TPDTs don't overlap from a GIVEN ECU. However Different ECUs can Communicate TPDTS asychronously.
                    // So the MSG QUE may hve sequencial TPDTs that are not associated.
                    // To sort this out I use an array that holds the virtual" msgID. This non-Zero value also denotes when a multi-packet message is active.
                    // The array is accessed using the ECU node Number. If the array item is non-Zero there is a multi-packet message in progress.

                    // Filter for any TPDTs
                    else if (((0xFFFF00 & CANmsgID) >> 8) == 0xEBFF)
                    {
                        int Node = 0xFF & CANmsgID;

                        msgIDsInTPCM[Node] = mpMessage.TPDTinfo(msgIDsInTPCM[Node], msg);
                    }
                    #endregion
                }
            } while (can_status == Canlib.canStatus.canOK && msgStatus == true);

            if ((tictoc % 100) == 0) TDlbl.Text = DateTime.Now.ToString("u").TrimEnd('Z');

            // Counter used to schedule messages in multiples of 10 milliSeconds. (10ms is the cyclic rate of mmTimer)
            if (++tictoc == 10000)                
                tictoc = 0;

            canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out execTime);    // Update the wait time limit every CANmsg.

            //execTime = Canlib.canReadTimer(CANsetupForm.CAN1hnd);
        }
        private void GUItimer_Tick(object sender, EventArgs e)
        {            
            if (CANrunningStat == true)
            {
                // BB1 MONITOR. Process the RCVD MSGs HERE. Cycle thru the List built in the mmTimer and
                // Display ALL the BB1 J1939 messages
                richTextBox1.Text = Buildmonitor(CANsetupForm.CAN1hnd);
                // END BB1 MONITOR

                // ESN MONITOR Process the RCVD MSGs HERE. Cycle thru the List built in the mmTimer and
                // Display ALL the ESN J1939 messages            
                richTextBox3.Text = Buildmonitor(CANsetupForm.CAN2hnd);
                // END ESN MONITOR
            }

            updateIntervalLbl.Text = "GUI Timer Interval: " + (GUIinterval - GUIintervalPV).ToString("D4") + " ms";
            updateIntervalLbl.Text += "  Execution Time: " + (execTime - GUIinterval).ToString("D4") + " ms";
        }
        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (richTextBox1.SelectionLength == 0 || e.Button != MouseButtons.Left) return;

            // Get the current line of a richTextBox            
            int lineNum = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);

            // Save the entire line for a test of its contenets
            string rtbTextLine = richTextBox1.Lines.ElementAt(lineNum).TrimEnd(' ');

            // When viewing multiple PGNs during a snapshot; Ignore the improper mouse click of an SPN from crashing the Program
            if (rtbTextLine.Contains("=>")) return;

            // save the J1939 ID to a string. Each SPN associated with the PGN has this ID at the beginning
            // EXAMPLE: SPN ---->  0CFF0211 AcceleratorPedalPos : 24|8@1+ (0.4,0) [0|100] "perc"
            string selctdID = richTextBox1.SelectedText;

            // Ignore the improper mouse clicks on text that isn't a J1939 ID during a snapshot of a CANmonitor
            if (selctdID.Length != 8 || selctdID.Contains("P") || selctdID.Contains("T")) return;
            else
            {
                // Ignore multiple mouse clicks on the same J1939 ID during a snapshot of a CANmonitor
                if (!viewedSPNinfo.Contains(selctdID))
                {
                    if (selctdID.Contains("18FECA00") || selctdID.Contains("18FECA3D") || selctdID.Contains("18FECA10"))
                        AutoClosingMessageBox.Show("The View DTC Form provides the SPN/FMI descriptions for DM1 Faults.", "CANmonitor Info", 3500);

                    viewedSPNinfo.Add(selctdID);

                    //Search the saved SPNs for the matching J1939 ID
                    SPNlines = SearchSPNlines(selctdID);

                    richTextBox1.Lines = UpdateRTBx(richTextBox1, SPNlines, lineNum);
                }
                else
                    return;
            }
        }
        private void richTextBox3_MouseUp(object sender, MouseEventArgs e)
        {            
            if (richTextBox3.SelectionLength == 0 || e.Button != MouseButtons.Left) return;

            // Get the current line of a richTextBox
            int lineNum = richTextBox3.GetLineFromCharIndex(richTextBox3.SelectionStart);

            // Save the entire line temporarily for a test of its contenets
            string rtbTextLine = richTextBox3.Lines.ElementAt(lineNum).TrimEnd(' ');

            // When viewing multiple PGNs during a snapshot; Ignore the improper mouse click of an SPN from crashing the Program
            if (rtbTextLine.Contains("=>")) return;

            // save the J1939 ID to a string. Each SPN associated with the PGN has this ID at the beginning
            // EXAMPLE: SPN ---->  0CFF0211 AcceleratorPedalPos : 24|8@1+ (0.4,0) [0|100] "perc"
            string selctdID = richTextBox3.SelectedText;

            // Ignore the improper mouse click on the SAME J1939 ID during a snapshot of a CANmonitor
            if (selctdID.Length != 8 || selctdID.Contains("P") || selctdID.Contains("T")) return;
            else
            {
                // Ignore multiple SPN displays of the SAME J1939 ID during a snapshot of a CANmonitor
                if (!viewedSPNinfo.Contains(selctdID))
                {
                    viewedSPNinfo.Add(selctdID);

                    //Search the saved SPNs for the matching J1939 ID
                    SPNlines = SearchSPNlines(selctdID);
                    
                    richTextBox3.Lines = UpdateRTBx(richTextBox3, SPNlines, lineNum);                    
                }
                else
                    return;
            }
        }
        
        private string [] SearchSPNlines(string canIDtxt)
        {
            UInt32 parsedData = 0;
            Int32 msgIDindx = 0;
            decimal scaledValue;
            
            string[] matchingSPNs = new string[50]; // Lines of Text like this -> 0CFF0211 AcceleratorPedalPos : 24|8@1+ (0.4,0) [0|100] "%"

            numOfSPNsFound = 0;

            // Search the saved SPNs for the matching J1939 ID
            foreach (string SPNline in SPNinfo)
            {
                // When a match is found parse the DBC formatted line of the SPN to get the attributes necessary for displaying the value associated with it.
                if (SPNline.Contains(canIDtxt))
                {
                    // I use the btn to determine which subnet. I should probably change this to use the subnet attribute.
                    if (BB1btn.BackColor == Color.LightGreen)
                    {
                        int id = Convert.ToInt32(canIDtxt, 16);
                        msgIDindx = CanMsgProc.msgIDs.FindIndex(s => s == id);

                        if (msgIDindx > 0)
                        {
                            currSPN = new SPNattrib(SPNline);

                            if ((currSPN.STARTBIT / 8) < CanMsgProc.msgDLCs[msgIDindx])
                            {
                                // Parse the SPN data using the startBit and dataLength attributes above
                                parsedData = FindRawValue(currSPN.STARTBIT, currSPN.BITLENGTH, CanMsgProc.msgData[msgIDindx]);
                            }
                            else    // I found the EC1 on BB1 message broadcast didn't contain all the SPNs detailed in the J1939-71 SPEC. Set to max values.
                            {
                                switch (currSPN.BITLENGTH)
                                {
                                    case 8:
                                        parsedData = 0xF;
                                        break;
                                    case 16:
                                        parsedData = 0xFF;
                                        break;
                                    case 32:
                                        parsedData = 0xFFFF;
                                        break;
                                }
                            }
                        }
                    }
                    else if (ESNbtn.BackColor == Color.LightGreen)
                    {
                        // Remember the ID in the msgLIST has the 32nd bit set to differentiate it from a CAN1 msgID. This msgID is not displayed
                        int id = (int)((Convert.ToInt32(canIDtxt, 16)) | 0x80000000);      

                        msgIDindx = CanMsgProc.msgIDs.FindIndex(s => s == id);

                        if (msgIDindx > 0)
                        {
                            currSPN = new SPNattrib(SPNline);

                            // Parse the SPN data using the startBit and dataLength attributes above
                            parsedData = FindRawValue(currSPN.STARTBIT, currSPN.BITLENGTH, CanMsgProc.msgData[msgIDindx]);
                        }
                    }
                    if (msgIDindx > 0)
                    {
                        // Calculate Scaled Value. If the data is LT a byte no scaling is applied so I just eliminate multipling by 1.
                        if (currSPN.BITLENGTH <= 64 && currSPN.BITLENGTH >= 8)
                        {
                            scaledValue = parsedData * currSPN.SCALER + currSPN.OFFSET;
                            // Write out the line
                            matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                scaledValue.ToString() + " " +
                                SPNline.Substring(SPNline.IndexOf("\""), SPNline.Length - SPNline.IndexOf("\"")).Replace("\"", "");

                        }
                        else if (currSPN.BITLENGTH < 8)
                        {
                            scaledValue = parsedData;
                            // Write out the line
                            matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                scaledValue.ToString() + " " +
                                SPNline.Substring(SPNline.IndexOf("\""), SPNline.Length - SPNline.IndexOf("\"")).Replace("\"", "");
                        }
                        else
                        {
                            if (canIDtxt.Contains("CD3005"))        //DM19_NOxIn or NOxOut
                            {
                                if (currSPN.STARTBIT < 152 && currSPN.STARTBIT >= 32)
                                {
                                    string asciiDATA = new ASCIIEncoding().GetString(CanMsgProc.msgData[msgIDindx]).TrimEnd('\0');
                                    asciiDATA = asciiDATA.Remove(0, 4);
                                    asciiDATA = asciiDATA.TrimEnd('\0', '?');
                                    matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                        asciiDATA + " " +
                                        SPNline.Substring(SPNline.IndexOf("\""), SPNline.Length - SPNline.IndexOf("\"")).Replace("\"", "");
                                }
                            }
                            else if (canIDtxt.Contains("CD30000"))
                            {
                                string asciiDATA = new ASCIIEncoding().GetString(CanMsgProc.msgData[msgIDindx]).TrimEnd('\0');
                                if (asciiDATA != "")
                                {
                                    string asciiDATA1 = asciiDATA.Substring(1, 12).Trim('\0', '?');
                                    string asciiDATA2 = asciiDATA.Substring(13, 12).Trim('\0', '?');
                                    string asciiDATA3 = asciiDATA.Substring(25, 12).Trim('\0', '?');
                                    matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                        asciiDATA1 + " " +
                                        asciiDATA2 + " " +
                                        asciiDATA3;
                                }
                            }
                            else if (canIDtxt.Contains("CD30011"))
                            {
                                string asciiDATA = new ASCIIEncoding().GetString(CanMsgProc.msgData[msgIDindx]).TrimEnd('\0');
                                string asciiDATA1 = asciiDATA.Substring(4, 16).TrimEnd('\0', '?');
                                string asciiDATA2 = asciiDATA.Substring(24, 16).TrimEnd('\0', '?');
                                matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                    asciiDATA1 + " " +
                                    asciiDATA2;
                            }
                            else
                            {
                                string asciiDATA = new ASCIIEncoding().GetString(CanMsgProc.msgData[msgIDindx]).TrimEnd('\0');
                                // Write out the line
                                matchingSPNs[numOfSPNsFound++] = "  => " + SPNline.Substring(9, (SPNline.IndexOf(":") + 2) - 9) +
                                    asciiDATA + " " +
                                    SPNline.Substring(SPNline.IndexOf("\""), SPNline.Length - SPNline.IndexOf("\"")).Replace("\"", "");
                            }
                        }
                    }
                }
            }
            return matchingSPNs;
        }
        private string[] UpdateRTBx(RichTextBox rtbX, string[] buffer, int lineIndx)
        {
            int cntr = 1;

            // Insert all the SPNs associated with the selected J1939 ID
            // Step 1: create a stringArray buffer to hold updated RTB text.  (PGNs + SPNs)
            buffer = new string[rtbX.Lines.Length + numOfSPNsFound];

            // Step 2: save the RTBtext lines up to and including the selected line to the buffer
            Array.Copy(rtbX.Lines, 0, buffer, 0, (lineIndx + 1));

            // Step 3: get the lines to be inserted found above in the search of SPNs. ARRAY indexing is ZERO based so ONE is added sometimes and taken away other times
            for (; cntr <= numOfSPNsFound; cntr++)
                buffer[lineIndx + cntr] = SPNlines[cntr - 1];

            // Step 4: write the rest of the RTB text, which is from the SPN insertion point to the end of the original RTB
            Array.Copy(rtbX.Lines, lineIndx + 1, buffer, lineIndx + cntr, rtbX.Lines.Length - (lineIndx + 1));

            return buffer;
        }
        private UInt32 FindRawValue (int SPNstartBit, int SPNdataLength, byte[] byteData)
        {
            int startByte = SPNstartBit / 8; 
            int fractionalStartBit = SPNstartBit % 8;
            UInt32 rawValue = 0;

            if (SPNdataLength == 8)
            {
                rawValue = byteData[startByte];
                rawValue &= 0xFF;
            }
            else if (SPNdataLength == 16)
            {
                rawValue = (UInt32)(byteData[startByte + 1] << 8) + byteData[startByte];
                rawValue &= 0xFFFF;
            }
            else if (SPNdataLength == 24)
            {
                rawValue = (UInt32)((byteData[startByte + 2] << 16) + (byteData[startByte + 1] << 8) + byteData[startByte]);
                rawValue &= 0xFFFFFF;
            }
            else if (SPNdataLength == 32)
            {
                rawValue = (UInt32)((byteData[startByte + 3] << 24) + (byteData[startByte + 2] << 16) + (byteData[startByte + 1] << 8) + byteData[startByte]);
                rawValue &= 0xFFFFFFFF;
            }
            else if (SPNdataLength < 8)
            {
                byte mask = 0xFF;
                rawValue = byteData[startByte];

                mask = (byte)(0xFF >> (8 - SPNdataLength));
                mask = (byte)(mask << fractionalStartBit);

                rawValue &= mask;
                // Shift the ON/OFF values to be Less Than 0xF
                rawValue = (byte)(rawValue >> fractionalStartBit);
            }
            return rawValue;
        }
        private void SortRTBlines()
        {
            richTextBox1.HideSelection = false; //for showing selection

            /*Saving current selection*/
            string selectedText = richTextBox1.SelectedText;

            /*Saving current line*/
            int firstCharInLineIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
            int currLineIndex = richTextBox1.Text.Substring(0, firstCharInLineIndex).Count(c => c == '\n');
            string currLine = richTextBox1.Lines[currLineIndex];
            int offset = richTextBox1.SelectionStart - firstCharInLineIndex;


            /*Sorting richTextBox requires putting the STRING into a STRING ARRAY LINE BY LINE*/
            string[] lines = richTextBox1.Lines;
            Array.Sort(lines, delegate (string str1, string str2) { return str1.CompareTo(str2); });
            richTextBox1.Lines = lines;

            // THIS CODE IS NOT NECESSARY AS THERE IS NOT ANY SIGNIFICANCE TO THE ORIGINAL CURSOR LOCATION IN THIS CASE
            /*
            if (!String.IsNullOrEmpty((selectedText)))
            {
                //restore selection if desired
                int newIndex = richTextBox1.Text.IndexOf(selectedText);
                richTextBox1.Select(newIndex, selectedText.Length);
            }
            else
            {   // Restore the cursor if there is not any test selected
                // in other words position the window at the line it was prior to SORTING
                int lineIdx = Array.IndexOf(richTextBox1.Lines, currLine);
                int textIndex = richTextBox1.Text.IndexOf(currLine);
                int fullIndex = textIndex + offset;
                richTextBox1.SelectionStart = fullIndex;
                richTextBox1.SelectionLength = 0;
            }
            */
        }
        private string Buildmonitor(int Chn)
        {
            string CANmonitorText = null;
            string msgName = null, shortTime;
            int len;

            for (int el = 0; el < CanMsgProc.msgIDs.Count; el++)
            {
                //Calculate the first column which is the message's cylic update rate
                if (CanMsgProc.msgTimeStamp[el] > CanMsgProc.msgTimeStamp_pv[el])
                    msgDeltaTime = CanMsgProc.msgTimeStamp[el] - CanMsgProc.msgTimeStamp_pv[el];
                else
                    msgDeltaTime = 0;

                if (CanMsgProc.msgSubnet[el] == Chn)
                {
                    string idString = (0x1FFFFFFF & CanMsgProc.msgIDs[el]).ToString("X8");

                    shortTime = CanMsgProc.msgTimeStamp[el].ToString();

                    if (shortTime.Length > 6)
                    {
                        // Limit the Description to 12 Characters
                        shortTime = CanMsgProc.msgTimeStamp[el].ToString().Substring(0, 5) + "...";

                        CANmonitorText += shortTime + "\t" + msgDeltaTime.ToString().PadLeft(5, '0') + "\t" + idString + "\t";
                    }
                    else
                        CANmonitorText += shortTime + "\t\t" + msgDeltaTime.ToString().PadLeft(5, '0') + "\t" + idString + "\t";

                    // Search thru the entries of the DBC File that are stored in the Hash Table. 
                    // The dbcMsgInfo hashTable contains the Msg's HEX ID/ABBREVIATED name

                    foreach (DictionaryEntry idNdesc in dbcMsgInfo)
                    {
                        msgName = null; // initialize to null

                        if ((string)idNdesc.Key == idString)
                        {
                            msgName = idNdesc.Value.ToString().TrimEnd(' ');

                            if (msgName.Length < 7)
                                CANmonitorText += msgName + "\t\t";
                            else
                                CANmonitorText += msgName + "\t";

                            //Once the msgName is found exit the foreach loop
                            break;
                        }

                    }

                    // If the DBC doesn't contain the MsgName insert 2 TABs
                    // So if the Form displays a blank name Check that the DBC File contains the J1939 ID-PGN/SPNs Info
                    if (msgName == null)
                        CANmonitorText += "\t\t";

                    // Set the byte length of all the J1939 PGNs. The multipacket length is the total Byte Count from the TPCM & standard msgs use the dlc length.                                
                    len = CanMsgProc.msgDLCs[el];

                    int msgByteCntr = 0;

                    // Add the data to the TEXT String
                    for (; msgByteCntr < len; msgByteCntr++)
                    {
                        CANmonitorText += CanMsgProc.msgData[el][msgByteCntr].ToString("X2") + " ";
                        
                        if (CanMsgProc.msgIDs[el] == DM1_E && len > 8 )
                        {
                            // When finished printing the data to the monitor
                            if (msgByteCntr == len - 1)
                                CanMsgProc.DM1tpServiced = true; // This flag assures the momentary DTCs, 2 DTCs or more, are shown on the screen
                        }/*
                        if (CanMsgProc.msgIDs[el] == DM1_T && len > 8)
                        {
                            // Make a copy of the DATA here
                            if (msgByteCntr == len - 1)
                                CanMsgProc.DM1tpServiced = true; // This flag assures the momentary DTCs, 2 DTCs or more, are shown on the screen
                        }*/
                    }

                    // For the REQ msg just Pad the text with phantom data bytes to keep the columns algined
                    if (msgByteCntr < 8)
                    {
                        for (; msgByteCntr < 8; msgByteCntr++)
                            CANmonitorText += "   ";     //2 spaces for each phantom data byte and one for the space in between to keep the column alginment
                        len = 8;    // to print CHN num.
                    }

                    // Add the Text String to the Form's richTextBox. The multiPacket messages sort of mess this column up. Decided not to show it.
                    if (len == 8)
                        CANmonitorText += "\tCHN " + CanMsgProc.msgSubnet[el];

                    CANmonitorText += Environment.NewLine;
                }
            }
            return CANmonitorText;
        }

        private string BuildBB2nPWTmonitor()
        {
            string PWTmsgName = null, PWTCANmonitorText = null, shortTime;
            int len;

            for (int el = 0; el < PWTmsgIDs.Count; el++)
            {
                //Calculate the first column which is the message's cylic update rate
                if (PWTmsgTimeStamp[el] >= PWTmsgTimeStamp_pv[el])
                    PWTmsgDeltaTime = PWTmsgTimeStamp[el] - PWTmsgTimeStamp_pv[el];
                else
                    PWTmsgDeltaTime = 0;

                string idString = PWTmsgIDs[el].ToString("X8");

                shortTime = PWTmsgTimeStamp[el].ToString();

                if (shortTime.Length > 6)
                {
                    // Limit the Description to 12 Characters
                    shortTime = PWTmsgTimeStamp[el].ToString().Substring(0, 5) + "...";

                    PWTCANmonitorText += shortTime + "\t" + PWTmsgDeltaTime.ToString().PadLeft(5, '0') + "\t" + idString + "\t";
                }
                else
                    PWTCANmonitorText += shortTime + "\t\t" + PWTmsgDeltaTime.ToString().PadLeft(5, '0') + "\t" + idString + "\t";

                // Search thru the entries of the DBC File that are stored in the Hash Table
                // Search the hashTable for the MsgName/Description that has been received

                PWTmsgName = null; // initialize to null

                foreach (DictionaryEntry idNdesc in dbcMsgInfo)
                {
                    if ((string)idNdesc.Key == idString)
                    {
                        PWTmsgName = idNdesc.Value.ToString().TrimEnd(' ');

                        if (PWTmsgName.Length < 7)
                            PWTCANmonitorText += PWTmsgName + "\t\t";
                        else
                            PWTCANmonitorText += PWTmsgName + "\t";

                        //Once the msgName is found exit the foreach loop
                        break;
                    }
                }

                // If the DBC doesn't contain the MsgName insert 2 TABs
                if (PWTmsgName == null)
                {
                    PWTCANmonitorText += "\t\t";
                }
                
                // Set the byte length of all the J1939 PGNs. The multipacket length is the total Byte Count from the TPCM & standard msgs use the dlc length.                                
                len = PWTmsgDLCs[el];

                
                // Add the data to the TEXT String
                for (int msgByte = 0; msgByte < len; msgByte++)
                {
                    PWTCANmonitorText += PWTmsgData[el][msgByte].ToString("X2") + " ";
                }
                // Add the Text String to the Form's richTextBox. The multiPacket messages sort of mess this column up. Decided not to show it.
                if (len <= 8)
                    PWTCANmonitorText += "\tCHN 4";

                // Add a note to the end of the data. This messed up the channel column alignment
                if (idString == "18ECFF00")
                {
                    PWTCANmonitorText += "\tBAM for PGN ";
                    if (pwtTPCM_E_active == true)
                        PWTCANmonitorText += "VP26_E";
                }

                PWTCANmonitorText += Environment.NewLine;
            }

            string BB2msgName = null, BB2CANmonitorText = null;

            for (int el = 0; el < BB2msgIDs.Count; el++)
            {
                if (BB2msgTimeStamp[el] > BB2msgTimeStamp_pv[el])
                    BB2msgDeltaTime = BB2msgTimeStamp[el] - BB2msgTimeStamp_pv[el];
                else
                    BB2msgDeltaTime = 0;

                shortTime = BB2msgTimeStamp[el].ToString();

                if (shortTime.Length > 6)
                {
                    // Limit the Description to 12 Characters
                    shortTime = BB2msgTimeStamp[el].ToString().Substring(0, 5) + "...";

                    BB2CANmonitorText += shortTime + "\t" + BB2msgDeltaTime.ToString().PadLeft(5, '0') + "\t" + BB2msgIDs[el].ToString("X8") + "\t";
                }
                else
                    BB2CANmonitorText += shortTime + "\t\t" + BB2msgDeltaTime.ToString().PadLeft(5, '0') + "\t" + BB2msgIDs[el].ToString("X8") + "\t";                 

                // Search thru the entries of the DBC File that are stored in the Hash Table
                // Search the hashTable for the MsgName/Description that has been received

                foreach (DictionaryEntry idNdesc in dbcMsgInfo)
                {
                    BB2msgName = null; // initialize to null

                    if ((string)idNdesc.Key == BB2msgIDs[el].ToString("X8"))
                    {
                        BB2msgName = idNdesc.Value.ToString().TrimEnd(' ');

                        if (BB2msgName.Length < 7)
                            BB2CANmonitorText += BB2msgName + "\t\t";
                        else
                            BB2CANmonitorText += BB2msgName + "\t";

                        //Once the msgName is found exit the foreach loop
                        break;
                    }
                }

                // If the DBC doesn't contain the MsgName insert 2 TABs
                if (BB2msgName == null)
                {
                    BB2CANmonitorText += "\t\t";
                }

                // The BB2 only displays Standard 8-Byte Messages
                for (int msgByte = 0; msgByte < 8; msgByte++)
                {
                    BB2CANmonitorText += BB2msgData[el][msgByte].ToString("X2") + " ";
                }
                BB2CANmonitorText += "\tCHN 3";
                BB2CANmonitorText += Environment.NewLine;
            }

            return BB2CANmonitorText + PWTCANmonitorText;
        }
        
        /// <summary>
        /// Stop the timers and Kill all the processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            mmTimer.Stop();
            GUItimer.Stop();
            Process.GetCurrentProcess().Kill();
            Application.Exit();
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really Quit?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }
        private void sWVersionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(PersonalDirPath + "SWvers: " + SWvers, "CAN Setup Version");
        }
        private void kvaserDriverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This APP was developed using Kv_DRVR 8.40.102. The current version is:\r\n" + CANsetupForm.kvDrvrString, "KVASER STATUS BOX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void configHintsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("The CANconfig.txt file is in: " + dbcDirPath + "\\CANmonitor\nThis file simply holds the folder's path and DBC file name.\nAdditionally, to fully exploit info of the the DTC monitor the user needs a SPN INI file. " +
                "I've included a file in the project to use as an example.\nPlace it in your user's OneDrive\\Documents\\CANmonitor folder.", "Help");
        }
        private void viewTPMsgsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tpForm.ShowDialog() == DialogResult.OK)
            {
                saveEMSnACMFaultsToolStripMenuItem.Enabled = true;
            }
        }
        private void releaseNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (versionHistory.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void saveEMSnACMFaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog x = new SaveFileDialog();
            x.Filter = "TXT|*.txt|ASC|*.asc";
            x.FileName = "FaultText.txt";
            x.InitialDirectory = dbcDirPath + "\\CANmonitor";

            if (x.ShowDialog() != DialogResult.Cancel)
            {
                richTextBox1.Text = MultiPacketForm.EMSnACMfaults;
                // Write Output File 
                richTextBox1.SaveFile(x.FileName, RichTextBoxStreamType.PlainText);
                File.WriteAllLines(x.FileName, richTextBox1.Lines, Encoding.UTF8);
            }
        }
        private void createANewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = PersonalDirPath + @"\CANmonitor\CANconfig.txt";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                MessageBox.Show("CONFIGURATION FILE DELETED.", "Check Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            if (idNselcCAN_HW.ShowDialog() == DialogResult.OK)
            {
                CANstatus_cB.Checked = true;
            }
            else if (DialogResult == DialogResult.Abort)
            {
                MainWin_FormClosing(null, null);
            }
            else if (DialogResult == DialogResult.Cancel)
            {
                MainWin_FormClosing(null, null);
            }
        }
        private void viewCANMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoClosingMessageBox.Show(CANsetupForm.txtCAN_HW, "CANsetup Info", 3500);
        }

    }
}
