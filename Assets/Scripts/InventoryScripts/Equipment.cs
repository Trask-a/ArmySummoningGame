using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public float healthModifier;
    public float damageModifier;
    public float attackSpeedModifier;
    public EquipmentSlot equipmentSlot;

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}

public enum EquipmentSlot { 
    Head,
    Arms,
    Chest,
    Necklace,
    Ring1,
    Legs,
    Ring2,
    Feet,
    Weapon
}
