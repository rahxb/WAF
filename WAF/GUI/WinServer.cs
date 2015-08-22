using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WAF
{
    public partial class WinServer : Form
    {
        FServer _server = new FServer();

        public WinServer()
        {
            InitializeComponent();

            _server.listen(2000);
        }

    }
}
