using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WiimoteLib;
using System.Runtime.InteropServices;
namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Controle_Conectar.xaml
    /// </summary>
    public partial class Controle_Conectar : Window
    {   
        [DllImport("user32.dll")]
        extern static uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);       
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        public class win32api
        {
            [DllImport("user32.dll")]
            public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        }

        Boolean isADown = false;
        Boolean isBDown = false;
        Boolean isHomeDown = false;
        Boolean isMinusDown = false;
        Boolean isPlusDown = false;
        Boolean isUpDown = false;
        Boolean isDownDown = false;
        Boolean isLeftDown = false;
        Boolean isRightDown = false;
        Boolean isOneDown = false;
        Boolean isTwoDown = false;
        Byte VK_UP = 0x26;
        Byte VK_DOWN = 0x28;
        Byte VK_LEFT = 0x25;
        Byte VK_RIGHT = 0x27;
        Byte VK_ENTER = 0x0D;
        Byte VK_BS = 0x08;
        Byte VK_WINDOWS = 0x5B;
        Byte VK_ALT = 0xA4;
        Byte VK_TAB = 0x09;
        Wiimote wm = new Wiimote();
        public Controle_Conectar()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            wm.Connect();                                        
            wm.WiimoteChanged += wm_WiimoteChanged;              
            wm.SetReportType(InputReport.IRExtensionAccel, true);
            wm.InitializeMotionPlus();
            wm.SetLEDs(1);
            this.Hide();
        }
      
        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            WiiControl(args.WiimoteState);
        }


        private void GyroMouse(WiimoteState ws)
        {

            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X + getGyroX(ws), System.Windows.Forms.Cursor.Position.Y + getGyroZ(ws));
        }



        private int getGyroX(WiimoteState ws)
        {

            int rawValue = 7859 - ws.MotionPlusState.RawValues.X;
            int sign = rawValue < 0 ? -1 : 1;

            return sign * (int)(120.0 * Math.Pow((double)Math.Abs(rawValue) / 7859.0, 1.3));
        }



        const int GYRO_NEUTRAL_Y = 7893;
        private int getGyroY(WiimoteState ws)
        {
            int rawValue = 7893 - ws.MotionPlusState.RawValues.Y;
            int sign = rawValue < 0 ? 1 : -1;

            return sign * (int)(120.0 * Math.Pow((double)Math.Abs(rawValue) / 7893.0, 1.3));
        }


        const int GYRO_NEUTRAL_Z = 7825;
        private int getGyroZ(WiimoteState ws)
        {
            int rawValue = 7825 - ws.MotionPlusState.RawValues.Z;
            int sign = rawValue < 0 ? 1 : -1;

            return sign * (int)(120.0 * Math.Pow((double)Math.Abs(rawValue) / 7825.0, 1.3));
        }



        public void WiiControl(WiimoteState ws)
        {
            INPUT[] input = new INPUT[1];                             
            if (ws.ButtonState.B)
                GyroMouse(ws);
            if (ws.ButtonState.A)
            {
              
                if (!isADown)
                {
                    input[0].mi.dwFlags = 0x0002;                       
                    SendInput(1, input, Marshal.SizeOf(input[0]));   
                    isADown = true;
                 
                }
            }
            else
            {
                if (isADown)
                {
                    
                    isADown = false;

                    input[0].mi.dwFlags = 0x0004;                     
                    SendInput(1, input, Marshal.SizeOf(input[0]));    
                    
                }
            }

           
            if (ws.ButtonState.One)
            {
               

                if (!isOneDown)
                {

                    input[0].mi.dwFlags = 0x0008;                    
                    SendInput(1, input, Marshal.SizeOf(input[0]));  

                    isOneDown = true;
                }
            }
            else
            {
                if (isOneDown)
                {
                    isOneDown = false;
                    input[0].mi.dwFlags = 0x0010;                    
                    SendInput(1, input, Marshal.SizeOf(input[0]));   
                  
                }
            }
            if (ws.ButtonState.Up)
            {

                if (!isUpDown)
                {
                 
                    win32api.keybd_event(VK_UP, 0, 0, (UIntPtr)0);
                    isUpDown = true;
                  
                }
            }
            else
            {
                if (isUpDown)
                {
                   
                    win32api.keybd_event(VK_UP, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                
                    isUpDown = false;

                }
            }

            if (ws.ButtonState.Down)
            {

                if (!isDownDown)
                {
                    
                    win32api.keybd_event(VK_DOWN, 0, 0, (UIntPtr)0);
                    isDownDown = true;
                   
                }
            }
            else
            {
                if (isDownDown)
                {
                   
                    win32api.keybd_event(VK_DOWN, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                    
                    isDownDown = false;

                }
            }


            if (ws.ButtonState.Right)
            {

                if (!isRightDown)
                {
                    win32api.keybd_event(VK_RIGHT, 0, 0, (UIntPtr)0);
                    isRightDown = true;
                
                }
            }
            else
            {
                if (isRightDown)
                {
          
                    win32api.keybd_event(VK_RIGHT, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                  
                    isRightDown = false;

                }
            }


            if (ws.ButtonState.Left)
            {

                if (!isLeftDown)
                {
                
                    win32api.keybd_event(VK_LEFT, 0, 0, (UIntPtr)0);
                    isLeftDown = true;
                
                }
            }
            else
            {
                if (isLeftDown)
                {
                    
                    win32api.keybd_event(VK_LEFT, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
             
                    isLeftDown = false;

                }
            }


            if (ws.ButtonState.Plus)
            {

                if (!isPlusDown)
                {
                    isPlusDown = true;
                  
                  
                    win32api.keybd_event(VK_ENTER, 0, 0, (UIntPtr)0);
                }
            }
            else
            {
                if (isPlusDown)
                {
                    
                    win32api.keybd_event(VK_ENTER, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                    
                    isPlusDown = false;

                }
            }


            if (ws.ButtonState.Minus)
            {

                if (!isMinusDown)
                {
                   
                    win32api.keybd_event(VK_BS, 0, 0, (UIntPtr)0);
                    isMinusDown = true;
                  
                }
            }
            else
            {
                if (isMinusDown)
                {
                   
                    win32api.keybd_event(VK_BS, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                   
                    isMinusDown = false;

                }
            }

            if (ws.ButtonState.Home)
            {

                if (!isHomeDown)
                {
              
                    win32api.keybd_event(VK_WINDOWS, 0, 0, (UIntPtr)0);
                    isHomeDown = true;
                   
                }
            }
            else
            {
                if (isHomeDown)
                {
                   
                    win32api.keybd_event(VK_WINDOWS, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                    
                    isHomeDown = false;

                }
            }


            if (ws.ButtonState.Two)
            {

                if (!isTwoDown)
                {
                 
                    win32api.keybd_event(VK_TAB, 0, 0, (UIntPtr)0);
                    isTwoDown = true;
                }
            }
            else
            {
                if (isTwoDown)
                {
                 
                    win32api.keybd_event(VK_TAB, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                    win32api.keybd_event(VK_DOWN, 0, 0, (UIntPtr)0);
                    win32api.keybd_event(VK_DOWN, 0, 2/*KEYEVENTF_KEYUP*/, (UIntPtr)0);
                  
                    isTwoDown = false;

                }
            }


            if (ws.ButtonState.B)
            {

                if (!isBDown)
                {
                    isBDown = true;
                   
                }
            }
            else
            {
                if (isBDown)
                {
                   
                    isBDown = false;
                }
            }


        }
    }
}
