using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

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

        
        public void WriteLine(string strMessage
            , [CallerFilePath] string strSourceFileName = ""
            , [CallerLineNumber] int nSourceFileLine = 0
            , [CallerMemberName] string strMethodName = ""
            )
        {
            try
            {
                string strSrcFileName = Path.GetFileName(strSourceFileName);
                string strLog = string.Format("{0}\t{1} ({2} line) - {3}()\t{4}\n"
                    , DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                    , strSrcFileName
                    , nSourceFileLine
                    , strMethodName
                    , strMessage
                    );
                string strFile = string.Format("{0}.log", _logname);

                File.AppendAllText(strFile, strLog);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }


    }
}
