using System;
using System.IO;
using System.Net;
using System.Text;

namespace Ycg.Util
{
    public static class HttpHelper
    {
        public static void DownloadFile(string url, string saveFilePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    throw new NullReferenceException("The response stream is null.");
                using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create))
                {
                    byte[] bytes = new byte[1024];
                    int length = stream.Read(bytes, 0, bytes.Length);
                    while (length > 0)
                    {
                        fileStream.Write(bytes, 0, length);
                        length = stream.Read(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Post(string url, string data, int timeoutSecond = 3)
        {
            #region Create http request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 1000 * timeoutSecond;
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            #endregion

            if (request == null)
                throw new NullReferenceException("The http request is null");
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
            {
                streamWriter.Write(data);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (responseStream == null)
                    throw new NullReferenceException("The http response stream is null");
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}