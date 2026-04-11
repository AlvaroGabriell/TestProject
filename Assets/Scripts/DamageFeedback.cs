using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(AttributesSystem))]
public class DamageFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private HealthSystem health;
    private AttributesSystem attributes;

    private Coroutine invulnerabilityCoroutine;

    void Awake()
    {
        health = GetComponent<HealthSystem>();
        attributes = GetComponent<AttributesSystem>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        health.OnDamageTaken += OnDamageTaken;
    }

    void OnDestroy()
    {
        health.OnDamageTaken -= OnDamageTaken;
    }

    private void OnDamageTaken(float damage, DamageSource source)
    {
        float duration = attributes.invulnerabilityTime.FinalValue;

        if (invulnerabilityCoroutine != null) StopCoroutine(invulnerabilityCoroutine);
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(duration));
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        health.isInvulnerable = true;

        Color original = spriteRenderer.color;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;
        float blinkInterval = 0.1f;

        Color gray = new(0.6f, 0.6f, 0.6f, original.a);

        spriteRenderer.color = gray;

        while (elapsed < (duration - 0.1f))
        {
            spriteRenderer.color = gray;
            yield return new WaitForSeconds(blinkInterval);
        
            spriteRenderer.color = original;
            yield return new WaitForSeconds(blinkInterval);
        
            elapsed += blinkInterval * 2;
        }

        spriteRenderer.color = original;
        health.isInvulnerable = false;
        invulnerabilityCoroutine = null;
    }
}
