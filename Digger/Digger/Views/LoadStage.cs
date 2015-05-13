using System.Linq;
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
    public class LoadStage : IXnaUseable
    {
        private static int _maxLevel = 6;

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
            if (Window.Context.Players.Count == 0)
            {
                _noPlayers = new Label("There are no players yet.");
            }
            else
            {
                _players = new Button[Window.Context.Players.Count];
                for (int i = 0; i < Window.Context.Players.Count; i++)
                {
                    if (Window.Context.Players[i].Level > _maxLevel) Window.Context.Players[i].Level = _maxLevel;
                    _players[i] = new Button(typeof(Stage), Window.Context.Players[i].Level, Window.Context.Players[i].Name);
                }
            }

            // Nawigacja
            _previousPage = new Button(typeof(Menu));
            _exit = new Button(typeof(Exit));

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

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

            if (Window.Context.Players.Count == 0)
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

            if (Window.Context.Players.Count == 0)
            {
                _noPlayers.Initialize(new Vector2(screenWidth / 2, screenHeight / 2));
            }
            else
            {
                for (int i = 0; i < Window.Context.Players.Count; i++)
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

            if (Window.Context.Players.Count == 0)
            {
                _noPlayers.Draw(spriteBatch);
            }
            else
            {
                for (int i = 0; i < Window.Context.Players.Count; i++)
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
                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    _previousPage.ButtonState = ButtonState.Click;
                }
                else
                {
                    // Zmień wyglad buttona
                    _previousPage.ButtonState = ButtonState.MouseOn;
                }
            }
            else
            {
                _previousPage.ButtonState = ButtonState.MouseLeave;
            }

            if (_exit.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                // Jeżeli kliknięty to szalej
                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    _exit.ButtonState = ButtonState.Click;
                }
                else
                {
                    // Zmień wyglad buttona
                    _exit.ButtonState = ButtonState.MouseOn;
                }
            }
            else
            {
                _exit.ButtonState = ButtonState.MouseLeave;
            }

            for (int i = 0; i < _players.Length; i++)
            {
                _players[i].Update(gameTime);

                if (_players[i].Label.Rectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        Window.Context.LoadPlayerGame(i);
                        _players[i].ButtonState = ButtonState.Click;
                        continue;
                    }

                    // Zmień wyglad buttona
                    _players[i].ButtonState = ButtonState.MouseOn;
                    continue;
                }

                _players[i].ButtonState = ButtonState.MouseLeave;
            }
        }
    }
}