using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WAF.LibCommon
{
    public class Log
    {
        private StreamWriter _stream;

        public Log(Stream s)
        {
            _stream = new StreamWriter(s);
        }

        private void Write(string strEventName, string strMessage)
        {
            string str = string.Format("{0}\t{1}\t{2}"
                , DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                , strEventName
                , strMessage
                );
            _stream.Write(str);
        }

        public void WriteLine(string strEventName, string strMessage)
        {
            Write(strEventName, strMessage);
        }

        public void Write(string strMessage)
        {
            Write("", strMessage);
        }

    }
}
