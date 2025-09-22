# SimpleDI — Dependency Injection for Unity  

I like the Service Locator pattern, but its main downside is hidden dependencies.  
So I decided to get rid of them — and ended up with a very simple DI container.  

This is a small side project to gain at least a surface-level understanding of how dependency injection works in Unity and how to properly manage dependencies.  

---

## Installation  

Just copy the **SimpleDI** folder into your Unity project.  
No extra setup, no external packages required.  

---

## Key Features  

- **Register only instances** — you cannot register just types.  
- **No lifetimes (Singleton / Scoped / Transient):**  
  - **Singleton →** register in the global context.  
  - **Scoped →** register in a child context.  
  - **Transient →** no registration needed, just create with:  

```csharp
public T CreateInstance<T>() 
    => ActiveContext.CreateWithDependencies<T>();
```
or instantiate prefabs with:
```csharp
public T Instantiate<T>(T prefab, Transform parent = null, bool includeChildren = true)
    where T : MonoBehaviour
    => ActiveContext.InstantiateWithDependency(prefab, parent, includeChildren)
```

(These are just wrappers around DependencyContext methods.)

**Child contexts** — allow you to create local dependencies that don’t interfere with global ones.

 **[Dependency]** attribute — injects dependencies into fields, properties, methods, or constructors.

**DependencyFactory** — a helper that encapsulates global and child contexts and makes object creation / prefab instantiation simpler.
(You can write your own wrapper if you prefer.)

**Resolve<T>()** — resolves and injects dependencies into an object on its first resolve call.

**Supports Unity prefabs & components** — dependencies are automatically injected into all components on a prefab, their siblings, and children.

---

# Public API

## DependencyContext

**`Register<T>(T instance)`** -	Registers an instance of type T in the current context.

**`RegisterWithDontDestroyOnLoad<T>(T instance)`** - Registers the instance and marks its GameObject as DontDestroyOnLoad.

**`Resolve<T>()`** - Returns a registered object of type T. If not found, it checks the parent context. Injects dependencies on first resolve.

**`CreateChild()`**	- Creates a new child context and automatically assigns the current context as its parent.

**`CreateWithDependencies<T>()`**	- Creates an object of type T via its constructor, injecting all required dependencies.

**`InstantiateWithDependency<T>(T prefab, Transform parent = null, bool includeChildren = true)`**	- Instantiates the prefab and injects dependencies into all of its components, siblings, and children.

## DependencyFactory (This is just a wrapper around DependencyContext to make code easier)


**`SetChildContext(DependencyContext child)`** -	Sets the child context that will be used for object creation.

**`CreateInstance<T>()`** -	Wrapper over **ActiveContext.CreateWithDependencies<T>()**.

**`Instantiate<T>(T prefab, Transform parent = null, bool includeChildren = true)`** -	Wrapper over **ActiveContext.InstantiateWithDependency(...)**.

**`Resolve<T>()`** -	Wrapper over **ActiveContext.Resolve<T>()**.
