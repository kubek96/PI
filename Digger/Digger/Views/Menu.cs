using System;
using Digger.Game.Common;
using Digger.Game.Elements;
using Digger.Graphic;
using Digger.Views.Common;
using Digger.Views.Common.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ButtonState = Digger.Views.Common.Control.ButtonState;

namespace Digger.Views
{
    /// <summary>
    /// Klasa menu głównego gry.
    /// </summary>
    public class Menu : IXnaUseable
    {
        private FixedGraphic _background;
        private ParallaxingGraphic[] _grass;
        private Button[] _menu;
        private FixedGraphic _sky;
        private ParallaxingGraphic _cloud;
        private FixedGraphic _groundBuffer;
        private FixedGraphic _menuFrame;

        /// <summary>
        /// Konstruktor menu głównego.
        /// </summary>
        public Menu()
        {
            // Tło
            _background = new FixedGraphic();
            // Animowane tło
            _grass = new ParallaxingGraphic[3];
            for (int i = 0; i < _grass.Length; i++)
            {
                _grass[i] = new ParallaxingGraphic();
            }
            _sky = new FixedGraphic();
            _cloud = new ParallaxingGraphic();
            _groundBuffer = new FixedGraphic();

            // Menu
            _menuFrame = new FixedGraphic();
            _menu = new Button[5];
            _menu[0] = new Button(typeof(NewGame));
            _menu[1] = new Button(typeof(LoadStage));
            _menu[2] = new Button(typeof(BestScores));
            _menu[3] = new Button(typeof(Help));
            _menu[4] = new Button(typeof(Exit));
            
            // Wczytaj zawartość
            LoadContent(Window.Context.Content);

            // Zainicjalizuj
            Initialize();

            MediaPlayer.Stop();
        }

        /// <summary>
        /// Metoda wczytuje grafiki i czcionkni dla wszystkich obiektów znajdujących się w Menu.
        /// </summary>
        /// <param name="content">Manager zasobów.</param>
        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");

            _grass[0].LoadContent(content, "Views/Menu/GrassOne");
            _grass[1].LoadContent(content, "Views/Menu/GrassTwo");
            _grass[2].LoadContent(content, "Views/Menu/GrassThree");
            _sky.LoadContent(content, "Views/Menu/Sky");
            _cloud.LoadContent(content, "Views/Menu/Clouds");
            _groundBuffer.LoadContent(content, "Views/Menu/GroundBuffer");

            _menuFrame.LoadContent(content, "Views/Common/MenuFrame");
            _menu[0].LoadContent(content, "Views/NavigationButtons/NewGame");
            _menu[1].LoadContent(content, "Views/NavigationButtons/LoadGame");
            _menu[2].LoadContent(content, "Views/NavigationButtons/BestScores");
            _menu[3].LoadContent(content, "Views/NavigationButtons/Help");
            _menu[4].LoadContent(content, "Views/NavigationButtons/Exit");
        }

        /// <summary>
        /// Metoda inicjalizująca widok menu. 
        /// Ustawia na odpowiednie pozycje wszystkie obiekty graficzne.
        /// </summary>
        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            _grass[0].Initialize(new Vector2(0, 160), Color.White, screenWidth, 1);
            _grass[1].Initialize(new Vector2(0, 160-7), Color.White, screenWidth, -1);
            _grass[2].Initialize(new Vector2(0, 160-12), Color.White, screenWidth, -2);

            _sky.Initialize(Vector2.Zero, Color.White);
            _cloud.Initialize(new Vector2(0, 30), Color.White, screenWidth, -1);
            _groundBuffer.Initialize(new Vector2(0, 229), Color.White);

            _menuFrame.Initialize(new Vector2((screenWidth-_menuFrame.Image.Width)/2, 320), Color.White);
            int nextPosition = 490;
            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].ButtonGraphic.Initialize(new Vector2((screenWidth - 340) / 2, nextPosition), 340, 25, 1, 100, Color.White);
                nextPosition += _menu[i].ButtonGraphic.FrameHeight+15;
            }
        }

        /// <summary>
        /// Metoda wywołująca rysowanie obiektów interfejsu.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            _sky.Draw(spriteBatch);
            _cloud.Draw(spriteBatch);
            _groundBuffer.Draw(spriteBatch);

            for (int i = _grass.Length - 1; i >= 0; i--)
            {
                _grass[i].Draw(spriteBatch);
            }

            _menuFrame.Draw(spriteBatch);
            for (int i = 0; i < _menu.Length; i++)
            {
                _menu[i].Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Metoda uaktualniająca obiekty interfejsu.
        /// Odpowiada za obsługę zdarzeń.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            // Grass
            for (int i = 0; i < _grass.Length; i++)
            {
                _grass[i].Update();
            }

            // Clouds
            _cloud.Update();

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
                        _menu[i].ButtonState = ButtonState.Click;
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