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
                textBox1.SelectionStart = 0;
                textBox1.SelectionLength = textBox1.Text.Length;
                PathFrom = textBox1.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.SelectionStart = 0;
                textBox2.SelectionLength = textBox1.Text.Length;
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
            f3.ShowDialog();
            f3.Close();
        }
    }
}
