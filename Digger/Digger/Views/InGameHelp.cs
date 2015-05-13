using Digger.Graphic;
using Digger.Views.Common;
using Digger.Views.Common.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MouseState = Digger.Views.Common.Control.MouseState;

namespace Digger.Views
{
    public class InGameHelp : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _goBack;
        private FixedGraphic _instruction;

        private bool _close;

        #region Properties

        public bool Close
        {
            get { return _close; }
            set { _close = value; }
        }

        #endregion

        public InGameHelp()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Help");

            // Część główna okna
            _instruction = new FixedGraphic();

            // Nawigacja
            _goBack = new Button(null);
            _exit = new Button(typeof(Exit));

            // Wczytaj zawartość
            LoadContent(Game1.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/TransparentBackground");
            _logo.LoadContent(content, "Views/Common/Logo");

            _instruction.LoadContent(content, "Views/Help/Instruction");

            _goBack.LoadContent(content, "Views/NavigationButtons/GoBack");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

            _header.LoadContent(content, "Fonts/Silkscreen");
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _exit.ButtonGraphic.Initialize(new Vector2(screenWidth - 360, 50), 340, 25, 1, 100, Color.White);
            _goBack.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            _instruction.Initialize(new Vector2((screenWidth - _instruction.Image.Width)/2, 240), Color.White);

            _header.Initialize(new Vector2(screenWidth / 2, 150));
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width) / 2, 20), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            _logo.Draw(spriteBatch);

            _instruction.Draw(spriteBatch);

            _goBack.Draw(spriteBatch, gameTime);
            _exit.Draw(spriteBatch, gameTime);

            _header.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _goBack.Update(gameTime);
            _exit.Update(gameTime);

            if (_goBack.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                // Jeżeli kliknięty to szalej
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    _close = true;
                }
                else
                {
                    // Zmień wyglad buttona
                    _goBack.MouseState = MouseState.MouseOn;
                }
            }
            else
            {
                _goBack.MouseState = MouseState.MouseLeave;
            }

            if (_exit.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                // Jeżeli kliknięty to szalej
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    _exit.MouseState = MouseState.Click;
                }
                else
                {
                    // Zmień wyglad buttona
                    _exit.MouseState = MouseState.MouseOn;
                }
            }
            else
            {
                _exit.MouseState = MouseState.MouseLeave;
            }
        }
    }
}