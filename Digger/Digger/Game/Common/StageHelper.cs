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
            _fruitsTemplates.Add(FruitType.Lemon, new Fruit(FruitType.Lemon, "Game/Fruits/Lemon", worm => worm.AcidShoots += 5, enemy => enemy.Evolve(enemy)));
            _fruitsTemplates.Add(FruitType.Orange, new Fruit(FruitType.Orange, "Game/Fruits/Orange", worm => worm.VenomShoots += 5, enemy => enemy.Evolve(enemy)));
            _fruitsTemplates.Add(FruitType.Kiwi, new Fruit(FruitType.Kiwi, "Game/Fruits/Kiwi", worm => worm.KiwisCount++, enemy => enemy.IsKilled=true));
            _fruitsTemplates.Add(FruitType.Watermelon, new Fruit(FruitType.Watermelon, "Game/Fruits/Watermelon", worm => worm.MudCount++, enemy => enemy.Evolve(enemy)));
            _fruitsTemplates.Add(FruitType.Plum, new Fruit(FruitType.Plum, "Game/Fruits/Plum", worm => worm.PlumsCount++, enemy => enemy.Evolve(enemy)));
            _fruitsTemplates.Add(FruitType.Candy, new Fruit(FruitType.Candy, "Game/Fruits/Candy", worm => worm.CandyCount++, enemy => enemy.Evolve(enemy)));
            _fruitsTemplates.Add(FruitType.RedFruit, new Fruit(FruitType.RedFruit, "Game/Fruits/RedOne", worm => worm.RedFruits++, null));

            _enemyTemplates = new Dictionary<EnemyType, Enemy>();
            _enemyTemplates.Add(EnemyType.Mouse, new Enemy(EnemyType.Mouse, "Game/Enemies/Mouse", 1, 1, 2, Direction.Left, false,
                delegate(Enemy enemy)
                {
                    Enemy e = new Enemy(_enemyTemplates[EnemyType.Spider]); 
                    e.Initialize(enemy.EnemyRectangle); 
                    e.Direction=enemy.Direction;
                    enemy = e;
                }, delegate(Enemy enemy, Direction[] availableDirections)
                {
                    // Podmień pozycję 
                    Random random = new Random();
                    int x = 0, y = 0;
                    int m = random.Next(availableDirections.Length);
                    switch (availableDirections[m])
                    {
                        case Direction.Up:
                            y -= 42;
                            break;
                        case Direction.Right:
                            x += 42;
                            break;
                        case Direction.Down:
                            y += 42;
                            break;
                        case Direction.Left:
                            x -= 42;
                            break;
                    }
                    enemy.Destination = new Point(enemy.EnemyRectangle.X + x, enemy.EnemyRectangle.Y + y);
                    enemy.IsMoving = true;
                }, null, null, delegate(Worm worm)
                {
                    worm.Life--;
                    // TODO: freeze;
                }));
            _enemyTemplates.Add(EnemyType.Beetle, new Enemy(EnemyType.Beetle, "Game/Enemies/Beetle", 1, 1, 1, Direction.Left,false, null, null,null, null, null));
            _enemyTemplates.Add(EnemyType.Spider, new Enemy(EnemyType.Spider, "Game/Enemies/Spider", 2, 1, 3, Direction.Left, false, null, null,null, null, null));
            _enemyTemplates.Add(EnemyType.RedSpider, new Enemy(EnemyType.RedSpider, "Game/Enemies/RedSpider", 2, 1, 3, Direction.Left,false,null, null,null, null, null));
            _enemyTemplates.Add(EnemyType.Rat, new Enemy(EnemyType.Rat, "Game/Enemies/Rat", null, 10, 7, Direction.Left, false, null, null, null, null, null));

            _shotTemplates = new Dictionary<ShotType, Shot>();
            _shotTemplates.Add(ShotType.Acid, new Shot(ShotType.Acid, "Game/Shots/AcidShot"));
            _shotTemplates.Add(ShotType.Venom, new Shot(ShotType.Venom, "Game/Shots/VenomShot"));
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