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
using System.IO;
using Aspose.Cells;


namespace UNIT4
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

        //申明读写器相关操作变量
        byte[] key = new byte[6];
        byte[] buf = new byte[32];
        byte[] beep = new byte[8];
        byte[] device = new byte[16];
        int de;

        private void button6_Click(object sender, EventArgs e)
        {
            //打开保存文件对话框
            string path;
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel files(*.xls)|*.xls";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                path = saveFile.FileName;
                //定义电子表格变量
                Workbook wb = new Workbook();
                Worksheet ws = wb.Worksheets[0];
                Cells cell = ws.Cells;
                //获取表格总行数和列数
                int _rows, _cells;
                _rows = dataGridView1.Rows.Count;
                _cells = dataGridView1.Rows[1].Cells.Count;
                //定义字符串二维数组
                string[,] reportStr = new string[_rows, _cells];
                //将表格数据复制到字符串二维数组
                for (int i = 0; i < _rows; i++)
                {
                    for (int j = 0; j < _cells; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null)
                            reportStr[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        else
                            reportStr[i, j] = "";
                    }
                }
                //给单元格列命名
                for (int k = 0; k < _cells; k++)
                {
                    cell[0, k].PutValue(dataGridView1.Columns[k].Name.ToString());
                }
                //给单元格赋值
                for (int i = 0; i < _rows; i++)
                {
                    for (int j = 0; j < _cells; j++)
                    {
                        cell[i + 1, j].PutValue(reportStr[i, j]);
                    }
                }
                //保存Excel表格
                try
                {
                    wb.Save(path);
                }
                catch
                {
                    MessageBox.Show("保存Excel表格失败！检查同名文件是否正在使用！", "提示");
                    return;
                }
                MessageBox.Show("保存成功！", "提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //创建并打开连接
            //conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=studentsMdb.mdb";
            conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=studentsMdb.accdb";
            conn.Open();
            comm.Connection = conn;
            //执行查询命令
            comm.CommandText = "select * from studentsTab";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //为表格控件建立数据源
            ds.Clear();
            adapter.Fill(ds, "studentsTab");
            dataGridView1.DataSource = ds.Tables["studentsTab"];
            //计算记录总数
            textBox1.Text = (dataGridView1.Rows.Count - 1).ToString();
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //初始化读写器
            de = myApi.GetSerNum(device);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！");
                return;
            }

            //将字段数据取出
            int rowIndex = dataGridView1.CurrentCellAddress.Y;
            string _XH = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString().Trim();
            string _XM = dataGridView1.Rows[rowIndex].Cells[2].Value.ToString().Trim();
            _XH = _XH.PadRight(16, ' ');
            _XM = _XM.PadRight(16, ' ');
            string bufString = _XH + _XM;

            //开始写数据
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 0xff;

            //写学号到第1块
            buf = System.Text.Encoding.Default.GetBytes(bufString);//将字符串转换成字节数组
            de = myApi.MF_Write(0, 1, 2, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("写数据失败！");
                return;
            };

            //处理uid码
            string strtemp = "";
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
            dataGridView1.Rows[rowIndex].Cells[3].Value = uidCode;
            //获取当前序号
            //string curXH = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            //构建SQL语句
            comm.CommandText = "Update studentsTab SET UID码='" + uidCode + "' Where 学号 ='" + _XH + "'";
            comm.ExecuteNonQuery();
            myApi.ControlBuzzer(5, 1, beep);
            MessageBox.Show("制卡成功！");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //初始化读写器
            de = myApi.GetSerNum(device);
            if (de != 0)
            {
                MessageBox.Show("读写器初始化失败！");
                return;
            }

            //读数据
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 1, 2, key, buf);
            if (de != 0)
            {
                myApi.ControlBuzzer(20, 1, beep);
                MessageBox.Show("读学号失败！");
                return;
            };
            //将字节数组转换成字符串
            string students = System.Text.Encoding.Default.GetString(buf);

            //显示卡信息
            listBox1.Items.Add("学号：" + students.Substring(0, 16));
            listBox1.Items.Add("姓名：" + students.Substring(16, 8));

            myApi.ControlBuzzer(5, 1, beep);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text.Trim() == "开始考勤")
            {
                button5.Text = "停止考勤";
                for (int i = 0; i < dataGridView1.Rows.Count; i++)//遍历数据表
                {
                    dataGridView1.Rows[i].Cells["出勤"].Value = "×";
                }
                //adapter.Update(ds);//刷新数据库
                textBox2.Text = "0";
                timer1.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //读学号
            key[0] = key[1] = key[2] = key[3] = key[4] = key[5] = 255;
            de = myApi.MF_Read(0, 1, 1, key, buf);
            if (de != 0) return;
            //将字节数组转换成字符串
            string xh = System.Text.Encoding.Default.GetString(buf).ToString().Trim();
            xh = xh.Substring(0, 15);
            //刷新数据
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.ToString().Trim() == xh)
                {
                    if (dataGridView1.Rows[i].Cells[4].Value.ToString().Trim() == "×")
                    {
                        dataGridView1.Rows[i].Cells[4].Value = "√";
                        textBox2.Text = (Convert.ToInt32(textBox2.Text) + 1).ToString();
                    }
                    break;
                }
            }

            myApi.ControlBuzzer(5, 1, beep);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*textBox1.Text = (dataGridView1.Rows.Count - 1).ToString();
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;*/
        }
    }
}
