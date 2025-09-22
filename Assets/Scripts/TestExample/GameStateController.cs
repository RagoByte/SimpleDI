using System.Threading.Tasks;
using SimpleDI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestExample
{
    public class GameStateController : MonoBehaviour
    {
        [Dependency] private GlobalDependencyContext _globalDependencyContext;
        private DependencyContext _childDependencyContext;
        [Dependency] private DependencyFactory _dependencyFactory;
        [Dependency] private Config _сonfig;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                LoadGameScene();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Player player = _dependencyFactory.Resolve<Player>();
                player.gameObject.SetActive(!player.isActiveAndEnabled);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                _dependencyFactory.Instantiate(_сonfig.BoxPrefab);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Enemy enemy = _dependencyFactory.CreateInstance<Enemy>();
                enemy.Attack();
            }
        }

        private async Task LoadGameScene()
        {
            if (_childDependencyContext) Destroy(_childDependencyContext.gameObject);
            await SceneManager.LoadSceneAsync("GameScene");

            _childDependencyContext = _globalDependencyContext.CreateChild();
            _dependencyFactory.SetChildContext(_childDependencyContext);

            _childDependencyContext.Register(Instantiate(_сonfig.PlayerPrefab));
        }
    }
}