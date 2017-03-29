// Decompiled with JetBrains decompiler
// Type: WiimoteLib.WiimoteCollection
// Assembly: WiimoteLib, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 28943724-0E4E-4D79-8A57-1C742E32D3E4
// Assembly location: C:\Users\h\Desktop\New Folder (9)\WiimoteLib_1.8\WiimoteLib.dll

using System.Collections.ObjectModel;

namespace WiimoteLib
{
    public class WiimoteCollection : Collection<Wiimote>
    {
        public void FindAllWiimotes()
        {
            Wiimote.FindWiimote(new Wiimote.WiimoteFoundDelegate(this.WiimoteFound));
        }

        private bool WiimoteFound(string devicePath)
        {
            this.Add(new Wiimote(devicePath));
            return true;
        }
    }
}
