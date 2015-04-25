using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Ycg.Windows;

/*
 
 * 2014-1-28 完善第一个版本
 * 
 *  1.关于KeyPress的解释
 *      在控件有焦点的情况下按下键时发生。
 *      键事件按下列顺序发生： 
            KeyDown
            KeyPress
            KeyUp

            非字符键不会引发 KeyPress 事件；但非字符键却可以引发 KeyDown 和 KeyUp 事件。
            使用 KeyChar 属性在运行时对键击进行取样，并且使用或修改公共键击的子集。
            若要仅在窗体级别处理键盘事件而不允许其他控件接收键盘事件，
 *         请将窗体的 KeyPress 事件处理方法中的 KeyPressEventArgs.Handled 属性设置为 true。
 *         
 *      摘自MSDN上的说明
 *      KeyPressEventArgs 指定在用户按键时撰写的字符。例如，当用户按 Shift + K 时，KeyChar 属性返回一个大写字母 K。
         当用户按下任意键时，发生 KeyPress 事件。与 KeyPress 事件紧密相关的两个事件为 KeyUp 和 KeyDown。
 *      当用户按下某个键时，KeyDown 事件先于每个 KeyPress 事件发生；当用户释放某个键时发生 KeyUp 事件。
 *      当用户按住某个键时，每次字符重复时，KeyDown 和 KeyPress 事件也都重复发生。一个 KeyUp 事件在释放按键时生成。

         KeyPressEventArgs 随着 KeyPress 事件的每次发生而被传递。
 *      KeyEventArgs 随着 KeyDown 和 KeyUp 事件的每次发生而被传递。
 *      KeyEventArgs 指定是否有任一个组合键（Ctrl、Shift 或 Alt）在另一个键按下的同时也曾按下。
 *      此修饰符信息也可以通过 Control 类的 ModifierKeys 属性获得。

         将 Handled 设置为 true，以取消 KeyPress 事件。这可防止控件处理按键。

         注意注意： 
         有些控件将会在 KeyDown 上处理某些击键。
 *      例如，RichTextBox 在调用 KeyPress 前处理 Enter 键。
 *      在这种情况下，您无法取消 KeyPress 事件，而是必须从 KeyDown 取消击键。
 *      
 * 
 *  2014-1-28 1:00 PM
 *      1. 完成了对组合键的监测代码，通过获取KeyState来判断是否按了组合键
 
 */

namespace Ycg.Hook
{
    public sealed class HookManager
    {
        #region Event And Field
        public event CustomKeyEventHandler KeyUp;
        public event CustomKeyEventHandler KeyDown;
        public event CustomKeyEventHandler KeyPress;

        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler LeftMouseClickUp;
        public event MouseEventHandler RightMouseClickUp;
        public event MouseEventHandler LeftMouseClickDown;
        public event MouseEventHandler RightMouseClickDown;
        public event MouseEventHandler LeftMouseDoubleClick;
        public event MouseEventHandler RightMouseDoubleClick;

        private static int _mouseHookHandle;
        private static int _keyboardHookHandlel;

        private static HookProcCallBack _mouseHookCallBack;
        private static HookProcCallBack _keyboardHookCallBack;

        private static readonly HookManager _instance = new HookManager();

        private static readonly int _currentThreadID = AppDomain.GetCurrentThreadId();
        private static readonly IntPtr _currentMoudleHandle = WindowsAPI.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
        
        #endregion

        #region Instance

        private HookManager()
        { }

        public static HookManager Instance
        {
            get { return _instance; }
        } 

        #endregion

        #region Install Hook

        /// <summary>
        /// Install the hook.
        /// </summary>
        /// <param name="installType">Select the hook install type.</param>
        public void InstallHook(HookInstallType installType = HookInstallType.MouseAndKeyBoard)
        {
            switch (installType)
            {
                case HookInstallType.Mouse:
                    this.InstallMouseHook();
                    break;
                case HookInstallType.KeyBoard:
                    this.InstallKeyBoardHook();
                    break;
                case HookInstallType.MouseAndKeyBoard:
                    this.InstallMouseHook();
                    this.InstallKeyBoardHook();
                    break;
            }
        } 

        #endregion

        #region Mouse Hook Monitor

        /// <summary>
        /// Install the mouse hook.
        /// Default install mouse global hook - [14];
        /// </summary>
        /// <param name="hookType">Select mouse hook type.</param>
        public void InstallMouseHook(HookType hookType = HookType.WH_MOUSE_LL)
        {
            if (_mouseHookHandle == default(int))
            {
                _mouseHookCallBack = new HookProcCallBack(this.MouseHookCallBack);
                if (hookType == HookType.WH_MOUSE)
                {
                    _mouseHookHandle = HookAPI.SetWindowsHookEx((int)hookType, _mouseHookCallBack, IntPtr.Zero, _currentThreadID);
                }
                else
                {
                    _mouseHookHandle = HookAPI.SetWindowsHookEx((int)hookType, _mouseHookCallBack, _currentMoudleHandle, 0);
                }
                this.CheckHandleIsZero(_mouseHookHandle);
            }
        }

        private int MouseHookCallBack(int nCode, int wParam, IntPtr lParam)
        {
            MouseButtons mouseOperation = MouseButtons.None;
            Point mousePoint = (Point)Marshal.PtrToStructure(lParam, typeof(Point));

            switch (wParam)
            {
                case (int)WindowsMessage.WM_LBUTTONDOWN:
                    mouseOperation = MouseButtons.Left;
                    this.InvokeMouseEvent(this.LeftMouseClickDown, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_LBUTTONUP:
                    mouseOperation = MouseButtons.Left;
                    this.InvokeMouseEvent(this.LeftMouseClickUp, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_LBUTTONDBLCLK:
                    mouseOperation = MouseButtons.Left;
                    this.InvokeMouseEvent(this.LeftMouseDoubleClick, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_RBUTTONDOWN:
                    mouseOperation = MouseButtons.Right;
                    this.InvokeMouseEvent(this.RightMouseClickDown, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_RBUTTONUP:
                    mouseOperation = MouseButtons.Right;
                    this.InvokeMouseEvent(this.RightMouseClickUp, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_RBUTTONDBLCLK:
                    mouseOperation = MouseButtons.Right;
                    this.InvokeMouseEvent(this.RightMouseDoubleClick, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_MOUSEMOVE:
                    this.InvokeMouseEvent(this.MouseMove, mouseOperation, mousePoint);
                    break;
                case (int)WindowsMessage.WM_MOUSEWHEEL:
                    this.InvokeMouseEvent(this.MouseWheel, mouseOperation, mousePoint);
                    break;
            }

            return HookAPI.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        private void InvokeMouseEvent(MouseEventHandler mouseEvent, MouseButtons mouseButton, Point point)
        {
            if (mouseEvent != null)
            {
                MouseEventArgs mouseArgs = new MouseEventArgs(mouseButton, 0, point.X, point.Y, 0);
                mouseEvent(this, mouseArgs);
            }
        } 

        #endregion

        #region KeyBoaed Hook Monitor

        /// <summary>
        /// Install the keyboard hook.
        /// Default install keyboard global hook - [13].
        /// </summary>
        /// <param name="hookType">Select keyboard hook type.</param>
        public void InstallKeyBoardHook(HookType hookType = HookType.WH_KEYBOARD_LL)
        {
            if (_keyboardHookHandlel == default(int))
            {
                _keyboardHookCallBack = new HookProcCallBack(this.KeyBoradHookCallBack);
                if (hookType == HookType.WH_KEYBOARD)
                {
                    _keyboardHookHandlel = HookAPI.SetWindowsHookEx((int)hookType, _keyboardHookCallBack, IntPtr.Zero, _currentThreadID);
                }
                else
                {
                    _keyboardHookHandlel = HookAPI.SetWindowsHookEx((int)hookType, _keyboardHookCallBack, _currentMoudleHandle, 0);
                }
                this.CheckHandleIsZero(_keyboardHookHandlel);
            }
        }

        private int KeyBoradHookCallBack(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                CustomKeyBoard keyInfo = (CustomKeyBoard)Marshal.PtrToStructure(lParam, typeof(CustomKeyBoard));

                if (this.KeyDown != null
                    && (wParam == (int)WindowsMessage.WM_KEYDOWN || wParam == (int)WindowsMessage.WM_SYSKEYDOWN))
                {
                    this.InvokeKeyBoardEvent(this.KeyDown, (Keys)keyInfo.VirtualKeyCode);
                }

                if (this.KeyPress != null && wParam == (int)WindowsMessage.WM_KEYDOWN)
                {
                    this.InvokeKeyBoardEvent(this.KeyPress, (Keys)keyInfo.VirtualKeyCode);
                }

                if (this.KeyUp != null
                    && (wParam == (int)WindowsMessage.WM_KEYUP || wParam == (int)WindowsMessage.WM_SYSKEYUP))
                {
                    this.InvokeKeyBoardEvent(this.KeyUp, (Keys)keyInfo.VirtualKeyCode);
                }
            }

            return HookAPI.CallNextHookEx(_keyboardHookHandlel, nCode, wParam, lParam);
        }

        private void InvokeKeyBoardEvent(CustomKeyEventHandler keyEvent, Keys keyData)
        {
            CustomKeyEventArgs customKeyArgs = new CustomKeyEventArgs(keyData);
            keyEvent(this, customKeyArgs);
        } 

        #endregion

        #region Common

        private void CheckHandleIsZero(int handle)
        {
            if (handle == 0)
            {
                int errorID = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorID);
            }
        }

        public void UninstallHook()
        {
            if (_mouseHookHandle != default(int))
            {
                if (HookAPI.UnhookWindowsHookEx(_mouseHookHandle))
                {
                    _mouseHookHandle = default(int);
                }
            }
            if (_keyboardHookHandlel != default(int))
            {
                if (HookAPI.UnhookWindowsHookEx(_keyboardHookHandlel))
                {
                    _keyboardHookHandlel = default(int);
                }
            }
        } 

        #endregion
    }
}
