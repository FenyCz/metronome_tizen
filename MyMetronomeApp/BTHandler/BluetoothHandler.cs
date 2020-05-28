/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

using MyMetronomeApp.SettingsViews;
using MyMetronomeApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Network.Bluetooth;

namespace MyMetronomeApp.BTHandler
{
    // třída uchovávájící si informace o Bluetooth připojení 
    public class BluetoothHandler
    {  
        public static IBluetoothClientSocket Client;
        public static BluetoothSocketState ClientState;
        public static SocketConnection ClientConnection;
        public static BluetoothError ClientResult;

        public bool flagDeviceFound = false;
        public bool flagCreateClientDone = false;
        public bool flagConnect = false;
        public bool flagServerDataReceived = false;

        public List<MListItem> bListItems = new List<MListItem>();

        public string mobileName = null;
        public PlayerViewModel pViewModel;

        public BluetoothHandler(PlayerViewModel pViewModel)
        {
            this.pViewModel = pViewModel;
        }
    }
}
