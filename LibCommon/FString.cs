using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;

namespace WAF.LibCommon
{
    public class FString
    {

        /// <summary>
        /// プレーン文字列をBase64文字列に変換する
        /// </summary>
        /// <param name="strPlainText"></param>
        /// <returns></returns>
        static public string ToBase64(string strPlainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strPlainText));
        }

        /// <summary>
        /// Base64文字列をプレーン文字列に変換する
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        static public string FromBase64(string strBase64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(strBase64));
        }


        /// <summary>
        /// バイト配列から文字列に変換する(UTF8)
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        static public string DataToString(byte[] bin)
        {
            return Encoding.UTF8.GetString(bin);
        }

        /// <summary>
        /// 文字列からバイト配列に変換する(UTF8)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public byte[] DataToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

    }
}
