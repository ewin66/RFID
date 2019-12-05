using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using unit9;


namespace UNIT9
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
        int FrmHandle = 0;//读写器设备句柄
        

        private void Form1_Load(object sender, EventArgs e)
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开端口")
            {
                int Port = 1;
                int ComAdr = 0XFF;
                int Baud = 5;

                int st = myApi.OpenComPort(Port, ref ComAdr, Baud, ref FrmHandle);
                if (st != 0)
                {
                    MessageBox.Show("通讯口初始化失败！");
                    return;
                }
                else
                {
                    button1.Text = "关闭端口";
                    MessageBox.Show("通讯口初始化成功！" + FrmHandle);
                    button4.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;
                    button7.Enabled = true;
                    button8.Enabled = true;
                    button9.Enabled = true;
                }
            }
            else
            {
                myApi.CloseComPort();
                button1.Text = "打开端口";
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
            }                      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            adapter.Update(ds, "studentsTab");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //定义需要传递的变量
            int ComAdr = 0;
            int MaskMem = 0;
            int MaskAdr = 0;
            int MaskLen = 0;
            byte[] MaskData = new byte[8];
            //int MaskData = 0;
            int MaskFlag = 0;
            int AdrTID = 0;
            int LenTID = 4;
            int TIDFlag = 0;
            byte[] EPClenandEPC = new byte[300];
            int Ant = 1;
            int Totallen = 0;
            int CardNum = 0;
            //询查标签
            short st = myApi.Inventory_G2(ref ComAdr, MaskMem, ref MaskAdr, MaskLen,
                                                MaskData, MaskFlag, AdrTID, LenTID,
                                                TIDFlag, EPClenandEPC, ref Ant, ref Totallen,
                                                ref CardNum, FrmHandle);
            if (st < 1 | st > 4)
            {
                MessageBox.Show("询查标签失败！");
                return;
            }

            //判断是不是单一标签
            if (CardNum != 1)
            {
                MessageBox.Show("不是单一标签，不能对一种货物初始化！");
                return;
            }

            //取出EPC码
            string TempStr = "";
            string EPCstr = "";
            for (int k = 1; k < 13; k++)
            {
                TempStr = EPClenandEPC[k].ToString("X2");
                EPCstr += TempStr;
            }
            dataGridView1.CurrentRow.Cells["EPC码"].Value = EPCstr;

            //读TID码
            ComAdr = 0;
            byte[] EPC = new byte[12];
            for (int j = 0; j < 12; j++)
            {
                EPC[j] = EPClenandEPC[j + 1];
            }
            int Enum = 6;
            int Mem = 2;
            int WordPtr = 2;
            int Num = 8;
            byte[] _Password = new byte[4];
            _Password[0] = _Password[1] = _Password[2] = _Password[3] = 0;
            MaskMem = 0;
            MaskAdr = 0;
            MaskLen = 0;
            byte[] _Data = new byte[16];
            int errorcode = 0;

            st = myApi.ReadData_G2(ref ComAdr, EPC, Enum, Mem, WordPtr, Num,
                                    _Password, MaskMem, ref MaskAdr, MaskLen,
                                    MaskData, _Data, ref errorcode, FrmHandle);
            if (st != 0)
            {
                MessageBox.Show("读TID码失败！" + st);
                return;
            }

            //取出TID码
            TempStr = "";
            string TIDstr = "";
            for (int k = 0; k < 8; k++)
            {
                TempStr = _Data[k].ToString("X2");
                TIDstr += TempStr;
            }
            dataGridView1.CurrentRow.Cells["TID码"].Value = TIDstr;
            MessageBox.Show("读TID码成功！");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //判断EPC码的合法性
            string EPC_BAK = dataGridView1.CurrentRow.Cells["EPC码"].Value.ToString().Trim();
            if (EPC_BAK.Length != 24)
            {
                MessageBox.Show("输入的EPC长度不是24！");
                return;
            }

            string char_set = "0123456789abcdefABCDEF";
            for (int k = 0; k < 24; k++)
            {
                string _char = EPC_BAK.Substring(k, 1);
                if (char_set.IndexOf(_char) < 0)
                {
                    MessageBox.Show("输入非法字符！");
                    return;
                }
            }

            //构建EPC字符串
            byte[] epc16 = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                byte epc10 = Convert.ToByte(EPC_BAK.Substring(i * 2, 2), 16);
                epc16[i] = epc10;
            }

            //询查标签
            //定义需要传递的变量
            int ComAdr = 0;
            int MaskMem = 0;
            int MaskAdr = 0;
            int MaskLen = 0;
            byte[] MaskData = new byte[8];
            //int MaskData = 0;
            int MaskFlag = 0;
            int AdrTID = 0;
            int LenTID = 4;
            int TIDFlag = 0;
            byte[] EPClenandEPC = new byte[300];
            int Ant = 1;
            int Totallen = 0;
            int CardNum = 0;
            //询查标签
            short st = myApi.Inventory_G2(ref ComAdr, MaskMem, ref MaskAdr, MaskLen,
                                                MaskData, MaskFlag, AdrTID, LenTID,
                                                TIDFlag, EPClenandEPC, ref Ant, ref Totallen,
                                                ref CardNum, FrmHandle);
            if (st < 1 | st > 4)
            {
                MessageBox.Show("询查标签失败！");
                return;
            }

            //判断是不是单一标签
            if (CardNum != 1)
            {
                MessageBox.Show("不是单一标签，不能对同一人初始化！");
                return;
            }

            //写EPC码
            //定义需要传递的变量
            ComAdr = 0;
            byte[] EPC = new byte[12];
            for (int j = 0; j < 12; j++)
            {
                EPC[j] = EPClenandEPC[j + 1];
            }
            int Wnum = 6;
            int Enum = 6;
            int Mem = 1;
            int WordPtr = 2;
            byte[] Writedata = epc16;
            byte[] _Password = new byte[4];
            _Password[0] = _Password[1] = _Password[2] = _Password[3] = 0;
            MaskMem = 0;
            MaskAdr = 0;
            MaskLen = 0;
            //byte[] MaskData = new byte[8];
            int errorcode = 0;
            st = myApi.WriteData_G2(ref ComAdr, EPC, Wnum, Enum,
                                   Mem, WordPtr, Writedata, _Password,
                                   MaskMem, ref MaskAdr, MaskLen,
                                   MaskData, ref errorcode, FrmHandle);
            if (st != 0)
            {
                MessageBox.Show("写EPC码失败！" + st);
                return;
            }

            MessageBox.Show("写EPC码成功！");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //定义需要传递的变量
            int ComAdr = 0;
            int MaskMem = 0;
            int MaskAdr = 0;
            int MaskLen = 0;
            byte[] MaskData = new byte[8];
            //int MaskData = 0;
            int MaskFlag = 0;
            int AdrTID = 0;
            int LenTID = 4;
            int TIDFlag = 0;
            byte[] EPClenandEPC = new byte[300];
            int Ant = 1;
            int Totallen = 0;
            int CardNum = 0;
            //询查标签
            short st = myApi.Inventory_G2(ref ComAdr, MaskMem, ref MaskAdr, MaskLen,
                                                MaskData, MaskFlag, AdrTID, LenTID,
                                                TIDFlag, EPClenandEPC, ref Ant, ref Totallen,
                                                ref CardNum, FrmHandle);
            if (st < 1 | st > 4)
            {
                MessageBox.Show("询查标签失败！");
                return;
            }

            //判断是不是单一标签
            if (CardNum != 1)
            {
                MessageBox.Show("不是单一标签，不能多个电子标签进行永久锁定！");
                return;
            }

            //定义变量，开始锁定标签EPC
            ComAdr = 0;
            byte[] EPC = new byte[12];
            for (int j = 0; j < 12; j++)
            {
                EPC[j] = EPClenandEPC[j + 1];
            }
            int Enum = 6;
            int Select = 2;
            int setprotect = 3;
            byte[] _Password = new byte[4];
            _Password[0] = _Password[1] = _Password[2] = _Password[3] = 0;
            MaskMem = 0;
            MaskAdr = 0;
            MaskLen = 0;
            //byte[] MaskData = new byte[16];
            int errorcode = 0;

            st = myApi.Lock_G2(ref  ComAdr, EPC, Enum, Select, setprotect,
                                  _Password, MaskMem, ref  MaskAdr, MaskLen,
                                   MaskData, ref  errorcode, FrmHandle);
            if (st != 0)
            {
                MessageBox.Show("永久锁定EPC码失败！");
                return;
            }
            MessageBox.Show("永久锁定EPC码成功！");
  
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int ComAdr = 0;
            int Read_mode = 1;
            int st = myApi.SetWorkMode(ref ComAdr, Read_mode, FrmHandle);
            if (st != 0)
            {
                MessageBox.Show("设置读写器模式失败！");
                return;
            }
            MessageBox.Show("设置读写器模式成功！");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.Text == "接收数据")
            {
                button8.Text = "停止接收";
                timer1.Enabled = true;
            }
            else
            {
                button8.Text = "接收数据";
                timer1.Enabled = false;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int ComAdr = 0;
            int Read_mode = 0;
            int st = myApi.SetWorkMode(ref ComAdr, Read_mode, FrmHandle);
            if (st != 0)
            {
                MessageBox.Show("设置读写器模式失败！");
                return;
            }
            MessageBox.Show("设置读写器模式成功！");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] ActiveModeData = new byte[3000];
            int Datalength = 0;
            int st = myApi.ReadActiveModeData(ActiveModeData, ref Datalength, FrmHandle);

            //计算标签数量
            int Num = Datalength / 30;
            if (Num < 1) return;
            string TempStr;
            string EPCstr;
            int k, j, i;
            string _XM;
            for (k = 0; k < Num; k++)
            {
                //取出EPC码
                TempStr = "";
                EPCstr = "";
                for (j = 1; j < 9; j++)
                {
                    TempStr = ActiveModeData[k * 30 + j + 15].ToString("X2");
                    EPCstr += TempStr;
                }

                //label1.Text = EPCstr;
                //到数据库查找对应的EPC码匹配记录
                string s1;
                for (i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    s1 = dataGridView1.Rows[i].Cells["EPC码"].Value.ToString().Trim();
                    if (s1.IndexOf(EPCstr) >= 0)
                    {
                        _XM = dataGridView1.Rows[i].Cells["姓名"].Value.ToString();
                        label1.Text = "欢迎" + _XM + "同学参加会议！";
                        dataGridView1.Rows[i].Cells["状态"].Value = "1";
                        break;
                    }
                }
            }
        }
    }
}
