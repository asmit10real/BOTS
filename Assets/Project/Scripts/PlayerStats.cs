using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    [Serializable]
    public struct StatValue {
        public float BaseValue;
        public float MinValue;
        public float MaxValue;

        public float GetRandomValue() {
            return UnityEngine.Random.Range(MinValue, MaxValue);
        }
    }

    [Serializable]
    public struct StatEntry {
        public StatType Stat;
        public float Value;
    }

    [Serializable]
    public struct AttackStatEntry {
        public StatType AttackType;
        public StatValue DamageRange;
    }

    // ✅ Visible in Inspector
    [Header("General Stats")]
    [SerializeField] private List<StatEntry> statEntries = new List<StatEntry>();

    [Header("Attack Stats")]
    [SerializeField] private List<AttackStatEntry> attackStatEntries = new List<AttackStatEntry>();

    // 🛠️ Dictionaries (For runtime usage)
    private Dictionary<StatType, float> Stats = new Dictionary<StatType, float>();
    private Dictionary<StatType, StatValue> AttackStats = new Dictionary<StatType, StatValue>();

    private void Awake() {
        InitializeStats();
    }

    private void InitializeStats() {
        // ✅ If statEntries is empty, initialize default values
        if (statEntries.Count == 0) {
            statEntries.Add(new StatEntry { Stat = StatType.HP, Value = 100 });
            statEntries.Add(new StatEntry { Stat = StatType.Evade, Value = 0 });
            statEntries.Add(new StatEntry { Stat = StatType.Critical, Value = 5 });
            statEntries.Add(new StatEntry { Stat = StatType.SpecialTrans, Value = 0 });
            statEntries.Add(new StatEntry { Stat = StatType.Speed, Value = 1.0f });
            statEntries.Add(new StatEntry { Stat = StatType.RangedAttack, Value = 0 });
            statEntries.Add(new StatEntry { Stat = StatType.Luck, Value = 0 });
            statEntries.Add(new StatEntry { Stat = StatType.TransSpeed, Value = 1.0f });
            statEntries.Add(new StatEntry { Stat = StatType.TransBotDefense, Value = 0 });
            statEntries.Add(new StatEntry { Stat = StatType.TransGauge, Value = 0 });
        }

        // ✅ If attackStatEntries is empty, initialize attack ranges
        if (attackStatEntries.Count == 0) {
            attackStatEntries.Add(new AttackStatEntry { 
                AttackType = StatType.AttackBasic, 
                DamageRange = new StatValue { BaseValue = 30, MinValue = 30, MaxValue = 35 } 
            });

            attackStatEntries.Add(new AttackStatEntry { 
                AttackType = StatType.AttackTrans, 
                DamageRange = new StatValue { BaseValue = 94, MinValue = 94, MaxValue = 110 } 
            });
        }

        // ✅ Populate the runtime dictionary
        Stats.Clear();
        foreach (var entry in statEntries) {
            Stats[entry.Stat] = entry.Value;
        }

        // ✅ Populate attack stats
        AttackStats.Clear();
        foreach (var entry in attackStatEntries) {
            AttackStats[entry.AttackType] = entry.DamageRange;
        }
    }

    public float GetStat(StatType type) {
        return Stats.ContainsKey(type) ? Stats[type] : 0;
    }

    public float GetRandomAttackDamage(StatType attackType) {
        return AttackStats.ContainsKey(attackType) ? AttackStats[attackType].GetRandomValue() : 0;
    }

    public void ModifyStat(StatType type, float amount) {
        if (Stats.ContainsKey(type)) {
            Stats[type] += amount;

            // ✅ Also update the inspector-visible list
            for (int i = 0; i < statEntries.Count; i++) {
                if (statEntries[i].Stat == type) {
                    statEntries[i] = new StatEntry { Stat = type, Value = Stats[type] };
                    break;
                }
            }
        }
    }
}
