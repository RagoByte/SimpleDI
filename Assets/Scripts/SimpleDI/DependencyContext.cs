using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleDI
{
    public class DependencyContext : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _registry = new();
        private readonly HashSet<object> _dependenciesAppliedObjects = new();
        private DependencyContext _parent;

        private static readonly Dictionary<Type, CachedDependencyInfo> _dependencyCache = new();
        private static readonly Dictionary<Type, ConstructorInfo> _constructorCache = new();

        private readonly struct CachedDependencyInfo
        {
            public readonly FieldInfo[] Fields;
            public readonly PropertyInfo[] Properties;
            public readonly MethodInfo[] Methods;

            public CachedDependencyInfo(FieldInfo[] fields, PropertyInfo[] properties, MethodInfo[] methods)
            {
                Fields = fields;
                Properties = properties;
                Methods = methods;
            }
        }

        public void Register<TInterface>(TInterface instance)
        {
            _registry[typeof(TInterface)] = instance;
        }

        public void RegisterWithDontDestroyOnLoad<T>(T instance) where T : MonoBehaviour
        {
            Register(instance);
            DontDestroyOnLoad(instance.gameObject);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        private object Resolve(Type type)
        {
            if (_registry.TryGetValue(type, out var instance))
            {
                ApplyDependenciesOnce(instance);
                return instance;
            }

            if (_parent != null)
                return _parent.Resolve(type);

            throw new Exception($"Type {type.FullName} not registered.");
        }

        public DependencyContext CreateChild()
        {
            var childGO = new GameObject("DependencyContext_Child");
            var child = childGO.AddComponent<DependencyContext>();
            child.SetParent(this);
            return child;
        }

        private void SetParent(DependencyContext parent) => _parent = parent;

        public T CreateWithDependencies<T>()
        {
            var type = typeof(T);
            var constructor = GetConstructor(type);

            var parameters = constructor.GetParameters();
            object[] args = parameters.Length > 0 ? new object[parameters.Length] : Array.Empty<object>();
            for (int i = 0; i < parameters.Length; i++)
                args[i] = Resolve(parameters[i].ParameterType);

            var instance = (T)constructor.Invoke(args);
            ApplyDependenciesOnce(instance);

            return instance;
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            if (_constructorCache.TryGetValue(type, out var ctor))
                return ctor;

            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            ctor = constructors.FirstOrDefault(c => Attribute.IsDefined(c, typeof(DependencyAttribute)))
                   ?? constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (ctor == null)
                throw new Exception($"No constructor found for {type.Name}");

            _constructorCache[type] = ctor;
            return ctor;
        }

        public T InstantiateWithDependency<T>(T prefab, Transform parent = null, bool includeChildren = true) where T : Component
        {
            T instance = Object.Instantiate(prefab, parent);
            Component[] components = includeChildren
                ? instance.GetComponentsInChildren<Component>(includeInactive: true)
                : new Component[] { instance };

            for (int i = 0; i < components.Length; i++)
                ApplyDependenciesOnce(components[i]);

            return instance;
        }

        private void ApplyDependenciesOnce(object target)
        {
            if (!_dependenciesAppliedObjects.Add(target))
                return;

            var dependenciesInfo = GetCachedDependencyInfo(target.GetType());

            for (int i = 0; i < dependenciesInfo.Fields.Length; i++)
                dependenciesInfo.Fields[i].SetValue(target, Resolve(dependenciesInfo.Fields[i].FieldType));

            for (int i = 0; i < dependenciesInfo.Properties.Length; i++)
                dependenciesInfo.Properties[i].SetValue(target, Resolve(dependenciesInfo.Properties[i].PropertyType));

            for (int i = 0; i < dependenciesInfo.Methods.Length; i++)
            {
                var method = dependenciesInfo.Methods[i];
                var parameters = method.GetParameters();
                object[] args = parameters.Length > 0 ? new object[parameters.Length] : Array.Empty<object>();
                for (int j = 0; j < parameters.Length; j++)
                    args[j] = Resolve(parameters[j].ParameterType);
                method.Invoke(target, args);
            }
        }

        private static CachedDependencyInfo GetCachedDependencyInfo(Type type)
        {
            if (_dependencyCache.TryGetValue(type, out var cached))
                return cached;

            var fields = new List<FieldInfo>();
            var props = new List<PropertyInfo>();
            var methods = new List<MethodInfo>();

            var t = type;
            var hierarchy = new Stack<Type>();
            while (t != null && t != typeof(MonoBehaviour))
            {
                hierarchy.Push(t);
                t = t.BaseType;
            }

            while (hierarchy.Count > 0)
            {
                var current = hierarchy.Pop();
                fields.AddRange(current.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(f => Attribute.IsDefined(f, typeof(DependencyAttribute))));

                props.AddRange(current.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => Attribute.IsDefined(p, typeof(DependencyAttribute)) && p.CanWrite));

                methods.AddRange(current.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => Attribute.IsDefined(m, typeof(DependencyAttribute))));
            }

            cached = new CachedDependencyInfo(fields.ToArray(), props.ToArray(), methods.ToArray());
            _dependencyCache[type] = cached;
            return cached;
        }
    }
}