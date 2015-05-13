using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Graphic
{
    /// <summary>
    /// Klasa animowanej grafiki. 
    /// Dziedziczy po grafice statycznej.
    /// </summary>
    public class AnimatedGraphic : FixedGraphic
    {
        private Rectangle _sourceRectangle;

        private int _frameCount;
        private int _currentFrame;
        private int _frameTime;
        private int _elapsedTime;
        private int _frameWidth;
        private int _frameHeight;

        private bool _active;
        private bool _looping;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public AnimatedGraphic()
        {
            _destRectangle = new Rectangle();
            _sourceRectangle = new Rectangle();
        }

        #region Properites

        /// <summary>
        /// Pozycja grafiki.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Szerokość wyświetlanej ramki.
        /// </summary>
        public int FrameWidth
        {
            get { return _frameWidth; }
        }

        /// <summary>
        /// Wysokość wyświetlanej ramki.
        /// </summary>
        public int FrameHeight
        {
            get { return _frameHeight; }
        }

        /// <summary>
        /// Czy grafika jest aktywna?
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        /// <summary>
        /// Czas jednej wyświetlania jednej ramki.
        /// </summary>
        public int FrameTime
        {
            get { return _frameTime; }
        }

        #endregion

        /// <summary>
        /// Inicjalizacjia
        /// </summary>
        /// <param name="position">Pozycja docelowa.</param>
        /// <param name="frameWidth">Szerokość jednej ramki.</param>
        /// <param name="frameHeight">Wysokość jednej ramki.</param>
        /// <param name="frameCount">Ilość ramek.</param>
        /// <param name="frameTime">Czas wyświetlania jednej ramki.</param>
        /// <param name="color">Kolor tła obrazu.</param>
        /// <param name="looping">Czy animacja jest zapętlona?</param>
        /// <param name="scale">Skala.</param>
        public void Initialize(Vector2 position, int frameWidth,
                                        int frameHeight, int frameCount, int frameTime,
                                        Color color, bool looping = true, float scale = 1)
        {
            _backColor = color;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _scale = scale;

            _looping = looping;
            _position = position;

            _elapsedTime = 0;
            _currentFrame = 0;

            _active = true;
        }

        /// <summary>
        /// Funkcja uaktualniająca animację grafiki.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            if (_active == false)
                return;

            _elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_elapsedTime > _frameTime)
            {
                _currentFrame++;

                if (_currentFrame == _frameCount)
                {
                    _currentFrame = 0;

                    if (_looping == false)
                        
                    _active= false;
                }

                _elapsedTime = 0;
            }

            _sourceRectangle = new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);
            
            _destRectangle = new Rectangle((int)_position.X, (int)_position.Y,
                                          (int)(_frameWidth * _scale), (int)(_frameHeight * _scale));
        }

        /// <summary>
        /// Rysowanie grafiki.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_active)
            {
                spriteBatch.Draw(_image, _destRectangle, _sourceRectangle, _backColor);
                //spriteBatch.Draw(_image, _destRectangle, _sourceRectangle, _backColor, (float)Math.PI/2, new Vector2(_image.Width, _image.Height), SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Metoda umożliwiająca przesunięcie wyświetlanej grafiki do innej ramki.
        /// </summary>
        /// <param name="frameNumber">Numer ramki docelowej.</param>
        public void MoveToFrame(int frameNumber)
        {
            _currentFrame = frameNumber;
            _sourceRectangle = new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);
        }

        /// <summary>
        /// Metoda klonująca grafikę.
        /// </summary>
        /// <returns>Nowy obiekt animowanej grafiki.</returns>
        public AnimatedGraphic Clone()
        {
            AnimatedGraphic temp = new AnimatedGraphic();

            temp._image = _image;
            temp._position = new Vector2(_position.X, _position.Y);
            temp._scale = _scale;
            temp._isVisible = _isVisible;
            temp._backColor = _backColor;
            temp._destRectangle = new Rectangle(_destRectangle.X, _destRectangle.Y, _destRectangle.Width, _destRectangle.Height);

            temp._frameWidth = _frameWidth;
            temp._frameHeight = _frameHeight;
            temp._frameCount = _frameCount;
            temp._frameTime = _frameTime;
            temp._scale = _scale;

            temp._looping = _looping;
            temp._position = _position;

            temp._elapsedTime = _elapsedTime;
            temp._currentFrame = _currentFrame;

            temp._active = _active;

            return temp;
        }
    }
}