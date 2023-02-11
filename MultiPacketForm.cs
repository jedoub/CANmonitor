using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Linq;

/*
 * 
SA 00 SPN 000636 Engine Position Sensr                           FMI 007 Mech System Not Responding____"
SA 00 SPN 000098 Engine Oil Level                                FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000651 Engine Fuel 1 Inj Cyl 1                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000652 Engine Fuel 1 Inj Cyl 2                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000653 Engine Fuel 1 Inj Cyl 3                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000654 Engine Fuel 1 Inj Cyl 4                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000655 Engine Fuel 1 Inj Cyl 5                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000656 Engine Fuel 1 Inj Cyl 6                         FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 003216 EATS 1 SCR In NOx 1                             FMI 009 Abnormal Update rate__________"
SA 00 SPN 003226 EATS 1 Out NOx 1                                FMI 009 Abnormal Update rate__________"
SA 00 SPN 000411 Engine EGR 1 Diff Pressure                      FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000102 Engine In Manifold 1 Pressure                   FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000175 Engine Oil Temperature 1                        FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000105 Engine In Manifold 1 Temperature                FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 003480 EATS 1 Fuel Pressure 1                          FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000729 (-)Engine In Air Htr Drvr 1                     FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000100 Engine Oil Pressure                             FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000730 (-)Engine In Air Htr Drvr 2                     FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000094 Engine Fuel Delivery Pressure                   FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000110 Engine Coolant Temperature                      FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000101 Engine Crankcase Pressure1                      FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 002629 Engine Turbo 1 Compr Out Temperature            FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000412 Engine EGR 1 Temperature                        FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000626 Engine Start Enable Device 1                    FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 003490 EATS 1 Purge Air Act                            FMI 005 Current < Normal || Open Circ_"
SA 00 SPN 000647 (-)Engine Fan Clutch 1 Output Drvr              FMI 005 Current < Normal || Open Circ_"

SA 61 SPN 004375 EATS DEF Pump Drv Percage                       FMI 012 Bad Device or Component_______"
SA 61 SPN 004376 EATS DEF Dosing Unit 1 Diverter Valve           FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 000744 Retarder Modulation Solenoid Valve              FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 003510 Sensr supply voltage 2                          FMI 003 Voltage > Normal || Shorted Hi"
SA 61 SPN 000119 Hydraulic Retarder Pressure                     FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 003251 EATS 1 DPF Diff Pressure                        FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 003598 ECU Pwr Output Supply Voltage 2                 FMI 003 Voltage > Normal || Shorted Hi"
SA 61 SPN 004356 EATS DEF Line Htr 3                             FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 004354 EATS DEF Line Htr 1                             FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 003363 EATS 1 DEF Tank Htr                             FMI 005 Current < Normal || Open Circ_"
SA 61 SPN 000628 (-)Engineering Only Program Memory              FMI 012 Bad Device or Component_______"
SA 61 SPN 005394 EATS 1 DEF Doser Valve 1                        FMI 005 Current < Normal || Open Circ_"
 */

namespace CANmonitor
{
    public partial class MultiPacketForm : Form
    {
        public static string EMSnACMfaults;

        private static int maxDescLen = 0;
        private static string[] SPNstrings = new string[524288];
        private static string[] FMIs = new string[256];

        public MultiPacketForm()
        {
            InitializeComponent();
        }

        private void MultiPacketForm_Load(object sender, EventArgs e)
        {
            var resourceName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CANmonitor\\J1939SPNs.ini";
            int indx;
            string spnIndx;
            string DTCdesc = "SPN Description is TBD";
            DTCdesc = DTCdesc.PadRight(47);

            // Initialize all the possible SPN numbers with the text above.
            for (UInt32 w = 0; w < 524288; w++)
                SPNstrings[w] = DTCdesc;


            if (File.Exists(resourceName) == true)
            {
                System.IO.StreamReader objReader;
                objReader = new System.IO.StreamReader(resourceName);
                string textline;
                do
                {
                    textline = objReader.ReadLine();    // Disregard the comment lines; they start with a semicolon ; If you modify this file for your use, you should either rename it or copy it
                } while (textline.Contains(";"));         

                textline = objReader.ReadLine();   // Throw this line away [SPNs]

                do
                {
                    textline = objReader.ReadLine();    // 1st Real Line describing an SPN is [16	=	Engine Fuel Filter Suction Side]
                    indx = textline.IndexOf("=", StringComparison.CurrentCulture) + 1;
                    DTCdesc = textline.Substring(indx, textline.Length - indx);
                    DTCdesc = DTCdesc.TrimStart(' ', '\t');

                    if (DTCdesc.Length < 47)
                        DTCdesc = DTCdesc.PadRight(47);

                    if (DTCdesc.Length > maxDescLen)
                        maxDescLen = DTCdesc.Length;

                    spnIndx = textline.Substring(0, indx - 1);  
                    spnIndx = spnIndx.TrimEnd(' ', '\t');       //TAB or SPACE either one gets trimmed

                    // First set the build action of the text file to "EmbeddedResource" by right clicking on the file 
                    // and editing the properties under the RESOURCE folder in the Solution Explorer.
                    // Also make sure that the build action is to copy the files to the output folder.
                    // Finally, make sure they are packaged along with the executable if you create an installer.
                    //********************************************************************************
                    // DM1 J1939 FAULT DATABASE FILE
                    //Initialize all SPNStrings to TBD then over write with the know descriptions
                    //********************************************************************************
                    uint SPN_ID = Convert.ToUInt32(spnIndx, 10);
                    SPNstrings[SPN_ID] = DTCdesc;

                } while (objReader.Peek() != -1);

                objReader.Close();
            }
            else
                // The File is missing                            
                MessageBox.Show("Error: missing file " + resourceName + "\n\nYou need a conforming SPN.ini file containing descriptions.", "Check Information");

            // Initialize the Strings with TBD
            for (int w = 0; w < 256; w++)
                FMIs[w] = "            TBD || ???        \"";

            FMIs[0] = "Signal Valid > NormOp::Hi_____\"";
            FMIs[1] = "Signal Valid < NormOp::Hi_____\"";
            FMIs[2] = "Data Erratic__________________\"";
            FMIs[3] = "Voltage > Normal || Shorted Hi\"";
            FMIs[4] = "Voltage < Normal || Shorted Lo\"";
            FMIs[5] = "Current < Normal || Open Circ_\"";
            FMIs[6] = "Current > Normal || Circ GNDed\"";
            FMIs[7] = "Mech System Not Responding____\"";
            FMIs[8] = "Abnormal Freq > NormOp________\"";
            FMIs[9] = "Abnormal Update rate__________\"";
            FMIs[10] = "Abnormal Rate of Change_______\"";
            FMIs[11] = "Root Cause not ID'd___________\"";
            FMIs[12] = "Bad Device or Component_______\"";
            FMIs[13] = "Out of Calibration____________\"";
            FMIs[14] = "Special instructions__________\"";
            FMIs[15] = "Valid > NormOp::Low___________\"";
            FMIs[16] = "Valid > NormOp::Med___________\"";
            FMIs[17] = "Valid < NormOp::Low___________\"";
            FMIs[18] = "Valid < NormOp::Med___________\"";
            FMIs[19] = "Recv'd Network Error__________\"";
            FMIs[31] = "Fault Conditions Exist________\"";

            timer1.Enabled = true;
            timer1.Start();

            richTextBox2.ForeColor = Color.DarkBlue;
            richTextBox2.Font = new Font("Lucida Console", 8);
            richTextBox2.Clear();

            richTextBox1.ForeColor = System.Drawing.Color.DarkBlue;
            richTextBox1.Font = new Font("Lucida Console", 7);
            richTextBox1.BackColor = Color.Bisque;
            richTextBox1.Clear();

            richTextBox4.ForeColor = Color.DarkBlue;
            richTextBox4.Font = new Font("Lucida Console", 8);
            richTextBox4.Clear();

            richTextBox3.ForeColor = System.Drawing.Color.DarkBlue;
            richTextBox3.Font = new Font("Lucida Console", 7);
            richTextBox3.BackColor = Color.Bisque;
            richTextBox3.Clear();

            richTextBox6.ForeColor = Color.DarkBlue;
            richTextBox6.Font = new Font("Lucida Console", 7);
            richTextBox6.Clear();

            richTextBox7.ForeColor = System.Drawing.Color.DarkBlue;
            richTextBox7.Font = new Font("Lucida Console", 7);
            richTextBox7.BackColor = Color.Bisque;
            richTextBox7.Clear();

            //richTextBox5.ForeColor = System.Drawing.Color.DarkBlue;
            //richTextBox5.Font = new Font("Lucida Console", 8);
            //richTextBox5.BackColor = Color.Bisque;
            //richTextBox5.Clear();

            //  MSG ID         CTRL BYTES PKTS   PGN FECA
            // 18ECFF00x Rx d 8 20 (86 00) 14 FF (CA FE 00) BAM

            //                  LS LS SPN19  FMI  OC LSB
            // 18EBFF00x Rx d 8 01 50 3F 7D 02 09 01 98     TPDT
            // 18EBFF00x Rx d 8 02 0D 05 01 61 00 04 01     TPDT
            // 18EBFF00x Rx d 8 03 45 0A 05 01 9C 01 05     TPDT
            // 18EBFF00x Rx d 8 04 01 62 00 05 01 45 0A     TPDT
            // 18EBFF00x Rx d 8 05 00 01 F8 0B 0B 01 D9     TPDT
            // 18EBFF00x Rx d 8 06 02 07 01 DA 02 07 01     TPDT
            // 18EBFF00x Rx d 8 07 81 02 07 01 CC 12 04     TPDT
            // 18EBFF00x Rx d 8 08 01 A2 0D 05 01 CD 12     TPDT
            // 18EBFF00x Rx d 8 09 04 01 E4 0D 05 01 8B     TPDT
            // 18EBFF00x Rx d 8 0A 02 03 01 8C 02 05 01     TPDT
            // 18EBFF00x Rx d 8 0B 8D 02 05 01 8E 02 05     TPDT
            // 18EBFF00x Rx d 8 0C 01 8F 02 05 01 90 02     TPDT
            // 18EBFF00x Rx d 8 0D 05 01 D3 04 02 0D E7     TPDT
            // 18EBFF00x Rx d 8 0E 07 09 01 DC 07 09 01     TPDT
            // 18EBFF00x Rx d 8 0F E1 07 09 01 E7 07 13     TPDT
            // 18EBFF00x Rx d 8 10 01 E1 07 13 01 A9 0C     TPDT
            // 18EBFF00x Rx d 8 11 13 01 21 0D 0C 01 B1     TPDT
            // 18EBFF00x Rx d 8 12 0C 13 01 AD 0C 13 01     TPDT
            // 18EBFF00x Rx d 8 13 74 02 0C 01 F8 0B 10     TPDT
            // 18EBFF00x Rx d 8 14 01 FF FF FF FF FF FF     TPDT

            /*
                4)4 [FF] (7C 02 0)7 [06] (62       7
                
                00 0)5 [01] (8B 02 0)5 [01]     14
                (8C 02 0)5 [01] (8D 02 0)5      21
                [01] (8E 02 0)5 [01] (8F 02     28
                0)5 [01] (90 02 0)5 [01] (90    35
                
                0C 0)9 [05] (9A 0C 0)9 [0A]     42
                (9B 01 0)5 [01] (66 00 0)5 
                [01] (AF 00 0)5 [01] (69 00 
                0)5 [01] (98 0D 0)5 [01] (D9    63 
                
                02 0)5 [01] (64 00 0)5 [01]     70
                DA 02 05 01 5E 00 05 
                01 6E 00 05 01 65 00 
                05 01 45 0A 05 01 9C            91 
                
                01 05 01 72 02 05 7E            98
                A2 0D 05 67 87 02 05 
                01 E4 0D 05 67 E7 07 
                09 22 E7 07 13 1F D3            119
                
                07 0)9 [06] (DF F0 E)9 [01]     126
                (81 02 0)9 [04] (6A F0 E)9      133
                36 5B 00 04 04 2E 02            140
                03 0B 
             * */
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            FLASHING LAMP BIT DEFINITION
            00 Slow Flash (1 Hz, 50% duty cycle)
            01 Fast Flash (2 Hz or faster, 50% duty cycle)
            10 Class C DTC (for WWH OBD discriminatory display systems, not applicable for other
            OBD non-discriminatory display systems)
            11 Unavailable / Do Not Flash
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

            richTextBox2.Text = " SA  LS  LS \t\tMIL\tRSL\tAWL\tPEL\t\tXMIL\tXRSL\tXAWL\tXPEL";
            richTextBox2.Text += Environment.NewLine;
            richTextBox2.Text += " EMS ";

            // Get the index of the RCV'D List using the extended msgID
            int msgIDindx = CanMsgProc.msgIDs.FindIndex(s => s == Form1.DM1_E);

            richTextBox2.Text += CanMsgProc.msgData[msgIDindx][0].ToString("X2") + "  ";
            richTextBox2.Text += CanMsgProc.msgData[msgIDindx][1].ToString("X2") + "  ";
            
            richTextBox2.Text += "  \t" + ((0xC0 & CanMsgProc.msgData[msgIDindx][0]) >> 6).ToString("X2")
                + "  \t" + ((0x30 & CanMsgProc.msgData[msgIDindx][0]) >> 4).ToString("X2") 
                + "  \t" + ((0xC & CanMsgProc.msgData[msgIDindx][0]) >> 2).ToString("X2")
                + "  \t" + (0x3 & CanMsgProc.msgData[msgIDindx][0]).ToString("X2")
                + "  \t\t" + ((0xC0 & CanMsgProc.msgData[msgIDindx][1]) >>6).ToString("X2")
                + "  \t" + ((0x30 & CanMsgProc.msgData[msgIDindx][1]) >> 4).ToString("X2")
                + "  \t" + ((0xC & CanMsgProc.msgData[msgIDindx][1]) >> 2).ToString("X2")
                + "  \t" + (0x3 & CanMsgProc.msgData[msgIDindx][1]).ToString("X2");

            richTextBox1.Text = " SA\tSPN19\tDescription\t\t\t\tFMI\t\tCODE\t\tOC";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;

            // RICHTEXTBOX1 DISPLAYS THE DM1E FAULTS
            richTextBox1.Lines = displayDMinfo(richTextBox1, 0, CanMsgProc.msgDLCs[msgIDindx], CanMsgProc.msgData[msgIDindx]);
            
            bool hitSearch = false;
            for (int c = 0; c < richTextBox1.Lines.Count(); c++)
            {   // Parse through the RTB line-by-line
                if (hitSearch == true)
                {
                    hitSearch = highlightLineContaining(richTextBox1, c, "000", Color.DarkRed);
                }
                else
                    hitSearch = highlightLineContaining(richTextBox1, c, "Snapshot", Color.DarkRed);
            }

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // RICHTEXTBOX3 DISPLAYS THE DM1A FAULTS
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            richTextBox4.Text = " SA  LS  LS \t\tMIL\tRSL\tAWL\tPEL\t\tXMIL\tXRSL\tXAWL\tXPEL";
            richTextBox4.Text += Environment.NewLine;
            richTextBox4.Text += " ACM ";

            // Get the index of the RCV'D List using the extended msgID
            msgIDindx = CanMsgProc.msgIDs.FindIndex(s => s == Form1.DM1_A); 
            
            richTextBox4.Text += CanMsgProc.msgData[msgIDindx][0].ToString("X2") + "  ";
            richTextBox4.Text += CanMsgProc.msgData[msgIDindx][1].ToString("X2") + "  ";

            richTextBox4.Text += "  \t" + ((0xC0 & CanMsgProc.msgData[msgIDindx][0]) >> 6).ToString("X2")
                + "  \t" + ((0x30 & CanMsgProc.msgData[msgIDindx][0]) >> 4).ToString("X2")
                + "  \t" + ((0xC & CanMsgProc.msgData[msgIDindx][0]) >> 2).ToString("X2")
                + "  \t" + (0x3 & CanMsgProc.msgData[msgIDindx][0]).ToString("X2")
                + "  \t\t" + ((0xC0 & CanMsgProc.msgData[msgIDindx][1]) >> 6).ToString("X2")
                + "  \t" + ((0x30 & CanMsgProc.msgData[msgIDindx][1]) >> 4).ToString("X2")
                + "  \t" + ((0xC & CanMsgProc.msgData[msgIDindx][1]) >> 2).ToString("X2")
                + "  \t" + (0x3 & CanMsgProc.msgData[msgIDindx][1]).ToString("X2");

            richTextBox3.Text = " SA\tSPN19\tDescription\t\t\t\tFMI\t\tCODE\t\tOC";
            richTextBox3.Text += Environment.NewLine;
            richTextBox3.Text += Environment.NewLine;

            richTextBox3.Lines = displayDMinfo(richTextBox3, 61, CanMsgProc.msgDLCs[msgIDindx], CanMsgProc.msgData[msgIDindx]);

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // RICHTEXTBOX6 DISPLAYS THE CAN1 SUBNET NODES
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            CanMsgProc.msgNodes.Sort();

            richTextBox6.Text = "Nodes on CAN1:";
            richTextBox6.Text += Environment.NewLine;

            foreach (int Node in CanMsgProc.msgNodes)
            {
                foreach (DictionaryEntry nodeNdesc in Form1.dbcNodes)
                {
                    int NodeNum = Convert.ToInt32((string)nodeNdesc.Value);
                    if (NodeNum == Node)
                    {
                        richTextBox6.Text += " " + nodeNdesc.Key + " " + nodeNdesc.Value + ",";
                        break;
                    }
                }
                
            }

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // RICHTEXTBOX7 DISPLAYS THE DM1R FAULTS
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // Get the index of the RCV'D List using the extended msgID
            msgIDindx = Form1.msgIDs.FindIndex(s => s == Form1.DM1_R);
            
            richTextBox6.Text += Form1.msgData[msgIDindx][0].ToString("X2") + "  ";
            richTextBox6.Text += Form1.msgData[msgIDindx][1].ToString("X2") + "  ";

            richTextBox6.Text += "  \t" + ((0xC0 & Form1.msgData[msgIDindx][0]) >> 6).ToString("X2")
                + "  \t" + ((0x30 & Form1.msgData[msgIDindx][0]) >> 4).ToString("X2")
                + "  \t" + ((0xC & Form1.msgData[msgIDindx][0]) >> 2).ToString("X2")
                + "  \t" + (0x3 & Form1.msgData[msgIDindx][0]).ToString("X2")
                + "  \t\t" + ((0xC0 & Form1.msgData[msgIDindx][1]) >> 6).ToString("X2")
                + "  \t" + ((0x30 & Form1.msgData[msgIDindx][1]) >> 4).ToString("X2")
                + "  \t" + ((0xC & Form1.msgData[msgIDindx][1]) >> 2).ToString("X2")
                + "  \t" + (0x3 & Form1.msgData[msgIDindx][1]).ToString("X2");

            richTextBox7.Text = " SA\tSPN19\tDescription\t\t\t\tFMI\t\tCODE\t\tOC";
            richTextBox7.Text += Environment.NewLine;
            richTextBox7.Text += Environment.NewLine;

            richTextBox7.Lines = displayDMinfo(richTextBox7, Form1.msgDLCs[msgIDindx], Form1.msgData[msgIDindx]); 
            */

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // RICHTEXTBOX5 DISPLAYS THE CI_E Component Identification string
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            string asciiCI_E = new ASCIIEncoding().GetString(Form1.CI_Emsg).TrimEnd('\0');
            richTextBox5.Text = " CI_E Component Identification string: " + asciiCI_E;
            richTextBox5.Text += Environment.NewLine;
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // RICHTEXTBOX5 DISPLAYS THE SI_E Software Identification string
            * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            string asciiSI_E = new ASCIIEncoding().GetString(Form1.SI_Emsg).TrimEnd('\0');
            richTextBox5.Text += " SI_E Software Component Identification string: " + asciiSI_E;
            richTextBox5.Text += Environment.NewLine;
            //* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
            //Print out the ENGINE CONFIGURATION MESSAGE for the EMS 
            // EC1_X_E  50 14 AA A0 41 9E 80 25 C6 E0 2E C4 40 38 BB A0 41 FF FF F4 0A 50 46 FF 32 FA 7D E1 C0 5D FF FF FF FF FF 00 00 00 00 

            richTextBox5.Text += " EC1_X_E Engine Configuration: ";
            
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[1] * 256 + Form1.EC1_Emsg[0]) * 0.125).PadLeft(4, ' ') + "rpm Lo Idle, ");   // Idle SPD
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[2] - 125)) + "% Torq @ Idle,\n\t");                                 // Idle % TORQ
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[4] * 256 + Form1.EC1_Emsg[3]) * 0.125).PadLeft(4, ' ') + "rpm SPD Pt2, ");    // SPD Pt 2
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[5] - 125)) + "% Torq @ Pt2,\n\t");                                 // Pt 2 % TORQ
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[7] * 256 + Form1.EC1_Emsg[6]) * 0.125).PadLeft(4, ' ') + "rpm SPD Pt3, ");    // SPD Pt 3
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[8] - 125)) + "% Torq @ Pt3,\n\t");                                  // Pt 3 % TORQ
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[10] * 256 + Form1.EC1_Emsg[9]) * 0.125).PadLeft(4, ' ') + "rpm SPD Pt4, ");    // SPD Pt 4
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[11] - 125)) + "% Torq @ Pt4,\n\t");                                  // Pt 4 % TORQ
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[13] * 256 + Form1.EC1_Emsg[12]) * 0.125).PadLeft(4, ' ') + "rpm SPD Pt5, ");    // SPD Pt 5
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[14] - 125)) + "% Torq @ Pt5,\n\t");                                  // Pt 5 % TORQ
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[16] * 256 + Form1.EC1_Emsg[15]) * 0.125).PadLeft(4, ' ') + "rpm Hi Idle, ");    // Hi Idle
            
            richTextBox5.Text += (Convert.ToString((Form1.EC1_Emsg[20] * 256 + Form1.EC1_Emsg[19])) + "Nm Ref Torq");              // Ref Torq
                        
            */
        }

        private string[] displayDMinfo(RichTextBox rtbx, byte srcAddr, int DMlen, byte [] DMmsg)
        {
            int z;    // Indexes the 4 bytes that repeat in the DM1 (SPN, FMI, OC)
            int i;    // Indexes thru the total number of Bytes
            int SPN = 0, msbOfSPN;
            int FMI;
            int OC;

            if (DMlen == 8)
            {
                // There can only be 1 DTC since a second one requires 10 bytes.
                if (DMmsg[0] > 0)
                {
                    SPN = (((DMmsg[4] & 0xE0) >> 5) << 16) + (DMmsg[3] << 8) + DMmsg[2];
                    FMI = (DMmsg[4] & 0x1F);
                    OC = (DMmsg[5] & 0x7F);
                    rtbx.Text += srcAddr.ToString("D3") + "\t" + SPN.ToString("D6") + "  " + SPNstrings[SPN] + "  " + FMI.ToString("D3") + "  " + FMIs[FMI] + "  ";
                    rtbx.Text += OC.ToString("D3") + Environment.NewLine;
                }
                else
                    rtbx.Text += "No Active DTCs" + Environment.NewLine;
            }
            else 
            {
                for (i = 2; i < DMlen;)
                {
                    for (z = 0; z < 4; z++)
                    {
                        switch (z)
                        {
                            case 0:
                                SPN = DMmsg[i++];
                                break;
                            case 1:
                                SPN = (DMmsg[i++] << 8) + SPN;     //16 bits of the SPN
                                break;
                            case 2:
                                msbOfSPN = (DMmsg[i] & 0xE0) >> 5;
                                SPN += msbOfSPN << 16;
                                if (SPN < 524288)
                                {
                                    FMI = (DMmsg[i] & 0x1F);
                                    rtbx.Text += srcAddr.ToString("D3") + "\t" + SPN.ToString("D6") + "  " + SPNstrings[SPN] + "  " + FMI.ToString("D3") + "  " + FMIs[FMI] + "  ";
                                }
                                else
                                {
                                    FMI = (DMmsg[i] & 0x1F);    // THIS else CODE ONLY RUNS WHEN SOMETHING IS WRONG. SAE Limits the SPN to 524288
                                    rtbx.Text += srcAddr.ToString("D3") + "\t" + SPN.ToString("D6") + "  " + SPNstrings[520999] + "  " + FMI.ToString("D3") + "  " + FMIs[FMI] + "  ";
                                }
                                i++;
                                break;
                            case 3:
                                OC = (DMmsg[i++] & 0x7F);
                                rtbx.Text += OC.ToString("D3");
                                break;
                        }
                    }
                    rtbx.Text += Environment.NewLine;
                }
            }

            // I watch for EECU Transient DTCs as a special case. So I have a small duplicate buffer that I read here
            // after the transient DTC disappears.
            if (Form1.DM1Etpmsg[0] > 0 && DMlen == 8)
            {
                rtbx.Text += Environment.NewLine + "Snapshot of all the EECU DTCs when there is an intermittent DTC!" + Environment.NewLine;
                // The order in which the DTCs appear is not certain. Just print out all of them.
                for (i = 2; i < Form1.DM1Etpmsg.Length;)
                {
                    for (z = 0; z < 4; z++)
                    {
                        switch (z)
                        {
                            case 0:
                                SPN = Form1.DM1Etpmsg[i++];
                                break;
                            case 1:
                                SPN = (Form1.DM1Etpmsg[i++] << 8) + SPN;     //16 bits of the SPN
                                break;
                            case 2:
                                msbOfSPN = (Form1.DM1Etpmsg[i] & 0xE0) >> 5;
                                SPN += msbOfSPN << 16;
                                if (SPN > 0 && SPN < 524288)
                                {
                                    FMI = (Form1.DM1Etpmsg[i] & 0x1F);
                                    rtbx.Text += srcAddr.ToString("D3") + "\t" + SPN.ToString("D6") + "  " + SPNstrings[SPN] + "  " + FMI.ToString("D3") + "  " + FMIs[FMI] + "  ";
                                }
                                i++;    // Increment here incase SPN == 0
                                break;
                            case 3:
                                if (SPN > 0 && SPN < 524288)
                                {
                                    OC = (Form1.DM1Etpmsg[i] & 0x7F);
                                    rtbx.Text += OC.ToString("D3");
                                }
                                i++;    // Increment here incase SPN == 0
                                break;
                        }
                    }
                    rtbx.Text += Environment.NewLine;
                }
                Form1.DM1Etpmsg[0] = 0;
            }
            return rtbx.Lines;
        }
        private void OKbtn_Click(object sender, EventArgs e)
        {
            EMSnACMfaults = richTextBox1.Text + "\r\n" + richTextBox3.Text;
            
            MessageBox.Show("To Save the EMS and ACM Faults,\nuse the sub-menu item under View TP Msgs.", "CAN Trace Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            this.DialogResult = DialogResult.OK;
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {   
            this.DialogResult = DialogResult.Cancel;
        }

        private bool highlightLineContaining(RichTextBox rtb, int line, string search, Color color)
        {
            bool hitSearch = false;

            int c0 = rtb.GetFirstCharIndexFromLine(line);
            int c1 = rtb.GetFirstCharIndexFromLine(line + 1);
            if (c1 < 0) c1 = rtb.Text.Length;
            rtb.SelectionStart = c0;
            rtb.SelectionLength = c1 - c0;
            if (rtb.SelectedText.Contains(search))
            {
                rtb.SelectionColor = color;
                hitSearch = true;
            }
            rtb.SelectionLength = 0;
            return hitSearch;
        }
    }
}
