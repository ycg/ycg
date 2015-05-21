using System;
using System.IO;
using System.Text;

namespace Ycg.YLog.Target
{
    public class TextLogger : ILogger
    {
        private long _fileSize;
        private string _fileName;
        private string _filePath;
        private string _fileDirectory;

        private const long DefaultFileSize = 1024 * 50;
        private const string DefaultDirectory = @"C:\Log";

        public TextLogger()
            : this(DefaultDirectory)
        {
        }

        public TextLogger(string fileDirectory)
            : this(fileDirectory, DefaultFileSize)
        {
        }

        public TextLogger(string fileDirectory, long fileSize)
        {
            this._fileSize = fileSize;
            this._fileDirectory = fileDirectory;

            if (!Directory.Exists(this._fileDirectory))
            {
                Directory.CreateDirectory(this._fileDirectory);
            }
        }

        public long FileSize
        {
            get { return _fileSize; }
            private set { _fileSize = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            private set { _filePath = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            private set { _fileName = value; }
        }

        public string FileDirectory
        {
            get { return _fileDirectory; }
            private set { this._fileDirectory = value; }
        }

        public void Write(string content)
        {
            if (content == null)
                throw new ArgumentNullException();
            if (content.TrimStart().TrimEnd() == string.Empty)
                return;

            if (this._fileSize > 1000)
            {
                //TO DO 获取新的文件名
                this._fileName = this.GetFileName();
                this._filePath = Path.Combine(this._fileDirectory, this._fileName);
            }

            using (StreamWriter writer = File.AppendText(this._filePath))
            {
                writer.Write(content);
            }
        }

        public void Write(string[] contents)
        {
            if (contents == null)
                throw new ArgumentNullException();
            if (contents.Length == 0)
                return;

            StringBuilder stringBuilder = new StringBuilder(1000);

            foreach (string content in contents)
            {
                stringBuilder.Append(content + Environment.NewLine);
            }

            this.Write(stringBuilder.ToString());
        }

        private string GetFileName()
        {
            //这边会有文件命名代码规则
            // 日期 + 日志类型 + (在同一天相同类型日志被拆分了)
            //每天有每天的日志
            string date = DateTime.Now.ToString("yyyyMMdd");
            return string.Empty;
        }
    }
}