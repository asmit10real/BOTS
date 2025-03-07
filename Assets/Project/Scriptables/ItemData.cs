using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject {
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int value; // For currency items or sell price
    public int attackPower; // If applicable (for weapons)
}

public enum ItemType {
    Head, Body, Arms, Minibot, Aura, Currency, Misc
}
