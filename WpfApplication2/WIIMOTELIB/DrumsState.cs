﻿// Decompiled with JetBrains decompiler
// Type: WiimoteLib.DrumsState
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System;

namespace WiimoteLib
{
    [DataContract]
    [Serializable]
    public struct DrumsState
    {
        public bool Red;
        public bool Green;
        public bool Blue;
        public bool Orange;
        public bool Yellow;
        public bool Pedal;
        public int RedVelocity;
        public int GreenVelocity;
        public int BlueVelocity;
        public int OrangeVelocity;
        public int YellowVelocity;
        public int PedalVelocity;
        public bool Plus;
        public bool Minus;
        public Point RawJoystick;
        public PointF Joystick;
    }
}
