﻿using System;
using System.Windows.Forms;

namespace Archiver
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var f1 = new Form1();
            this.Hide();
            f1.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f2 = new Form2();
            this.Hide();
            f2.ShowDialog();
            this.Close();
        }
    }
}
