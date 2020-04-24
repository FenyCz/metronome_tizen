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

            // pokud je spojeni navazano, tak informuje - connected
            if (bh.flagConnect)
            {
                SetConnected();
            }
        }

        // nastaveni barev a listviewu pro stav - connected
        public void SetConnected()
        {
            discover.BackgroundColor = Color.FromHex("#008000");
            disconnect.BackgroundColor = Color.FromHex("#008000");

            BTList.ItemsSource = null;
            bh.bListItems.Clear();
            bh.bListItems.Add(new MListItem("Connected!"));
            BTList.ItemsSource = bh.bListItems;
        }

        // nastaveni barev a listviewu pro stav - disconnected
        public void SetDisconnected()
        {
            discover.BackgroundColor = Color.FromHex("#E65100");
            disconnect.BackgroundColor = Color.FromHex("#E65100");
            BTList.ItemsSource = null;
            bh.bListItems.Clear();
        }

        // stisknuti tlacitka discover, zacne vyhledavat viditelna zarizeni BTs
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

                    Toast.DisplayText("Discovering...");
                    
                    await WaitDiscoveryFlag();
                    BluetoothAdapter.DiscoveryStateChanged -= DiscoveryStateChangedEventHandler;

                    // pokud nalezneme ukoncime vyhledavani, vyhledavani je narocne na zdroje BT
                    if (bh.flagDeviceFound)
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

        // po kliknuti na vybrane zarizeni zahajime pokus o spojeni
        private void MakeConnection(int i)
        {

            // vytvoreni socketu 
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

            // pokud jsme uspesne vytvorili clientsocket
            if (bh.flagCreateClientDone)
            {
                // pripojeni k zarizeni
                try
                {
                    BluetoothHandler.Client.Connect();
                    BluetoothHandler.Client.DataReceived += DataReceivedServerEventHandler;
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

        // po kliknuti na vybrane zarizeni zahajime pokus o vytvoreni socketu a spojeni
        private void Connect(object sender, ItemTappedEventArgs e)
        {
            // vyhledame item na ktery jsme kliknuli
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
                        // pokus zahajeni spojeni
                        MakeConnection(i);
                        break;
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

                    // zruseni eventHandleru a nastaveni flagu do vychozi hodnoty
                    BluetoothHandler.Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                    BluetoothHandler.Client.DataReceived -= DataReceivedServerEventHandler;
                    bh.flagCreateClientDone = false;
                    bh.flagConnect = false;
                    bh.flagDeviceFound = false;

                    // nastaveni barev a informace pro uzivatele o odpojeni
                    SetDisconnected();

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

        // eventHandler pro vyhledávání zařízení
        public static void DiscoveryStateChangedEventHandler(object sender, DiscoveryStateChangedEventArgs args)
        {
            if (args.DiscoveryState == BluetoothDeviceDiscoveryState.Found)
            {

                // vlozeni naleznuteho zarizeni do seznamu, pokud tam jeste neni
                MListItem findedItem = new MListItem(args.DeviceFound.Name.ToString(), args.DeviceFound);

                if (!bh.bListItems.Contains(findedItem)){
                    bh.bListItems.Add(findedItem);

                    bh.flagDeviceFound = true;
                }
            }
        }

        // eventHandler pro vytvoření spojení
        public void ConnectionStateChangedEventHandler(object sender, SocketConnectionStateChangedEventArgs args)
        {
            BluetoothHandler.ClientState = args.State;
            BluetoothHandler.ClientConnection = args.Connection;
            BluetoothHandler.ClientResult = args.Result;

            if (args.State == BluetoothSocketState.Connected)
            {
                
                if (BluetoothHandler.ClientConnection != null)
                {
                    // pokud je spojeni navazano na spravny socket
                    if(BluetoothHandler.ClientConnection.SocketFd != -1)
                    {
                        Toast.DisplayText("Connected!");

                        SetConnected();

                        bh.flagConnect = true;
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
            
            // pokud dojde k preruseni socketu, zarizeni je odpojeno
            else
            {
                Toast.DisplayText("Callback: Disconnected!");

                SetDisconnected();

                // zruseni eventHandleru a nastaveni flagu do vychozi hodnoty
                BluetoothHandler.Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                BluetoothHandler.Client.DataReceived -= DataReceivedServerEventHandler;

                bh.flagConnect = false;
                bh.flagCreateClientDone = false;
                bh.flagDeviceFound = false;
            }
        }

        // eventHandler pro přijímání dat
        private void DataReceivedServerEventHandler(object sender, SocketDataReceivedEventArgs args)
        {
            //BluetoothSetup.Data = args.Data;
            //LogUtils.Write(LogUtils.DEBUG, LogUtils.TAG, "DataReceived in client: " + args.Data.Data);
            Toast.DisplayText("DataReceived in client: " + args.Data.Data);
            //flagServerDataReceived = true;
        }

        // pomocna funkce pro vyhledavani BT zarizeni, celkem hledame 20s
        public static async Task WaitDiscoveryFlag()
        {
            int loop = 0;
            while (true)
            {
                await Task.Delay(2000);

                loop++;
                if (bh.flagDeviceFound)
                {
                    break;
                }
                if (loop == 10)
                {
                    break;
                }
            }
        }

    }
}