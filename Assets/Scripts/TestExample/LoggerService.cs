using UnityEngine;

namespace TestExample
{
    public class LoggerService : ILoggerService
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}