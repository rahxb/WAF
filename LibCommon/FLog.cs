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
        private string _logname = "untitled";

        public Log(string logname)
        {
            if (string.IsNullOrWhiteSpace(logname) == false)
                _logname = logname;
        }

        private void Write(string strEventName, string strMessage)
        {
            try
            {
                string str = string.Format("{0}\t{1}\t{2}"
                    , DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                    , strEventName
                    , strMessage
                    );
                File.AppendText(str);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public void WriteLine(string strEventName, string strMessage)
        {
            Write(strEventName, strMessage + "\r\n");
        }

        public void Write(string strMessage)
        {
            Write("", strMessage + "\r\n");
        }

    }
}
