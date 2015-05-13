using Digger.Graphic;
using Digger.Views.Common;
using Digger.Views.Common.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Digger.Views.Common.Control.ButtonState;

namespace Digger.Views
{
    public class Win : IXnaUseable
    {
        private static int _maxLevel = 7;

        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _infoLabel;
        private Button _goToNextLevel;
        private Button _goToMainMenuButton;

        private int _nextLevel;
        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public Win(int nextLevel)
        {
            _nextLevel = nextLevel;
            _isVisible = false;

            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();

            // Centurm
            if (_nextLevel != 7)
            {
                _infoLabel = new Label("Congratulations!");
                _goToNextLevel = new Button(typeof (Stage), nextLevel);
            }
            else
            {
                _infoLabel = new Label("You finish the game!");
            }
            _goToMainMenuButton = new Button(typeof(Menu));

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/TransparentBackground");
            _logo.LoadContent(content, "Views/Common/Logo");

            _goToMainMenuButton.LoadContent(content, "Views/NavigationButtons/MainMenu");

            if (_nextLevel != 7)
            {
                _goToNextLevel.LoadContent(content, "Views/NavigationButtons/NextLevel");
            }
            
            _infoLabel.LoadContent(content, "Fonts/Silkscreen");
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width) / 2, 20), Color.White);
            _goToMainMenuButton.ButtonGraphic.Initialize(new Vector2((screenWidth - 340) / 2, (screenHeight / 2) + 100), 340, 25, 1, 100, Color.White);
            _infoLabel.Initialize(new Vector2(screenWidth/2, 300));
            if (_nextLevel != 7)
            {
                _goToNextLevel.ButtonGraphic.Initialize(new Vector2((screenWidth - 350) / 2, 420), 370, 72, 1, 100, Color.White);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            if (_nextLevel != 7)
            {
                _goToNextLevel.Draw(spriteBatch, gameTime);
            }

            _logo.Draw(spriteBatch);

            _goToMainMenuButton.Draw(spriteBatch, gameTime);

            _infoLabel.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _goToMainMenuButton.Update(gameTime);
            if (_nextLevel != 7)
            {
                _goToNextLevel.Update(gameTime);

                if (_goToNextLevel.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        _goToNextLevel.ButtonState = ButtonState.Click;
                    }
                    else
                    {
                        // Zmień wyglad buttona
                        _goToNextLevel.ButtonState = ButtonState.MouseOn;
                    }
                }
                else
                {
                    _goToNextLevel.ButtonState = ButtonState.MouseLeave;
                }
            }

            if (_goToMainMenuButton.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                // Jeżeli kliknięty to szalej
                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    _goToMainMenuButton.ButtonState = ButtonState.Click;
                }
                else
                {
                    // Zmień wyglad buttona
                    _goToMainMenuButton.ButtonState = ButtonState.MouseOn;
                }
            }
            else
            {
                _goToMainMenuButton.ButtonState = ButtonState.MouseLeave;
            }
        }
    }
}