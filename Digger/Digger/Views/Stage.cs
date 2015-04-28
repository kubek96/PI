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
        private int _nextEnemieElapsedTime;
        private int _enemieMoveElapsedTime;
        private List<Enemy> _enemies;
        private List<Fruit> _redFruits;
        private List<Fruit> _grabbableFruits;
        private List<Fruit> _rootenKiwis;
        private Door _door;
        private List<Shot> _shots;

        private Fruit[] _interfaceFruits;
        private Label[] _interfaceFruitsXs;
        private Label[] _interfaceFruitsCounts;
        
        private FixedGraphic _interfaceLife;
        private Label _interfaceLifeX;
        private Label _interfaceLifeCount;

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
            _nextEnemieElapsedTime = 0;
            _enemieMoveElapsedTime = 0; 

            _worm = new Worm();
            _lastPressedKeys = new Keys[0];

            // Owoce
            _redFruits = _stageHelper.GenerateRedFruits(10);
            _grabbableFruits = _stageHelper.GenerateGrabableFruits(20);
            _rootenKiwis = new List<Fruit>();

            // Drzwi
            _door = new Door(_stageHelper.GenerateEnemies(6), 8000);

            // Wrogowie 
            _enemies = new List<Enemy>();

            // Strzały
            _shots = new List<Shot>();

            // Interfejs użytkownika
            _interfaceFruits = _stageHelper.GenerateInterfaceFruits();
            _interfaceFruitsXs = new Label[5];
            for (int i = 0; i < _interfaceFruitsXs.Length; i++)
            {
                _interfaceFruitsXs[i] = new Label("x");
            }
            _interfaceFruitsCounts = new Label[5];
            _interfaceFruitsCounts[0] = new Label(_worm.AcidShoots.ToString());
            _interfaceFruitsCounts[1] = new Label(_worm.VenomShoots.ToString());
            _interfaceFruitsCounts[2] = new Label(_worm.PlumsCount.ToString());
            _interfaceFruitsCounts[3] = new Label(_worm.KiwisCount.ToString());
            _interfaceFruitsCounts[4] = new Label(_worm.MudCount.ToString());

            _interfaceLife = new FixedGraphic();
            _interfaceLifeX = new Label("x");
            _interfaceLifeCount = new Label(_worm.Life.ToString());

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

            _interfaceLife.LoadContent(content, "Game/Interface/Heart");
            _interfaceLifeX.LoadContent(content, "Fonts/Silkscreen");
            _interfaceLifeCount.LoadContent(content, "Fonts/Silkscreen");
        }

        public void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _background.Initialize(new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            int horizontalShift = (screenWidth - (int)Math.Sqrt(_grounds.Length) * 42)/2;
            int verticalShift = (screenHeight - (int)Math.Sqrt(_grounds.Length) * 42)/2;
            _gameField = new Rectangle(horizontalShift, verticalShift, (int)Math.Sqrt(_grounds.Length) * 42, (int)Math.Sqrt(_grounds.Length) * 42);

            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    _grounds[i,j].Initialize(new Vector2(i * 42 + horizontalShift, j * 42 + verticalShift));
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
                x = random.Next(0, (int) Math.Sqrt(_grounds.Length))*42 + horizontalShift; 
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + verticalShift;
                // Randomowe koordynaty
                // TODO: Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                _grabbableFruits[i].Initialize(new Rectangle(x, y, 40, 40));
            }
            for (int i = 0; i < _redFruits.Count; i++)
            {
                x = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + horizontalShift;
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + verticalShift;
                // Randomowe koordynaty
                // TODO: Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                _redFruits[i].Initialize(new Rectangle(x, y, 40, 40));
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

            y = screenHeight / 2;
            _interfaceLife.Initialize(new Vector2(x, y - 200), Color.White);
            _interfaceLifeX.Initialize(new Vector2(x+50, y - 200));
            _interfaceLifeCount.Initialize(new Vector2(x + 100, y - 200));
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
            // Rysuj grabalbe owocki
            for (int i = 0; i < _grabbableFruits.Count; i++)
            {
                _grabbableFruits[i].Draw(spriteBatch);
            }
            // Rysuj czerwone owocki
            for (int i = 0; i < _redFruits.Count; i++)
            {
                _redFruits[i].Draw(spriteBatch);
            }

            _door.Draw(spriteBatch);
            
            // Rysuj zgniłe kiwi
            for (int i = 0; i < _rootenKiwis.Count; i++)
            {
                _rootenKiwis[i].Draw(spriteBatch);
            }
            
            // Rysuj przeciwników
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Draw(spriteBatch);
            }

            // Rysuj strzały
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Draw(spriteBatch);
            }

            // Elementy interfejsu użytkownika
            for (int i = 0; i < _interfaceFruits.Length; i++)
            {
                _interfaceFruits[i].Draw(spriteBatch);
                _interfaceFruitsCounts[i].Draw(spriteBatch);
                _interfaceFruitsXs[i].Draw(spriteBatch);
            }
            _interfaceLife.Draw(spriteBatch);
            _interfaceLifeX.Draw(spriteBatch);
            _interfaceLifeCount.Draw(spriteBatch);

            _worm.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            // Obsługuj ruch robaczka
            KeyboardState newKeyboardState = Keyboard.GetState();
            Keys[] keys = newKeyboardState.GetPressedKeys();

            _elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            _nextEnemieElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            _enemieMoveElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            for (int i = 0; i < keys.Length; i++)
            {
                if (_elapsedTime < _worm.Speed) 
                    continue;
                //if (!_lastPressedKeys.Contains(keys[i]))
                // Klawisz w dole

                // Ruch:
                if (!Game1.Context.Player.UserControls.ContainsKey(keys[i]))
                {
                    // Budowanie grudek ziemi
                    if (keys[i] == Keys.D5 && _worm.MudCount > 0)
                    {
                        Rectangle rectangle = new Rectangle();
                        switch (_worm.Direction)
                        {
                            case Direction.Up:
                                rectangle = new Rectangle(_worm.WormRectangle.X, _worm.WormRectangle.Y - 42, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Right:
                                rectangle = new Rectangle(_worm.WormRectangle.X + 42, _worm.WormRectangle.Y, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Down:
                                rectangle = new Rectangle(_worm.WormRectangle.X, _worm.WormRectangle.Y + 42, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Left:
                                rectangle = new Rectangle(_worm.WormRectangle.X - 42, _worm.WormRectangle.Y, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                        }
                        for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                        {
                            for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                            {
                                if (rectangle.Intersects(_grounds[x, y].Rectangle))
                                {
                                    if (_grounds[x, y].GroundType != GroundType.Free) continue;
                                    // Zmniejsz licznik dostępnej ziemi
                                    _worm.MudCount--;
                                    _grounds[x, y].GroundType = GroundType.Normal;
                                }
                            }
                        }
                        continue;
                    }
                    // Strzelanie Acid
                    if (keys[i] == Keys.D1 && _worm.AcidShoots > 0)
                    {
                        Shot shot = new Shot(_stageHelper.ShotTemplates[ShotType.Acid]);
                        shot.Initialize(_worm.WormRectangle, _worm.Direction);
                        _shots.Add(shot);
                        _worm.AcidShoots--;
                        continue;
                    }
                    // Strzelanie Venom
                    if (keys[i] == Keys.D2 && _worm.VenomShoots > 0)
                    {
                        Shot shot = new Shot(_stageHelper.ShotTemplates[ShotType.Venom]);
                        shot.Initialize(_worm.WormRectangle, _worm.Direction);
                        _shots.Add(shot);
                        _worm.VenomShoots--;
                        continue;
                    }
                    // Wykorzystanie śliwki
                    if (keys[i] == Keys.D3 && _worm.PlumsCount > 0)
                    {
                        _worm.MoveFaster(6, 5000);
                        continue;
                    }
                    // Zastawienie kiwi
                    if (keys[i] == Keys.D4 && _worm.KiwisCount > 0)
                    {
                        Rectangle rectangle = new Rectangle();
                        switch (_worm.Direction)
                        {
                            case Direction.Up:
                                rectangle = new Rectangle(_worm.WormRectangle.X, _worm.WormRectangle.Y - 42, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Right:
                                rectangle = new Rectangle(_worm.WormRectangle.X + 42, _worm.WormRectangle.Y, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Down:
                                rectangle = new Rectangle(_worm.WormRectangle.X, _worm.WormRectangle.Y + 42, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                            case Direction.Left:
                                rectangle = new Rectangle(_worm.WormRectangle.X - 42, _worm.WormRectangle.Y, _worm.WormRectangle.Width, _worm.WormRectangle.Height);
                                break;
                        }
                        for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                        {
                            for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                            {
                                if (rectangle.Intersects(_grounds[x, y].Rectangle))
                                {
                                    if (_grounds[x, y].GroundType != GroundType.Free) continue;
                                    Fruit kiwi = new Fruit(_stageHelper.FruitsTemplates[FruitType.Kiwi]);
                                    kiwi.Initialize(rectangle);
                                    _rootenKiwis.Add(kiwi);
                                    _worm.KiwisCount--;
                                }
                            }
                        }
                        continue;
                    }
                    continue;
                }

                // Sprawdź, czy po ruchu gracz jest wewnątrz obszaru grania
                if (!_gameField.Contains(_worm.TestMove(Game1.Context.Player.UserControls[keys[i]]))) 
                    continue;
                // Sprawdź czy robaczek nie w ruchu
                if (_worm.IsMoving) 
                    continue;

                _worm.MakeMove(Game1.Context.Player.UserControls[keys[i]]);
               
                _elapsedTime = 0;
            }
            _lastPressedKeys = keys.ToArray();
            // Robaczek

            // Sprawdź przecięcia
            for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
            {
                // Przecięcia z ziemią
                for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                {
                    if (_worm.WormRectangle.Intersects(_grounds[x, y].Rectangle))
                    {
                        if (_grounds[x, y].GroundType == GroundType.Free) continue;

                        // Rozpocznij proces zwolnienia
                        _worm.IsDigging = true;
                        // Usun ziemię
                        _grounds[x, y].GroundType = GroundType.Free;
                    }
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
                for (int i = 0; i < _enemies.Count; i++)
                {
                    if (_enemies[i].EnemyRectangle.Intersects(_grabbableFruits[j].FruitRectangle) && _grabbableFruits[j].IsUsed == false)
                    {
                        _grabbableFruits[j].EnemyUse(_enemies[i]);
                        _grabbableFruits[j].IsUsed = true;
                    }
                }
            }
            // Przecięcia z czerwonymi owocami
            for (int j = 0; j < _redFruits.Count; j++)
            {
                if (_worm.WormRectangle.Intersects(_redFruits[j].FruitRectangle) && _redFruits[j].IsUsed == false)
                {
                    _redFruits[j].PlayerUse(_worm);
                    _redFruits[j].IsUsed = true;
                }
            }

            _worm.Update(gameTime);
            _door.Update(gameTime);

            // Generuj kolejnych przeciwników
            if (_nextEnemieElapsedTime > _door.TimeToNextEnemie)
            {
                Enemy e =_door.ReleaseNextEnemy();
                if (e != null) _enemies.Add(e);
                _nextEnemieElapsedTime = 0;
            }

            if (_enemieMoveElapsedTime > 200)
            {
                // Poruszaj przeciwnikami
                for (int i = 0; i < _enemies.Count; i++)
                {
                    // Sprawdź, czy się juz nie przesuwa 
                    if (_enemies[i].IsMoving) continue;

                    // Sprawdź przecięcia tego enemie z otaczajacymi go obiektami
                    List<Direction> avaliableDirections = new List<Direction>();
                    Rectangle upRectangle, downRectangle, rightRectangle, leftRectangle;
                    upRectangle = new Rectangle(_enemies[i].EnemyRectangle.X, _enemies[i].EnemyRectangle.Y - 42, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    rightRectangle = new Rectangle(_enemies[i].EnemyRectangle.X + 42, _enemies[i].EnemyRectangle.Y, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    downRectangle = new Rectangle(_enemies[i].EnemyRectangle.X, _enemies[i].EnemyRectangle.Y + 42, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    leftRectangle = new Rectangle(_enemies[i].EnemyRectangle.X - 42, _enemies[i].EnemyRectangle.Y, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    bool[] freeSpace = new bool[4];
                    // Przecięcia z ziemią
                    for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                    {
                        for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                        {
                            if (_grounds[x, y].GroundType != GroundType.Free) continue;

                            if (upRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[0] = true;
                            if (rightRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[1] = true;
                            if (downRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[2] = true;
                            if (leftRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[3] = true;
                        }
                    }
                    // Up
                    if (_gameField.Contains(upRectangle) && freeSpace[0]) avaliableDirections.Add(Direction.Up);
                    // Right
                    if (_gameField.Contains(rightRectangle) && freeSpace[1]) avaliableDirections.Add(Direction.Right);
                    // Down
                    if (_gameField.Contains(downRectangle) && freeSpace[2]) avaliableDirections.Add(Direction.Down);
                    // Left
                    if (_gameField.Contains(leftRectangle) && freeSpace[3]) avaliableDirections.Add(Direction.Left);
                    // MakeMove
                    _enemies[i].MakeMove(_enemies[i], avaliableDirections.ToArray());
                }
                _enemieMoveElapsedTime = 0;
            }
            // Aktualizuj istniejąych przeciwników
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Move();
                _enemies[i].Update(gameTime);

                // Przecięcie enemie z robaczkiem
                // TODO: tu będzie metoda obserwe
                if (_worm.WormRectangle.Intersects(_enemies[i].EnemyRectangle) && !_enemies[i].IsKilled)
                {
                    _enemies[i].Attack(_worm);
                    _enemies[i].IsKilled = true;
                }
            }

            // Akutalizacja strzałów
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Update(gameTime);
                _shots[i].Move();
            }

            // Przesuwanie robaczka
            _worm.Move();

            // Uaktualnienie elementów interfejsu użytkownika
            _interfaceFruitsCounts[0].Text = _worm.AcidShoots.ToString();
            _interfaceFruitsCounts[1].Text = _worm.VenomShoots.ToString();
            _interfaceFruitsCounts[2].Text = _worm.PlumsCount.ToString();
            _interfaceFruitsCounts[3].Text = _worm.KiwisCount.ToString();
            _interfaceFruitsCounts[4].Text = _worm.MudCount.ToString();
            _interfaceLifeCount.Text = _worm.Life.ToString();

            // Obsługa usuwania elementów
            // Usuwanie użytych owoców
            for (int i = 0; i < _grabbableFruits.Count; i++)
            {
                if (_grabbableFruits[i].IsUsed) _grabbableFruits.RemoveAt(i);
            }
            // Usuwanie zebranyc czerwonych owoców
            for (int i = 0; i < _redFruits.Count; i++)
            {
                if (_redFruits[i].IsUsed) _redFruits.RemoveAt(i);
            }
            // Usuwanie zabitych wrogów
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].IsKilled) _enemies.RemoveAt(i);
            }
        }
    }
}