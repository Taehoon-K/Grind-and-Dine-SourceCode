using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    /*
    public static KeyBindingManager Instance; // �̱������� ���
    private string saveKey = "KeyBindings";   // PlayerPrefs Ű
    public KeyBindingData keyBindingData = new KeyBindingData();

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        LoadKeyBindings(); // ���� �� Ű ���ε� �ε�
    }

    // Ű ���ε� ����
    public void SetKeyBinding(string actionName, KeyCode newKey)
    {
        foreach (var binding in keyBindingData.bindings)
        {
            if (binding.actionName == actionName)
            {
                binding.keyCode = newKey;
                SaveKeyBindings();
                return;
            }
        }

        // �ش� �׼��� ������ ���� �߰�
        keyBindingData.bindings.Add(new KeyBinding { actionName = actionName, keyCode = newKey });
        SaveKeyBindings();
    }

    // Ư�� ������ Ű ��ȯ
    public KeyCode GetKeyBinding(string actionName)
    {
        foreach (var binding in keyBindingData.bindings)
        {
            if (binding.actionName == actionName)
                return binding.keyCode;
        }

        return KeyCode.None; // �⺻��
    }

    // Ű ���ε� ����
    public void SaveKeyBindings()
    {
        string json = JsonUtility.ToJson(keyBindingData);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    // Ű ���ε� �ε�
    public void LoadKeyBindings()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            keyBindingData = JsonUtility.FromJson<KeyBindingData>(json);
        }
        else
        {
            // �⺻ ���ε� ����
            keyBindingData.bindings = new List<KeyBinding>
            {
                new KeyBinding { actionName = "MoveForward", keyCode = KeyCode.W },
                new KeyBinding { actionName = "MoveBackward", keyCode = KeyCode.S },
                new KeyBinding { actionName = "MoveLeft", keyCode = KeyCode.A },
                new KeyBinding { actionName = "MoveRight", keyCode = KeyCode.D },
                new KeyBinding { actionName = "Jump", keyCode = KeyCode.Space },
                new KeyBinding { actionName = "Sprint", keyCode = KeyCode.LeftShift }
            };
            SaveKeyBindings();
        }
    }*/
}

