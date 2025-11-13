namespace EachOneMatters.Common
{
    public class Health
    {
        private int _healthPoint;

        public int HealthPoint => _healthPoint;

        public void SetHealthPoint(int healthPoint)
        {
            _healthPoint = healthPoint;
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
            {
                _healthPoint -= damage;
            }

            if (_healthPoint < 0)
            {
                _healthPoint = 0;
            }
        }
    }
}