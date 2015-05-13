using Digger.Game.Common;
using Microsoft.Xna.Framework;

namespace Digger.Graphic
{
    /// <summary>
    /// Interfejs gwaranrujacy możliwość poruszania.
    /// </summary>
    public interface IMoveable
    {
        void MakeMove(Direction direction);
        Rectangle TestMove(Direction direction);
    }
}