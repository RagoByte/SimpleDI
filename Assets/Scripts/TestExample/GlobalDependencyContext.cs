using SimpleDI;
using UnityEngine;

namespace TestExample
{
    public class GlobalDependencyContext : DependencyContext
    {
        [SerializeField] private Config config;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Registrations();
            Resolve<GameStateController>(); //start
        }

        private void Registrations()
        {
            Register<GlobalDependencyContext>(this);
            Register(new DependencyFactory());
            Register(config);

            RegisterWithDontDestroyOnLoad(Instantiate(config.GameStateControllerPrefab));

            Register<ILoggerService>(new LoggerService());
        }
    }
}