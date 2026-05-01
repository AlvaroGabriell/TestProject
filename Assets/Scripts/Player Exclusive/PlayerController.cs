using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(AttributesSystem))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private HealthSystem health;
    [HideInInspector]
    public AttributesSystem attributes;

    private Vector2 movement = Vector2.zero;

    public static event Action OnPlayerDeath;

    private Coroutine damageBlinkCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
        attributes = GetComponent<AttributesSystem>();

        // Setting attributes
        attributes.maxHealth.SetBaseValue(20f);
        attributes.healthRegen.SetBaseValue(0f);
        attributes.regenSpeed.SetBaseValue(2f);
        attributes.moveSpeed.SetBaseValue(4f);
        attributes.attackDamage.SetBaseValue(6.5f);
        attributes.attackSpeed.SetBaseValue(1.5f);
        attributes.projectileSpeed.SetBaseValue(8f);
        attributes.criticalChance.SetPercentValue(5f);
        attributes.criticalMultiplier.SetBaseValue(2f);
        attributes.pickupRange.SetBaseValue(3f);
        attributes.invulnerabilityTime.SetBaseValue(1f);

        health.attributes = attributes;
        health.SetMaxHealthAndFullHeal(attributes.maxHealth.FinalValue);

        health.OnDeath += OnDeath;
        health.OnDamageTaken += OnDamageTaken;
    }

    void OnDestroy()
    {
        health.OnDeath -= OnDeath;
        health.OnDamageTaken -= OnDamageTaken;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDeath(DamageSource source)
    {
        gameObject.transform.position = new Vector3(0f, -5.26f, 0f);
        health.Revive();

        OnPlayerDeath?.Invoke();
    }
    private void OnDamageTaken(float damage, DamageSource source)
    {
        if(damageBlinkCoroutine != null) StopCoroutine(damageBlinkCoroutine);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<EnemyBehaviour>() != null)
            {
                health.TakeDamage(collision.gameObject.GetComponent<EnemyBehaviour>().GetEnemyDamage(), DamageSource.ENEMY);
                //enemy.GetComponent<HealthSystem>().Kill(DamageSource.PLAYER);
            } else if(collision.gameObject.transform.parent.GetComponent<EnemyBehaviour>() != null)
            {
                health.TakeDamage(collision.gameObject.transform.parent.GetComponent<EnemyBehaviour>().GetEnemyDamage(), DamageSource.ENEMY);
                //collision.gameObject.transform.parent.GetComponent<HealthSystem>().Kill(DamageSource.PLAYER);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) OnTriggerEnter2D(collision);
    }

    //Calcula e executa o movimento do jogador
    public void HandleMovement()
    {
        rb.linearVelocity = movement * attributes.moveSpeed.FinalValue;

        if (movement.x > 0f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (movement.x < 0f) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    //Captura o input de movimento
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
}