using System;
using System.Collections.Generic;
using System.Linq;
using Digger.Data;
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
    /// <summary>
    /// Widok najlepszych wyników.
    /// </summary>
    public class BestScores : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _previousPage;
        private Label[,] _players;

        /// <summary>
        /// Konstruktor.
        /// Wczytuje listę 16 najlepszych graczy oraz sortuje ich od najlepszego wyniku w dół.
        /// </summary>
        public BestScores()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Best scores:");

            // Część główna okna
            if (Window.Context.Players.Count == 0)
            {
                _players = new Label[1, 1];
                _players[0,0] = new Label("There are no players yet.");
            }
            else
            {
                List<Player> p = Window.Context.Players.ToList();
                p.Sort((p1, p2) => p2.Points.CompareTo(p1.Points));

                int howMany = p.Count < 16 ? p.Count : 16;
                _players = new Label[howMany, 2];
                for (int i = 0; i < howMany; i++)
                {
                    _players[i, 0] = new Label(p[i].Name);
                    _players[i, 1] = new Label(p[i].Points.ToString());
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

        /// <summary>
        /// Metoda wczytuje grafiki i czcionkni dla wszystkich obiektów znajdujących się w BestScores.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
            _logo.LoadContent(content, "Views/Common/Logo");

            _previousPage.LoadContent(content, "Views/NavigationButtons/GoBack");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

            _header.LoadContent(content, "Fonts/Silkscreen");

            for (int i = 0; i < _players.GetLength(0); i++)
            {
                for (int j = 0; j < _players.GetLength(1); j++)
                {
                    _players[i, j].LoadContent(content, "Fonts/Silkscreen");
                }
            }
        }

        /// <summary>
        /// Metoda inicjalizująca obiekty interfejsu użytkownika.
        /// </summary>
        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _exit.ButtonGraphic.Initialize(new Vector2(screenWidth - 360, 50), 340, 25, 1, 100, Color.White);
            _previousPage.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            if (Window.Context.Players.Count == 0)
            {
                _players[0, 0].Initialize(new Vector2(screenWidth / 2, screenHeight / 2));
            }
            else
            {
                for (int i = 0; i < _players.GetLength(0); i++)
                {
                    _players[i, 0].Initialize(new Vector2((screenWidth/2) - 150, 230 + i * 40));
                    _players[i, 1].Initialize(new Vector2((screenWidth / 2) +160, 230 + i * 40));
                    if (i%2 != 0)
                    {
                        _players[i, 0].Color = _players[i, 1].Color = Color.White;
                    }
                    else
                    {
                        _players[i, 0].Color = _players[i, 1].Color = Color.Yellow;
                    }
                }
            }

            _header.Initialize(new Vector2(screenWidth / 2, 150));
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width)/ 2, 20), Color.White);
        }

        /// <summary>
        /// Metoda wywołująca rysowanie obiektów interfejsu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            for (int i = 0; i < _players.GetLength(0); i++)
            {
                for (int j = 0; j < _players.GetLength(1); j++)
                {
                    _players[i, j].Draw(spriteBatch);
                }
            }

            _logo.Draw(spriteBatch);

            _previousPage.Draw(spriteBatch, gameTime);
            _exit.Draw(spriteBatch, gameTime);

            _header.Draw(spriteBatch);
        }

        /// <summary>
        /// Metoda uaktualniająca obiekty interfejsu.
        /// Odpowiada za obsługę zdarzeń.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
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
        }
    }
}