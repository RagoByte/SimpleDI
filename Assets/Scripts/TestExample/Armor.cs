using SimpleDI;
using UnityEngine;

namespace TestExample
{
    public class Armor : MonoBehaviour
    {
        [Dependency]
        private void Construct(ILoggerService logger)
        {
            logger.Log("Construct from Armor");
        }
    }
}