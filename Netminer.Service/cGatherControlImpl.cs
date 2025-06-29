using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Interface.WCF;

namespace SoukeyService
{
    public class cGatherControlImpl : iTaskControl
    {
        public void StartTask(string TaskName)
        {
            cGlobal.sEngine.StartTask(TaskName);
        }

        public void StopTask(string TaskName)
        {
            cGlobal.sEngine.StopTask(TaskName);
        }

        public void SetTaskExtPara(string TaskName,string Paras)
        {
            cGlobal.sEngine.SetTaskExtPara(TaskName, Paras);
        }
    }
}
