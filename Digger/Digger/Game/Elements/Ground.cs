using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public enum GroundType
    {
        Normal,
        Free
    }

    public class Ground
    {
        private FixedGraphic _freeGroundGraphic;
        private FixedGraphic _groundGraphic;
        private GroundType _groundType;

        public Ground()
        {
            _groundGraphic = new FixedGraphic();
            _freeGroundGraphic = new FixedGraphic();
        }

        public GroundType GroundType
        {
            get { return _groundType; }
            set { _groundType = value; }
        }

        public Rectangle Rectangle
        {
            get { return _groundGraphic.DestRectangle; }
        }

        public void LoadContent(ContentManager content, string normalGroundGraphic, string freeGroundGraphic)
        {
            _groundGraphic.LoadContent(content, normalGroundGraphic);
            _freeGroundGraphic.LoadContent(content, freeGroundGraphic);
        }

        public void Initialize(Vector2 position, GroundType groundType=GroundType.Normal)
        {
            _groundType = groundType;
            _groundGraphic.Initialize(position, Color.White);
            _freeGroundGraphic.Initialize(position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_groundType == GroundType.Normal)
            {
                _groundGraphic.Draw(spriteBatch);
                return;
            }
            if (_groundType == GroundType.Free)
            {
                _freeGroundGraphic.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}