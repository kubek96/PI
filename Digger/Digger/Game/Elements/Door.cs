using System.Collections.Generic;
using Digger.Graphic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Game.Elements
{
    /// <summary>
    /// Klasa drzwi.
    /// </summary>
    public class Door
    {
        private AnimatedGraphic _doorGraphic;
        private Stack<Enemy> _enemiesToGenerate;
        private Rectangle _rectangle;
        private int _timeToNextEnemie;
        private Enemy _rat;
        private bool _areOpen;

        #region Properties
        /// <summary>
        /// Obszar zajmowany przez drzwi.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
        }

        /// <summary>
        /// Mówi, czy drzwi są już otwarte.
        /// </summary>
        public bool AreOpen
        {
            get { return _areOpen; }
        }
        
        /// <summary>
        /// Zwraca czas, jaki został ustawiony do wypuszczenia kolejnego wroga.
        /// </summary>
        public int TimeToNextEnemie
        {
            get { return _timeToNextEnemie; }
        }

        /// <summary>
        /// Obiekt szczura.
        /// </summary>
        public Enemy Rat
        {
            get { return _rat; }
            set { _rat = value; }
        }
        #endregion

        /// <summary>
        /// Konstruktor.
        /// Ustawia obiekt szczura na null oraz informację dotyczącą stanu otwarcia drzwi na false.
        /// </summary>
        /// <param name="enemies">Stos wrogów, jacy mają wychodzić przez drzwi.</param>
        /// <param name="timeToNextEnemie">Odstęp czasu jaki ma być między kolejnymi pojawieniami się wrogów.</param>
        public Door(Stack<Enemy> enemies, int timeToNextEnemie)
        {
            _doorGraphic = new AnimatedGraphic();
            _enemiesToGenerate = enemies;
            _timeToNextEnemie = timeToNextEnemie;
            _rat = null;
            _areOpen = false;
        }

        /// <summary>
        /// Funkcja otwierająca dzrwi.
        /// </summary>
        /// <returns>Obiekt szczura (jeżeli został ustawiony) lub null, jeżeli żaden z wrogów nie wkroczył w cukierek.</returns>
        public Enemy OpenDoor()
        {
            _areOpen = true;
            _doorGraphic.MoveToFrame(1);
            return _rat;
        }

        /// <summary>
        /// Funkcja ustawiająca obiekt szczura.
        /// Dokonuje jego inicjalizacji w miejscu drzwi.
        /// </summary>
        /// <param name="enemy">Obiekt szczura (lub defakto każdego innego wroga).</param>
        public void ReleaseRatWhenOpen(Enemy enemy)
        {
            if (_rat != null || _areOpen) return;
            _rat = enemy;
            _rat.Initialize(_rectangle);
        }

        /// <summary>
        /// Funkcja uwalniajaca kolejnego wroga.
        /// Jako jego pozycję ustawia położenie drzwi.
        /// </summary>
        /// <returns>Obiekt wroga.</returns>
        public Enemy ReleaseNextEnemy()
        {
            if (_enemiesToGenerate.Count == 0) return null;

            Enemy e = _enemiesToGenerate.Pop();
            e.Initialize(_rectangle);
            return e;
        }

        /// <summary>
        /// Metoda pozwala na natychmiastowe uwolnienie wskazanego wroga z pozycji drzwi.
        /// </summary>
        /// <param name="enemy">Obiekt wroga.</param>
        /// <returns>Obiekt wroga w nowej pozycji.</returns>
        public Enemy ReleaseNewEnemyNow(Enemy enemy)
        {
            enemy.Initialize(_rectangle);
            return enemy;
        }

        /// <summary>
        /// Wczytuje zawrtości graficzne drzwi.
        /// </summary>
        /// <param name="content">Obiekt managera zasobów.</param>
        /// <param name="assetName">Nazwa zasobu.</param>
        public void LoadContent(ContentManager content, string assetName)
        {
            _doorGraphic.LoadContent(content, assetName);
        }

        /// <summary>
        /// Dokonuje inicjalizację obiektu drzwi w podanej pozycji.
        /// </summary>
        /// <param name="rectangle">Pozycja drzwi.</param>
        public void Initialize(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _doorGraphic.Initialize(new Vector2(_rectangle.X + 5, _rectangle.Y + 5), 30, 30, 1, 100, Color.White);
        }

        /// <summary>
        /// Wykonuje rysowanie obiektu drzwi w ich odpowiednim stanie. 
        /// </summary>
        /// <param name="spriteBatch">Powłoka graficzna.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_areOpen) _doorGraphic.MoveToFrame(0);
            else _doorGraphic.MoveToFrame(1);

            _doorGraphic.Draw(spriteBatch);
        }

        /// <summary>
        /// Dokonuje aktualizacji grafiki drzwi.
        /// </summary>
        /// <param name="gameTime">Ramka czasowa.</param>
        public void Update(GameTime gameTime)
        {
            _doorGraphic.Update(gameTime);
        }
    }
}