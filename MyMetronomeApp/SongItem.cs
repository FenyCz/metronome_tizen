/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

using Tizen.Network.Bluetooth;

namespace MyMetronomeApp
{
    // třída pro vytváření písničky, která se zobrazuje v rámci přehrávače Playlistů
    public class SongItem
    {
        public string Name { get; set; }

        public int Tempo { get; set; }

        public SongItem(string name, int tempo)
        {
            Name = name;
            Tempo = tempo;
        }
    }
}