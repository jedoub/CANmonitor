# CANmonitor
CANmonitor is the project name. The main WinForm provides a readout of the J1939 CAN messages on 2 subnets.
If a CAN dbc file is associated with the APP, (a sample is included in the project). the PGN message names are displayed next to the numeric ID.
When the user pauses the monitor and selects (double-clicks) an ID the view explodes the message to display the SPNs.
The message data is parsed and scaled to the physical units detailed in the dbc file.
The J1939 Transport protocol (TPCM/TPDT) messages are inserted into the trace and displayed as "virtual" J1939 messages.
The data of these types of messages are also displayed when selected provided the DBC file contains their definition.

