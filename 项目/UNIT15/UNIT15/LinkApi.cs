using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace UNIT15
{
    public partial class myApi
    {
        //以下是PDF417动态库连接函数申明
        [DllImport("EnCodePdf.dll", CharSet = CharSet.Auto)]
        public static extern void SetPdfConFile(IntPtr strConFileName);

        [DllImport("EnCodePdf.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static extern string EnPdfText(byte[] strInfo, IntPtr barFile);

        //以下是DataMatrix动态库连接函数申明
        [DllImport("EnDataMatrix.dll", CharSet = CharSet.Auto)]
        public static extern void SetDmConFile(IntPtr strConFileName);

        [DllImport("EnDataMatrix.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static extern string EnDmText(byte[] strInfo, IntPtr barFile);
    }
}