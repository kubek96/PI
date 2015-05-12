using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Digger.Views.Common.Control
{
    public class Input
    {
        private string _string;
        private SpriteFont _font;
        private Vector2 _position;
        private AnimatedGraphic _iBeam;
        private Keys[] _lastPressedKeys;

        public string Text
        {
            get { return _string; }
            set { _string = value; }
        }

        public Input(string label ="")
        {
            _string = label;

            // Initialize ibeam
            _iBeam = new AnimatedGraphic();

            // Zainicjalizuj tablcę ostatio wciśniętych klawiszy
            _lastPressedKeys = new Keys[0];
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _font = content.Load<SpriteFont>(assetName);

            // Load graphic
            _iBeam.LoadContent(content, "Views/Common/IBeam");
        }

        public void Initialize(Vector2 position)
        {
            _position = position;

            // Ibeam
            _iBeam.Initialize(new Vector2(_position.X, _position.Y-(_iBeam.Image.Height/2) - 2), 5, _iBeam.Image.Height, 2, 500, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Find the center of the string
            Vector2 fontOrigin = _font.MeasureString(_string) / 2;
            // Draw the string
            spriteBatch.DrawString(_font, _string, _position, Color.LightGreen, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            _iBeam.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _iBeam.Update(gameTime);

            // Get keyborad state:
            // http://stackoverflow.com/questions/10154046/making-text-input-in-xna-for-entering-names-chatting
            KeyboardState newKeyboardState = Keyboard.GetState();
            Keys[] keys = newKeyboardState.GetPressedKeys();

            // Ostatnio wciśnięte klawisze
            for (int i = 0; i < _lastPressedKeys.Length; i++)
            {
                if (!keys.Contains(_lastPressedKeys[i]))
                {
                    // Klawisz w górze
                    if (_lastPressedKeys[i] >= Keys.A && _lastPressedKeys[i] <= Keys.Z)
                    {
                        if (_string.Length < 10)
                        {
                            _string += _lastPressedKeys[i].ToString();
                            // Przesuń ibeam
                            //_iBeam.Position = _position + _font.MeasureString(_string) / 2;
                            _iBeam.Position = new Vector2(_position.X + (_font.MeasureString(_string) / 2).X, _iBeam.Position.Y);
                            continue;
                        }
                    }

                    // Czy jest cofanie
                    // http://stackoverflow.com/questions/9456089/how-to-determine-if-a-key-is-a-letter-or-number
                    if (_lastPressedKeys[i] == Keys.Back)
                    {
                        if (_string.Length > 0)
                        {
                            _string = _string.Remove(_string.Length - 1);
                            _iBeam.Position = new Vector2(_position.X + (_font.MeasureString(_string) / 2).X, _iBeam.Position.Y);
                        }
                    }
                }    
            }

            _lastPressedKeys = keys.ToArray();
        } 
    }


}