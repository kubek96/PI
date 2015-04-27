using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Views.Common.Control
{
    public class Label
    {
        private string _string;
        private SpriteFont _font;
        private Vector2 _position;

        public Label(string label = "")
        {
            _string = label;
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _font = content.Load<SpriteFont>(assetName);
        }

        public void Initialize(Vector2 position)
        {
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Find the center of the string
            Vector2 fontOrigin = _font.MeasureString(_string)/2;
            // Draw the string
            spriteBatch.DrawString(_font, _string, _position, Color.LightGreen, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}