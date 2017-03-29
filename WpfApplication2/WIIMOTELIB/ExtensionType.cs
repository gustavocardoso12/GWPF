// Decompiled with JetBrains decompiler
// Type: WiimoteLib.ExtensionType
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

namespace WiimoteLib
{
    [DataContract]
    public enum ExtensionType : long
    {
        None = 0L,
        Nunchuk = 2753560576L,
        ClassicController = 0xfd,
        Guitar = 2753560835L,
        TaikoDrum = 2753560849L,
        BalanceBoard = 1004200405L,
        MotionPlus = 1102265189381,
        Drums = 1102265188611L,
        ParitallyInserted = 281474976710655L,
    }
}
