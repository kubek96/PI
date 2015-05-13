using Digger.Game.Common;
using Microsoft.Xna.Framework;

namespace Digger.Graphic
{
    /// <summary>
    /// Interfejs gwaranrujacy możliwość poruszania.
    /// </summary>
    public interface IMoveable
    {
        void Move();
        void MakeMove(Direction direction);
    }
}