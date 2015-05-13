using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Digger.Game.Common;
using Digger.Game.Elements;
using Microsoft.Xna.Framework;

namespace Digger.Game.Specified
{
    public class Rat : Enemy
    {
        private bool[,] _visitedGrounds;
        private Ground[,] _prevGrounds;
        private Ground _currentGround;
        private int x, y; // Będą indeksami tablicy 

        // A*
        private double[,] _distance;
        private List<Tuple<Ground,int,int>> _t;
        private bool _aIsEnabled;
        //private List<Ground> _aStarGrounds;
        private Tuple<Ground,int,int>[,] _aStarGrounds;
        private Stack<Point> _aNextPositions; 

        /// <summary>
        /// Konstruktor kopiujący
        /// </summary>
        /// <param name="enemy">Obiekt źródłowy</param>
        public Rat(Enemy enemy) : base(enemy)
        {
        }

        public Rat(EnemyType enemyType, string assetName, int? life, int strenght, int speed, Direction direction, bool isFreeze, EvolveDelegate evolve, TestMoveDelegate testMove, WebShootDelegate webShoot, ObserveDelegate observe, AttackDelegate attack) : base(enemyType, assetName, life, strenght, speed, direction, isFreeze, evolve, testMove, webShoot, observe, attack)
        {
            _testMove = TestMove;
            _attack = Attack;
            _observe = Observe;
            _visitedGrounds = new bool[20, 20];
            _prevGrounds = new Ground[20,20];
            x = 19;
            y = 0;

            // A*
            _distance = new double[20,20];
            for (int i = 0; i < _distance.GetLength(0); i++)
            {
                for (int j = 0; j < _distance.GetLength(1); j++)
                {
                    _distance[i, j] = double.MaxValue;
                }
            }
            _t=new List<Tuple<Ground, int, int>>();
            _aIsEnabled = false;
            _aStarGrounds = new Tuple<Ground, int, int>[20,20];
            _aNextPositions = new Stack<Point>();
            //_aStarGrounds = new List<Ground>();
        }

        public override void Initialize(Rectangle startPosition)
        {
            base.Initialize(startPosition);
        }

        public void Attack(Worm worm)
        {
            worm.Life = 0;
        }

        public override bool IsKilled
        {
            get { return _isKilled; }
            set { _isKilled = false; } // Igoruj
        }

        public void Observe(Enemy enemy, Worm worm, Ground[,] grounds)
        {
            enemy.SawWorm = false;

            // Spradz, czy gracz drazy tunele
            if (worm.IsDigging || _aIsEnabled)
            {
                if (Math.Sqrt(Math.Pow(enemy.Rectangle.X - worm.WormRectangle.X, 2) +
                              Math.Pow(enemy.Rectangle.Y - worm.WormRectangle.Y, 2)) <= 4 * 42 || _aIsEnabled)
                {
                    _aIsEnabled = true;
                    _distance[x, y] = 0;
                    int freeGroundsCount = 0;

                    // A*
                    // Utwórz zbiór wierzchołków, do których można się dostać
                    _t.Clear();
                    for (int i = 0; i < grounds.GetLength(0); i++)
                    {
                        for (int j = 0; j < grounds.GetLength(1); j++)
                        {
                            if (grounds[i, j].GroundType == GroundType.Free)
                            {
                                _t.Add(new Tuple<Ground, int, int>(grounds[i, j],i,j));
                                freeGroundsCount++;
                            }
                            //_aStarGrounds[i, j] = null;
                        }
                    }

                    int index = 0;
                    int sum = 0;
                    while (_t.Count > 0)
                    {
                        // Source to nasz robaczek
                        double min = double.MaxValue;
                        for (int i = 0; i < _t.Count; i++)
                        {
                            if (_distance[_t[i].Item2, _t[i].Item3] +
                                Math.Sqrt(Math.Pow(_t[i].Item1.Rectangle.X - worm.Destination.X, 2) +
                                          Math.Pow(_t[i].Item1.Rectangle.Y - worm.Destination.Y, 2)) <= min)
                            {
                                min = _distance[_t[i].Item2, _t[i].Item3] +
                                      Math.Sqrt(Math.Pow(_t[i].Item1.Rectangle.X - worm.Destination.X, 2) +
                                                Math.Pow(_t[i].Item1.Rectangle.Y - worm.Destination.Y, 2));
                                index = i;
                            }
                        }

                        // Czy algorytm nie osiągnął już celu?
                        if (worm.Destination.Y == _t[index].Item1.Rectangle.Y &&
                            worm.Destination.X == _t[index].Item1.Rectangle.X)
                        {
                            break;
                        }

                        // Wyszukaj i aktualizuj sąsiadów
                        for (int i = 0; i < _t.Count; i++)
                        {
                            if (i == index) continue;

                            if ((_t[i].Item1.Rectangle.Y - 42 == _t[index].Item1.Rectangle.Y && _t[i].Item1.Rectangle.X == _t[index].Item1.Rectangle.X) ||
                                (_t[i].Item1.Rectangle.X + 42 == _t[index].Item1.Rectangle.X && _t[i].Item1.Rectangle.Y == _t[index].Item1.Rectangle.Y) ||
                                (_t[i].Item1.Rectangle.Y + 42 == _t[index].Item1.Rectangle.Y && _t[i].Item1.Rectangle.X == _t[index].Item1.Rectangle.X) ||
                                (_t[i].Item1.Rectangle.X - 42 == _t[index].Item1.Rectangle.X && _t[i].Item1.Rectangle.Y == _t[index].Item1.Rectangle.Y))
                            {
                                if (_distance[_t[i].Item2, _t[i].Item3] > _distance[_t[index].Item2, _t[index].Item3] + 42)
                                {
                                    _distance[_t[i].Item2, _t[i].Item3] = _distance[_t[index].Item2, _t[index].Item3] + 42;
                                    _aStarGrounds[_t[i].Item2, _t[i].Item3] =
                                        new Tuple<Ground, int, int>(grounds[_t[index].Item2, _t[index].Item3], _t[index].Item2, _t[index].Item3);
                                    //_aStarGrounds[_t[i].Item2, _t[i].Item3].Item1.GroundType = GroundType.Normal;
                                    //_aStarGrounds.Add(_t[i]);
                                }
                            }
                        }

                        //_aStarGrounds.Add(_t[index].Item1);
                        _t.RemoveAt(index);
                    }

                    if (_t.Count == 0)
                    {
                        //_aIsEnabled = false;
                        return;
                    }

                    // Przegląd prevów w celu uzyskania kolejnego ruchu
                    // Wciąż pamiętany jest ostatni indeks
                    int tempX, tempY;
                    tempX = _t[index].Item2;
                    tempY = _t[index].Item3;
                    Point nextPoint = new Point(_aStarGrounds[tempX, tempY].Item1.Rectangle.X, _aStarGrounds[tempX, tempY].Item1.Rectangle.Y);

                    while ((enemy.Rectangle.Y - 42 != nextPoint.Y || enemy.Rectangle.X != nextPoint.X) &&
                                (enemy.Rectangle.X + 42 != nextPoint.X || enemy.Rectangle.Y != nextPoint.Y) &&
                                (enemy.Rectangle.Y + 42 != nextPoint.Y || enemy.Rectangle.X != nextPoint.X) &&
                                (enemy.Rectangle.X - 42 != nextPoint.X || enemy.Rectangle.Y != nextPoint.Y))
                    {
                        //_aStarGrounds[tempX, tempY].Item1.GroundType = GroundType.Normal;
                        //_aNextPositions.Push(new Point(_aStarGrounds[tempX, tempY].Item1.Rectangle.X, _aStarGrounds[tempX, tempY].Item1.Rectangle.Y));
                        if (_aStarGrounds[tempX, tempY] == null) return;
                        nextPoint = new Point(_aStarGrounds[tempX, tempY].Item1.Rectangle.X, _aStarGrounds[tempX, tempY].Item1.Rectangle.Y);
                        int temp = tempX;
                        tempX = _aStarGrounds[tempX, tempY].Item2;
                        tempY = _aStarGrounds[temp, tempY].Item3;
                    }

                    //for (int i = 0; i < _aStarGrounds.GetLength(0); i++)
                    //{
                    //    for (int j = 0; j < _aStarGrounds.GetLength(1); j++)
                    //    {
                    //        if (_aStarGrounds[i, j] != null)
                    //        {
                    //            _aStarGrounds[i, j].GroundType = GroundType.Normal;
                    //        }
                    //    }
                    //}

                    // Determinate direction
                    // TOP 
                    if (enemy.Rectangle.Y - 42 == nextPoint.Y && enemy.Rectangle.X == nextPoint.X)
                    {
                        enemy.MoveFaster(7);
                        enemy.SawWorm = true;
                        //_prevGrounds[_t[index].Item2, _t[index].Item3] = grounds[x, y];
                        y--;
                        enemy.MakeMove(Direction.Up);
                        return;
                    }
                    // RIGHT
                    if (enemy.Rectangle.X + 42 == nextPoint.X && enemy.Rectangle.Y == nextPoint.Y)
                    {
                        enemy.MoveFaster(7);
                        enemy.SawWorm = true;
                        //_prevGrounds[_t[index].Item2, _t[index].Item3] = grounds[x, y];
                        x++;
                        enemy.MakeMove(Direction.Right);
                        return;
                    }
                    // BOTTOM
                    if (enemy.Rectangle.Y + 42 == nextPoint.Y && enemy.Rectangle.X == nextPoint.X)
                    {
                        enemy.MoveFaster(7);
                        enemy.SawWorm = true;
                        //_prevGrounds[_t[index].Item2, _t[index].Item3] = grounds[x, y];
                        y++;
                        enemy.MakeMove(Direction.Down);
                        return;
                    }
                    // LEFT
                    if (enemy.Rectangle.X - 42 == nextPoint.X && enemy.Rectangle.Y == nextPoint.Y)
                    {
                        enemy.MoveFaster(7);
                        enemy.SawWorm = true;
                        x--;
                        enemy.MakeMove(Direction.Left);
                        return;
                    }

                    //_t.RemoveAt(index);
                    //return;
                }
            }

            _t.Clear();
            for (int i = 0; i < grounds.GetLength(0); i++)
            {
                for (int j = 0; j < grounds.GetLength(1); j++)
                {
                    _aStarGrounds[i, j] = null;
                    _distance[i, j] = 10000;
                }
            }
            _aIsEnabled = false;

            switch (enemy.Direction)
            {
                case Direction.Up:
                    if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                    if (worm.WormRectangle.Y > enemy.Rectangle.Y) break;
                    for (int i = 0; i < Math.Sqrt(grounds.Length); i++)
                    {
                        for (int j = 0; j < Math.Sqrt(grounds.Length); j++)
                        {
                            if (grounds[i,j].GroundType == GroundType.Free) continue;
                            if (grounds[i,j].Rectangle.X != enemy.Rectangle.X) continue;
                            if (enemy.Rectangle.Y > grounds[i,j].Rectangle.Y && grounds[i,j].Rectangle.Y > worm.WormRectangle.Y)
                            {
                                return;
                            }
                        }
                    }
                    // Oznacz jako odwiedzony
                    _visitedGrounds[x, y] = true;
                    // Ustaw poprzednika
                    _prevGrounds[x, y - 1] = grounds[x, y];
                    y--;
                    _distance[x, y] = 0; // A*

                    enemy.MoveFaster(7);
                    enemy.SawWorm = true;
                    enemy.MakeMove(Direction.Up);
                    return;
                case Direction.Down:
                    if (worm.WormRectangle.X != enemy.Rectangle.X) break;
                    if (worm.WormRectangle.Y < enemy.Rectangle.Y) break;
                    for (int i = 0; i < Math.Sqrt(grounds.Length); i++)
                    {
                        for (int j = 0; j < Math.Sqrt(grounds.Length); j++)
                        {
                            if (grounds[i, j].GroundType == GroundType.Free) continue;
                            if (grounds[i, j].Rectangle.X != enemy.Rectangle.X) continue;
                            if (enemy.Rectangle.Y < grounds[i, j].Rectangle.Y && grounds[i, j].Rectangle.Y < worm.WormRectangle.Y)
                            {
                                return;
                            }
                        }
                    }
                    // Oznacz jako odwiedzony
                    _visitedGrounds[x, y] = true;
                    // Ustaw poprzednika
                    _prevGrounds[x, y + 1] = grounds[x, y];
                    y++;
                    _distance[x, y] = 0; // A*

                    enemy.MoveFaster(7);
                    enemy.SawWorm = true;
                    enemy.MakeMove(Direction.Down);
                    return;
                case Direction.Right:
                    if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                    if (worm.WormRectangle.X < enemy.Rectangle.X) break;
                    for (int i = 0; i < Math.Sqrt(grounds.Length); i++)
                    {
                        for (int j = 0; j < Math.Sqrt(grounds.Length); j++)
                        {
                            if (grounds[i, j].GroundType == GroundType.Free) continue;
                            if (grounds[i, j].Rectangle.Y != enemy.Rectangle.Y) continue;
                            if (enemy.Rectangle.X < grounds[i, j].Rectangle.X && grounds[i, j].Rectangle.X < worm.WormRectangle.X)
                            {
                                return;
                            }
                        }
                    }
                    // Oznacz jako odwiedzony
                    _visitedGrounds[x, y] = true;
                    // Ustaw poprzednika
                    _prevGrounds[x + 1, y] = grounds[x, y];
                    x++;
                    _distance[x, y] = 0; // A*

                    enemy.MoveFaster(7);
                    enemy.SawWorm = true;
                    enemy.MakeMove(Direction.Right);
                    return;
                case Direction.Left:
                    if (worm.WormRectangle.Y != enemy.Rectangle.Y) break;
                    if (worm.WormRectangle.X > enemy.Rectangle.X) break;
                    for (int i = 0; i < Math.Sqrt(grounds.Length); i++)
                    {
                        for (int j = 0; j < Math.Sqrt(grounds.Length); j++)
                        {
                            if (grounds[i, j].GroundType == GroundType.Free) continue;
                            if (grounds[i, j].Rectangle.Y != enemy.Rectangle.Y) continue;
                            if (enemy.Rectangle.X > grounds[i, j].Rectangle.X &&
                                grounds[i, j].Rectangle.X > worm.WormRectangle.X)
                            {
                                return;
                            }
                        }
                    }
                    // Oznacz jako odwiedzony
                    _visitedGrounds[x, y] = true;
                    // Ustaw poprzednika
                    _prevGrounds[x - 1, y] = grounds[x, y];
                    x--;
                    _distance[x, y] = 0; // A*

                    enemy.MoveFaster(7);
                    enemy.SawWorm = true;
                    enemy.MakeMove(Direction.Left);
                    return;
            }
            enemy.SawWorm = false;
        }

        public void TestMove(Enemy enemy, Ground[,] grounds, Rectangle gameField)
        {
            // Sprawdź, czy nie jesteś na samym początku planszy
            if (x == 19 && y == 0)
            {
                // Wyczyść kolejność przechodzenia
                _visitedGrounds = new bool[20, 20];
                _prevGrounds = new Ground[20, 20];
            }

            // Oznacz jako odwiedzony
            _visitedGrounds[x, y] = true;

            // Sprawdź, czy można przenieść się na sąsiednie prostokąty
            // Top
            if (y > 0)
                if (grounds[x, y - 1].GroundType == GroundType.Free && !_visitedGrounds[x,y-1])
                {
                    // Ustaw poprzednika
                    _prevGrounds[x, y - 1] = grounds[x, y];
                    y--;
                    _distance[x, y] = 0; // A*
                    // Wykonaj ruch
                    enemy.MakeMove(Direction.Up);
                    return;
                }
            // Right
            if (x < 19)
                if (grounds[x + 1, y].GroundType == GroundType.Free && !_visitedGrounds[x+1, y])
                {
                    // Ustaw poprzednika
                    _prevGrounds[x+1, y] = grounds[x, y];
                    x++;
                    // Wykonaj ruch
                    enemy.MakeMove(Direction.Right);
                    return;
                }
            // Bottom
            if (y < 19)
                if (grounds[x, y + 1].GroundType == GroundType.Free && !_visitedGrounds[x, y + 1])
                {
                    // Ustaw poprzednika
                    _prevGrounds[x, y + 1] = grounds[x, y];
                    y++;
                    _distance[x, y] = 0; // A*
                    // Wykonaj ruch
                    enemy.MakeMove(Direction.Down);
                    return;
                }
            // Left
            if (x > 0)
                if (grounds[x - 1, y].GroundType == GroundType.Free && !_visitedGrounds[x-1, y])
                {
                    // Ustaw poprzednika
                    _prevGrounds[x - 1, y] = grounds[x, y];
                    x--;
                    _distance[x, y] = 0; // A*
                    // Wykonaj ruch
                    enemy.MakeMove(Direction.Left);
                    return;
                }
            
            // Jeżeli sterowanie dojdzie tutaj, oznacza to, że dochodzimy do cofania się "rekursji"
            // x,y - obecne koordynaty
            // Obecne położenie robaczka this.Rectangle lub currentGround
            // Zdeterminuj kierunek poruszania się
            // TOP or BOTTOM
            if (enemy.Rectangle.Y != _prevGrounds[x, y].Rectangle.Y)
            {
                // do TOP
                if (_prevGrounds[x, y].Rectangle.Y < enemy.Rectangle.Y)
                {
                    enemy.MakeMove(Direction.Up);
                    y--;
                    _distance[x, y] = 0; // A*
                    return;
                }
                // do BOTTOM
                if (_prevGrounds[x, y].Rectangle.Y > enemy.Rectangle.Y)
                {
                    enemy.MakeMove(Direction.Down);
                    y++;
                    _distance[x, y] = 0; // A*
                    return;
                }
            }
            // RIGHT or LEFT
            if (enemy.Rectangle.X != _prevGrounds[x, y].Rectangle.X)
            {
                // to RIGHT
                if (_prevGrounds[x, y].Rectangle.X > enemy.Rectangle.X)
                {
                    enemy.MakeMove(Direction.Right);
                    x++;
                    _distance[x, y] = 0; // A*
                    return;
                }
                // to LEFT
                if (_prevGrounds[x, y].Rectangle.X < enemy.Rectangle.X)
                {
                    enemy.MakeMove(Direction.Left);
                    x--;
                    _distance[x, y] = 0; // A*
                    return;
                }
            }
        }
    }
}