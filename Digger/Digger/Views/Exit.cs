using Digger.Graphic;
using Digger.Views.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Views
{
    public class Exit : IXnaUseable
    {
        private FixedGraphic _background;

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

        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, screenHeight, screenWidth, screenHeight), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            Window.Context.ReadyToExit = true;
        }
    }
}