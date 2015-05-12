﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Views.Common.Control
{
    public class Label
    {
        private string _string;
        private SpriteFont _font;
        private Vector2 _position;
        private Color _color;
        private Rectangle _rectangle;

        public Rectangle Rectangle
        {
            get { return _rectangle; }
        }

        public Label(string label = "")
        {
            _string = label;
            _color = Color.LightGreen;
        }

        public string Text
        {
            get { return _string; }
            set { _string = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _font = content.Load<SpriteFont>(assetName);
        }

        public void Initialize(Vector2 position)
        {
            _position = position;
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)_font.MeasureString(_string).X, (int)_font.MeasureString(_string).Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Find the center of the string
            Vector2 fontOrigin = _font.MeasureString(_string)/2;
            _rectangle = new Rectangle((int)(_position.X - fontOrigin.X), (int)(_position.Y-fontOrigin.Y), (int)_font.MeasureString(_string).X, (int)_font.MeasureString(_string).Y);
            // Draw the string
            spriteBatch.DrawString(_font, _string, _position, _color, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}