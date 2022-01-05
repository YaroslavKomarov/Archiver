using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Archiver.Application;

namespace Archiver
{
    public partial class Form2 : Form
    {
        private string PathFrom { get; set; }
        private string PathTo { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                PathFrom = textBox1.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox2.Text))
            {
                PathTo = textBox2.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var appLayer = new ApplicationLayer();
            appLayer.Decompress(PathFrom, PathTo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f3 = new Form3();
            this.Hide();
            f3.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fd.InitialDirectory + fd.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = fd.SelectedPath;
            }
        }
    }
}
