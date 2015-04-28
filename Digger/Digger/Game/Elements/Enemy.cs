using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

    public delegate void EvolveDelegate(Enemy enemy);
    public delegate void MakeMoveDelegate(Enemy enemy, Direction[] availableDirections);
    public delegate Shot WebShootDelegate();
    public delegate void ObserveDelegate(Enemy enemy, Worm worm);
    public delegate void AttackDelegate(Worm w);

    public class Enemy
    {
        private AnimatedGraphic _enemyGraphic;
        private Rectangle _enemyRectangle;

        private EnemyType _enemyType;
        private int? _life;
        private int _strenght;
        private int _speed;
        private Direction _direction;

        private Point _destination;
        private bool _isDigging;
        private bool _isMoving;

        private bool _isFreeze;
        private bool _isKilled;

        private EvolveDelegate _evolve;
        private MakeMoveDelegate _makeMove;
        private WebShootDelegate _webShoot;
        private ObserveDelegate _observe;
        private AttackDelegate _attack;

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
            _direction = enemy._direction;

            _isFreeze = enemy._isFreeze;
            _isKilled = false;

            _evolve = enemy._evolve;
            _makeMove = enemy._makeMove;
            _webShoot = enemy._webShoot;
            _observe = enemy._observe;
            _attack = enemy._attack;
        }

        public Enemy(EnemyType enemyType, string assetName, int? life, int strenght, int speed, Direction direction, bool isFreeze, EvolveDelegate evolve,MakeMoveDelegate makeMove,
            WebShootDelegate webShoot, ObserveDelegate observe, AttackDelegate attack)
        {
            _enemyType = enemyType;
            _life = life;
            _strenght = strenght;
            _speed = speed;
            _direction = direction;
            _isFreeze = isFreeze;
            
            _enemyGraphic = new AnimatedGraphic();

            _evolve = evolve;
            _makeMove = makeMove;
            _webShoot = webShoot;
            _observe = observe;
            _attack = attack;

            LoadContent(Game1.Context.Content, assetName);

            _isKilled = false;
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _enemyGraphic.LoadContent(content, assetName);
            _enemyRectangle = new Rectangle();
        }

        public void Initialize(Rectangle rectangle)
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
        }

        public bool IsKilled
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

        public void Move()
        {
            if (!_isMoving) return;
            if (_isDigging) _speed = 1;
            else _speed = 3;

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

        public void Kill()
        {
            
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

        public MakeMoveDelegate MakeMove
        {
            get { return _makeMove; }
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
    }
}