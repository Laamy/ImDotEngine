using SFML.Graphics;

abstract class BaseItem
{
    public string Name { get; set; }
    public string ID { get; set; }

#if CLIENT
    // texture stuff
    public Texture Texture { get; set; } = null; // i really hope this doesnt clone
#endif

    public BaseItem(string name, string id)
    {
        this.Name = name;
        this.ID = id;
    }
}