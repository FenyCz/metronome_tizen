/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 *
 * Funkce pro práci s databází InitDatabase(), InsertDatabase() a DeleteDatabase() jsou obdobné samplu od Samsungu pro práci s SQLite
 * Název samplu: SQLite.Net.Sample
 * Programovací jazyk: C#
 * Dostupné online: https://github.com/Samsung/Tizen-CSharp-Samples/tree/master/Wearable/SQLite.NET.Sample
 * Licence samplu: 
 *
 * Copyright 2018 Samsung Electronics Co., Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

    // třída která je zodpovědná za vkládání do databáze a získávání dat o playlistech
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

        // konstruktor
        public PlayerViewModel() {

            InitDatabase();

            InsertDefault();

            startPlayer = new Command(PlayStopCommand);

            timer.Elapsed += ClickEvent;
        }

        // funkce pro inicializaci databáze
        public void InitDatabase()
        {
            bool flagCreateTable = false;

            // inicializace databaze
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider(true);

            string writablePath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;

            dbPath = Path.Combine(writablePath, "SQLite3.db3");

            // kontrola jestli existuje databázový soubor
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

        }

        // vložení dat z databáze do playlistů
        public void InsertDefault()
        {
            var playlistList = dbConnection.Table<Playlist>();
            foreach (var item in playlistList) { pListItems.Add(new Playlist(item.Name)); }

            var songList = dbConnection.Table<Song>();
            foreach (var item in songList) { sListItems.Add(new Song(item.Name, item.Tempo, item.PlaylistName)); }
        }

        // funkce pro odstranění databáze playlistů
        public void DeleteDatabase()
        {
            dbConnection.DeleteAll<Playlist>();
            dbConnection.DeleteAll<Song>();
            pListItems.Clear();
            sListItems.Clear();
        }

        // funkce pro vložení playlistů do databáze
        public void InsertDatabase(string data)
        {
            string receivedData = data;

            char[] charArray = receivedData.ToCharArray();

            char[] dataArray;

            List<string> stringList = new List<string>();

            List<string> songData = new List<string>();

            // vezmu prijate bajty a rozdelim je na songy a playlisty, oddelovac je #
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


            // vezmu songy a playlisty a postupne je roztridim 
            for (int k = 0; k < stringList.Count; k++)
            {

                dataArray = stringList[k].ToCharArray();
                
                // pokud jde o playlist - format napr. P*Majvely
                if (dataArray[0] == 'P')
                {
                    
                    int l = 2;

                    // nactu cele jeho jmeno
                    while(dataArray.Length != l)
                    {
                        string charName = "";
                        charName = dataArray[l].ToString();
                        playlistNameString = string.Concat(playlistNameString, charName);
                        l++;
                    }

                    // vytvorim novy playlist
                    Playlist playlist = new Playlist
                    {
                        Name = playlistNameString,
                    };

                    Playlist playlistForList = new Playlist(playlistNameString);

                    if (!pListItems.Any(item => item.Name == playlistForList.Name))
                    {
                        // vlozim do databaze
                        dbConnection.Insert(playlist);
                    }
                    
                    playlistNameString = ""; 
                }

                // pokud jde o song - format napr. S*Boure*140*Majvely
                else if (dataArray[0] == 'S')
                {
                    int m = 2;

                    // nactu jmeno a tempo
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

                        // ulozim data o songu do seznamu
                        songData.Add(songNameString);
                        songNameString = "";
                    }

                    // nactu playlist, do ktereho song patri
                    while(dataArray.Length != m)
                    {
                        string charName = "";
                        charName = dataArray[m].ToString();
                        songNameString = string.Concat(songNameString, charName);
                        m++;
                    }

                    // ulozim nazev playlistu do seznamu
                    songData.Add(songNameString);
                    songNameString = "";

                    // vytvorim novy song
                    Song song = new Song
                    {
                        Name = songData[0],
                        Tempo = int.Parse(songData[1]),
                        PlaylistName = songData[2],
                    };

                    Song songForList = new Song(songData[0], int.Parse(songData[1]), songData[2]);

                    if (!sListItems.Any(item => item.Name == songForList.Name && item.PlaylistName == songForList.PlaylistName))
                    {
                        // vlozim do databaze
                        dbConnection.Insert(song);
                    }

                    song = null;
                    songForList = null;

                    songData.Clear();
                }
            }
            
            // vložím playlisty do databáze s playlisty
            var playlistList = dbConnection.Table<Playlist>();
            foreach (var item in playlistList) { 
                if(!pListItems.Any(i => i.Name == item.Name))
                {
                    pListItems.Add(new Playlist(item.Name));
                }
            }

            // vložím písničky do databáze s písničkami
            var songList = dbConnection.Table<Song>();
            foreach (var item in songList) { 
                if(!sListItems.Any(i => i.Name == item.Name && i.PlaylistName == item.PlaylistName)){
                    sListItems.Add(new Song(item.Name, item.Tempo, item.PlaylistName));
                }
            }
        }

        // proměnná pro nastavení současného tempa písničky
        public int CurrentValue
        {
            set
            {
                if (value != currentValue)
                {
                    currentValue = value;
                    SetProperty(ref currentValue, value);

                    // pokud přepnu mezi písničkami, zapnu znovu
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

        // funkce pro zapnutí přehrávání playlistu
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

        // funkce pro vybrování metronomu
        private void ClickEvent(object sender, ElapsedEventArgs e)
        {
            vibrator.Vibrate(60, 100);
            timer.Start();
        }

        public Command StartPlayer
        {
            get { return startPlayer; }
        }
    }
}
