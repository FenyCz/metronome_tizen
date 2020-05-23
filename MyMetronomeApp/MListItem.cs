using Tizen.Network.Bluetooth;

namespace MyMetronomeApp
{
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