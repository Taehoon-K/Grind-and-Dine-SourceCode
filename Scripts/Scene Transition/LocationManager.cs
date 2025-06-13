using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneTransitionManager;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    public List<StartPoint> startPoints;

    //��ųʸ��� �̾��� ���� �߰�
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
    //�÷��̾� �ö� ��Ÿ�� ��ġ ã��
    public Transform GetPlayerStartingPosition(SceneTransitionManager.Location enteringFrom)
    {
        StartPoint startingPoint = startPoints.Find(x => x.enteringFrom == enteringFrom);

        if (startingPoint.Equals(default(StartPoint))) // �⺻���� ��
        {
            startingPoint = startPoints.Find(x => x.enteringFrom == Location.Proto); ; // �⺻ ��ġ�� ������ Transform
            return startingPoint.playerStart;
        }

        return startingPoint.playerStart;
        
    }

    public Transform GetExitPosition(Location exitingTo)
    {
        /*Transform startpoint = GetPlayerStartingPosition(exitingTo);  
        return startpoint.parent.GetComponentInChildren<LocationEntryPoint>().transform; //�����̼ǿ�Ʈ������Ʈ�� ��ġ ��������*/

        // �÷��̾��� ���� ��ġ ��������
        Transform startpoint = GetPlayerStartingPosition(exitingTo);

        if (startpoint == null) //����� ��� �뵵
        {
            Debug.LogError($"[ERROR] GetPlayerStartingPosition returned NULL for exitingTo: {exitingTo}");
            return null;
        }

        if (startpoint.parent == null)
        {
            Debug.LogError($"[ERROR] startpoint ({startpoint.name}) has no parent! Check the hierarchy.");
            return null;
        }

        // startpoint�� �θ� ������Ʈ�� ���Ե� ��� LocationEntryPoint ������Ʈ ��������
        LocationEntryPoint[] entryPoints = startpoint.parent.GetComponentsInChildren<LocationEntryPoint>();
        Debug.Log($"Checking {entryPoints.Length} LocationEntryPoints under parent: {startpoint.parent.name}");

        // �� LocationEntryPoint�� �˻��Ͽ� locationToSwitch�� exitingTo�� ��ġ�ϴ��� Ȯ��
        foreach (LocationEntryPoint entryPoint in entryPoints)
        {
            if (entryPoint.locationToSwitch == exitingTo)
            {
                // ��ġ�ϴ� LocationEntryPoint�� ã���� �ش� transform ��ȯ
                return entryPoint.transform;
            }
        }

        // ��ġ�ϴ� LocationEntryPoint�� ã�� ���� ��� �⺻������ ù ��° entryPoint�� transform ��ȯ (�ʿ信 ���� ���� ó�� ����)
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
