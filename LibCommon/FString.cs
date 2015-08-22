using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAF.LibCommon
{
    public class FString
    {

        /// <summary>
        /// バイト配列から文字列に変換する(UTF8)
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        static public string DataToString(byte[] bin)
        {
            return System.Text.Encoding.UTF8.GetString(bin);
        }

        /// <summary>
        /// 文字列からバイト配列に変換する(UTF8)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public byte[] DataToByteArray(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

    }
}
