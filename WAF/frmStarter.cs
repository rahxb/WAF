using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tcp
{
    public partial class frmStarter : Form
    {
        public frmStarter()
        {
            InitializeComponent();
        }

        WinServer s = new WinServer();
        WinClient c = new WinClient();
        WinClient c2 = new WinClient();

        private void frmStarter_Load(object sender, EventArgs e)
        {
            s.Show();
            c.Show();
            c2.Show();
        }
    }
}
