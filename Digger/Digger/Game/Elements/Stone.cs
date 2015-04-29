using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public class Stone
    {
        private AnimatedGraphic _stoneGraphic;
        private Rectangle _stoneRectangle;

        private int _speed;

        private Point _destination;
        private bool _isMoving;

        private bool _isShatter;
        private bool _wasFalling;

        public Stone()
        {
            _stoneGraphic = new AnimatedGraphic();
            _stoneRectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _isShatter = false;
            _wasFalling = false;
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _stoneGraphic.LoadContent(content, assetName);
        }

        public void Initialize(Rectangle rectangle)
        {
            _stoneRectangle = rectangle;
            _stoneGraphic.Initialize(new Vector2(_stoneRectangle.X + 5, _stoneRectangle.Y + 10), 30, 25, 1, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _stoneGraphic.MoveToFrame(1);
            else _stoneGraphic.MoveToFrame(0);

            _stoneGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _stoneGraphic.Update(gameTime);
        }

        public bool WasFalling
        {
            get { return _wasFalling; }
        }

        public void Shatter()
        {
            _isShatter = true;
        }

        public bool IsShatter
        {
            get { return _isShatter; }
        }

        public void MakeMove(Direction direction)
        {
            int x = 0, y = 0;
            switch (direction)
            {
                case Direction.Up:
                    y -= 42;
                    break;
                case Direction.Right:
                    x += 42;
                    break;
                case Direction.Down:
                    y += 42;
                    _wasFalling = true;
                    break;
                case Direction.Left:
                    x -= 42;
                    break;
            }
            _destination = new Point(_stoneRectangle.X + x, _stoneRectangle.Y + y);
            _isMoving = true;
        }

        public void Move()
        {
            if (!_isMoving) return;

            if (_destination.X != _stoneRectangle.X)
                if (_destination.X > _stoneRectangle.X)
                {
                    _stoneRectangle = new Rectangle(_stoneRectangle.X + _speed, _stoneRectangle.Y, _stoneRectangle.Width, _stoneRectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X + _speed, _stoneGraphic.Position.Y);
                }
                else
                {
                    _stoneRectangle = new Rectangle(_stoneRectangle.X - _speed, _stoneRectangle.Y, _stoneRectangle.Width, _stoneRectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X - _speed, _stoneGraphic.Position.Y);
                }

            if (_destination.Y != _stoneRectangle.Y)
                if (_destination.Y > _stoneRectangle.Y)
                {
                    _stoneRectangle = new Rectangle(_stoneRectangle.X, _stoneRectangle.Y + _speed, _stoneRectangle.Width, _stoneRectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X, _stoneGraphic.Position.Y + _speed);
                }
                else
                {
                    _stoneRectangle = new Rectangle(_stoneRectangle.X, _stoneRectangle.Y - _speed, _stoneRectangle.Width, _stoneRectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X, _stoneGraphic.Position.Y - _speed);
                }

            if (_stoneRectangle.X == _destination.X && _stoneRectangle.Y == _destination.Y)
            {
                _isMoving = false;
            }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        public Rectangle StoneRectangle
        {
            get { return _stoneRectangle; }
            set { _stoneRectangle = value; }
        }
    }
}