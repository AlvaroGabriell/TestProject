using System;
using Unity.VisualScripting;
using UnityEngine;

public enum PatrolDirection
{
    None,
    Horizontal,
    Vertical,
    Left,
    Right,
    Up,
    Down
}

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

    [Header("Patrol Settings")]
    public PatrolDirection patrolDirection = PatrolDirection.Horizontal;
    public float patrolRange = 2f;

    private Vector2 startPosition;

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
        attributes.moveSpeed.SetBaseValue(1f);
        attributes.attackDamage.SetBaseValue(5f);
        attributes.criticalChance.SetPercentValue(0f);

        if (player == null) player = Utils.GetPlayer();
    }

    void Start()
    {
        startPosition = rb.position; // salva a posição inicial aqui
        health.OnDeath += OnDeath;
        OnEnemySpawn?.Invoke(this);
    }

    void FixedUpdate()
{
    Vector2 targetPos;

    switch (patrolDirection)
    {
        case PatrolDirection.Horizontal:
            targetPos = new Vector2(
                startPosition.x + Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue) * patrolRange,
                startPosition.y
            );
            break;

        case PatrolDirection.Vertical:
            targetPos = new Vector2(
                startPosition.x,
                startPosition.y + Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue) * patrolRange
            );
            break;

        case PatrolDirection.Left:
            targetPos = new Vector2(
                startPosition.x - Mathf.Abs(Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue)) * patrolRange,
                startPosition.y
            );
            break;

        case PatrolDirection.Right:
            targetPos = new Vector2(
                startPosition.x + Mathf.Abs(Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue)) * patrolRange,
                startPosition.y
            );
            break;

        case PatrolDirection.Up:
            targetPos = new Vector2(
                startPosition.x,
                startPosition.y + Mathf.Abs(Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue)) * patrolRange
            );
            break;

        case PatrolDirection.Down:
            targetPos = new Vector2(
                startPosition.x,
                startPosition.y - Mathf.Abs(Mathf.Sin(Time.fixedTime * attributes.moveSpeed.FinalValue)) * patrolRange
            );
            break;

        default:
            targetPos = rb.position;
            break;
    }

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