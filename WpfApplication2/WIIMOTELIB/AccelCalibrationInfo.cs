// Decompiled with JetBrains decompiler
// Type: WiimoteLib.AccelCalibrationInfo
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public struct AccelCalibrationInfo
    {
        [DataMember]
        public byte X0;
        [DataMember]
        public byte Y0;
        [DataMember]
        public byte Z0;
        [DataMember]
        public byte XG;
        [DataMember]
        public byte YG;
        [DataMember]
        public byte ZG;
    }
}
