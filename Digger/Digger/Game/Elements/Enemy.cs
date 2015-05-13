using System;
using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Digger.Game.Elements
{
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

    public class Enemy
    {
        protected AnimatedGraphic _enemyGraphic;
        protected Rectangle _enemyRectangle;

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

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        /// <param name="enemy"></param>
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

        public EnemyType EnemyType
        {
            get { return _enemyType; }
        }

        public bool SawWorm
        {
            get { return _sawWorm; }
            set { _sawWorm = value; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _enemyGraphic.LoadContent(content, assetName);
            _enemyRectangle = new Rectangle();
        }

        public virtual void Initialize(Rectangle rectangle)
        {
            _enemyRectangle = rectangle;
            _enemyGraphic.Initialize(new Vector2(_enemyRectangle.X + 10, _enemyRectangle.Y + 10), 22, 20, 1, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isMoving) _enemyGraphic.MoveToFrame(1);
            else _enemyGraphic.MoveToFrame(0);

            _enemyGraphic.Draw(spriteBatch);
        }

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

        public bool AddAsNew
        {
            get { return _addAsNew; }
            set { _addAsNew = value; }
        }

        public virtual bool IsKilled
        {
            get { return _isKilled; }
            set { _isKilled = value; }
        }

        public bool IsFreeze
        {
            get { return _isFreeze; }
            set { _isFreeze = value; }
        }

        public Point Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        public bool IsDigging
        {
            get { return _isDigging; }
            set { _isDigging = value; }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }

        public void Kill()
        {
            if (_enemyType == EnemyType.Rat) return;
            _isKilled = true;
        }

        public void Move()
        {
            if (_isFreeze) return;
            if (!_isMoving) return;
            if (_isDigging) _speed = 1;
            if (!_sawWorm) _speed = _startSpeed;

            if (_destination.X != _enemyRectangle.X)
                if (_destination.X > _enemyRectangle.X)
                {
                    _enemyRectangle = new Rectangle(_enemyRectangle.X + _speed, _enemyRectangle.Y, _enemyRectangle.Width, _enemyRectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X + _speed, _enemyGraphic.Position.Y);
                }
                else
                {
                    _enemyRectangle = new Rectangle(_enemyRectangle.X - _speed, _enemyRectangle.Y, _enemyRectangle.Width, _enemyRectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X - _speed, _enemyGraphic.Position.Y);
                }

            if (_destination.Y != _enemyRectangle.Y)
                if (_destination.Y > _enemyRectangle.Y)
                {
                    _enemyRectangle = new Rectangle(_enemyRectangle.X, _enemyRectangle.Y + _speed, _enemyRectangle.Width, _enemyRectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X, _enemyGraphic.Position.Y + _speed);
                }
                else
                {
                    _enemyRectangle = new Rectangle(_enemyRectangle.X, _enemyRectangle.Y - _speed, _enemyRectangle.Width, _enemyRectangle.Height);
                    _enemyGraphic.Position = new Vector2(_enemyGraphic.Position.X, _enemyGraphic.Position.Y - _speed);
                }

            if (_enemyRectangle.X == _destination.X && _enemyRectangle.Y == _destination.Y)
            {
                _isMoving = false;
                _isDigging = false;
            }
        }

        public Rectangle EnemyRectangle
        {
            get { return _enemyRectangle; }
            set
            {
                _enemyRectangle = value;
                _enemyGraphic.Position = new Vector2(_enemyRectangle.X + 10, _enemyRectangle.Y + 10);
            }
        }

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

        public void Freeze(int effectTime)
        {
            _isFreeze = true;
            _freezeEffectTime = effectTime;
            _elapsedFreezeTime = 0;
        }

        public int? Life
        {
            get { return _life; }
            set
            {
                _life = value;
                if (_life == 0) _isKilled = true;
            }
        }

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public EvolveDelegate Evolve
        {
            get { return _evolve; }
        }

        public TestMoveDelegate TestMove
        {
            get { return _testMove; }
        }

        public WebShootDelegate WebShoot
        {
            get { return _webShoot; }
        }

        public ObserveDelegate Observe
        {
            get { return _observe; }
        }

        public AttackDelegate Attack
        {
            get { return _attack; }
        }

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
            _destination = new Point(_enemyRectangle.X + x, _enemyRectangle.Y + y);
            _isMoving = true;
        }

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