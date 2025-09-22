using SimpleDI;
using UnityEngine;

namespace TestExample
{
    public class Box : MonoBehaviour
    {
        ILoggerService _loggerService;
        private Player _player;

        [Dependency]
        private void Construct(ILoggerService logger, Player player)
        {
            _player = player;
            _loggerService = logger;
            logger.Log("Box Constructed");
        }

        private void Start()
        {
            _loggerService.Log("start invoked in Box");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _player.Up();
            }
        }
    }
}