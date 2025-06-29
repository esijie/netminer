using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SoukeyNetget.CustomControl
{
    public partial class cMyTabPage : TabPage
    {
        public cMyTabPage()
        {
            InitializeComponent();
            base.DoubleBuffered = true;

            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            UpdateStyles(); 
        }

        public cMyTabPage(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            base.DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles(); 
        }

    }
}
