using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace unit9
{
    public partial class myApi
    {

        //以下是超高频ISO18000-6C标准的API，需要申明函数才能使用
        [DllImport("UHFReader28.dll", EntryPoint = "OpenComPort")]
        public static extern int OpenComPort(int port, ref int comadd, int baud, ref int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "CloseComPort")]
        public static extern int CloseComPort();

        [DllImport("UHFReader28.dll", EntryPoint = "Inventory_G2")]
        public static extern short  Inventory_G2(ref int ComAdr ,int MaskMem ,ref int MaskAdr,int MaskLen,
                                              [In]byte[]  MaskData,int MaskFlag,int AdrTID,int LenTID,
                                              int TIDFlag, [In]byte[] EPClenandEPC, ref int Ant, ref int Totallen,
                                              ref int CardNum,int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "ReadData_G2")]
        public static extern short ReadData_G2(ref int ComAdr, [In]byte[] EPC, int Enum, int Mem,int WordPtr, int Num,
                                               [In]byte[] _Password, int MaskMem,ref int MaskAdr, int MaskLen, 
                                               [In]byte[] MaskData, [In]byte[] _Data ,ref int errorcode, int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "WriteData_G2")]
        public static extern short WriteData_G2(ref int ComAdr, [In]byte[] EPC,int Wnum, int Enum, int Mem, int WordPtr,
                                               [In]byte[] Writedata, [In]byte[] _Password, int MaskMem, ref int MaskAdr, int MaskLen,
                                               [In]byte[] MaskData,  ref int errorcode, int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "SetWorkMode")]
        public static extern short SetWorkMode(ref int ComAdr, int Read_mode, int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "ReadActiveModeData")]
        public static extern short ReadActiveModeData([In]byte[] ActiveModeData, ref int Datalength, int FrmHandle);

        [DllImport("UHFReader28.dll", EntryPoint = "Lock_G2")]
        public static extern short Lock_G2(ref int ComAdr, [In]byte[] EPC, int Enum, int Select, int setprotect ,
                                  [In]byte[] _Password, int MaskMem,ref int MaskAdr,int MaskLen,
                                  [In]byte[] MaskData,ref int errorcode, int FrmHandle);
 
    }
}