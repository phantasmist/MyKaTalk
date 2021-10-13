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
    public partial class frmServer : Form
    {
        public frmServer()
        {
            InitializeComponent();
        }
        //이것만 참조하면 되니까..
        public string port = "";
        private void btnOK_Click(object sender, EventArgs e)
        {
            port = tbServerPort.Text;
        }
    }
}
