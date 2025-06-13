using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneTransitionManager;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    public List<StartPoint> startPoints;

    //딕셔너리로 이어진 씬들 추가
    private static readonly Dictionary<Location, List<Location>> sceneConnection = new Dictionary<Location, List<Location>>()
    {
        {Location.Tutorial, new List<Location>{Location.HomeGround} },
        {Location.Proto, new List<Location>{Location.HomeGround, Location.Gym, Location.Villar1, Location.Villar2, Location.SereneH, Location.JakeH, Location.PoliceOffice, Location.Villar3, Location.CharlesH, Location.Apart101, Location.Lottery, Location.RohanH } },
        {Location.Gym, new List<Location>{Location.Proto} },
        {Location.Villar1, new List<Location>{Location.Proto} },
        {Location.Villar2, new List<Location>{Location.Proto} },
        {Location.SereneH, new List<Location>{Location.Proto} },
        {Location.JakeH, new List<Location>{Location.Proto} },
        {Location.PoliceOffice, new List<Location>{Location.Proto} },
        {Location.Villar3, new List<Location>{Location.Proto} },
        {Location.CharlesH, new List<Location>{Location.Proto} },
        {Location.Apart101, new List<Location>{Location.Proto} },
        {Location.HomeGround, new List<Location>{Location.Proto, Location.Tutorial } },
        {Location.Lottery, new List<Location>{Location.Proto} },
        {Location.RohanH, new List<Location>{Location.Proto} }
    };

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    //플레이어 올때 스타팅 위치 찾기
    public Transform GetPlayerStartingPosition(SceneTransitionManager.Location enteringFrom)
    {
        StartPoint startingPoint = startPoints.Find(x => x.enteringFrom == enteringFrom);

        if (startingPoint.Equals(default(StartPoint))) // 기본값과 비교
        {
            startingPoint = startPoints.Find(x => x.enteringFrom == Location.Proto); ; // 기본 위치로 설정된 Transform
            return startingPoint.playerStart;
        }

        return startingPoint.playerStart;
        
    }

    public Transform GetExitPosition(Location exitingTo)
    {
        /*Transform startpoint = GetPlayerStartingPosition(exitingTo);  
        return startpoint.parent.GetComponentInChildren<LocationEntryPoint>().transform; //로케이션엔트리포인트의 위치 가져오기*/

        // 플레이어의 시작 위치 가져오기
        Transform startpoint = GetPlayerStartingPosition(exitingTo);

        if (startpoint == null) //디버그 잡는 용도
        {
            Debug.LogError($"[ERROR] GetPlayerStartingPosition returned NULL for exitingTo: {exitingTo}");
            return null;
        }

        if (startpoint.parent == null)
        {
            Debug.LogError($"[ERROR] startpoint ({startpoint.name}) has no parent! Check the hierarchy.");
            return null;
        }

        // startpoint의 부모 오브젝트에 포함된 모든 LocationEntryPoint 컴포넌트 가져오기
        LocationEntryPoint[] entryPoints = startpoint.parent.GetComponentsInChildren<LocationEntryPoint>();
        Debug.Log($"Checking {entryPoints.Length} LocationEntryPoints under parent: {startpoint.parent.name}");

        // 각 LocationEntryPoint를 검사하여 locationToSwitch가 exitingTo와 일치하는지 확인
        foreach (LocationEntryPoint entryPoint in entryPoints)
        {
            if (entryPoint.locationToSwitch == exitingTo)
            {
                // 일치하는 LocationEntryPoint를 찾으면 해당 transform 반환
                return entryPoint.transform;
            }
        }

        // 일치하는 LocationEntryPoint를 찾지 못한 경우 기본적으로 첫 번째 entryPoint의 transform 반환 (필요에 따라 예외 처리 가능)
        Debug.LogWarning("Matching LocationEntryPoint not found, returning default.");
        return entryPoints.Length > 0 ? entryPoints[0].transform : null;
    }
    public static Location GetNextLocation(Location currentScene, Location finalDestination)
    {
        Dictionary<Location, bool> visited = new Dictionary<Location, bool>();

        Dictionary<Location, Location> previousLocation = new Dictionary<Location, Location>();

        Queue<Location> workList = new Queue<Location>();

        visited.Add(currentScene, false);
        workList.Enqueue(currentScene);

        //BFS traversal
        while(workList.Count != 0)
        {
            Location scene = workList.Dequeue();
            if(scene == finalDestination)
            {
                while(previousLocation.ContainsKey(scene) && previousLocation[scene] != currentScene)
                {
                    scene = previousLocation[scene];
                }
                return scene;
            }

            if (sceneConnection.ContainsKey(scene))
            {
                List<Location> possibleDestinations = sceneConnection[scene];

                foreach(Location neighbour in possibleDestinations)
                {
                    if (!visited.ContainsKey(neighbour))
                    {
                        visited.Add(neighbour, false);
                        previousLocation.Add(neighbour, scene);
                        workList.Enqueue(neighbour);
                    }
                }
            }
        }

        return currentScene;
    }
}
