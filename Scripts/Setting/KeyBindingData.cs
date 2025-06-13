using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyBinding
{
    public string actionName; // 동작 이름
    public KeyCode keyCode;   // 키 코드
}

[System.Serializable]
public class KeyBindingData
{
    public List<KeyBinding> bindings = new List<KeyBinding>();
}
