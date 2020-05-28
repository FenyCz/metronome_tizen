/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

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

    // třída sloužící k vytvoření Bluetooth komunikace
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bluetooth : CirclePage
    {
        static BluetoothHandler bh;

        public static string service_uuid = "00001101-0000-1000-8000-00805F9B34FB";

        // konstruktor
        public Bluetooth(BluetoothHandler data)
        {
            InitializeComponent();
            bh = data;

            // pokud je již spojení navázáno, informuje uživatele textem a nastavením barev
            if (bh.flagConnect)
            {
                SetConnected();
            }
        }

        // nastavení barev a listView pro stav, kdy je připojení navázáno
        public void SetConnected()
        {
            discover.BackgroundColor = Color.FromHex("#008000");
            disconnect.BackgroundColor = Color.FromHex("#008000");

            BTList.ItemsSource = null;
            bh.bListItems.Clear();
            bh.bListItems.Add(new MListItem("Connected!"));
            BTList.ItemsSource = bh.bListItems;
        }

        // nastavení barev a listView pro stav, kdy připojení navázáno není
        public void SetDisconnected()
        {
            discover.BackgroundColor = Color.FromHex("#E65100");
            disconnect.BackgroundColor = Color.FromHex("#E65100");
            BTList.ItemsSource = null;
            bh.bListItems.Clear();
        }

        // stisknutím na tlačítko Discover začne vyhledávání bluetooth zařízení
        private async void Discover(object sender, EventArgs e)
        {
            try
            {
                // pokud je bluetooth vypnutý
                if (!BluetoothAdapter.IsBluetoothEnabled)
                {
                    Toast.DisplayText("Please turn on Bluetooth.", 2000);
                }

                // pokud je bluetooth zapnutý
                else
                {
                    BTList.ItemsSource = null;
                    bh.bListItems.Clear();

                    Toast.DisplayText("Discovering...", 2000);

                    // začneme vyhledávat dostupná zařízení
                    BluetoothAdapter.DiscoveryStateChanged += DiscoveryStateChangedEventHandler;
                    BluetoothAdapter.StartDiscovery();
                    await WaitDiscoveryFlag();
                    BluetoothAdapter.DiscoveryStateChanged -= DiscoveryStateChangedEventHandler;

                    // pokud nalezneme, ukončíme vyhledávání, vyhledávání je náročné na zdroje BT
                    if (bh.flagDeviceFound)
                    {
                        BluetoothAdapter.StopDiscovery();
                        BTList.ItemsSource = bh.bListItems;
                    }

                    else
                    {
                        Toast.DisplayText("No founded BT devices", 2000);
                    }
                }
            }

            // pokud nedošlo k navázání BT spojení vypíše chybu
            catch (Exception ex)
            {
                Toast.DisplayText("DiscoverError: " + ex.Message, 2000);
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
                Toast.DisplayText("CreateSocketError: " + ex.Message, 2000);
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
                    Toast.DisplayText("CreateSocketError: " + ex.Message, 2000);
                }
            }
            else
            {
                Toast.DisplayText("SocketError: Socket isn't created.", 2000);
            }
        }

        // po kliknuti na vybrane zarizeni zahajime pokus o vytvoreni socketu a spojeni
        private void Connect(object sender, ItemTappedEventArgs e)
        {
            // vyhledame item na ktery jsme kliknuli
            MListItem item = (MListItem)e.Item;
            for (int i = 0; i < bh.bListItems.Count(); i++)
            {
                if(item.Name.Equals("Connected!"))
                {
                    Toast.DisplayText("You are connected!", 1000);
                }
                
                else if (item.Name.Equals(bh.bListItems[i].Name))
                {
                    // bluetooth musi byt zapnuty
                    if (!BluetoothAdapter.IsBluetoothEnabled)
                    {
                        Toast.DisplayText("Please turn on Bluetooth.", 2000);
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
                    bh.flagServerDataReceived = false;

                    // nastaveni barev a informace pro uzivatele o odpojeni
                    SetDisconnected();

                   // Toast.DisplayText("Disconnected!", 2000);
                }
                else
                {
                    Toast.DisplayText("You are not connected.", 2000);
                }
            }
            catch (Exception ex)
            {
                Toast.DisplayText("DisconnectError: " + ex.Message);

                BluetoothHandler.Client.Disconnect();

                // zruseni eventHandleru a nastaveni flagu do vychozi hodnoty
                BluetoothHandler.Client.ConnectionStateChanged -= ConnectionStateChangedEventHandler;
                BluetoothHandler.Client.DataReceived -= DataReceivedServerEventHandler;
                bh.flagCreateClientDone = false;
                bh.flagConnect = false;
                bh.flagDeviceFound = false;
                bh.flagServerDataReceived = false;
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

                        SetConnected();

                        bh.flagConnect = true;
                    }
                    else 
                    {
                        Toast.DisplayText("SeverError: Prepare your mobile and reconnect!", 2000);
                    }
                }
                else
                {
                    Toast.DisplayText("Callback: No connection data", 2000);
                }
            }
            
            // pokud dojde k preruseni socketu, zarizeni je odpojeno
            else
            {
                Toast.DisplayText("Callback: Disconnected!", 2000);

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

            // oznámení uživateli, že jsme dostali data
            Toast.DisplayText("Received", 1000);

            // pokud došlo k příjmu prvních dat
            if (!bh.flagServerDataReceived)
            {
                bh.pViewModel.DeleteDatabase();
                bh.flagServerDataReceived = true;
            }

            // vložíme data do databáze
            bh.pViewModel.InsertDatabase(args.Data.Data);
            
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