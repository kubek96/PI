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

    public delegate Enemy EvolveDelegate(Enemy enemy);
    public delegate void MoveDelegate(Enemy enemy, Direction[] availableDirections);
    public delegate Shot WebShootDelegate();
    public delegate void ObserveDelegate();
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

        private bool _isFreeze;
        private bool _isKilled;

        private EvolveDelegate _evolve;
        private MoveDelegate _move;
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
            _move = enemy._move;
            _webShoot = enemy._webShoot;
            _observe = enemy._observe;
            _attack = enemy._attack;
        }

        public Enemy(EnemyType enemyType, string assetName, int? life, int strenght, int speed, Direction direction, bool isFreeze, EvolveDelegate evolve,MoveDelegate move,
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
            _move = move;
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
            _enemyGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _enemyGraphic.Update(gameTime);
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

        public MoveDelegate Move
        {
            get { return _move; }
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