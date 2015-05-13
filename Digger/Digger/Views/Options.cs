using Digger.Data.Control;
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
    public class Options : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _goBack;

        private Label _keyboardLabel;
        private Button[] _keyboardLayout;

        private Label _musicLabel;
        private Button[] _musicOnOff;

        private bool _close;

        public bool Close
        {
            get { return _close; }
            set { _close = value; }
        }

        public Options()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Options");

            // Część główna okna
            _keyboardLabel = new Label("Control type:");
            _keyboardLayout = new Button[2];
            _keyboardLayout[0] = new Button(null);
            _keyboardLayout[1] = new Button(null);

            _musicLabel = new Label("Music is:");
            _musicOnOff = new Button[2];
            _musicOnOff[0] = new Button(null);
            _musicOnOff[1] = new Button(null);

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

            _goBack.LoadContent(content, "Views/NavigationButtons/GoBack");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

            _keyboardLabel.LoadContent(content, "Fonts/Silkscreen");
            _keyboardLayout[0].LoadContent(content, "Views/OptionsButtons/Arrows");
            _keyboardLayout[1].LoadContent(content, "Views/OptionsButtons/Wsad");

            _musicLabel.LoadContent(content, "Fonts/Silkscreen");
            _musicOnOff[0].LoadContent(content, "Views/OptionsButtons/On");
            _musicOnOff[1].LoadContent(content, "Views/OptionsButtons/Off");

            _header.LoadContent(content, "Fonts/Silkscreen");
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _exit.ButtonGraphic.Initialize(new Vector2(screenWidth - 360, 50), 340, 25, 1, 100, Color.White);
            _goBack.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            _keyboardLabel.Initialize(new Vector2(screenWidth/2, 320));
            _keyboardLayout[0].ButtonGraphic.Initialize(new Vector2((screenWidth - 460) / 2, 360), 230, 25, 1, 100, Color.White);
            _keyboardLayout[1].ButtonGraphic.Initialize(new Vector2((screenWidth) / 2, 360), 230, 25, 1, 100, Color.White);

            _musicLabel.Initialize(new Vector2(screenWidth / 2, 520));
            _musicOnOff[0].ButtonGraphic.Initialize(new Vector2((screenWidth - 300) / 2, 560), 150, 25, 1, 100, Color.White);
            _musicOnOff[1].ButtonGraphic.Initialize(new Vector2((screenWidth) / 2, 560), 150, 25, 1, 100, Color.White);

            _header.Initialize(new Vector2(screenWidth / 2, 150));
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width) / 2, 20), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            _keyboardLabel.Draw(spriteBatch);
            for (int i = 0; i < _keyboardLayout.Length; i++)
            {
                _keyboardLayout[i].Draw(spriteBatch,gameTime);
            }

            _musicLabel.Draw(spriteBatch);
            for (int i = 0; i < _musicOnOff.Length; i++)
            {
                _musicOnOff[i].Draw(spriteBatch,gameTime);
            }

            _logo.Draw(spriteBatch);

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

            for (int i = 0; i < _musicOnOff.Length; i++)
            {
                _musicOnOff[i].Update(gameTime);

                if (_musicOnOff[i].ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        switch (i)
                        {
                            case 0:
                                // Uruchom muzykę
                                Game1.Context.Player.IsMusicOn = true;
                                break;
                            case 1:
                                // Wyłącz muzykę
                                Game1.Context.Player.IsMusicOn = false;
                                break;
                        }
                        continue;
                    }

                    // Zmień wyglad buttona
                    _musicOnOff[i].MouseState = MouseState.MouseOn;
                    continue;
                }

                _musicOnOff[i].MouseState = MouseState.MouseLeave;
            }

            for (int i = 0; i < _keyboardLayout.Length; i++)
            {
                _keyboardLayout[i].Update(gameTime);

                if (_keyboardLayout[i].ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        switch (i)
                        {
                            case 0:
                                // Przełącz na arrows
                                Game1.Context.Player.UserKeyboraPreferences = KeyboradLayout.Arrows;
                                break;
                            case 1:
                                // Przełącz na wsad
                                Game1.Context.Player.UserKeyboraPreferences = KeyboradLayout.WSAD;
                                break;
                        }
                        continue;
                    }

                    // Zmień wyglad buttona
                    _keyboardLayout[i].MouseState = MouseState.MouseOn;
                    continue;
                }

                _keyboardLayout[i].MouseState = MouseState.MouseLeave;
            }

            // Sprawdź, czy użytkownik ma uruchomioną muzykę
            if (Game1.Context.Player.IsMusicOn)
            {
                _musicOnOff[0].ButtonGraphic.MoveToFrame(2);
            }
            else
            {
                _musicOnOff[1].ButtonGraphic.MoveToFrame(2);
            }

            // Sprawdź, który tryb sterowania ma wybrany gracz
            if (Game1.Context.Player.UserKeyboraPreferences == KeyboradLayout.Arrows)
            {
                _keyboardLayout[0].ButtonGraphic.MoveToFrame(2);
            }
            else
            {
                _keyboardLayout[1].ButtonGraphic.MoveToFrame(2);
            }
        }
    }
}