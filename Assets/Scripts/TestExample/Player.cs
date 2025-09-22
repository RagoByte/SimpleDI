using SimpleDI;
using UnityEngine;

namespace TestExample
{
    public class Player : MonoBehaviour
    {
        ILoggerService _loggerService;

        [Dependency]
        private void Construct(ILoggerService loggerService)
        {
            _loggerService = loggerService;
            loggerService.Log("Player Constructed");
        }


        public void Up()
        {
            transform.transform.position += Vector3.up;
        }
    }
}