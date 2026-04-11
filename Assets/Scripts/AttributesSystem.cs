using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributesSystem : MonoBehaviour
{
    public ScalableAttribute maxHealth;
    public ScalableAttribute healthRegen;
    public ScalableAttribute regenSpeed;
    public ScalableAttribute moveSpeed;
    public ScalableAttribute attackDamage;
    public ScalableAttribute attackSpeed;
    public ScalableAttribute projectileSpeed;
    public ScalableAttribute criticalChance;
    public ScalableAttribute criticalMultiplier;
    public ScalableAttribute pickupRange;
    public ScalableAttribute invulnerabilityTime;

    private Dictionary<Attribute, ScalableAttribute> attributes;

    public event Action<Attribute, ScalableAttribute> OnAttributeChanged;

    void Awake()
    {
        attributes = new Dictionary<Attribute, ScalableAttribute>
        {
            { Attribute.maxHealth, maxHealth },
            { Attribute.healthRegen, healthRegen },
            { Attribute.regenSpeed, regenSpeed },
            { Attribute.moveSpeed, moveSpeed },
            { Attribute.attackDamage, attackDamage },
            { Attribute.attackSpeed, attackSpeed },
            { Attribute.projectileSpeed, projectileSpeed },
            { Attribute.criticalChance, criticalChance },
            { Attribute.criticalMultiplier, criticalMultiplier },
            { Attribute.pickupRange, pickupRange },
            { Attribute.invulnerabilityTime, invulnerabilityTime }
        };

        foreach (var kvp in attributes)
        {
            Attribute type = kvp.Key;
            ScalableAttribute attr = kvp.Value;

            attr.OnAttributeChanged += (changedAttr) => OnAttributeChanged?.Invoke(type, changedAttr);
        }
    }

    public ScalableAttribute GetAttributeByType(Attribute attr)
    {
        return attributes[attr];
    }

    public Dictionary<Attribute, ScalableAttribute> GetAttributeDictionary()
    {
        return attributes;
    }
}

[System.Serializable]
public class ScalableAttribute
{
    public event Action<ScalableAttribute> OnAttributeChanged;

    public float baseValue = 1;
    public float modifier = 1f; // 1 = 100%

    public float FinalValue => baseValue * modifier;

    public void ApplyBaseUpgrade(float amount)
    {
        baseValue += amount;
        OnAttributeChanged?.Invoke(this);
    }
    public void SetBaseValue(float amount)
    {
        baseValue = amount;
        OnAttributeChanged?.Invoke(this);
    }
    public float GetBaseValue()
    {
        return baseValue;
    }
    public void ApplyPercentUpgrade(float percent)
    {
        modifier *= 1f + (percent / 100f);
        OnAttributeChanged?.Invoke(this);
    }
    public void SetPercentValue(float percent)
    {
        modifier = percent / 100f;
        OnAttributeChanged?.Invoke(this);
    }
    public float GetPercentValue()
    {
        return modifier * 100f;
    }
}

public enum Attribute
{
    maxHealth,
    healthRegen,
    regenSpeed,
    moveSpeed,
    attackDamage,
    attackSpeed,
    projectileSpeed,
    criticalChance,
    criticalMultiplier,
    pickupRange,
    invulnerabilityTime
}