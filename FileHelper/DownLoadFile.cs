using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public class DownLoadFile
    {
        public bool DownLoad(string url, string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
            request.Timeout = 5000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (!Directory.Exists(Path.GetDirectoryName(path)))//如果不存在就创建文件夹
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                if (File.Exists(Path.GetFullPath(path)))
                {
                    stream.Close();
                    return false;
                }
                StreamWriter sw = new StreamWriter(path);
                stream.CopyTo(sw.BaseStream);

                sw.Flush();
                sw.Close();

                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
