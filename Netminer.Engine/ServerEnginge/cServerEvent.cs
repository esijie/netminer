using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMiner.Engine.ServerEngine
{
    public class SplitCompletedEventArgs:EventArgs
    {
        public SplitCompletedEventArgs(int tID)
        {
            _tID = tID;
        }

        private int _tID;
        public int tID
        {
            get { return _tID; }
            set { _tID = value; }
        }
    }


}
