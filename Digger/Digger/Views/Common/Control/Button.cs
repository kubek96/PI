using System;
using System.Runtime.InteropServices;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Digger.Views.Common.Control
{
    /// <summary>
    /// Klasa przycisków.
    /// </summary>
    public class Button
    {
        private AnimatedGraphic _buttonAnimation;
        private AnimatedGraphic _buttonGraphic;
        private Type _navigationType;
        private ButtonState _buttonState; // czy myszka znajduje się nad obiektem
        private int? _passingParatemer;
        private Label _label;
        
        /// <summary>
        /// Konstruktor.
        /// Jeżeli pole label zostanie ustawione button przestanie działać w trybie wyświetlania grafiki.
        /// Będzie wyświetlał i wykonywał operacje na tekście.
        /// </summary>
        /// <param name="navigationType">Typ klasy widoku, do którego ma nastąpić przekierowanie w razie zmiany stanu na kliknięto.</param>
        /// <param name="args">Parametry opcjonalne potrzebne do inicjowania obiektów widoków.</param>
        /// <param name="label">Tekst do trybu tekstowego buttona.</param>
        public Button(Type navigationType, int? args = null, string label=null)
        {
            _buttonState = ButtonState.MouseLeave;
            _navigationType = navigationType;
            _passingParatemer = args;

            _buttonGraphic = new AnimatedGraphic();
            if (label != null) _label = new Label(label);
            else _label = null;
        }

        #region Properties
        public AnimatedGraphic ButtonGraphic
        {
            get { return _buttonGraphic; }
            set { _buttonGraphic = value; }
        }

        public ButtonState ButtonState
        {
            get { return _buttonState; }
            set { _buttonState = value; }
        }

        public Label Label
        {
            get { return _label; }
        }
        #endregion

        /// <summary>
        /// Metoda wczytująca zasoby potrzebne do wyświetlania przycisku.
        /// </summary>
        /// <param name="content">Wskazanie na obiekt zarządcy zasobami.</param>
        /// <param name="assetName">Ścieżka do zasobu.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            if (_label != null)
            {
                _label.LoadContent(content, assetName); 
                return;
            }
            _buttonGraphic.LoadContent(content, assetName);
        }

        /// <summary>
        /// Metoda rysująca button.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Informacja na temat czasu gry.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_label != null)
            {
                _label.Draw(spriteBatch);
                return;
            }

            _buttonGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Wykonuje operacje związane z obsługą zachowania buttona.
        /// </summary>
        /// <param name="gameTime">Informacja na temat czasu gry.</param>
        public void Update(GameTime gameTime)
        {
            if (_label != null)
            {
                if (_buttonState == ButtonState.MouseOn)
                {
                    _label.Color = Color.Yellow;
                    return;
                }
                if (_buttonState == ButtonState.Click)
                {
                    _label.Color = Color.Magenta;
                    // Invoke navigate mission
                    Navigator.NavigateTo(_navigationType, _passingParatemer);
                    return;
                }
                _label.Color = Color.White;
                return;
            }

            if (_buttonState == ButtonState.MouseOn)
            {
                _buttonGraphic.MoveToFrame(1);
                return;
            }

            if (_buttonState == ButtonState.Click)
            {
                _buttonGraphic.MoveToFrame(2);
                // Invoke navigate mission
                Navigator.NavigateTo(_navigationType, _passingParatemer);
                return;
            }

            _buttonGraphic.MoveToFrame(0);
            _buttonGraphic.Update(gameTime);
        } 
    }
}