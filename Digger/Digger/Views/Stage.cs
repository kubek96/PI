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
using Microsoft.Xna.Framework.Media;

namespace Digger.Views
{
    public class Stage : IXnaUseable
    {
        private static StageHelper _stageHelper = new StageHelper();
        private static int _redFruitsCount = 10;

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
        private int _level;

        #region Interfejs
        private Fruit[] _interfaceFruits;
        private Label[] _interfaceFruitsXs;
        private Label[] _interfaceFruitsCounts;
        
        private FixedGraphic _interfaceLife;
        private Label _interfaceLifeX;
        private Label _interfaceLifeCount;

        private Fruit _interfaceRedFruit;
        private Label _interfaceRedFruitXs;
        private Label _interfaceRedFruitCounts;

        private Label _interfaceLevel;
        private Label _interfaceLevelNumber;

        private Label _interfacePoints;
        private Label _interfacePointsCount;
        #endregion

        private Song _song;

        // Pause
        private bool _isGamePaused;
        private Pause _pauseView;
        private Options _optionsView;
        private InGameHelp _helpView;

        // Win
        private Win _winView;
        // Lose
        private Lose _loseView;

        public Stage(int level)
        {
            _level = level;

            #region Widoki
            _pauseView = new Pause();
            _isGamePaused = false;

            _optionsView = new Options();
            _helpView = new InGameHelp();

            // Win
            _winView = new Win(_level + 1);

            // Lose
            _loseView = new Lose(_level);
            #endregion

            #region Logika gry
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
            _redFruits = _stageHelper.GenerateRedFruits(_redFruitsCount);
            _grabbableFruits = new List<Fruit>();
            _rootenKiwis = new List<Fruit>();

            // Drzwi
            _totalEnemiesCount = 0;
            int[] enemiesCount = new int[5];
            for (int i = 0; i < 5; i++) enemiesCount[i] = 0;
            //for (int i = 0; i < _stageHelper.EnemiesLevelCount[_level].Count; i++)
            foreach (var pair in _stageHelper.EnemiesLevelCount[_level])
            {
                _totalEnemiesCount += pair.Value;
                enemiesCount[(int) pair.Key] = pair.Value;
                //_totalEnemiesCount += _stageHelper.EnemiesLevelCount[_level][(EnemyType) i];
                //enemiesCount[i] = _stageHelper.EnemiesLevelCount[_level][(EnemyType)i];
            }
            _door = new Door(_stageHelper.GenerateEnemies(enemiesCount[0], enemiesCount[1], enemiesCount[2], enemiesCount[3], enemiesCount[4]), 8000);
            _totalKilledEnemiesCount = 0;

            // Wrogowie 
            _enemies = new List<Enemy>();

            // Strzały
            _shots = new List<Shot>();
            _webShots = new List<Shot>();

            // Skały
            _stones = _stageHelper.GenerateStones(10);

            // Sakiewki
            _purses = _stageHelper.GeneratePurses(10);

            #endregion

            #region Interfejs użytkownika
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

            _interfaceRedFruit = new Fruit(_stageHelper.FruitsTemplates[FruitType.RedFruit]);
            _interfaceRedFruitXs = new Label("x");
            _interfaceRedFruitCounts = new Label(_worm.RedFruits.ToString());

            _interfaceLevel = new Label("Level no.");
            _interfaceLevelNumber = new Label(_level.ToString());

            _interfacePoints = new Label("Points: ");
            _interfacePointsCount = new Label(Game1.Context.Player.Points.ToString());

            #endregion

            // Wczytaj zawartość
            LoadContent(Game1.Context.Content);

            // Zainicjalizuj
            Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            #region Muzyka
            _song = content.Load<Song>("Sounds/Sound1");
            #endregion

            #region Elementy gry

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

            #endregion

            #region Elementy interfejsu użytkownika
            for (int i = 0; i < _interfaceFruitsXs.Length; i++)
            {
                _interfaceFruitsXs[i].LoadContent(content, "Fonts/Silkscreen");
                _interfaceFruitsCounts[i].LoadContent(content, "Fonts/Silkscreen");
            }

            _interfaceLife.LoadContent(content, "Game/Interface/Heart");
            _interfaceLifeX.LoadContent(content, "Fonts/Silkscreen");
            _interfaceLifeCount.LoadContent(content, "Fonts/Silkscreen");

            _interfaceRedFruitXs.LoadContent(content, "Fonts/Silkscreen");
            _interfaceRedFruitCounts.LoadContent(content, "Fonts/Silkscreen");

            _interfaceLevel.LoadContent(content, "Fonts/Silkscreen");
            _interfaceLevelNumber.LoadContent(content, "Fonts/Silkscreen");

            _interfacePoints.LoadContent(content, "Fonts/Silkscreen");
            _interfacePointsCount.LoadContent(content, "Fonts/Silkscreen");

            #endregion
        }

        public void Initialize()
        {
            #region Elementy gry

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

            Point[] freeGrounds = _stageHelper.GenerateFreeGroundsCoordinates(3+_level);
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
                bool c = false;
                // Randomowe koordynaty
                x = random.Next(0, (int) Math.Sqrt(_grounds.Length))*42 + horizontalShift; 
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)-1) * 42 + verticalShift;
                // Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                for (int j = 0; j < i; j++)
                {
                    if (_stones[j].StoneRectangle.X == x && _stones[j].StoneRectangle.Y == y)
                    {
                        i--;
                        c = true;
                        break;
                    }
                }
                for (int j = 0; j < _grounds.GetLength(0); j++)
                {
                    if (c) break;
                    for (int k = 0; k < _grounds.GetLength(1); k++)
                    {
                        if (_grounds[j,k].GroundType != GroundType.Free) continue;
                        if (_grounds[j, k].Rectangle.X == x && _grounds[j, k].Rectangle.Y == y)
                        {
                            i--;
                            c = true;
                            break;
                        }
                    }
                }
                if (c) continue;
                _stones[i].Initialize(new Rectangle(x, y, 40, 40));
            }
            for (int i = 0; i < _purses.Count; i++)
            {
                bool c = false;
                x = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + horizontalShift;
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)-1) * 42 + verticalShift;
                // Randomowe koordynaty
                // Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                for (int j = 0; j < i; j++)
                {
                    if ((_purses[j].PurseRectangle.X == x && _purses[j].PurseRectangle.Y == y))
                    {
                        i--;
                        c = true;
                        break;
                    }
                }
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (_stones[j].StoneRectangle.X == x && _stones[j].StoneRectangle.Y == y)
                    {
                        i--;
                        c = true;
                        break;
                    }
                }
                for (int j = 0; j < _grounds.GetLength(0); j++)
                {
                    if (c) break;
                    for (int k = 0; k < _grounds.GetLength(1); k++)
                    {
                        if (_grounds[j, k].GroundType != GroundType.Free) continue;
                        if (_grounds[j, k].Rectangle.X == x && _grounds[j, k].Rectangle.Y == y)
                        {
                            i--;
                            c = true;
                            break;
                        }
                    }
                }
                if (c) continue;
                _purses[i].Initialize(new Rectangle(x, y, 40, 40));
            }
            for (int i = 0; i < _redFruits.Count; i++)
            {
                bool c = false;
                x = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + horizontalShift;
                y = random.Next(0, (int)Math.Sqrt(_grounds.Length)) * 42 + verticalShift;
                // Randomowe koordynaty
                // Zrób tak, żeby na siebie nie nachodziły (sprawdź, czy już nie została ta pozycja wylosowana)
                for (int j = 0; j < _purses.Count; j++)
                {
                    if ((_purses[j].PurseRectangle.X == x && _purses[j].PurseRectangle.Y == y))
                    {
                        i--;
                        c = true;
                        break;
                    }
                }
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (_stones[j].StoneRectangle.X == x && _stones[j].StoneRectangle.Y == y)
                    {
                        i--;
                        c = true;
                        break;
                    }
                }
                if (c) continue;
                _redFruits[i].Initialize(new Rectangle(x, y, 40, 40));
            }

            #endregion

            #region Inicjalizuj obiekty interfesju użytkownika
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

            _interfaceRedFruit.Initialize(new Rectangle(x, y-210, 40,40));
            _interfaceRedFruitXs.Initialize(new Vector2(x+60,y-200));
            _interfaceRedFruitCounts.Initialize(new Vector2(x+100,y-200));

            _interfaceLevel.Initialize(new Vector2(x, 200));
            _interfaceLevelNumber.Initialize(new Vector2(x+100,200));

            _interfacePoints.Initialize(new Vector2(x, 100));
            _interfacePointsCount.Initialize(new Vector2(x+100,100));

            #endregion

            #region Music
            if (Game1.Context.Player.IsMusicOn) MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = true;
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            #region Elementy gry

            _background.Draw(spriteBatch);

            for (int i = 0; i < Math.Sqrt(_grounds.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(_grounds.Length); j++)
                {
                    _grounds[i, j].Draw(spriteBatch);
                }
            }

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

            _worm.Draw(spriteBatch);

            #endregion

            #region Elementy interfejsu użytkownika
            for (int i = 0; i < _interfaceFruits.Length; i++)
            {
                _interfaceFruits[i].Draw(spriteBatch);
                _interfaceFruitsCounts[i].Draw(spriteBatch);
                _interfaceFruitsXs[i].Draw(spriteBatch);
            }
            _interfaceLife.Draw(spriteBatch);
            _interfaceLifeX.Draw(spriteBatch);
            _interfaceLifeCount.Draw(spriteBatch);

            _interfaceRedFruit.Draw(spriteBatch);
            _interfaceRedFruitXs.Draw(spriteBatch);
            _interfaceRedFruitCounts.Draw(spriteBatch);

            _interfaceLevel.Draw(spriteBatch);
            _interfaceLevelNumber.Draw(spriteBatch);

            _interfacePoints.Draw(spriteBatch);
            _interfacePointsCount.Draw(spriteBatch);

            #endregion

            #region Pause
            if (_isGamePaused)
            {
                if (_pauseView.ShowOptions)
                {
                    _optionsView.Draw(spriteBatch, gameTime);
                    return;
                }
                if (_pauseView.ShowHelp)
                {
                    _helpView.Draw(spriteBatch, gameTime);
                    return;
                }
                _pauseView.Draw(spriteBatch, gameTime);
                return;
            }
            #endregion

            #region Win

            if (_winView.IsVisible)
            {
                _winView.Draw(spriteBatch,gameTime);
                return;
            }

            #endregion

            #region Lose

            if (_loseView.IsVisible)
            {
                _loseView.Draw(spriteBatch,gameTime);
            }

            #endregion
        }

        public void Update(GameTime gameTime)
        {
            #region Update

            _worm.Update(gameTime);
            _door.Update(gameTime);

            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Update(gameTime);
            }
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Update(gameTime);
            }
            for (int i = 0; i < _webShots.Count; i++)
            {
                _webShots[i].Update(gameTime);
            }
            for (int i = 0; i < _stones.Count; i++)
            {
                _stones[i].Update(gameTime);
            }
            for (int i = 0; i < _purses.Count; i++)
            {
                _purses[i].Update(gameTime);
            }

            #endregion

            #region Music
            if (!Game1.Context.Player.IsMusicOn) MediaPlayer.Stop();
            else
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(_song);
                } 
            }
            #endregion

            KeyboardState newKeyboardState = Keyboard.GetState();
            Keys[] keys = newKeyboardState.GetPressedKeys();

            #region Pause
            // Czy wciśnięto spacje lub p lub esc
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] != Keys.Space && keys[i] != Keys.P && keys[i] != Keys.Escape) continue;

                _isGamePaused = true;
                _pauseView.Close = false;
            }

            // Czy pause
            if (_isGamePaused)
            {
                if (_pauseView.ShowOptions)
                {
                    _optionsView.Update(gameTime);
                    if (_optionsView.Close)
                    {
                        _optionsView.Close = false;
                        _pauseView.ElapsedGoBackTime = 0;
                        _pauseView.ShowOptions = false;
                    }
                    return;
                }

                if (_pauseView.ShowHelp)
                {
                    _helpView.Update(gameTime);
                    if (_helpView.Close)
                    {
                        _helpView.Close = false;
                        _pauseView.ElapsedGoBackTime = 0;
                        _pauseView.ShowHelp = false;
                    }
                    return;
                }

                _pauseView.Update(gameTime);
                if (_pauseView.Close) _isGamePaused = false;
                return;
            }
            #endregion

            #region Win

            if (_winView.IsVisible)
            {
                _winView.Update(gameTime);
                return;
            }

            #endregion

            #region Lose

            if (_loseView.IsVisible)
            {
                _loseView.Update(gameTime);
                return;
            }

            #endregion

            #region Reakcja na wciskanie klawiszy przez użytkownika
            // Obsługuj ruch robaczka
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
                //if (!Game1.Context.Player.UserControls.ContainsKey(keys[i]))
                if (!Digger.Data.Control.Keyboard.Layout[Game1.Context.Player.UserKeyboraPreferences].ContainsKey(keys[i]))
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
                //if (!_gameField.Contains(_worm.TestMove(Game1.Context.Player.UserControls[keys[i]]))) 
                if (!_gameField.Contains(_worm.TestMove(Digger.Data.Control.Keyboard.Layout[Game1.Context.Player.UserKeyboraPreferences][keys[i]]))) 
                    continue;
                // Sprawdź czy robaczek nie w ruchu
                if (_worm.IsMoving) 
                    continue;

                _worm.MakeMove(Digger.Data.Control.Keyboard.Layout[Game1.Context.Player.UserKeyboraPreferences][keys[i]]);
               
                _elapsedTime = 0;
            }
            _lastPressedKeys = keys.ToArray();
            #endregion

            #region Wrom akcja kopania
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
            #endregion

            #region Przecięcia owoców z robaczkiem
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
                    Game1.Context.Player.Points++;
                }
            }
            #endregion

            #region Generowanie kolejnych przeciwników
            // Generuj kolejnych przeciwników
            if (_nextEnemieElapsedTime > _door.TimeToNextEnemie)
            {
                Enemy e =_door.ReleaseNextEnemy();
                if (e != null) _enemies.Add(e);
                _nextEnemieElapsedTime = 0;
            }
            #endregion

            #region Enemies poruszanie
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

                #region Przecięcia emeies z strzałami
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
                #endregion

                #region Przecięcia eniemies z zgniłymi owcami
                for (int j = 0; j < _rootenKiwis.Count; j++)
                {
                    if (_rootenKiwis[j].FruitRectangle.Intersects(_enemies[i].EnemyRectangle))
                    {
                        _rootenKiwis[j].EnemyUse(_enemies[i]);
                        _rootenKiwis[j].IsUsed = true;
                    }
                }
                #endregion
            }
            #endregion

            #region Obsługa ruchu strzałów użytkownika
            // Akutalizacja strzałów
            for (int i = 0; i < _shots.Count; i++)
            {
                _shots[i].Move();

                // Zderzenie z sakiewką
                for (int j = 0; j < _purses.Count; j++)
                {
                    if (_purses[j].IsShatter) continue;
                    if (_shots[i].ShotRectangle.Intersects(_purses[j].PurseRectangle))
                    {
                        Fruit f = _purses[j].Shatter();
                        if (f != null) _grabbableFruits.Add(f);
                    }
                }
                // Zderzenie z kamieniem
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (_stones[j].IsShatter) continue;
                    if (_shots[i].ShotRectangle.Intersects(_stones[j].StoneRectangle))
                    {
                        _stones[j].Shatter();
                    }
                }
            }
            #endregion

            #region Obsługa strzałów wystrzelonych przez enemies
            for (int i = 0; i < _webShots.Count; i++)
            {
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

                // Zderzenie z sakiewką
                for (int j = 0; j < _purses.Count; j++)
                {
                    if (_purses[j].IsShatter) continue;
                    if (_webShots[i].ShotRectangle.Intersects(_purses[j].PurseRectangle))
                    {
                        Fruit f = _purses[j].Shatter();
                        if (f != null) _grabbableFruits.Add(f);
                    }
                }
                // Zderzenie z kamieniem
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (_stones[j].IsShatter) continue;
                    if (_webShots[i].ShotRectangle.Intersects(_stones[j].StoneRectangle))
                    {
                        _stones[j].Shatter();
                    }
                }
            }
            #endregion

            #region Kamienie
            // Aktualizacja kamieni
            for (int i = 0; i < _stones.Count; i++)
            {
                _stones[i].Move();

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
                bool[] ocupated = new bool[4];
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (upRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[0] = true;
                    if (rightRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[1] = true;
                    if (downRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[2] = true;
                    if (leftRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[3] = true;
                }
                // Czy nie zawadza mu sakiewka?
                for (int j = 0; j < _purses.Count; j++)
                {
                    if (upRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[0] = true;
                    if (rightRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[1] = true;
                    if (downRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[2] = true;
                    if (leftRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[3] = true;
                }

                // Pytanie, czy robaczek nie jest pod spodem
                if (downRectangle.Intersects(_worm.WormRectangle))
                {
                    // Pytanie czy spadało, czy robaczek dopiero co wykopał dziurę?
                    if (_stones[i].WasFalling)
                    {
                        // Spadało
                        _stones[i].Shatter();
                        _worm.Life--;
                        continue;
                    }
                    // Robaczek zrobił dziurę
                    freeSpace[2] = false;
                }

                if (_worm.WormRectangle.Intersects(_stones[i].StoneRectangle) && _worm.Direction == Direction.Right && _gameField.Contains(rightRectangle) && !ocupated[1])
                {
                    // Można przesunąć w prawo
                    _stones[i].MakeMove(Direction.Right);
                    continue;
                }

                if (_worm.WormRectangle.Intersects(_stones[i].StoneRectangle) && _worm.Direction == Direction.Left && _gameField.Contains(leftRectangle) && !ocupated[3])
                {
                    // Można przesunąć w lewo
                    _stones[i].MakeMove(Direction.Left);
                    continue;
                }

                if (freeSpace[2] && !ocupated[2]) _stones[i].MakeMove(Direction.Down);
            }

            #endregion

            #region Sakiewki
            // Akutalizacja sakiewek
            for (int i = 0; i < _purses.Count; i++)
            {
                _purses[i].Move();

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
                bool[] ocupated = new bool[4];
                for (int j = 0; j < _purses.Count; j++)
                {
                    if (upRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[0] = true;
                    if (rightRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[1] = true;
                    if (downRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[2] = true;
                    if (leftRectangle.Intersects(_purses[j].PurseRectangle)) ocupated[3] = true;
                }
                // Czy nie przeszkadza mu kamien?
                for (int j = 0; j < _stones.Count; j++)
                {
                    if (upRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[0] = true;
                    if (rightRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[1] = true;
                    if (downRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[2] = true;
                    if (leftRectangle.Intersects(_stones[j].StoneRectangle)) ocupated[3] = true;
                }

                // Pytanie, czy robaczek nie jest pod spodem
                if (downRectangle.Intersects(_worm.WormRectangle))
                {
                    // Pytanie czy spadało, czy robaczek dopiero co wykopał dziurę?
                    if (_purses[i].WasFalling)
                    {
                        // Spadało
                        Fruit f = _purses[i].Shatter();
                        if (f!=null) _grabbableFruits.Add(f);
                        _worm.Life--;
                        continue;
                    }
                    // Robaczek zrobił dziurę
                    freeSpace[2] = false;
                }

                if (_worm.WormRectangle.Intersects(_purses[i].PurseRectangle) && _worm.Direction == Direction.Right && _gameField.Contains(rightRectangle) && !ocupated[1])
                {
                    // Można przesunąć w prawo
                    _purses[i].MakeMove(Direction.Right);
                    continue;
                }

                if (_worm.WormRectangle.Intersects(_purses[i].PurseRectangle) && _worm.Direction == Direction.Left && _gameField.Contains(leftRectangle) && !ocupated[3])
                {
                    // Można przesunąć w lewo
                    _purses[i].MakeMove(Direction.Left);
                    continue;
                }

                if (freeSpace[2] && !ocupated[2]) _purses[i].MakeMove(Direction.Down);
            }
            #endregion

            #region Worm poruszanie
            // Przesuwanie robaczka
            _worm.Move();
            #endregion

            #region obsługa możliwości przejścia do kolejnej planszy
            // Sprawdź, czy gracz juz nie przeszedl gry
            if (_worm.RedFruits == _redFruitsCount || _totalEnemiesCount == _totalKilledEnemiesCount)
            {
                Enemy e = _door.OpenDoor();
                // Ew. dodaj szczura
                if (e != null) _enemies.Add(e);
            }
            #endregion

            #region Obsługa wygranej i przegranej
            // Czy gracz zeczywiscie przeszedl gre
            if (_door.AreOpen && _worm.WormRectangle.Intersects(_door.DoorRectangle))
            {
                // Wyświetl inofmracje o wygrnaej
                Game1.Context.Player.Level++;
                Game1.Context.SavePlayersToSerializedFile();
                _winView.IsVisible = true;
            }

            // Czy gracz przegrał
            if (_worm.Life == 0)
            {
                // Gracz przegral
                _loseView.IsVisible = true;
            }
            #endregion

            #region Interfejs użytkownika
            // Uaktualnienie elementów interfejsu użytkownika
            _interfaceFruitsCounts[0].Text = _worm.AcidShoots.ToString();
            _interfaceFruitsCounts[1].Text = _worm.VenomShoots.ToString();
            _interfaceFruitsCounts[2].Text = _worm.PlumsCount.ToString();
            _interfaceFruitsCounts[3].Text = _worm.KiwisCount.ToString();
            _interfaceFruitsCounts[4].Text = _worm.MudCount.ToString();
            _interfaceFruitsCounts[5].Text = _worm.CandyCount.ToString();
            _interfaceLifeCount.Text = _worm.Life.ToString();

            _interfaceRedFruitCounts.Text = _worm.RedFruits.ToString();
            _interfacePointsCount.Text = Game1.Context.Player.Points.ToString();

            #endregion

            #region Obsługa usuwania elementów
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
            #endregion
        }
    }
}