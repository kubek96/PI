using Digger.Game.Common;

namespace Digger.Game.Elements
{
    public enum EnemyType
    {
        Mouse,
        Beetle,
        Spider,
        RedSpider,  
        Rat
    }

    public delegate void EvolveDelegate();
    public delegate void Move();
    public delegate Shot WebShoot();
    public delegate void Observe();
    public delegate void Attack();

    public class Enemy
    {
        private EnemyType _enemyType;
        private EvolveDelegate _evolve;
        private int _life;
        private int _strenght;
        private bool isFreeze;
        private int _speed;
        private Direction _direction;

        

        public EvolveDelegate Evolve
        {
            get { return _evolve; }
        }

        public void Kill()
        {
            
        }
    }
}