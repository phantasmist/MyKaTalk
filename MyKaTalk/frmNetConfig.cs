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
    public partial class frmNetConfig : Form
    {
        public frmNetConfig(int sp=9000, int cp=9000, string cip="127.0.0.1", string uid="", string upwd="", bool isServer=true)
        {
            InitializeComponent();
            tbServerPort.Text = $"{sp}";
            tbConnectPort.Text = $"{cp}";
            tbConnectIP.Text = cip;
            tbUserID.Text = uid;
            tbPassword.Text = upwd;
            if (isServer) rbServer.Checked = true;
            else rbClient.Checked = true;
        }

        private void rbServer_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
