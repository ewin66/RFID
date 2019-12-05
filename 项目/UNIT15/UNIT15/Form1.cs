using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace UNIT15
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString().Trim().Length == 0)
            {
                MessageBox.Show("你没有输入任何字符！");
                return;
            }
            byte[] str = new byte[500];
            str = Encoding.UTF8.GetBytes(textBox1.Text);//转码
            //设置配置文件
            myApi.SetPdfConFile(Marshal.StringToHGlobalAnsi("MakeBarCode.ini"));
            //定义位图文件名
            IntPtr bmpFile = Marshal.StringToHGlobalAnsi("testPdf417.bmp");
            //生成PDF417
            string fileName = myApi.EnPdfText(str, bmpFile);
            //将图形显示出来
            Bitmap testBmp = new Bitmap("testPdf417.bmp");
            pictureBox1.Image = Image.FromHbitmap(testBmp.GetHbitmap());
            testBmp.Dispose();//释放图形文件资源，以便下一次使用
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString().Trim().Length == 0)
            {
                MessageBox.Show("你没有输入任何字符！");
                return;
            }
            byte[] str = new byte[500];
            str = Encoding.UTF8.GetBytes(textBox1.Text);//转码
            //设置配置文件
            myApi.SetDmConFile(Marshal.StringToHGlobalAnsi("MakeBarCode.ini"));
            //定义位图文件名
            IntPtr bmpFile = Marshal.StringToHGlobalAnsi("testDataMatrix.bmp");
            //生成DataMatrix
            string fileName = myApi.EnDmText(str, bmpFile);
            //将图形显示出来
            Bitmap testBmp = new Bitmap("testDataMatrix.bmp");
            pictureBox2.Image = Image.FromHbitmap(testBmp.GetHbitmap());
            testBmp.Dispose();//释放图形文件资源，以便下一次使用
        }
    }
}
