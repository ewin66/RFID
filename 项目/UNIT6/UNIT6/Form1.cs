using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Runtime.InteropServices;


namespace UNIT6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //设置变量
        string passWord;
        int sanNum;
        int lockAB;
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string passWord1 = textBox1.Text.ToString().Trim();
            string passWord2 = textBox2.Text.ToString().Trim();

            //判断密码输入合法性
            if (passWord1.Length != 6)
            {
                MessageBox.Show("密码长度必须为6！", "提示");
                return;
            }
            if (passWord1 != passWord2)
            {
                MessageBox.Show("两次密码输入不一致！", "提示");
                return;
            }
            passWord = passWord1; //取出密码

            //判断扇区输入合法性
            try
            {
                sanNum = Convert.ToInt32(textBox3.Text.ToString().Trim());
            }
            catch
            {
                MessageBox.Show("扇区输入错误（必须为0至15）！", "提示");
                return;
            }
            if (sanNum < 0 | sanNum > 15)
            {
                MessageBox.Show("扇区输入错误（必须为0至15）！", "提示");
                return;
            }

            //取出单选钮的值
            if (radioButton1.Checked == true)
                lockAB = 1;
            else
                lockAB = 2;

            //开始加密
            byte[] beep = new byte[8];
            byte[] buf = new byte[16];
            byte[] device = new byte[8];
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            int de;
            de = myApi.GetSerNum(device);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！", "提示");
                return;
            }

            //先读出密码块的数据
            int blkNo = (sanNum + 1) * 4 - 1;
            de = myApi.MF_Read(0, blkNo, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读数据失败！", "提示");
                return;
            }
            //处理密码块的数据
            string blkStr = System.Text.Encoding.Default.GetString(buf);
            if (lockAB == 1)
            {
                blkStr = passWord + blkStr.Substring(6, 10);
                buf = System.Text.Encoding.Default.GetBytes(blkStr);
            }
            else
            {
                blkStr = blkStr.Substring(0, 10) + passWord;
                buf = System.Text.Encoding.Default.GetBytes(blkStr);
            }
            //开始写密码
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Write(0, blkNo, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写密码失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("写密码成功！", "提示");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] beep = new byte[8];
            byte[] buf = new byte[16];
            byte[] key = new byte[6];
            int de;
            Bitmap bmpFile1 = new Bitmap("人.bmp");
            Bitmap bmpFile2 = new Bitmap("鬼.bmp");
            de = myApi.GetSerNum(buf);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！", "提示");
                return;
            }
            //将密码转换成字节数组
            key = System.Text.Encoding.Default.GetBytes(textBox4.Text.ToString());
            sanNum = Convert.ToInt32(textBox3.Text.ToString().Trim());
            int blkNo = (sanNum + 1) * 4 - 1;
            de = myApi.MF_Read(0, blkNo, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);

                this.pictureBox1.Image = Image.FromHbitmap(bmpFile2.GetHbitmap());
                MessageBox.Show("安全认证失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("安全认证成功！", "提示");
            this.pictureBox1.Image = Image.FromHbitmap(bmpFile1.GetHbitmap());
        }
    }
}
