using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Archiver.Domain.Models.ArchivesFiles;
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
            comboBox1.SelectedIndex = 0;
        }

        private void label1_Click(object sender, EventArgs e)
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
            appLayer.Compress(AlgorithmName.ToString(), PathFrom, PathTo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f3 = new Form3();
            this.Hide();
            f3.ShowDialog();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    AlgorithmName = "HuffmanArchiver";
                    break;
                case 1:
                    AlgorithmName = "LzwArchiver";
                    break;
                case 2:
                    AlgorithmName = "Unknown";
                    break;
            }
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
