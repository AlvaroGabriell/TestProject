using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Light2D))]
public class LightConeBehaviour : MonoBehaviour
{
    private Light2D light2D;
    private int segments = 10;

    private PolygonCollider2D polygonCollider;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        GenerateCollider();
    }

    void GenerateCollider()
    {
        float radius = light2D.pointLightOuterRadius - 0.35f;
        float angle = light2D.pointLightOuterAngle - 10f;

        Vector2[] points = new Vector2[segments + 2];

        points[0] = Vector2.zero;

        float startAngle = -angle / 2f;
        float step = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + step * i;
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
            points[i + 1] = dir * radius;
        }

        polygonCollider.SetPath(0, points);
    }
}