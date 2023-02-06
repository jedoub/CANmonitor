using System;

namespace CANmonitor
{
    class SPNattrib
    {                
        public SPNattrib(string spnDBCinfo)
        {
            int strtpt;
            int endpt;
            int lenpt;

            // 0CFF0211 AcceleratorPedalPos : 24|8@1+ (0.4,0) [0|100] "%"
            strtpt = spnDBCinfo.IndexOf(":") + 2;
            endpt = spnDBCinfo.IndexOf("|");
            lenpt = spnDBCinfo.IndexOf("@");

            string SPNstrtBit = spnDBCinfo.Substring(strtpt, endpt - strtpt);
            string SPNlength = spnDBCinfo.Substring(endpt + 1, lenpt - endpt - 1);

            // for every SPN the SPNinfo contains the start bit and the length
            _startBit = Convert.ToInt16(SPNstrtBit);
            _dataLength = Convert.ToInt16(SPNlength);

            strtpt = spnDBCinfo.IndexOf("(") + 1;
            endpt = spnDBCinfo.IndexOf(",");
            lenpt = spnDBCinfo.IndexOf(")");

            _scaler = Convert.ToDecimal(spnDBCinfo.Substring(strtpt, endpt - strtpt));
            _offset = Convert.ToDecimal(spnDBCinfo.Substring(endpt + 1, lenpt - endpt - 1));

            strtpt = spnDBCinfo.IndexOf("[") + 1;
            lenpt = spnDBCinfo.IndexOf("]");
            endpt = spnDBCinfo.IndexOf("|", strtpt, lenpt - strtpt);
            //decimal min = Convert.ToDecimal(spnDBCinfo.Substring(strtpt, endpt - strtpt));
            //double.Parse("1.50E-15", CultureInfo.InvariantCulture)    //Exponential numbers created and exception error. I found this example but I didn't implement it
            //decimal max = Convert.ToDecimal(spnDBCinfo.Substring(endpt + 1, lenpt - endpt - 1));
        }
        private int _startBit;

        public int STARTBIT
        {
            get { return _startBit; }
        }
        private int _dataLength;

        public int BITLENGTH
        {
            get { return _dataLength; }
        }
        private decimal _scaler;

        public decimal SCALER
        {
            get { return _scaler; }
        }
        private decimal _offset;

        public decimal OFFSET
        {
            get { return _offset; }
        }
    }
}
