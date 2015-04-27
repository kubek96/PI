using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Graphic
{
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

        public AnimatedGraphic()
        {
            _destRectangle = new Rectangle();
            _sourceRectangle = new Rectangle();
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int FrameWidth
        {
            get { return _frameWidth; }
        }

        public int FrameHeight
        {
            get { return _frameHeight; }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_active)
            {
                spriteBatch.Draw(_image, _destRectangle, _sourceRectangle, _backColor);
                //spriteBatch.Draw(_image, _destRectangle, _sourceRectangle, _backColor, (float)Math.PI/2, new Vector2(_image.Width, _image.Height), SpriteEffects.None, 0);
            }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public int FrameTime
        {
            get { return _frameTime; }
        }

        public void MoveToFrame(int frameNumber)
        {
            _currentFrame = frameNumber;
            _sourceRectangle = new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);
        }

        public AnimatedGraphic Clone()
        {
            // TODO: Clone
            return null;
        }
    }
}