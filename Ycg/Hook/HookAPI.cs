using System;
using System.Runtime.InteropServices;

namespace Ycg.Hook
{
    public static class HookAPI
    {
        /// <summary>
        /// 安装勾子
        /// </summary>
        /// <param name="idHook">钩子类型，此处用整形的枚举表示</param>
        /// <param name="hookCallBack">钩子发挥作用时的回调函数</param>
        /// <param name="moudleHandle">应用程序实例的模块句柄(一般来说是你钩子回调函数所在的应用程序实例模块句柄)</param>
        /// <param name="threadID">与安装的钩子子程相关联的线程的标识符
        /// <remarks>如果线程ID是0则针对系统级别的，否则是针对当前线程</remarks>
        /// </param>
        /// <returns>返回钩子句柄</returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProcCallBack hookCallBack, IntPtr moudleHandle, int threadID);


        /// <summary>
        /// 卸载勾子
        /// </summary>
        /// <param name="handle">要取消的钩子的句柄</param>
        /// <returns>卸载钩子是否成功</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int handle);

        /// <summary>
        /// 调用下一个钩子
        /// </summary>
        /// <param name="handle">当前钩子的句柄</param>
        /// <param name="nCode">钩子链传回的参数，非0表示要丢弃这条消息，0表示继续调用钩子</param>
        /// <param name="wParam">传递的参数</param>
        /// <param name="lParam">传递的参数</param>
        /// <returns>返回的句柄</returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int handle, int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 判断连续两次鼠标单击之间会被处理成双击事件的间隔时间
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetDoubleClickTime();

        /// <summary>
        /// 根据当前的扫描码和键盘信息，将一个虚拟键转换成ASCII字符
        /// </summary>
        /// <param name="uVirtKey"></param>
        /// <param name="uScanCode"></param>
        /// <param name="lpbKeyState"></param>
        /// <param name="lpwTransKey"></param>
        /// <param name="fuState"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        /// <summary>
        /// 取得键盘上每个虚拟键当前的状态
        /// </summary>
        /// <param name="pbKeyState"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        /// 针对已处理过的按键，在最近一次输入信息时，判断指定虚拟键的状态
        /// </summary>
        /// <param name="vKey">虚拟键盘常量</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);
    }
}
