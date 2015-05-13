using Digger.Controller;
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
    /// Klasa widoku interfejsu tworzenia nowej gry.
    /// </summary>
    public class NewGame : IXnaUseable
    {
        private FixedGraphic _background;
        private Button[] _menu;
        private FixedGraphic _menuFrame;
        private Input _userName;

        /// <summary>
        /// Konstrukotr widoku okna nowej gry.
        /// </summary>
        public NewGame()
        {
            // Tło
            _background = new FixedGraphic();
            _menuFrame = new FixedGraphic();

            // Menu
            _menu = new Button[3];
            _menu[0] = new Button(typeof(Stage));
            _menu[1] = new Button(typeof(Menu));
            _menu[2] = new Button(typeof(Exit));

            // UserName input
            _userName = new Input();

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        /// <summary>
        /// Metoda wczytuje grafiki i czcionkni dla wszystkich obiektów znajdujących się w NewGame.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
            _menuFrame.LoadContent(content, "Views/Common/MenuFrame");

            _menu[0].LoadContent(content, "Views/NavigationButtons/Play");
            _menu[1].LoadContent(content, "Views/NavigationButtons/GoBack");
            _menu[2].LoadContent(content, "Views/NavigationButtons/Exit");

            // UserInput
            _userName.LoadContent(content, "Fonts/Silkscreen");
        }

        /// <summary>
        /// Metoda inicjalizująca obiekty interfejsu użytkownika.
        /// </summary>
        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            _menuFrame.Initialize(new Vector2((screenWidth - _menuFrame.Image.Width) / 2, (screenHeight - _menuFrame.Image.Height) / 2), Color.White);

            _menu[0].ButtonGraphic.Initialize(new Vector2((screenWidth / 2) + 80, (screenHeight / 2) + 150), _menu[0].ButtonGraphic.Image.Width / 3, _menu[0].ButtonGraphic.Image.Height, 1, 100, Color.White);
            _menu[1].ButtonGraphic.Initialize(new Vector2((screenWidth / 2) - 300, (screenHeight / 2) + 150), 340, 25, 1, 100, Color.White);
            _menu[2].ButtonGraphic.Initialize(new Vector2(screenWidth - 360, 20), 340, 25, 1, 100, Color.White);

            // UserInput
            _userName.Initialize(new Vector2(screenWidth / 2, screenHeight / 2));
        }

        /// <summary>
        /// Metoda wywołująca rysowanie obiektów interfejsu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);
            _menuFrame.Draw(spriteBatch);

            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].Draw(spriteBatch, gameTime);
            }

            _userName.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Metoda uaktualniająca obiekty interfejsu.
        /// Odpowiada za obsługę zdarzeń.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            // Menu
            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].Update(gameTime);
            }

            // Menu & Mouse
            for (int i = 0; i < _menu.Length; i++)
            {
                if (_menu[i].ButtonGraphic.DestRectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    // Jeżeli kliknięty to szalej
                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        // Dokonaj szybkiej walidacji
                        if (i == 0)
                        {
                            // Czy wprowadzono imię gracza?
                            if (_userName.Text.Trim() == "") continue;
                            // Utwórz nowego gracza
                            Window.Context.CreateNewPlayer(_userName.Text);
                            Navigator.NavigateTo(typeof(Stage), 0);
                            continue;
                        }

                        _menu[i].ButtonState = ButtonState.Click;
                        continue;
                    }

                    // Zmień wyglad buttona
                    _menu[i].ButtonState = ButtonState.MouseOn;
                    continue;
                }

                _menu[i].ButtonState = ButtonState.MouseLeave;
            }

            // Input
            _userName.Update(gameTime);
        }
    }
}