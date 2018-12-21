using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogAnalyzer
{
    public partial class FrmMain : Form
    {
        Control logMgr = new LogManager();

        public FrmMain()
        {
            InitializeComponent();
            this.Text = Application.ProductName + "_v" + Application.ProductVersion + "    " + Application.CompanyName;

            this.Controls.Add(logMgr);
            
        }
    }
}
