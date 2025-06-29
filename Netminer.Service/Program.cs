﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SoukeyService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new MainService() 
			};
            ServiceBase.Run(ServicesToRun);
        }

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";
        }
    }
}
