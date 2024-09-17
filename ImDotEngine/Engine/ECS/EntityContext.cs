using System;
using System.Runtime.InteropServices;

/// <summary>
/// Util wrapper for SimpleRegsistry so you can just define this in your entities
/// </summary>
class EntityContext
{
    public EntityContext(SimpleRegistry Registry)
    {
        // store registry
        this.Registry = Registry;

        // create new entity & store its ID for this entitycontext
        EntityId = this.Registry.CreateEntity();
    }

    public SimpleRegistry Registry; //reference to the registry
    public int EntityId;

    /// <summary>
    /// Attempt to get the component from the entitycontext
    /// </summary>
    public T TryGetComponent<T>() where T : IEntityComponent
    {
        T component = null;

        this.Registry.Try_Get<T>(EntityId, out component);

        return component;
    }

    /// <summary>
    /// Remove a component from the entitycontext
    /// </summary>
    public void RemoveComponent<T>() where T : IEntityComponent
    {
        this.Registry.Remove<T>(EntityId);
    }

    /// <summary>
    /// Check if the entitycontext has a component
    /// </summary>
    public bool HasComponent<T>() where T : IEntityComponent
    {
        return this.Registry.HasComponent<T>(EntityId);
    }

    /// <summary>
    /// Attempts to get a component from the entitycontext if it exists, else it'll add it then return that
    /// </summary>
    public void TryGetOrAddComponent<T>(out T component) where T : IEntityComponent, new()
    {
        if (!this.HasComponent<T>())
            this.EmplaceComponent<T>();

        this.Registry.Try_Get<T>(EntityId, out component);
    }

    /// <summary>
    /// Emplace a component into the entitycontext
    /// </summary>
    public void EmplaceComponent<T>() where T : IEntityComponent, new()
    {
        this.Registry.Emplace<T>(EntityId, new T());
    }

    /// <summary>
    /// Toggle a flag component
    /// </summary>
    public void TryToggleFlag<T>() where T : IEntityComponent, new()
    {
        if (this.HasComponent<T>())
            this.RemoveComponent<T>();
        else this.EmplaceComponent<T>();
    }
}