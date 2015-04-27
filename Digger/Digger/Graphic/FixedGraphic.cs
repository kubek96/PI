using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Graphic
{
    public class FixedGraphic
    {
        protected Texture2D _image;
        protected Color _backColor;

        protected Vector2 _position;
        protected Rectangle _destRectangle;

        protected float _scale;
        protected bool _isVisible;

        public virtual void LoadContent(ContentManager content, string assetName)
        {
            _image = content.Load<Texture2D>(assetName);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize(Vector2 position, Color color, float scale = 1)
        {
            _backColor = color;
            _scale = scale;
            _position = position;

            _destRectangle = new Rectangle((int)_position.X, (int)_position.Y, _image.Width, _image.Height);
        }

        public void Initialize(Rectangle destRectangle, Color color, float scale = 1)
        {
            _backColor = color;
            _scale = scale;
            _destRectangle = destRectangle;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update()
        {

        }

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //if (_position != null) spriteBatch.Draw(_image, _position, _backColor);
            //if (_destRectangle != null) 
            spriteBatch.Draw(_image, _destRectangle, _backColor);
        }

        public Rectangle DestRectangle
        {
            get { return _destRectangle; }
        }

        public Texture2D Image
        {
            get { return _image; }
        }
    }
}