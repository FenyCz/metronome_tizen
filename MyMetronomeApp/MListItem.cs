/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

using Tizen.Network.Bluetooth;

namespace MyMetronomeApp
{
    // pomocná třída pro vytváření položek do listView
    public class MListItem
    {
        public BluetoothDevice DeviceFound { get; set; }

        public string Name { get; set; }

        public string IconPath { get; set; }

        public MListItem(string name, string path)
        {
            Name = name;
            IconPath = path;
        }

        public MListItem(string name)
        {
            Name = name;
        }

        public MListItem(string name, BluetoothDevice deviceFound)
        {
            DeviceFound = deviceFound;
            Name = name;
        }
    }
}