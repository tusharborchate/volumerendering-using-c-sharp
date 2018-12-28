using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment3
{
    public partial class Form1 : Form
    {
        string s;
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RayTracing();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            RayTracing();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            RayTracing();
        }

        public void RayTracing()
        {
            try
            {


            if (textBox1.Text != "" && textBox2.Text != "" && textBox2.Text != "" && textBox3.Text != "" 
                && textBox4.Text != "" && textBox5.Text != "" && textBox6.Text != "" && textBox7.Text != "" && textBox8.Text != ""
                && textBox9.Text != "" && textBox10.Text != "" && textBox11.Text != "" && textBox12.Text != ""


                && textBox13.Text != ""
                && textBox14.Text != ""
                && textBox15.Text != ""
                && textBox16.Text != ""
                && textBox17.Text != ""

                && textBox18.Text != ""
                && textBox19.Text != ""
                && textBox20.Text != ""
                && textBox21.Text != ""
                && textBox22.Text != ""
                && textBox23.Text != ""
                && textBox24.Text != ""
                && textBox25.Text != ""
                )
            {
                    this.Enabled = false;
                    Vector vrp = new Vector();
                vrp.X = Convert.ToDouble(textBox4.Text);
                vrp.Y = Convert.ToDouble(textBox5.Text);
                vrp.Z = Convert.ToDouble(textBox6.Text);

                Vector vpn = new Vector();
                vpn.X = Convert.ToDouble(textBox7.Text);
                vpn.Y = Convert.ToDouble(textBox8.Text);
                vpn.Z = Convert.ToDouble(textBox9.Text);


                Vector vup = new Vector();
                vup.X = Convert.ToDouble(textBox10.Text);
                vup.Y = Convert.ToDouble(textBox11.Text);
                vup.Z = Convert.ToDouble(textBox12.Text);

                    Sphere sphere = new Sphere();
                    sphere.Center = new Vector();
                    sphere.Center.X = Convert.ToDouble(textBox14.Text);
                    sphere.Center.Y = Convert.ToDouble(textBox13.Text);
                    sphere.Center.Z = Convert.ToDouble(textBox15.Text);
                    sphere.Radius = Convert.ToDouble(textBox16.Text);


                   Polygon Polygon = new Polygon();
                    Polygon.V0 = new Vector();
                    Polygon.V1 = new Vector();
                    Polygon.V2 = new Vector();
                    Polygon.V3 = new Vector();
                    Polygon.Normal = new Vector();


                    Polygon.V0.X = Convert.ToDouble(textBox17.Text);
                    Polygon.V0.Y = Convert.ToDouble(textBox18.Text);
                    Polygon.V0.Z = Convert.ToDouble(textBox19.Text);


                    Polygon.V1.X = Convert.ToDouble(textBox20.Text);
                    Polygon.V1.Y = Convert.ToDouble(textBox21.Text);
                    Polygon.V1.Z = Convert.ToDouble(textBox22.Text);

                    Polygon.V2.X = Convert.ToDouble(textBox23.Text);
                    Polygon.V2.Y = Convert.ToDouble(textBox24.Text);
                    Polygon.V2.Z = Convert.ToDouble(textBox25.Text);


                    RayTracingComputation rayTracingComputation = new RayTracingComputation();
                 s = rayTracingComputation.RayTracing(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), Convert.ToDouble(textBox3.Text), vrp, vpn, vup,sphere,Polygon);
                pictureBox1.Image = Image.FromFile(s);
                    this.Enabled = true;
            }

            }
            catch (Exception)
            {
                this.Enabled = true;
                
            }
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            RayTracing();


        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            RayTracing();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
            Process.Start(s);

            }
            catch (Exception)
            {

                
            }
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            RayTracing();
        }
    }
}
