using Digger.Game.Common;
using Microsoft.Xna.Framework;

namespace Digger.Graphic
{
    public interface IMoveable
    {
        void MakeMove(Direction direction);
        Rectangle TestMove(Direction direction);
    }
}