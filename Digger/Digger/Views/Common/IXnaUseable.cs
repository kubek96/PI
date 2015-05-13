using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Views.Common
{
    /// <summary>
    /// Interfejs gwarantujący kompatybilność obiektów wiodku z standardami XNA.
    /// </summary>
    public interface IXnaUseable
    {
        /// <summary>
        /// Odpowiada za wczytywanie plików peryferyjnych.
        /// </summary>
        /// <param name="content"></param>
        void LoadContent(ContentManager content);
        
        /// <summary>
        /// Wykonuje niezbędne operacje między innymi umiejscowienia obiektów na planszy.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Wywołuje operację rysowania się obiektów.
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        /// <param name="gameTime">Dostarcza informacji na temat czasu gry.</param>
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        
        /// <summary>
        /// Obsługuje logikę danego widoku.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacji na temat czasu gry.</param>
        void Update(GameTime gameTime);
    }
}