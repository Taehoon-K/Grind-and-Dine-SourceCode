using Michsky.UI.Dark;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindKey : MonoBehaviour
{
    [SerializeField] private InputActionReference actionToRemap;
    [SerializeField] private string bindingName;

    // Start is called before the first frame update
    public void Rebinding()
    {
        QualityManager.Instance.OnRebindRequest(actionToRemap, bindingName);
    }
}
