using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/** <summary>
* Script da vida. Seta um valor de vida máximo para o objeto, com variáveis pra controlar se
* o objeto pode tomar dano, pode morrer e se está vivo. Tem também métodos pra mudar a vida,
* pegar, curar e dar dano. 
* </summary> **/
public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    private float health; 
    [SerializeField] private float maxHealth = 20f;
    public bool canDie = true, canRegen = true, regenActive = false, isAlive = true, isInvulnerable = false;
    public AttributesSystem attributes;

    private Coroutine regenCoroutine;

    public event Action<DamageSource> OnDeath;
    public event Action<float, DamageSource> OnDamageTaken;
    public event Action OnRevive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.GetComponent<AttributesSystem>() != null) SetMaxHealthAndFullHeal(gameObject.GetComponent<AttributesSystem>().maxHealth.FinalValue);
        else health = maxHealth;
    }

    void Update()
    {
        if(maxHealth != attributes.maxHealth.FinalValue) SetMaxHealthAndFullHeal(attributes.maxHealth.FinalValue);
    }

    public void SetMaxHealth(float pMaxHealth)
    {
        maxHealth = pMaxHealth;
    }
    public void SetMaxHealthAndFullHeal(float pMaxHealth)
    {
        maxHealth = pMaxHealth;
        HealFullHealth();
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetHealth(float pHealth)
    {
        health = pHealth;
    }

    public void TakeDamage(float pDamage, DamageSource source)
    {
        if (isInvulnerable || !isAlive) return;

        health = Mathf.Max(health - pDamage, 0);
        OnDamageTaken?.Invoke(pDamage, source);

        if (ShouldDie() && canDie == true) Die(source);
    }

    public void Kill(DamageSource source)
    {
        if(!isAlive) return;

        Die(source);
    }

    private void Die(DamageSource source)
    {
        isAlive = false;

        OnDeath?.Invoke(source);
    }

    public void Revive()
    {
        isAlive = true;
        HealFullHealth();
        OnRevive?.Invoke();
    }

    public void HealHealth(float pHealing)
    {
        health = Mathf.Min(health + pHealing, maxHealth);
    }
    public void HealFullHealth()
    {
        health = maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool ShouldDie()
    {
        return health <= 0;
    }

    public void StartRegen()
    {
        regenCoroutine = StartCoroutine(Regen());
    }
    public void StopRegen()
    {
        if(regenCoroutine != null) StopCoroutine(regenCoroutine);
    }
    
    // Should not be called in non-player object, although it is possible.
    private IEnumerator Regen()
    {
        while (true)
        {
            if (isAlive && canRegen)
            {
                HealHealth(attributes.healthRegen.FinalValue);
            }

            float regenInterval = Mathf.Max(4f / attributes.regenSpeed.FinalValue, 0f);
            yield return new WaitForSeconds(regenInterval);
        }
    }
}

public enum DamageSource
{
    PLAYER,
    ENEMY,
    ENVIRONMENT,
    SELF,
    VOID
}