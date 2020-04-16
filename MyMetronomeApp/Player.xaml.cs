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

namespace MyMetronomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Player : CirclePage
    {
        public static BluetoothLeDevice leDevice = null;
        public static BluetoothGattClient client = null;
        public static string remote_addr = "B8:27:EB:DC:D9:EC";
        static bool StateChanged_flag = false;

        public Player()
        {
            InitializeComponent();

        }

        private async void OnClick(object sender, EventArgs e)
        {
            try
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
            }

        }

        public async Task WaitScanFlag()
        {
            await Task.Delay(10000);
        }

        private void scanResultEventHandler(object sender, AdapterLeScanResultChangedEventArgs e)
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
        }
    }
}