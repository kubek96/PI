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
        
        public Button(Type navigationType)
        {
            _mouseState = MouseState.MouseLeave;
            _navigationType = navigationType;

            _buttonGraphic = new AnimatedGraphic();
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

        public void LoadContent(ContentManager content, string buttonGraphic, string buttonAnimation = null)
        {
            _buttonGraphic.LoadContent(content, buttonGraphic);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //if (_mouseState == MouseState.MouseOn)
            //{
            //    //_buttonGraphic.
            //    return;    
            //}

            //if (_mouseState == MouseState.Click)
            //{
            //    return;  
            //}

            _buttonGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
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