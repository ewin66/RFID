using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace UNIT7
{
    public partial class myApi
    {

        //以下是二代身份证的API，需要申明函数才能使用
        //打开端口函数
        [DllImport("sdtapi.dll", EntryPoint = "SDT_OpenPort")]
        public static extern int SDT_OpenPort(int INint);
        //关闭端口函数
        [DllImport("sdtapi.dll", EntryPoint = "SDT_ClosePort")]
        public static extern int SDT_ClosePort(int INint);
        //寻卡函数
        [DllImport("sdtapi.dll", EntryPoint = "SDT_StartFindIDCard")]
        public static extern int SDT_StartFindIDCard(int INint1, [In]byte[] buf, int INint2);
        //选卡函数
        [DllImport("sdtapi.dll", EntryPoint = "SDT_SelectIDCard")]
        public static extern int SDT_SelectIDCard(int INint1, [In]byte[] buf, int INint2);
        //读基本信息函数
        [DllImport("sdtapi.dll", EntryPoint = "SDT_ReadBaseMsg")]
        public static extern int SDT_ReadBaseMsg(int INint1, [In]byte[] buf1, ref int INint2, [In]byte[] buf2, ref int INint3, int INint4);
        //
        [DllImport("sdtapi.dll", EntryPoint = "SDT_GetSAMIDToStr")]
        public static extern int SDT_GetSAMIDToStr(int INint1, [In]byte[] buf, int INint2);
        //读照片函数
        [DllImport("WltRS.dll", EntryPoint = "GetBmp")]
        public static extern int GetBmp([In]byte[] buf, int INint);

        //以下是高频ISO14443A标准的API，需要申明函数才能使用
        [DllImport("function.dll", EntryPoint = "GetSerNum")]
        public static extern int GetSerNum([In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "ControlBuzzer")]
        public static extern int ControlBuzzer(int freq, int duration, [In]byte[] beep);

        [DllImport("function.dll", EntryPoint = "MF_Read")]
        public static extern int MF_Read(int mode, int blk, int num, [In]byte[] snr, [In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "MF_Write")]
        public static extern int MF_Write(int mode, int blk, int num, [In]byte[] snr, [In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "MF_InitValue")]
        public static extern int MF_InitValue(int mode, int blk, [In]byte[] snr, [In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "MF_Dec")]
        public static extern int MF_Dec(int mode, int blk, [In]byte[] snr, [In]byte[] buf);

        [DllImport("function.dll", EntryPoint = "MF_Inc")]
        public static extern int MF_Inc(int mode, int blk, [In]byte[] snr, [In]byte[] buf);

        //以下是高频ISO15693标准的API，需要申明函数才能使用
        [DllImport("function.dll", EntryPoint = "ISO15693_Inventory")]
        public static extern int ISO15693_Inventory([In]byte[] Num, [In]byte[] buf);

    }
}