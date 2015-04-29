using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public class Purse
    {
        private AnimatedGraphic _purseGraphic;
        private Rectangle _purseRectangle;

        private int _speed;

        private Fruit _fruit;

        private Point _destination;
        private bool _isMoving;

        private bool _isShatter;
        private bool _wasFalling;

        public Purse(Fruit fruit)
        {
            _purseGraphic = new AnimatedGraphic();
            _purseRectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _isShatter = false;
            _wasFalling = false;

            _fruit = fruit;
        }

        public Purse()
        {
            _purseGraphic = new AnimatedGraphic();
            _purseRectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _fruit = null;
            _isShatter = false;
            _wasFalling = false;
        }

        public Fruit Fruit
        {
            get { return _fruit; }
            set { _fruit = value; }
        }

        public bool IsShatter
        {
            get { return _isShatter; }
            set { _isShatter = value; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _purseGraphic.LoadContent(content, assetName);
        }

        public void Initialize(Rectangle rectangle)
        {
            _purseRectangle = rectangle;
            _purseGraphic.Initialize(new Vector2(_purseRectangle.X + 5, _purseRectangle.Y + 10), 30, 25, 1, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _purseGraphic.MoveToFrame(1);
            else _purseGraphic.MoveToFrame(0);

            _purseGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _purseGraphic.Update(gameTime);
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
                    _wasFalling = true;
                    y += 42;
                    break;
                case Direction.Left:
                    x -= 42;
                    break;
            }
            _destination = new Point(_purseRectangle.X + x, _purseRectangle.Y + y);
            _isMoving = true;
        }

        public bool WasFalling
        {
            get { return _wasFalling; }
        }

        public void Move()
        {
            if (!_isMoving) return;

            if (_destination.X != _purseRectangle.X)
                if (_destination.X > _purseRectangle.X)
                {
                    _purseRectangle = new Rectangle(_purseRectangle.X + _speed, _purseRectangle.Y, _purseRectangle.Width, _purseRectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X + _speed, _purseGraphic.Position.Y);
                }
                else
                {
                    _purseRectangle = new Rectangle(_purseRectangle.X - _speed, _purseRectangle.Y, _purseRectangle.Width, _purseRectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X - _speed, _purseGraphic.Position.Y);
                }

            if (_destination.Y != _purseRectangle.Y)
                if (_destination.Y > _purseRectangle.Y)
                {
                    _purseRectangle = new Rectangle(_purseRectangle.X, _purseRectangle.Y + _speed, _purseRectangle.Width, _purseRectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X, _purseGraphic.Position.Y + _speed);
                }
                else
                {
                    _purseRectangle = new Rectangle(_purseRectangle.X, _purseRectangle.Y - _speed, _purseRectangle.Width, _purseRectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X, _purseGraphic.Position.Y - _speed);
                }

            if (_purseRectangle.X == _destination.X && _purseRectangle.Y == _destination.Y)
            {
                _isMoving = false;
            }
        }

        public Fruit Shatter()
        {
            if (_fruit == null)
            {
                _isShatter = true;
                return null;
            }
            _fruit.Initialize(_purseRectangle);
            _isShatter = true;
            return _fruit;
        }

        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        public Rectangle PurseRectangle
        {
            get { return _purseRectangle; }
            set { _purseRectangle = value; }
        }
    }
}