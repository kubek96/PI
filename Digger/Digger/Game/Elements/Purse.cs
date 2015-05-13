using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Klasa sakiewek.
    /// </summary>
    public class Purse : IMoveable
    {
        private AnimatedGraphic _purseGraphic;
        private Rectangle _rectangle;

        private int _speed;

        private Fruit _fruit;

        private Point _destination;
        private bool _isMoving;

        private bool _isShatter;
        private bool _wasFalling;

        #region Properties

        /// <summary>
        /// Owoc przetrzymywany wewnątrz sakiewki lub null, jeżeli jest pusta.
        /// </summary>
        public Fruit Fruit
        {
            get { return _fruit; }
            set { _fruit = value; }
        }

        /// <summary>
        /// Czy sakiewka roztrzaskała się.
        /// </summary>
        public bool IsShatter
        {
            get { return _isShatter; }
            set { _isShatter = value; }
        }

        /// <summary>
        /// Czy sakiewka upadała?
        /// </summary>
        public bool WasFalling
        {
            get { return _wasFalling; }
        }

        /// <summary>
        /// Czy sakieka przemieszcza się?
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        /// <summary>
        /// Przstrzeń zajmowana przez sakiewkę.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        #endregion

        /// <summary>
        /// Konstruktor kopiujący.
        /// </summary>
        /// <param name="fruit">Obiekt wzorcowy.</param>
        public Purse(Fruit fruit)
        {
            _purseGraphic = new AnimatedGraphic();
            _rectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _isShatter = false;
            _wasFalling = false;

            _fruit = fruit;
        }

        /// <summary>
        /// Konstruktor tworzący sakiewkę.
        /// Nie spada, nie jest roztrzaskana. Nie ma owocu.
        /// </summary>
        public Purse()
        {
            _purseGraphic = new AnimatedGraphic();
            _rectangle = new Rectangle();
            _destination = new Point();
            _isMoving = false;
            _speed = 6;
            _fruit = null;
            _isShatter = false;
            _wasFalling = false;
        }

        /// <summary>
        /// Metoda wczytująca grafikę sakiewki.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="assetName"></param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _purseGraphic.LoadContent(content, assetName);
        }

        /// <summary>
        /// Inicjalizacja pozycji sakiewki.
        /// </summary>
        /// <param name="rectangle">Pozycja sakiewki.</param>
        public void Initialize(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _purseGraphic.Initialize(new Vector2(_rectangle.X + 5, _rectangle.Y + 10), 30, 25, 1, 100, Color.White);
        }

        /// <summary>
        /// Metoda rysująca sakiewkę.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _purseGraphic.MoveToFrame(1);
            else _purseGraphic.MoveToFrame(0);

            _purseGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Metoda uaktalniająca grafikę sakiewki.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _purseGraphic.Update(gameTime);
        }

        /// <summary>
        /// Wykoananie ruchu w zadanym kierunku.
        /// </summary>
        /// <param name="direction">Kierunek poruszania się.</param>
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
            _destination = new Point(_rectangle.X + x, _rectangle.Y + y);
            _isMoving = true;
        }

        /// <summary>
        /// Wykonaj ruch w zadanym przez punkt docelowym kierunku.
        /// </summary>
        public void Move()
        {
            if (!_isMoving) return;

            if (_destination.X != _rectangle.X)
                if (_destination.X > _rectangle.X)
                {
                    _rectangle = new Rectangle(_rectangle.X + _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X + _speed, _purseGraphic.Position.Y);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X - _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X - _speed, _purseGraphic.Position.Y);
                }

            if (_destination.Y != _rectangle.Y)
                if (_destination.Y > _rectangle.Y)
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y + _speed, _rectangle.Width, _rectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X, _purseGraphic.Position.Y + _speed);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y - _speed, _rectangle.Width, _rectangle.Height);
                    _purseGraphic.Position = new Vector2(_purseGraphic.Position.X, _purseGraphic.Position.Y - _speed);
                }

            if (_rectangle.X == _destination.X && _rectangle.Y == _destination.Y)
            {
                _isMoving = false;
            }
        }

        /// <summary>
        /// Roztrzaskaj sakiewkę.
        /// </summary>
        /// <returns>Obiekt owocu przetrzymywanego w sakiewce lub null jeżeli sakiewka była pusta.</returns>
        public Fruit Shatter()
        {
            if (_fruit == null)
            {
                _isShatter = true;
                return null;
            }
            _fruit.Initialize(_rectangle);
            _isShatter = true;
            return _fruit;
        }

    }
}