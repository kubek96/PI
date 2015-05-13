using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public enum FruitType
    {
        RedFruit,
        Lemon,
        Orange,
        Kiwi,
        Watermelon,
        Plum,
        Candy
    }

    public delegate void PlayerUseDelegate(Worm worm);
    public delegate Enemy EnemyUseDelegate(Enemy enemy);

    public class Fruit
    {
        private FruitType _fruitType;
        private FixedGraphic _fruitGraphic;
        private Rectangle _fruitRectangle;
        private PlayerUseDelegate _playerUse;
        private EnemyUseDelegate _enemyUse;
        private bool _isUsed;

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        /// <param name="fruit"></param>
        public Fruit(Fruit fruit)
        {
            _fruitType = fruit._fruitType;
            _fruitGraphic = fruit._fruitGraphic.Clone();
            _fruitRectangle = fruit._fruitRectangle;
            _playerUse = fruit.PlayerUse;
            _enemyUse = fruit.EnemyUse;

            _isUsed = false;
        }

        public Fruit(FruitType fruitType, string assetName, PlayerUseDelegate playerUse, EnemyUseDelegate enemyUse)
        {
            _fruitType = fruitType;
            _playerUse = playerUse;
            _enemyUse = enemyUse;

            _fruitGraphic = new FixedGraphic();

            LoadContent(Window.Context.Content, assetName);

            _isUsed = false;
        }

        public Rectangle FruitRectangle
        {
            get { return _fruitRectangle; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _fruitGraphic.LoadContent(content, assetName);
            _fruitRectangle = new Rectangle();
        }

        public void Initialize(Rectangle rectangle)
        {
            _fruitRectangle = rectangle;
            _fruitGraphic.Initialize(_fruitRectangle, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _fruitGraphic.Draw(spriteBatch);
        }

        public PlayerUseDelegate PlayerUse
        {
            get { return _playerUse; }
        }

        public EnemyUseDelegate EnemyUse
        {
            get { return _enemyUse; }
        }

        public bool IsUsed
        {
            get { return _isUsed; }
            set { _isUsed = value; }
        }
    }
}