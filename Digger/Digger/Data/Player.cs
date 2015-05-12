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
        public KeyboradLayout UserKeyboraPreferences { get; set; } 
        public int Points { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsMusicOn { get; set; }

        public Player(string playerName)
        {
            Name = playerName;
            UserKeyboraPreferences = KeyboradLayout.Arrows;
            IsMusicOn = true;
        }
    }
}