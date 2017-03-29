// Decompiled with JetBrains decompiler
// Type: WiimoteLib.Wiimote
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace WiimoteLib
{
    public class Wiimote : IDisposable
    {
        private readonly WiimoteState mWiimoteState = new WiimoteState();
        private readonly AutoResetEvent mReadDone = new AutoResetEvent(false);
        private readonly AutoResetEvent mWriteDone = new AutoResetEvent(false);
        private readonly AutoResetEvent mStatusDone = new AutoResetEvent(false);
        private string mDevicePath = string.Empty;
        private readonly Guid mID = Guid.NewGuid();
        private const int VID = 0x057e;
        private const int PID = 0x0306;
        private const int REPORT_LENGTH = 22;
        private const int REGISTER_IR = 78643248;
        private const int REGISTER_IR_SENSITIVITY_1 = 78643200;
        private const int REGISTER_IR_SENSITIVITY_2 = 78643226;
        private const int REGISTER_IR_MODE = 78643251;
        private const int REGISTER_EXTENSION_INIT_1 = 77857008;
        private const int REGISTER_EXTENSION_INIT_2 = 77857019;
        private const int REGISTER_EXTENSION_TYPE = 77857018;
        private const int REGISTER_EXTENSION_TYPE_2 = 77857022;
        private const int REGISTER_EXTENSION_CALIBRATION = 77856800;
        private const int REGISTER_MOTIONPLUS_INIT = 77988094;
        private const int BSL = 43;
        private const int BSW = 24;
        private const float KG2LB = 2.204623f;
        private SafeFileHandle mHandle;
        private FileStream mStream;
        private byte[] mReadBuff;
        private int mAddress;
        private short mSize;
        private bool mAltWriteMethod;

        public WiimoteState WiimoteState
        {
            get
            {
                return this.mWiimoteState;
            }
        }

        public Guid ID
        {
            get
            {
                return this.mID;
            }
        }

        public string HIDDevicePath
        {
            get
            {
                return this.mDevicePath;
            }
        }

        public LastReadStatus LastReadStatus { get; private set; }

        public event EventHandler<WiimoteChangedEventArgs> WiimoteChanged;

        public event EventHandler<WiimoteExtensionChangedEventArgs> WiimoteExtensionChanged;

        public Wiimote()
        {
        }

        internal Wiimote(string devicePath)
        {
            this.mDevicePath = devicePath;
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(this.mDevicePath))
                Wiimote.FindWiimote(new Wiimote.WiimoteFoundDelegate(this.WiimoteFound));
            else
                this.OpenWiimoteDeviceHandle(this.mDevicePath);
        }

        internal static void FindWiimote(Wiimote.WiimoteFoundDelegate wiimoteFound)
        {
            int memberIndex = 0;
            bool flag = false;
            Guid gHid;
            HIDImports.HidD_GetHidGuid(out gHid);
            IntPtr classDevs = HIDImports.SetupDiGetClassDevs(ref gHid, (string)null, IntPtr.Zero, 16U);
            HIDImports.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            deviceInterfaceData.cbSize = Marshal.SizeOf((object)deviceInterfaceData);
            for (; HIDImports.SetupDiEnumDeviceInterfaces(classDevs, IntPtr.Zero, ref gHid, memberIndex, ref deviceInterfaceData); ++memberIndex)
            {
                uint requiredSize;
                HIDImports.SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, IntPtr.Zero, 0U, out requiredSize, IntPtr.Zero);
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();
                deviceInterfaceDetailData.cbSize = IntPtr.Size == 8 ? 8U : 5U;
                if (!HIDImports.SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, ref deviceInterfaceDetailData, requiredSize, out requiredSize, IntPtr.Zero))
                    throw new WiimoteException("SetupDiGetDeviceInterfaceDetail failed on index " + (object)memberIndex);
                SafeFileHandle file = HIDImports.CreateFile(deviceInterfaceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);
                HIDImports.HIDD_ATTRIBUTES Attributes = new HIDImports.HIDD_ATTRIBUTES();
                Attributes.Size = Marshal.SizeOf((object)Attributes);
                if (HIDImports.HidD_GetAttributes(file.DangerousGetHandle(), ref Attributes) && (int)Attributes.VendorID == 1406 && (int)Attributes.ProductID == 774)
                {
                    flag = true;
                    if (!wiimoteFound(deviceInterfaceDetailData.DevicePath))
                        break;
                }
                file.Close();
            }
            int num = (int)HIDImports.SetupDiDestroyDeviceInfoList(classDevs);
            if (!flag)
            {
                WpfApplication2.Window2 m = new WpfApplication2.Window2();
                m.Show();
            }
        }

        private bool WiimoteFound(string devicePath)
        {
            this.mDevicePath = devicePath;
            this.OpenWiimoteDeviceHandle(this.mDevicePath);
            return false;
        }

        private void OpenWiimoteDeviceHandle(string devicePath)
        {
            this.mHandle = HIDImports.CreateFile(devicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);
            HIDImports.HIDD_ATTRIBUTES Attributes = new HIDImports.HIDD_ATTRIBUTES();
            Attributes.Size = Marshal.SizeOf((object)Attributes);
            if (!HIDImports.HidD_GetAttributes(this.mHandle.DangerousGetHandle(), ref Attributes))
                return;
            if ((int)Attributes.VendorID == 1406 && (int)Attributes.ProductID == 774)
            {
                this.mStream = new FileStream(this.mHandle, FileAccess.ReadWrite, 22, true);
                this.BeginAsyncRead();
                try
                {
                    this.ReadWiimoteCalibration();
                }
                catch
                {
                    this.mAltWriteMethod = true;
                    this.ReadWiimoteCalibration();
                }
                this.GetStatus();
            }
            else
            {
                this.mHandle.Close();
                throw new WiimoteException("Attempted to open a non-Wiimote device.");
            }
        }

        public void InitializeMotionPlus()
        {
            this.WriteData(77988094, (byte)4);
        }

        public void Disconnect()
        {
            if (this.mStream != null)
                this.mStream.Close();
            if (this.mHandle == null)
                return;
            this.mHandle.Close();
        }

        private void BeginAsyncRead()
        {
            if (this.mStream == null || !this.mStream.CanRead)
                return;
            byte[] buffer = new byte[22];
            try
            {
                this.mStream.BeginRead(buffer, 0, 22, new AsyncCallback(this.OnReadData), (object)buffer);
            }
            catch (Exception ex)
            {
                WpfApplication2.Window2 m = new WpfApplication2.Window2();
                m.Show();
            }
            }

        private void OnReadData(IAsyncResult ar)
        {
            byte[] buff = (byte[])ar.AsyncState;
            try
            {
                this.mStream.EndRead(ar);
            }
            catch (Exception ex)
            {
                WpfApplication2.Window2 m = new WpfApplication2.Window2();
                m.Show();
            }
                if (this.ParseInputReport(buff) && this.WiimoteChanged != null)
                    this.WiimoteChanged((object)this, new WiimoteChangedEventArgs(this.mWiimoteState));
                this.BeginAsyncRead();
            
          
        }

        private bool ParseInputReport(byte[] buff)
        {
            switch ((InputReport)buff[0])
            {
                case InputReport.Status:
                    this.ParseButtons(buff);
                    this.mWiimoteState.BatteryRaw = buff[6];
                    this.mWiimoteState.Battery = (float)(4800.0 * (double)((float)buff[6] / 48f) / 192.0);
                    this.mWiimoteState.LEDState.LED1 = ((int)buff[3] & 16) != 0;
                    this.mWiimoteState.LEDState.LED2 = ((int)buff[3] & 32) != 0;
                    this.mWiimoteState.LEDState.LED3 = ((int)buff[3] & 64) != 0;
                    this.mWiimoteState.LEDState.LED4 = ((int)buff[3] & 128) != 0;
                    this.BeginAsyncRead();
                    byte[] numArray = this.ReadData(77857022, (short)1);
                    bool flag = ((int)buff[3] & 2) != 0;
                    if (this.mWiimoteState.Extension != flag || (int)numArray[0] == 4)
                    {
                        this.mWiimoteState.Extension = flag;
                        if (flag)
                        {
                            this.BeginAsyncRead();
                            this.InitializeExtension(numArray[0]);
                        }
                        else
                            this.mWiimoteState.ExtensionType = ExtensionType.None;
                        if (this.WiimoteExtensionChanged != null && this.mWiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                            this.WiimoteExtensionChanged((object)this, new WiimoteExtensionChangedEventArgs(this.mWiimoteState.ExtensionType, this.mWiimoteState.Extension));
                    }
                    this.mStatusDone.Set();
                    break;
                case InputReport.ReadData:
                    this.ParseButtons(buff);
                    this.ParseReadData(buff);
                    break;
                case InputReport.OutputReportAck:
                    this.mWriteDone.Set();
                    break;
                case InputReport.Buttons:
                    this.ParseButtons(buff);
                    break;
                case InputReport.ButtonsAccel:
                    this.ParseButtons(buff);
                    this.ParseAccel(buff);
                    break;
                case InputReport.IRAccel:
                    this.ParseButtons(buff);
                    this.ParseAccel(buff);
                    this.ParseIR(buff);
                    break;
                case InputReport.ButtonsExtension:
                    this.ParseButtons(buff);
                    this.ParseExtension(buff, 3);
                    break;
                case InputReport.ExtensionAccel:
                    this.ParseButtons(buff);
                    this.ParseAccel(buff);
                    this.ParseExtension(buff, 6);
                    break;
                case InputReport.IRExtensionAccel:
                    this.ParseButtons(buff);
                    this.ParseAccel(buff);
                    this.ParseIR(buff);
                    this.ParseExtension(buff, 16);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void InitializeExtension(byte extensionType)
        {
            if ((int)extensionType != 4)
            {
                this.WriteData(77857008, (byte)85);
                this.WriteData(77857019, (byte)0);
            }
            this.BeginAsyncRead();
            byte[] numArray1 = this.ReadData(77857018, (short)6);
            long num = (long)numArray1[0] << 40 | (long)numArray1[1] << 32 | (long)numArray1[2] << 24 | (long)numArray1[3] << 16 | (long)numArray1[4] << 8 | (long)numArray1[5];
            switch ((ExtensionType)num)
            {
                case ExtensionType.Drums:
                case ExtensionType.BalanceBoard:
                case ExtensionType.MotionPlus:
                case ExtensionType.TaikoDrum:
                case ExtensionType.ClassicController:
                case ExtensionType.Guitar:
                case ExtensionType.Nunchuk:
                    this.mWiimoteState.ExtensionType = (ExtensionType)num;
                    this.SetReportType(InputReport.ButtonsExtension, true);
                    switch (this.mWiimoteState.ExtensionType)
                    {


                        case (ExtensionType)2753560834:
                            return;
                        case ExtensionType.Guitar:
                            return;

                        default:
                            return;
                    }
                case ExtensionType.ParitallyInserted:
                case ExtensionType.None:
                    this.mWiimoteState.Extension = false;
                    this.mWiimoteState.ExtensionType = ExtensionType.None;
                    break;
                default:
                    throw new WiimoteException("Unknown extension controller found: " + num.ToString("x"));
            }
        }

        private byte[] DecryptBuffer(byte[] buff)
        {
            for (int index = 0; index < buff.Length; ++index)
                buff[index] = (byte)(((int)buff[index] ^ 23) + 23 & (int)byte.MaxValue);
            return buff;
        }

        private void ParseButtons(byte[] buff)
        {
            this.mWiimoteState.ButtonState.A = ((int)buff[2] & 8) != 0;
            this.mWiimoteState.ButtonState.B = ((int)buff[2] & 4) != 0;
            this.mWiimoteState.ButtonState.Minus = ((int)buff[2] & 16) != 0;
            this.mWiimoteState.ButtonState.Home = ((int)buff[2] & 128) != 0;
            this.mWiimoteState.ButtonState.Plus = ((int)buff[1] & 16) != 0;
            this.mWiimoteState.ButtonState.One = ((int)buff[2] & 2) != 0;
            this.mWiimoteState.ButtonState.Two = ((int)buff[2] & 1) != 0;
            this.mWiimoteState.ButtonState.Up = ((int)buff[1] & 8) != 0;
            this.mWiimoteState.ButtonState.Down = ((int)buff[1] & 4) != 0;
            this.mWiimoteState.ButtonState.Left = ((int)buff[1] & 1) != 0;
            this.mWiimoteState.ButtonState.Right = ((int)buff[1] & 2) != 0;
        }

        private void ParseAccel(byte[] buff)
        {
            this.mWiimoteState.AccelState.RawValues.X = (int)buff[3];
            this.mWiimoteState.AccelState.RawValues.Y = (int)buff[4];
            this.mWiimoteState.AccelState.RawValues.Z = (int)buff[5];
            this.mWiimoteState.AccelState.Values.X = ((float)this.mWiimoteState.AccelState.RawValues.X - (float)this.mWiimoteState.AccelCalibrationInfo.X0) / ((float)this.mWiimoteState.AccelCalibrationInfo.XG - (float)this.mWiimoteState.AccelCalibrationInfo.X0);
            this.mWiimoteState.AccelState.Values.Y = ((float)this.mWiimoteState.AccelState.RawValues.Y - (float)this.mWiimoteState.AccelCalibrationInfo.Y0) / ((float)this.mWiimoteState.AccelCalibrationInfo.YG - (float)this.mWiimoteState.AccelCalibrationInfo.Y0);
            this.mWiimoteState.AccelState.Values.Z = ((float)this.mWiimoteState.AccelState.RawValues.Z - (float)this.mWiimoteState.AccelCalibrationInfo.Z0) / ((float)this.mWiimoteState.AccelCalibrationInfo.ZG - (float)this.mWiimoteState.AccelCalibrationInfo.Z0);
        }

        private void ParseIR(byte[] buff)
        {
            this.mWiimoteState.IRState.IRSensors[0].RawPosition.X = (int)buff[6] | ((int)buff[8] >> 4 & 3) << 8;
            this.mWiimoteState.IRState.IRSensors[0].RawPosition.Y = (int)buff[7] | ((int)buff[8] >> 6 & 3) << 8;
            switch (this.mWiimoteState.IRState.Mode)
            {
                case IRMode.Basic:
                    this.mWiimoteState.IRState.IRSensors[1].RawPosition.X = (int)buff[9] | ((int)buff[8] & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[1].RawPosition.Y = (int)buff[10] | ((int)buff[8] >> 2 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[2].RawPosition.X = (int)buff[11] | ((int)buff[13] >> 4 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[2].RawPosition.Y = (int)buff[12] | ((int)buff[13] >> 6 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[3].RawPosition.X = (int)buff[14] | ((int)buff[13] & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[3].RawPosition.Y = (int)buff[15] | ((int)buff[13] >> 2 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[0].Size = 0;
                    this.mWiimoteState.IRState.IRSensors[1].Size = 0;
                    this.mWiimoteState.IRState.IRSensors[2].Size = 0;
                    this.mWiimoteState.IRState.IRSensors[3].Size = 0;
                    this.mWiimoteState.IRState.IRSensors[0].Found = (int)buff[6] != (int)byte.MaxValue || (int)buff[7] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[1].Found = (int)buff[9] != (int)byte.MaxValue || (int)buff[10] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[2].Found = (int)buff[11] != (int)byte.MaxValue || (int)buff[12] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[3].Found = (int)buff[14] != (int)byte.MaxValue || (int)buff[15] != (int)byte.MaxValue;
                    break;
                case IRMode.Extended:
                    this.mWiimoteState.IRState.IRSensors[1].RawPosition.X = (int)buff[9] | ((int)buff[11] >> 4 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[1].RawPosition.Y = (int)buff[10] | ((int)buff[11] >> 6 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[2].RawPosition.X = (int)buff[12] | ((int)buff[14] >> 4 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[2].RawPosition.Y = (int)buff[13] | ((int)buff[14] >> 6 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[3].RawPosition.X = (int)buff[15] | ((int)buff[17] >> 4 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[3].RawPosition.Y = (int)buff[16] | ((int)buff[17] >> 6 & 3) << 8;
                    this.mWiimoteState.IRState.IRSensors[0].Size = (int)buff[8] & 15;
                    this.mWiimoteState.IRState.IRSensors[1].Size = (int)buff[11] & 15;
                    this.mWiimoteState.IRState.IRSensors[2].Size = (int)buff[14] & 15;
                    this.mWiimoteState.IRState.IRSensors[3].Size = (int)buff[17] & 15;
                    this.mWiimoteState.IRState.IRSensors[0].Found = (int)buff[6] != (int)byte.MaxValue || (int)buff[7] != (int)byte.MaxValue || (int)buff[8] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[1].Found = (int)buff[9] != (int)byte.MaxValue || (int)buff[10] != (int)byte.MaxValue || (int)buff[11] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[2].Found = (int)buff[12] != (int)byte.MaxValue || (int)buff[13] != (int)byte.MaxValue || (int)buff[14] != (int)byte.MaxValue;
                    this.mWiimoteState.IRState.IRSensors[3].Found = (int)buff[15] != (int)byte.MaxValue || (int)buff[16] != (int)byte.MaxValue || (int)buff[17] != (int)byte.MaxValue;
                    break;
            }
            this.mWiimoteState.IRState.IRSensors[0].Position.X = (float)this.mWiimoteState.IRState.IRSensors[0].RawPosition.X / 1023.5f;
            this.mWiimoteState.IRState.IRSensors[1].Position.X = (float)this.mWiimoteState.IRState.IRSensors[1].RawPosition.X / 1023.5f;
            this.mWiimoteState.IRState.IRSensors[2].Position.X = (float)this.mWiimoteState.IRState.IRSensors[2].RawPosition.X / 1023.5f;
            this.mWiimoteState.IRState.IRSensors[3].Position.X = (float)this.mWiimoteState.IRState.IRSensors[3].RawPosition.X / 1023.5f;
            this.mWiimoteState.IRState.IRSensors[0].Position.Y = (float)this.mWiimoteState.IRState.IRSensors[0].RawPosition.Y / 767.5f;
            this.mWiimoteState.IRState.IRSensors[1].Position.Y = (float)this.mWiimoteState.IRState.IRSensors[1].RawPosition.Y / 767.5f;
            this.mWiimoteState.IRState.IRSensors[2].Position.Y = (float)this.mWiimoteState.IRState.IRSensors[2].RawPosition.Y / 767.5f;
            this.mWiimoteState.IRState.IRSensors[3].Position.Y = (float)this.mWiimoteState.IRState.IRSensors[3].RawPosition.Y / 767.5f;
            if (this.mWiimoteState.IRState.IRSensors[0].Found && this.mWiimoteState.IRState.IRSensors[1].Found)
            {
                this.mWiimoteState.IRState.RawMidpoint.X = (this.mWiimoteState.IRState.IRSensors[1].RawPosition.X + this.mWiimoteState.IRState.IRSensors[0].RawPosition.X) / 2;
                this.mWiimoteState.IRState.RawMidpoint.Y = (this.mWiimoteState.IRState.IRSensors[1].RawPosition.Y + this.mWiimoteState.IRState.IRSensors[0].RawPosition.Y) / 2;
                this.mWiimoteState.IRState.Midpoint.X = (float)(((double)this.mWiimoteState.IRState.IRSensors[1].Position.X + (double)this.mWiimoteState.IRState.IRSensors[0].Position.X) / 2.0);
                this.mWiimoteState.IRState.Midpoint.Y = (float)(((double)this.mWiimoteState.IRState.IRSensors[1].Position.Y + (double)this.mWiimoteState.IRState.IRSensors[0].Position.Y) / 2.0);
            }
            else
                this.mWiimoteState.IRState.Midpoint.X = this.mWiimoteState.IRState.Midpoint.Y = 0.0f;
        }

        private void ParseExtension(byte[] buff, int offset)
        {
            switch (this.mWiimoteState.ExtensionType)
            {
                case ExtensionType.BalanceBoard:
                    this.mWiimoteState.BalanceBoardState.SensorValuesRaw.TopRight = (short)((int)buff[offset] << 8 | (int)buff[offset + 1]);
                    this.mWiimoteState.BalanceBoardState.SensorValuesRaw.BottomRight = (short)((int)buff[offset + 2] << 8 | (int)buff[offset + 3]);
                    this.mWiimoteState.BalanceBoardState.SensorValuesRaw.TopLeft = (short)((int)buff[offset + 4] << 8 | (int)buff[offset + 5]);
                    this.mWiimoteState.BalanceBoardState.SensorValuesRaw.BottomLeft = (short)((int)buff[offset + 6] << 8 | (int)buff[offset + 7]);
                    this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopLeft = this.GetBalanceBoardSensorValue(this.mWiimoteState.BalanceBoardState.SensorValuesRaw.TopLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg0.TopLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg17.TopLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg34.TopLeft);
                    this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopRight = this.GetBalanceBoardSensorValue(this.mWiimoteState.BalanceBoardState.SensorValuesRaw.TopRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg0.TopRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg17.TopRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg34.TopRight);
                    this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft = this.GetBalanceBoardSensorValue(this.mWiimoteState.BalanceBoardState.SensorValuesRaw.BottomLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg0.BottomLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg17.BottomLeft, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg34.BottomLeft);
                    this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomRight = this.GetBalanceBoardSensorValue(this.mWiimoteState.BalanceBoardState.SensorValuesRaw.BottomRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg0.BottomRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg17.BottomRight, this.mWiimoteState.BalanceBoardState.CalibrationInfo.Kg34.BottomRight);
                    this.mWiimoteState.BalanceBoardState.SensorValuesLb.TopLeft = this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopLeft * 2.204623f;
                    this.mWiimoteState.BalanceBoardState.SensorValuesLb.TopRight = this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopRight * 2.204623f;
                    this.mWiimoteState.BalanceBoardState.SensorValuesLb.BottomLeft = this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft * 2.204623f;
                    this.mWiimoteState.BalanceBoardState.SensorValuesLb.BottomRight = this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomRight * 2.204623f;
                    this.mWiimoteState.BalanceBoardState.WeightKg = (float)(((double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopRight + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomRight) / 4.0);
                    this.mWiimoteState.BalanceBoardState.WeightLb = (float)(((double)this.mWiimoteState.BalanceBoardState.SensorValuesLb.TopLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesLb.TopRight + (double)this.mWiimoteState.BalanceBoardState.SensorValuesLb.BottomLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesLb.BottomRight) / 4.0);
                    float num1 = (float)(((double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft) / ((double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopRight + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomRight));
                    float num2 = (float)(((double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.TopRight) / ((double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomLeft + (double)this.mWiimoteState.BalanceBoardState.SensorValuesKg.BottomRight));
                    this.mWiimoteState.BalanceBoardState.CenterOfGravity.X = (float)((double)(num1 - 1f) / (double)(num1 + 1f) * -21.0);
                    this.mWiimoteState.BalanceBoardState.CenterOfGravity.Y = (float)((double)(num2 - 1f) / (double)(num2 + 1f) * -12.0);
                    break;
                case ExtensionType.MotionPlus:
                    this.mWiimoteState.MotionPlusState.YawFast = ((int)buff[offset + 3] & 2) >> 1 == 0;
                    this.mWiimoteState.MotionPlusState.PitchFast = ((int)buff[offset + 3] & 1) == 0;
                    this.mWiimoteState.MotionPlusState.RollFast = ((int)buff[offset + 4] & 2) >> 1 == 0;
                    this.mWiimoteState.MotionPlusState.RawValues.X = (int)buff[offset] | ((int)buff[offset + 3] & 250) << 6;
                    this.mWiimoteState.MotionPlusState.RawValues.Y = (int)buff[offset + 1] | ((int)buff[offset + 4] & 250) << 6;
                    this.mWiimoteState.MotionPlusState.RawValues.Z = (int)buff[offset + 2] | ((int)buff[offset + 5] & 250) << 6;
                    break;
                case ExtensionType.Drums:
                    this.mWiimoteState.DrumsState.RawJoystick.X = (int)buff[offset] & 63;
                    this.mWiimoteState.DrumsState.RawJoystick.Y = (int)buff[offset + 1] & 63;
                    this.mWiimoteState.DrumsState.Plus = ((int)buff[offset + 4] & 4) == 0;
                    this.mWiimoteState.DrumsState.Minus = ((int)buff[offset + 4] & 16) == 0;
                    this.mWiimoteState.DrumsState.Pedal = ((int)buff[offset + 5] & 4) == 0;
                    this.mWiimoteState.DrumsState.Blue = ((int)buff[offset + 5] & 8) == 0;
                    this.mWiimoteState.DrumsState.Green = ((int)buff[offset + 5] & 16) == 0;
                    this.mWiimoteState.DrumsState.Yellow = ((int)buff[offset + 5] & 32) == 0;
                    this.mWiimoteState.DrumsState.Red = ((int)buff[offset + 5] & 64) == 0;
                    this.mWiimoteState.DrumsState.Orange = ((int)buff[offset + 5] & 128) == 0;
                    this.mWiimoteState.DrumsState.Joystick.X = (float)(this.mWiimoteState.DrumsState.RawJoystick.X - 31) / 63f;
                    this.mWiimoteState.DrumsState.Joystick.Y = (float)(this.mWiimoteState.DrumsState.RawJoystick.Y - 31) / 63f;
                    if (((int)buff[offset + 2] & 64) != 0)
                        break;
                    int num3 = (int)buff[offset + 2] >> 1 & 31;
                    int num4 = (int)buff[offset + 3] >> 5;
                    if (num4 == 7)
                        break;
                    switch (num3)
                    {
                        case 14:
                            this.mWiimoteState.DrumsState.OrangeVelocity = num4;
                            return;
                        case 15:
                            this.mWiimoteState.DrumsState.BlueVelocity = num4;
                            return;
                        case 16:
                            return;
                        case 17:
                            this.mWiimoteState.DrumsState.YellowVelocity = num4;
                            return;
                        case 18:
                            this.mWiimoteState.DrumsState.GreenVelocity = num4;
                            return;
                        case 25:
                            this.mWiimoteState.DrumsState.RedVelocity = num4;
                            return;
                        case 26:
                            return;
                        case 27:
                            this.mWiimoteState.DrumsState.PedalVelocity = num4;
                            return;
                        default:
                            return;
                    }
                case ExtensionType.Nunchuk:
                    this.mWiimoteState.NunchukState.RawJoystick.X = (int)buff[offset];
                    this.mWiimoteState.NunchukState.RawJoystick.Y = (int)buff[offset + 1];
                    this.mWiimoteState.NunchukState.AccelState.RawValues.X = (int)buff[offset + 2];
                    this.mWiimoteState.NunchukState.AccelState.RawValues.Y = (int)buff[offset + 3];
                    this.mWiimoteState.NunchukState.AccelState.RawValues.Z = (int)buff[offset + 4];
                    this.mWiimoteState.NunchukState.C = ((int)buff[offset + 5] & 2) == 0;
                    this.mWiimoteState.NunchukState.Z = ((int)buff[offset + 5] & 1) == 0;
                    this.mWiimoteState.NunchukState.AccelState.Values.X = ((float)this.mWiimoteState.NunchukState.AccelState.RawValues.X - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0) / ((float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0);
                    this.mWiimoteState.NunchukState.AccelState.Values.Y = ((float)this.mWiimoteState.NunchukState.AccelState.RawValues.Y - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0) / ((float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.YG - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0);
                    this.mWiimoteState.NunchukState.AccelState.Values.Z = ((float)this.mWiimoteState.NunchukState.AccelState.RawValues.Z - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0) / ((float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.ZG - (float)this.mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0);
                    if ((int)this.mWiimoteState.NunchukState.CalibrationInfo.MaxX != 0)
                        this.mWiimoteState.NunchukState.Joystick.X = ((float)this.mWiimoteState.NunchukState.RawJoystick.X - (float)this.mWiimoteState.NunchukState.CalibrationInfo.MidX) / ((float)this.mWiimoteState.NunchukState.CalibrationInfo.MaxX - (float)this.mWiimoteState.NunchukState.CalibrationInfo.MinX);
                    if ((int)this.mWiimoteState.NunchukState.CalibrationInfo.MaxY == 0)
                        break;
                    this.mWiimoteState.NunchukState.Joystick.Y = ((float)this.mWiimoteState.NunchukState.RawJoystick.Y - (float)this.mWiimoteState.NunchukState.CalibrationInfo.MidY) / ((float)this.mWiimoteState.NunchukState.CalibrationInfo.MaxY - (float)this.mWiimoteState.NunchukState.CalibrationInfo.MinY);
                    break;
                case ExtensionType.TaikoDrum:
                    this.mWiimoteState.TaikoDrumState.OuterLeft = ((int)buff[offset + 5] & 32) == 0;
                    this.mWiimoteState.TaikoDrumState.InnerLeft = ((int)buff[offset + 5] & 64) == 0;
                    this.mWiimoteState.TaikoDrumState.InnerRight = ((int)buff[offset + 5] & 16) == 0;
                    this.mWiimoteState.TaikoDrumState.OuterRight = ((int)buff[offset + 5] & 8) == 0;
                    break;
                case ExtensionType.ClassicController:
                    this.mWiimoteState.ClassicControllerState.RawJoystickL.X = (int)(byte)((uint)buff[offset] & 63U);
                    this.mWiimoteState.ClassicControllerState.RawJoystickL.Y = (int)(byte)((uint)buff[offset + 1] & 63U);
                    this.mWiimoteState.ClassicControllerState.RawJoystickR.X = (int)(byte)((int)buff[offset + 2] >> 7 | ((int)buff[offset + 1] & 192) >> 5 | ((int)buff[offset] & 192) >> 3);
                    this.mWiimoteState.ClassicControllerState.RawJoystickR.Y = (int)(byte)((uint)buff[offset + 2] & 31U);
                    this.mWiimoteState.ClassicControllerState.RawTriggerL = (byte)(((int)buff[offset + 2] & 96) >> 2 | (int)buff[offset + 3] >> 5);
                    this.mWiimoteState.ClassicControllerState.RawTriggerR = (byte)((uint)buff[offset + 3] & 31U);
                    this.mWiimoteState.ClassicControllerState.ButtonState.TriggerR = ((int)buff[offset + 4] & 2) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Plus = ((int)buff[offset + 4] & 4) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Home = ((int)buff[offset + 4] & 8) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Minus = ((int)buff[offset + 4] & 16) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.TriggerL = ((int)buff[offset + 4] & 32) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Down = ((int)buff[offset + 4] & 64) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Right = ((int)buff[offset + 4] & 128) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Up = ((int)buff[offset + 5] & 1) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Left = ((int)buff[offset + 5] & 2) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.ZR = ((int)buff[offset + 5] & 4) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.X = ((int)buff[offset + 5] & 8) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.A = ((int)buff[offset + 5] & 16) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.Y = ((int)buff[offset + 5] & 32) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.B = ((int)buff[offset + 5] & 64) == 0;
                    this.mWiimoteState.ClassicControllerState.ButtonState.ZL = ((int)buff[offset + 5] & 128) == 0;
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL != 0)
                        this.mWiimoteState.ClassicControllerState.JoystickL.X = ((float)this.mWiimoteState.ClassicControllerState.RawJoystickL.X - (float)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MidXL) / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinXL);
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL != 0)
                        this.mWiimoteState.ClassicControllerState.JoystickL.Y = ((float)this.mWiimoteState.ClassicControllerState.RawJoystickL.Y - (float)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MidYL) / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinYL);
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR != 0)
                        this.mWiimoteState.ClassicControllerState.JoystickR.X = ((float)this.mWiimoteState.ClassicControllerState.RawJoystickR.X - (float)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MidXR) / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinXR);
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR != 0)
                        this.mWiimoteState.ClassicControllerState.JoystickR.Y = ((float)this.mWiimoteState.ClassicControllerState.RawJoystickR.Y - (float)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MidYR) / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinYR);
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL != 0)
                        this.mWiimoteState.ClassicControllerState.TriggerL = (float)this.mWiimoteState.ClassicControllerState.RawTriggerL / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerL);
                    if ((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR == 0)
                        break;
                    this.mWiimoteState.ClassicControllerState.TriggerR = (float)this.mWiimoteState.ClassicControllerState.RawTriggerR / (float)((int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR - (int)this.mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerR);
                    break;
                case ExtensionType.Guitar:
                    this.mWiimoteState.GuitarState.GuitarType = ((int)buff[offset] & 128) == 0 ? GuitarType.GuitarHeroWorldTour : GuitarType.GuitarHero3;
                    this.mWiimoteState.GuitarState.ButtonState.Plus = ((int)buff[offset + 4] & 4) == 0;
                    this.mWiimoteState.GuitarState.ButtonState.Minus = ((int)buff[offset + 4] & 16) == 0;
                    this.mWiimoteState.GuitarState.ButtonState.StrumDown = ((int)buff[offset + 4] & 64) == 0;
                    this.mWiimoteState.GuitarState.ButtonState.StrumUp = ((int)buff[offset + 5] & 1) == 0;
                    this.mWiimoteState.GuitarState.FretButtonState.Yellow = ((int)buff[offset + 5] & 8) == 0;
                    this.mWiimoteState.GuitarState.FretButtonState.Green = ((int)buff[offset + 5] & 16) == 0;
                    this.mWiimoteState.GuitarState.FretButtonState.Blue = ((int)buff[offset + 5] & 32) == 0;
                    this.mWiimoteState.GuitarState.FretButtonState.Red = ((int)buff[offset + 5] & 64) == 0;
                    this.mWiimoteState.GuitarState.FretButtonState.Orange = ((int)buff[offset + 5] & 128) == 0;
                    this.mWiimoteState.GuitarState.RawJoystick.X = (int)buff[offset] & 63;
                    this.mWiimoteState.GuitarState.RawJoystick.Y = (int)buff[offset + 1] & 63;
                    this.mWiimoteState.GuitarState.RawWhammyBar = (byte)((uint)buff[offset + 3] & 31U);
                    this.mWiimoteState.GuitarState.Joystick.X = (float)(this.mWiimoteState.GuitarState.RawJoystick.X - 31) / 63f;
                    this.mWiimoteState.GuitarState.Joystick.Y = (float)(this.mWiimoteState.GuitarState.RawJoystick.Y - 31) / 63f;
                    this.mWiimoteState.GuitarState.WhammyBar = (float)this.mWiimoteState.GuitarState.RawWhammyBar / 10f;
                    this.mWiimoteState.GuitarState.TouchbarState.Yellow = false;
                    this.mWiimoteState.GuitarState.TouchbarState.Green = false;
                    this.mWiimoteState.GuitarState.TouchbarState.Blue = false;
                    this.mWiimoteState.GuitarState.TouchbarState.Red = false;
                    this.mWiimoteState.GuitarState.TouchbarState.Orange = false;
                    switch ((int)buff[offset + 2] & 31)
                    {
                        case 4:
                            this.mWiimoteState.GuitarState.TouchbarState.Green = true;
                            return;
                        case 5:
                            return;
                        case 6:
                            return;
                        case 7:
                            this.mWiimoteState.GuitarState.TouchbarState.Green = true;
                            this.mWiimoteState.GuitarState.TouchbarState.Red = true;
                            return;
                        case 8:
                            return;
                        case 9:
                            return;
                        case 10:
                            this.mWiimoteState.GuitarState.TouchbarState.Red = true;
                            return;
                        case 11:
                            return;
                        case 12:
                        case 13:
                            this.mWiimoteState.GuitarState.TouchbarState.Red = true;
                            this.mWiimoteState.GuitarState.TouchbarState.Yellow = true;
                            return;
                        case 14:
                            return;
                        case 15:
                            return;
                        case 16:
                            return;
                        case 17:
                            return;
                        case 18:
                        case 19:
                            this.mWiimoteState.GuitarState.TouchbarState.Yellow = true;
                            return;
                        case 20:
                        case 21:
                            this.mWiimoteState.GuitarState.TouchbarState.Yellow = true;
                            this.mWiimoteState.GuitarState.TouchbarState.Blue = true;
                            return;
                        case 22:
                            return;
                        case 23:
                        case 24:
                            this.mWiimoteState.GuitarState.TouchbarState.Blue = true;
                            return;
                        case 25:
                            return;
                        case 26:
                            this.mWiimoteState.GuitarState.TouchbarState.Blue = true;
                            this.mWiimoteState.GuitarState.TouchbarState.Orange = true;
                            return;
                        case 31:
                            this.mWiimoteState.GuitarState.TouchbarState.Orange = true;
                            return;
                        default:
                            return;
                    }
            }
        }

        private float GetBalanceBoardSensorValue(short sensor, short min, short mid, short max)
        {
            if ((int)max == (int)mid || (int)mid == (int)min)
                return 0.0f;
            if ((int)sensor < (int)mid)
                return (float)(68.0 * ((double)((int)sensor - (int)min) / (double)((int)mid - (int)min)));
            return (float)(68.0 * ((double)((int)sensor - (int)mid) / (double)((int)max - (int)mid)) + 68.0);
        }

        private void ParseReadData(byte[] buff)
        {
            if (((int)buff[3] & 8) != 0)
                throw new WiimoteException("Error reading data from Wiimote: Bytes do not exist.");
            if (((int)buff[3] & 7) != 0)
            {
                this.LastReadStatus = LastReadStatus.ReadFromWriteOnlyMemory;
                this.mReadDone.Set();
            }
            else
            {
                int length = ((int)buff[3] >> 4) + 1;
                int num = (int)buff[4] << 8 | (int)buff[5];
                Array.Copy((Array)buff, 6, (Array)this.mReadBuff, num - this.mAddress, length);
                if (this.mAddress + (int)this.mSize == num + length)
                    this.mReadDone.Set();
                this.LastReadStatus = LastReadStatus.Success;
            }
        }

        private byte GetRumbleBit()
        {
            return this.mWiimoteState.Rumble ? (byte)1 : (byte)0;
        }

        private void ReadWiimoteCalibration()
        {
            byte[] numArray = this.ReadData(22, (short)7);
            this.mWiimoteState.AccelCalibrationInfo.X0 = numArray[0];
            this.mWiimoteState.AccelCalibrationInfo.Y0 = numArray[1];
            this.mWiimoteState.AccelCalibrationInfo.Z0 = numArray[2];
            this.mWiimoteState.AccelCalibrationInfo.XG = numArray[4];
            this.mWiimoteState.AccelCalibrationInfo.YG = numArray[5];
            this.mWiimoteState.AccelCalibrationInfo.ZG = numArray[6];
        }

        public void SetReportType(InputReport type, bool continuous)
        {
            this.SetReportType(type, IRSensitivity.Maximum, continuous);
        }

        public void SetReportType(InputReport type, IRSensitivity irSensitivity, bool continuous)
        {
            if (this.mWiimoteState.ExtensionType == ExtensionType.BalanceBoard)
                type = InputReport.ButtonsExtension;
            switch (type)
            {
                case InputReport.IRAccel:
                    this.EnableIR(IRMode.Extended, irSensitivity);
                    break;
                case InputReport.IRExtensionAccel:
                    this.EnableIR(IRMode.Basic, irSensitivity);
                    break;
                default:
                    this.DisableIR();
                    break;
            }
            byte[] report = this.CreateReport();
            report[0] = (byte)18;
            report[1] = (byte)((continuous ? 4 : 0) | (this.mWiimoteState.Rumble ? 1 : 0));
            report[2] = (byte)type;
            this.WriteReport(report);
        }

        public void SetLEDs(bool led1, bool led2, bool led3, bool led4)
        {
            this.mWiimoteState.LEDState.LED1 = led1;
            this.mWiimoteState.LEDState.LED2 = led2;
            this.mWiimoteState.LEDState.LED3 = led3;
            this.mWiimoteState.LEDState.LED4 = led4;
            byte[] report = this.CreateReport();
            report[0] = (byte)17;
            report[1] = (byte)((uint)((led1 ? 16 : 0) | (led2 ? 32 : 0) | (led3 ? 64 : 0) | (led4 ? 128 : 0)) | (uint)this.GetRumbleBit());
            this.WriteReport(report);
        }

        public void SetLEDs(int leds)
        {
            this.mWiimoteState.LEDState.LED1 = (leds & 1) > 0;
            this.mWiimoteState.LEDState.LED2 = (leds & 2) > 0;
            this.mWiimoteState.LEDState.LED3 = (leds & 4) > 0;
            this.mWiimoteState.LEDState.LED4 = (leds & 8) > 0;
            byte[] report = this.CreateReport();
            report[0] = (byte)17;
            report[1] = (byte)((uint)(((leds & 1) > 0 ? 16 : 0) | ((leds & 2) > 0 ? 32 : 0) | ((leds & 4) > 0 ? 64 : 0) | ((leds & 8) > 0 ? 128 : 0)) | (uint)this.GetRumbleBit());
            this.WriteReport(report);
        }

        public void SetRumble(bool on)
        {
            this.mWiimoteState.Rumble = on;
            this.SetLEDs(this.mWiimoteState.LEDState.LED1, this.mWiimoteState.LEDState.LED2, this.mWiimoteState.LEDState.LED3, this.mWiimoteState.LEDState.LED4);
        }

        public void GetStatus()
        {
            byte[] report = this.CreateReport();
            report[0] = (byte)21;
            report[1] = this.GetRumbleBit();
            this.WriteReport(report);
            if (!this.mStatusDone.WaitOne(3000, false))
            {
                WpfApplication2.Window2 m = new WpfApplication2.Window2();
                m.Show();
            }

        }

        private void EnableIR(IRMode mode, IRSensitivity irSensitivity)
        {
            this.mWiimoteState.IRState.Mode = mode;
            byte[] report = this.CreateReport();
            report[0] = (byte)19;
            report[1] = (byte)(4U | (uint)this.GetRumbleBit());
            this.WriteReport(report);
            Array.Clear((Array)report, 0, report.Length);
            report[0] = (byte)26;
            report[1] = (byte)(4U | (uint)this.GetRumbleBit());
            this.WriteReport(report);
            this.WriteData(78643248, (byte)8);
            switch (irSensitivity)
            {
                case IRSensitivity.WiiLevel1:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 2,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 100,
            (byte) 0,
            (byte) 254
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 253,
            (byte) 5
          });
                    break;
                case IRSensitivity.WiiLevel2:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 2,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 150,
            (byte) 0,
            (byte) 180
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 179,
            (byte) 4
          });
                    break;
                case IRSensitivity.WiiLevel3:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 2,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 170,
            (byte) 0,
            (byte) 100
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 99,
            (byte) 3
          });
                    break;
                case IRSensitivity.WiiLevel4:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 2,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 200,
            (byte) 0,
            (byte) 54
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 53,
            (byte) 3
          });
                    break;
                case IRSensitivity.WiiLevel5:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 7,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 114,
            (byte) 0,
            (byte) 32
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 1,
            (byte) 3
          });
                    break;
                case IRSensitivity.Maximum:
                    this.WriteData(78643200, (byte)9, new byte[9]
          {
            (byte) 2,
            (byte) 0,
            (byte) 0,
            (byte) 113,
            (byte) 1,
            (byte) 0,
            (byte) 144,
            (byte) 0,
            (byte) 65
          });
                    this.WriteData(78643226, (byte)2, new byte[2]
          {
            (byte) 64,
            (byte) 0
          });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("irSensitivity");
            }
            this.WriteData(78643251, (byte)mode);
            this.WriteData(78643248, (byte)8);
        }

        private void DisableIR()
        {
            this.mWiimoteState.IRState.Mode = IRMode.Off;
            byte[] report = this.CreateReport();
            report[0] = (byte)19;
            report[1] = this.GetRumbleBit();
            this.WriteReport(report);
            Array.Clear((Array)report, 0, report.Length);
            report[0] = (byte)26;
            report[1] = this.GetRumbleBit();
            this.WriteReport(report);
        }

        private byte[] CreateReport()
        {
            return new byte[22];
        }

        private void WriteReport(byte[] buff)
        {
            if (this.mAltWriteMethod)
                HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), buff, (uint)buff.Length);
            else if (this.mStream != null)
                this.mStream.Write(buff, 0, 22);
            if ((int)buff[0] != 22)
                return;
            this.mWriteDone.WaitOne(1000, false);
        }

        public byte[] ReadData(int address, short size)
        {
            
            byte[] report = this.CreateReport();
            this.mReadBuff = new byte[(int)size];
            this.mAddress = address & (int)ushort.MaxValue;
            this.mSize = size;
            report[0] = (byte)23;
            report[1] = (byte)((ulong)(((long)address & 4278190080L) >> 24) | (ulong)this.GetRumbleBit());
            report[2] = (byte)((address & 16711680) >> 16);
            report[3] = (byte)((address & 65280) >> 8);
            report[4] = (byte)(address & (int)byte.MaxValue);
            report[5] = (byte)(((int)size & 65280) >> 8);
            report[6] = (byte)((uint)size & (uint)byte.MaxValue);
            this.WriteReport(report);
            if (!this.mReadDone.WaitOne(1000, false))
                throw new WiimoteException("Error reading data from Wiimote...is it connected?");
            return this.mReadBuff;
        }

        public void WriteData(int address, byte data)
        {
            this.WriteData(address, (byte)1, new byte[1]
      {
        data
      });
        }

        public void WriteData(int address, byte size, byte[] data)
        {
            byte[] report = this.CreateReport();
            report[0] = (byte)22;
            report[1] = (byte)((ulong)(((long)address & 4278190080L) >> 24) | (ulong)this.GetRumbleBit());
            report[2] = (byte)((address & 16711680) >> 16);
            report[3] = (byte)((address & 65280) >> 8);
            report[4] = (byte)(address & (int)byte.MaxValue);
            report[5] = size;
            Array.Copy((Array)data, 0, (Array)report, 6, (int)size);
            this.WriteReport(report);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.Disconnect();
        }

        private enum OutputReport : byte
        {
            LEDs = (byte)17,
            DataReportType = (byte)18,
            IR = (byte)19,
            Status = (byte)21,
            WriteMemory = (byte)22,
            ReadMemory = (byte)23,
            IR2 = (byte)26,
        }

        internal delegate bool WiimoteFoundDelegate(string devicePath);
    }
}