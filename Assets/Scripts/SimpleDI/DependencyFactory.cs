using TestExample;
using UnityEngine;

namespace SimpleDI
{
    public class DependencyFactory
    {
        [Dependency] private Config _config;
        [Dependency] private readonly GlobalDependencyContext _globalDependencyContext;
        private DependencyContext _childDependencyContext;

        private DependencyContext ActiveContext => _childDependencyContext ?? _globalDependencyContext;

        public void SetChildContext(DependencyContext child)
        {
            _childDependencyContext = child;
        }

        public T CreateInstance<T>() => ActiveContext.CreateWithDependencies<T>();

        public T Instantiate<T>(T prefab, Transform parent = null, bool includeChild = true) where T : MonoBehaviour
            => ActiveContext.InstantiateWithDependency(prefab, parent, includeChild);

        public T Resolve<T>()
        {
            return ActiveContext.Resolve<T>();
        }
    }
}