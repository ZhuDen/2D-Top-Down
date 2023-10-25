using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetNull.ServerManager
{
    public partial class ServerManager : Form
    {
        Data data;

        public ServerManager()
        {
            data = new Data();
            InitializeComponent();
        }

        private void запускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.Start();
            contextMenuStrip1.Items[0].Enabled = false;
            contextMenuStrip1.Items[1].Enabled = true;
            contextMenuStrip1.Items[2].Enabled = true;
        }

        private void остановитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            data.Stop();
            contextMenuStrip1.Items[0].Enabled = true;
            contextMenuStrip1.Items[1].Enabled = false;
            contextMenuStrip1.Items[2].Enabled = false;
        }

        private void остановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.Restart();
            contextMenuStrip1.Items[0].Enabled = false;
            contextMenuStrip1.Items[1].Enabled = true;
            contextMenuStrip1.Items[2].Enabled = true;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.Stop();
            Application.Exit();
        }

        private void просмотрЛоговToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.OpenLogViwe();
        }
    }
}
