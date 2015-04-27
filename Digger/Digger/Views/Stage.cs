using System;
using System.Collections.Generic;
using System.Linq;
using Digger.Game;
using Digger.Game.Common;
using Digger.Game.Elements;
using Digger.Graphic;
using Digger.Views.Common;
using Digger.Views.Common.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Digger.Views
{
    public class Stage : IXnaUseable
    {
        private static StageHelper _stageHelper;

        private FixedGraphic _background;
        private Worm _worm;
        private Ground[,] _grounds;
        private Rectangle _gameField;
        private Keys[] _lastPressedKeys;
        private int _elapsedTime;
        private List<Enemy> _enemies;
        private List<Fruit> _redFruits;
        private List<Fruit> _grabbableFruits;
        private List<Fruit> _rootenKiwis;
        private Door _door;

        private Fruit[] _interfaceFruits;
        private Label[] _interfaceFruitsXs;
        private Label[] _interfaceFruitsCounts;

        public Stage()
        {
            _stageHelper = new StageHelper();

            // Tło
            _background = new FixedGraphic();

            _grounds = new Ground[20,20];
            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    _grounds[i,j] = new Ground();
                } 
            }

            _elapsedTime = 0;

            _worm = new Worm();
            _lastPressedKeys = new Keys[0];

            // Owoce
            _redFruits = new List<Fruit>();
            _grabbableFruits = _stageHelper.GenerateGrabableFruits(20);
            _rootenKiwis = new List<Fruit>();

            // Drzwi
            _door = new Door(null);

            // Interfejs użytkownika
            _interfaceFruits = _stageHelper.GenerateInterfaceFruits();
            _interfaceFruitsXs = new Label[4];
            for (int i = 0; i < _interfaceFruitsXs.Length; i++)
            {
                _interfaceFruitsXs[i] = new Label("x");
            }
            _interfaceFruitsCounts = new Label[4];
            _interfaceFruitsCounts[0] = new Label(_worm.AcidShoots.ToString());
            _interfaceFruitsCounts[1] = new Label(_worm.VenomShoots.ToString());
            _interfaceFruitsCounts[2] = new Label(_worm.KiwisCount.ToString());
            _interfaceFruitsCounts[3] = new Label(_worm.MudCount.ToString());

            // Wczytaj zawartość
            LoadContent(Game1.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            _background.LoadContent(content, "Views/Common/Background");

            Random rnd = new Random();
            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    if (rnd.Next(10) < 9) _grounds[i,j].LoadContent(content, "Game/GroundOne", "Game/GroundFree");
                    else _grounds[i,j].LoadContent(content, "Game/GroundTwo", "Game/GroundFree");
                }
            }

            _worm.LoadContent(content, "Game/Worm");
            _door.LoadContent(content, "Game/Door");

            // Elementy interfejsu użytkownika
            for (int i = 0; i < _interfaceFruitsXs.Length; i++)
            {
                _interfaceFruitsXs[i].LoadContent(content, "Fonts/Silkscreen");
                _interfaceFruitsCounts[i].LoadContent(content, "Fonts/Silkscreen");
            }
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            int horizontalShift = (screenWidth - (int)Math.Sqrt(_grounds.Length) * 41)/2;
            int verticalShift = (screenHeight - (int)Math.Sqrt(_grounds.Length) * 41)/2;
            _gameField = new Rectangle(horizontalShift, verticalShift, (int)Math.Sqrt(_grounds.Length) * 41, (int)Math.Sqrt(_grounds.Length) * 41);

            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    _grounds[i,j].Initialize(new Vector2(i * 41 + horizontalShift, j * 41 + verticalShift));
                }
            }

            Point[] freeGrounds = _stageHelper.GenerateFreeGroundsCoordinates(3);
            for (int i = 0; i < freeGrounds.Length; i++)
            {
                _grounds[freeGrounds[i].X, freeGrounds[i].Y].GroundType = GroundType.Free;
            }

            _worm.Initialize(_grounds[0, (int)Math.Sqrt(_grounds.Length)-1].Rectangle);
            _door.Initialize(_grounds[(int)Math.Sqrt(_grounds.Length) - 1, 0].Rectangle);

            Random random = new Random();
            int x, y;
            for (int i = 0; i < _grabbableFruits.Count; i++)
            {
                x = random.Next(0, (int) Math.Sqrt(_grounds.Length))*41 + horizontalShift; 
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 41 + verticalShift;
                // Randomowe koordynaty
                // TODO: Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                _grabbableFruits[i].Initialize(new Rectangle(x, y, 40, 40));
            }

            // Inicjalizuj obiekty interfesju użytkownika
            x = horizontalShift/4;
            y = screenHeight/2;
            for (int i = 0; i < _interfaceFruits.Length; i++)
            {
                _interfaceFruitsXs[i].Initialize(new Vector2(x+50, y));
                _interfaceFruitsCounts[i].Initialize(new Vector2(x + 100, y));
                _interfaceFruits[i].Initialize(new Rectangle(x, y, 40, 40));
                y += 50;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _background.Draw(spriteBatch);

            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    _grounds[i, j].Draw(spriteBatch);
                }
            }

            //spriteBatch.Draw(_background.Image, _gameField, Color.Violet);
            for (int i = 0; i < _grabbableFruits.Count; i++)
            {
                _grabbableFruits[i].Draw(spriteBatch);
            }

            _worm.Draw(spriteBatch);
            _door.Draw(spriteBatch);

            // Elementy interfejsu użytkownika
            for (int i = 0; i < _interfaceFruits.Length; i++)
            {
                _interfaceFruits[i].Draw(spriteBatch);
                _interfaceFruitsCounts[i].Draw(spriteBatch);
                _interfaceFruitsXs[i].Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Obsługuj ruch robaczka
            KeyboardState newKeyboardState = Keyboard.GetState();
            Keys[] keys = newKeyboardState.GetPressedKeys();

            _elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            for (int i = 0; i < keys.Length; i++)
            {
                if (_elapsedTime < _worm.Speed) 
                    continue;
                //if (!_lastPressedKeys.Contains(keys[i]))
                // Klawisz w dole
                if (!Game1.Context.Player.UserControls.ContainsKey(keys[i]))
                    continue;
                // Sprawdź, czy po ruchu gracz jest wewnątrz obszaru grania
                if (!_gameField.Contains(_worm.TestMove(Game1.Context.Player.UserControls[keys[i]]))) 
                    continue;

                _worm.Move(Game1.Context.Player.UserControls[keys[i]]);
                // Sprawdź przecięcia
                for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                {
                    // Przecięcia z ziemią
                    for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                    {
                        if (_worm.WormRectangle.Intersects(_grounds[x, y].Rectangle))
                        {
                            _grounds[x, y].GroundType = GroundType.Free;
                        }
                    }
                    // Przecięcia z owocami
                    for (int j = 0; j < _grabbableFruits.Count; j++)
                    {
                        if (_worm.WormRectangle.Intersects(_grabbableFruits[j].FruitRectangle) && _grabbableFruits[j].IsUsed == false)
                        {
                            _grabbableFruits[j].PlayerUse(_worm);
                            _grabbableFruits[j].IsUsed = true;
                        }
                    }
                }
                _elapsedTime = 0;
            }

            _lastPressedKeys = keys.ToArray();
            // Robaczek
            _worm.Update(gameTime);
            _door.Update(gameTime);


            // Uaktualnienie elementów interfejsu użytkownika
            _interfaceFruitsCounts[0].Text = _worm.AcidShoots.ToString();
            _interfaceFruitsCounts[1].Text = _worm.VenomShoots.ToString();
            _interfaceFruitsCounts[2].Text = _worm.KiwisCount.ToString();
            _interfaceFruitsCounts[3].Text = _worm.MudCount.ToString();

            // Obsługa usuwania elementów
            // Usuwanie użytych owoców
            for (int i = 0; i < _grabbableFruits.Count; i++)
            {
                if (_grabbableFruits[i].IsUsed) _grabbableFruits.RemoveAt(i);
            }
        }
    }
}