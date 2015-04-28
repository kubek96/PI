using System.Runtime.Serialization.Formatters;
using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public class Worm : IMoveable
    {
        private readonly AnimatedGraphic _wormGraphic;
        private int _framesCount;
        private Direction _direction;
        private int _speed;
        private int _elapsedSpeedTime;
        private int _elapsedFreezeTime;
        private int _lastSpeed;
        private Rectangle _wormRectangle;
        private Point _destination;
        private int _redFruits;
        private int _acidShoots;
        private int _venomShoots;
        private int _kiwisCount;
        private int _plumsCount;
        private int _mudCount;
        private int _candyCount;
        private int _life;
        private int _speedEffectTime;
        private bool _isFreeze;

        private bool _isDigging;
        private bool _isMoving;

        public Worm()
        {
            _wormGraphic = new AnimatedGraphic();
            _framesCount = 0;
            _speed = _lastSpeed = 3;
            _redFruits = 0;
            _isDigging = false;
            _isMoving = false;
            _life = 10;
            _elapsedSpeedTime = 0;
            _speedEffectTime = 0;
            _isFreeze = false;
        }

        public int CandyCount
        {
            get { return _candyCount; }
            set { _candyCount = value; }
        }

        public int PlumsCount
        {
            get { return _plumsCount; }
            set { _plumsCount = value; }
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
            set
            {
                _isDigging = value;
                if (_isDigging) _lastSpeed = _speed;
            }
        }

        public int Life
        {
            get { return _life; }
            set { _life = value; }
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
            if (_isMoving) _wormGraphic.MoveToFrame(1);
            else _wormGraphic.MoveToFrame(0);

            _wormGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _wormGraphic.Update(gameTime);

            // Sprawdź, czy nie upłynął już czas przyśpieszenia
            _elapsedSpeedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_elapsedSpeedTime > _speedEffectTime)
            {
                _speed = _lastSpeed = 3;
                _elapsedSpeedTime = 0;
            }
            if (_isFreeze)
            {
                _elapsedFreezeTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_elapsedFreezeTime > 200)
                {
                    _isFreeze = false;
                    _elapsedFreezeTime = 0;
                }
            }
        }

        public void MoveFaster(int speed, int effectTime)
        {
            if (_plumsCount == 0) return;
            _plumsCount--;
            // Wyrównaj klatki przesunięcia
            _speed = speed - _speed;
            Move();
            // Ustaw nową prędkość
            _speed = _lastSpeed = speed;
            _speedEffectTime = effectTime;
            _elapsedSpeedTime = 0;
        }

        public void Heal()
        {
            _life = 10;
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public void MakeMove(Direction direction)
        {
            if (_isFreeze) return;

            _isMoving = true;

            switch (direction)
            {
                case Direction.Up:
                    _destination = new Point(_wormRectangle.X, _wormRectangle.Y - 42);
                    _direction = Direction.Up;
                    //_wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y-42, _wormRectangle.Width, _wormRectangle.Height);
                    //_wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y - 42);
                    break;
                case Direction.Right:
                    _destination = new Point(_wormRectangle.X + 42, _wormRectangle.Y);
                    _direction = Direction.Right;
                    //_wormRectangle = new Rectangle(_wormRectangle.X + 42, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    //_wormGraphic.Position = new Vector2(_wormGraphic.Position.X + 42, _wormGraphic.Position.Y);
                    break;
                case Direction.Down:
                    _destination = new Point(_wormRectangle.X, _wormRectangle.Y + 42);
                    _direction = Direction.Down;
                    //_wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y + 42, _wormRectangle.Width, _wormRectangle.Height);
                    //_wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y + 42);
                    break;
                case Direction.Left:
                    _destination = new Point(_wormRectangle.X - 42, _wormRectangle.Y);
                    _direction = Direction.Left;
                    //_wormRectangle = new Rectangle(_wormRectangle.X - 42, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    //_wormGraphic.Position = new Vector2(_wormGraphic.Position.X - 42, _wormGraphic.Position.Y);
                    break;
            }
        }

        public void Move()
        {
            if (!_isMoving) return;
            if (_isDigging) _speed = 1;
            else _speed = _lastSpeed;

            if (_destination.X != _wormRectangle.X)
                if (_destination.X > _wormRectangle.X)
                {
                    _wormRectangle = new Rectangle(_wormRectangle.X + _speed, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X + _speed, _wormGraphic.Position.Y);
                }
                else
                {
                    _wormRectangle = new Rectangle(_wormRectangle.X - _speed, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X - _speed, _wormGraphic.Position.Y);
                }

            if (_destination.Y != _wormRectangle.Y)
                if (_destination.Y > _wormRectangle.Y)
                {
                    _wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y + _speed, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y + _speed);
                }
                else
                {
                    _wormRectangle = new Rectangle(_wormRectangle.X, _wormRectangle.Y - _speed, _wormRectangle.Width, _wormRectangle.Height);
                    _wormGraphic.Position = new Vector2(_wormGraphic.Position.X, _wormGraphic.Position.Y - _speed);
                }

            if (_wormRectangle.X == _destination.X && _wormRectangle.Y == _destination.Y)
            {
                _isMoving = false;
                _isDigging = false;
                _isFreeze = true;
                _elapsedFreezeTime = 0;
            }
        }

        public Rectangle TestMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Rectangle(_wormRectangle.X, _wormRectangle.Y - 42, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Right:
                    return new Rectangle(_wormRectangle.X + 42, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Down:
                    return new Rectangle(_wormRectangle.X, _wormRectangle.Y + 42, _wormRectangle.Width, _wormRectangle.Height);
                case Direction.Left:
                    return new Rectangle(_wormRectangle.X - 42, _wormRectangle.Y, _wormRectangle.Width, _wormRectangle.Height);
                default:
                    return new Rectangle();
            }
        }
    }
}