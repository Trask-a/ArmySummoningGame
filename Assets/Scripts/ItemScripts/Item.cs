using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public Rarity rarity;
    public string description = "Its an item";

    public virtual void Use()
    {
        // use the item
        Debug.Log("Using " + name);
        // meant to be overriden
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}

public enum Rarity
{
    common,
    rare,
    epic,
    legendary
}
