namespace Ycg.YLog
{
    public interface ILogger
    {
        /// <summary>
        /// 写入日志内容
        /// </summary>
        /// <param name="content">日志内容</param>
        void Write(string content);

        /// <summary>
        /// 写入多个日志内容
        /// </summary>
        /// <param name="contents">多个日志内容</param>
        void Write(string[] contents);
    }
}