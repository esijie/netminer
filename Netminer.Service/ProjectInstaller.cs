﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace SoukeyService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void sominerServerInstall_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
