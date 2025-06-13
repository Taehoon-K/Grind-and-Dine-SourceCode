using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    static readonly string[] FILEPATH = { 
        Application.persistentDataPath + "/AutoSave.json",
        Application.persistentDataPath + "/Save1.json",
        Application.persistentDataPath + "/Save2.json",
        Application.persistentDataPath + "/Save3.json",
     };

    public static void Save(GameSaveState save,int index)
    {

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        string json = JsonConvert.SerializeObject(save, settings);
        File.WriteAllText(FILEPATH[index], json);

    }

    public static GameSaveState Load(int index)
    {
        GameSaveState loadedSave = null;

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        if (File.Exists(FILEPATH[index]))
        {
            string json = File.ReadAllText(FILEPATH[index]);
            loadedSave = JsonConvert.DeserializeObject<GameSaveState>(json, settings);
        }
        return loadedSave;
    }

    public static bool HasSave(int slot) //세이브파일 존재하는지 여부
    {
        return File.Exists(FILEPATH[slot]);
    }

    public static DateTime GetDateTime(int slot) //세이브파일 저장 시간 불러오기
    {
        return File.GetLastWriteTime(FILEPATH[slot]);
    }
}
