using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment3
{
    public partial class Form2 : Form
    {
        RayTracing RayTracing = new RayTracing();
        int x, y, z;
        public Form2()
        {
            InitializeComponent();
            x = 3;
            y = 3;
            z = 3;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void Left(object sender, EventArgs e)
        {
            this.Enabled = false;
            x = x + 3;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(2, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));

            this.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(3, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.Start(0, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(5, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(6, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(4, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            pictureBox1.Image = Image.FromFile(RayTracing.GetImagePath(1, float.Parse(textBox1.Text.ToString()), float.Parse(textBox2.Text), float.Parse(textBox3.Text), float.Parse(textBox5.Text), float.Parse(textBox4.Text)));
            this.Enabled = true;
        }
    }
}
