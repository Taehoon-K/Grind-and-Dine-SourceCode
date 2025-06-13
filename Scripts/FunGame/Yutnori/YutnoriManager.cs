using Kupa;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.InputSystem;

public class YutnoriManager : MonoBehaviour, HelpPanel
{
    [SerializeField]
    private Rigidbody[] yutSticks; // 윷 막대 Rigidbody 배열
    [SerializeField]
    private float maxThrowForce = 10f; // 게이지 최대 값으로 설정되는 힘
    [SerializeField]
    public float rotationForce = 1f; // 회전에 사용할 랜덤 회전 힘
    [SerializeField]
    private Transform throwStartPosition; // 윷을 던질 시작 위치
    [SerializeField]
    private Collider boundaryCollider; // 바닥 위의 3차원 콜라이더
    private bool isFoul = false; // 낙 여부 체크

    private bool[] yutResults = new bool[4];     // 윷의 결과 (true: 앞면, false: 뒷면)

    [SerializeField]
    private GameObject throwButton; //던지기 버튼
    [SerializeField] private GameObject playerTurn, cpuTurn;

    [Header("YutBoard")]
    public Transform[] playerPieces; // 플레이어의 말 3개
    public Transform[] cpuPieces;     // CPU의 말 3개
    public Button[] pieceButtons;    // 말 선택 버튼 (3개)
    public int[] playerPositions;    // 각 말의 현재 위치 (-1은 대기 중)
    public int[] cpuPositions;        // CPU 말의 위치 (-1: 대기 중)

    public List<Button> boardButtons; // 보드의 모든 칸 버튼
    public List<Transform> boardMap;  // 각 칸의 실제 위치
    public int moveCount;            // 윷 던지기 결과값

    private List<Button> highlightedButtons = new List<Button>(); // 하이라이트된 버튼

    private Vector3[] playerPiecesStart; // 플레이어의 말 3개 시작지점
    private Vector3[] cpuPiecesStart;     // CPU의 말 3개 시작지점
    private Vector3[] pieceButtonsStart; // 버튼의 초기 위치 저장

    private int playerArrived = 0; // 플레이어의 도착 말 개수
    private int cpuArrived = 0;    // CPU의 도착 말 개수
    private List<int>[] stackedPieces; // 각 말에 업힌 말들의 인덱스를 저장
    private List<int>[] cpuStackedPieces; // 각 말에 업힌 말들의 인덱스를 저장

    private bool isPlayerTurn;

    [SerializeField] private Canvas canvas; // 캔버스를 연결
    [SerializeField] private GameObject highlightPrefab; // 하이라이트 버튼 프리팹

    [SerializeField]
    protected GameObject resultPanel; //결과 패널

    public static YutnoriManager instance = null;
    public int money;
    public Status stat; //원래 가지고 있던 스탯 수치 저장

    [SerializeField]
    protected GameObject[] doGaePanel; //결과 패널

    private bool gameStop;

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //도움말 버튼

    private void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    private void Start()
    {
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //시간 흐르는거 멈추게하기
        }
        
        if (PlayerPrefs.HasKey("SelectedMoney"))
        {
            money = PlayerPrefs.GetInt("SelectedMoney");
            Debug.Log("Received Money: " + money);
        }
        if(StatusManager.instance != null)
        {
            stat = StatusManager.instance.GetStatus().Clone();
        }
        

        isPlayerTurn = true;
        ClearHighlights();
        InitializeStackedPieces();
        InitalizePosition();

        Cursor.visible = true;
    }
    private void Update()
    {
        if (helpButton)
        {
            helpButton = false;
            if (Time.timeScale != 0) //시간 안멈춰있을때만
            {
                helpPanel.SetActive(true);
            }
        }
    }

    private void InitalizePosition()
    {
        playerPiecesStart = new Vector3[playerPieces.Length];
        for (int i = 0; i < playerPieces.Length; i++)
        {
            playerPiecesStart[i] = playerPieces[i].position;
        }
        cpuPiecesStart = new Vector3[cpuPieces.Length];
        for (int i = 0; i < cpuPieces.Length; i++)
        {
            cpuPiecesStart[i] = cpuPieces[i].position;
        }
        // 버튼 위치를 저장할 배열 초기화
        pieceButtonsStart = new Vector3[pieceButtons.Length];

        // 버튼들의 초기 위치 저장
        for (int i = 0; i < pieceButtons.Length; i++)
        {
            pieceButtonsStart[i] = pieceButtons[i].GetComponent<RectTransform>().position;
        }
    }
    private void InitializeStackedPieces() //업기 스택 초기화
    {
        stackedPieces = new List<int>[playerPieces.Length];
        cpuStackedPieces = new List<int>[playerPieces.Length];
        for (int i = 0; i < stackedPieces.Length; i++)
        {
            stackedPieces[i] = new List<int>();
            cpuStackedPieces[i] = new List<int>();
        }
    }

    #region ThrowYut
    public void ThrowYut(float gauge)
    {
        throwButton.SetActive(false); //버튼 없애기
        // 게이지 값에 따라 힘 계산
        float throwForce = gauge * maxThrowForce + 1.5f;

        // 윷 막대를 시작 위치로 리셋하고, 던짐
        for(int i = 0; i < yutSticks.Length; i++)
        {
            yutSticks[i].velocity = Vector3.zero; // 기존 속도 초기화
            yutSticks[i].angularVelocity = Vector3.zero; // 기존 회전 초기화
            yutSticks[i].transform.position = throwStartPosition.position + new Vector3(0.04f * i,0f,0f); // 시작 위치 설정
            yutSticks[i].transform.rotation = throwStartPosition.rotation; // 초기 회전값 설정


            Vector3 throwDirection = new Vector3(Random.Range(-0.2f, 0.2f), 1f, Random.Range(-0.2f, 0.2f)).normalized;
            // 힘을 위쪽으로 추가 (위쪽 방향은 Vector3.up)
            yutSticks[i].AddForce(throwDirection * throwForce, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3( //랜덤 회전력 추가
                Random.Range(-rotationForce * 2, rotationForce * 2),
                Random.Range(-rotationForce, rotationForce),
                Random.Range(-rotationForce, rotationForce)
            );
            yutSticks[i].AddRelativeTorque(randomTorque, ForceMode.VelocityChange);
        }
        SoundManager.instance.PlaySound2D("drop-stick");

        // 낙 체크 시작
        StartCoroutine(CheckOutOfBounds());
    }

    private IEnumerator CheckOutOfBounds()
    {
        isFoul = false;

        // 윷이 안정된 후 낙 여부를 판단하기 위해 3초 정도 대기
        yield return new WaitForSeconds(1.5f);

        foreach (Rigidbody yut in yutSticks)
        {
            if (!boundaryCollider.bounds.Contains(yut.transform.position))
            {
                isFoul = true;
                doGaePanel[6].SetActive(true); //낙 띄우기
                yield return new WaitForSeconds(1f);
                doGaePanel[6].SetActive(false); //낙 띄우기

                ChangeTurn(false); //턴 넘기기
                yield break; // 코루틴 종료
            }
        }

        if (!isFoul)
        {
           // Debug.Log("Caluclllllllllllllllllllllllll");
            CalculateYutResult();
        }
    }

    public void CalculateYutResult()
    {
        if (gameStop)
        {
            return; //만약 겜 종료되면 중지
        }

        int frontCount = 0;  // 앞면 개수
        //int backCount = 0;   // 뒷면 개수

        // 윷의 결과를 판별
        for (int i = 0; i < yutSticks.Length; i++)
        {
            // 윷이 앞면인지 뒷면인지 판별하는 로직 (예: Rigidbody의 각도나 특정 값 확인)
            // 이 예시에서는 단순히 임의로 true/false를 할당
            // 실제로는 윷의 각도를 기반으로 판단하거나, 앞면/뒷면을 식별할 수 있는 방법을 추가해야 함

            // 예시로, 각 윷의 Y축 각도를 기준으로 앞면/뒷면을 판별
            if ((Mathf.Abs(GetZRotation(yutSticks[i].transform.rotation)) > 90 && Mathf.Abs(GetXRotation(yutSticks[i].transform.rotation)) > 130)||
                (Mathf.Abs(GetZRotation(yutSticks[i].transform.rotation)) < 90 && Mathf.Abs(GetXRotation(yutSticks[i].transform.rotation)) < 50))
            {
                yutResults[i] = true; // 앞면
                frontCount++;
            }
            else
            {
                yutResults[i] = false; // 뒷면
                //backCount++;
            }
        }

        // 윷 결과 판별
        int result = 0;

        if(!yutResults[3] && frontCount == 3)
        {
            result = -1;
        }
        else if (frontCount == 4)
        {
            result = 5;
        }
        else if (frontCount == 3)
        {
            result = 1;
        }
        else if (frontCount == 2)
        {
            result = 2;
        }
        else if (frontCount == 1)
        {
            result = 3;
        }
        else if (frontCount == 0)
        {
            result = 4;
        }

        if (result == -1)
        {
            doGaePanel[0].SetActive(true);
        }
        else
        {
            doGaePanel[result].SetActive(true);
        }
        StartCoroutine(DelayYutThrowResult(result, 1f));
    }
    IEnumerator DelayYutThrowResult(int result, float delay)
    {
        yield return new WaitForSeconds(delay); // 1초 기다림
        if (result == -1)
        {
            doGaePanel[0].SetActive(false);
        }
        else
        {
            doGaePanel[result].SetActive(false);
        }
        OnYutThrowResult(result);
    }

    private float GetZRotation(Quaternion rotation)
    {
        // 회전 행렬을 사용해 Z축 회전값을 추출
        float zRotation = rotation.eulerAngles.z;
        if (zRotation > 180)  // -180 ~ 180으로 변환
        {
            zRotation -= 360;
        }
        return zRotation;
    }
    private float GetXRotation(Quaternion rotation)
    {
        // 회전 행렬을 사용해 Z축 회전값을 추출
        float xRotation = rotation.eulerAngles.x;
        if (xRotation > 180)  // -180 ~ 180으로 변환
        {
            xRotation -= 360;
        }
        return xRotation;
    }
    #endregion

    // 윷 던지기 결과를 받은 후 호출
    // 윷 던지기 결과를 받고 이동 프로세스 시작
    public void OnYutThrowResult(int moveCount)
    {
        this.moveCount = moveCount; // 윷 결과 저장

        if (isPlayerTurn)
        {
            ShowPieceSelection(moveCount); // 이동할 말 선택 UI 표시
        }
        else
        {
            CpuWhereMove(moveCount);
        }
    }
    // 말 선택 UI 표시
    private void ShowPieceSelection(int moveCount)
    {
        int howMove = 0;
        for (int i = 0; i < playerPieces.Length; i++)
        {
            int pieceIndex = i; // Closure 문제 방지

            // 업힌 상태인지 확인
            bool isStacked = false;
            foreach (var stack in stackedPieces)
            {
                if (stack.Contains(pieceIndex))
                {
                    isStacked = true;
                    break;
                }
            }

            if (playerPositions[i] != -2 && !isStacked && (playerPositions[i] == -1 || CanMove(playerPositions[i], moveCount)))
            {
                // 말 선택 버튼 활성화
                pieceButtons[i].gameObject.SetActive(true);
                pieceButtons[i].onClick.RemoveAllListeners();
                pieceButtons[i].onClick.AddListener(() => OnPieceSelected(pieceIndex, moveCount));
            }
            else
            {
                pieceButtons[i].gameObject.SetActive(false); // 이동 불가능한 말은 선택 불가
                howMove++;
            }
        }
        if(howMove == 3) //만약 움직일 수 있는거 없으면
        {
            Debug.Log("움직일 수 있는 말이 없습니다.");
            ChangeTurn(false);
        }
    }

    // 이동 가능성 검사
    private bool CanMove(int currentPosition, int moveCount)
    {
        if (moveCount == -1) return currentPosition >= 0; // 백도는 0 이상일 때만 가능
        return true; // 다른 경우 기본적으로 이동 가능
    }

    // 말 선택 UI 표시
    // 선택한 말과 윷 결과로 이동 가능한 칸 표시
    private void OnPieceSelected(int pieceIndex, int moveCount) //클릭시 실행될 함수
    {
        ShowPieceSelection(moveCount); //다시 다른 애들 화살표 띄우기
        pieceButtons[pieceIndex].gameObject.SetActive(false); //자신은 화살표 지우고

        ClearHighlights(); // 이전 상태 초기화

        int currentPosition = playerPositions[pieceIndex];

        // 대기 중인 말이면 시작점만 하이라이트
        /* if (currentPosition == -1)
         {
             HighlightPosition(0); // 시작점
             return;
         }*/
        /*
         // 백도 처리
         if (moveCount == -1)
         {
             if (currentPosition > 0)
             {
                 HighlightPosition(currentPosition - 1);
             }
             return;
         }*/

        int targetPosition = currentPosition + moveCount;
        int secondTargetPo =0;
        // 이동 가능한 말들 중 최적의 움직임 결정

        bool isTwo = false; //만약 갈림길인지
        // 갈림길 처리
        if (currentPosition == 4) // 4번에서 갈림길
        {
            isTwo = true;
            secondTargetPo = currentPosition + moveCount +15;
        }
        else if (currentPosition == 22) // 지름길에서 갈림길
        {
            isTwo = true;
            if(moveCount >= 3) //만약 걸 이상이면
            {
                targetPosition = currentPosition + moveCount - 11; //일반 경로 재설정
            }
            if (moveCount == 3) //만약 걸이면
            {
                secondTargetPo = 19; //마지막 지점으로
            }
            else if (moveCount > 3) //만약 걸 이상이면
            {
                secondTargetPo = 30; //마지막 지점으로
            }
            else if (moveCount < 3) //만약 도 개면
            {
                secondTargetPo = currentPosition + moveCount + 5; //경로 재설정
            }
        }
        else if (currentPosition == 9) // 지름길에서 갈림길
        {
            isTwo = true;
            secondTargetPo = currentPosition + moveCount +15;
            if(secondTargetPo == 27)
            {
                secondTargetPo = 22; //중앙 넘버 재조정
            }
        }else if (currentPosition >= 15 && currentPosition <= 19)
        {
            if(targetPosition >= 20) //만약 골인이라면
            {
                targetPosition = 30;
            }        
        }else if (currentPosition >= 20 && currentPosition <= 24)
        {
            if (targetPosition >= 25) //만약 타겟포인트 오바되면
            {
                targetPosition = currentPosition + moveCount - 11;
            }
        }else if(currentPosition == 25 || currentPosition == 26)
        {
            if(targetPosition == 27)
            {
                targetPosition = 22; //중앙 넘버 재조정
            }
        }
        if(currentPosition >= 25 && currentPosition <= 29)
        {
            if(targetPosition == 30)
            {
                targetPosition = 19;
            }
        }
        if (targetPosition == -1 || targetPosition == -2) //백도 걸렸을 시
        {
            targetPosition = 19;
        }

        HighlightPosition(targetPosition); // 일반 경로
        if (isTwo)
        {
            HighlightPosition(secondTargetPo); //갈림길 경로
        }
        // 이동 가능한 버튼에 클릭 이벤트 추가
        foreach (var button in highlightedButtons)
        {
            button.onClick.RemoveAllListeners();

            // 버튼의 위치 정보 가져오기
            int buttonPosition = button.GetComponent<HighlightData>().Position;

            // 일반 경로 버튼인 경우
            if (buttonPosition == targetPosition || (targetPosition > 30 && buttonPosition == 30))
            {
                button.onClick.AddListener(() => MovePiece(pieceIndex, targetPosition, true)); // 일반 경로로 이동
            }
            // 갈림길 경로 버튼인 경우
            else if (isTwo && buttonPosition == secondTargetPo)
            {
                button.onClick.AddListener(() => MovePiece(pieceIndex, secondTargetPo, true)); // 갈림길 경로로 이동
            }
        }
    }

    // 특정 칸을 하이라이트
    private void HighlightPosition(int position)
    {
        if(position > 30)
        {
            position = 30; //만약 도착점보다 크면 도착으로 간주
        }
        /*if (boardButtons[position] != null)
        {
            boardButtons[position].gameObject.SetActive(true);

            highlightedButtons.Add(boardButtons[position]);
        }*/
        if (position < 0 || position >= 31)
            return;

        // 버튼 생성 및 초기화
        Button highlightButton = Instantiate(highlightPrefab, canvas.transform).GetComponent<Button>();
        highlightButton.transform.position = boardButtons[position].transform.position; // 버튼 위치를 설정

        // 버튼에 위치 데이터를 추가
        highlightButton.gameObject.AddComponent<HighlightData>().Position = position;

        // 하이라이트된 버튼 리스트에 추가
        highlightedButtons.Add(highlightButton);
    }

    // 모든 하이라이트 제거
    private void ClearHighlights()
    {
        foreach (var button in highlightedButtons)
        {
            Destroy(button.gameObject);
        }
        highlightedButtons.Clear();
    }
    private void ClearPieceButton()
    {
        foreach (var button in pieceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    // 선택된 말을 이동
    public void MovePiece(int pieceIndex, int targetPosition, bool isPlayer)
    {
        if (targetPosition >= 30) 
        { 
            targetPosition = 30;

            if (isPlayer)
            {
                // 말 숨기기
                playerPieces[pieceIndex].gameObject.SetActive(false);
                playerArrived++;
                // 말의 현재 위치 갱신
                playerPositions[pieceIndex] = -2;

                // 업힌 말도 함께 도착 처리
                foreach (int stackedIndex in stackedPieces[pieceIndex])
                {
                    playerPieces[stackedIndex].gameObject.SetActive(false);
                    playerArrived++;
                    Debug.Log($"업힌 말 {stackedIndex}도 도착했습니다.");
                    playerPositions[stackedIndex] = -2;
                }

                // 승리 조건 확인
                CheckWinCondition(true);
                // 상태 초기화 및 턴 종료
                ClearHighlights();
                ClearPieceButton();

                ChangeTurn(false); //턴 판단
                return;
            }
            else
            {
                // 말 숨기기
                cpuPieces[pieceIndex].gameObject.SetActive(false);
                cpuArrived++;
                // 말의 현재 위치 갱신
                cpuPositions[pieceIndex] = -2;

                // 업힌 말도 함께 도착 처리
                foreach (int stackedIndex in cpuStackedPieces[pieceIndex])
                {
                    cpuPieces[stackedIndex].gameObject.SetActive(false);
                    cpuArrived++;
                    Debug.Log($"업힌 말 {stackedIndex}도 도착했습니다.");
                    cpuPositions[stackedIndex] = -2;
                }

                // 승리 조건 확인
                CheckWinCondition(false);
                // 상태 초기화 및 턴 종료
                ClearHighlights();
                ClearPieceButton();

                ChangeTurn(false); //턴 판단
                return; 
            }
        }
        // 말 이동

        if (isPlayer)
        {
           // playerPieces[pieceIndex].position = boardMap[targetPosition].position;
            StartCoroutine(MovePieceSmoothly(playerPieces[pieceIndex], boardMap[targetPosition].position)); // speed 값을 조정하여 이동 속도 변경

            // 말의 현재 위치 갱신
            playerPositions[pieceIndex] = targetPosition;

            // 업힌 말 이동 처리
            foreach (int stackedIndex in stackedPieces[pieceIndex])
            {
                playerPositions[stackedIndex] = targetPosition;
               // playerPieces[stackedIndex].position = boardMap[targetPosition].position; // 리더 말과 동일한 위치로 이동
                StartCoroutine(MovePieceSmoothly(playerPieces[stackedIndex], boardMap[targetPosition].position));
                Debug.Log($"업힌 말 {stackedIndex}이(가) 함께 이동했습니다.");

                pieceButtons[stackedIndex].GetComponent<RectTransform>().position =
                boardButtons[targetPosition].GetComponent<RectTransform>().position;
            }

            //말 버튼 옮기기
            pieceButtons[pieceIndex].GetComponent<RectTransform>().position =
            boardButtons[targetPosition].GetComponent<RectTransform>().position;
        }
        else
        {
            //cpuPieces[pieceIndex].position = boardMap[targetPosition].position;
            StartCoroutine(MovePieceSmoothly(cpuPieces[pieceIndex], boardMap[targetPosition].position));
            // 말의 현재 위치 갱신
            cpuPositions[pieceIndex] = targetPosition;

            // 업힌 말 이동 처리
            foreach (int stackedIndex in cpuStackedPieces[pieceIndex])
            {
                cpuPositions[stackedIndex] = targetPosition;
               // cpuPieces[stackedIndex].position = boardMap[targetPosition].position; // 리더 말과 동일한 위치로 이동
                StartCoroutine(MovePieceSmoothly(cpuPieces[stackedIndex], boardMap[targetPosition].position));

                Debug.Log($"업힌 말 {stackedIndex}이(가) 함께 이동했습니다.");
            }
        }

        bool isOneMore = false;
        // 상대 말 잡기
        if (CaptureOpponent(targetPosition, isPlayer) || moveCount >=4) //만약 잡았다면, 아니면 윷 이상이면
        {
            isOneMore = true;
        }

        // 업기 처리
        StackPiece(targetPosition, isPlayer, pieceIndex);


        // 상태 초기화 및 턴 종료
        ClearHighlights();
        ClearPieceButton();

        ChangeTurn(isOneMore); //턴 판단
    }
    private void ChangeTurn(bool oneMore) //여기서 턴 넘길지 한번 더하는지 결정
    {
        if (!oneMore) //만약 한번더 아니라면
        {
            Debug.Log("턴 종료. 다음 플레이어의 차례입니다!");
            isPlayerTurn = !isPlayerTurn;
        }
        else
        {
            Debug.Log("턴 한번더!");
        }
        EndTurn();
    }
    // 턴 종료 처리
    private void EndTurn()
    {
        if (isPlayerTurn)
        {
            throwButton.SetActive(true); //던지기
            playerTurn.SetActive(true);
            cpuTurn.SetActive(false);
        }
        else
        {
            playerTurn.SetActive(false);
            cpuTurn.SetActive(true);
            Invoke(nameof(CPUMove), 1f);
        }
    }

    private bool CanCapture(int position) //상대 말 잡을수있는지 판단하는 코드
    {
        int[] opponentPositions = playerPositions;

        // 상대 말이 해당 위치에 있는지 확인
        for (int i = 0; i < opponentPositions.Length; i++)
        {
            if (opponentPositions[i] == position)
            {
                return true; // 잡을 수 있는 말이 있음
            }
        }
        return false; // 잡을 수 있는 말 없음
    }

    // 상대 말 잡기 처리
    private bool CaptureOpponent(int position, bool isPlayer)
    {
        bool isCatch = false;
        int[] opponentPositions = isPlayer ? cpuPositions : playerPositions;
        Transform[] opponentPieces = isPlayer ? cpuPieces : playerPieces;
        List<int>[] opponentStackedPieces = isPlayer ? cpuStackedPieces : stackedPieces; // 상대 스택 정보

        for (int i = 0; i < opponentPositions.Length; i++)
        {
            if (opponentPositions[i] == position)
            {
                opponentPositions[i] = -1; // 잡힌 말 대기 상태로 이동
                opponentPieces[i].position = isPlayer ? cpuPiecesStart[i] : playerPiecesStart[i]; // 대기 위치
                Debug.Log($"상대 말 잡음! 위치: {position}");
                isCatch = true;
                if (!isPlayer)
                {
                    pieceButtons[i].GetComponent<RectTransform>().position = pieceButtonsStart[i];
                }

                // 해당 말에 업힌 말도 함께 처리
                foreach (int stackedIndex in opponentStackedPieces[i])
                {
                    opponentPositions[stackedIndex] = -1; // 업힌 말도 대기 상태로 이동
                    opponentPieces[stackedIndex].position = isPlayer ? cpuPiecesStart[stackedIndex] : playerPiecesStart[stackedIndex]; // 대기 위치
                    Debug.Log($"업힌 상대 말 {stackedIndex}도 함께 잡힘!");
                    if (!isPlayer)
                    {
                        pieceButtons[stackedIndex].GetComponent<RectTransform>().position = pieceButtonsStart[stackedIndex];
                    }
                }

                // 스택 초기화
                opponentStackedPieces[i].Clear();
            }
        }
        return isCatch;
    }

    // 업기 처리
    private void StackPiece(int position, bool isPlayer, int currentPieceIndex)
    {
        /*
        if (isPlayer)
        {
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // 자신과 같은 말을 업힐 수 없으며, 이미 스택에 없는 경우에만 추가
                // 현재 이동 중인 말은 제외하고 같은 위치에 다른 말이 있는지 확인
                if (i != currentPieceIndex && playerPositions[i] == position && !stackedPieces[currentPieceIndex].Contains(i))
                {
                    Debug.Log($"말 {i}이(가) 말 {currentPieceIndex}에 업혔습니다.");
                    stackedPieces[currentPieceIndex].Add(i); // 업힌 말 추가
                }
            }
        }
        else
        {
            for (int i = 0; i < cpuPositions.Length; i++)
            {
                // 현재 이동 중인 말은 제외하고 같은 위치에 다른 말이 있는지 확인
                if (i != currentPieceIndex && cpuPositions[i] == position && !cpuStackedPieces[currentPieceIndex].Contains(i))
                {
                    Debug.Log($"말 {i}이(가) 말 {currentPieceIndex}에 업혔습니다.");
                    cpuStackedPieces[currentPieceIndex].Add(i); // 업힌 말 추가
                }
            }
        }*/
        if (isPlayer)
        {
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // 자신과 같은 말을 업힐 수 없으며, 이미 스택에 없는 경우에만 추가
                // 현재 이동 중인 말은 제외하고 같은 위치에 다른 말이 있는지 확인
                if (i != currentPieceIndex && playerPositions[i] == position)
                {
                    // 업힌 말이 다른 스택에서 이동해야 할 경우 처리
                    RemoveFromPreviousStack(i, stackedPieces);

                    // 현재 스택에 추가
                    Debug.Log($"말 {i}이(가) 말 {currentPieceIndex}에 업힙니다.");
                    stackedPieces[currentPieceIndex].Add(i);

                    // 현재 업힌 말(i)의 스택을 가져와 전부 currentPieceIndex에 추가
                    foreach (int subStacked in stackedPieces[i])
                    {
                        Debug.Log($"말 {subStacked}도 말 {currentPieceIndex}에 업힙니다.");
                        stackedPieces[currentPieceIndex].Add(subStacked);
                    }

                    // 업힌 말(i)의 스택 초기화 (i는 이제 currentPieceIndex에 포함됨)
                    stackedPieces[i].Clear();
                }
            }
        }
        else
        {
            for (int i = 0; i < cpuPositions.Length; i++)
            {
                // 현재 이동 중인 말은 제외하고 같은 위치에 다른 말이 있는지 확인
                if (i != currentPieceIndex && cpuPositions[i] == position)
                {
                    // 업힌 말이 다른 스택에서 이동해야 할 경우 처리
                    RemoveFromPreviousStack(i, cpuStackedPieces);

                    // 현재 스택에 추가
                    Debug.Log($"CPU 말 {i}이(가) CPU 말 {currentPieceIndex}에 업힙니다.");
                    cpuStackedPieces[currentPieceIndex].Add(i);

                    // 현재 업힌 말(i)의 스택을 가져와 전부 currentPieceIndex에 추가
                    foreach (int subStacked in cpuStackedPieces[i])
                    {
                        Debug.Log($"CPU 말 {subStacked}도 CPU 말 {currentPieceIndex}에 업힙니다.");
                        cpuStackedPieces[currentPieceIndex].Add(subStacked);
                    }

                    // 업힌 말(i)의 스택 초기화
                    cpuStackedPieces[i].Clear();
                }
            }
        }
    }
    // 이전 스택에서 해당 말을 제거하는 함수
    private void RemoveFromPreviousStack(int pieceIndex, List<int>[] stackedPiecesArray)
    {
        for (int i = 0; i < stackedPiecesArray.Length; i++)
        {
            if (stackedPiecesArray[i].Contains(pieceIndex))
            {
                Debug.Log($"말 {pieceIndex}이(가) 이전 스택 {i}에서 제거됩니다.");
                stackedPiecesArray[i].Remove(pieceIndex);
                break;
            }
        }
    }
    // CPU 이동
    private void CPUMove()
    {
        float gage = Random.Range(0.1f, 0.4f);
        ThrowYut(gage);
    }
    private void CpuWhereMove(int count)
    {
        int bestPieceIndex = -1;
        int bestTargetPosition = -1;
        int secondBestTarget = -1;
        bool isTwo = false; // 갈림길 여부
        int highestScore = int.MinValue; // 가장 높은 점수

        // 현재 위치를 기준으로 이동 가능한 말을 탐색
        for (int i = 0; i < cpuPositions.Length; i++)
        {
            isTwo = false;
            int currentPosition = cpuPositions[i];

            // 이미 골인한 말은 스킵
            if (currentPosition == -2)
                continue;


            // **업힌 말인지 확인**
            bool isCarried = false;
            for (int j = 0; j < cpuStackedPieces.Length; j++)
            {
                if (cpuStackedPieces[j].Contains(i))
                {
                    isCarried = true;
                    break;
                }
            }
            // 업힌 말이라면 이동 대상에서 제외
            if (isCarried)
            {
                Debug.Log($"말 {i}는 업힌 상태이므로 이동에서 제외됩니다.");
                continue;
            }
            /*
            // 대기 중인 말이라면 이동 가능
            if (currentPosition == -1)
            {
                int potentialPosition = -1 + count; // 시작점부터 이동
                if (bestPieceIndex == -1 || !IsSafePosition(potentialPosition)) // 안전한 위치 확인
                {
                    bestPieceIndex = i;
                    bestTargetPosition = potentialPosition;
                }
                continue;
            }
            // 이동 가능한 말들 중 최적의 움직임 결정
            int targetPosition = currentPosition + count;*/
            // 이동 가능한 말의 목표 위치 계산
            int targetPosition = currentPosition == -1 ? count - 1 : currentPosition + count;
            int currentScore = 0; // 현재 선택의 점수

            if (targetPosition == -2) //만약 시작 안했는데 백도라면
            {
                continue;
            }

            if (currentPosition == 4) // 4번에서 갈림길
            {
                isTwo = true;
                secondBestTarget = currentPosition + moveCount + 15;
            }
            else if (currentPosition == 22) // 지름길에서 갈림길
            {
                isTwo = true;
                if (moveCount >= 3) //만약 걸 이상이면
                {
                    targetPosition = currentPosition + moveCount - 11; //일반 경로 재설정
                }
                if (moveCount == 3) //만약 걸이면
                {
                    secondBestTarget = 19; //마지막 지점으로
                }
                else if (moveCount > 3) //만약 걸 이상이면
                {
                    secondBestTarget = 30; //마지막 지점으로
                }
                else if (moveCount < 3) //만약 도 개면
                {
                    secondBestTarget = currentPosition + moveCount + 5; //경로 재설정
                }
            }
            else if (currentPosition == 9) // 지름길에서 갈림길
            {
                isTwo = true;
                secondBestTarget = currentPosition + moveCount + 15;
                if (secondBestTarget == 27)
                {
                    secondBestTarget = 22; //중앙 넘버 재조정
                }
            }
            else if (currentPosition >= 15 && currentPosition <= 19)
            {
                if (targetPosition >= 20) //만약 골인이라면
                {
                    targetPosition = 30;
                }
            }
            else if (currentPosition >= 20 && currentPosition <= 24)
            {
                if (targetPosition >= 25) //만약 타겟포인트 오바되면
                {
                    targetPosition = currentPosition + moveCount - 11;
                }
            }
            else if (currentPosition == 25 || currentPosition == 26)
            {
                if (targetPosition == 27)
                {
                    targetPosition = 22; //중앙 넘버 재조정
                }
            }
            if (currentPosition >= 25 && currentPosition <= 29)
            {
                if (targetPosition == 30)
                {
                    targetPosition = 19;
                }
            }
            if (targetPosition == -1) //백도 걸렸을 시
            {
                targetPosition = 19;
            }


            if (isTwo) //만약 갈림길이면
            {
                int normalPathScore = EvaluateForkPath(targetPosition, currentPosition, moveCount, false);
                int shortcutPathScore = EvaluateForkPath(secondBestTarget, currentPosition, moveCount, true);

                Debug.Log($"말 {i}의 갈림길: 일반 경로 점수 = {normalPathScore}, 지름길 점수 = {shortcutPathScore}");

                if (shortcutPathScore > normalPathScore) //만약 지름길 점수가 크면
                {
                    targetPosition = secondBestTarget; //타겟포지션 변경
                }
            }

            // 최적의 말 선택 (예: 상대 말 잡기 또는 골인)
            // 골인 점수
            if (targetPosition >= 30)
            {
                currentScore += 500; // 골인은 최우선
                targetPosition = 30;
            }
            // 상대 말 잡기 점수
            if (CanCapture(targetPosition))
            {
                currentScore += 1500; // 상대방 말 잡으면 큰 이득
            }
            // 안전한 위치 점수
            if (IsSafePosition(targetPosition))
            {
                currentScore += 200; // 안전한 위치로 이동은 선호
            }

            // 위험한 말 보호 점수
            if (!IsSafePosition(currentPosition))
            {
                currentScore += 100; // 현재 위험한 말이라면 이동 우선
            }

            // 업힌 말 이동 점수
            if (cpuStackedPieces[i].Count > 0)
            {
                currentScore += 400; // 업힌 말은 효율적인 이동
            }

            // 최적의 선택 갱신
            if (currentScore > highestScore)
            {
                highestScore = currentScore;
                bestPieceIndex = i;
                bestTargetPosition = targetPosition;
            }
        }

        // 이동 실행
        if (bestPieceIndex != -1)
        {
            Debug.Log($"CPU가 말 {bestPieceIndex}를 {bestTargetPosition}으로 이동합니다.");

            MovePiece(bestPieceIndex, bestTargetPosition, false);

        }
        else
        {
            Debug.Log("CPU가 이동 가능한 말이 없습니다.");
            ChangeTurn(false); // 턴 넘기기
        }
    }

    // 안전한 위치 확인 (적의 공격 범위 피하기)
    private bool IsSafePosition(int position)
    {
        foreach (int playerPos in playerPositions)
        {
            if (playerPos != -1 && playerPos - position <= 4) // 공격 범위
            {
                return false;
            }
        }
        return true;
    }


    private void CheckWinCondition(bool isPlayer)
    {
        if (isPlayer && playerArrived == 3)
        {
            Debug.Log("플레이어 승리!");
            resultPanel.SetActive(true);
            resultPanel.GetComponent<FunResultPanel>().Render();
            gameStop = true;
        }
        else if (!isPlayer && cpuArrived == 3)
        {
            Debug.Log("CPU 승리!");
            resultPanel.SetActive(true);
            resultPanel.GetComponent<FunResultPanel>().Render(true);
            gameStop = true;

        }
    }


    private IEnumerator MovePieceSmoothly(Transform piece, Vector3 targetPosition, float speed = 0.5f)
    {
        // 방향 계산
        Vector3 direction = (targetPosition - piece.position).normalized;

        // 목표 지점에 도달할 때까지 이동
        while (Vector3.Distance(piece.position, targetPosition) > 0.01f) // 0.01f는 오차 허용값
        {
            piece.position += direction * speed * Time.deltaTime; // 일정 속도로 이동
            yield return null; // 다음 프레임 대기
        }
        piece.position = targetPosition; // 정확한 위치로 보정
    }


    #region ForkPoint     //갈림길 점수 계산 용도
    private int EvaluateForkPath(int targetPosition, int currentPosition, int moveCount, bool isShortcut)
    {
        int score = 0;

        // 골인 점수
        if (targetPosition >= 30)
        {
            score += 500; // 골인 시 큰 점수
        }

        // 상대 말 잡기
        if (CanCapture(targetPosition))
        {
            score += 1000; // 상대 말 잡으면 이점
        }

        // 지름길 우선 점수
        if (isShortcut)
        {
            score += 300; // 지름길은 빠른 골인 가능성
        }

        // 안전한 위치 점수
        if (IsSafePosition(targetPosition))
        {
            score += 200; // 안전한 위치는 선호
        }

        // 위험 회피 점수
        if (!IsSafePosition(currentPosition))
        {
            score += 100; // 현재 위험하다면 이동 선호
        }

        return score;
    }


    #endregion

    public void HelpOn()
    {
        helpPanel.SetActive(true);
    }
    public void HelpOff()
    {
        helpPanel.SetActive(false);
    }
    public void OnHelpButton(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                helpButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                helpButton = false;
                break;
        }
    }
}
