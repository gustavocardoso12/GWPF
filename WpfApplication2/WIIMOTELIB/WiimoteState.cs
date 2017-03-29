// Decompiled with JetBrains decompiler
// Type: WiimoteLib.WiimoteState
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public class WiimoteState
    {
        [DataMember]
        public AccelCalibrationInfo AccelCalibrationInfo;
        [DataMember]
        public AccelState AccelState;
        [DataMember]
        public ButtonState ButtonState;
        [DataMember]
        public IRState IRState;
        [DataMember]
        public byte BatteryRaw;
        [DataMember]
        public float Battery;
        [DataMember]
        public bool Rumble;
        [DataMember]
        public bool Extension;
        [DataMember]
        public ExtensionType ExtensionType;
        [DataMember]
        public NunchukState NunchukState;
        [DataMember]
        public ClassicControllerState ClassicControllerState;
        [DataMember]
        public GuitarState GuitarState;
        [DataMember]
        public DrumsState DrumsState;
        public BalanceBoardState BalanceBoardState;
        public TaikoDrumState TaikoDrumState;
        public MotionPlusState MotionPlusState;
        [DataMember]
        public LEDState LEDState;

        public WiimoteState()
        {
            this.IRState.IRSensors = new IRSensor[4];
        }
    }
}
