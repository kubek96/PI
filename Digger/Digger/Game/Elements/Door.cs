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
        private List<Enemy> _enemiesToGenerate;
        private int _releasedEnemies;
        private Rectangle _doorRectangle;


        public Door(List<Enemy> enemies)
        {
            _doorGraphic = new AnimatedGraphic();
            _enemiesToGenerate = enemies;
            _releasedEnemies = 0;
        }

        public Enemy ReleaseNextEnemy()
        {
            return _enemiesToGenerate[_releasedEnemies++];
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
    }
}