using Digger.Data;
using Digger.Views;
using Digger.Views.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Controller
{
    public class Context
    {
        private IXnaUseable _currentView;
        private ContentManager _content;
        private bool _readyToExit;
        private Player _player;

        //private GraphicsDevice _graphicsDevice;

        public Context(ContentManager content)
        {
            _content = content;
            //_graphicsDevice = graphicsDevice;
            _readyToExit = false;
            // TODO: To znika:
            _player = new Player();
        }

        public IXnaUseable CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }

        public bool ReadyToExit
        {
            get { return _readyToExit; }
            set { _readyToExit = value; }
        }

        public ContentManager Content
        {
            get { return _content; }
        }

        public Player Player
        {
            get { return _player; }
        }
    }
}