using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Archiver.Domain.Models.File;
using Archiver.Application;

namespace Archiver
{
    public partial class Form1 : Form
    {
        private string AlgorithmName { get; set; }
        private string PathFrom { get; set; }
        private string PathTo { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            AlgorithmName = "HaffmanArchiver";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            AlgorithmName = "LzwArchiver";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            AlgorithmName = "Unknown";
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
            appLayer.Compress(AlgorithmName.ToString(), PathFrom, PathTo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f3 = new Form3();
            f3.ShowDialog();
            f3.Close();
        }
    }
}
