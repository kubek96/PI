using System.Collections.Generic;
using Digger.Game.Common;
using Microsoft.Xna.Framework.Input;

namespace Digger.Data.Control
{
    /// <summary>
    /// Enumerator możliwych konfiguracji klawiatury.
    /// </summary>
    public enum KeyboradLayout
    {
        Arrows,
        WSAD
    }

    /// <summary>
    /// Klasa zarządcy klawiaturą.
    /// Zawiera słownik możliwych konfiguracji sterowania.
    /// </summary>
    public static class Keyboard
    {
         public static Dictionary<KeyboradLayout, Dictionary<Keys, Direction>> Layout = new Dictionary<KeyboradLayout, Dictionary<Keys, Direction>>()
         {
             {KeyboradLayout.Arrows, new Dictionary<Keys, Direction>()
             {
                 {Keys.Up, Direction.Up},
                 {Keys.Right, Direction.Right},
                 {Keys.Down, Direction.Down},
                 {Keys.Left, Direction.Left}
             }},
             {KeyboradLayout.WSAD, new Dictionary<Keys, Direction>()
             {
                 {Keys.W, Direction.Up},
                 {Keys.D, Direction.Right},
                 {Keys.S, Direction.Down},
                 {Keys.A, Direction.Left}
             }}
         };
    }
}