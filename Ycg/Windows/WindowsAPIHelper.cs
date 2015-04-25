using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ycg.Windows
{
    public static class WindowsAPIHelper
    {
        /// <summary>
        /// Click the button.
        /// But the method only apply control class type is button.
        /// </summary>
        /// <param name="handle">Button handle.</param>
        public static void Click(IntPtr handle)
        {
            WindowsAPI.SendMessage(handle, WindowsConst.WM_LBUTTONDOWN, 0, 0);
            WindowsAPI.SendMessage(handle, WindowsConst.WM_LBUTTONUP, 0, 0);
        }

        /// <summary>
        /// Get control class name.
        /// </summary>
        /// <param name="handle">Control handle.</param>
        /// <returns>Class name.</returns>
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder stringBuilder = new StringBuilder(100);
            WindowsAPI.GetClassName(handle, stringBuilder, stringBuilder.Capacity);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get window form text.
        /// </summary>
        /// <param name="handle">Window form handle.</param>
        /// <returns>Window form text.</returns>
        public static string GetWindowText(IntPtr handle)
        {
            StringBuilder stringBuilder = new StringBuilder(100);
            WindowsAPI.GetWindowText(handle, stringBuilder, stringBuilder.Capacity);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Move the scroll bar.
        /// Apply all control scroll bar.
        /// </summary>
        /// <param name="handle">Control handle.</param>
        /// <param name="lineNumbers">Move numbers.</param>
        public static void MoveScrollbar(IntPtr handle, int lineNumbers)
        {
            #region ScrollBar消息描述
            //WM_VSCROLL - 垂直滚动条
            //WM_HSCROLL - 水平滚动条
            //SB_LINEDOWN - 移动一行
            //SB_BOTTOM - 移动到底部 
            #endregion

            for (int i = 1; i <= lineNumbers; i++)
            {
                WindowsAPI.SendMessage(handle, WindowsConst.WM_VSCROLL, WindowsConst.SB_LINEDOWN, 0);
            }
        }

        /// <summary>
        /// Set the date control value.
        /// </summary>
        /// <param name="handle">Date control handle.</param>
        /// <param name="systemtime">Date time value.</param>
        public static void SetDate(IntPtr handle, SYSTEMTIME systemtime)
        {
            int objSize = Marshal.SizeOf(typeof(SYSTEMTIME));
            byte[] buffer = new byte[objSize];
            IntPtr flagHandle = Marshal.AllocHGlobal(objSize);
            Marshal.StructureToPtr(systemtime, flagHandle, true);
            Marshal.Copy(flagHandle, buffer, 0, objSize);
            Marshal.FreeHGlobal(flagHandle);

            //①获取远程进程ID
            int processID = default(int);
            WindowsAPI.GetWindowThreadProcessId(handle, ref processID);

            //②获取远程进程句柄
            IntPtr processHandle = WindowsAPI.OpenProcess(WindowsConst.PROCESS_ALL_ACCESS, false, processID);

            //③在远程进程申请内存空间并返回内存地址
            IntPtr memoryAddress = WindowsAPI.VirtualAllocEx(processHandle, IntPtr.Zero, objSize, WindowsConst.MEM_COMMIT, WindowsConst.PAGE_READWRITE);

            //④把数据写入上一步申请的内存空间
            WindowsAPI.WriteProcessMemory(processHandle, memoryAddress, buffer, buffer.Length, 0);

            //⑤发送消息给句柄叫它更新数据
            WindowsAPI.SendMessage(handle, WindowsConst.DTM_SETSYSTEMTIME, WindowsConst.GDT_VALID, memoryAddress);

            //⑥释放内存并关闭句柄
            WindowsAPI.VirtualFreeEx(processHandle, memoryAddress, objSize, WindowsConst.MEM_RELEASE);
            WindowsAPI.CloseHandle(processHandle);
        }

        /// <summary>
        /// Expand the tree view control node.
        /// </summary>
        /// <param name="handle">Tree view handle.</param>
        /// <param name="nodeIndex">Tree view node index.</param>
        public static void ExpandTreeViewNode(IntPtr handle, int nodeIndex)
        {
            //①获取根节点
            int rootNodeNum = WindowsAPI.SendMessage(handle, WindowsConst.TVM_GETNEXTITEM, WindowsConst.TVGN_ROOT, 0);
            IntPtr rootNodeHandle = new IntPtr(rootNodeNum);

            //③遍历所有一级节点，获取我想要的节点句柄
            IntPtr selectNodeHandle = rootNodeHandle;
            for (int num = 1; num <= nodeIndex; num++)
            {
                int flagNodeNum = WindowsAPI.SendMessage(handle, WindowsConst.TVM_GETNEXTITEM, WindowsConst.TVGN_NEXT, selectNodeHandle);
                selectNodeHandle = new IntPtr(flagNodeNum);
            }

            //④展开节点
            WindowsAPI.SendMessage(handle, WindowsConst.TVM_SELECTITEM, WindowsConst.TVGN_CARET, selectNodeHandle);
            WindowsAPI.SendMessage(handle, WindowsConst.TVM_EXPAND, WindowsConst.TVE_EXPAND, selectNodeHandle);
        }

        /// <summary>
        /// Set mouse cursor to the control postion.
        /// </summary>
        /// <param name="handle">Control handle.</param>
        /// <param name="additionX">Add the X value.</param>
        /// <param name="additionY">Add the Y value.</param>
        public static void SetCursorPos(IntPtr handle, int additionX, int additionY)
        {
            Rectangle rectangle;
            WindowsAPI.GetWindowRect(handle, out rectangle);
            WindowsAPI.SetCursorPos(rectangle.Left + additionX, rectangle.Top + additionY);
        }
    }
}
