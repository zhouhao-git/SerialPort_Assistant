using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        int count;
        int time;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            int i;
            for (i = 1; i < 100; i++)
            {
                comboBox1.Items.Add(i.ToString() + " 秒");
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string str;

            count++;
            str = comboBox1.Text;
            time = Convert.ToInt16(str.Substring(0,2));
            label3.Text = (time - count).ToString() + "秒";
            progressBar1.Maximum = time;
            progressBar1.Value = count;
            if (time == count)
            {
                timer1.Stop();
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("time down!","提示");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "开始计时")
            {
                if (comboBox1.Text != "0")
                {
                    timer1.Start();
                }
                else
                {
                    MessageBox.Show("请输入正确的时间","提示");
                }
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
