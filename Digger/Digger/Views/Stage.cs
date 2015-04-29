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
        private int _totalEnemiesCount;
        private int _totalKilledEnemiesCount;
        private List<Fruit> _redFruits;
        private List<Fruit> _grabbableFruits;
        private List<Fruit> _rootenKiwis;
        private Door _door;
        private List<Shot> _shots;
        private List<Shot> _webShots; 
        private List<Stone> _stones;
        private List<Purse> _purses;

        private int _redFruitsCount;

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
            _redFruitsCount = 10;
            _redFruits = _stageHelper.GenerateRedFruits(_redFruitsCount);
            _grabbableFruits = new List<Fruit>();
            _rootenKiwis = new List<Fruit>();

            // Drzwi
            _totalEnemiesCount = 8;
            _totalKilledEnemiesCount = 0;
            _door = new Door(_stageHelper.GenerateEnemies(2,2,1,3), 8000);

            // Wrogowie 
            _enemies = new List<Enemy>();

            // Strzały
            _shots = new List<Shot>();
            _webShots = new List<Shot>();

            // Skały
            _stones = _stageHelper.GenerateStones(10);

            // Sakiewki
            _purses = _stageHelper.GeneratePurses(10);

            // Interfejs użytkownika
            _interfaceFruits = _stageHelper.GenerateInterfaceFruits();
            _interfaceFruitsXs = new Label[6];
            for (int i = 0; i < _interfaceFruitsXs.Length; i++)
            {
                _interfaceFruitsXs[i] = new Label("x");
            }
            _interfaceFruitsCounts = new Label[6];
            _interfaceFruitsCounts[0] = new Label(_worm.AcidShoots.ToString());
            _interfaceFruitsCounts[1] = new Label(_worm.VenomShoots.ToString());
            _interfaceFruitsCounts[2] = new Label(_worm.PlumsCount.ToString());
            _interfaceFruitsCounts[3] = new Label(_worm.KiwisCount.ToString());
            _interfaceFruitsCounts[4] = new Label(_worm.MudCount.ToString());
            _interfaceFruitsCounts[5] = new Label(_worm.CandyCount.ToString());

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

            // Skały
            for (int i = 0; i < _stones.Count; i++)
            {
                _stones[i].LoadContent(content, "Game/Stone");
            }
            
            // Sakiewki
            for (int i = 0; i < _purses.Count; i++)
            {
                _purses[i].LoadContent(content, "Game/Purse");
            }

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
            for (int i = 0; i < _stones.Count; i++)
            {
                x = random.Next(0, (int) Math.Sqrt(_grounds.Length))*42 + horizontalShift; 
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + verticalShift;
                // Randomowe koordynaty
                // TODO: Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                _stones[i].Initialize(new Rectangle(x, y, 40, 40));
            }
            for (int i = 0; i < _purses.Count; i++)
            {
                x = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + horizontalShift;
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + verticalShift;
                // Randomowe koordynaty
                // TODO: Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                _purses[i].Initialize(new Rectangle(x, y, 40, 40));
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
                _interfaceFruitsXs[i].Initialize(new Vector2(x+60, y));
                _interfaceFruitsCounts[i].Initialize(new Vector2(x + 100, y));
                _interfaceFruits[i].Initialize(new Rectangle(x, y-27, 40, 40));
                y += 50;
            }

            y = screenHeight / 2;
            _interfaceLife.Initialize(new Vector2(x, y - 210), Color.White);
            _interfaceLifeX.Initialize(new Vector2(x+60, y - 200));
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

            // Kamienie
            for (int i = 0; i < _stones.Count; i++)
            {
                _stones[i].Draw(spriteBatch);
            }

            // Sakiewki
            for (int i = 0; i < _purses.Count; i++)
            {
                _purses[i].Draw(spriteBatch);
            }

            // Rysuj strzały
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Draw(spriteBatch);
            }
            for (int i = 0; i < _webShots.Count; i++)
            {
                _webShots[i].Draw(spriteBatch);
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
                    if (!_lastPressedKeys.Contains(Keys.D5) && keys[i] == Keys.D5 && _worm.MudCount > 0)
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
                    if (!_lastPressedKeys.Contains(Keys.D1) && keys[i] == Keys.D1 && _worm.AcidShoots > 0)
                    {
                        Shot shot = new Shot(_stageHelper.ShotTemplates[ShotType.Acid]);
                        shot.Initialize(_worm.WormRectangle, _worm.Direction);
                        _shots.Add(shot);
                        _worm.AcidShoots--;
                        continue;
                    }
                    // Strzelanie Venom
                    if (!_lastPressedKeys.Contains(Keys.D2) && keys[i] == Keys.D2 && _worm.VenomShoots > 0)
                    {
                        Shot shot = new Shot(_stageHelper.ShotTemplates[ShotType.Venom]);
                        shot.Initialize(_worm.WormRectangle, _worm.Direction);
                        _shots.Add(shot);
                        _worm.VenomShoots--;
                        continue;
                    }
                    // Wykorzystanie śliwki
                    if (!_lastPressedKeys.Contains(Keys.D3) && keys[i] == Keys.D3 && _worm.PlumsCount > 0)
                    {
                        _worm.MoveFaster(6, 5000);
                        continue;
                    }
                    // Zastawienie kiwi
                    if (!_lastPressedKeys.Contains(Keys.D4) && keys[i] == Keys.D4 && _worm.KiwisCount > 0)
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
                    if (!_lastPressedKeys.Contains(Keys.D6) && keys[i] == Keys.D6 && _worm.CandyCount > 0)
                    {
                        _worm.CandyCount--;
                        _worm.Heal();
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
                    if (_grounds[x, y].GroundType == GroundType.Free) continue;

                    if (_worm.WormRectangle.Intersects(_grounds[x, y].Rectangle))
                    {
                        // Rozpocznij proces zwolnienia
                        _worm.IsDigging = true;
                        // Usun ziemię
                        _grounds[x, y].GroundType = GroundType.Free;
                    }

                    // Czy strzał osiągnął ziemię
                    for (int i = 0; i < _shots.Count; i++)
                    {
                        if (_shots[i].ShotRectangle.Intersects(_grounds[x, y].Rectangle))
                        {
                            _shots[i].ShootSomething = true;
                        }
                    }

                    // Czy strzał tego ego no weba osiągnął ziemię
                    for (int i = 0; i < _webShots.Count; i++)
                    {
                        if (_webShots[i].ShotRectangle.Intersects(_grounds[x, y].Rectangle))
                        {
                            _webShots[i].ShootSomething = true;
                        }
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

                    if (_enemies[i].Observe != null) _enemies[i].Observe(_enemies[i], _worm, _grounds);
                    if (!_enemies[i].SawWorm) _enemies[i].TestMove(_enemies[i], _grounds, _gameField);
                    if (_enemies[i].WebShoot != null)
                    {
                        Shot s = _enemies[i].WebShoot(_enemies[i], _worm, _grounds);
                        if (s != null) _webShots.Add(s);
                    }

                    Rectangle upRectangle, downRectangle, rightRectangle, leftRectangle;
                    upRectangle = new Rectangle(_enemies[i].EnemyRectangle.X, _enemies[i].EnemyRectangle.Y - 42, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    rightRectangle = new Rectangle(_enemies[i].EnemyRectangle.X + 42, _enemies[i].EnemyRectangle.Y, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    downRectangle = new Rectangle(_enemies[i].EnemyRectangle.X, _enemies[i].EnemyRectangle.Y + 42, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);
                    leftRectangle = new Rectangle(_enemies[i].EnemyRectangle.X - 42, _enemies[i].EnemyRectangle.Y, _enemies[i].EnemyRectangle.Width, _enemies[i].EnemyRectangle.Height);

                    bool[] wormInRange = new bool[4];
                    if (upRectangle.Intersects(_worm.WormRectangle)) wormInRange[0] = true;
                    if (rightRectangle.Intersects(_worm.WormRectangle)) wormInRange[1] = true;
                    if (downRectangle.Intersects(_worm.WormRectangle)) wormInRange[2] = true;
                    if (leftRectangle.Intersects(_worm.WormRectangle)) wormInRange[3] = true;
                    for (int j = 0; j < wormInRange.Length; j++)
                    {
                        if (!_enemies[i].IsFreeze && wormInRange[j] && !_enemies[i].IsKilled)
                        {
                            _enemies[i].Attack(_worm);
                            _enemies[i].Freeze(2000);
                        }
                    }

                    for (int j = 0; j < _grabbableFruits.Count; j++)
                    {
                        if (_enemies[i].EnemyRectangle.Intersects(_grabbableFruits[j].FruitRectangle) && _grabbableFruits[j].IsUsed == false)
                        {
                            Enemy e = _grabbableFruits[j].EnemyUse(_enemies[i]);

                            if (e == null) continue;

                            if (!e.AddAsNew)
                            {
                                _enemies[i] = e;
                                continue;
                            }

                            if (e.EnemyType == EnemyType.Rat)
                            {
                                _door.ReleaseRatWhenOpen(e);
                                continue;
                            }

                            _enemies.Add(_door.ReleaseNewEnemyNow(e));
                            _totalEnemiesCount++;

                            _grabbableFruits[j].IsUsed = true;
                        }
                    }
                }
                _enemieMoveElapsedTime = 0;
            }
            // Aktualizuj istniejąych przeciwników
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Move();
                _enemies[i].Update(gameTime);

                for (int j = 0; j < _shots.Count; j++)
                {
                    if (_shots[j].ShootSomething) 
                        continue;
                    if (_shots[j].ShotRectangle.Intersects(_enemies[i].EnemyRectangle))
                    {
                        _shots[j].ShootEnemy(_enemies[i]);
                        _shots[j].ShootSomething = true;
                    }
                }

                for (int j = 0; j < _rootenKiwis.Count; j++)
                {
                    if (_rootenKiwis[j].FruitRectangle.Intersects(_enemies[i].EnemyRectangle))
                    {
                        _rootenKiwis[j].EnemyUse(_enemies[i]);
                        _rootenKiwis[j].IsUsed = true;
                    }
                }
            }

            // Akutalizacja strzałów
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Update(gameTime);
                _shots[i].Move();
            }
            for (int i = 0; i < _webShots.Count; i++)
            {
                _webShots[i].Update(gameTime);
                _webShots[i].Move();

                if (_webShots[i].ShotRectangle.Intersects(_worm.WormRectangle) && !_webShots[i].ShootSomething)
                {
                    _webShots[i].ShootSomething = true;
                    _worm.MoveSlower(1,5000);
                }

                // Zderzenie dwóch shots
                for (int j = 0; j < _shots.Count; j++)
                {
                    if (_webShots[i].ShotRectangle.Intersects(_shots[j].ShotRectangle) && !_webShots[i].ShootSomething && !_shots[j].ShootSomething)
                    {
                        _webShots[i].ShootSomething = true;
                        _shots[j].ShootSomething = true;
                    }
                }
            }

            // Aktualizacja kamieni
            for (int i = 0; i < _stones.Count; i++)
            {
                _stones[i].Move();
                _stones[i].Update(gameTime);

                Rectangle downRectangle = new Rectangle(_stones[i].StoneRectangle.X, _stones[i].StoneRectangle.Y + 42, _stones[i].StoneRectangle.Width, _stones[i].StoneRectangle.Height);

                if (_stones[i].IsMoving)
                {
                    // Sprawdź przecięcia z worgami
                    for (int j = 0; j < _enemies.Count; j++)
                    {
                        if (_stones[i].StoneRectangle.Intersects(_enemies[j].EnemyRectangle))
                        {
                            _enemies[j].Kill();
                            _stones[i].Shatter();
                        }
                    }

                    continue;
                }

                // Sprawdź, czy spadał a następnie osiągnęła koniec planszy
                if (!_gameField.Contains(downRectangle) && _stones[i].WasFalling)
                {
                    _stones[i].Shatter();
                    continue;
                } 

                Rectangle upRectangle, rightRectangle, leftRectangle;
                upRectangle = new Rectangle(_stones[i].StoneRectangle.X, _stones[i].StoneRectangle.Y - 42, _stones[i].StoneRectangle.Width, _stones[i].StoneRectangle.Height);
                rightRectangle = new Rectangle(_stones[i].StoneRectangle.X + 42, _stones[i].StoneRectangle.Y, _stones[i].StoneRectangle.Width, _stones[i].StoneRectangle.Height);
                leftRectangle = new Rectangle(_stones[i].StoneRectangle.X - 42, _stones[i].StoneRectangle.Y, _stones[i].StoneRectangle.Width, _stones[i].StoneRectangle.Height);
                bool[] freeSpace = new bool[4];
                // Przecięcia z ziemią
                for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                {
                    for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                    {
                        if (_grounds[x, y].GroundType != GroundType.Free)
                        {
                            if (downRectangle.Intersects(_grounds[x, y].Rectangle) && _stones[i].WasFalling) _stones[i].Shatter();
                            continue;
                        }

                        if (upRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[0] = true;
                        if (rightRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[1] = true;
                        if (downRectangle.Intersects(_grounds[x, y].Rectangle) && _gameField.Contains(downRectangle)) freeSpace[2] = true;
                        if (leftRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[3] = true;
                    }
                }
                bool[] ocupatedByStone = new bool[4];
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (upRectangle.Intersects(_stones[j].StoneRectangle)) ocupatedByStone[0] = true;
                    if (rightRectangle.Intersects(_stones[j].StoneRectangle)) ocupatedByStone[1] = true;
                    if (downRectangle.Intersects(_stones[j].StoneRectangle)) ocupatedByStone[2] = true;
                    if (leftRectangle.Intersects(_stones[j].StoneRectangle)) ocupatedByStone[3] = true;
                }

                // Pytanie, czy robaczek nie jest pod spodem
                if (downRectangle.Intersects(_worm.WormRectangle)) freeSpace[2] = false;

                if (_worm.WormRectangle.Intersects(_stones[i].StoneRectangle) && _worm.Direction == Direction.Right && _gameField.Contains(rightRectangle) && !ocupatedByStone[1])
                {
                    // Można przesunąć w prawo
                    _stones[i].MakeMove(Direction.Right);
                    continue;
                }

                if (_worm.WormRectangle.Intersects(_stones[i].StoneRectangle) && _worm.Direction == Direction.Left && _gameField.Contains(leftRectangle) && !ocupatedByStone[3])
                {
                    // Można przesunąć w lewo
                    _stones[i].MakeMove(Direction.Left);
                    continue;
                }

                if (freeSpace[2] && !ocupatedByStone[2]) _stones[i].MakeMove(Direction.Down);
            }

            // Akutalizacja sakiewek
            for (int i = 0; i < _purses.Count; i++)
            {
                _purses[i].Move();
                _purses[i].Update(gameTime);

                Rectangle downRectangle = new Rectangle(_purses[i].PurseRectangle.X, _purses[i].PurseRectangle.Y + 42, _purses[i].PurseRectangle.Width, _purses[i].PurseRectangle.Height);

                if (_purses[i].IsMoving)
                {
                    // Sprawdź przecięcia z worgami
                    for (int j = 0; j < _enemies.Count; j++)
                    {
                        if (_purses[i].PurseRectangle.Intersects(_enemies[j].EnemyRectangle))
                        {
                            _enemies[j].Kill();
                            Fruit fruit = _purses[i].Shatter();
                            if (fruit != null) _grabbableFruits.Add(fruit);
                        }
                    }   
                    continue;
                }

                // Sprawdź, czy spadała a następnie osiągnęła koniec planszy
                if (!_gameField.Contains(downRectangle) && _purses[i].WasFalling)
                {
                    Fruit fruit = _purses[i].Shatter();
                    if (fruit != null) _grabbableFruits.Add(fruit);
                    continue;
                } 

                Rectangle upRectangle, rightRectangle, leftRectangle;
                upRectangle = new Rectangle(_purses[i].PurseRectangle.X, _purses[i].PurseRectangle.Y - 42, _purses[i].PurseRectangle.Width, _purses[i].PurseRectangle.Height);
                rightRectangle = new Rectangle(_purses[i].PurseRectangle.X + 42, _purses[i].PurseRectangle.Y, _purses[i].PurseRectangle.Width, _purses[i].PurseRectangle.Height);
                leftRectangle = new Rectangle(_purses[i].PurseRectangle.X - 42, _purses[i].PurseRectangle.Y, _purses[i].PurseRectangle.Width, _purses[i].PurseRectangle.Height);
                bool[] freeSpace = new bool[4];
                // Przecięcia z ziemią
                for (int x = 0; x < Math.Sqrt(_grounds.Length); x++)
                {
                    for (int y = 0; y < Math.Sqrt(_grounds.Length); y++)
                    {
                        if (_grounds[x, y].GroundType != GroundType.Free)
                        {
                            if (downRectangle.Intersects(_grounds[x, y].Rectangle) && _purses[i].WasFalling)
                            {
                                Fruit fruit = _purses[i].Shatter();
                                if (fruit != null) _grabbableFruits.Add(fruit);
                            }
                            continue;
                        }

                        if (upRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[0] = true;
                        if (rightRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[1] = true;
                        if (downRectangle.Intersects(_grounds[x, y].Rectangle) && _gameField.Contains(downRectangle)) freeSpace[2] = true;
                        if (leftRectangle.Intersects(_grounds[x, y].Rectangle)) freeSpace[3] = true;
                    }
                }
                bool[] ocupatedByPurse = new bool[4];
                for (int j = 0; j < _purses.Count; j++)
                {
                    if (upRectangle.Intersects(_purses[j].PurseRectangle)) ocupatedByPurse[0] = true;
                    if (rightRectangle.Intersects(_purses[j].PurseRectangle)) ocupatedByPurse[1] = true;
                    if (downRectangle.Intersects(_purses[j].PurseRectangle)) ocupatedByPurse[2] = true;
                    if (leftRectangle.Intersects(_purses[j].PurseRectangle)) ocupatedByPurse[3] = true;
                }

                // Pytanie, czy robaczek nie jest pod spodem
                if (downRectangle.Intersects(_worm.WormRectangle)) freeSpace[2] = false;

                if (_worm.WormRectangle.Intersects(_purses[i].PurseRectangle) && _worm.Direction == Direction.Right && _gameField.Contains(rightRectangle) && !ocupatedByPurse[1])
                {
                    // Można przesunąć w prawo
                    _purses[i].MakeMove(Direction.Right);
                    continue;
                }

                if (_worm.WormRectangle.Intersects(_purses[i].PurseRectangle) && _worm.Direction == Direction.Left && _gameField.Contains(leftRectangle) && !ocupatedByPurse[3])
                {
                    // Można przesunąć w lewo
                    _purses[i].MakeMove(Direction.Left);
                    continue;
                }

                if (freeSpace[2] && !ocupatedByPurse[2]) _purses[i].MakeMove(Direction.Down);
            }

            // Przesuwanie robaczka
            _worm.Move();

            // Sprawdź, czy gracz juz nie przeszedl gry
            if (_worm.RedFruits == _redFruitsCount || _totalEnemiesCount == _totalKilledEnemiesCount)
            {
                Enemy e = _door.OpenDoor();
                // Ew. dodaj szczura
                if (e != null) _enemies.Add(e);
            }

            // Czy gracz zeczywiscie przeszedl gre
            if (_door.AreOpen && _worm.WormRectangle.Intersects(_door.DoorRectangle))
            {
                // TODO: wyświetl inofmracje o wygrnaej
            }

            // Czy gracz przegrał
            if (_worm.Life == 0)
            {
                // TODO: Gracz przegral
            }

            // Uaktualnienie elementów interfejsu użytkownika
            _interfaceFruitsCounts[0].Text = _worm.AcidShoots.ToString();
            _interfaceFruitsCounts[1].Text = _worm.VenomShoots.ToString();
            _interfaceFruitsCounts[2].Text = _worm.PlumsCount.ToString();
            _interfaceFruitsCounts[3].Text = _worm.KiwisCount.ToString();
            _interfaceFruitsCounts[4].Text = _worm.MudCount.ToString();
            _interfaceFruitsCounts[5].Text = _worm.CandyCount.ToString();
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
                if (_enemies[i].IsKilled)
                {
                    _enemies.RemoveAt(i);
                    _totalKilledEnemiesCount++;
                }
            }
            // Usuwanie strzalow, ktore cos osiagnely
            for (int i = 0; i < _shots.Count; i++)
            {
                if (_shots[i].ShootSomething || !_gameField.Contains(_shots[i].ShotRectangle)) _shots.RemoveAt(i);
            }
            for (int i = 0; i < _webShots.Count; i++)
            {
                if (_webShots[i].ShootSomething || !_gameField.Contains(_webShots[i].ShotRectangle)) _webShots.RemoveAt(i);
            }
            // Usuwanie kiwi
            for (int i = 0; i < _rootenKiwis.Count; i++)
            {
                if (_rootenKiwis[i].IsUsed) _rootenKiwis.RemoveAt(i);
            }
            // Usuwanie sakiewek 
            for (int i = 0; i < _purses.Count; i++)
            {
                if (_purses[i].IsShatter) _purses.RemoveAt(i);
            }
            for (int i = 0; i < _stones.Count; i++)
            {
                if (_stones[i].IsShatter) _stones.RemoveAt(i);
            }
        }
    }
}