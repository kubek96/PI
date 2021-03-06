﻿using System;
using System.Collections.Generic;
using Digger.Game.Elements;
using Digger.Game.Specified;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Digger.Game.Common
{
    /// <summary>
    /// Statyczna klasa umożliwiająca szybkie kopiowanie obiektów owoców, wrogów, strzałów ze wzorców.
    /// Dodatkowo posiada słownik mówiący o ilości danych przeciwników na danym poziome.
    /// Dostarcza metod umożliwijących szybkie generowanie planszy.
    /// </summary>
    public class StageHelper
    {
        private Dictionary<FruitType, Fruit> _fruitsTemplates;
        private Dictionary<EnemyType, Enemy> _enemyTemplates;
        private Dictionary<ShotType, Shot> _shotTemplates;
        private Dictionary<int, Dictionary<EnemyType, int>> _enemiesLevelCount;

        /// <summary>
        /// Konstruktor inicjalicujący wszystkie słowniki.
        /// </summary>
        public StageHelper()
        {
            _fruitsTemplates = new Dictionary<FruitType, Fruit>();
            _fruitsTemplates.Add(FruitType.Lemon, new Fruit(FruitType.Lemon, "Game/Fruits/Lemon", worm => worm.AcidShoots += 5, delegate(Enemy enemy)
            {
                if (enemy.Evolve!= null) return enemy.Evolve(enemy);
                return null;
            }));
            _fruitsTemplates.Add(FruitType.Orange, new Fruit(FruitType.Orange, "Game/Fruits/Orange", worm => worm.VenomShoots += 5, _fruitsTemplates[FruitType.Lemon].EnemyUse));
            _fruitsTemplates.Add(FruitType.Kiwi, new Fruit(FruitType.Kiwi, "Game/Fruits/Kiwi", worm => worm.KiwisCount++,
                delegate(Enemy enemy)
                {
                    enemy.IsKilled = true;
                    return null;
                }));
            _fruitsTemplates.Add(FruitType.Watermelon, new Fruit(FruitType.Watermelon, "Game/Fruits/Watermelon", worm => worm.MudCount++, _fruitsTemplates[FruitType.Lemon].EnemyUse));
            _fruitsTemplates.Add(FruitType.Plum, new Fruit(FruitType.Plum, "Game/Fruits/Plum", worm => worm.PlumsCount++, _fruitsTemplates[FruitType.Lemon].EnemyUse));
            _fruitsTemplates.Add(FruitType.Candy, new Fruit(FruitType.Candy, "Game/Fruits/Candy", worm => worm.CandyCount++, delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.Rat]);
                    e.Initialize(enemy.Rectangle);
                    e.Direction = enemy.Direction;
                    e.AddAsNew = true;
                    return e;
                }));
            _fruitsTemplates.Add(FruitType.RedFruit, new Fruit(FruitType.RedFruit, "Game/Fruits/Red", worm => worm.RedFruits++, null));

            //---------
            _enemyTemplates = new Dictionary<EnemyType, Enemy>();
            _enemyTemplates.Add(EnemyType.Mouse, new Enemy(EnemyType.Mouse, "Game/Enemies/Mouse", 1, 1, 2, Direction.Left, false,
                delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.Spider]);
                    e.Initialize(enemy.Rectangle);
                    e.Direction = enemy.Direction;
                    return e;
                }, delegate(Enemy enemy, Ground[,] grounds, Rectangle gameField)
                {
                    List<Direction> avaliableDirections = new List<Direction>();
                    Rectangle upRectangle, downRectangle, rightRectangle, leftRectangle;
                    upRectangle = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y - 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    rightRectangle = new Rectangle(enemy.Rectangle.X + 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    downRectangle = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y + 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    leftRectangle = new Rectangle(enemy.Rectangle.X - 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    bool[] freeSpace = new bool[4];
                    // Przecięcia z ziemią
                    for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                    {
                        for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                        {
                            if (grounds[x, y].GroundType != GroundType.Free) continue;

                            if (upRectangle.Intersects(grounds[x, y].Rectangle)) freeSpace[0] = true;
                            if (rightRectangle.Intersects(grounds[x, y].Rectangle)) freeSpace[1] = true;
                            if (downRectangle.Intersects(grounds[x, y].Rectangle)) freeSpace[2] = true;
                            if (leftRectangle.Intersects(grounds[x, y].Rectangle)) freeSpace[3] = true;
                        }
                    }
                    // Up
                    if (gameField.Contains(upRectangle) && freeSpace[0]) avaliableDirections.Add(Direction.Up);
                    // Right
                    if (gameField.Contains(rightRectangle) && freeSpace[1]) avaliableDirections.Add(Direction.Right);
                    // Down
                    if (gameField.Contains(downRectangle) && freeSpace[2]) avaliableDirections.Add(Direction.Down);
                    // Left
                    if (gameField.Contains(leftRectangle) && freeSpace[3]) avaliableDirections.Add(Direction.Left);
                    // MakeMove
                    Random random = new Random();
                    enemy.MakeMove(avaliableDirections[random.Next(avaliableDirections.Count)]);
                }, null, delegate(Enemy enemy, Worm worm, Ground[,] grounds)
                {
                    switch (enemy.Direction)
                    {
                        case Direction.Up:
                            if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                            if (worm.WormRectangle.Y > enemy.Rectangle.Y) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.X != enemy.Rectangle.X) continue;
                                    if (enemy.Rectangle.Y > grounds[x, y].Rectangle.Y && grounds[x, y].Rectangle.Y > worm.WormRectangle.Y)
                                    {
                                        enemy.SawWorm = false;
                                        return;
                                    }
                                }
                            }
                            enemy.SawWorm = true;
                            enemy.MakeMove(Direction.Up);
                            return;
                        case Direction.Down:
                            if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                            if (worm.WormRectangle.Y < enemy.Rectangle.Y) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.X != enemy.Rectangle.X) continue;
                                    if (enemy.Rectangle.Y < grounds[x, y].Rectangle.Y && grounds[x, y].Rectangle.Y < worm.WormRectangle.Y)
                                    {
                                        enemy.SawWorm = false;
                                        return;
                                    }
                                }
                            }
                            enemy.SawWorm = true;
                            enemy.MakeMove(Direction.Down);
                            return;
                        case Direction.Right:
                            if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                            if (worm.WormRectangle.X < enemy.Rectangle.X) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.Y != enemy.Rectangle.Y) continue;
                                    if (enemy.Rectangle.X < grounds[x, y].Rectangle.X && grounds[x, y].Rectangle.X < worm.WormRectangle.X)
                                    {
                                        enemy.SawWorm = false;
                                        return;
                                    }
                                }
                            }
                            enemy.SawWorm = true;
                            enemy.MakeMove(Direction.Right);
                            return;
                        case Direction.Left:
                            if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                            if (worm.WormRectangle.X > enemy.Rectangle.X) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.Y != enemy.Rectangle.Y) continue;
                                    if (enemy.Rectangle.X > grounds[x, y].Rectangle.X &&
                                        grounds[x, y].Rectangle.X > worm.WormRectangle.X)
                                    {
                                        enemy.SawWorm = false; 
                                        return;
                                    }
                                }
                            }
                            enemy.SawWorm = true;
                            enemy.MakeMove(Direction.Left);
                            return;
                    }
                    enemy.SawWorm = false;
                }, worm => worm.Life--));
            _enemyTemplates.Add(EnemyType.Beetle, new Enemy(EnemyType.Beetle, "Game/Enemies/Beetle", 1, 1, 1, Direction.Left,false,
                delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.Beetle]);
                    e.Initialize(enemy.Rectangle);
                    e.Direction = enemy.Direction;
                    e.AddAsNew = true;
                    return e;
                }, 
                delegate(Enemy enemy, Ground[,] grounds, Rectangle gameField)
                {
                    List<Direction> avaliableDirections = new List<Direction>();
                    Rectangle[] rectangles = new Rectangle[4];
                    rectangles[0] = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y - 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    rectangles[1] = new Rectangle(enemy.Rectangle.X + 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    rectangles[2] = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y + 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                    rectangles[3] = new Rectangle(enemy.Rectangle.X - 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);

                    // Up
                    if (gameField.Contains(rectangles[0])) avaliableDirections.Add(Direction.Up);
                    // Right
                    if (gameField.Contains(rectangles[1])) avaliableDirections.Add(Direction.Right);
                    // Down
                    if (gameField.Contains(rectangles[2])) avaliableDirections.Add(Direction.Down);
                    // Left
                    if (gameField.Contains(rectangles[3])) avaliableDirections.Add(Direction.Left);

                    Random random = new Random();
                    Direction choosenDirection = avaliableDirections[random.Next(avaliableDirections.Count)];
                    int index = 0;

                    switch (choosenDirection)
                    {
                        case Direction.Up:
                            index = 0;
                            break;
                        case Direction.Right:
                            index = 1;
                            break;
                        case Direction.Down:
                            index = 2;
                            break;
                        case Direction.Left:
                            index = 3;
                            break;
                    }

                    for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                    {
                        for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                        {
                            if (grounds[x, y].GroundType == GroundType.Free) continue;

                            if (rectangles[index].Intersects(grounds[x, y].Rectangle))
                            {
                                enemy.IsDigging = true;
                                grounds[x, y].GroundType = GroundType.Free;
                            }
                        }
                    }

                    enemy.MakeMove(choosenDirection);
                }, null, delegate(Enemy enemy, Worm worm, Ground[,] grounds)
                {
                    if (Math.Sqrt(Math.Pow(enemy.Rectangle.X - worm.WormRectangle.X, 2) +
                         Math.Pow(enemy.Rectangle.Y - worm.WormRectangle.Y, 2)) <= 4*42)
                    {
                        enemy.SawWorm = true;
                        if (enemy.Rectangle.Y < worm.WormRectangle.Y)
                        {
                            Rectangle r = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y + 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;

                                    if (r.Intersects(grounds[x, y].Rectangle))
                                    {
                                        enemy.IsDigging = true;
                                        grounds[x, y].GroundType = GroundType.Free;
                                    }
                                }
                            }
                            enemy.MakeMove(Direction.Down);
                            return;
                        }
                        if (enemy.Rectangle.Y > worm.WormRectangle.Y)
                        {
                            Rectangle r = new Rectangle(enemy.Rectangle.X, enemy.Rectangle.Y - 42, enemy.Rectangle.Width, enemy.Rectangle.Height);
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;

                                    if (r.Intersects(grounds[x, y].Rectangle))
                                    {
                                        enemy.IsDigging = true;
                                        grounds[x, y].GroundType = GroundType.Free;
                                    }
                                }
                            }
                            enemy.MakeMove(Direction.Up);
                            return;
                        }
                        if (enemy.Rectangle.X > worm.WormRectangle.X)
                        {
                            Rectangle r = new Rectangle(enemy.Rectangle.X - 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;

                                    if (r.Intersects(grounds[x, y].Rectangle))
                                    {
                                        enemy.IsDigging = true;
                                        grounds[x, y].GroundType = GroundType.Free;
                                    }
                                }
                            }
                            enemy.MakeMove(Direction.Left);
                            return;
                        }
                        if (enemy.Rectangle.X < worm.WormRectangle.X)
                        {
                            Rectangle r = new Rectangle(enemy.Rectangle.X + 42, enemy.Rectangle.Y, enemy.Rectangle.Width, enemy.Rectangle.Height);
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;

                                    if (r.Intersects(grounds[x, y].Rectangle))
                                    {
                                        enemy.IsDigging = true;
                                        grounds[x, y].GroundType = GroundType.Free;
                                    }
                                }
                            }
                            enemy.MakeMove(Direction.Right);
                            return;
                        }
                    }
                    enemy.SawWorm = false;
                }, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.Spider, new Enemy(EnemyType.Spider, "Game/Enemies/Spider", 2, 1, 3, Direction.Left, false, delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.RedSpider]);
                    e.Initialize(enemy.Rectangle);
                    e.Direction = enemy.Direction;
                    return e;
                }, _enemyTemplates[EnemyType.Mouse].TestMove, null, _enemyTemplates[EnemyType.Mouse].Observe, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.RedSpider, new Enemy(EnemyType.RedSpider, "Game/Enemies/RedSpider", 2, 1, 3, Direction.Left, false, delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.RedSpider]);
                    e.Initialize(enemy.Rectangle);
                    e.Direction = enemy.Direction;
                    e.AddAsNew = true;
                    return e;
                }, _enemyTemplates[EnemyType.Mouse].TestMove, delegate(Enemy enemy, Worm worm, Ground[,] grounds)
                {
                    Shot s = null;
                    switch (enemy.Direction)
                    {
                        case Direction.Up:
                            if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                            if (worm.WormRectangle.Y > enemy.Rectangle.Y) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.X != enemy.Rectangle.X) continue;
                                    if (enemy.Rectangle.Y > grounds[x, y].Rectangle.Y && grounds[x, y].Rectangle.Y > worm.WormRectangle.Y) break;
                                }
                            }
                            s = new Shot(_shotTemplates[ShotType.Web]);
                            s.Initialize(enemy.Rectangle, Direction.Up);
                            return s;
                        case Direction.Down:
                            if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                            if (worm.WormRectangle.Y < enemy.Rectangle.Y) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.X != enemy.Rectangle.X) continue;
                                    if (enemy.Rectangle.Y < grounds[x, y].Rectangle.Y && grounds[x, y].Rectangle.Y < worm.WormRectangle.Y) break;
                                }
                            }
                            s = new Shot(_shotTemplates[ShotType.Web]);
                            s.Initialize(enemy.Rectangle, Direction.Down);
                            return s;
                        case Direction.Right:
                            if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                            if (worm.WormRectangle.X < enemy.Rectangle.X) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.Y != enemy.Rectangle.Y) continue;
                                    if (enemy.Rectangle.X < grounds[x, y].Rectangle.X && grounds[x, y].Rectangle.X < worm.WormRectangle.X) break;
                                }
                            }
                            s = new Shot(_shotTemplates[ShotType.Web]);
                            s.Initialize(enemy.Rectangle, Direction.Right);
                            return s;
                        case Direction.Left:
                            if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                            if (worm.WormRectangle.X > enemy.Rectangle.X) break;
                            for (int x = 0; x < Math.Sqrt(grounds.Length); x++)
                            {
                                for (int y = 0; y < Math.Sqrt(grounds.Length); y++)
                                {
                                    if (grounds[x, y].GroundType == GroundType.Free) continue;
                                    if (grounds[x, y].Rectangle.Y != enemy.Rectangle.Y) continue;
                                    if (enemy.Rectangle.X > grounds[x, y].Rectangle.X && grounds[x, y].Rectangle.X > worm.WormRectangle.X) break;
                                }
                            }
                            s = new Shot(_shotTemplates[ShotType.Web]);
                            s.Initialize(enemy.Rectangle, Direction.Left);
                            return s;
                    }
                    return s;
                }, null, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.Rat, new Rat(EnemyType.Rat, "Game/Enemies/Rat", null, 10, 3, Direction.Left, false, null, null, null, null, null));

            _shotTemplates = new Dictionary<ShotType, Shot>();
            _shotTemplates.Add(ShotType.Acid, new Shot(ShotType.Acid, "Game/Shots/AcidShot", enemy => enemy.Freeze(5000)));
            _shotTemplates.Add(ShotType.Venom, new Shot(ShotType.Venom, "Game/Shots/VenomShot", enemy => enemy.Life--));
            _shotTemplates.Add(ShotType.Web, new Shot(ShotType.Web, "Game/Shots/WebShot", null));

            _enemiesLevelCount = new Dictionary<int, Dictionary<EnemyType, int>>();
            for (int i = 0; i < 7; i++)
            {
                _enemiesLevelCount.Add(i, new Dictionary<EnemyType, int>());
                _enemiesLevelCount[i].Add(EnemyType.Mouse, 5);
            }

            // Myszy
            _enemiesLevelCount[1][EnemyType.Mouse] += 1;
            _enemiesLevelCount[4][EnemyType.Mouse] += 1;

            // Poziomy 2 do 6
            // Żuk lub pająk:
            Random rnd = new Random();
            for (int i = 2; i < 7; i++)
            {
                if (rnd.Next(100) > 50) _enemiesLevelCount[i].Add(EnemyType.Spider, 1);
                else _enemiesLevelCount[i].Add(EnemyType.Beetle, 1);
            }
            
            // Czerwony pająk
            for (int i = 3; i < 7; i++)
            {
                _enemiesLevelCount[i].Add(EnemyType.RedSpider, 1);
            }
            _enemiesLevelCount[4][EnemyType.RedSpider] += 1;

            if (rnd.Next(100) > 50)
            {
                if (_enemiesLevelCount[5].ContainsKey(EnemyType.Spider)) _enemiesLevelCount[5][EnemyType.Spider] += 1;
                else _enemiesLevelCount[5].Add(EnemyType.Spider, 1);
            }
            else
            {
                if (_enemiesLevelCount[5].ContainsKey(EnemyType.Beetle)) _enemiesLevelCount[5][EnemyType.Beetle] += 1;
                else _enemiesLevelCount[5].Add(EnemyType.Beetle, 1);
            }

            if (rnd.Next(100) > 50)
            {
                if (_enemiesLevelCount[6].ContainsKey(EnemyType.Spider)) _enemiesLevelCount[6][EnemyType.Spider] += 1;
                else _enemiesLevelCount[6].Add(EnemyType.Spider, 1);
            }
            else
            {
                if (_enemiesLevelCount[6].ContainsKey(EnemyType.Beetle)) _enemiesLevelCount[6][EnemyType.Beetle] += 1;
                else _enemiesLevelCount[6].Add(EnemyType.Beetle, 1);
            }
        }

        /// <summary>
        /// Metoda generująca koordynaty, które mają być wolne od ziemi (ścieżki).
        /// Wykorzystuje do tego algorytm opracowany w dokumentacji technicznej na stronie 15.
        /// </summary>
        /// <param name="n">Liczba "zagięć" korytaża.</param>
        /// <param name="width">Szerokość planszy, domyślnie 20.</param>
        /// <param name="height">Wysokość planszy, domyślnie 20.</param>
        /// <returns>Tablica punktów z "wolnymi" koordynatami.</returns>
        public Point[] GenerateFreeGroundsCoordinates(int n, int width=20, int height=20)
        {
            // Popraw do indeksowania od 0;
            width--;
            height--;

            Random random = new Random();
            Point[] points = new Point[n+2];
            points[0] = new Point(0,height);
            points[n + 1] = new Point(width,0);

            for (int i = 1; i < n+1; i++)
            {
                points[i] = new Point(random.Next(points[i - 1].X, width - 1), random.Next(1, points[i - 1].Y));
            }

            List<Point> tunnel = new List<Point>();
            for (int i = 1; i < n + 2; i++)
            {
                if (i%2 != 0)
                {
                    // Najpierw buduj poziomo
                    for (int x = points[i - 1].X; x < points[i].X; x++)
                    {
                        tunnel.Add(new Point(x, points[i - 1].Y));
                    }
                    // Potem pionowo
                    for (int y = points[i - 1].Y; y > points[i].Y; y--)
                    {
                        tunnel.Add(new Point(points[i].X, y));
                    }
                }
                else
                {
                    // Najpierw buduj pionowo
                    for (int y = points[i - 1].Y; y > points[i].Y; y--)
                    {
                        tunnel.Add(new Point(points[i-1].X, y));
                    }
                    // Potem poziomo
                    for (int x = points[i - 1].X; x < points[i].X; x++)
                    {
                        tunnel.Add(new Point(x, points[i].Y));
                    }
                }
            }
            tunnel.Add(new Point(width,0));

            return tunnel.ToArray();
        }

        /// <summary>
        /// Funkcja generująca owoce, które nie są czerwone.
        /// Wybór dokonuje się w sposób losowy.
        /// Zwrócone obiekty wymagają wywołania metody Initialize!
        /// </summary>
        /// <param name="n">Liczba owoców do wygenerowania.</param>
        /// <returns>Lista owoców.</returns>
        public List<Fruit> GenerateGrabableFruits(int n)
        {
            Random random = new Random();
            List<Fruit> fruits = new List<Fruit>();
            for (int i = 0; i < n; i++)
            {
                fruits.Add(new Fruit(_fruitsTemplates[(FruitType)random.Next((int)FruitType.Lemon,(int)FruitType.Candy+1)]));
            }
            return fruits;
        }

        /// <summary>
        /// Funkcja generująca listę czerwonych owoców.
        /// Zwrócone obiekty wymagają wywołania metody Initialize!
        /// </summary>
        /// <param name="n">Liczba owoców do wygenerowania.</param>
        /// <returns>Lista czerwonych owoców.</returns>
        public List<Fruit> GenerateRedFruits(int n)
        {
            List<Fruit> fruits = new List<Fruit>();
            for (int i = 0; i < n; i++)
            {
                fruits.Add(new Fruit(_fruitsTemplates[FruitType.RedFruit]));
            }
            return fruits;
        }

        /// <summary>
        /// Funkcja generująca strukturę, na której kolejno ułożone są kolejne jednostki wrogów.
        /// </summary>
        /// <param name="mouses">Liczba myszy.</param>
        /// <param name="beetles">Liczba żuków.</param>
        /// <param name="spiders">Liczba pająków.</param>
        /// <param name="redSpiders">Liczba czerownych pająków.</param>
        /// <param name="rats">Liczba szczurów.</param>
        /// <returns>Stos wrogów.</returns>
        public Stack<Enemy> GenerateEnemies(int mouses = 5, int beetles = 0, int spiders = 0, int redSpiders = 0, int rats = 0)
        {
            Stack<Enemy> enemies = new Stack<Enemy>();

            for (int i = 0; i < rats; i++)
            {
                enemies.Push(new Enemy(_enemyTemplates[EnemyType.Rat]));
            }
            for (int i = 0; i < redSpiders; i++)
            {
                enemies.Push(new Enemy(_enemyTemplates[EnemyType.RedSpider]));
            }
            for (int i = 0; i < spiders; i++)
            {
                enemies.Push(new Enemy(_enemyTemplates[EnemyType.Spider]));
            }
            for (int i = 0; i < beetles; i++)
            {
                enemies.Push(new Enemy(_enemyTemplates[EnemyType.Beetle]));
            }
            for (int i = 0; i < mouses; i++)
            {
                enemies.Push(new Enemy(_enemyTemplates[EnemyType.Mouse]));
            }

            return enemies;
        } 

        /// <summary>
        /// Metoda pozwalająca w szybki sposób wyciągnąć odpowiednie obiekty owoców w celu umieszczenia ich w interfejsie użytkownika.
        /// </summary>
        /// <returns>Tablica owoców.</returns>
        public Fruit[] GenerateInterfaceFruits()
        {
            Fruit[] fruits = new Fruit[6];

            fruits[0] = new Fruit(_fruitsTemplates[FruitType.Lemon]);
            fruits[1] = new Fruit(_fruitsTemplates[FruitType.Orange]);
            fruits[2] = new Fruit(_fruitsTemplates[FruitType.Plum]);
            fruits[3] = new Fruit(_fruitsTemplates[FruitType.Kiwi]);
            fruits[4] = new Fruit(_fruitsTemplates[FruitType.Watermelon]);
            fruits[5] = new Fruit(_fruitsTemplates[FruitType.Candy]);

            return fruits;
        }

        /// <summary>
        /// Metoda generująca odpowiednią liczbę obiektów kamieni.
        /// </summary>
        /// <param name="n">Liczba kamieni.</param>
        /// <returns>Lista kamieni.</returns>
        public List<Stone> GenerateStones(int n)
        {
            List<Stone> stones = new List<Stone>();

            while (n-- > 0)
            {
                stones.Add(new Stone());
            }

            return stones;
        }
        
        /// <summary>
        /// Metoda generująca sakiewki.
        /// Każda z nich z różnym prawdopodobieństwem może zawierać różne owocki bądź być pustą.
        /// </summary>
        /// <param name="n">Liczba sakiewek do wygenerowania.</param>
        /// <returns>Lista sakiewek.</returns>
        public List<Purse> GeneratePurses(int n)
        {
            List<Purse> purses = new List<Purse>();
            Random random = new Random();
            int next;
            FruitType fruitType;

            while (n-- > 0)
            {
                next = random.Next(101);

                if (next >= 90)
                {
                    purses.Add(new Purse());
                    continue;
                }

                fruitType = next >= 85
                    ? FruitType.Candy
                    : next >= 70
                        ? FruitType.Kiwi
                        : next >= 55
                            ? FruitType.Plum
                            : next >= 40
                                ? FruitType.Watermelon
                                : next >= 20 ? FruitType.Orange : FruitType.Lemon;

                purses.Add(new Purse(new Fruit(_fruitsTemplates[fruitType])));
            }

            return purses;
        }

        #region Properites

        /// <summary>
        /// Słownik strzałów.
        /// </summary>
        public Dictionary<ShotType, Shot> ShotTemplates
        {
            get { return _shotTemplates; }
        }

        /// <summary>
        /// Słownik owoców.
        /// </summary>
        public Dictionary<FruitType, Fruit> FruitsTemplates
        {
            get { return _fruitsTemplates; }
        }

        /// <summary>
        /// Słownik wrogów.
        /// </summary>
        public Dictionary<EnemyType, Enemy> EnemyTemplates
        {
            get { return _enemyTemplates; }
        }

        /// <summary>
        /// Słownik liczności wrogów na danym poziome gry.
        /// </summary>
        public Dictionary<int, Dictionary<EnemyType, int>> EnemiesLevelCount
        {
            get { return _enemiesLevelCount; }
        }
        #endregion
    }
}