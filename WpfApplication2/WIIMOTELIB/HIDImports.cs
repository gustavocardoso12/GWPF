// Decompiled with JetBrains decompiler
// Type: WiimoteLib.HIDImports
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WiimoteLib
{
    internal class HIDImports
    {
        public const int DIGCF_DEFAULT = 1;
        public const int DIGCF_PRESENT = 2;
        public const int DIGCF_ALLCLASSES = 4;
        public const int DIGCF_PROFILE = 8;
        public const int DIGCF_DEVICEINTERFACE = 16;

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid gHid);

        [DllImport("hid.dll")]
        public static extern bool HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDImports.HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll")]
        internal static extern bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, [MarshalAs(UnmanagedType.LPTStr)] string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInvo, ref Guid interfaceClassGuid, int memberIndex, ref HIDImports.SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref HIDImports.SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref HIDImports.SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ushort SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] HIDImports.EFileAttributes flags, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 1U,
            Hidden = 2U,
            System = 4U,
            Directory = 16U,
            Archive = 32U,
            Device = 64U,
            Normal = 128U,
            Temporary = 256U,
            SparseFile = 512U,
            ReparsePoint = 1024U,
            Compressed = 2048U,
            Offline = 4096U,
            NotContentIndexed = 8192U,
            Encrypted = 16384U,
            Write_Through = 2147483648U,
            Overlapped = 1073741824U,
            NoBuffering = 536870912U,
            RandomAccess = 268435456U,
            SequentialScan = 134217728U,
            DeleteOnClose = 67108864U,
            BackupSemantics = 33554432U,
            PosixSemantics = 16777216U,
            OpenReparsePoint = 2097152U,
            OpenNoRecall = 1048576U,
            FirstPipeInstance = 524288U,
        }

        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr RESERVED;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public uint cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public short VendorID;
            public short ProductID;
            public short VersionNumber;
        }
    }
}
