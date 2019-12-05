using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNIT16
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String _STR;
            Image BARfile;
            _STR = textBox1.Text.ToString().Trim();
            if (_STR.Length < 1)
            {
                MessageBox.Show("您还没输入数字字符!");
                return;
            }
            string D = "0123456789";
            for (int k = 0; k < _STR.Length; k++)
            {
                string subD = _STR.Substring(k, 1);
                if (D.IndexOf(subD) < 0)
                {
                    MessageBox.Show("输入的内容含有非法字符!" + subD);
                    return;
                }
            }
            BarcodeLib.Barcode barCode = new BarcodeLib.Barcode();
            BARfile = barCode.Encode(BarcodeLib.TYPE.CODE128, _STR, 300, 100);
            pictureBox1.Image = BARfile;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("barFile.jpg");

            PrintDocument printDocument = new PrintDocument();

            //设置文档名
            printDocument.DocumentName = "条形码打印";//设置完后可在打印对话框及队列中显示（默认显示document）

            //设置纸张大小（可以不设置，取默认设置）
            PaperSize ps = new PaperSize("Your Paper Name", 200, 100);
            ps.RawKind = 150; //如果是自定义纸张，就要大于118，（A4值为9，详细纸张类型与值的对照请看http://msdn.microsoft.com/zh-tw/library/system.drawing.printing.papersize.rawkind(v=vs.85).aspx）
            printDocument.DefaultPageSettings.PaperSize = ps;

            //打印输出（过程）
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            //跳出打印对话框，提供打印参数可视化设置，如选择哪个打印机打印此文档等
            PrintDialog pd = new PrintDialog();
            pd.Document = printDocument;
            if (DialogResult.OK == pd.ShowDialog()) //如果确认，将会覆盖所有的打印参数设置
            {
                //页面设置对话框（可以不使用，其实PrintDialog对话框已提供页面设置）
                PageSetupDialog psd = new PageSetupDialog();
                psd.Document = printDocument;
                if (DialogResult.OK == psd.ShowDialog())
                {
                    //打印预览
                    PrintPreviewDialog ppd = new PrintPreviewDialog();
                    ppd.Document = printDocument;
                    if (DialogResult.OK == ppd.ShowDialog())
                        printDocument.Print(); //打印
                }
            }
        }

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            //打印啥东东就在这写了

            int x = 200;
            int y = 100;

            Rectangle destRect = new Rectangle(0, 0, x, y);//背景图片打印区域
            Image layoutImage = Image.FromFile("barFile.jpg"); //图片文件路径（当前文件夹）

            //layoutImage.Width, layoutImage.Height获取图片的长和宽
            e.Graphics.DrawImage(layoutImage, destRect, 0, 0, layoutImage.Width, layoutImage.Height, System.Drawing.GraphicsUnit.Pixel);
        }
    }
}
