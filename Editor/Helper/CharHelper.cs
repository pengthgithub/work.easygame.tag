using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

    public static class CharHelper
    {
        public static string Gb2312ToUTF8(string str)
        {
            Encoding utf8;
            Encoding gb2312;
            utf8 = Encoding.GetEncoding("UTF-8");
            gb2312 = Encoding.GetEncoding("GB2312");
            byte[] gb = gb2312.GetBytes(str);
            gb = Encoding.Convert(gb2312, utf8, gb);
            return utf8.GetString(gb);
        }

        public static string UTF8ToGb2312(string text)
        {
            byte[] bs = Encoding.GetEncoding("UTF-8").GetBytes(text);
            bs = Encoding.Convert(Encoding.GetEncoding("UTF-8"), Encoding.GetEncoding("GB2312"), bs);
            return Encoding.GetEncoding("GB2312").GetString(bs);
        }

        public static string UTF8ToDefault(string text)
        {
            byte[] bs = Encoding.Default.GetBytes(text);
            //bs = Encoding.Convert(Encoding.GetEncoding("UTF-8"), Encoding.Default, bs);
            return Encoding.Default.GetString(bs);
        }
    }
