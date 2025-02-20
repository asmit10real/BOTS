using UnityEngine;

[System.Serializable]
public class LootItem {
    public GameObject lootPrefab;
    public float dropChance; // 0.0 to 1.0 (e.g., 0.2 = 20%)
}

public class DropTable : MonoBehaviour {
    public LootItem[] lootItems;

    public GameObject GetRandomLoot() {
        float roll = Random.value; // Get random number 0-1
        float cumulativeChance = 0f;

        foreach (var loot in lootItems) {
            cumulativeChance += loot.dropChance;
            if (roll <= cumulativeChance) {
                return loot.lootPrefab;
            }
        }

        return null; // No loot if nothing passed the chance check
    }
}
