using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class ElevatorPrompt : UiPrompt
{
    [SerializeField]
    private GameObject fourF;
    [SerializeField]
    private GameObject threeF;

    [SerializeField]
    private GameObject screen;

    Elevator elve;

    public void OpenScreen(int floor) //화면 킬 때 호출하는 함수
    {
        elve = FindObjectOfType<Elevator>();
        screen.SetActive(true);
        if (floor == 1)
        {
            threeF.SetActive(false);
            fourF.SetActive(true);
        }
        else
        {
            threeF.SetActive(true);
            fourF.SetActive(false);
        }
    }

    public void ToProto()
    {
        SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Proto);
    }

    public void CallEleve(int floor)
    {
        elve.GoToFloor(floor);
    }
    public void Exit()
    {
        screen.SetActive(false);
        //UIManager.instance.CurrentUIState = UIState.None;
    }
}
