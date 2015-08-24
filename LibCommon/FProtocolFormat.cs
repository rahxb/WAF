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
        /// コマンド名とパラメータ(複数)を格納するクラス
        /// </summary>
        public class CommandAndParams
        {
            public string CommandName { get; set; }
            public Dictionary<string, string> Params { get; set; }
        }

        /// <summary>
        /// コマンド名とパラメータを分割して返す
        /// </summary>
        /// <param name="strReceivedData"></param>
        /// <returns></returns>
        static public CommandAndParams GetCommandParams(string strReceivedData)
        {
            CommandAndParams result = new CommandAndParams();

            strReceivedData = strReceivedData.Replace("\r\n", "");
            string[] arNameAndValue = strReceivedData.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (1 <= arNameAndValue.Length)
                // コマンド名は大小文字の区別をなくすため、強制的に大文字にする
                result.CommandName = arNameAndValue[0].ToUpper();

            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            for (int i = 1; i < arNameAndValue.Length; i++)
            {
                // ”パラメータ名:パラメータ値”の区切りを分割する
                string[] arParamNameAndValues = arNameAndValue[i].Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                // パラメータ名は大小文字の区別をなくすため、強制的に大文字にする
                string strParamName = arParamNameAndValues[0].ToUpper();

                if (1 == arParamNameAndValues.Length)
                {
                    // パラメータ値なしは、
                    // パラメータ名のみパラメータ辞書に追加する
                    dicParams.Add(strParamName, "");
                }
                else if (2 <= arParamNameAndValues.Length)
                {
                    // パラメータをBase64デコードして、パラメータ辞書に追加する
                    dicParams.Add(strParamName, FString.FromBase64(arParamNameAndValues[1]));
                }
            }
            result.Params = dicParams;

            return result;
        }

        /// <summary>
        /// コマンド送信用フォーマット(全員にメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <returns></returns>
        static public string ClientMessage(string strMessae)
        {
            return string.Format("{0}\t" 
                + "MESSAGE:{1}"

                , "PUBLIC-MESSAGE"              // COMMAND
                , FString.ToBase64(strMessae)   // MESSAGE
                );
        }

        /// <summary>
        /// コマンド送信用フォーマット(全員にメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <param name="strFrom"></param>
        /// <returns></returns>
        static public string ServerMessage(string strMessae, string strFromName)
        {
            return string.Format("{0}\t"
                + "MESSAGE:{1}\t" 
                + "FROM-NAME:{2}"

                , "PUBLIC-MESSAGE"              // COMMAND
                , FString.ToBase64(strMessae)   // MESSAGE
                , FString.ToBase64(strFromName) // FROM-NAME
                );
        }

        /// <summary>
        /// コマンド送信用フォーマット(特定の相手(単数)にのみメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <param name="strToName"></param>
        /// <returns></returns>
        static public string ClientMessage(string strMessae, string strToName)
        {
            return string.Format("{0}\t" 
                + "MESSAGE:{1}\t" 
                + "FROM-NAME:{2}"
                
                , "PRIVATE-MESSAGE"             // COMMAND
                , FString.ToBase64(strMessae)   // MESSAGE
                , FString.ToBase64(strToName)   // FROM-NAME
                );
        }

        /// <summary>
        /// コマンド送信用フォーマット(特定の相手(単数)にのみメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <param name="strFromName"></param>
        /// <param name="strToName"></param>
        /// <returns></returns>
        static public string ServerMessage(string strMessae, string strFromName, string strToName)
        {
            return string.Format("{0}\t" 
                + "MESSAGE:{1}\t" 
                + "FROM-NAME:{2}\t" 
                + "TO-NAME:{3}"
                
                , "PRIVATE-MESSAGE"             // COMMAND
                , strMessae                     // MESSAGE
                , FString.ToBase64(strFromName) // FROM-NAME
                , FString.ToBase64(strToName)   // TO-NAME
                );
        }

    }
}
