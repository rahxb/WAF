using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAF.LibCommon
{
    public class FProtocolFormat
    {

        /// <summary>
        /// コマンド送信用フォーマット(全員にメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <returns></returns>
        static public string Message(string strMessae)
        {
            return string.Format("{0}\tMESSAGE:{1}", "PUBLIC-MESSAGE", FString.ToBase64(strMessae));
        }

        /// <summary>
        /// コマンド送信用フォーマット(特定の相手(単数)にのみメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <param name="strTargetName"></param>
        /// <returns></returns>
        static public string Message(string strMessae, string strTargetName)
        {
            return string.Format("{0}\tMESSAGE:{1}\tTARGET-NAME:{2}", "PRIVATE-MESSAGE", strMessae, FString.ToBase64(strTargetName));
        }

    }
}
