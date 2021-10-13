using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyKaTalk
{
    public partial class frmClient : Form
    {
        public frmClient()
        {
            InitializeComponent();
        }
        //이 두 값만 반환하면 됨
        public string strIP = "";
        public string port = "";

        private void btnOK_Click(object sender, EventArgs e)
        {
            strIP = tbServerIP.Text;
            port = tbServerPort.Text;
        }
    }
}
