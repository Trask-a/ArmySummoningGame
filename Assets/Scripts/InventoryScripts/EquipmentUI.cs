using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform itemsParent;
    EquipmentManager equipmentManager;
    EquipSlot[] slots;
    public GameObject UI;
    // Start is called before the first frame update
    void Start()
    {
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<EquipSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UI.SetActive(!UI.activeSelf);
        }
    }

    void UpdateUI(Equipment newItem, Equipment oldItem, int slot)
    {
        if(newItem != null)
        {
            Debug.Log("Update UI by adding Item");
            slots[slot].AddItem(newItem);
        }
        else
        {
            Debug.Log("Update UI by deleting Item");
            slots[slot].ClearSlot();
        }
        
    }
}
