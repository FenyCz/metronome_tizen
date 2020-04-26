using System;
using System.Collections.Generic;
using System.Timers;
using Tizen.System;
using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using SQLite;
using SQLitePCL;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Linq;

namespace MyMetronomeApp.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        public Command startPlayer;
        public int currentValue = 120;
        public double currentInterval;
        public Timer timer = new Timer();
        public bool isPlaying = false;
        readonly Vibrator vibrator = Vibrator.Vibrators[0];

        public List<Playlist> pListItems = new List<Playlist>();
        public List<Song> sListItems = new List<Song>();

        SQLiteConnection dbConnection;
        static string dbPath;

        public Playlist playlist1;
        public Playlist playlist2;

        string playlistNameString;
        string songNameString;

        public PlayerViewModel() {

            //InitDatabase();

            //InsertDatabase("af");

            startPlayer = new Command(PlayStopCommand);

            timer.Elapsed += ClickEvent;
        }

        public void InitDatabase()
        {
            bool flagCreateTable = false;

            // inicializace databaze
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider(true);

            string writablePath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;

            dbPath = Path.Combine(writablePath, "SQLite3.db3");

            // Check the database file to decide table creation.
            if (!File.Exists(dbPath))
            {
                flagCreateTable = true;
            }

            dbConnection = new SQLiteConnection(dbPath);
            if (flagCreateTable)
            {
                dbConnection.CreateTable<Playlist>();
                dbConnection.CreateTable<Song>();
            }

            dbConnection.DeleteAll<Playlist>();
            dbConnection.DeleteAll<Song>();

        }

        public void InsertDatabase(string data)
        {
            string receivedData = data;

            char[] charArray = receivedData.ToCharArray();

            char[] dataArray;

            List<string> stringList = new List<string>();

            List<string> songData = new List<string>();

            for (int p = 0; p < charArray.Length; p++)
            {

                if (charArray[p] == '#')
                {
                    stringList.Add(playlistNameString);
                    playlistNameString = "";
                }
                else
                {
                    playlistNameString = string.Concat(playlistNameString, charArray[p]);
                }
            }

            for (int k = 0; k < stringList.Count; k++)
            {

                dataArray = stringList[k].ToCharArray();
                
                if (dataArray[0] == 'P')
                {
                    int l = 2;
                    while(dataArray.Length != l)
                    {
                        string charName = "";
                        charName = dataArray[l].ToString();
                        playlistNameString = string.Concat(playlistNameString, charName);
                        l++;
                    }

                    Playlist playlist = new Playlist
                    {
                        Name = playlistNameString,
                    };

                    dbConnection.Insert(playlist);

                    playlistNameString = ""; 
                }

                else if (dataArray[0] == 'S')
                {
                    int m = 2;

                    for (int n = 0; n < 2; n++)
                    {

                        while (dataArray[m] != '*')
                        {
                            string charName = "";
                            charName = dataArray[m].ToString();
                            songNameString = string.Concat(songNameString, charName);
                            m++;
                        }
                        m++;

                        songData.Add(songNameString);
                        songNameString = "";
                    }

                    while(dataArray.Length != m)
                    {
                        string charName = "";
                        charName = dataArray[m].ToString();
                        songNameString = string.Concat(songNameString, charName);
                        m++;
                    }

                    songData.Add(songNameString);
                    songNameString = "";

                    Song song = new Song
                    {
                        Name = songData[0],
                        Tempo = int.Parse(songData[1]),
                        PlaylistName = songData[2],
                    };

                    dbConnection.Insert(song);

                    songData.Clear();
                }
            }





            //insert into table
            /*playlist1 = new Playlist
            {
                Name = "Majvely",
            };*/

            //playlist1.Songs.Add(new SongItem("Naděje svítá nám", 145));
            //playlist1.Songs.Add(new SongItem("Florentská romance", 140));

            /*playlist2 = new Playlist
            {
                Name = "Calienté",
            };*/

                    //playlist2.Songs.Add(new SongItem("Calienté", 145));
                    //playlist2.Songs.Add(new SongItem("Výtahovej song", 140));

                    //dbConnection.Insert(playlist1);
            //dbConnection.Insert(playlist2);

           /* Song song = new Song
            {
                Name = "Tvarohová",
                Tempo = 100,
                PlaylistName = "Majvely",
            };*/

            /*Song song1 = new Song
            {
                Name = "Naděje svítá nám a taky že jo vy tlamy",
                Tempo = 200,
                SongsId = 1,
            };*/

            //dbConnection.Insert(song);

            //dbConnection.Insert(song1);

            var playlistList = dbConnection.Table<Playlist>();
            foreach (var item in playlistList) { pListItems.Add(new Playlist(item.Name)); }

            var songList = dbConnection.Table<Song>();
            foreach (var item in songList) { sListItems.Add(new Song(item.Name, item.Tempo, item.PlaylistName)); }

            //var inList = dbConnection.Table<Playlist>();

            /*foreach (var item in inList)
            {
                if (string.Compare(item.Name, playlist1.Name ) != 0)
                {
                    dbConnection.Insert(playlist1);
                }

                if (string.Compare(item.Name, playlist2.Name) != 0)
                {
                    dbConnection.Insert(playlist2);
                }
            }*/

        }

        public int CurrentValue
        {
            set
            {
                if (value != currentValue)
                {
                    currentValue = value;
                    SetProperty(ref currentValue, value);

                    if (isPlaying)
                    {
                        timer.Stop();
                        Task.Delay(500);
                        currentInterval = 59000 / currentValue;
                        timer.AutoReset = false;
                        timer.Interval = currentInterval;
                        timer.Start();
                    }

                }

            }

            get { return currentValue; }
        }

        private void PlayStopCommand(object obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                //DevicePowerRequestLock((int)Power_Type.CPU, 0);
                currentInterval = 59000 / currentValue;
                timer.AutoReset = false;
                timer.Interval = currentInterval;
                timer.Start();
            }

            else
            {
                isPlaying = false;
                timer.Stop();
            }
        }

        private void ClickEvent(object sender, ElapsedEventArgs e)
        {
            vibrator.Vibrate(60, 100);
            //timer.AutoReset = true;
            timer.Start();
        }

        public Command StartPlayer
        {
            get { return startPlayer; }
        }
    }
}
