using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace UNIT3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //声明相关变量
        OleDbConnection conn = new OleDbConnection();
        OleDbCommand comm = new OleDbCommand();
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataSet ds = new DataSet();
        private void Form1_Load(object sender, EventArgs e)
        {
            //创建并打开连接
            //conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Students_Mdb.mdb";
            conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=students_Mdb.accdb";
            conn.Open();
            comm.Connection = conn;
            //执行查询命令
            comm.CommandText = "select * from Students_Tab";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //为表格控件建立数据源
            ds.Clear();
            adapter.Fill(ds, "Students_Tab");
            dataGridView1.DataSource = ds.Tables["Students_Tab"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //初始化读写器
            byte[] beep = new byte[8];
            byte[] buf = new byte[16];
            byte[] device = new byte[16];
            byte[] key = new byte[6];
            string _XH, _XM, _ZH, _ZY, _DH;
            int de;
            de = myApi.GetSerNum(device);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！");
                return;
            };
            //ControlBuzzer(5, 1, beep);
            //MessageBox.Show("读写器初始化成功！");

            //将字段数据取出
            int rowIndex = dataGridView1.CurrentCellAddress.Y;
            _XH = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString().Trim();
            _XM = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString().Trim();
            _ZH = dataGridView1.Rows[rowIndex].Cells[3].Value.ToString().Trim();
            _ZY = dataGridView1.Rows[rowIndex].Cells[4].Value.ToString().Trim();
            _DH = dataGridView1.Rows[rowIndex].Cells[5].Value.ToString().Trim();

            //开始写数据
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;

            //写学号到第1块
            buf = System.Text.Encoding.Default.GetBytes(_XH);//将字符串转换成字节数组
            de = myApi.MF_Write(0, 1, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写学号失败！");
                return;
            };

            //写姓名到第2块
            buf = System.Text.Encoding.Default.GetBytes(_XM);
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Write(0, 2, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写姓名失败！");
                return;
            };

            //写身份证到第4块
            buf = System.Text.Encoding.Default.GetBytes(_ZH);
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Write(0, 4, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写身份证失败！");
                return;
            };

            //写专业到第5块
            buf = System.Text.Encoding.Default.GetBytes(_ZY);
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Write(0, 5, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写专业失败！");
                return;
            };

            //写电话到第6块
            buf = System.Text.Encoding.Default.GetBytes(_DH);
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Write(0, 6, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写电话失败！");
                return;
            };

            //处理uid码
            string strtemp;
            string uidCode = "";
            for (int j = 0; j < 4; j++)
            {
                if (key[j] < 0)
                    strtemp = Convert.ToByte(Convert.ToInt32(key[j]) + 256).ToString("X2");
                else
                    //字节转成16进制串
                    strtemp = key[j].ToString("X2");

                uidCode += strtemp;
            }

            //刷新数据到数据库
            dataGridView1.Rows[rowIndex].Cells[6].Value = uidCode;
            //获取当前学号
            int curY = dataGridView1.CurrentCellAddress.Y;
            string curXH = dataGridView1.Rows[curY].Cells[1].Value.ToString();
            //构建SQL语句
            comm.CommandText = "Update Students_Tab SET UID码='" + uidCode + "' Where 学号='" + curXH + "'";
            comm.ExecuteNonQuery();
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("制卡成功！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] key = new byte[6];
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            byte[] device = new byte[16];
            byte[] buf = new byte[16];
            byte[] beep = new byte[8];
            string xh = "", xm = "", zh = "", zy = "", dh = "";
            int de;

            de = myApi.GetSerNum(device);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！");
                return;
            };

            //读学号
            de = myApi.MF_Read(0, 1, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读学号失败！");
                return;
            };
            //将字节数组转换成字符串
            xh = System.Text.Encoding.Default.GetString(buf);

            //读姓名
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 2, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读姓名失败！");
                return;
            };
            //将字节数组转换成字符串
            xm = System.Text.Encoding.Default.GetString(buf);

            //读身份证
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 4, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读身份证失败！");
                return;
            };
            //将字节数组转换成字符串
            zh = System.Text.Encoding.Default.GetString(buf);

            //读专业
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 5, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读专业失败！");
                return;
            };
            //将字节数组转换成字符串
            zy = System.Text.Encoding.Default.GetString(buf);

            //读电话
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 6, 1, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读电话失败！");
                return;
            };
            //将字节数组转换成字符串
            dh = System.Text.Encoding.Default.GetString(buf);


            listBox1.Items.Add("******************");
            listBox1.Items.Add("学号：" + xh);
            listBox1.Items.Add("姓名：" + xm);
            listBox1.Items.Add("身份证：" + zh);
            listBox1.Items.Add("专业：" + zy);
            listBox1.Items.Add("电话：" + dh);
            listBox1.Items.Add("******************");
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("读卡成功！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }
    }
}
