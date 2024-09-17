using System;
using System.Collections.Generic;

class SimpleRegistry
{
    private Dictionary<Type, Dictionary<int, IEntityComponent>> componentStore = new Dictionary<Type, Dictionary<int, IEntityComponent>>();
    private Dictionary<int, HashSet<Type>> entityComponents = new Dictionary<int, HashSet<Type>>();

    private int nextEntityId = 1;

    /// <summary>
    /// Create a new entity inside of the registry
    /// </summary>
    /// <returns>EntityID</returns>
    public int CreateEntity()
    {
        int entityId = nextEntityId++;
        entityComponents[entityId] = new HashSet<Type>();

        return entityId;
    }

    /// <summary>
    /// Emplace a component inside of an entity
    /// </summary>
    public void Emplace<T>(int entityId, T component) where T : IEntityComponent
    {
        Type type = typeof(T);

        if (!componentStore.ContainsKey(type))
            componentStore[type] = new Dictionary<int, IEntityComponent>();

        componentStore[type][entityId] = component;
        entityComponents[entityId].Add(type);
    }

    /// <summary>
    /// Check if an entity has a component
    /// </summary>
    public bool HasComponent<T>(int entityId) where T : IEntityComponent
    {
        Type type = typeof(T);

        return componentStore.ContainsKey(type) && componentStore[type].ContainsKey(entityId);
    }

    /// <summary>
    /// Remove an emplaced component from inside an entity
    /// </summary>
    public void Remove<T>(int entityId) where T : IEntityComponent
    {
        Type type = typeof(T);

        if (componentStore.ContainsKey(type))
        {
            if (componentStore[type].ContainsKey(entityId))
            {
                componentStore.Remove(type);

                if (entityComponents.ContainsKey(entityId))
                {
                    entityComponents[entityId].Remove(type);

                    // might add a cleanup for entities not sure yet though
                    //if (entityComponents[entityId].Count == 0)
                    //    entityComponents.Remove(entityId);
                }
            }
        }
    }

    /// <summary>
    /// Attempt to get a component from an entity
    /// </summary>
    public bool Try_Get<T>(int entityId, out T component) where T : IEntityComponent
    {
        Type type = typeof(T);

        if (componentStore.TryGetValue(type, out var store) && store.TryGetValue(entityId, out var comp))
        {
            component = (T)comp;
            return true;
        }

        component = null;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<int> EntitiesWithComponents(params Type[] componentTypes)
    {
        HashSet<int> entities = new HashSet<int>();

        foreach (var componentType in componentTypes)
        {
            if (componentStore.TryGetValue(componentType, out var store))
            {
                foreach (var entityId in store.Keys)
                {
                    if (entities.Contains(entityId) || AllComponentsPresent(entityId, componentTypes))
                        entities.Add(entityId);
                }
            }
        }

        return entities;
    }

    private bool AllComponentsPresent(int entityId, Type[] componentTypes)
    {
        foreach (var componentType in componentTypes)
        {
            if (!entityComponents[entityId].Contains(componentType))
                return false;
        }

        return true;
    }

    #region lazy stuff for concept
    public IEnumerable<(int entityId, T1 component1)> GetComponents<T1>() where T1 : IEntityComponent
    {
        Type type1 = typeof(T1);

        if (componentStore.TryGetValue(type1, out var store))
        {
            foreach (var kvp in store)
                yield return (kvp.Key, (T1)kvp.Value);
        }
    }

    public IEnumerable<(int entityId, T1 component1, T2 component2)> GetComponents<T1, T2>()
        where T1 : IEntityComponent
        where T2 : IEntityComponent
    {
        Type type1 = typeof(T1);
        Type type2 = typeof(T2);

        foreach (var entityId in EntitiesWithComponents(type1, type2))
        {
            if (Try_Get(entityId, out T1 comp1) && Try_Get(entityId, out T2 comp2))
                yield return (entityId, comp1, comp2);
        }
    }

    public IEnumerable<(int entityId, T1 component1, T2 component2, T3 component3)> GetComponents<T1, T2, T3>()
        where T1 : IEntityComponent
        where T2 : IEntityComponent
        where T3 : IEntityComponent
    {
        Type type1 = typeof(T1);
        Type type2 = typeof(T2);
        Type type3 = typeof(T3);

        foreach (var entityId in EntitiesWithComponents(type1, type2, type3))
        {
            if (Try_Get(entityId, out T1 comp1) && Try_Get(entityId, out T2 comp2) && Try_Get(entityId, out T3 comp3))
                yield return (entityId, comp1, comp2, comp3);
        }
    }
    #endregion

    private bool TryGetComponent(int entityId, Type type, out object component)
    {
        if (componentStore.TryGetValue(type, out var store) && store.TryGetValue(entityId, out var comp))
        {
            component = comp;
            return true;
        }

        component = null;
        return false;
    }
}