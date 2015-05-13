using Digger.Graphic;
using Digger.Views.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Views
{
    /// <summary>
    /// Klasa, której obiekt wywoływany jest bezpośrednio przed samym zamknięciem gry.
    /// </summary>
    public class Exit : IXnaUseable
    {
        private FixedGraphic _background;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public Exit()
        {
            // Tło
            _background = new FixedGraphic();

            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();

            Logger.Report("rozpoczęto procedurę wyłączania gry.");
        }

        /// <summary>
        /// Metoda wczytuje tło w Exit.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
        }

        /// <summary>
        /// Metoda inicjalizująca obiekty interfejsu użytkownika.
        /// </summary>
        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, screenHeight, screenWidth, screenHeight), Color.White);
        }

        /// <summary>
        /// Metoda wywołująca rysowanie obiektów interfejsu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);
        }

        /// <summary>
        /// Metoda uaktualniająca obiekty interfejsu.
        /// Odpowiada za obsługę zdarzeń.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            Window.Context.ReadyToExit = true;
        }
    }
}