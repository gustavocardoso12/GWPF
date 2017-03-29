// Decompiled with JetBrains decompiler
// Type: WiimoteLib.ButtonState
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public struct ButtonState
    {
        [DataMember]
        public bool A;
        [DataMember]
        public bool B;
        [DataMember]
        public bool Plus;
        [DataMember]
        public bool Home;
        [DataMember]
        public bool Minus;
        [DataMember]
        public bool One;
        [DataMember]
        public bool Two;
        [DataMember]
        public bool Up;
        [DataMember]
        public bool Down;
        [DataMember]
        public bool Left;
        [DataMember]
        public bool Right;
    }
}
