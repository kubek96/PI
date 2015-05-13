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
    /// Klasa pomocy dostępna z poziomu menu.
    /// </summary>
    public class Help : IXnaUseable
    {
        // Interferj użytkownika
        private FixedGraphic _background;
        private FixedGraphic _logo;
        private Label _header;
        private Button _exit;
        private Button _previousPage;

        // Elementy okna
        private FixedGraphic _instruction;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public Help()
        {
            // Tło
            _background = new FixedGraphic();

            // Nagłówek
            _logo = new FixedGraphic();
            _header = new Label("Help");

            // Częśc główna okna
            _instruction = new FixedGraphic();

            // Nawigacja
            _previousPage = new Button(typeof(Menu));
            _exit = new Button(typeof(Exit));

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        /// <summary>
        /// Metoda wczytuje grafiki i czcionkni dla wszystkich obiektów znajdujących się w Help.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
            _logo.LoadContent(content, "Views/Common/Logo");

            _instruction.LoadContent(content, "Views/Help/Instruction");

            _previousPage.LoadContent(content, "Views/NavigationButtons/GoBack");
            _exit.LoadContent(content, "Views/NavigationButtons/Exit");

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
            _previousPage.ButtonGraphic.Initialize(new Vector2(0, 50), 340, 25, 1, 100, Color.White);

            _instruction.Initialize(new Vector2((screenWidth - _instruction.Image.Width) / 2, 240), Color.White);

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

            _logo.Draw(spriteBatch);

            _instruction.Draw(spriteBatch);

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