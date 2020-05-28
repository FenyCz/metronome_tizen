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

namespace MyMetronomeApp.Model
{

    // třída představující Model v architektuře MVVM, uchovává informace o tempu metronomu
    class MetronomeModel
    {

        public int Tempo { get; set; } = 120;

        public MetronomeModel()
        {
        }
    }
}
