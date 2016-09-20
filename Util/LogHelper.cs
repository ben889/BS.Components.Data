using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BS.Components.Data.Util
{
    public class LogHelper
    {
        private static readonly object writeFile = new object();

        /// <summary>
        /// 在本地写入错误日志
        /// </summary>
        /// <param name="folder">绝对路径</param> 
        /// <param name="Prefix">文件前辍</param> 
        /// <param name="debugstr">内容</param> 
        public static void writeLog(string folder, string Prefix, string debugstr)
        {
            lock (writeFile)
            {
                FileStream fs = null;
                StreamWriter sw = null;

                try
                {
                    string filename = Prefix + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    fs = new FileStream(folder + "\\" + filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.WriteLine(debugstr + "\r\n");
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Dispose();
                        sw = null;
                    }
                    if (fs != null)
                    {
                        //     fs.Flush();
                        fs.Dispose();
                        fs = null;
                    }
                }
            }
        }
    }
}
