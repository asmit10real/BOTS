using UnityEngine;
using System.Collections.Generic;
public enum ItemType {
    Head,
    Body,
    Arms,
    Gun,
    MiniBot,
    EnergyField,
    Wings,
    Shield,
    Shoulder,
    Flag,
    Booster,
    TransPack,
    ActiveSkill,
    PassiveSkill,
    Consumable,
    Mercenary,
    FieldPack,
    Miscellaneous
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject {
    public string itemName;
    public Sprite itemIcon;
    
    public ItemType itemType; // ✅ Useful for UI & sorting
    public bool isEquippable = true; // ✅ Defaults to true for most items

    public List<StatType> statTypes = new List<StatType>(); // ✅ Multiple stats
    public List<int> statValues = new List<int>(); // ✅ Corresponding values

    private Dictionary<StatType, int> statModifiers; // ✅ Cached for performance

    // 🚀 Initialize stat dictionary
    private void InitializeStatModifiers() {
        statModifiers = new Dictionary<StatType, int>();
        for (int i = 0; i < statTypes.Count; i++) {
            if (i < statValues.Count) {
                statModifiers[statTypes[i]] = statValues[i];
            }
        }
    }

    public Dictionary<StatType, int> GetStatModifiers() {
        if (statModifiers == null) {
            InitializeStatModifiers();
        }
        return statModifiers;
    }

    public void Equip(PlayerStats playerStats) {
        if (!isEquippable) return; // ❌ Skip if non-equippable
        
        foreach (var stat in GetStatModifiers()) {
            playerStats.ModifyStat(stat.Key, stat.Value);
        }
    }

    public void Unequip(PlayerStats playerStats) {
        if (!isEquippable) return; // ❌ Skip if non-equippable
        
        foreach (var stat in GetStatModifiers()) {
            playerStats.ModifyStat(stat.Key, -stat.Value);
        }
    }
}
