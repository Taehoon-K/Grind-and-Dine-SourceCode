using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public struct BlackboardEntryData
{
    public string keyName;
    public enum ValueType { Int, Float, Bool, String, Vector3}

    public ValueType valueType;

    //각 유형에 맞게 값 저장
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public string stringValue;
    public Vector3 vector3Value;

    //Function to get value
    public object GetValue()
    {
        switch (valueType)
        {
            case ValueType.Int:
                return intValue;
            case ValueType.Float:
                return floatValue;
            case ValueType.Bool:
                return boolValue;
            case ValueType.String:
                return stringValue;
            case ValueType.Vector3:
                return vector3Value;
            default:
                return null;
        }
    }
}
