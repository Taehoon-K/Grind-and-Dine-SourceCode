using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelStick : MonoBehaviour
{
    public RectTransform needle;

    private void Update()
    {
        UpdateNeedle();
    }
    void UpdateNeedle()
    {
        if (gameObject.activeSelf)
        {
            float t = Mathf.InverseLerp(0, 1f, GasStationManager.instance.currentFuelGauge);
            float angle = Mathf.Lerp(112, -72f, t); // 여기서 반대로!
            needle.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
