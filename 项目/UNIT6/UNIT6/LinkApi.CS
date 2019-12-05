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

namespace UNIT6
{
    public partial class myApi
    {

        //以下是高频ISO14443A标准的API，需要申明函数才能使用
        [DllImport("function.dll", EntryPoint = "GetSerNum")]
        public static extern int GetSerNum([In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "ControlBuzzer")]
        public static extern int ControlBuzzer(int freq, int duration, [In]byte[] beep);

        [DllImport("function.dll", EntryPoint = "MF_Read")]
        public static extern int MF_Read(int mode, int blk, int num, [In]byte[] snr, [In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "MF_Write")]
        public static extern int MF_Write(int mode, int blk, int num, [In]byte[] snr, [In]byte[] buf);

    }
}
