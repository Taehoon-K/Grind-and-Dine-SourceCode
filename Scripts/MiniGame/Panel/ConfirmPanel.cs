using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanel : MonoBehaviour
{
    bool cursorOn;
    private void OnEnable()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            cursorOn = true;
        }
        Time.timeScale = 0;
    }
    private void OnDisable()
    {
        if (cursorOn)
        {
            Cursor.visible = false;
            cursorOn = false;
        }
        Time.timeScale = 1;
    }
}
