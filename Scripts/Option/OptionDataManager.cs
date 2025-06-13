using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OptionDataManager : MonoBehaviour
{
    public static OptionDataManager instance = null;

    // --- 게임 데이터 파일이름 설정 ---
    private string OptionDataFileName = "Option.json";

    public OptionData OptionData;

    private void Awake()
    {
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        LoadOptionData();
        SaveOptionData();

        //언어 확인 후 UI언어들 초기화


        //옵션 확인 후 옵션 UI 초기화
        //FindObjectOfType<OptionManager>().InitMenuLayouts();

        //품질 설정
        //QualitySettings.SetQualityLevel(OptionData.currentSelectQuilityID, true);
    }

    // 저장된 게임 불러오기
    private void LoadOptionData()
    {
        string filePath = Application.persistentDataPath + OptionDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            print("옵션 파일 불러오기 성공");
            string FromJsonData = File.ReadAllText(filePath);
            OptionData = JsonUtility.FromJson<OptionData>(FromJsonData);

            if (OptionData == null)
            {
                Debug.Log("옵션 파일 없음");
            }
        }

        // 저장된 게임이 없다면
        else
        {
            ResetOptionData();
        }
    }

    //데이터를 초기화(새로 생성 포함)하는경우
    public void ResetOptionData()
    {
        print("새로운 옵션 파일 생성");
        OptionData = null;
        OptionData = new OptionData();

        //새로 생성하는 데이터들은 이곳에 선언하기
        //OptionData.language = Application.systemLanguage;


        //옵션 데이터 저장
        SaveOptionData();
    }

    // 옵션 데이터 저장하기
    public void SaveOptionData()
    {
        string ToJsonData = JsonUtility.ToJson(OptionData);
        string filePath = Application.persistentDataPath + OptionDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰기
        File.WriteAllText(filePath, ToJsonData);
    }



}
