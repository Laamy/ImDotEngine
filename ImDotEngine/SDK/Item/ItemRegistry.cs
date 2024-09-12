using System;
using System.Collections.Concurrent;

static class ItemRegistry
{
    // these items wont actually be used (stupid)
    private static ConcurrentDictionary<string, BaseItem> items = new ConcurrentDictionary<string, BaseItem>();

    public static void Initialize()
    {
        AddItem<DirtBlock>();
    }

    // why does C# make this harder then C++ templates
    private static void AddItem<T>() where T : BaseItem
    {
        // WTF? C#??????? WHAT?????????
        var item = (T)Activator.CreateInstance(typeof(T));

        if (!items.TryAdd(item.ID, item))
            throw new Exception($"Failed to add item {item.ID}");
    }

    public static BaseItem GetItemByID(string ID)
    {
        if (items.ContainsKey(ID))
            return items[ID];

        throw new Exception($"Item {ID} doesnt exist");
    }

    public static BaseItem Create<T>() where T : BaseItem, new()
    {
        return new T();
    }

    public static BaseItem Create(string id)
    {
        BaseItem item = GetItemByID(id);

        if (item != null)
            return Activator.CreateInstance(item.GetType()) as BaseItem;

        throw new Exception($"Item {id} doesnt exist");
    }
}