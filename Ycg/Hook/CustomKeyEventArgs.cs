using System;
using System.Windows.Forms;

using Ycg.Windows;

namespace Ycg.Hook
{
    public class CustomKeyEventArgs : EventArgs
    {
        private bool _alt;
        private bool _shift;
        private bool _control;
        private Keys _keyData;
        private const int KeyValue = -127;

        public CustomKeyEventArgs(Keys keyData)
        {
            this._keyData = keyData;
            this._alt = this.GetKeyState(WindowsConst.VK_MENU);
            this._shift = this.GetKeyState(WindowsConst.VK_SHIFT);
            this._control = this.GetKeyState(WindowsConst.VK_CONTROL);
        }

        public Keys Key
        {
            get { return this._keyData; }
        }

        public bool Alt
        {
            get { return this._alt; }
        }

        public bool Shift
        {
            get { return this._shift; }
        }

        public bool Control
        {
            get { return this._control; }
        }

        private bool GetKeyState(int keyConst)
        {
            return HookAPI.GetKeyState(keyConst) <= KeyValue;
        }
    }
}
