/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Pims.Contacts.ContactsViews;

namespace MyMetronomeApp
{
    // třída pro vytváření playlistů, které se vkládají do databáze
    public class Playlist
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        //public List<SongItem> Songs { get; set; }

        public Playlist()
        {
        }

        public Playlist(string name)
        {
            Name = name;
        }
    }
    
    // třída pro vytváření písniček, které se vkládají do databáze
    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Tempo { get; set; }

        public string PlaylistName { get; set; }

        public Song()
        {
        }

        public Song(string name, int tempo, string playlist)
        {
            Name = name;
            Tempo = tempo;
            PlaylistName = playlist;
        }
    }
    
}
