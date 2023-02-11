using CANsetup;
using System.Collections.Generic;
using Kvaser.CanLib;

namespace CANmonitor
{
    class CanMsgProc
    {
        private static Canlib.canStatus  canTimer_status = 0;
        private static long CurrentTime;
        public static bool doOnce = false;
        public static List<int> msgSubnet = new List<int>();
        public static List<int> msgIDs = new List<int>();
        public static List<int> msgDLCs = new List<int>();
        public static List<byte[]> msgData = new List<byte[]>();
        public static List<long> msgTimeStamp = new List<long>();
        public static List<long> msgTimeStamp_pv = new List<long>();
        public static List<int> msgNodes = new List<int>();
        public static bool DM1tpServiced = true;

        public bool StandardMsgInfo(int subnetHandle, int id, byte[] CANmsg, int dlc, long CurrentTime)
        {
            bool msgHandled = false;

            if (msgHandled == false)
            {
                // HANDLE standard 8-Byte messages here
                if (!msgIDs.Contains(id))
                {
                    // I made a collection of Lists one with the J1939 MSG ID and one with the DATA etc.
                    // Each list is populated for every RCV'D msg to keep the indexes in sync.
                    // Add unique messages that are received on the CAN BUS to the List
                    msgSubnet.Add(subnetHandle);
                    msgIDs.Add(id);
                    msgDLCs.Add(dlc);
                    msgData.Add(CANmsg);
                    if (canTimer_status == Canlib.canStatus.canOK)
                    {
                        msgTimeStamp.Add(CurrentTime - Form1.zeroTime);
                        msgTimeStamp_pv.Add(CurrentTime - Form1.zeroTime);   //Just assign the same time initially.
                    }

                    // Capture unique Nodes on the J1939 CAN subnet. I show the NODES and Names in the DTC FORM
                    if (!msgNodes.Contains(0xFF & id))
                    {
                        msgNodes.Add(0xFF & id);
                    }
                }
                else
                {
                    // Update the data of messages already received here
                    // The index of the msg is found using a linq lambda expression which is a shorthand notation AKA "anonymous" method
                    // When dealing with Lists and such C# manages the delgation of an object & parameter type when the C# parser sees => in a statement.
                    int msgIDindx = msgIDs.FindIndex(s => s == id);
                    // Refresh these data after the CANmonitor has displayed the multi-packet message.
                    // The boolean indicates the DM1 has been shown in the CANmonitor window. It is OK to go back to a standard DM1msg (ie None or 1 active DTC)
                    if (DM1tpServiced == true)
                    {                        
                        msgDLCs[msgIDindx] = dlc;
                        msgData[msgIDindx] = new byte[dlc];
                        msgData[msgIDindx] = CANmsg;
                    }
                    msgTimeStamp_pv[msgIDindx] = msgTimeStamp[msgIDindx];     // save the previous time
                    if (canTimer_status == Canlib.canStatus.canOK) 
                        msgTimeStamp[msgIDindx] = CurrentTime;                                       // update the current time
                }
            }
            return msgHandled = true;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * TPCM/TPDT EXAMPLE of the EC1_E message. 
         * This msg has several NA data bytes at the end. You need to use the total byte cnt 39d to determine the last real data byte
           * BAM                  0.714654 1  18ECFF00x       Rx   d 8 20 27 00 06 FF E3 FE 00
           * TPDT                 0.718595 1  18EBFB00x       Rx   d 8 01 E0 15 A8 70 3F B3 20
           * TPDT                 0.719711 1  18EBFB00x       Rx   d 8 02 1C B2 F0 23 C9 70 30
           * TPDT                 0.720275 1  18EBFB00x       Rx   d 8 03 C6 78 3F FF FF A4 0D
           * TPDT                 0.720831 1  18EBFB00x       Rx   d 8 04 20 4E 3C 3C CB 7D C9
           * TPDT                 0.721407 1  18EBFB00x       Rx   d 8 05 78 3F A5 01 FF FF FF
           * TPDT                 0.722568 1  18EBFB00x       Rx   d 8 06 FF FF FF[FF]FF FF FF
           * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public int TPCMinfo(int subnetNum, int msgID, byte [] BAM)
        {
            int msgIDindx = 0;

            canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out CurrentTime);    // Update the wait for the TPDT msg that follows.
            // This is an over check on the consistencay of the data stream. Consecutive TPDT packet should be within 50ms of one another.

            // Idendify the PGN contained in the TPCM using the Last 3 bytes of the BAM
            int pgn = (BAM[7] << 16) + (BAM[6] << 8) + BAM[5];

            // Identify the Sender ECU using the last 2 digits of the J1939 TPCM MSG ID
            string sourceECU = (0xFF & msgID).ToString("X2");

            // 6 of the 8 HEX digits (Bits 0-23) of the 29-Bit ID are known using the PGN and Sender Node.
            int extID = (pgn << 8) + (0xFF & msgID);

            // But Bits 24-29 are Bit mapped and are defined by the SAE to indicate Priority + J1939 connection management attributes.
            // These bits that are determinied by the physical HW connected to the network.
            // I assume these bits are the same value as used in the BAM to yield the 29-Bit ID used.
            extID += msgID & 0x7F000000;

            // if the msg is on CAN2 I artificially add the 32nd BIT to differentiate the msgID from a CAN1 msg. I remove it later.
            if (subnetNum == CANsetupForm.CAN2hnd)
                extID = (int)(extID | 0x80000000);

            // Make a "virtual message" and add it into the overall RCV'D message list and create an array to hold the data.
            if (!msgIDs.Contains(extID))
            {
                msgSubnet.Add(subnetNum);
                // Save the extended message ID into the list of TPCM_E. After the data is totally received add the extID to the list of standard msgs
                msgIDs.Add(extID);
                // Save the number of data Bytes that are used
                msgDLCs.Add(BAM[1]);
                // Allocate the size of the data array at runtime using the Packet count * 7.
                // This over allocates a few bytes but represents the true number of bytes transmitted and simplifies the TPDT handler logic.
                msgData.Add(new byte[BAM[3] * 7]);
                if (canTimer_status == Canlib.canStatus.canOK)
                {
                    msgTimeStamp.Add(CurrentTime - Form1.zeroTime);
                    msgTimeStamp_pv.Add(CurrentTime - Form1.zeroTime);
                }
                if (extID == Form1.DM1_E)
                {
                    DM1tpServiced = false;
                    Form1.DM1Etpmsg = new byte[BAM[1]];
                }
            }
            else
            {
                msgIDindx = msgIDs.FindIndex(s => s == extID);

                msgDLCs[msgIDindx] = BAM[1];
                msgData[msgIDindx] = new byte[BAM[1]];
                msgTimeStamp_pv[msgIDindx] = msgTimeStamp[msgIDindx];
                if (canTimer_status == Canlib.canStatus.canOK)
                {
                    msgTimeStamp[msgIDindx] = CurrentTime - Form1.zeroTime;
                }
            }
            // How do I know which Message ID to link the TPDT data to?
            // I return the MSG ID so when the TPDTs arrive the data is associated with the MSG ID returned here.            
            // Additionally, the returned extID is used as a flag to indicate a Multi-Packet message is in progress in the TPDT function. 
            return extID;
        }
        public int TPDTinfo(int extID, byte [] data)
        {
            int TPDT_finished = extID;
            int msgIDindx = 0;

            canTimer_status = Canlib.kvReadTimer64(CANsetupForm.CAN1hnd, out CurrentTime);
            if (extID == 0x18FECA03)
                extID = 0x18FECA03;

            // At This point the Multipacket message ID has been added and space reserved from the TPCMinfo into the RCV'D message List.
            // Find the msg in the LIST using a lambda search, then use the msg List index to associate & populate the data in the RCV'D msgList.
            msgIDindx = msgIDs.FindIndex(s => s == extID);
            
            // Index is valid if the returned value is not negative
            if (msgIDindx != -1)
            {
                // PROCESS ACTIVE MULTI-PACKET MSG DATA announced in the TPCM. 
                // the 1st byte data[0] of the TPDT holds the packet number so that can be used to index into the data array.
                if (data[0] * 7 < msgDLCs[msgIDindx])
                {
                    // Add the newly received message data bytes to the specific msg array 
                    for (byte i = 1; i < 8; i++)
                    {
                        msgData[msgIDindx][(i - 1) + (data[0] - 1) * 7] = data[i];

                        // I watch for EECU Transient DTCs as a special case. So I have a small duplicate buffer 
                        if (extID == Form1.DM1_E)
                            Form1.DM1Etpmsg[(i - 1) + (data[0] - 1) * 7] = data[i];
                    }
                }
                else
                {
                    // This is the final packet so, Add the newly received message data & update the message time. 
                    int bytesLeft = 7 - ((data[0] * 7) - msgDLCs[msgIDindx]);

                    for (byte i = 1; i <= bytesLeft; i++)
                    {
                        msgData[msgIDindx][(i - 1) + (data[0] - 1) * 7] = data[i];
                        
                        // I watch for EECU Transient DTCs as a special case. So I have a small duplicate buffer 
                        if (extID == Form1.DM1_E)
                            Form1.DM1Etpmsg[(i - 1) + (data[0] - 1) * 7] = data[i];
                    }
                    TPDT_finished = 0;
                }
            }
            return TPDT_finished;
        }
    }
}
