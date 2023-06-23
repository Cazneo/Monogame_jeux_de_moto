using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet_jeux_moto.Model
{
    public class GameSettingsModel
    {
        // La vitesse de la moto
        public float MotoSpeed { get; set; }

        // Multiplicateur de la vitesse des voitures
        public float CarSpeedMultiplier { get; set; }

        // L'intervalle d'apparition des voitures
        public float CarSpawnInterval { get; set; }

        // La vitesse des voitures
        public float CarSpeed { get; set; }

        // Le temps écoulé depuis la dernière voiture
        public float TimeSinceLastCar { get; set; }
    }

}
