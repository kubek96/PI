using System;
using System.Collections.Generic;
using Digger.Data.Control;
using Digger.Game.Common;
using Digger.Game.Elements;
using Microsoft.Xna.Framework.Input;
using Keyboard = Digger.Data.Control.Keyboard;

namespace Digger.Data
{
    [Serializable]
    public class Player
    {
        /// <summary>
        /// Preferencje układu klawiatury gracza.
        /// </summary>
        public KeyboradLayout UserKeyboraPreferences { get; set; } 

        /// <summary>
        /// Liczba punktów, które posiada gracz.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Imię gracza.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Maksymalny level, do którego dotarł gracz.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Informacja o preferencjach dotyczących tego, czy muzyka ma być włączona, czy też nie.
        /// </summary>
        public bool IsMusicOn { get; set; }

        /// <summary>
        /// Konstruktor tworzący obiekt nowego gracza.
        /// Domyślnie za układ sterowania przyjmowane są strzałki.
        /// Muzyka jest ustawiona na włączoną.
        /// </summary>
        /// <param name="playerName">Imię nowego gracza</param>
        public Player(string playerName)
        {
            Name = playerName;
            UserKeyboraPreferences = KeyboradLayout.Arrows;
            IsMusicOn = true;
        }

        /// <summary>
        /// Konstruktor bezparametrowy używany do serializacji.
        /// </summary>
        public Player()
        {
            
        }
    }
}