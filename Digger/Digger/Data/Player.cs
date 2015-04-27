using System.Collections.Generic;
using Digger.Game.Common;
using Digger.Game.Elements;
using Microsoft.Xna.Framework.Input;

namespace Digger.Data
{
    public class Player
    {
        private Dictionary<Keys, Direction> _userControls;

        public Player()
        {
            _userControls = new Dictionary<Keys, Direction>(4);
            _userControls.Add(Keys.Up, Direction.Up);
            _userControls.Add(Keys.Right, Direction.Right);
            _userControls.Add(Keys.Down, Direction.Down);
            _userControls.Add(Keys.Left, Direction.Left); 
        }

        public Dictionary<Keys, Direction> UserControls
        {
            get { return _userControls; }
        }
    }
}