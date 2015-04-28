using System.Collections.Generic;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    public class Door
    {
        private AnimatedGraphic _doorGraphic;
        private Stack<Enemy> _enemiesToGenerate;
        private Rectangle _doorRectangle;
        private int _timeToNextEnemie;

        public Door(Stack<Enemy> enemies, int timeToNextEnemie)
        {
            _doorGraphic = new AnimatedGraphic();
            _enemiesToGenerate = enemies;
            _timeToNextEnemie = timeToNextEnemie;
        }

        public Enemy ReleaseNextEnemy()
        {
            if (_enemiesToGenerate.Count == 0) return null;

            Enemy e = _enemiesToGenerate.Pop();
            e.Initialize(_doorRectangle);
            return e;
        }

        public void LoadContent(ContentManager content, string assetName)
        {
            _doorGraphic.LoadContent(content, assetName);
        }

        public void Initialize(Rectangle rectangle)
        {
            _doorRectangle = rectangle;
            _doorGraphic.Initialize(new Vector2(_doorRectangle.X + 5, _doorRectangle.Y + 5), 30, 30, 1, 100, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _doorGraphic.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _doorGraphic.Update(gameTime);
        }

        public int TimeToNextEnemie
        {
            get { return _timeToNextEnemie; }
        }
    }
}