using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YarnDialogueName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private string previousText;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();

        if (string.IsNullOrWhiteSpace(textMeshPro.text))
        {
            OnTextChanged(); // �ؽ�Ʈ ���� �� ȣ���� �Լ�
        }
    }

    private void Update()
    {
        if (string.IsNullOrWhiteSpace(textMeshPro.text))
        {
            OnTextChanged(); // �ؽ�Ʈ ���� �� ȣ���� �Լ�
        }
        else
        {
            image.enabled = true;
        }
    }

    private void OnTextChanged()
    {
        image.enabled = false;
    }
}
