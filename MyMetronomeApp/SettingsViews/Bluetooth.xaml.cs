using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.Network.Bluetooth;
using Tizen.Applications;
using System.Runtime.CompilerServices;
using MyMetronomeApp.BTHandler;

namespace MyMetronomeApp.SettingsViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bluetooth : CirclePage
    {
        static BluetoothHandler bh;
        /*public  IBluetoothClientSocket Client;
        public static BluetoothSocketState ClientState;
        public static SocketConnection ClientConnection;
        public static BluetoothError ClientResult;*/

        public static string service_uuid = "00001101-0000-1000-8000-00805F9B34FB";

        public Bluetooth(BluetoothHandler data)
        {
            InitializeComponent();
            bh = data;
        }

        private async void Connect(object sender, EventArgs e)
        {
            try
            {
                // pokud je BT vypnut
                if (!BluetoothAdapter.IsBluetoothEnabled)
                {
                    Toast.DisplayText("Please turn on Bluetooth.");
                }

                else
                {

                    // zacneme vyhledavat dostupna zarizeni
                    BluetoothAdapter.DiscoveryStateChanged += DiscoveryStateChangedEventHandler;
                    BluetoothAdapter.StartDiscovery();
                    await WaitDiscoveryFlag();
                    BluetoothAdapter.DiscoveryStateChanged -= DiscoveryStateChangedEventHandler;
                    
                    if (bh.FlagDeviceFound)
                    {
                        bh.flagCreateClientDone = true;
                    }
                    
                    // pokud jsme uspesne vytvorili clientsocket
                    if (bh.flagCreateClientDone)
                    {
                        // vypneme vyhledavani, velmi narocne na zdroje hodinek
                        BluetoothAdapter.StopDiscovery();


                        // pripojeni k zarizeni
                        BluetoothHandler.Client.ConnectionStateChanged += ConnectionStateChangedEventHandler;
                        BluetoothHandler.Client.Connect();
                        BluetoothHandler.Client.DataReceived += DataReceivedServerEventHandler;
                        //Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                        bh.flagConnect = true;

                    }
                    else
                    {
                        Toast.DisplayText("Connect error - Try to reconnect.");
                    }

                }
            }

            // pokud nedoslo k navazani BT spojeni vypise chybu
            catch (Exception ex)
            {
                Toast.DisplayText("Error: " + ex.Message);
            }
        }

        // funkce pro zaslani zpravy
        private void Send(object sender, EventArgs e)
        {
            try
            {
                if (bh.flagConnect)
                {
                    string dataFromClient = "Message";
                    BluetoothHandler.Client.SendData(dataFromClient);
                    Toast.DisplayText("Data sended...");
                }
                else
                {
                    Toast.DisplayText("Connect first.");
                }
            }
            catch (Exception ex)
            {
                Toast.DisplayText("Error: " + ex.Message);
            }
        }

        // funkce pro ukonceni BT spojeni
        private void Disconnect(object sender, EventArgs e)
        {
            try
            {
                if (bh.flagConnect)
                {
                    BluetoothHandler.Client.Disconnect();
                    
                    //unregistr receivcer
                    BluetoothHandler.Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                    BluetoothHandler.Client.DataReceived -= DataReceivedServerEventHandler;
                    bh.flagCreateClientDone = false;
                    bh.flagConnect = false;
                    Toast.DisplayText("Disconnected!");
                }
                else
                {
                    Toast.DisplayText("You are not connected.");
                }
            }
            catch (Exception ex)
            {
                Toast.DisplayText("Error: " + ex.Message);
            }
        }

                /*try
                {
                    if (!BluetoothAdapter.IsBluetoothEnabled)
                    {
                        Toast.DisplayText("Please turn on Bluetooth.");
                    }
                    else
                    {

                        /// For more information on scanning for BLE devices, see Managing Bluetooth LE Scans
                        BluetoothAdapter.ScanResultChanged += scanResultEventHandler;
                        if (leDevice == null)
                        {
                            BluetoothAdapter.StartLeScan();
                            /// Wait while the system searches for the LE target you want to connect to
                            /// If you find the LE target you want, stop the scan
                            await WaitScanFlag();

                            BluetoothAdapter.StopLeScan();
                            await Task.Delay(5000);
                        }

                        else
                        {
                            Toast.DisplayText("Lets get connected.");
                            //client = BluetoothGattClient.CreateClient(leDevice.RemoteAddress);
                            leDevice.GattConnectionStateChanged += GattClient_ConnectionStateChanged;
                            //await client.ConnectAsync(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast.DisplayText("Error: " + ex.Message);
                }*/

        public static void DiscoveryStateChangedEventHandler(object sender, DiscoveryStateChangedEventArgs args)
        {
            Toast.DisplayText("DiscoveryStateChanged callback " + args.DiscoveryState);
            if (args.DiscoveryState == BluetoothDeviceDiscoveryState.Found)
            {
                Toast.DisplayText("DiscoveryStateChanged callback device found: " + args.DeviceFound.Name);
                BluetoothHandler.Client = args.DeviceFound.CreateSocket(service_uuid);
                bh.FlagDeviceFound = true;
            }
        }

        public static async Task WaitDiscoveryFlag()
        {
            int count = 0;
            while (true)
            {
                await Task.Delay(2000);
                count++;
                if (bh.FlagDeviceFound)
                {
                    break;
                }
                if (count == 15)
                {
                    break;
                }
            }
        }

        public static void ConnectionStateChangedEventHandler(object sender, SocketConnectionStateChangedEventArgs args)
        {
            Toast.DisplayText("ConnectionStateChanged callback in client " + args.State);
            BluetoothHandler.ClientState = args.State;
            BluetoothHandler.ClientConnection = args.Connection;
            BluetoothHandler.ClientResult = args.Result;

            if (args.State == BluetoothSocketState.Connected)
            {
                Toast.DisplayText("Callback: Connected.");
                if (BluetoothHandler.ClientConnection != null)
                {
                    Toast.DisplayText("Callback: Socket of connection: " + BluetoothHandler.ClientConnection.SocketFd);
                    Toast.DisplayText("Callback: Address of connection: " + BluetoothHandler.ClientConnection.Address);
                }
                else
                {
                    Toast.DisplayText("Callback: No connection data");
                }
            }
            else
            {
                Toast.DisplayText("Callback: Disconnected.");
                if (BluetoothHandler.ClientConnection != null)
                {
                    Toast.DisplayText("Callback: Socket of disconnection: " + BluetoothHandler.ClientConnection.SocketFd);
                    Toast.DisplayText("Callback: Address of disconnection: " + BluetoothHandler.ClientConnection.Address);
                }
                else
                {
                    Toast.DisplayText("Callback: No connection data");
                }
            }
        }

        private void DataReceivedServerEventHandler(object sender, SocketDataReceivedEventArgs args)
        {
            //BluetoothSetup.Data = args.Data;
            //LogUtils.Write(LogUtils.DEBUG, LogUtils.TAG, "DataReceived in client: " + args.Data.Data);
            Toast.DisplayText("DataReceived in client: " + args.Data.Data);
            //flagServerDataReceived = true;
        }

        /*public async Task WaitScanFlag()
        {
            await Task.Delay(10000);
        }*/

        /*private void scanResultEventHandler(object sender, AdapterLeScanResultChangedEventArgs e)
        {
            // Found new device in your area.
            Toast.DisplayText("Lets get connected 2.");
        }

        public static void GattClient_ConnectionStateChanged(object sender, GattConnectionStateChangedEventArgs e)
        {
            if (e.Result != (int)BluetoothError.None)
            {
                StateChanged_flag = false;
            }
            else if (!e.RemoteAddress.Equals(remote_addr))
            {
                StateChanged_flag = false;
            }
            else if (e.IsConnected.Equals(false))
            {
                StateChanged_flag = false;
            }
            else
            {
                StateChanged_flag = true;
            }
        }*/
    }
}