using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Graphic
{
    public class ParallaxingGraphic : FixedGraphic
    {
        private Vector2[] _positions;
        private int _speed;

        public void Initialize(Vector2 position, Color color, int screenWidth, int speed, float scale = 1)
        {
            base.Initialize(position, color, scale);

            _positions = new Vector2[screenWidth];
            
            // Begining position
            for (int i = 0; i < _positions.Length; i++)
            {
                // Image one behind another
                _positions[i] = new Vector2(i * _image.Width, _position.Y);
            } 
            _speed = speed;
        }

        public new void Update()
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                _positions[i].X += _speed;

                if (_positions[i].X <= -_image.Width)
                {
                    _positions[i].X = _image.Width * (_positions.Length - 1);
                }
                else
                {
                    if (_positions[i].X >= _image.Width * (_positions.Length - 1))
                    {
                        _positions[i].X = -_image.Width;
                    }
                }
            }
        } 

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                spriteBatch.Draw(_image, _positions[i], Color.White);
            } 
        }
    }
}