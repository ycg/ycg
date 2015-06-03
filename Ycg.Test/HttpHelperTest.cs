using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ycg.Util;

namespace Ycg.Test
{
    [TestClass()]
    public class HttpHelperTest
    {
        [TestMethod()]
        public void DownloadFileTest()
        {
            //string url = "http://img.plures.net/f1c8/9ce3/bcb8/e905/1cc0/e736/b022/161c.jpg";
            //string saveFilePath = @"C:\Users\Tim\Desktop\my.jpg";
            //HttpHelper.DownloadFile(url, saveFilePath);

            //if (File.Exists(saveFilePath))
            //{
            //    Assert.IsTrue(true);
            //}

            HttpHelper httpHelper = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "http://www.sufeinet.com/thread-12219-1-1.html",
                //Method = "post"
            };

            HttpResult result = httpHelper.GetHtml(item);

            Console.WriteLine(result.Html);
            Assert.IsTrue(true);
        }
    }
}