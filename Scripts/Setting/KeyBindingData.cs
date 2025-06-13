using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyBinding
{
    public string actionName; // ���� �̸�
    public KeyCode keyCode;   // Ű �ڵ�
}

[System.Serializable]
public class KeyBindingData
{
    public List<KeyBinding> bindings = new List<KeyBinding>();
}
