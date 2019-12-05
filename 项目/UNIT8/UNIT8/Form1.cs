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



namespace UNIT8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //询查电子标签
            byte[] beep = new byte[8];
            byte[] buf = new byte[300];
            byte[] Num = new byte[1];
            int nums = 0;
            int de;
            de = myApi.ISO15693_Inventory(Num, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(28, 1, beep);
                MessageBox.Show("询查电子标签失败！", "提示");
                return;
            }
            myApi.ControlBuzzer(5, 1, beep);
            nums = Num[0];
            if (nums != 1)
            {
                MessageBox.Show("电子标签不只一个，必须为单个电子标签！", "提示");
                return;
            }
            //处理uid码
            string strtemp;
            string uidCode = "";
            for (int j = 2; j < 10; j++)
            {
                if (buf[j] < 0)
                    strtemp = Convert.ToByte(Convert.ToInt32(buf[j]) + 256).ToString("X2");
                else
                    //字节转成16进制串
                    strtemp = buf[j].ToString("X2");

                uidCode += strtemp;
            }
            //判断是何种物品
            if (radioButton1.Checked) textBox1.Text = uidCode;
            if (radioButton2.Checked) textBox2.Text = uidCode;
            if (radioButton3.Checked) textBox3.Text = uidCode;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "开启报警")
            {
                button2.Text = "关闭报警";
                timer1.Enabled = true;
            }
            else
            {
                button2.Text = "开启报警";
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //询查电子标签
            byte[] beep = new byte[8];
            byte[] buf = new byte[300];
            byte[] Num = new byte[1];
            int nums = 0;
            int de;
            de = myApi.ISO15693_Inventory(Num, buf);
            //if (de != 0)
            //{
            //    myApi.ControlBuzzer(28, 1, beep);
            //    MessageBox.Show("询查电子标签失败！", "提示");
            //    return;
            //}
            nums = Num[0];

            //处理uid码
            string strtemp;
            string uidCode = "";
            for (int j = 0; j < nums * 10; j++)
            {
                if (buf[j] < 0)
                    strtemp = Convert.ToByte(Convert.ToInt32(buf[j]) + 256).ToString("X2");
                else
                    //字节转成16进制串
                    strtemp = buf[j].ToString("X2");

                uidCode += strtemp;
            }

            //判断失窃商品
            if (uidCode.IndexOf(textBox1.Text.Trim()) < 0) ListShow("手表");

            if (
                uidCode.IndexOf(textBox2.Text.Trim()) < 0) ListShow("钻石");

            if (uidCode.IndexOf(textBox3.Text.Trim()) < 0) ListShow("戒指");

        }

        //显示被盗信息函数
        void ListShow(string ThingName)
        {
            byte[] beep = new byte[8];
            listBox1.Items.Add("********失窃信息**********");
            listBox1.Items.Add("被盗物品：" + ThingName);
            listBox1.Items.Add("被盗时间：" + DateTime.Now.ToString());
            listBox1.Items.Add("**************************");
            myApi.ControlBuzzer(28, 3, beep);
            Bitmap bmpFile = new Bitmap("报警.JPG");
            switch (ThingName)
            {
                case "手表":
                    pictureBox1.Image = Image.FromHbitmap(bmpFile.GetHbitmap());
                    break;
                case "钻石":
                    pictureBox2.Image = Image.FromHbitmap(bmpFile.GetHbitmap());
                    break;
                case "戒指":
                    pictureBox3.Image = Image.FromHbitmap(bmpFile.GetHbitmap());
                    break;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
