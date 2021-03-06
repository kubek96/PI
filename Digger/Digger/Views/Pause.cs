﻿using Digger.Graphic;
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
    /// Klasa widoku Pausy urachamianego widocznego tylko w trybie rozgrywki.
    /// </summary>
    public class Pause : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _continue;
        private Button[] _menu;

        private bool _close;
        private bool _showOptions;
        private bool _showHelp;

        private int _elapsedGoBackTime;

        #region Properties

        public bool Close
        {
            get { return _close; }
            set { _close = value; }
        }

        public bool ShowOptions
        {
            get { return _showOptions; }
            set { _showOptions = value; }
        }

        public bool ShowHelp
        {
            get { return _showHelp; }
            set { _showHelp = value; }
        }

        public int ElapsedGoBackTime
        {
            get { return _elapsedGoBackTime; }
            set { _elapsedGoBackTime = value; }
        }

        #endregion

        /// <summary>
        /// Kontruktor widoku pausy.
        /// Widok ustawiony jest jako niewidoczny.
        /// </summary>
        public Pause()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Pause");

            // Część główna okna
            _menu = new Button[3];
            _menu[0] = new Button(typeof(Menu));
            _menu[1] = new Button(null);
            _menu[2] = new Button(null);

            // Nawigacja
            _continue = new Button(null);
            _exit = new Button(typeof(Exit));

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();

            // Czas 
            _elapsedGoBackTime = 0;
        }

        /// <summary>
        /// Metoda wczytuje grafiki i czcionkni dla wszystkich obiektów znajdujących się w Pause.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/TransparentBackground");
            _logo.LoadContent(content, "Views/Common/Logo");

            _continue.LoadContent(content, "Views/NavigationButtons/Continue");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

            _menu[0].LoadContent(content, "Views/NavigationButtons/MainMenu");
            _menu[1].LoadContent(content, "Views/NavigationButtons/Options");
            _menu[2].LoadContent(content, "Views/NavigationButtons/Help");

            _header.LoadContent(content, "Fonts/Silkscreen");
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
            _continue.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].ButtonGraphic.Initialize(new Vector2((screenWidth - 340)/2, 420+i*50), 340, 25,1,100,Color.White);
            }

            _header.Initialize(new Vector2(screenWidth / 2, 150));
            _logo.Initialize(new Vector2((screenWidth - _logo.Image.Width) / 2, 20), Color.White);
        }

        /// <summary>
        /// Metoda wywołująca rysowanie obiektów interfejsu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].Draw(spriteBatch, gameTime);
            }

            _logo.Draw(spriteBatch);

            _continue.Draw(spriteBatch, gameTime);
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
            _continue.Update(gameTime);
            _exit.Update(gameTime);

            if (_continue.ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _elapsedGoBackTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                // Jeżeli kliknięty to szalej
                if (_elapsedGoBackTime > 120) { 
                    _elapsedGoBackTime = 0;
                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        _close = true;
                    }
                }
                // Zmień wyglad buttona
                _continue.ButtonState = ButtonState.MouseOn;
            }
            else
            {
                _continue.ButtonState = ButtonState.MouseLeave;
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

            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].Update(gameTime);

                if (_menu[i].ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        switch (i)
                        {
                            case 1:
                                _showOptions = true;
                                break;
                            case 2:
                                _showHelp = true;
                                break;
                            default:
                                _menu[i].ButtonState = ButtonState.Click;
                                break;
                        }

                        continue;
                    }

                    // Zmień wyglad buttona
                    _menu[i].ButtonState = ButtonState.MouseOn;
                    continue;
                }

                _menu[i].ButtonState = ButtonState.MouseLeave;
            }
        }
    }
}