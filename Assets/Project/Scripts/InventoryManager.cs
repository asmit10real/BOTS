using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public List<ItemData> inventory = new List<ItemData>();

    public bool AddItem(ItemData item) {
        if (inventory.Count >= 10) {
            Debug.Log("Inventory Full!");
            return false;
        }
        inventory.Add(item);
        Debug.Log($"Added {item.itemName} to inventory.");
        return true;
    }

    public void EquipItem(int index, PlayerStats playerStats) {
        if (index < 0 || index >= inventory.Count) return;

        ItemData item = inventory[index];

        if (!item.isEquippable) {
            Debug.Log($"{item.itemName} cannot be equipped!");
            return;
        }

        item.Equip(playerStats);

        // ✅ Display all stat changes in the log
        foreach (var stat in item.GetStatModifiers()) {
            Debug.Log($"Equipped {item.itemName}, adding {stat.Value} to {stat.Key}");
        }
    }

    public void UnequipItem(int index, PlayerStats playerStats) {
        if (index < 0 || index >= inventory.Count) return;

        ItemData item = inventory[index];

        if (!item.isEquippable) return;

        item.Unequip(playerStats);

        // ✅ Display all stat changes in the log
        foreach (var stat in item.GetStatModifiers()) {
            Debug.Log($"Unequipped {item.itemName}, removing {stat.Value} from {stat.Key}");
        }
    }
}
