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
    }
}