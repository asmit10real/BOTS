using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;

    public int maxInventorySize = 10; // Default inventory size
    private List<ItemData> inventory = new List<ItemData>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool AddItem(ItemData item) {
        if (inventory.Count >= maxInventorySize) {
            Debug.Log("Inventory Full! Cannot add item.");
            return false;
        }
        inventory.Add(item);
        Debug.Log($"Added {item.itemName} to inventory!");
        return true;
    }

    public void RemoveItem(ItemData item) {
        if (inventory.Contains(item)) {
            inventory.Remove(item);
            Debug.Log($"Removed {item.itemName} from inventory.");
        }
    }

    public List<ItemData> GetInventoryItems() {
        return inventory;
    }
}
