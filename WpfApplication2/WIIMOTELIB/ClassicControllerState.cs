// Decompiled with JetBrains decompiler
// Type: WiimoteLib.ClassicControllerState
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public struct ClassicControllerState
    {
        [DataMember]
        public ClassicControllerCalibrationInfo CalibrationInfo;
        [DataMember]
        public ClassicControllerButtonState ButtonState;
        [DataMember]
        public Point RawJoystickL;
        [DataMember]
        public Point RawJoystickR;
        [DataMember]
        public PointF JoystickL;
        [DataMember]
        public PointF JoystickR;
        [DataMember]
        public byte RawTriggerL;
        [DataMember]
        public byte RawTriggerR;
        [DataMember]
        public float TriggerL;
        [DataMember]
        public float TriggerR;
    }
}
