using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Fillables")]
    public GameObject detectionObject;
    public RectMask2D detectionMask;

    void Update()
    {
        if (Utils.GetPlayer().IsUnityNull()) return;

        float highestProgress = GetHighestDetectionProgress();
        SetFill(detectionMask, highestProgress);

        detectionObject.SetActive(highestProgress > 0f);
    }

    void LateUpdate()
    {
        if (Utils.GetPlayer().IsUnityNull()) return;

        Vector3 worldPos = Utils.GetPlayer().transform.position + new Vector3(0, 0.8f, 0);
        detectionObject.transform.position = worldPos;
    }

    private float GetHighestDetectionProgress()
    {
        LightConeBehaviour[] cones = FindObjectsByType<LightConeBehaviour>();

        float highest = 0f;
        foreach (LightConeBehaviour cone in cones)
        {
            if (cone.DetectionProgress > highest)
                highest = cone.DetectionProgress;
        }

        return highest;
    }

    private void SetFill(RectMask2D mask, float progress)
    {
        float fullWidth = mask.gameObject.GetComponent<RectTransform>().rect.width;
        float padding = fullWidth * (1f - Mathf.Clamp01(progress));
        mask.padding = new Vector4(0, 0, padding, 0);
    }
}