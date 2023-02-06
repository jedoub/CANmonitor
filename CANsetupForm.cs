using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;                        // File handling
using System.Linq;
using System.Management;                // For finding the CANLIB Device Driver
using System.Windows.Forms;
using Kvaser.CanLib;


/* Tips for reusing SW by adding existing Classes into a Project/Solution.
 * A Class is simply a chuck of code that does a particular job.
 * I'll mention two alternatives for how to do this and show a third example.
 * 
 * Objects are created from Classes. The Windows Form itself is a Class, and when you run your program, you are creating Objects.
 * 
 * Now, let's get started.
 * Install the CANLIB SDK found on the Kvaser Website. I have version 5.27 of the Kvaser SDK.
 * 
 * In the Documentation Folder of the SDK the user will find the CANlib API methods for dotNet and C#.
 * Their are hyper-links to rather trivial examples, and several are pure C-code. 
 * The examples, demonstrate the use of the methods in the library.
 * 
 * I hope this code will help jump start your CAN application.
 * 
 * What you won't find in the code are grandiose examples of polymorphism and inheritance constructs touted by C# aficionados.
 * 
 * Create a Windows Forms Project for demonstration purposes in Visual Studio.
 * 
 * ADD the necessary REFERENCES to DLLs used by the APP.
 * The VS Project's Solution Explorer Tab contains the References container object.
 * Add canlibCLSNET & System.Management by navigating to their source.
 * Speaking of DLLs, you can accomplish code reuse in this manner. [In Visual Studio begin by selecting a C++ Project type DLL].
 * 
 * Know/Find the location of the CANsetup project source, or Clone a solution from the GIT repository for your use.
 * 
 * Right-Click on the project in Solution explorer
 * Select Add Existing Item
 * For This particular example, Browse to the xxxForm.cs file in another project, and Single-click to select the file
 * 
 * Only select ONE of the xxxForm.cs files to add to the Project at a time. 
 * If you select multiple .cs files for different WinForms, Visual Studio doesn't add them into the solution correctly. Repeat as necessary.
 * Now that the File is selected in the browse window, mouse over and click the down-arrow button to the right of Add and select Add as Link.
 * Then select the CANsetupForm.designer.cs, (CANsetupForm.resx will be generated), and add as Link. 
 * Code re-use can also be accomplished by copying the source files and then adding the files to the Solution Explorer. The same rules apply.
 * 
 * You now have 2 of the 4 source files necessary for the CANsetup Form referenced by two projects.
 * 
 * I also made an additional Form that pops up a box allowing the user associate a name with a CAN subnet. 
 * I typically use the subnet names like BB1, ESN, BB2, PWT, plus ISN for Instrumentation subnet, but this provides naming flexibility like other generic CAN tools.
 * Separately LINK these source files as well:
 * EnterSubnetForm.cs, EnterSubnetForm.Designer.cs, (EnterSubnetForm.resx will be generated)
 * 
 * The source files for the CANsetup utilize the same namespace so a single using reference is all that is necessary in the Form1.cs of your project. (i.e. using CANsetup;)
 * 
 * To work with CANsetup Form the developer also needs to instantiate the class. This is added using the new keyword as shown below.
 * CANsetupForm idNselcCAN_HW = new CANsetupForm();
 * 
 * To automate the launch of a Form contained in a Class I use the C# ShowDialog() function.
 * 
 * Also, note the use of private and public data types. This is a simplistic way to exchange data between different classes contained in different files of a project.
 * 
 * Another, less manageable way to reuse C code is to copy the source files into the project's folder and add the files into the VS Solution Explorer. 
 */

namespace CANsetup
{
    /// <summary>
    /// This APP polls the CAN HW to determine the type and quantity of devices connected to the PC.
    /// This information is used to populate the ComboBox Selections.
    /// The user then "maps" the CAN HW to the physical connections used in the test bed.
    /// Attributes like Subnet Name, Device, Channel and Baudrate are assigned based on the user's selections in the drop downs.
    /// The channels are opened and initialized in preparation to being used by the "Main" Application.
    /// Finally, a configuration file is written and reused so this effort is only necessary once as long as the physical HW remains the same.
    /// </summary>
    public partial class CANsetupForm : Form
    {
        // Instantiate Class using the New keyword.
        EnterSubnetForm typeSubnetName = new EnterSubnetForm();
        //**********************************************************************************************************

        //******************************************* KVASER CAN OBJECTS  ******************************************
        //**********************************************************************************************************
        private static Canlib.canStatus can_status;   //A KVASER CANLIBRARY OBJECT
        private static object CANinfo, devSN;
        //**********************************************************************************************************

        public static String kvDrvrString = null;
        public static int devCnt = 0, CAN1hnd = 0, CAN2hnd = 1, CAN3hnd = 2, CAN4hnd = 3, CAN5hnd = 4, subNetCnt = 0;
        public static string[] textLine = new string[12], subNetName = new string[6] { "virtual", "virtual", "virtual", "virtual", "virtual", "virtual" };
        public static int[] channelCntOfDevice = new int[16];
        public static Boolean EnterCAN1X = false, EnterCAN2X = true, EnterCAN3X = true, EnterCAN4X = true, EnterCAN5X = true;
        public static string txtCAN_HW = null;
        public static Boolean chn1ConfigSuccess = false, chn2ConfigSuccess = false, chn3ConfigSuccess = false, chn4ConfigSuccess = false, chn5ConfigSuccess = false;
        private static Boolean CAN1chanSelc = false, CAN2chanSelc = false, CAN3chanSelc = false, CAN4chanSelc = false, CAN5chanSelc = false;
        private static int chnCnt = 0, chnCntPerDev = 0, channelCount = 0, lineCnt = 0;
        private static string[] serialNumOfDevice = new string[5] { "0", "1", "2", "3", "4" };
        private static string devSN_pv = null;
        private static string dirName = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CANsetup";
        private static Boolean useCANconfigFile_s = false;

        public CANsetupForm()
        {
            InitializeComponent();

            // CALL THE KVASER CANLIBRARY
            Canlib.canInitializeLibrary();
        }

        #region "Create CAN configuration OR Read CAN configuration File"

        /// <summary>
        /// ******************** INIT CAN HW Using a CANconfig file or Interrogation *********************************
        /// Interrogate the CAN Devices connected to the PC and populate the CAN comboBoxes with their SNs and ChnCnt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KvCANsetupForm_Load(object sender, EventArgs e)
        {
            int endIndx = 0;
            string subNetID = "";
            chnCnt = 0;
            devCnt = 0;
            devSN_pv = "0";

            CAN_HW_tB.Font = new Font("Lucida Console", 7);

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver");

            groupBox1.ForeColor = System.Drawing.Color.Green;
            groupBox1.Font = new Font("Lucida Console", 7);

            //Display the Kvaser Driver running on the PC
            foreach (ManagementObject obj in searcher.Get())
            {
                //The full kvDrvrString = String.Format("Dev='{0}', Make='{1}', Driver='{2}' ", obj["DeviceName"], obj["Manufacturer"], obj["DriverVersion"]);
                kvDrvrString = String.Format("{0}, Driver {1}", obj["Manufacturer"], obj["DriverVersion"]);

                // End the foreach iteration since the Kvaser Driver was encountered.
                if (kvDrvrString.Contains("Kvaser AB"))
                    break;
            }
            // Print the Kvaser driver version into the Title area of the group box
            groupBox1.Text += " " + kvDrvrString;

            // Automate the Gui Refresh click
            refresh_Btn_Click(null, null);

            if (!Directory.Exists(dirName))
            {
                useCANconfigFile_s = false;

                Directory.CreateDirectory(dirName);
            }
            else
            {
                // Directory found now check if the File is present
                if (!System.IO.File.Exists(dirName + "\\CANconfig.txt"))
                {
                    // The File is missing                            
                    MessageBox.Show("Error NO CONFIGURATION FILE FOUND in\n" + dirName + "\\CANconfig.txt.\n\nYou must Initialize and Select the CAN HW.", "Check Information");

                    useCANconfigFile_s = false;

                    timer1.Start();
                }
                else
                {
                    if (MessageBox.Show("CANconfig.txt FOUND:\n" + dirName + "\n\nDo you want to use it?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        lineCnt = 0;

                        // Read the config File and save each line into a string ARRAY
                        using (StreamReader sr = new StreamReader(dirName + "\\CANconfig.txt"))
                        {
                            while (sr.Peek() >= 0)
                            {
                                try
                                {
                                    textLine[lineCnt++] = sr.ReadLine();
                                }
                                catch (Exception err)
                                {
                                    MessageBox.Show(err.ToString(), "Check Information");
                                }
                            }
                        }

                        List<string> LineOfText = textLine.ToList<string>();

                        LineOfText.RemoveAll(str => String.IsNullOrEmpty(str));

                        textLine = LineOfText.ToArray();
                        lineCnt = LineOfText.Count;

                        if (lineCnt == 0)                       // Check if there are Lines in the Config File
                        {
                            // File contains no information                            
                            MessageBox.Show("Error in " + dirName + "\\CANconfig.txt.\nYou must re-configure the CAN HW.", "Check Information");

                            useCANconfigFile_s = false;
                        }
                        else
                        {
                            while (--lineCnt >= 0)
                            {
                                switch (lineCnt)
                                {
                                    case 0:
                                        endIndx = textLine[lineCnt].IndexOf('=') - 1;
                                        subNetID = textLine[lineCnt].Substring(0, endIndx);
                                        CAN1Lbl.Text = CAN1Lbl.Text.Replace("CAN1", "subNetID");
                                        CAN1chnLbl.Text = CAN1chnLbl.Text.Replace("CAN1", "subNetID");
                                        CAN1BdLbl.Text = CAN1Lbl.Text.Replace("CAN1", "subNetID");
                                        subNetName[1] = subNetID;
                                        break;
                                    case 1:
                                        endIndx = textLine[lineCnt].IndexOf('=') - 1;
                                        subNetID = textLine[lineCnt].Substring(0, endIndx);
                                        CAN2Lbl.Text = CAN2Lbl.Text.Replace("CAN2", "subNetID");
                                        CAN2chnLbl.Text = CAN2chnLbl.Text.Replace("CAN2", "subNetID");
                                        CAN2BdLbl.Text = CAN2Lbl.Text.Replace("CAN2", "subNetID");
                                        subNetName[2] = subNetID;
                                        break;
                                    case 2:
                                        endIndx = textLine[lineCnt].IndexOf('=') - 1;
                                        subNetID = textLine[lineCnt].Substring(0, endIndx);
                                        CAN3Lbl.Text = CAN3Lbl.Text.Replace("CAN3", "subNetID");
                                        CAN3chnLbl.Text = CAN3chnLbl.Text.Replace("CAN3", "subNetID");
                                        CAN3BdLbl.Text = CAN3Lbl.Text.Replace("CAN3", "subNetID");
                                        subNetName[3] = subNetID;
                                        break;
                                    case 3:
                                        endIndx = textLine[lineCnt].IndexOf('=') - 1;
                                        subNetID = textLine[lineCnt].Substring(0, endIndx);
                                        CAN4Lbl.Text = CAN4Lbl.Text.Replace("CAN4", "subNetID");
                                        CAN4chnLbl.Text = CAN4chnLbl.Text.Replace("CAN4", "subNetID");
                                        CAN4BdLbl.Text = CAN4Lbl.Text.Replace("CAN4", "subNetID");
                                        subNetName[4] = subNetID;
                                        break;
                                    case 4:
                                        endIndx = textLine[lineCnt].IndexOf('=') - 1;
                                        subNetID = textLine[lineCnt].Substring(0, endIndx);
                                        CAN5Lbl.Text = CAN5Lbl.Text.Replace("CAN5", "subNetID");
                                        CAN5chnLbl.Text = CAN5chnLbl.Text.Replace("CAN5", "subNetID");
                                        CAN5BdLbl.Text = CAN5Lbl.Text.Replace("CAN5", "subNetID");
                                        subNetName[5] = subNetID;
                                        break;
                                }
                            }

                            lineCnt = LineOfText.Count; // Restore the LineCnt variable

                            UseConfig();
                        }
                    }
                    else
                    {
                        useCANconfigFile_s = false;
                        timer1.Start();
                    }
                }
            }
            // Reduce the Device description to fit the text box  
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Kvaser ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Professional ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Light ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace(" v2 ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("HS/H", "2xHS");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("#", "Dev ");
        }

        private void UseConfig()
        {
            // First line read above to see if it contains text.
            // string textline = objReader.ReadLine();    //Line1
            // Since their isn't an absolute standard on how to connect a CAN line
            // Each text line is tested for the possibility that it could be connected to any CAN line (BB1, ESN, BB2, PWT)
            useCANconfigFile_s = true;

            for (int c = 0; c < lineCnt; c++)
            {
                if (textLine[c].Contains(subNetName[1]))
                {
                    // Extract the Serial Number
                    int strIndx = textLine[c].LastIndexOf('N') + 2;
                    int endIndx = textLine[c].IndexOf('@') - 1;
                    string sn = textLine[c].Substring(strIndx, endIndx - strIndx);

                    bool hwDevHit = false;
                    int loopCntr = 0;

                    // Check if the sn contained in the ConfigFile is currently one of the CAN interface devices connected to the PC
                    while (loopCntr < devCnt)
                    {
                        if (sn.Contains(serialNumOfDevice[loopCntr]))
                        {
                            chnOneSN_comboBox.Items.Add(sn);
                            chnOneSN_comboBox.SelectedItem = sn;
                            chnOneSN_comboBox.Enabled = true;

                            hwDevHit = true;
                        }
                        loopCntr++;
                    }

                    if (!hwDevHit)
                    {
                        // The Configuration File contains bad/stale information of the CAN HW connected
                        if (MessageBox.Show("The Config File has a stale SN for the CAN HW connected to BB1!\n\nDo you need to Initialize the CAN HW connected?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("Re-Launch the APP and Initialize the connected CAN HW.", "Check Information");

                            System.IO.File.Delete(dirName + "\\CANconfig.txt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("This subnet entry will simply be erased from the CANconfig.", "Check Information");
                    }
                    else
                    {
                        if (textLine[c].Contains("(Channel 0)"))
                        {
                            chnOne_comboBox.Items.Add("1");
                            chnOne_comboBox.SelectedItem = "1";
                        }
                        else if (textLine[c].Contains("(Channel 1)"))
                        {
                            chnOne_comboBox.Items.Add("2");
                            chnOne_comboBox.SelectedItem = "2";
                        }
                        else if (textLine[c].Contains("(Channel 2)"))
                        {
                            chnOne_comboBox.Items.Add("3");
                            chnOne_comboBox.SelectedItem = "3";
                        }
                        else if (textLine[c].Contains("(Channel 3)"))
                        {
                            chnOne_comboBox.Items.Add("4");
                            chnOne_comboBox.SelectedItem = "4";
                        }
                        else if (textLine[c].Contains("(Channel 4)"))
                        {
                            chnOne_comboBox.Items.Add("5");
                            chnOne_comboBox.SelectedItem = "5";
                        }

                        //chnOneBR_comboBox.Enabled = true;
                        if (textLine[c].Contains("500"))
                            chnOneBR_comboBox.SelectedIndex = 1;
                        else
                            chnOneBR_comboBox.SelectedIndex = 0;
                    }
                }
                else if (textLine[c].Contains(subNetName[2]))
                {
                    // Extract the Serial Number
                    int strIndx = textLine[c].LastIndexOf('N') + 2;
                    int endIndx = textLine[c].IndexOf('@') - 1;
                    string sn = textLine[c].Substring(strIndx, endIndx - strIndx);

                    bool hwDevHit = false;
                    int loopCntr = 0;

                    // Check if the sn contained in the ConfigFile is currently one of the CAN interface devices connected to the PC
                    while (loopCntr < devCnt)
                    {
                        if (sn.Contains(serialNumOfDevice[loopCntr]))
                        {
                            chnTwoSN_comboBox.Items.Add(sn);
                            chnTwoSN_comboBox.SelectedItem = sn;
                            chnTwoSN_comboBox.Enabled = true;

                            hwDevHit = true;
                        }
                        loopCntr++;
                    }

                    if (!hwDevHit)
                    {
                        // The Configuration File contains bad/stale information of the CAN HW connected                            
                        if (MessageBox.Show("The Config File has a stale SN for the CAN HW connected to ESN!\n\nDo you need to Initialize the CAN HW connected?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("Re-Launch the APP and Initialize the connected CAN HW.", "Check Information");

                            System.IO.File.Delete(dirName + "\\CANconfig.txt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("This subnet entry will simply be erased from the CANconfig.", "Check Information");
                    }
                    else
                    {
                        if (textLine[c].Contains("(Channel 0)"))
                        {
                            chnTwo_comboBox.Items.Add("1");
                            chnTwo_comboBox.SelectedItem = "1";
                        }
                        else if (textLine[c].Contains("(Channel 1)"))
                        {
                            chnTwo_comboBox.Items.Add("2");
                            chnTwo_comboBox.SelectedItem = "2";
                        }
                        else if (textLine[c].Contains("(Channel 2)"))
                        {
                            chnTwo_comboBox.Items.Add("3");
                            chnTwo_comboBox.SelectedItem = "3";
                        }
                        else if (textLine[c].Contains("(Channel 3)"))
                        {
                            chnTwo_comboBox.Items.Add("4");
                            chnTwo_comboBox.SelectedItem = "4";
                        }
                        else if (textLine[c].Contains("(Channel 4)"))
                        {
                            chnTwo_comboBox.Items.Add("5");
                            chnTwo_comboBox.SelectedItem = "5";
                        }
                    }
                }
                else if (textLine[c].Contains(subNetName[3]))
                {
                    // Extract the Serial Number
                    int strIndx = textLine[c].LastIndexOf('N') + 2;
                    int endIndx = textLine[c].IndexOf('@') - 1;
                    string sn = textLine[c].Substring(strIndx, endIndx - strIndx);

                    bool hwDevHit = false;
                    int loopCntr = 0;

                    // Check if the sn contained in the ConfigFile is currently one of the CAN interface devices connected to the PC
                    while (loopCntr < devCnt)
                    {
                        if (sn.Contains(serialNumOfDevice[loopCntr]))
                        {
                            chnThreeSN_comboBox.Items.Add(sn);
                            chnThreeSN_comboBox.SelectedItem = sn;
                            chnThreeSN_comboBox.Enabled = true;

                            hwDevHit = true;
                        }
                        loopCntr++;
                    }

                    if (!hwDevHit)
                    {
                        // The Configuration File contains bad/stale information of the CAN HW connected                            
                        if (MessageBox.Show("The Config File has a stale SN for the CAN HW connected to BB2!\n\nDo you need to Initialize the CAN HW connected?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("Re-Launch the APP and Initialize the connected CAN HW.", "Check Information");

                            System.IO.File.Delete(dirName + "\\CANconfig.txt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("This subnet entry will simply be erased from the CANconfig.", "Check Information");
                    }
                    else
                    {
                        if (textLine[c].Contains("(Channel 0)"))
                        {
                            chnThree_comboBox.Items.Add("1");
                            chnThree_comboBox.SelectedItem = "1";
                        }
                        else if (textLine[c].Contains("(Channel 1)"))
                        {
                            chnThree_comboBox.Items.Add("2");
                            chnThree_comboBox.SelectedItem = "2";
                        }
                        else if (textLine[c].Contains("(Channel 2)"))
                        {
                            chnThree_comboBox.Items.Add("3");
                            chnThree_comboBox.SelectedItem = "3";
                        }
                        else if (textLine[c].Contains("(Channel 3)"))
                        {
                            chnThree_comboBox.Items.Add("4");
                            chnThree_comboBox.SelectedItem = "4";
                        }
                        else if (textLine[c].Contains("(Channel 4)"))
                        {
                            chnThree_comboBox.Items.Add("5");
                            chnThree_comboBox.SelectedItem = "5";
                        }
                    }
                }
                else if (textLine[c].Contains(subNetName[4]))
                {
                    // Extract the Serial Number
                    int strIndx = textLine[c].LastIndexOf('N') + 2;
                    int endIndx = textLine[c].IndexOf('@') - 1;
                    string sn = textLine[c].Substring(strIndx, endIndx - strIndx);

                    bool hwDevHit = false;
                    int loopCntr = 0;

                    // Check if the sn contained in the ConfigFile is currently one of the CAN interface devices connected to the PC
                    while (loopCntr < devCnt)
                    {
                        if (sn.Contains(serialNumOfDevice[loopCntr]))
                        {
                            chnFourSN_comboBox.Items.Add(sn);
                            chnFourSN_comboBox.SelectedItem = sn;
                            chnFourSN_comboBox.Enabled = true;
                            hwDevHit = true;
                        }
                        loopCntr++;
                    }

                    if (!hwDevHit)
                    {
                        // The Configuration File contains bad/stale information of the CAN HW connected                            
                        if (MessageBox.Show("The Config File has a stale SN for the CAN HW connected to PWT!\n\nDo you need to Initialize the CAN HW connected?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("Re-Launch the APP and Initialize the connected CAN HW.", "Check Information");

                            System.IO.File.Delete(dirName + "\\CANconfig.txt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("This subnet entry will simply be erased from the CANconfig.", "Check Information");
                    }
                    else
                    {
                        if (textLine[c].Contains("(Channel 0)"))
                        {
                            chnFour_comboBox.Items.Add("1");
                            chnFour_comboBox.SelectedItem = "1";
                        }
                        else if (textLine[c].Contains("(Channel 1)"))
                        {
                            chnFour_comboBox.Items.Add("2");
                            chnFour_comboBox.SelectedItem = "2";
                        }
                        else if (textLine[c].Contains("(Channel 2)"))
                        {
                            chnFour_comboBox.Items.Add("3");
                            chnFour_comboBox.SelectedItem = "3";
                        }
                        else if (textLine[c].Contains("(Channel 3)"))
                        {
                            chnFour_comboBox.Items.Add("4");
                            chnFour_comboBox.SelectedItem = "4";
                        }
                        else if (textLine[c].Contains("(Channel 4)"))
                        {
                            chnFour_comboBox.Items.Add("5");
                            chnFour_comboBox.SelectedItem = "5";
                        }
                    }
                }
                else if (textLine[c].Contains(subNetName[5]))
                {
                    // Extract the Serial Number
                    int strIndx = textLine[c].LastIndexOf('N') + 2;
                    int endIndx = textLine[c].IndexOf('@') - 1;
                    string sn = textLine[c].Substring(strIndx, endIndx - strIndx);

                    bool hwDevHit = false;
                    int loopCntr = 0;

                    // Check if the sn contained in the ConfigFile is currently one of the CAN interface devices connected to the PC
                    while (loopCntr < devCnt)
                    {
                        if (sn.Contains(serialNumOfDevice[loopCntr]))
                        {
                            chnFiveSN_comboBox.Items.Add(sn);
                            chnFiveSN_comboBox.SelectedItem = sn;
                            chnFiveSN_comboBox.Enabled = true;
                            hwDevHit = true;
                        }
                        loopCntr++;
                    }

                    if (!hwDevHit)
                    {
                        // The Configuration File contains bad/stale information of the CAN HW connected                            
                        if (MessageBox.Show("The Config File has a stale SN for the CAN HW connected to ISN!\n\nDo you need to Initialize the CAN HW connected?", "CAN Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            MessageBox.Show("Re-Launch the APP and Initialize the connected CAN HW.", "Check Information");

                            System.IO.File.Delete(dirName + "\\CANconfig.txt");
                            Process.GetCurrentProcess().Kill();
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("This subnet entry will simply be erased from the CANconfig.", "Check Information");
                    }
                    else
                    {
                        if (textLine[c].Contains("(Channel 0)"))
                        {
                            chnFive_comboBox.Items.Add("1");
                            chnFive_comboBox.SelectedItem = "1";
                        }
                        else if (textLine[c].Contains("(Channel 1)"))
                        {
                            chnFive_comboBox.Items.Add("2");
                            chnFive_comboBox.SelectedItem = "2";
                        }
                        else if (textLine[c].Contains("(Channel 2)"))
                        {
                            chnFive_comboBox.Items.Add("3");
                            chnFive_comboBox.SelectedItem = "3";
                        }
                        else if (textLine[c].Contains("(Channel 3)"))
                        {
                            chnFive_comboBox.Items.Add("4");
                            chnFive_comboBox.SelectedItem = "4";
                        }
                        else if (textLine[c].Contains("(Channel 4)"))
                        {
                            chnFive_comboBox.Items.Add("5");
                            chnFive_comboBox.SelectedItem = "5";
                        }
                    }
                }
            }
            // Automate the CAN Configuration click
            applyCANconfigBtn_Click(null, null);
            applyCANconfigBtn.Enabled = false;
        }

        #endregion

        #region "Initialize and Apply CAN Config"
        /// <summary>
        /// ******************** INIT CAN HW **************************************
        /// Interrogate the CAN Devices connected to the PC and populate the CAN comboBoxes with their SNs and ChnCnt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CAN_Init_btn_Click(object sender, EventArgs e)
        {
            chnCnt = 0;
            devCnt = 0;
            devSN_pv = "0";
            chnCntPerDev = 0;

            CAN_HW_tB.Clear();

            // Poll HW to determine the number of channels. Since Virtual Channels are included I added a test for that.
            can_status = Canlib.canGetNumberOfChannels(out channelCount);

            if (can_status == Canlib.canStatus.canOK)
            {
                if (channelCount <= 2)
                {
                    //GET info about the existing CAN HW CHANNEL connected to the PC
                    can_status = Canlib.canGetChannelData(chnCnt, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);

                    // If the first channel is a virtual Kvaser Device Ignore it and abort after informing the user
                    if ((CANinfo.ToString().Contains("Virtual")))
                    {
                        MessageBox.Show("No CAN Interface HW found! ", "KVASER STATUS BOX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.Exit();
                    }
                }

                directionLbl.Visible = true;    //"Use the combo boxes to configure the CAN subnet.                    

                while (chnCnt < channelCount)
                {
                    // GET info about the existing CAN HW CHANNEL connected to the PC
                    can_status = Canlib.canGetChannelData(chnCnt, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);

                    // Ignore virtual Kvaser Devices/channels at the end of the while loop
                    if (!CANinfo.ToString().Contains("Virtual"))
                    {
                        can_status = Canlib.canGetChannelData(chnCnt, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                        if (can_status == Canlib.canStatus.canOK)
                        {
                            // Check if the Device has been encountered previously
                            if (!devSN_pv.Contains(devSN.ToString()))
                            {
                                devSN_pv = devSN.ToString();                    // "Register" the devices serial number

                                serialNumOfDevice[devCnt] = devSN_pv;   //  devSN.ToString();   // Save the new serial number

                                chnCntPerDev = 1;       // This is a newly discovered device start counting the channels associated with it

                                devCnt++;               // Count the devices as well
                            }
                            else
                            {
                                chnCntPerDev++;         // Add to the channel count per Device
                                channelCntOfDevice[devCnt - 1] = chnCntPerDev;  //Save the channel count of the last device
                            }

                            CAN_HW_tB.Text += CANinfo.ToString();

                            CAN_HW_tB.Text += " SN " + devSN.ToString() + "\r\n";

                            // Check BB1sn_combo to see if this is a new serial number. Add to the list for MCDsn_combo
                            if (!chnOneSN_comboBox.Items.Contains(devSN.ToString()))
                                chnOneSN_comboBox.Items.Add(devSN.ToString());                                 //Cmd to ADD the devSN to the list
                            if (!chnTwoSN_comboBox.Items.Contains(devSN.ToString()))
                                chnTwoSN_comboBox.Items.Add(devSN.ToString());
                            if (!chnThreeSN_comboBox.Items.Contains(devSN.ToString()))
                                chnThreeSN_comboBox.Items.Add(devSN.ToString());
                            if (!chnFourSN_comboBox.Items.Contains(devSN.ToString()))
                                chnFourSN_comboBox.Items.Add(devSN.ToString());
                            if (!chnFiveSN_comboBox.Items.Contains(devSN.ToString()))
                                chnFiveSN_comboBox.Items.Add(devSN.ToString());
                        }
                    }
                    else
                    {
                        // If the first channel is a virtual Kvaser Device Ignore it and abort after informing the user
                        if (chnCnt == 0)
                        {
                            MessageBox.Show("Error! CAN Interface HW missing! Check connections!\n" + CANinfo.ToString(), "KVASER STATUS BOX");
                            Application.Exit();
                        }
                    }
                    chnCnt++;   //Total number of actual channels
                }
                // Null out the unused devices serial numbers
                for (int n = devCnt; n < 5; n++)
                {
                    serialNumOfDevice[n] = null;
                }

                // If there is only one Device apply the serial number to all the serial number combo boxes
                // If the Device is only Two channels, map the active channels by assigning the SN and selecting the channel.
                if ( devCnt == 1)
                {      
                    if (chnCntPerDev == 4) 
                    {
					    chnOneSN_comboBox.SelectedIndex = 0;
                        chnThreeSN_comboBox.SelectedIndex = 0;
                        chnTwoSN_comboBox.SelectedIndex = 0;
                        chnFourSN_comboBox.SelectedIndex = 0;
					}					
                    else if (chnCntPerDev == 5) chnFiveSN_comboBox.SelectedIndex = 0;
                }      
            }
            else
            {
                MessageBox.Show("Error with the Kvaser CAN API!\n" + CANinfo.ToString(), "KVASER STATUS BOX");
                Application.Exit();
            }
        }

        /// <summary>
        /// Set/Re-set the default values for various items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refresh_Btn_Click(object sender, EventArgs e)
        {
            applyCANconfigBtn.Text = "Apply CAN Configuration";
            applyCANconfigBtn.Enabled = false;

            CAN1chanSelc = false;
            CAN2chanSelc = false;
            CAN3chanSelc = false;
            CAN4chanSelc = false;
            CAN5chanSelc = false;

            chnTwo_comboBox.Items.Clear();
            chnTwoSN_comboBox.Items.Clear();
            chnFour_comboBox.Items.Clear();
            chnFourSN_comboBox.Items.Clear();
            chnThree_comboBox.Items.Clear();
            chnThreeSN_comboBox.Items.Clear();
            chnOne_comboBox.Items.Clear();
            chnOneSN_comboBox.Items.Clear();
            chnFive_comboBox.Items.Clear();
            chnFiveSN_comboBox.Items.Clear();

            chnTwo_comboBox.Text = null;
            chnTwoSN_comboBox.Text = null;
            chnFour_comboBox.Text = null;
            chnFourSN_comboBox.Text = null;
            chnThree_comboBox.Text = null;
            chnThreeSN_comboBox.Text = null;
            chnOne_comboBox.Text = null;
            chnOneSN_comboBox.Text = null;
            chnOneBR_comboBox.Text = null;
            chnFive_comboBox.Text = null;
            chnFiveSN_comboBox.Text = null;

            chnTwo_comboBox.Enabled = false;
            chnTwoSN_comboBox.Enabled = true;
            chnFour_comboBox.Enabled = false;
            chnFourSN_comboBox.Enabled = true;
            chnThree_comboBox.Enabled = false;
            chnThreeSN_comboBox.Enabled = true;
            chnOne_comboBox.Enabled = false;
            chnOneBR_comboBox.Enabled = false;
            chnOneSN_comboBox.Enabled = true;
            chnFive_comboBox.Enabled = false;
            chnFiveSN_comboBox.Enabled = true;

            // Assign a "DEFAULT" Baudrate to the subnets.
            chnOneBR_comboBox.SelectedIndex = 0;
            chnThreeBR_comboBox.SelectedIndex = 1;
            chnTwoBR_comboBox.SelectedIndex = 0;
            chnFourBR_comboBox.SelectedIndex = 1;
            chnFiveBR_comboBox.SelectedIndex = 0;

            CAN_HW_tB.Clear();

            // Automate the CAN initialization click
            CAN_Init_btn_Click(null, null);
        }
        /// <summary>
        /// This timer prompts the user to enter the CAN subnet Name
        /// It changes the titles to the combo boxes for configuring the subnets
        /// And save the names when writing to the CANconfig file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (subNetCnt)
            {
                case 0:
                    if (!EnterCAN1X)
                    {
                        EnterCAN1X = true;
                        if (typeSubnetName.ShowDialog() == DialogResult.OK)
                        {
                            CAN1Lbl.Text = CAN1Lbl.Text.Replace("CAN1", typeSubnetName.CANsubnetID);
                            CAN1chnLbl.Text = CAN1chnLbl.Text.Replace("CAN1", typeSubnetName.CANsubnetID);
                            CAN1BdLbl.Text = CAN1BdLbl.Text.Replace("CAN1", typeSubnetName.CANsubnetID);
                            subNetName[subNetCnt] = typeSubnetName.CANsubnetID;
                        }
                        else //if (typeSubnetName.ShowDialog() == DialogResult.Cancel)
                        {
                            CAN1Lbl.Visible = false;
                            CAN1chnLbl.Visible = false;
                            CAN1BdLbl.Visible = false;
                            chnOneBR_comboBox.Visible = false;
                            chnOne_comboBox.Visible = false;
                            chnOneSN_comboBox.Visible = false;
                        }
                    }
                    break;
                case 1:
                    if (!EnterCAN2X)
                    {
                        EnterCAN2X = true;
                        if (typeSubnetName.ShowDialog() == DialogResult.OK)
                        {
                            CAN2Lbl.Text = CAN2Lbl.Text.Replace("CAN2", typeSubnetName.CANsubnetID);
                            CAN2chnLbl.Text = CAN2chnLbl.Text.Replace("CAN2", typeSubnetName.CANsubnetID);
                            CAN2BdLbl.Text = CAN2BdLbl.Text.Replace("CAN2", typeSubnetName.CANsubnetID);
                            subNetName[subNetCnt] = typeSubnetName.CANsubnetID;
                        }
                        else //if (typeSubnetName.ShowDialog() == DialogResult.Cancel)
                        {
                            CAN2Lbl.Visible = false;
                            CAN2chnLbl.Visible = false;
                            CAN2BdLbl.Visible = false;
                            chnTwoBR_comboBox.Visible = false;
                            chnTwo_comboBox.Visible = false;
                            chnTwoSN_comboBox.Visible = false;
                        }
                    }
                    break;
                case 2:
                    if (!EnterCAN3X)
                    {
                        EnterCAN3X = true;
                        if (typeSubnetName.ShowDialog() == DialogResult.OK)
                        {
                            CAN3Lbl.Text = CAN3Lbl.Text.Replace("CAN3", typeSubnetName.CANsubnetID);
                            CAN3chnLbl.Text = CAN3chnLbl.Text.Replace("CAN3", typeSubnetName.CANsubnetID);
                            CAN3BdLbl.Text = CAN3BdLbl.Text.Replace("CAN3", typeSubnetName.CANsubnetID);
                            subNetName[subNetCnt] = typeSubnetName.CANsubnetID;
                        }
                        else //if (typeSubnetName.ShowDialog() == DialogResult.Cancel)
                        {
                            CAN3Lbl.Visible = false;
                            CAN3chnLbl.Visible = false;
                            CAN3BdLbl.Visible = false;
                            chnThreeBR_comboBox.Visible = false;
                            chnThree_comboBox.Visible = false;
                            chnThreeSN_comboBox.Visible = false;
                        }
                    }
                    break;
                case 3:
                    if (!EnterCAN4X)
                    {
                        EnterCAN4X = true;
                        if (typeSubnetName.ShowDialog() == DialogResult.OK)
                        {
                            CAN4Lbl.Text = CAN4Lbl.Text.Replace("CAN4", typeSubnetName.CANsubnetID);
                            CAN4chnLbl.Text = CAN4chnLbl.Text.Replace("CAN4", typeSubnetName.CANsubnetID);
                            CAN4BdLbl.Text = CAN4BdLbl.Text.Replace("CAN4", typeSubnetName.CANsubnetID);
                            subNetName[subNetCnt] = typeSubnetName.CANsubnetID;
                        }
                        else //if (typeSubnetName.ShowDialog() == DialogResult.Cancel)
                        {
                            CAN4Lbl.Visible = false;
                            CAN4chnLbl.Visible = false;
                            CAN4BdLbl.Visible = false;
                            chnFourBR_comboBox.Visible = false;
                            chnFour_comboBox.Visible = false;
                            chnFourSN_comboBox.Visible = false;
                        }
                    }
                    break;
                case 4:
                    if (!EnterCAN5X)
                    {
                        EnterCAN5X = true;
                        if (typeSubnetName.ShowDialog() == DialogResult.OK)
                        {
                            CAN5Lbl.Text = CAN5Lbl.Text.Replace("CAN5", typeSubnetName.CANsubnetID);
                            CAN5chnLbl.Text = CAN5chnLbl.Text.Replace("CAN5", typeSubnetName.CANsubnetID);
                            CAN5BdLbl.Text = CAN5BdLbl.Text.Replace("CAN5", typeSubnetName.CANsubnetID);
                            subNetName[subNetCnt] = typeSubnetName.CANsubnetID;
                        }
                        else //if (typeSubnetName.ShowDialog() == DialogResult.Cancel)
                        {
                            CAN5Lbl.Visible = false;
                            CAN5chnLbl.Visible = false;
                            CAN5BdLbl.Visible = false;
                            chnFiveBR_comboBox.Visible = false;
                            chnFive_comboBox.Visible = false;
                            chnFiveSN_comboBox.Visible = false;
                        }
                    }
                    break;
                case 5:
                    // Automate the CAN initialization click
                    subNetCnt = 0;
                    CAN_Init_btn_Click(null, null);

                    break;
            }
        }
        #endregion

        #region "Apply CAN Configuration"
        /// <summary>
        /// Take the values from the CANcofig comboBoxes and applies handles and the settings to the channels and turn on the CAN lines.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyCANconfigBtn_Click(object sender, EventArgs e)
        {
            CAN_HW_tB.Clear();

            if (CAN1chanSelc)
            {
                //You must pass a channel number and not a channel handle.
                can_status = Canlib.canGetChannelData(CAN1hnd, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);
                can_status = Canlib.canGetChannelData(CAN1hnd, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                if (can_status == Canlib.canStatus.canOK)
                {
                    CAN1hnd = Canlib.canOpenChannel(CAN1hnd, Canlib.canOPEN_OVERRIDE_EXCLUSIVE);

                    can_status = Canlib.canSetBusOutputControl(CAN1hnd, Canlib.canDRIVER_NORMAL);

                    // Check the Bitrate for BB1 [TEA2+ (250kBd) or TEA2+T2 (500kBd)]
                    if (chnOneBR_comboBox.SelectedIndex == 0)
                    {
                        can_status = Canlib.canSetBusParams(CAN1hnd, Canlib.canBITRATE_250K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 250KBd\r\n";

                        CAN1chanSelc = false;
                    }
                    else
                    {
                        can_status = Canlib.canSetBusParams(CAN1hnd, Canlib.canBITRATE_500K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 500KBd\r\n";

                        CAN1chanSelc = false;
                    }
                }

                if (can_status != Canlib.canStatus.canOK)
                {
                    chnCnt = channelCount;
                    CAN1chanSelc = false;
                    CAN2chanSelc = false;
                    CAN3chanSelc = false;
                    CAN4chanSelc = false;
                    CAN5chanSelc = false;
                    useCANconfigFile_s = false;

                    MessageBox.Show("Error Initializing CHANNEL 1 " + can_status.ToString(), "KVASER STATUS BOX");
                    Exit_Btn_Click(null, null);
                }
                else
                    chn1ConfigSuccess = true;
            }
            if (CAN2chanSelc)
            {
                can_status = Canlib.canGetChannelData(CAN2hnd, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);
                can_status = Canlib.canGetChannelData(CAN2hnd, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                if (can_status == Canlib.canStatus.canOK)
                {
                    CAN2hnd = Canlib.canOpenChannel(CAN2hnd, Canlib.canOPEN_OVERRIDE_EXCLUSIVE);
                    can_status = Canlib.canSetBusParams(CAN2hnd, Canlib.canBITRATE_250K, 0, 0, 0, 0);

                    if (can_status == Canlib.canStatus.canOK)
                        can_status = Canlib.canSetBusOutputControl(CAN2hnd, Canlib.canDRIVER_NORMAL);
                    else
                        MessageBox.Show("Error Initializing ESN " + can_status.ToString(), "KVASER STATUS BOX");

                    if (can_status == Canlib.canStatus.canOK)
                    {
                        CAN_HW_tB.Text += subNetName[2] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 250KBd\r\n";

                        CAN2chanSelc = false;
                    }
                    else
                    {
                        can_status = Canlib.canSetBusParams(CAN2hnd, Canlib.canBITRATE_500K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 500KBd\r\n";

                        CAN1chanSelc = false;
                    }
                }

                if (can_status != Canlib.canStatus.canOK)
                {
                    chnCnt = channelCount;
                    CAN1chanSelc = false;
                    CAN2chanSelc = false;
                    CAN3chanSelc = false;
                    CAN4chanSelc = false;
                    CAN5chanSelc = false;
                    useCANconfigFile_s = false;

                    MessageBox.Show("Error Initializing CHANNEL 2 " + can_status.ToString(), "KVASER STATUS BOX");
                    Exit_Btn_Click(null, null);
                }
                else
                    chn2ConfigSuccess = true;
            }
            if (CAN3chanSelc)
            {
                can_status = Canlib.canGetChannelData(CAN3hnd, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);
                can_status = Canlib.canGetChannelData(CAN3hnd, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                if (can_status == Canlib.canStatus.canOK)
                {
                    CAN3hnd = Canlib.canOpenChannel(CAN3hnd, Canlib.canOPEN_OVERRIDE_EXCLUSIVE);
                    can_status = Canlib.canSetBusParams(CAN3hnd, Canlib.canBITRATE_500K, 0, 0, 0, 0);

                    if (can_status == Canlib.canStatus.canOK)
                        can_status = Canlib.canSetBusOutputControl(CAN3hnd, Canlib.canDRIVER_NORMAL);
                    else
                        MessageBox.Show("Error Initializing BB2 " + can_status.ToString(), "KVASER STATUS BOX");

                    if (can_status == Canlib.canStatus.canOK)
                    {
                        CAN_HW_tB.Text += subNetName[3] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 500KBd\r\n";

                        CAN3chanSelc = false;
                    }
                    else
                    {
                        can_status = Canlib.canSetBusParams(CAN3hnd, Canlib.canBITRATE_250K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 250KBd\r\n";

                        CAN1chanSelc = false;
                    }
                }

                if (can_status != Canlib.canStatus.canOK)
                {
                    chnCnt = channelCount;
                    CAN1chanSelc = false;
                    CAN2chanSelc = false;
                    CAN3chanSelc = false;
                    CAN4chanSelc = false;
                    CAN5chanSelc = false;
                    useCANconfigFile_s = false;

                    MessageBox.Show("Error Initializing CHANNEL 3 " + can_status.ToString(), "KVASER STATUS BOX");
                    Exit_Btn_Click(null, null);
                }
                else
                    chn3ConfigSuccess = true;
            }
            
            if (CAN4chanSelc)
            {
                can_status = Canlib.canGetChannelData(CAN4hnd, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);
                can_status = Canlib.canGetChannelData(CAN4hnd, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                if (can_status == Canlib.canStatus.canOK)
                {
                    CAN4hnd = Canlib.canOpenChannel(CAN4hnd, Canlib.canOPEN_OVERRIDE_EXCLUSIVE);
                    can_status = Canlib.canSetBusParams(CAN4hnd, Canlib.canBITRATE_500K, 0, 0, 0, 0);

                    if (can_status == Canlib.canStatus.canOK)
                        can_status = Canlib.canSetBusOutputControl(CAN4hnd, Canlib.canDRIVER_NORMAL);
                    else
                        MessageBox.Show("Error Initializing PWT " + can_status.ToString(), "KVASER STATUS BOX");

                    if (can_status == Canlib.canStatus.canOK)
                    {
                        CAN_HW_tB.Text += subNetName[4] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 500KBd\r\n";

                        CAN4chanSelc = false;
                    }
                    else
                    {
                        can_status = Canlib.canSetBusParams(CAN4hnd, Canlib.canBITRATE_250K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 250KBd\r\n";

                        CAN1chanSelc = false;
                    }
                }

                if (can_status != Canlib.canStatus.canOK)
                {
                    chnCnt = channelCount;
                    CAN1chanSelc = false;
                    CAN2chanSelc = false;
                    CAN3chanSelc = false;
                    CAN4chanSelc = false;
                    CAN5chanSelc = false;
                    useCANconfigFile_s = false;

                    MessageBox.Show("Error Initializing CHANNEL 4 " + can_status.ToString(), "KVASER STATUS BOX");
                    Exit_Btn_Click(null, null);
                }
                else
                    chn4ConfigSuccess = true;
            }
            if (CAN5chanSelc)
            {
                can_status = Canlib.canGetChannelData(CAN5hnd, Canlib.canCHANNELDATA_CHANNEL_NAME, out CANinfo);
                can_status = Canlib.canGetChannelData(CAN5hnd, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out devSN);

                if (can_status == Canlib.canStatus.canOK)
                {
                    CAN5hnd = Canlib.canOpenChannel(CAN5hnd, Canlib.canOPEN_OVERRIDE_EXCLUSIVE);
                    can_status = Canlib.canSetBusParams(CAN5hnd, Canlib.canBITRATE_250K, 0, 0, 0, 0);

                    if (can_status == Canlib.canStatus.canOK)
                        can_status = Canlib.canSetBusOutputControl(CAN5hnd, Canlib.canDRIVER_NORMAL);
                    else
                        MessageBox.Show("Error Initializing ISN " + can_status.ToString(), "KVASER STATUS BOX");

                    if (can_status == Canlib.canStatus.canOK)
                    {
                        CAN_HW_tB.Text += subNetName[5] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 250KBd\r\n";

                        CAN5chanSelc = false;
                    }
                    else
                    {
                        can_status = Canlib.canSetBusParams(CAN5hnd, Canlib.canBITRATE_500K, 0, 0, 0, 0);

                        CAN_HW_tB.Text += subNetName[1] + " = " + CANinfo.ToString();

                        CAN_HW_tB.Text += " SN " + devSN.ToString() + " @ 500KBd\r\n";

                        CAN1chanSelc = false;
                    }
                }

                if (can_status != Canlib.canStatus.canOK)
                {
                    chnCnt = channelCount;
                    CAN1chanSelc = false;
                    CAN2chanSelc = false;
                    CAN3chanSelc = false;
                    CAN4chanSelc = false;
                    CAN5chanSelc = false;
                    useCANconfigFile_s = false;

                    MessageBox.Show("Error Initializing CHANNEL 3 " + can_status.ToString(), "KVASER STATUS BOX");
                    Exit_Btn_Click(null, null);
                }
                else
                    chn5ConfigSuccess = true;
            }

            //Keep the text lines short and consistent.
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Kvaser ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Professional ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("Light ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace(" v2 ", "");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("HS/H", "2xHS");
            CAN_HW_tB.Text = CAN_HW_tB.Text.Replace("#", "Dev ");

            applyCANconfigBtn.Enabled = false;
            applyCANconfigBtn.Text = "Applied CAN Configuration";

            if (useCANconfigFile_s)
            {
                // Automate the OK button press
                OK_Btn_Click(null, null);
            }
        }
        #endregion


        #region "ComboBox region of CAN Config"
        private void chnOneSN_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CAN1chanSelc = false;
            chnOne_comboBox.Enabled = true;
            applyCANconfigBtn.Enabled = true;
            chnOne_comboBox.Items.Clear();

            foreach (string var in serialNumOfDevice)
            {
                if (chnOneSN_comboBox.SelectedItem.ToString() == var)
                {
                    if (channelCntOfDevice[chnOneSN_comboBox.SelectedIndex] == 1)
                    {
                        chnOne_comboBox.Items.Add("1");
                    }
                    else if (channelCntOfDevice[chnOneSN_comboBox.SelectedIndex] == 2)
                    {
                        chnOne_comboBox.Items.Add("1");
                        chnOne_comboBox.Items.Add("2");
                    }
                    else if (channelCntOfDevice[chnOneSN_comboBox.SelectedIndex] == 4)
                    {
                        chnOne_comboBox.Items.Add("1");
                        chnOne_comboBox.Items.Add("2");
                        chnOne_comboBox.Items.Add("3");
                        chnOne_comboBox.Items.Add("4");
                    }
                    else if (channelCntOfDevice[chnOneSN_comboBox.SelectedIndex] == 5)
                    {
                        chnOne_comboBox.Items.Add("1");
                        chnOne_comboBox.Items.Add("2");
                        chnOne_comboBox.Items.Add("3");
                        chnOne_comboBox.Items.Add("4");
                        chnOne_comboBox.Items.Add("5");
                    }
                }
            }
            // ENABLE IT FOR bit rate selection
            chnOneBR_comboBox.Enabled = true;
        }

        private void chnThreeSN_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CAN3chanSelc = false;
            chnThree_comboBox.Enabled = true;
            applyCANconfigBtn.Enabled = true;
            chnThree_comboBox.Items.Clear();

            foreach (string var in serialNumOfDevice)
            {
                if (chnThreeSN_comboBox.SelectedItem.ToString() == var)
                {
                    if (channelCntOfDevice[chnThreeSN_comboBox.SelectedIndex] == 1)
                    {
                        chnThree_comboBox.Items.Add("1");
                    }
                    else if (channelCntOfDevice[chnThreeSN_comboBox.SelectedIndex] == 2)
                    {
                        chnThree_comboBox.Items.Add("1");
                        chnThree_comboBox.Items.Add("2");
                    }
                    else if (channelCntOfDevice[chnThreeSN_comboBox.SelectedIndex] == 4)
                    {
                        chnThree_comboBox.Items.Add("1");
                        chnThree_comboBox.Items.Add("2");
                        chnThree_comboBox.Items.Add("3");
                        chnThree_comboBox.Items.Add("4");
                    }
                    else if (channelCntOfDevice[chnThreeSN_comboBox.SelectedIndex] == 5)
                    {
                        chnThree_comboBox.Items.Add("1");
                        chnThree_comboBox.Items.Add("2");
                        chnThree_comboBox.Items.Add("3");
                        chnThree_comboBox.Items.Add("4");
                        chnThree_comboBox.Items.Add("5");
                    }
                }
            }
            // ENABLE IT FOR bit rate selection
            chnThreeBR_comboBox.Enabled = true;
        }

        private void chnTwoSN_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CAN2chanSelc = false;
            chnTwo_comboBox.Enabled = true;
            applyCANconfigBtn.Enabled = true;
            chnTwo_comboBox.Items.Clear();

            foreach (string var in serialNumOfDevice)
            {
                if (chnTwoSN_comboBox.SelectedItem.ToString() == var)
                {
                    if (channelCntOfDevice[chnTwoSN_comboBox.SelectedIndex] == 1)
                    {
                        chnTwo_comboBox.Items.Add("1");
                    }
                    else if (channelCntOfDevice[chnTwoSN_comboBox.SelectedIndex] == 2)
                    {
                        chnTwo_comboBox.Items.Add("1");
                        chnTwo_comboBox.Items.Add("2");
                    }
                    else if (channelCntOfDevice[chnTwoSN_comboBox.SelectedIndex] == 4)
                    {
                        chnTwo_comboBox.Items.Add("1");
                        chnTwo_comboBox.Items.Add("2");
                        chnTwo_comboBox.Items.Add("3");
                        chnTwo_comboBox.Items.Add("4");
                    }
                    else if (channelCntOfDevice[chnTwoSN_comboBox.SelectedIndex] == 5)
                    {
                        chnTwo_comboBox.Items.Add("1");
                        chnTwo_comboBox.Items.Add("2");
                        chnTwo_comboBox.Items.Add("3");
                        chnTwo_comboBox.Items.Add("4");
                        chnTwo_comboBox.Items.Add("5");
                    }
                }
            }
            // ENABLE IT FOR bit rate selection
            chnTwoBR_comboBox.Enabled = true;
        }

        private void chnFourSN_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CAN4chanSelc = false;
            chnFour_comboBox.Enabled = true;
            applyCANconfigBtn.Enabled = true;
            chnFour_comboBox.Items.Clear();

            foreach (string var in serialNumOfDevice)
            {
                if (chnFourSN_comboBox.SelectedItem.ToString() == var)
                {
                    if (channelCntOfDevice[chnFourSN_comboBox.SelectedIndex] == 1)
                    {
                        chnFour_comboBox.Items.Add("1");
                    }
                    else if (channelCntOfDevice[chnFourSN_comboBox.SelectedIndex] == 2)
                    {
                        chnFour_comboBox.Items.Add("1");
                        chnFour_comboBox.Items.Add("2");
                    }
                    else if (channelCntOfDevice[chnFourSN_comboBox.SelectedIndex] == 4)
                    {
                        chnFour_comboBox.Items.Add("1");
                        chnFour_comboBox.Items.Add("2");
                        chnFour_comboBox.Items.Add("3");
                        chnFour_comboBox.Items.Add("4");
                    }
                    else if (channelCntOfDevice[chnFourSN_comboBox.SelectedIndex] == 5)
                    {
                        chnFour_comboBox.Items.Add("1");
                        chnFour_comboBox.Items.Add("2");
                        chnFour_comboBox.Items.Add("3");
                        chnFour_comboBox.Items.Add("4");
                        chnFour_comboBox.Items.Add("5");
                    }
                }
            }
            // ENABLE IT FOR bit rate selection
            chnFourBR_comboBox.Enabled = true;
        }

        private void chnFiveSN_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CAN5chanSelc = false;
            chnFive_comboBox.Enabled = true;
            applyCANconfigBtn.Enabled = true;
            chnFive_comboBox.Items.Clear();

            foreach (string var in serialNumOfDevice)
            {
                if (chnFiveSN_comboBox.SelectedItem.ToString() == var)
                {
                    if (channelCntOfDevice[chnFiveSN_comboBox.SelectedIndex] == 1)
                    {
                        chnFive_comboBox.Items.Add("1");
                    }
                    else if (channelCntOfDevice[chnFiveSN_comboBox.SelectedIndex] == 2)
                    {
                        chnFive_comboBox.Items.Add("1");
                        chnFive_comboBox.Items.Add("2");
                    }
                    else if (channelCntOfDevice[chnFiveSN_comboBox.SelectedIndex] == 4)
                    {
                        chnFive_comboBox.Items.Add("1");
                        chnFive_comboBox.Items.Add("2");
                        chnFive_comboBox.Items.Add("3");
                        chnFive_comboBox.Items.Add("4");
                    }
                    else if (channelCntOfDevice[chnFiveSN_comboBox.SelectedIndex] == 5)
                    {
                        chnFive_comboBox.Items.Add("1");
                        chnFive_comboBox.Items.Add("2");
                        chnFive_comboBox.Items.Add("3");
                        chnFive_comboBox.Items.Add("4");
                        chnFive_comboBox.Items.Add("5");
                    }
                }
            }
            // ENABLE IT FOR bit rate selection
            chnFiveBR_comboBox.Enabled = true;
        }

        private void chnOne_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Since the SN comboBox enables the CHANNEL comboBox, the flag used by the Apply Config method to indicate the CAN channel is necessary for the MAIN APPLICATION
            // is simply a check on the Text in the channel comboBox. I added an over-check in case the saved configuration had the channel but it is no longer connected.
            if (chnOne_comboBox.Text.Length != 0)
            {
                //Match the Channel count from the init with the ComboBox selection that the user made
                if (serialNumOfDevice[0] != null)
                {
                    if (chnOneSN_comboBox.GetItemText(chnOneSN_comboBox.SelectedItem).Contains(serialNumOfDevice[0]))
                    {
                        CAN1hnd = Convert.ToInt16(chnOne_comboBox.SelectedItem) - 1;
                        CAN1chanSelc = true;
                    }
                }
                if (serialNumOfDevice[1] != null)
                {
                    if (chnOneSN_comboBox.GetItemText(chnOneSN_comboBox.SelectedItem).Contains(serialNumOfDevice[1]))
                    {
                        CAN1hnd = Convert.ToInt16(chnOne_comboBox.SelectedItem) + channelCntOfDevice[0] - 1;
                        CAN1chanSelc = true;
                    }
                }
                if (serialNumOfDevice[2] != null)
                {
                    if (chnOneSN_comboBox.GetItemText(chnOneSN_comboBox.SelectedItem).Contains(serialNumOfDevice[2]))
                    {
                        CAN1hnd = Convert.ToInt16(chnOne_comboBox.SelectedItem) + channelCntOfDevice[0] + channelCntOfDevice[1] - 1;
                        CAN1chanSelc = true;
                    }
                }
            }
        }

        private void chnThree_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Since the SN comboBox enables the CHANNEL comboBox, the flag used by the Apply Config method to indicate the CAN channel is necessary for the MAIN APPLICATION
            // is simply a check on the Text in the channel comboBox. I added an over-check in case the saved configuration had the channel but it is no longer connected.
            if (chnThree_comboBox.Text.Length != 0)
            {
                //Match the Channel count from the init with the ComboBox selection that the user made
                if (serialNumOfDevice[0] != null)
                {
                    if (chnThreeSN_comboBox.GetItemText(chnThreeSN_comboBox.SelectedItem).Contains(serialNumOfDevice[0]))
                    {
                        CAN3hnd = Convert.ToInt16(chnThree_comboBox.SelectedItem) - 1;
                        CAN3chanSelc = true;
                    }
                }
                if (serialNumOfDevice[1] != null)
                {
                    if (chnThreeSN_comboBox.GetItemText(chnThreeSN_comboBox.SelectedItem).Contains(serialNumOfDevice[1]))
                    {
                        CAN3hnd = Convert.ToInt16(chnThree_comboBox.SelectedItem) + channelCntOfDevice[0] - 1;
                        CAN3chanSelc = true;
                    }
                }
                if (serialNumOfDevice[2] != null)
                {
                    if (chnThreeSN_comboBox.GetItemText(chnThreeSN_comboBox.SelectedItem).Contains(serialNumOfDevice[2]))
                    {
                        CAN3hnd = Convert.ToInt16(chnThree_comboBox.SelectedItem) + channelCntOfDevice[0] + channelCntOfDevice[1] - 1;
                        CAN3chanSelc = true;
                    }
                }
            }
        }

        private void chnTwo_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Since the SN comboBox enables the CHANNEL comboBox, the flag used by the Apply Config method to indicate the CAN channel is necessary for the MAIN APPLICATION
            // is simply a check on the Text in the channel comboBox. I added an over-check in case the saved configuration had the channel but it is no longer connected.
            if (chnTwo_comboBox.Text.Length != 0)
            {
                //Match the Channel count from the init with the ComboBox selection that the user made
                if (serialNumOfDevice[0] != null)
                {
                    if (chnTwoSN_comboBox.GetItemText(chnTwoSN_comboBox.SelectedItem).Contains(serialNumOfDevice[0]))
                    {
                        CAN2hnd = Convert.ToInt16(chnTwo_comboBox.SelectedItem) - 1;
                        CAN2chanSelc = true;
                    }
                }
                if (serialNumOfDevice[1] != null)
                {
                    if (chnTwoSN_comboBox.GetItemText(chnTwoSN_comboBox.SelectedItem).Contains(serialNumOfDevice[1]))
                    {
                        CAN2hnd = Convert.ToInt16(chnTwo_comboBox.SelectedItem) + channelCntOfDevice[0] - 1;
                        CAN2chanSelc = true;
                    }
                }
                if (serialNumOfDevice[2] != null)
                {
                    if (chnTwoSN_comboBox.GetItemText(chnTwoSN_comboBox.SelectedItem).Contains(serialNumOfDevice[2]))
                    {
                        CAN2hnd = Convert.ToInt16(chnTwo_comboBox.SelectedItem) + channelCntOfDevice[0] + channelCntOfDevice[1] - 1;
                        CAN2chanSelc = true;
                    }
                }
            }
        }

        private void chnFour_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Since the SN comboBox enables the CHANNEL comboBox, the flag used by the Apply Config method to indicate the CAN channel is necessary for the MAIN APPLICATION
            // is simply a check on the Text in the channel comboBox. I added an over-check in case the saved configuration had the channel but it is no longer connected.
            if (chnFour_comboBox.Text.Length != 0)
            {
                //Match the Channel count from the init with the ComboBox selection that the user made
                if (serialNumOfDevice[0] != null)
                {
                    if (chnFourSN_comboBox.GetItemText(chnFourSN_comboBox.SelectedItem).Contains(serialNumOfDevice[0]))
                    {
                        CAN4hnd = Convert.ToInt16(chnFour_comboBox.SelectedItem) - 1;
                        CAN4chanSelc = true;
                    }
                }
                if (serialNumOfDevice[1] != null)
                {
                    if (chnFourSN_comboBox.GetItemText(chnFourSN_comboBox.SelectedItem).Contains(serialNumOfDevice[1]))
                    {
                        CAN4hnd = Convert.ToInt16(chnFour_comboBox.SelectedItem) + channelCntOfDevice[0] - 1;
                        CAN4chanSelc = true;
                    }
                }
                if (serialNumOfDevice[2] != null)
                {
                    if (chnFourSN_comboBox.GetItemText(chnFourSN_comboBox.SelectedItem).Contains(serialNumOfDevice[2]))
                    {
                        CAN4hnd = Convert.ToInt16(chnFour_comboBox.SelectedItem) + channelCntOfDevice[0] + channelCntOfDevice[1] - 1;
                        CAN4chanSelc = true;
                    }
                }
            }
        }

        private void chnFive_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Since the SN comboBox enables the CHANNEL comboBox, the flag used by the Apply Config method to indicate the CAN channel is necessary for the MAIN APPLICATION
            // is simply a check on the Text in the channel comboBox. I added an over-check in case the saved configuration had the channel but it is no longer connected.
            if (chnFive_comboBox.Text.Length != 0)
            {
                //Match the Channel count from the init with the ComboBox selection that the user made
                if (serialNumOfDevice[0] != null)
                {
                    if (chnFiveSN_comboBox.GetItemText(chnFiveSN_comboBox.SelectedItem).Contains(serialNumOfDevice[0]))
                    {
                        CAN5hnd = Convert.ToInt16(chnFive_comboBox.SelectedItem) - 1;
                        CAN5chanSelc = true;
                    }
                }
                if (serialNumOfDevice[1] != null)
                {
                    if (chnFiveSN_comboBox.GetItemText(chnFiveSN_comboBox.SelectedItem).Contains(serialNumOfDevice[1]))
                    {
                        CAN5hnd = Convert.ToInt16(chnFive_comboBox.SelectedItem) + channelCntOfDevice[0] - 1;
                        CAN5chanSelc = true;
                    }
                }
                if (serialNumOfDevice[2] != null)
                {
                    if (chnFiveSN_comboBox.GetItemText(chnFiveSN_comboBox.SelectedItem).Contains(serialNumOfDevice[2]))
                    {
                        CAN5hnd = Convert.ToInt16(chnFive_comboBox.SelectedItem) + channelCntOfDevice[0] + channelCntOfDevice[1] - 1;
                        CAN5chanSelc = true;
                    }
                }
            }
        }
        #endregion

        #region "GUI Buttons"
        /// <summary>
        /// Use OK_Btn for advancing the APP
        /// <summary>
        private void OK_Btn_Click(object sender, EventArgs e)
        {
            // Save the CAN Configuration to a file
            if (CAN_HW_tB.Text.Contains(subNetName[1]) || CAN_HW_tB.Text.Contains(subNetName[2]) || CAN_HW_tB.Text.Contains(subNetName[3]) || CAN_HW_tB.Text.Contains(subNetName[4]) || CAN_HW_tB.Text.Contains(subNetName[5]))
            {
                string[] lines = CAN_HW_tB.Lines.ToArray(); //This is to split the rich text box into an array
                string richText = string.Empty;
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        //Here is where you re-build the text in the rich text box with lines that have some data in them
                        richText += line;
                        richText += Environment.NewLine;
                    }
                    else
                        continue;
                }
                richText = richText.Substring(0, richText.Length - 2); //Need to remove the last Environment.NewLine, else it will look at it as an other line in the rick text box.
                CAN_HW_tB.Text = richText;

                File.WriteAllLines(dirName + "\\CANconfig.txt", CAN_HW_tB.Lines);
            }
            else
            {
                // ADD the Subnet Identification that is associated with a given CAN channel
                // HOW HOW HOW ??? Use the virtual apply Button click
                applyCANconfigBtn_Click(null, null);
            }
            // Use this string to hold the CANconfig Lines
            txtCAN_HW = CAN_HW_tB.Text;

            if (CAN_HW_tB.Text.Contains(subNetName[1]))
            {
                Canlib.canBusOn(CAN1hnd);
            }
            if (CAN_HW_tB.Text.Contains(subNetName[2]))
            {
                Canlib.canBusOn(CAN2hnd);
            }
            if (CAN_HW_tB.Text.Contains(subNetName[3]))
            {
                Canlib.canBusOn(CAN3hnd);
            }
            if (CAN_HW_tB.Text.Contains(subNetName[4]))
            {
                Canlib.canBusOn(CAN4hnd);
            }
            if (CAN_HW_tB.Text.Contains(subNetName[5]))
            {
                Canlib.canBusOn(CAN5hnd);
            }
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Use Abort to exit the APP if something is wrong with the installed CAN HW
        /// <summary>
        private void Exit_Btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
        }
        #endregion
    }
}
