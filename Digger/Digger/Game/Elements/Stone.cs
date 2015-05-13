using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public class Stone : IMoveable
    {
        private AnimatedGraphic _stoneGraphic;
        private Rectangle _rectangle;

        private int _speed;

        private Point _destination;
        private bool _isMoving;

        private bool _isShatter;
        private bool _wasFalling;

        #region Properties
        /// <summary>
        /// Informacja o tym, czy kamień spadał.
        /// </summary>
        public bool WasFalling
        {
            get { return _wasFalling; }
        }

        /// <summary>
        /// Czy kamień już się roztrzaskał?
        /// </summary>
        public bool IsShatter
        {
            get { return _isShatter; }
        }

        /// <summary>
        /// Czy kamień porusza się?
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        /// <summary>
        /// Obszar zajmowany przez kamień.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }
        #endregion

        /// <summary>
        /// Konstruktor kamienia.
        /// Nie porusza się, nie spada.
        /// Jego prędkość to 6.
        /// </summary>
        public Stone()
        {
            _stoneGraphic = new AnimatedGraphic();
            _rectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _isShatter = false;
            _wasFalling = false;
        }

        /// <summary>
        /// Wczytuje grafikę kamienia.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        /// <param name="assetName">Ścieżka do zasobu grafiki.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _stoneGraphic.LoadContent(content, assetName);
        }

        /// <summary>
        /// Metoda inicjalizująca pozycję kamienia.
        /// </summary>
        /// <param name="rectangle">Pozycja kamienia.</param>
        public void Initialize(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _stoneGraphic.Initialize(new Vector2(_rectangle.X + 5, _rectangle.Y + 10), 30, 25, 1, 100, Color.White);
        }

        /// <summary>
        /// Metoda rysująca kamień.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _stoneGraphic.MoveToFrame(1);
            else _stoneGraphic.MoveToFrame(0);

            _stoneGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Metoda aktualizująca grafikę kamienia.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            _stoneGraphic.Update(gameTime);
        }

        /// <summary>
        /// Metoda roztrzaskująca kamień.
        /// </summary>
        public void Shatter()
        {
            _isShatter = true;
        }

        /// <summary>
        /// Funkcja inicjalizująca wykonywanie ruchu w zadanym kierunku.
        /// </summary>
        /// <param name="direction">Kierunek poruszania.</param>
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
            _destination = new Point(_rectangle.X + x, _rectangle.Y + y);
            _isMoving = true;
        }

        /// <summary>
        /// Metoda wykonująca faktyczny ruch.
        /// </summary>
        public void Move()
        {
            if (!_isMoving) return;

            if (_destination.X != _rectangle.X)
                if (_destination.X > _rectangle.X)
                {
                    _rectangle = new Rectangle(_rectangle.X + _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X + _speed, _stoneGraphic.Position.Y);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X - _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X - _speed, _stoneGraphic.Position.Y);
                }

            if (_destination.Y != _rectangle.Y)
                if (_destination.Y > _rectangle.Y)
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y + _speed, _rectangle.Width, _rectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X, _stoneGraphic.Position.Y + _speed);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y - _speed, _rectangle.Width, _rectangle.Height);
                    _stoneGraphic.Position = new Vector2(_stoneGraphic.Position.X, _stoneGraphic.Position.Y - _speed);
                }

            if (_rectangle.X == _destination.X && _rectangle.Y == _destination.Y)
            {
                _isMoving = false;
            }
        }
        
    }
}