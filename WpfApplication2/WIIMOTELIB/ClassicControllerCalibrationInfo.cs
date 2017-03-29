// Decompiled with JetBrains decompiler
// Type: WiimoteLib.ClassicControllerCalibrationInfo
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public struct ClassicControllerCalibrationInfo
    {
        [DataMember]
        public byte MinXL;
        [DataMember]
        public byte MidXL;
        [DataMember]
        public byte MaxXL;
        [DataMember]
        public byte MinYL;
        [DataMember]
        public byte MidYL;
        [DataMember]
        public byte MaxYL;
        [DataMember]
        public byte MinXR;
        [DataMember]
        public byte MidXR;
        [DataMember]
        public byte MaxXR;
        [DataMember]
        public byte MinYR;
        [DataMember]
        public byte MidYR;
        [DataMember]
        public byte MaxYR;
        [DataMember]
        public byte MinTriggerL;
        [DataMember]
        public byte MaxTriggerL;
        [DataMember]
        public byte MinTriggerR;
        [DataMember]
        public byte MaxTriggerR;
    }
}
