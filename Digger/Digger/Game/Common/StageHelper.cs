using System;
using System.Collections.Generic;
using Digger.Game.Elements;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Digger.Game.Common
{
    public class StageHelper
    {
        private Dictionary<FruitType, Fruit> _fruitsTemplates;
        private Dictionary<EnemyType, Enemy> _enemyTemplates;
        private Dictionary<ShotType, Shot> _shotTemplates; 

        public StageHelper()
        {
            _fruitsTemplates = new Dictionary<FruitType, Fruit>();
            _fruitsTemplates.Add(FruitType.Lemon, new Fruit(FruitType.Lemon, "Game/Fruits/Lemon", worm => worm.AcidShoots += 5, delegate(Enemy enemy)
            {
                return enemy.Evolve(enemy);
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
            _fruitsTemplates.Add(FruitType.Candy, new Fruit(FruitType.Candy, "Game/Fruits/Candy", worm => worm.CandyCount++, _fruitsTemplates[FruitType.Lemon].EnemyUse));
            _fruitsTemplates.Add(FruitType.RedFruit, new Fruit(FruitType.RedFruit, "Game/Fruits/RedOne", worm => worm.RedFruits++, null));

            _enemyTemplates = new Dictionary<EnemyType, Enemy>();
            _enemyTemplates.Add(EnemyType.Mouse, new Enemy(EnemyType.Mouse, "Game/Enemies/Mouse", 1, 1, 2, Direction.Left, false,
                delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.Spider]);
                    e.Initialize(enemy.EnemyRectangle);
                    e.Direction = enemy.Direction;
                    return e;
                }, delegate(Enemy enemy, Ground[,] grounds, Rectangle gameField)
                {
                    List<Direction> avaliableDirections = new List<Direction>();
                    Rectangle upRectangle, downRectangle, rightRectangle, leftRectangle;
                    upRectangle = new Rectangle(enemy.EnemyRectangle.X, enemy.EnemyRectangle.Y - 42, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    rightRectangle = new Rectangle(enemy.EnemyRectangle.X + 42, enemy.EnemyRectangle.Y, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    downRectangle = new Rectangle(enemy.EnemyRectangle.X, enemy.EnemyRectangle.Y + 42, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    leftRectangle = new Rectangle(enemy.EnemyRectangle.X - 42, enemy.EnemyRectangle.Y, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
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
                }, null, null, worm => worm.Life--));
            _enemyTemplates.Add(EnemyType.Beetle, new Enemy(EnemyType.Beetle, "Game/Enemies/Beetle", 1, 1, 1, Direction.Left,false,
                delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.RedSpider]);
                    e.Initialize(enemy.EnemyRectangle);
                    e.Direction = enemy.Direction;
                    return e;
                }, 
                delegate(Enemy enemy, Ground[,] grounds, Rectangle gameField)
                {
                    List<Direction> avaliableDirections = new List<Direction>();
                    Rectangle[] rectangles = new Rectangle[4];
                    rectangles[0] = new Rectangle(enemy.EnemyRectangle.X, enemy.EnemyRectangle.Y - 42, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    rectangles[1] = new Rectangle(enemy.EnemyRectangle.X + 42, enemy.EnemyRectangle.Y, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    rectangles[2] = new Rectangle(enemy.EnemyRectangle.X, enemy.EnemyRectangle.Y + 42, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);
                    rectangles[3] = new Rectangle(enemy.EnemyRectangle.X - 42, enemy.EnemyRectangle.Y, enemy.EnemyRectangle.Width, enemy.EnemyRectangle.Height);

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
                }, null, null, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.Spider, new Enemy(EnemyType.Spider, "Game/Enemies/Spider", 2, 1, 3, Direction.Left, false, null, _enemyTemplates[EnemyType.Mouse].TestMove, null, null, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.RedSpider, new Enemy(EnemyType.RedSpider, "Game/Enemies/RedSpider", 2, 1, 3, Direction.Left, false, null, _enemyTemplates[EnemyType.Mouse].TestMove, null, null, _enemyTemplates[EnemyType.Mouse].Attack));
            _enemyTemplates.Add(EnemyType.Rat, new Enemy(EnemyType.Rat, "Game/Enemies/Rat", null, 10, 7, Direction.Left, false, null, null, null, null, null));

            _shotTemplates = new Dictionary<ShotType, Shot>();
            _shotTemplates.Add(ShotType.Acid, new Shot(ShotType.Acid, "Game/Shots/AcidShot", enemy => enemy.Freeze(5000)));
            _shotTemplates.Add(ShotType.Venom, new Shot(ShotType.Venom, "Game/Shots/VenomShot", enemy => enemy.Life--));
        }

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

        // Zwrócone obiekty wymagają initialize!
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

        public List<Fruit> GenerateRedFruits(int n)
        {
            List<Fruit> fruits = new List<Fruit>();
            for (int i = 0; i < n; i++)
            {
                fruits.Add(new Fruit(_fruitsTemplates[FruitType.RedFruit]));
            }
            return fruits;
        }

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

        public List<Stone> GenerateStones(int n)
        {
            List<Stone> stones = new List<Stone>();

            while (n-- > 0)
            {
                stones.Add(new Stone());
            }

            return stones;
        }

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

        public Dictionary<ShotType, Shot> ShotTemplates
        {
            get { return _shotTemplates; }
        }

        public Dictionary<FruitType, Fruit> FruitsTemplates
        {
            get { return _fruitsTemplates; }
        }

        public Dictionary<EnemyType, Enemy> EnemyTemplates
        {
            get { return _enemyTemplates; }
        }
    }
}