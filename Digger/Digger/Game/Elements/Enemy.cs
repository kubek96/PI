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

    public class Enemy
    {
        private EnemyType _enemyType;
        private EvolveDelegate _evolve;

        public EvolveDelegate Evolve
        {
            get { return _evolve; }
        }

        public void Kill()
        {
            
        }
    }
}