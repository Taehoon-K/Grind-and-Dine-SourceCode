using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    /*
    public static KeyBindingManager Instance; // 싱글턴으로 사용
    private string saveKey = "KeyBindings";   // PlayerPrefs 키
    public KeyBindingData keyBindingData = new KeyBindingData();

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        LoadKeyBindings(); // 시작 시 키 바인딩 로드
    }

    // 키 바인딩 설정
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

        // 해당 액션이 없으면 새로 추가
        keyBindingData.bindings.Add(new KeyBinding { actionName = actionName, keyCode = newKey });
        SaveKeyBindings();
    }

    // 특정 동작의 키 반환
    public KeyCode GetKeyBinding(string actionName)
    {
        foreach (var binding in keyBindingData.bindings)
        {
            if (binding.actionName == actionName)
                return binding.keyCode;
        }

        return KeyCode.None; // 기본값
    }

    // 키 바인딩 저장
    public void SaveKeyBindings()
    {
        string json = JsonUtility.ToJson(keyBindingData);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    // 키 바인딩 로드
    public void LoadKeyBindings()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            keyBindingData = JsonUtility.FromJson<KeyBindingData>(json);
        }
        else
        {
            // 기본 바인딩 설정
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

