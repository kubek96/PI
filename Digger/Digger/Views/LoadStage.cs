using System.Linq;
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
    public class LoadStage : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _previousPage;
        private Button[] _players;
        private Label _noPlayers;

        public LoadStage()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Choose player name to play:");

            // Część główna okna
            if (Game1.Context.Players.Count == 0)
            {
                _noPlayers = new Label("There are no players yet.");
            }
            else
            {
                _players = new Button[Game1.Context.Players.Count];
                for (int i = 0; i < Game1.Context.Players.Count; i++)
                {
                    _players[i] = new Button(typeof(NewGame), Game1.Context.Players[i].Level, Game1.Context.Players[i].Name);
                }
            }

            // Nawigacja
            _previousPage = new Button(typeof(Menu));
            _exit = new Button(typeof(Exit));

            // Wczytaj zawartość
            LoadContent(Game1.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
            _logo.LoadContent(content, "Views/Common/Logo");

            _previousPage.LoadContent(content, "Views/NavigationButtons/GoBack");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

            _header.LoadContent(content, "Fonts/Silkscreen");

            if (Game1.Context.Players.Count == 0)
            {
                _noPlayers.LoadContent(content, "Fonts/Silkscreen");
            }
            else
            {
                for (int i = 0; i < _players.Length; i++)
                {
                    _players[i].LoadContent(content, "Fonts/Silkscreen");
                }
            }
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _exit.ButtonGraphic.Initialize(new Vector2(screenWidth - 360, 50), 340, 25, 1, 100, Color.White);
            _previousPage.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            if (Game1.Context.Players.Count == 0)
            {
                _noPlayers.Initialize(new Vector2(screenWidth / 2, screenHeight / 2));
            }
            else
            {
                for (int i = 0; i < Game1.Context.Players.Count; i++)
                {
                    _players[i].Label.Initialize(new Vector2(screenWidth / 2,230+40*i));
                }
            }

            _header.Initialize(new Vector2(screenWidth / 2, 150));
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width) / 2, 20), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            if (Game1.Context.Players.Count == 0)
            {
                _noPlayers.Draw(spriteBatch);
            }
            else
            {
                for (int i = 0; i < Game1.Context.Players.Count; i++)
                {
                    _players[i].Draw(spriteBatch, gameTime);
                }
            }

            _logo.Draw(spriteBatch);

            _previousPage.Draw(spriteBatch, gameTime);
            _exit.Draw(spriteBatch, gameTime);

            _header.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _previousPage.Update(gameTime);
            _exit.Update(gameTime);

            if (_previousPage.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                // Jeżeli kliknięty to szalej
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    _previousPage.MouseState = MouseState.Click;
                }
                else
                {
                    // Zmień wyglad buttona
                    _previousPage.MouseState = MouseState.MouseOn;
                }
            }
            else
            {
                _previousPage.MouseState = MouseState.MouseLeave;
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

            for (int i = 0; i < _players.Length; i++)
            {
                _players[i].Update(gameTime);

                if (_players[i].Label.Rectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        _players[i].MouseState = MouseState.Click;
                        continue;
                    }

                    // Zmień wyglad buttona
                    _players[i].MouseState = MouseState.MouseOn;
                    continue;
                }

                _players[i].MouseState = MouseState.MouseLeave;
            }
        }
    }
}