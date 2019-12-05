using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace UNIT5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strValue;
            UInt32 intValue;
            strValue = textBox1.Text.ToString().Trim();
            if (strValue.Length == 0)
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }
            try
            {
                intValue = Convert.ToUInt32(strValue);
            }
            catch
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }

            if (intValue < 1 | intValue > 999)
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }
            int de;
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            byte[] buf = new byte[4];
            buf = BitConverter.GetBytes(intValue);
            byte[] beep = new byte[8];
            byte[] device = new byte[8];

            //初始化值段数据
            de = myApi.MF_InitValue(0, 4, key, buf);
            if (de != 0)
            {
                MessageBox.Show("初始化值段数据失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("初始化值段数据成功！", "提示");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int de;
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            byte[] buf = new byte[4];
            buf = BitConverter.GetBytes(0);
            byte[] beep = new byte[8];
            de = myApi.MF_Dec(0, 4, key, buf);
            if (de != 0)
            {
                MessageBox.Show("初始化值段数据失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            int intValue = buf[0] + buf[1] * 256 + buf[2] * 256 * 256;
            MessageBox.Show("余额为：" + intValue, "提示");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strValue;
            int intValue;
            strValue = textBox2.Text.ToString().Trim();
            if (strValue.Length == 0)
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }
            try
            {
                intValue = Convert.ToInt32(strValue);
            }
            catch
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }
            if (intValue < 1 | intValue > 999)
            {
                MessageBox.Show("无效的充值金额！", "提示");
                return;
            }
            //开始充值
            int de;
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            byte[] buf = new byte[4];
            buf = BitConverter.GetBytes(intValue);
            byte[] beep = new byte[8];
            de = myApi.MF_Inc(0, 4, key, buf);
            if (de != 0)
            {
                MessageBox.Show("充值失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("充值成功！", "提示");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) + 10).ToString();
            else
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) - 10).ToString();
        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            int intValue;
            intValue = Convert.ToInt32(textBox3.Text.ToString());

            if (intValue == 0)
            {
                MessageBox.Show("无效的消费金额！", "提示");
                return;
            }
            //开始刷卡消费
            int de;
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            byte[] buf = new byte[4];
            buf = BitConverter.GetBytes(intValue);
            byte[] beep = new byte[8];
            de = myApi.MF_Dec(0, 4, key, buf);
            if (de != 0)
            {
                MessageBox.Show("刷卡消费失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("刷卡消费成功！", "提示");
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) + 12).ToString();
            else
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) - 12).ToString();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) + 8).ToString();
            else
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) - 8).ToString();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) + 8).ToString();
            else
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) - 8).ToString();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) + 5).ToString();
            else
                textBox3.Text = (Convert.ToInt32(textBox3.Text.ToString()) - 5).ToString();
        }
    
        
    }
}
