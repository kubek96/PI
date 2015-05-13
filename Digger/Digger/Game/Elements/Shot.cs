using System;
using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Enumerator pozwalaj¹cy stwierdziæ typ strza³u.
    /// </summary>
    public enum ShotType
    {
        Acid,
        Venom,
        Web
    }

    public delegate void ShootEnemy(Enemy enemy);

    /// <summary>
    /// Klasa strza³ów.
    /// </summary>
    public class Shot : IMoveable
    {
        private AnimatedGraphic _shotGraphic;
        private Rectangle _rectangle;
        private Direction _direction;
        private ShotType _shotType;
        private int _speed;
        private bool _shootSomething;
        private ShootEnemy _shootEnemy;

        #region Properites

        /// <summary>
        /// Metoda zestrzelenia wroga.
        /// </summary>
        public ShootEnemy ShootEnemy
        {
            get { return _shootEnemy; }
        }

        /// <summary>
        /// Obszar zajmowany przez strza³
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
        }

        /// <summary>
        /// Czy coœ ju¿ zosta³o zestrzelone?
        /// </summary>
        public bool ShootSomething
        {
            get { return _shootSomething; }
            set { _shootSomething = value; }
        }

        #endregion

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="shotType">Typ strza³u.</param>
        /// <param name="assetName">Œcie¿ka do zasobu grafiki strza³u.</param>
        /// <param name="shootEnemy">Funkcja zestrzelenia wroga.</param>
        public Shot(ShotType shotType, string assetName, ShootEnemy shootEnemy)
        {
            _shotType = shotType;
            _shotGraphic = new AnimatedGraphic();
            _speed = 14;
            _shootSomething = false;
            LoadContent(Window.Context.Content, assetName);
            _shootEnemy = shootEnemy;
        }

        /// <summary>
        /// Konstruktor kopiuj¹cy.
        /// </summary>
        /// <param name="shot">Obiekt wzorcowy.</param>
        public Shot(Shot shot)
        {
            _shotGraphic = shot._shotGraphic.Clone();
            _shotType = shot._shotType;
            _speed = shot._speed;
            _shootEnemy = shot._shootEnemy;
        }

        /// <summary>
        /// Meotda wczytuj¹ca grafikê strza³u.
        /// </summary>
        /// <param name="content">Obiekt managera zasobów.</param>
        /// <param name="assetName">Œcie¿ka do zasobu grafiki.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _shotGraphic.LoadContent(content, assetName);
        }

        /// <summary>
        /// Metoda inicjalizuj¹ca po³o¿enie pocz¹tkowe strza³u.
        /// </summary>
        /// <param name="rectangle">Obszar zajmowany na pocz¹tku przez strza³.</param>
        /// <param name="direction">Kierunek strza³u.</param>
        public void Initialize(Rectangle rectangle, Direction direction)
        {
            _direction = direction;
            _rectangle = rectangle;
            _shotGraphic.Initialize(new Vector2(_rectangle.X + 15, _rectangle.Y + 15), 10, 10, 2, 100, Color.White);
        }

        /// <summary>
        /// Metoda rysuj¹ca obiekt strza³u.
        /// </summary>
        /// <param name="spriteBatch">Pow³oka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _shotGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Metoda aktualizuj¹ca grafikê strza³u.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            _shotGraphic.Update(gameTime);
        }

        /// <summary>
        /// Metoda poruszania strza³u.
        /// </summary>
        public void Move()
        {
            switch (_direction)
            {
                case Direction.Up:
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y - _speed, _rectangle.Width, _rectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X, _shotGraphic.Position.Y - _speed);
                    break;
                case Direction.Right:
                    _rectangle = new Rectangle(_rectangle.X + _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X + _speed, _shotGraphic.Position.Y);
                    break;
                case Direction.Down:
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y + _speed, _rectangle.Width, _rectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X, _shotGraphic.Position.Y + _speed);
                    break;
                case Direction.Left:
                    _rectangle = new Rectangle(_rectangle.X - _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X - _speed, _shotGraphic.Position.Y);
                    break;
            }
        }

        /// <summary>
        /// Metoda wykoania ruchu.
        /// </summary>
        /// <param name="direction"></param>
        public void MakeMove(Direction direction)
        {
            _direction = direction;
        }
    }
}