using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsHelper
{
    public class MouseHook
    {

        //鼠标事件常量  
        public enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        //键盘事件常量  
        public enum KeyEventFlag : int
        {
            Down = 0x0000,
            Up = 0x0002,
        }

        //鼠标事件函数  
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void mouse_event(MouseEventFlag dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //鼠标移动函数  
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(int x, int y);

        //键盘事件函数  
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk, byte bScan, KeyEventFlag dwFlags, int dwExtraInfo);

        //定时器  
        private System.Timers.Timer atimer = new System.Timers.Timer();

        //自动释放键值  
        private byte vbk;

        //初始化  
        public MouseHook()
        {

            //设置定时器事件  
            this.atimer.Elapsed += new ElapsedEventHandler(atimer_Elapsed);
            this.atimer.AutoReset = true;
        }


        //鼠标操作 _dx,_dy 是鼠标距离当前位置的二维移动向量  
        public void mouse(MouseEventFlag _dwFlags, int _dx, int _dy)
        {
            mouse_event(_dwFlags, _dx, _dy, 0, 0);
        }

        //鼠标操作  
        public void mouse_click(string button = "L", bool is_double = false)

        {
            switch (button)
            {
                case "L":
                    mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, 0);
                    if (is_double)
                    {
                        mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, 0);
                    }
                    break;
                case "R":
                    mouse_event(MouseEventFlag.RightDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.RightUp, 0, 0, 0, 0);
                    if (is_double)
                    {
                        mouse_event(MouseEventFlag.RightDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.RightUp, 0, 0, 0, 0);
                    }
                    break;
                case "M":
                    mouse_event(MouseEventFlag.MiddleDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.MiddleUp, 0, 0, 0, 0);
                    if (is_double)
                    {
                        mouse_event(MouseEventFlag.MiddleDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.MiddleUp, 0, 0, 0, 0);
                    }
                    break;
            }
        }

        //鼠标移动到 指定位置(_dx,_dy)  
        public void mouse_move(int _dx, int _dy)
        {
            SetCursorPos(_dx, _dy);
        }

        //键盘操作  
        public void keybd(byte _bVk, KeyEventFlag _dwFlags)
        {
            keybd_event(_bVk, 0, _dwFlags, 0);
        }

        //键盘操作 带自动释放 dwFlags_time 单位:毫秒  
        public void keybd(byte __bVk, int dwFlags_time = 100)
        {

            this.vbk = __bVk;
            //设置定时器间隔时间  
            this.atimer.Interval = dwFlags_time;
            keybd(this.vbk, KeyEventFlag.Down);
            this.atimer.Enabled = true;
        }

        //键盘操作 组合键 带释放  
        public void keybd(byte[] _bVk)
        {
            if (_bVk.Length >= 2)
            {
                //按下所有键  
                foreach (byte __bVk in _bVk)
                {
                    keybd(__bVk, KeyEventFlag.Down);
                }
                //反转按键排序  
                _bVk = _bVk.Reverse().ToArray();

                //松开所有键  
                foreach (byte __bVk in _bVk)
                {
                    keybd(__bVk, KeyEventFlag.Up);
                }
            }
        }

        void atimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.atimer.Enabled = false;

            //释放按键  
            keybd(vbk, KeyEventFlag.Up);
        }
    }
}
