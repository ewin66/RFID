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
using System.IO; 

namespace UNIT2
{
    public partial class Form1 : Form
    {

        OleDbConnection conn = new OleDbConnection();
        OleDbCommand comm = new OleDbCommand();
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataSet ds = new DataSet();
        string _ZH, _XM, _DZ;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Person_MDB.mdb";
            conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Person_MDB.accdb";
            conn.Open();
            comm.Connection = conn;
            comm.CommandText = "select * from Person_TAB";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            ds.Clear();
            adapter.Fill(ds, "Person_TAB");
            dataGridView1.DataSource = ds.Tables["Person_TAB"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //定义变量
            int iResultValue = 0;
            byte[] CardPUCIIN = new byte[4];
            byte[] CardPUCSN = new byte[8];
            byte[] mf = new byte[512];
            byte[] pf = new byte[1024];
            int ml = 0;
            int pl = 0;


            //初始化端口
            if (myApi.SDT_OpenPort(1001) != 0x90)
            {
                MessageBox.Show("读写器初始化失败！");
                return;
            }

            //寻卡
            iResultValue = myApi.SDT_StartFindIDCard(1001, CardPUCIIN, 1);
            if (iResultValue != 0x9f)
            {
                MessageBox.Show("寻身份证失败！请重新放卡身份证！");
                return;
            }

            //选卡
            iResultValue = myApi.SDT_SelectIDCard(1001, CardPUCSN, 1);
            if (iResultValue != 0x90)
            {
                MessageBox.Show("选卡失败！");
                return;
            }

            //读基本信息
            iResultValue = myApi.SDT_ReadBaseMsg(1001, mf, ref ml, pf, ref pl, 1);
            if (iResultValue != 0x90)
            {
                MessageBox.Show("读基本信息失败！");
                return;
            }

            //读取照片
            FileStream savefile = File.Create("result.wlt");
            savefile.Write(pf, 0, pl);
            savefile.Close();
            byte[] f_name = System.Text.Encoding.Default.GetBytes("result.wlt");
            iResultValue = myApi.GetBmp(f_name, 2);
            if (iResultValue != 1)
            {
                MessageBox.Show("读身份证照片失败！" + iResultValue);
                return;
            }


            //转换数据格式
            string str = System.Text.ASCIIEncoding.Unicode.GetString(mf);
            _XM = str.Substring(0, 12).Trim();
            _DZ = str.Substring(29, 32).Trim();
            _ZH = str.Substring(50, 29).Trim();
            //将结果显示出来
            textBox1.Text = _XM;
            textBox2.Text = _DZ;
            textBox3.Text = _ZH;
            Bitmap bmpFile = new Bitmap("result.bmp");
            this.pictureBox1.Image = Image.FromHbitmap(bmpFile.GetHbitmap());

            //关闭读写器端口
            myApi.SDT_ClosePort(1001);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //送数据入库
            string _ID = Convert.ToString(dataGridView1.Rows.Count);
            _XM = textBox1.Text.Trim();
            _DZ = textBox2.Text.Trim();
            _ZH = textBox3.Text.Trim();
            comm.CommandText = "Insert Into Person_Tab (id,name,address,idcode) values ('"
                       + _ID + "','" + _XM + "','" + _DZ + "','" + _ZH + "')";
            comm.ExecuteNonQuery();
            //再次查询
            comm.CommandText = "select * from Person_Tab";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);

            //为表格控件刷新数据源
            ds.Clear();
            adapter.Fill(ds, "Person_Tab");
            dataGridView1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }
    }
}
