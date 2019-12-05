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

namespace UNIT7
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
        string uidCodes = ""; 
        private void Form1_Load(object sender, EventArgs e)
        {
            //创建并打开连接
            conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=bookSet.mdb";
            conn.Open();
            comm.Connection = conn;
            //执行查询命令
            comm.CommandText = "select * from book";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //为表格控件建立数据源
            ds.Clear();
            adapter.Fill(ds, "book");
            dataGridView1.DataSource = ds.Tables["book"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adapter.Update(ds, "book");
        }

        private void button2_Click(object sender, EventArgs e)
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
            dataGridView1.CurrentRow.Cells["UID码"].Value = uidCode;
            adapter.Update(ds, "book");
        }

        private void button3_Click(object sender, EventArgs e)
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

            //处理uid码
            string strtemp;
            string uidCode = "";
            uidCodes = "";
            for (int i = 0; i < nums; i++)
            {
                for (int j = 2; j < 10; j++)
                {
                    if (buf[i * 10 + j] < 0)
                        strtemp = Convert.ToByte(Convert.ToInt32(buf[i * 10 + j]) + 256).ToString("X2");
                    else
                        //字节转成16进制串
                        strtemp = buf[i * 10 + j].ToString("X2");

                    uidCode += strtemp;
                }
                //将所有电子标签的UID码合并到一起
                uidCodes = uidCodes + uidCode;
            }

            //遍历数据表
            listBox1.Items.Clear();
            listBox1.Items.Add("************************");
            for (int k = 0; k < dataGridView1.Rows.Count - 1; k++)
            {
                string uidCodek = dataGridView1.Rows[k].Cells[2].Value.ToString().Trim();
                if (uidCodes.IndexOf(uidCodek) != -1 & uidCodek.Length > 0)
                    listBox1.Items.Add(dataGridView1.Rows[k].Cells[1].Value.ToString().Trim());
            }
            listBox1.Items.Add("************************");
        }

        private void button4_Click(object sender, EventArgs e)
        {

            //遍历数据表
            for (int k = 0; k < dataGridView1.Rows.Count - 1; k++)
            {
                string uidCodek = dataGridView1.Rows[k].Cells[2].Value.ToString().Trim();
                if (uidCodes.IndexOf(uidCodek) != -1 & uidCodek.Length > 0)
                {
                    dataGridView1.Rows[k].Cells[4].Value
                        = Convert.ToInt32(dataGridView1.Rows[k].Cells[4].Value) - 1;
                }
            }
            adapter.Update(ds, "book");
            MessageBox.Show("成功借阅！", "提示");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }
    }
}
