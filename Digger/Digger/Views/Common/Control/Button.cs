using System;
using System.Runtime.InteropServices;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Digger.Views.Common.Control
{
    public class Button
    {
        private AnimatedGraphic _buttonAnimation;
        private AnimatedGraphic _buttonGraphic;
        private Type _navigationType;
        private MouseState _mouseState; // czy myszka znajduje się nad obiektem
        private int? _passingParatemer;
        private Label _label;
        
        public Button(Type navigationType, int? args = null, string label=null)
        {
            _mouseState = MouseState.MouseLeave;
            _navigationType = navigationType;
            _passingParatemer = args;

            _buttonGraphic = new AnimatedGraphic();
            if (label != null) _label = new Label(label);
            else _label = null;
        }

        public AnimatedGraphic ButtonGraphic
        {
            get { return _buttonGraphic; }
            set { _buttonGraphic = value; }
        }

        public MouseState MouseState
        {
            get { return _mouseState; }
            set { _mouseState = value; }
        }

        public Label Label
        {
            get { return _label; }
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            if (_label != null)
            {
                _label.LoadContent(content, assetName); 
                return;
            }
            _buttonGraphic.LoadContent(content, assetName);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_label != null)
            {
                _label.Draw(spriteBatch);
                return;
            }

            _buttonGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (_label != null)
            {
                if (_mouseState == MouseState.MouseOn)
                {
                    _label.Color = Color.Yellow;
                    return;
                }
                if (_mouseState == MouseState.Click)
                {
                    _label.Color = Color.Magenta;
                    return;
                }
                _label.Color = Color.White;
                return;
            }

            if (_mouseState == MouseState.MouseOn)
            {
                _buttonGraphic.MoveToFrame(1);
                return;
            }

            if (_mouseState == MouseState.Click)
            {
                _buttonGraphic.MoveToFrame(2);
                // Invoke navigate mission
                Navigator.NavigateTo(_navigationType);
                return;
            }

            _buttonGraphic.MoveToFrame(0);
            _buttonGraphic.Update(gameTime);
        } 
    }
}