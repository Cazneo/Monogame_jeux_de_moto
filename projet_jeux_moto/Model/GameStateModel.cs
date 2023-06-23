using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jeux_moto;

namespace projet_jeux_moto.Model
{
    public class GameStateModel
    {
        public GameState CurrentGameState { get; set; }
        public bool IsPaused { get; set; }
        public int Score { get; set; }
        public float Distance { get; set; }
    }

}
