using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttributesSystem))]
[RequireComponent(typeof(HealthSystem))]
public class EnemyBehaviour : MonoBehaviour
{
    private GameObject player;
    private GameObject visualObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AttributesSystem attributes;
    private HealthSystem health;

    public static event Action<EnemyBehaviour, DamageSource> OnEnemyDeath;
    public static event Action<EnemyBehaviour> OnEnemySpawn;

    void Awake()
    {
        visualObject = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = visualObject.GetComponent<SpriteRenderer>();
        attributes = GetComponent<AttributesSystem>();
        health = GetComponent<HealthSystem>();
        health.attributes = attributes;

        attributes.maxHealth.SetBaseValue(20f);
        attributes.healthRegen.SetBaseValue(0f);
        attributes.moveSpeed.SetBaseValue(2f);
        attributes.attackDamage.SetBaseValue(5f);
        attributes.criticalChance.SetPercentValue(0f);

        if (player == null) player = Utils.GetPlayer();
    }

    void Start()
    {
        health.OnDeath += OnDeath;
        OnEnemySpawn?.Invoke(this);
    }

    void FixedUpdate()
    {
        Vector2 targetPos = new Vector2(Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue) * 2, rb.position.y);
        rb.MovePosition(targetPos);
    }

    void OnDestroy()
    {
        health.OnDeath -= OnDeath;
    }

    private void OnDeath(DamageSource source)
    {
        OnEnemyDeath.Invoke(this, source);
        Destroy(gameObject);
    }

    public float GetEnemyDamage()
    {
        return attributes.attackDamage.FinalValue;
    }
}