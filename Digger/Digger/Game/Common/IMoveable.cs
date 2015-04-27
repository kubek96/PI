using Digger.Game.Common;
using Microsoft.Xna.Framework;

namespace Digger.Graphic
{
    public interface IMoveable
    {
        void Move(Direction direction);
        Rectangle TestMove(Direction direction);
    }
}