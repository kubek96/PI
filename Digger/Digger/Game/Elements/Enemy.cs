using System;
using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Enumerator pozwalający na jednoznaczne zidentyfikowanie typu wroga.
    /// </summary>
    public enum EnemyType
    {
        Mouse,
        Beetle,
        Spider,
        RedSpider,  
        Rat
    }

    public delegate Enemy EvolveDelegate(Enemy enemy);
    public delegate void TestMoveDelegate(Enemy enemy, Ground[,] grounds, Rectangle gameField);
    public delegate Shot WebShootDelegate(Enemy enemy, Worm worm, Ground[,] grounds);
    public delegate void ObserveDelegate(Enemy enemy, Worm worm, Ground[,] grounds);
    public delegate void AttackDelegate(Worm w);

    /// <summary>
    /// Klasa wroga.
    /// </summary>
    public class Enemy : IMoveable
    {
        protected AnimatedGraphic _enemyGraphic;
        protected Rectangle _rectangle;

        protected EnemyType _enemyType;
        protected int? _life;
        protected int _strenght;

        protected int _speed;
        protected int _startSpeed;
        protected int _slowEffectTime;
        protected int _elapsedSlowTime;

        protected Direction _direction;

        protected Point _destination;
        protected bool _isDigging;
        protected bool _isMoving;

        protected int _elapsedFreezeTime;
        protected int _freezeEffectTime;
        protected bool _isFreeze;

        protected bool _isKilled;
        protected bool _addAsNew;
        protected bool _sawWorm;

        protected EvolveDelegate _evolve;
        protected TestMoveDelegate _testMove;
        protected WebShootDelegate _webShoot;
        protected ObserveDelegate _observe;
        protected AttackDelegate _attack;

        #region Properties

        /// <summary>
        /// Typ wroga.
        /// </summary>
        public EnemyType EnemyType
        {
            get { return _enemyType; }
        }

        /// <summary>
        /// Informacja o tym, czy w zasięgu wzroku/węchu/słuchu wroga jest robaczek.
        /// </summary>
        public bool SawWorm
        {
            get { return _sawWorm; }
            set { _sawWorm = value; }
        }

        /// <summary>
        /// Czy został dodany jako nowy wróg.
        /// </summary>
        public bool AddAsNew
        {
            get { return _addAsNew; }
            set { _addAsNew = value; }
        }

        /// <summary>
        /// Czy przypadkiem nie jest już martwy.
        /// </summary>
        public virtual bool IsKilled
        {
            get { return _isKilled; }
            set { _isKilled = value; }
        }

        /// <summary>
        /// Czy jest zamrożony/zatrzymany.
        /// </summary>
        public bool IsFreeze
        {
            get { return _isFreeze; }
            set { _isFreeze = value; }
        }

        /// <summary>
        /// Kierunek poruszania się wroga.
        /// </summary>
        public Point Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        /// <summary>
        /// Informacja o tym, czy kopie w ziemi.
        /// </summary>
        public bool IsDigging
        {
            get { return _isDigging; }
            set { _isDigging = value; }
        }

        /// <summary>
        /// Informacja o tym, czy się porusza.
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        /// <summary>
        /// Życie wroga.
        /// Przy zejściu do wartości 0 uśmierca wroga.
        /// </summary>
        public int? Life
        {
            get { return _life; }
            set
            {
                _life = value;
                if (_life == 0) _isKilled = true;
            }
        }

        /// <summary>
        /// Kiedunek poruszania się wroga.
        /// </summary>
        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Obszar zajmowany przez wroga.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set
            {
                _rectangle = value;
                _enemyGraphic.Position = new Vector2(_rectangle.X + 10, _rectangle.Y + 10);
            }
        }

        /// <summary>
        /// Funkcja ewolucji.
        /// </summary>
        public EvolveDelegate Evolve
        {
            get { return _evolve; }
        }

        /// <summary>
        /// Funkcja sprawdzenia możliwości ruchu.
        /// </summary>
        public TestMoveDelegate TestMove
        {
            get { return _testMove; }
        }

        /// <summary>
        /// Funkcja strzału.
        /// </summary>
        public WebShootDelegate WebShoot
        {
            get { return _webShoot; }
        }

        /// <summary>
        /// Funkcja obserowania/nasłuchiwania/węchu otoczenia.
        /// </summary>
        public ObserveDelegate Observe
        {
            get { return _observe; }
        }

        /// <summary>
        /// Funkcja ataku.
        /// </summary>
        public AttackDelegate Attack
        {
            get { return _attack; }
        }

        #endregion

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        /// <param name="enemy">Obiekt wzorcowy</param>
        public Enemy(Enemy enemy)
        {
            _enemyGraphic = enemy._enemyGraphic.Clone();

            _enemyType = enemy._enemyType;
            _life = enemy._life;
            _strenght = enemy._strenght;
            _speed = enemy._speed;
            _startSpeed = enemy._startSpeed;
            _direction = enemy._direction;

            _isFreeze = enemy._isFreeze;
            _isKilled = false;

            _evolve = enemy._evolve;
            _testMove = enemy._testMove;
            _webShoot = enemy._webShoot;
            _observe = enemy._observe;
            _attack = enemy._attack;
            _sawWorm = enemy._sawWorm;
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="enemyType">Typ wroga.</param>
        /// <param name="assetName">Nazwa zasobu grafiki wroga.</param>
        /// <param name="life">Liczba życia wroga.</param>
        /// <param name="strenght">Siła ataku.</param>
        /// <param name="speed">Prędkość będąca dzielnikiem liczby 42.</param>
        /// <param name="direction">Kierunek początkowego porusznia się.</param>
        /// <param name="isFreeze">Czy jest zatrzymany w miejscu.</param>
        /// <param name="evolve">Delegatura do funkcji ewolucji.</param>
        /// <param name="testMove">Delegatura do funkcji testowania ruchu.</param>
        /// <param name="webShoot">Delegatura do funkcji strzelania.</param>
        /// <param name="observe">Delegatura do funkcji obserwowania w poszukiwaniu robaczka.</param>
        /// <param name="attack">Delegatura do funckji ataku na robaczka.</param>
        public Enemy(EnemyType enemyType, string assetName, int? life, int strenght, int speed, Direction direction, bool isFreeze, EvolveDelegate evolve,TestMoveDelegate testMove,
            WebShootDelegate webShoot, ObserveDelegate observe, AttackDelegate attack)
        {
            _enemyType = enemyType;
            _life = life;
            _strenght = strenght;

            _speed = _startSpeed = speed;
            _slowEffectTime = 0;
            _elapsedSlowTime = 0;

            _direction = direction;

            _isFreeze = isFreeze;
            _freezeEffectTime = 0;
            _elapsedFreezeTime = 0;
            
            _enemyGraphic = new AnimatedGraphic();

            _evolve = evolve;
            _testMove = testMove;
            _webShoot = webShoot;
            _observe = observe;
            _attack = attack;

            LoadContent(Window.Context.Content, assetName);

            _isKilled = false;
            _sawWorm = false;
        }

        /// <summary>
        /// Funkcja uśmiercająca wroga.
        /// </summary>
        public void Kill()
        {
            if (_enemyType == EnemyType.Rat) return;
            _isKilled = true;
        }

        /// <summary>
        /// Funkcja ładująca grafikę wroga oraz inicjalizuja obiekt jego położenia.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        /// <param name="assetName">Ścieżka zasobu grafiki wroga.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _enemyGraphic.LoadContent(content, assetName);
            _rectangle = new Rectangle();
        }

        /// <summary>
        /// Metoda inicjalizująca położenie wroga.
        /// </summary>
        /// <param name="rectangle">Obszar.</param>
        public virtual void Initialize(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _enemyGraphic.Initialize(new Vector2(_rectangle.X + 10, _rectangle.Y + 10), 22, 20, 1, 100, Color.White);
        }

        /// <summary>
        /// Metoda wykonująca rysowanie wroga.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _enemyGraphic.MoveToFrame(1);
            else _enemyGraphic.MoveToFrame(0);

            _enemyGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Medota obsługująca logikę zachowania postaci wroga.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            _enemyGraphic.Update(gameTime);
            // Sprawdź, czy nie minął już czas spowolnieniea
            if (_sawWorm && !_isFreeze) return;
            _elapsedSlowTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_elapsedSlowTime > _slowEffectTime)
            {
                _speed = _startSpeed;
                _elapsedSlowTime = 0;
            }
            if (_isFreeze)
            {
                _elapsedFreezeTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_elapsedFreezeTime > _freezeEffectTime)
                {
                    _isFreeze = false;
                    _elapsedFreezeTime = 0;
                }
            }
        }

        /// <summary>
        /// Wykoananie ruch w celu osiągnięcia odpowiedniego punktu docelowego.
        /// </summary>
        public void Move()
        {
            if (_isFreeze) return;
            if (!_isMoving) return;
            if (_isDigging) _speed = 1;
            if (!_sawWorm) _speed = _startSpeed;

            if (_destination.X != _rectangle.X)
                if (_destination.X > _rectangle.X)
                {
                    _rectangle = new Rectangle(_rectangle.X + _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X + _speed, _enemyGraphic.Position.Y);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X - _speed, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X - _speed, _enemyGraphic.Position.Y);
                }

            if (_destination.Y != _rectangle.Y)
                if (_destination.Y > _rectangle.Y)
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y + _speed, _rectangle.Width, _rectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X, _enemyGraphic.Position.Y + _speed);
                }
                else
                {
                    _rectangle = new Rectangle(_rectangle.X, _rectangle.Y - _speed, _rectangle.Width, _rectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X, _enemyGraphic.Position.Y - _speed);
                }

            if (_rectangle.X == _destination.X && _rectangle.Y == _destination.Y)
            {
                _isMoving = false;
                _isDigging = false;
            }
        }

        /// <summary>
        /// Medota wykonująca spowolnienie wroga.
        /// </summary>
        /// <param name="speed">Nowa prędkość.</param>
        /// <param name="effectTime">Czas trwania efektu.</param>
        public void SlowDown(int speed, int effectTime)
        {
            // Wyrównaj klatki przesunięcia
            _speed = _speed - speed;
            Move();
            // Ustaw nową prędkość
            _speed = speed;
            _slowEffectTime = effectTime;
            _elapsedSlowTime = 0;
        }

        /// <summary>
        /// Metoda zatrzymująca w miejscu (zamrażająca) wroga.
        /// </summary>
        /// <param name="effectTime">Czas trwania efektu.</param>
        public void Freeze(int effectTime)
        {
            _isFreeze = true;
            _freezeEffectTime = effectTime;
            _elapsedFreezeTime = 0;
        }

        /// <summary>
        /// Metoda zmieniająca punkt docelowy, w którym przemieszcza się obiekt.
        /// </summary>
        /// <param name="direction">Kierunek, w którm ma zostać wykonany ruch.</param>
        public void MakeMove(Direction direction)
        {
            // Podmień pozycję 
            int x = 0, y = 0;
            _direction = direction;
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
                    break;
                case Direction.Left:
                    x -= 42;
                    break;
            }
            _destination = new Point(_rectangle.X + x, _rectangle.Y + y);
            _isMoving = true;
        }

        /// <summary>
        /// Metoda przyśpieszająca jednostkę na czas nieokreślony.
        /// </summary>
        /// <param name="speed">Nowa prędkość.</param>
        public void MoveFaster(int speed)
        {
            // Wyrównaj klatki przesunięcia
            _speed = speed - _speed;
            _isMoving = true;
            _sawWorm = true;
            Move();
            // Ustaw nową prędkość
            _speed = speed;
        }
    }
}