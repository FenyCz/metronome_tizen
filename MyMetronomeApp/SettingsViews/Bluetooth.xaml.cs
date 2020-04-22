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

        public static string service_uuid = "00001101-0000-1000-8000-00805F9B34FB";

        public Bluetooth(BluetoothHandler data)
        {
            InitializeComponent();
            bh = data;
        }

        private async void Discover(object sender, EventArgs e)
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
                    BTList.ItemsSource = null;
                    bh.bListItems.Clear();

                    // zacneme vyhledavat dostupna zarizeni
                    BluetoothAdapter.DiscoveryStateChanged += DiscoveryStateChangedEventHandler;
                    BluetoothAdapter.StartDiscovery();
                    await WaitDiscoveryFlag();
                    BluetoothAdapter.DiscoveryStateChanged -= DiscoveryStateChangedEventHandler;

                    if (bh.FlagDeviceFound)
                    {
                        BluetoothAdapter.StopDiscovery();
                        BTList.ItemsSource = bh.bListItems;
                    }
                    else
                    {
                        Toast.DisplayText("No founded BT devices");
                    }
                }
            }
            // pokud nedoslo k navazani BT spojeni vypise chybu
            catch (Exception ex)
            {
                Toast.DisplayText("DiscoverError: " + ex.Message);
            }
        }

        private void MakeConnection()
        {
            // pokud jsme uspesne vytvorili clientsocket
            if (bh.flagCreateClientDone)
            {
                // pripojeni k zarizeni
                try
                {
                    BluetoothHandler.Client.Connect();
                    BluetoothHandler.Client.DataReceived += DataReceivedServerEventHandler;
                    //Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                    bh.flagConnect = true;
                }
                catch (Exception ex)
                {
                    Toast.DisplayText("CreateSocketError: " + ex.Message);
                }

            }
            else
            {
                Toast.DisplayText("SocketError: Socket isn't created.");
            }
        }

        private void Connect(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                MListItem item = (MListItem)e.Item;
                for (int i = 0; i < bh.bListItems.Count(); i++)
                {
                    if (item.Name.Equals(bh.bListItems[i].Name))
                    {
                        // bluetooth musi byt zapnuty
                        if (!BluetoothAdapter.IsBluetoothEnabled)
                        {
                            Toast.DisplayText("Please turn on Bluetooth.");
                        }
                        else
                        {
                            // připojení 
                            try
                            {
                                BluetoothHandler.Client = bh.bListItems[i].DeviceFound.CreateSocket(service_uuid);
                                BluetoothHandler.Client.ConnectionStateChanged += ConnectionStateChangedEventHandler;
                                bh.flagCreateClientDone = true;

                            }
                            catch (Exception ex)
                            {
                                Toast.DisplayText("CreateSocketError: " + ex.Message);
                            }

                            MakeConnection();
                            break;
                        }
                    }
                }
            }
        }

        // funkce pro zaslani zpravy
        /*private void Send(object sender, EventArgs e)
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
        }*/

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

                    BTList.ItemsSource = null;
                    bh.bListItems.Clear();

                    Toast.DisplayText("Disconnected!");
                }
                else
                {
                    Toast.DisplayText("You are not connected.");
                }
            }
            catch (Exception ex)
            {
                Toast.DisplayText("DisconnectError: " + ex.Message);
            }
        }

        public static void DiscoveryStateChangedEventHandler(object sender, DiscoveryStateChangedEventArgs args)
        {
            //Toast.DisplayText("DiscoveryStateChanged callback " + args.DiscoveryState);
            if (args.DiscoveryState == BluetoothDeviceDiscoveryState.Found)
            {
                // nalezeno nove zarizeni
                //Toast.DisplayText("DiscoveryStateChanged callback device found: " + args.DeviceFound.Name);

                // vlozeni noveho zarizeni do seznamu, pokud tam jeste neni
                MListItem findedItem = new MListItem(args.DeviceFound.Name.ToString(), args.DeviceFound);

                if (!bh.bListItems.Contains(findedItem)){
                    bh.bListItems.Add(findedItem);
                    //BluetoothHandler.Client = args.DeviceFound.CreateSocket(service_uuid);
                    bh.FlagDeviceFound = true;
                }
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
            //Toast.DisplayText("ConnectionStateChanged callback in client " + args.State);
            BluetoothHandler.ClientState = args.State;
            BluetoothHandler.ClientConnection = args.Connection;
            BluetoothHandler.ClientResult = args.Result;

            if (args.State == BluetoothSocketState.Connected)
            {
                
                if (BluetoothHandler.ClientConnection != null)
                {
                    //Toast.DisplayText("Callback: Socket of connection: " + BluetoothHandler.ClientConnection.SocketFd);
                    //Toast.DisplayText("Callback: Address of connection: " + BluetoothHandler.ClientConnection.Address);
                    if(BluetoothHandler.ClientConnection.SocketFd != -1)
                    {
                        Toast.DisplayText("Callback: Connected!");
                    }
                    else 
                    {
                        Toast.DisplayText("SeverError: Prepare your mobile and reconnect!");
                    }
                }
                else
                {
                    Toast.DisplayText("Callback: No connection data");
                }
            }
            else
            {
                Toast.DisplayText("Callback: Disconnected!");
                if (BluetoothHandler.ClientConnection != null)
                {
                    //Toast.DisplayText("Callback: Socket of disconnection: " + BluetoothHandler.ClientConnection.SocketFd);
                    //Toast.DisplayText("Callback: Address of disconnection: " + BluetoothHandler.ClientConnection.Address);
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

    }
}