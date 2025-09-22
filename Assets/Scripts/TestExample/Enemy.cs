namespace TestExample
{
    public class Enemy
    {
        private ILoggerService _logger;

        private Enemy(ILoggerService logger)
        {
            _logger = logger;
            _logger.Log("Enemy создан!");
        }


        public void Attack() => _logger.Log("Атака!");
    }
}