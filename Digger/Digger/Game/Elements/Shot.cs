using System;
using Digger.Game.Common;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public enum ShotType
    {
        Acid,
        Venom
    }

    public class Shot
    {
        private AnimatedGraphic _shotGraphic;
        private Rectangle _shotRectangle;
        private Direction _direction;
        private ShotType _shotType;
        private int _speed;
        private bool _shootSomething;

        public Shot(ShotType shotType, string assetName)
        {
            _shotType = shotType;
            _shotGraphic = new AnimatedGraphic();
            _speed = 14;
            _shootSomething = false;
            LoadContent(Game1.Context.Content, assetName);
        }

        public Rectangle ShotRectangle
        {
            get { return _shotRectangle; }
        }

        public Shot(Shot shot)
        {
            _shotGraphic = shot._shotGraphic.Clone();
            _shotType = shot._shotType;
            _speed = shot._speed;
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _shotGraphic.LoadContent(content, assetName);
        }

        public void Initialize(Rectangle rectangle, Direction direction)
        {
            _direction = direction;
            _shotRectangle = rectangle;
            _shotGraphic.Initialize(new Vector2(_shotRectangle.X + 15, _shotRectangle.Y + 15), 10, 10, 2, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _shotGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _shotGraphic.Update(gameTime);
        }

        public bool ShootSomething
        {
            get { return _shootSomething; }
            set { _shootSomething = value; }
        }

        public void Move()
        {
            switch (_direction)
            {
                case Direction.Up:
                    _shotRectangle = new Rectangle(_shotRectangle.X, _shotRectangle.Y - _speed, _shotRectangle.Width, _shotRectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X, _shotGraphic.Position.Y - _speed);
                    break;
                case Direction.Right:
                    _shotRectangle = new Rectangle(_shotRectangle.X + _speed, _shotRectangle.Y, _shotRectangle.Width, _shotRectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X + _speed, _shotGraphic.Position.Y);
                    break;
                case Direction.Down:
                    _shotRectangle = new Rectangle(_shotRectangle.X, _shotRectangle.Y + _speed, _shotRectangle.Width, _shotRectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X, _shotGraphic.Position.Y + _speed);
                    break;
                case Direction.Left:
                    _shotRectangle = new Rectangle(_shotRectangle.X - _speed, _shotRectangle.Y, _shotRectangle.Width, _shotRectangle.Height);
                    _shotGraphic.Position = new Vector2(_shotGraphic.Position.X - _speed, _shotGraphic.Position.Y);
                    break;
            }
        }
    }
}