using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public Image icon;
    public Equipment equipment;
    public Button removeButton;
    public int equipmentSlot;

    public void AddItem(Equipment newItem)
    {
        equipment = newItem;

        icon.sprite = equipment.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        equipment = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        EquipmentManager.instance.Unequip(equipmentSlot);
    }
}
