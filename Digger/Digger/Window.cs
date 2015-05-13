using System;
using System.Collections.Generic;
using System.Linq;
using Digger.Controller;
using Digger.Views;
using Digger.Views.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Digger
{
    /// <summary>
    /// G³ówne okno aplikacji.
    /// </summary>
    public class Window : Microsoft.Xna.Framework.Game
    {
        public static Context Context;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Konstruktor bezparametrowy
        /// </summary>
        public Window()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Wielkoœæ obrazu
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;

            if (graphics.PreferredBackBufferHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height &&
            graphics.PreferredBackBufferWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
            {
                Logger.Report("Uruchomienie w trybie pe³nego ekranu.");
                graphics.IsFullScreen = true;
            }
            else
            {
                Logger.Report("Uruchomienie w trybie okna. Gra nie mieœci siê na ekranie.");
            }

            IsMouseVisible = true;
        }

        /// <summary>
        /// Inicjalizuj obiekty.
        /// Wykonuje inicjalizacje bie¿¹cego kontekstu oraz przekierowywuje widok do menu.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Initialize Context object
            Context = new Context(Content);
            Navigator.NavigateTo(typeof(Menu));
        }

        /// <summary>
        /// Wczytuje pliki zewnêtrzne.
        /// Inicjalizuje pow³okê graficzn¹.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Zadaniem funkcji jest posprz¹tanie po zakoñczeniu pracy.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Wywo³uje logikê danego obiketu widoku.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacji na temat czasu gry.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Context.CurrentView.Update(gameTime);
            if (Context.ReadyToExit) Exit();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Pozwala na rysowanie obiektów znajduj¹cych siê w wybranym widoku.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacji na temat czasu gry.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            Context.CurrentView.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
