using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

[Serializable]
public class GameBlackboard
{
    public Dictionary<string, object> entries;
    public GameBlackboard()
    {
        entries = new Dictionary<string, object>();
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        int dotIndex = key.IndexOf(".");
        string nestedObjectKey = string.Empty;
        bool nested = false;
        //만약 nested object라면
        if(dotIndex != -1)
        {
            nestedObjectKey = key.Substring(dotIndex + 1);
            nested = true;
            key = key.Substring(0, dotIndex);
        }

        if(entries.TryGetValue(key,out object v))
        {
            if (nested)
            {
                value = (T)GetNestedObject(nestedObjectKey, v);
                if(value is not null)
                {
                    return true;
                }
                return false;
            }

            // 직렬화된 객체가 JToken 형태로 저장되었을 수 있으므로, 이를 명시적으로 T 타입으로 변환합니다.
            value = (T)v;
            
            if (value is not null)
            {
                return true;
            }
        }
        value = default(T);
        return false;
    }
    object GetNestedObject(string keys, object parent)
    {
        if (keys == string.Empty) return null;

        //check if parent is null
        if(parent == null)
        {
            UnityEngine.Debug.LogError($"Could not acees nested Obj '{keys}' because parent obj null");
            return null;
        }
        string[] keyParts = keys.Split('.');

        object currentObject = parent;
        foreach(string keyPart in keyParts)
        {
            if(currentObject == null)
            {
                Debug.LogError($"property '{keys}' becuase parent obj in the path was null");
                return null;
            }

            // try to get the property using reflection
            var property = currentObject.GetType().GetProperty(keyPart);
            if (property != null)
            {
                currentObject = property.GetValue(currentObject);
            }
            else
            {
                // if the property is not found, try to get the field
                var field = currentObject.GetType().GetField(keyPart);
                if (field != null)
                {
                    currentObject = field.GetValue(currentObject);
                }
                else
                {
                    Debug.LogError($"Property or field '{keyPart}' not found in object of type '{currentObject.GetType().Name}'");
                    return null;
                }
            }
        }

        return currentObject;
    }
    public void SetValue<T>(string key, T value)
    {
        if (ContainsKey(key))
        {
            if(entries.TryGetValue(key, out object v))
            {

            }
            entries[key] = value;
            return;
        }

        entries.Add(key, value);

        Debug.Log("Add " + key + "  " + value);
    }
    
    //리스트 타입 값 가져오기나 초기화하기
    public List<T> GetOrInitList<T>(string key)
    {
        List<T> value = new List<T>();
        if(entries.TryGetValue(key, out object v))
        {
            value = (List<T>)v;
        }
        SetValue(key, value);
        return value;
    }

    public bool ContainsKey(string key) => entries.ContainsKey(key);
    public void RemoveKey(string key) => entries.Remove(key);

    public bool CompareValue(BlackboardCondition condition)
    {
        switch (condition.blackboardEntryData.valueType)
        {
            case BlackboardEntryData.ValueType.Bool:
                if(TryGetValue(condition.blackboardEntryData.keyName, out bool boolValue))
                {
                    return (boolValue == condition.blackboardEntryData.boolValue);
                }
                break;
            case BlackboardEntryData.ValueType.Int:
                if (TryGetValue(condition.blackboardEntryData.keyName, out int intValue))
                {
                    return CompareValueLogic(intValue, condition.blackboardEntryData.intValue, condition.comparison);
                }
                break;
            case BlackboardEntryData.ValueType.Float:
                if (TryGetValue(condition.blackboardEntryData.keyName, out float floatValue))
                {
                    return CompareValueLogic(floatValue, condition.blackboardEntryData.floatValue, condition.comparison);
                }
                break;
            case BlackboardEntryData.ValueType.String:
                if (TryGetValue(condition.blackboardEntryData.keyName, out string stringValue))
                {
                    return (stringValue == condition.blackboardEntryData.stringValue);
                }
                break;
            case BlackboardEntryData.ValueType.Vector3:
                if (TryGetValue(condition.blackboardEntryData.keyName, out Vector3 vector3Value))
                {
                    return (vector3Value == condition.blackboardEntryData.vector3Value);
                }
                break;
        }
        return false;
    }

    //비교 로직
    bool CompareValueLogic<T>(T value1, T value2, BlackboardCondition.ComparisonType comparisonType)
    {
        //숫자 아니거나 비교가 같지 않다면 틀린 비교
        if((typeof(T) != typeof(int) && typeof(T) != typeof(float)) && comparisonType != BlackboardCondition.ComparisonType.Equal)
        {
            Debug.LogError($"Invalid comparision for type: {typeof(T).Name} less than");
            return false;
        }

        switch (comparisonType)
        {
            case BlackboardCondition.ComparisonType.Equal:
                return value1.Equals(value2);
            case BlackboardCondition.ComparisonType.GreaterThanOrEqualTo:
                if(typeof(T) == typeof(int))
                {
                    return (int)(object)value1 >= (int)(object)value2;
                }else if(typeof(T) == typeof(float))
                {
                    return (float)(object)value1 >= (float)(object)value2;
                }
                else
                {
                    Debug.LogError($"Invalid comparision for type: {typeof(T).Name} greater or equal than");
                    return false;
                }
            case BlackboardCondition.ComparisonType.LessThanOrEqualTo:
                if (typeof(T) == typeof(int))
                {
                    return (int)(object)value1 <= (int)(object)value2;
                }
                else if (typeof(T) == typeof(float))
                {
                    return (float)(object)value1 <= (float)(object)value2;
                }
                else
                {
                    Debug.LogError($"Invalid comparision for type: {typeof(T).Name} less or equal than");
                    return false;
                }
            case BlackboardCondition.ComparisonType.GreaterThan:
                if (typeof(T) == typeof(int))
                {
                    return (int)(object)value1 > (int)(object)value2;
                }
                else if (typeof(T) == typeof(float))
                {
                    return (float)(object)value1 > (float)(object)value2;
                }
                else
                {
                    Debug.LogError($"Invalid comparision for type: {typeof(T).Name} greater than");
                    return false;
                }
            case BlackboardCondition.ComparisonType.LessThan:
                if (typeof(T) == typeof(int))
                {
                    return (int)(object)value1 < (int)(object)value2;
                }
                else if (typeof(T) == typeof(float))
                {
                    return (float)(object)value1 < (float)(object)value2;
                }
                else
                {
                    Debug.LogError($"Invalid comparision for type: {typeof(T).Name} less than");
                    return false;
                }
            default:
                Debug.LogError($"Invalid comparision for type: "+comparisonType);
                return false;
        }
    }
}
