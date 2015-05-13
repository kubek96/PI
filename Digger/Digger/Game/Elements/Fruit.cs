using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Enumerator pozwalający na rozróżnienie typów owoców.
    /// </summary>
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

    /// <summary>
    /// Klasa owocu.
    /// </summary>
    public class Fruit
    {
        private FruitType _fruitType;
        private FixedGraphic _fruitGraphic;
        private Rectangle _rectangle;
        private PlayerUseDelegate _playerUse;
        private EnemyUseDelegate _enemyUse;
        private bool _isUsed;

        #region Properties

        /// <summary>
        /// Obszar zajmowany przez owoc.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
        }

        /// <summary>
        /// Funkcja wywoływana przy zjedzeniu owocu przez gracza.
        /// </summary>
        public PlayerUseDelegate PlayerUse
        {
            get { return _playerUse; }
        }

        /// <summary>
        /// Funkcja wywoływana przy zjedzeniu owocy przez wroga.
        /// </summary>
        public EnemyUseDelegate EnemyUse
        {
            get { return _enemyUse; }
        }

        /// <summary>
        /// Czy owoc został już zjedzony?
        /// </summary>
        public bool IsUsed
        {
            get { return _isUsed; }
            set { _isUsed = value; }
        }

        #endregion

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        /// <param name="fruit">Obiekt wzorcowy.</param>
        public Fruit(Fruit fruit)
        {
            _fruitType = fruit._fruitType;
            _fruitGraphic = fruit._fruitGraphic.Clone();
            _rectangle = fruit._rectangle;
            _playerUse = fruit.PlayerUse;
            _enemyUse = fruit.EnemyUse;

            _isUsed = false;
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="fruitType">Typ owocu.</param>
        /// <param name="assetName">Ścieżka do zasobu grafiki.</param>
        /// <param name="playerUse">Funkcja zjedzenia owocu przez gracza.</param>
        /// <param name="enemyUse">Funkcja zjedzenia owocu przez wroga.</param>
        public Fruit(FruitType fruitType, string assetName, PlayerUseDelegate playerUse, EnemyUseDelegate enemyUse)
        {
            _fruitType = fruitType;
            _playerUse = playerUse;
            _enemyUse = enemyUse;

            _fruitGraphic = new FixedGraphic();

            LoadContent(Window.Context.Content, assetName);

            _isUsed = false;
        }

        /// <summary>
        /// Metoda wczytująca grafike owocu.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        /// <param name="assetName">Ścieżka do zasobu.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _fruitGraphic.LoadContent(content, assetName);
            _rectangle = new Rectangle();
        }

        /// <summary>
        /// Metoda inicjalizująca położenie owocu.
        /// </summary>
        /// <param name="rectangle">Obszar, który ma być zajmowany przez owoc.</param>
        public void Initialize(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _fruitGraphic.Initialize(_rectangle, Color.White);
        }

        /// <summary>
        /// Metoda rysująca owoc.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _fruitGraphic.Draw(spriteBatch);
        }
    }
}