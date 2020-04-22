using MyMetronomeApp.SettingsViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Network.Bluetooth;

namespace MyMetronomeApp.BTHandler
{
    public class BluetoothHandler
    {  
        public static IBluetoothClientSocket Client;
        public static BluetoothSocketState ClientState;
        public static SocketConnection ClientConnection;
        public static BluetoothError ClientResult;

        public bool FlagDeviceFound = false;
        public bool flagCreateClientDone = false;
        public bool flagConnect = false;

        public List<MListItem> bListItems = new List<MListItem>();

        public string mobileName = null;
    }
}
