using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Enumerator pozwalający na jednoznaczne rozróżnienie między typami ziemi.
    /// </summary>
    public enum GroundType
    {
        Normal,
        Free
    }

    /// <summary>
    /// Klasa ziemi.
    /// </summary>
    public class Ground
    {
        private FixedGraphic _freeGroundGraphic;
        private FixedGraphic _groundGraphic;
        private GroundType _groundType;

        #region Properites
        /// <summary>
        /// Typ ziemi.
        /// </summary>
        public GroundType GroundType
        {
            get { return _groundType; }
            set { _groundType = value; }
        }

        /// <summary>
        /// Przestrzeń zajmowana przez ziemię.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _groundGraphic.DestRectangle; }
        }
        #endregion

        /// <summary>
        /// Konstruktor. Inicjalizuje obiekty grafiki ziemi.
        /// </summary>
        public Ground()
        {
            _groundGraphic = new FixedGraphic();
            _freeGroundGraphic = new FixedGraphic();
        }

        /// <summary>
        /// Metoda wczytująca grafiki ziemi.
        /// </summary>
        /// <param name="content">Menager zasobów.</param>
        /// <param name="normalGroundGraphic">Ścieżka do zasobu normalnej ziemi.</param>
        /// <param name="freeGroundGraphic">Ścieżka do zasobu pustej ziemi.</param>
        public void LoadContent(ContentManager content, string normalGroundGraphic, string freeGroundGraphic)
        {
            _groundGraphic.LoadContent(content, normalGroundGraphic);
            _freeGroundGraphic.LoadContent(content, freeGroundGraphic);
        }

        /// <summary>
        /// Metoda inicjalizująca pozycję grudki ziemi.
        /// </summary>
        /// <param name="position">Pozycja.</param>
        /// <param name="groundType">Typ ziemi w jakim ma obecnie ona się pokazywać.</param>
        public void Initialize(Vector2 position, GroundType groundType=GroundType.Normal)
        {
            _groundType = groundType;
            _groundGraphic.Initialize(position, Color.White);
            _freeGroundGraphic.Initialize(position, Color.White);
        }

        /// <summary>
        /// Metoda rysująca zimię w zależności od jej typu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_groundType == GroundType.Normal)
            {
                _groundGraphic.Draw(spriteBatch);
                return;
            }
            if (_groundType == GroundType.Free)
            {
                _freeGroundGraphic.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Metoda aktualizująca.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            
        }
    }
}