using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Graphic
{
    /// <summary>
    /// Klasa grafiki statycznej.
    /// </summary>
    public class FixedGraphic
    {
        protected Texture2D _image;
        protected Color _backColor;

        protected Vector2 _position;
        protected Rectangle _destRectangle;

        protected float _scale;
        protected bool _isVisible;

        #region Properites

        /// <summary>
        /// Docelowa pozycja grafiki.
        /// </summary>
        public Rectangle DestRectangle
        {
            get { return _destRectangle; }
        }

        /// <summary>
        /// Właściwy obraz.
        /// </summary>
        public Texture2D Image
        {
            get { return _image; }
        }

        #endregion

        /// <summary>
        /// Metoda pozwalająca na załadowanie grafiki z zasobnika.
        /// </summary>
        /// <param name="content">Zasobnik.</param>
        /// <param name="assetName">Ścieżka do zasobu grafiki.</param>
        public virtual void LoadContent(ContentManager content, string assetName)
        {
            _image = content.Load<Texture2D>(assetName);
        }

        /// <summary>
        /// Inicjalizacja obiektu na wskazanej pozycji.
        /// </summary>
        /// <param name="position">Pozycja.</param>
        /// <param name="color">Kolor tła. Biały dla przeźroczystości.</param>
        /// <param name="scale">Skala.</param>
        public void Initialize(Vector2 position, Color color, float scale = 1)
        {
            _backColor = color;
            _scale = scale;
            _position = position;

            _destRectangle = new Rectangle((int)_position.X, (int)_position.Y, _image.Width, _image.Height);
        }

        /// <summary>
        /// Inicjalizacja obiektu na wskazanym obszarze.
        /// </summary>
        /// <param name="destRectangle">Obszar.</param>
        /// <param name="color">Kolor tła. Biały dla przeźroczystości.</param>
        /// <param name="scale">Skala.</param>
        public void Initialize(Rectangle destRectangle, Color color, float scale = 1)
        {
            _backColor = color;
            _scale = scale;
            _destRectangle = destRectangle;
        }

        /// <summary>
        /// Metoda klonująca obiekt grafiki statycznej.
        /// </summary>
        /// <returns>Kopia grafiki.</returns>
        public FixedGraphic Clone()
        {
            FixedGraphic temp = new FixedGraphic();
            temp._image = _image;
            temp._position = new Vector2(_position.X, _position.Y);
            temp._scale = _scale;
            temp._isVisible = _isVisible;
            temp._backColor = _backColor;
            temp._destRectangle = new Rectangle(_destRectangle.X, _destRectangle.Y, _destRectangle.Width, _destRectangle.Height);
            return temp;
        }

        /// <summary>
        /// Metoda rysująca.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //if (_position != null) spriteBatch.Draw(_image, _position, _backColor);
            //if (_destRectangle != null) 
            spriteBatch.Draw(_image, _destRectangle, _backColor);
        }
    }
}