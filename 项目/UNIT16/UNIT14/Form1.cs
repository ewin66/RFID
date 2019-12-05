using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCode.Codec;
using System.Data.OleDb;
using System.IO;

namespace UNIT14
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //声明数据库操作相关变量
        OleDbConnection conn = new OleDbConnection();
        OleDbCommand comm = new OleDbCommand();
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataSet ds = new DataSet();
        private void button1_Click(object sender, EventArgs e)
        {
            String _STR = textBox1.Text.ToString().Trim();
            if (_STR.Length == 0)
            {
                MessageBox.Show("没有输入任何编码信息！");
                return;
            }
            Bitmap QRfile;
            QRCodeEncoder qrCode = new QRCodeEncoder();
            qrCode.QRCodeVersion = 6;
            QRfile = qrCode.Encode(_STR, Encoding.UTF8);
            pictureBox1.Image = Image.FromHbitmap(QRfile.GetHbitmap());
            this.Update();
            dataGridView1.CurrentRow.Cells["二维码"].Value = Image.FromHbitmap(QRfile.GetHbitmap());
            adapter.Update(ds, "stuffTab");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string _str = "编号:" + dataGridView1.CurrentRow.Cells["编号"].Value.ToString() +
                          "/姓名:" + dataGridView1.CurrentRow.Cells["姓名"].Value.ToString() +
                          "/性别:" + dataGridView1.CurrentRow.Cells["性别"].Value.ToString() +
                          "/地址:" + dataGridView1.CurrentRow.Cells["地址"].Value.ToString();
            textBox1.Text = _str;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            adapter.Update(ds, "stuffTab");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //创建并打开连接
            conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=stuffMdb.mdb";
            conn.Open();
            comm.Connection = conn;
            //执行查询命令
            comm.CommandText = "select * from stuffTab";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //为表格控件建立数据源
            ds.Clear();
            adapter.Fill(ds, "stuffTab");
            dataGridView1.DataSource = ds.Tables["stuffTab"];
        }
    }
}
