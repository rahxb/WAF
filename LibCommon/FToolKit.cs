using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WAF.LibCommon
{
    public class FToolKit
    {

        /// <summary>
        /// メモリーストリームの内容をクリアにする
        /// </summary>
        /// <param name="mem"></param>
        static public void ClearMemoryStream(MemoryStream mem)
        {
            mem.Seek(0, SeekOrigin.Begin);
            mem.SetLength(0);
        }

    }
}
