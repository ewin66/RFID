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

namespace UNIT10
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
        int FrmHandle = 0;  //设备句柄
        int Card_Num = 0;   //询查标签的数量
        string[] EPC_Code = new string[10];//定义标签EPC码数组

        //窗体Load事件代码
        private void Form1_Load(object sender, EventArgs e)
        {
            //创建并打开连接
            conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ThingMdb.mdb";
            conn.Open();
            comm.Connection = conn;
            //执行查询命令
            comm.CommandText = "select * from ThingTab";
            adapter.SelectCommand = comm;
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //为表格控件建立数据源
            ds.Clear();
            adapter.Fill(ds, "ThingTab");
            dataGridView1.DataSource = ds.Tables["ThingTab"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开端口")
            {
                int Port = 9;
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
                }
            }
            else
            {
                myApi.CloseComPort();
                button1.Text = "打开端口";
            }            
        }

        private void button3_Click(object sender, EventArgs e)
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
                MessageBox.Show("读TID码失败！");
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

        private void button4_Click(object sender, EventArgs e)
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
                MessageBox.Show("不是单一标签，不能对一种货物初始化！");
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
                MessageBox.Show("写EPC码失败！");
                return;
            }

            MessageBox.Show("写EPC码成功！");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //询查标签
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

            listBox1.Items.Clear();
            listBox1.Items.Add("*************************");
            listBox1.Items.Add("共扫描到标签总数为：" + CardNum);
            Card_Num = CardNum;


            int j = 0;
            int k = 0;
            string TempStr;
            string EPCstr;
            int recCount = dataGridView1.RowCount;

            for (j = 0; j < CardNum; j++)
            {
                TempStr = "";
                EPCstr = "";
                for (k = 1; k < 13; k++)
                {
                    TempStr = EPClenandEPC[j * 13 + k].ToString("X2");
                    EPCstr += TempStr;
                }
                EPC_Code[j] = EPCstr;
                for (k = 0; k < recCount - 1; k++)
                {
                    string dataEPC = dataGridView1.Rows[k].Cells["EPC码"].Value.ToString().Trim();
                    if (dataEPC == EPCstr)
                    {
                        listBox1.Items.Add(dataGridView1.Rows[k].Cells["物品名称"].Value);
                    }
                }
            }
            listBox1.Items.Add("*************************");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int recCount = dataGridView1.RowCount;
            for (int k = 0; k < recCount - 1; k++)
            {
                string dataEPC = dataGridView1.Rows[k].Cells["EPC码"].Value.ToString().Trim();
                string dataCL = dataGridView1.Rows[k].Cells["存量"].Value.ToString().Trim();
                for (int j = 0; j < Card_Num; j++)
                {
                    if (dataEPC == EPC_Code[j])
                    {
                        int newCL = Convert.ToInt32(dataCL);
                        newCL--;
                        dataGridView1.Rows[k].Cells["存量"].Value = newCL + "";
                        adapter.Update(ds, "ThingTab");
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adapter.Update(ds, "ThingTab");
        }
    }
}
