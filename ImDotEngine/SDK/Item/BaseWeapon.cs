class BaseWeapon : BaseItem
{
    public int Damage { get; set; }

    public BaseWeapon(string name, string id, int damage) : base(name, id)
    {
        this.Damage = damage;
    }

    public virtual void Use() { }
}