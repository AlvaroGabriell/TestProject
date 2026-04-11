using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Fillables")]
    public GameObject hpObject;
    public RectMask2D hpMask;

    void Update()
    {
        if(Utils.GetPlayer().IsUnityNull()) return;
        SetFill(hpMask, Utils.GetPlayer().GetComponent<HealthSystem>().GetHealth(), Utils.GetPlayer().GetComponent<HealthSystem>().GetMaxHealth());
    }

    void LateUpdate()
    {
        Vector3 worldPos = Utils.GetPlayer().transform.position + new Vector3(0, 0.8f, 0);

        hpObject.transform.position = worldPos;
    }

    private void SetFill(RectMask2D mask, float actualValue, float maxValue)
    {
        float fullWidth = mask.gameObject.GetComponent<RectTransform>().rect.width;
        float padding = fullWidth * (1 - (actualValue / maxValue));
        mask.padding = new Vector4(0, 0, padding, 0);
    }
}