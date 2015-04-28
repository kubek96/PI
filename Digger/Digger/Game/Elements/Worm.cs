using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public enum WormState
    {
        Normal,
        Eating
    }

    public enum WormSpeed
    {
        Normal = 120,
        Eating = 20
    }

    public class Worm : IMoveable
    {
        private readonly AnimatedGraphic _wormGraphic;
        private int _framesCount;
        private Direction _direction;
        private int _speed;
        private Rectangle _wormRectangle;
        private int _redFruits;
        private int _acidShoots;
        private int _venomShoots;
        private int _kiwisCount;
        private int _mudCount;

        private bool _isDigging;
        private bool _isMoving;

        public Worm()
        {
            _wormGraphic = new AnimatedGraphic();
            _framesCount = 0;
            _speed = 120;
            _redFruits = 0;
            _isDigging = false;
            _isMoving = false;
        }

        public int MudCount
        {
            get { return _mudCount; }
            set { _mudCount = value; }
        }

        public int KiwisCount
        {
            get { return _kiwisCount; }
            set { _kiwisCount = value; }
        }

        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public int AcidShoots
        {
            get { return _acidShoots; }
            set { _acidShoots = value; }
        }

        public int RedFruits
        {
            get { return _redFruits; }
            set { _redFruits = value; }
        }

        public int FramesCount
        {
            get { return _framesCount; }
            set { _framesCount = value; }
        }

        public int VenomShoots
        {
            get { return _venomShoots; }
            set { _venomShoots = value; }
        }

        public bool IsDigging
        {
            get { return _isDigging; }
            set { _isDigging = value; }
        }

        public AnimatedGraphic WormGraphic
        {
            get { return _wormGraphic; }
        }

        public Rectangle WormRectangle
        {
            get { return _wormRectangle; }
        }

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _wormGraphic.LoadContent(content, assetName);
        }

        public void Initialize(Rectangle rectangle)
        {
            _wormRectangle = rectangle;
            _wormGraphic.Initialize(new Vector2(_wormRectangle.X+10,_wormRectangle.Y+10), 22, 20, 1, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _wormGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _wormGraphic.Update(gameTime);
        }

        public void MoveFaster(int speed, int effectTime)
        {
            
        }

        public void Heal()
        {
            
        }

        public void Move(Direction direction)
        {
            _isMoving = true;
            _wormGraphic.MoveToFrame(1);

            switch (direction)
            {
                case Direction.Up:
                    _wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y-41, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y - 41);
                    break;
                case Direction.Right:
                    _wormRectangle = new Rectangle(_wormRectangle.X + 41, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X + 41, _wormGraphic.Position.Y);
                    break;
                case Direction.Down:
                    _wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y + 41, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y + 41);
                    break;
                case Direction.Left:
                    _wormRectangle = new Rectangle(_wormRectangle.X - 41, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X - 41, _wormGraphic.Position.Y);
                    break;
            }
        }

        public Rectangle TestMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Rectangle(_wormRectangle.X, _wormRectangle.Y - 41, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Right:
                    return new Rectangle(_wormRectangle.X + 41, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Down:
                    return new Rectangle(_wormRectangle.X, _wormRectangle.Y + 41, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Left:
                    return new Rectangle(_wormRectangle.X - 41, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                default:
                    return new Rectangle();
            }
        }
    }
}