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
    private Rigidbody[] yutSticks; // �� ���� Rigidbody �迭
    [SerializeField]
    private float maxThrowForce = 10f; // ������ �ִ� ������ �����Ǵ� ��
    [SerializeField]
    public float rotationForce = 1f; // ȸ���� ����� ���� ȸ�� ��
    [SerializeField]
    private Transform throwStartPosition; // ���� ���� ���� ��ġ
    [SerializeField]
    private Collider boundaryCollider; // �ٴ� ���� 3���� �ݶ��̴�
    private bool isFoul = false; // �� ���� üũ

    private bool[] yutResults = new bool[4];     // ���� ��� (true: �ո�, false: �޸�)

    [SerializeField]
    private GameObject throwButton; //������ ��ư
    [SerializeField] private GameObject playerTurn, cpuTurn;

    [Header("YutBoard")]
    public Transform[] playerPieces; // �÷��̾��� �� 3��
    public Transform[] cpuPieces;     // CPU�� �� 3��
    public Button[] pieceButtons;    // �� ���� ��ư (3��)
    public int[] playerPositions;    // �� ���� ���� ��ġ (-1�� ��� ��)
    public int[] cpuPositions;        // CPU ���� ��ġ (-1: ��� ��)

    public List<Button> boardButtons; // ������ ��� ĭ ��ư
    public List<Transform> boardMap;  // �� ĭ�� ���� ��ġ
    public int moveCount;            // �� ������ �����

    private List<Button> highlightedButtons = new List<Button>(); // ���̶���Ʈ�� ��ư

    private Vector3[] playerPiecesStart; // �÷��̾��� �� 3�� ��������
    private Vector3[] cpuPiecesStart;     // CPU�� �� 3�� ��������
    private Vector3[] pieceButtonsStart; // ��ư�� �ʱ� ��ġ ����

    private int playerArrived = 0; // �÷��̾��� ���� �� ����
    private int cpuArrived = 0;    // CPU�� ���� �� ����
    private List<int>[] stackedPieces; // �� ���� ���� ������ �ε����� ����
    private List<int>[] cpuStackedPieces; // �� ���� ���� ������ �ε����� ����

    private bool isPlayerTurn;

    [SerializeField] private Canvas canvas; // ĵ������ ����
    [SerializeField] private GameObject highlightPrefab; // ���̶���Ʈ ��ư ������

    [SerializeField]
    protected GameObject resultPanel; //��� �г�

    public static YutnoriManager instance = null;
    public int money;
    public Status stat; //���� ������ �ִ� ���� ��ġ ����

    [SerializeField]
    protected GameObject[] doGaePanel; //��� �г�

    private bool gameStop;

    [Header("HelpButton")]
    [SerializeField] private GameObject helpPanel;
    private bool helpButton; //���� ��ư

    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }
    private void Start()
    {
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //�ð� �帣�°� ���߰��ϱ�
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
            if (Time.timeScale != 0) //�ð� �ȸ�����������
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
        // ��ư ��ġ�� ������ �迭 �ʱ�ȭ
        pieceButtonsStart = new Vector3[pieceButtons.Length];

        // ��ư���� �ʱ� ��ġ ����
        for (int i = 0; i < pieceButtons.Length; i++)
        {
            pieceButtonsStart[i] = pieceButtons[i].GetComponent<RectTransform>().position;
        }
    }
    private void InitializeStackedPieces() //���� ���� �ʱ�ȭ
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
        throwButton.SetActive(false); //��ư ���ֱ�
        // ������ ���� ���� �� ���
        float throwForce = gauge * maxThrowForce + 1.5f;

        // �� ���븦 ���� ��ġ�� �����ϰ�, ����
        for(int i = 0; i < yutSticks.Length; i++)
        {
            yutSticks[i].velocity = Vector3.zero; // ���� �ӵ� �ʱ�ȭ
            yutSticks[i].angularVelocity = Vector3.zero; // ���� ȸ�� �ʱ�ȭ
            yutSticks[i].transform.position = throwStartPosition.position + new Vector3(0.04f * i,0f,0f); // ���� ��ġ ����
            yutSticks[i].transform.rotation = throwStartPosition.rotation; // �ʱ� ȸ���� ����


            Vector3 throwDirection = new Vector3(Random.Range(-0.2f, 0.2f), 1f, Random.Range(-0.2f, 0.2f)).normalized;
            // ���� �������� �߰� (���� ������ Vector3.up)
            yutSticks[i].AddForce(throwDirection * throwForce, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3( //���� ȸ���� �߰�
                Random.Range(-rotationForce * 2, rotationForce * 2),
                Random.Range(-rotationForce, rotationForce),
                Random.Range(-rotationForce, rotationForce)
            );
            yutSticks[i].AddRelativeTorque(randomTorque, ForceMode.VelocityChange);
        }
        SoundManager.instance.PlaySound2D("drop-stick");

        // �� üũ ����
        StartCoroutine(CheckOutOfBounds());
    }

    private IEnumerator CheckOutOfBounds()
    {
        isFoul = false;

        // ���� ������ �� �� ���θ� �Ǵ��ϱ� ���� 3�� ���� ���
        yield return new WaitForSeconds(1.5f);

        foreach (Rigidbody yut in yutSticks)
        {
            if (!boundaryCollider.bounds.Contains(yut.transform.position))
            {
                isFoul = true;
                doGaePanel[6].SetActive(true); //�� ����
                yield return new WaitForSeconds(1f);
                doGaePanel[6].SetActive(false); //�� ����

                ChangeTurn(false); //�� �ѱ��
                yield break; // �ڷ�ƾ ����
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
            return; //���� �� ����Ǹ� ����
        }

        int frontCount = 0;  // �ո� ����
        //int backCount = 0;   // �޸� ����

        // ���� ����� �Ǻ�
        for (int i = 0; i < yutSticks.Length; i++)
        {
            // ���� �ո����� �޸����� �Ǻ��ϴ� ���� (��: Rigidbody�� ������ Ư�� �� Ȯ��)
            // �� ���ÿ����� �ܼ��� ���Ƿ� true/false�� �Ҵ�
            // �����δ� ���� ������ ������� �Ǵ��ϰų�, �ո�/�޸��� �ĺ��� �� �ִ� ����� �߰��ؾ� ��

            // ���÷�, �� ���� Y�� ������ �������� �ո�/�޸��� �Ǻ�
            if ((Mathf.Abs(GetZRotation(yutSticks[i].transform.rotation)) > 90 && Mathf.Abs(GetXRotation(yutSticks[i].transform.rotation)) > 130)||
                (Mathf.Abs(GetZRotation(yutSticks[i].transform.rotation)) < 90 && Mathf.Abs(GetXRotation(yutSticks[i].transform.rotation)) < 50))
            {
                yutResults[i] = true; // �ո�
                frontCount++;
            }
            else
            {
                yutResults[i] = false; // �޸�
                //backCount++;
            }
        }

        // �� ��� �Ǻ�
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
        yield return new WaitForSeconds(delay); // 1�� ��ٸ�
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
        // ȸ�� ����� ����� Z�� ȸ������ ����
        float zRotation = rotation.eulerAngles.z;
        if (zRotation > 180)  // -180 ~ 180���� ��ȯ
        {
            zRotation -= 360;
        }
        return zRotation;
    }
    private float GetXRotation(Quaternion rotation)
    {
        // ȸ�� ����� ����� Z�� ȸ������ ����
        float xRotation = rotation.eulerAngles.x;
        if (xRotation > 180)  // -180 ~ 180���� ��ȯ
        {
            xRotation -= 360;
        }
        return xRotation;
    }
    #endregion

    // �� ������ ����� ���� �� ȣ��
    // �� ������ ����� �ް� �̵� ���μ��� ����
    public void OnYutThrowResult(int moveCount)
    {
        this.moveCount = moveCount; // �� ��� ����

        if (isPlayerTurn)
        {
            ShowPieceSelection(moveCount); // �̵��� �� ���� UI ǥ��
        }
        else
        {
            CpuWhereMove(moveCount);
        }
    }
    // �� ���� UI ǥ��
    private void ShowPieceSelection(int moveCount)
    {
        int howMove = 0;
        for (int i = 0; i < playerPieces.Length; i++)
        {
            int pieceIndex = i; // Closure ���� ����

            // ���� �������� Ȯ��
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
                // �� ���� ��ư Ȱ��ȭ
                pieceButtons[i].gameObject.SetActive(true);
                pieceButtons[i].onClick.RemoveAllListeners();
                pieceButtons[i].onClick.AddListener(() => OnPieceSelected(pieceIndex, moveCount));
            }
            else
            {
                pieceButtons[i].gameObject.SetActive(false); // �̵� �Ұ����� ���� ���� �Ұ�
                howMove++;
            }
        }
        if(howMove == 3) //���� ������ �� �ִ°� ������
        {
            Debug.Log("������ �� �ִ� ���� �����ϴ�.");
            ChangeTurn(false);
        }
    }

    // �̵� ���ɼ� �˻�
    private bool CanMove(int currentPosition, int moveCount)
    {
        if (moveCount == -1) return currentPosition >= 0; // �鵵�� 0 �̻��� ���� ����
        return true; // �ٸ� ��� �⺻������ �̵� ����
    }

    // �� ���� UI ǥ��
    // ������ ���� �� ����� �̵� ������ ĭ ǥ��
    private void OnPieceSelected(int pieceIndex, int moveCount) //Ŭ���� ����� �Լ�
    {
        ShowPieceSelection(moveCount); //�ٽ� �ٸ� �ֵ� ȭ��ǥ ����
        pieceButtons[pieceIndex].gameObject.SetActive(false); //�ڽ��� ȭ��ǥ �����

        ClearHighlights(); // ���� ���� �ʱ�ȭ

        int currentPosition = playerPositions[pieceIndex];

        // ��� ���� ���̸� �������� ���̶���Ʈ
        /* if (currentPosition == -1)
         {
             HighlightPosition(0); // ������
             return;
         }*/
        /*
         // �鵵 ó��
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
        // �̵� ������ ���� �� ������ ������ ����

        bool isTwo = false; //���� ����������
        // ������ ó��
        if (currentPosition == 4) // 4������ ������
        {
            isTwo = true;
            secondTargetPo = currentPosition + moveCount +15;
        }
        else if (currentPosition == 22) // �����濡�� ������
        {
            isTwo = true;
            if(moveCount >= 3) //���� �� �̻��̸�
            {
                targetPosition = currentPosition + moveCount - 11; //�Ϲ� ��� �缳��
            }
            if (moveCount == 3) //���� ���̸�
            {
                secondTargetPo = 19; //������ ��������
            }
            else if (moveCount > 3) //���� �� �̻��̸�
            {
                secondTargetPo = 30; //������ ��������
            }
            else if (moveCount < 3) //���� �� ����
            {
                secondTargetPo = currentPosition + moveCount + 5; //��� �缳��
            }
        }
        else if (currentPosition == 9) // �����濡�� ������
        {
            isTwo = true;
            secondTargetPo = currentPosition + moveCount +15;
            if(secondTargetPo == 27)
            {
                secondTargetPo = 22; //�߾� �ѹ� ������
            }
        }else if (currentPosition >= 15 && currentPosition <= 19)
        {
            if(targetPosition >= 20) //���� �����̶��
            {
                targetPosition = 30;
            }        
        }else if (currentPosition >= 20 && currentPosition <= 24)
        {
            if (targetPosition >= 25) //���� Ÿ������Ʈ ���ٵǸ�
            {
                targetPosition = currentPosition + moveCount - 11;
            }
        }else if(currentPosition == 25 || currentPosition == 26)
        {
            if(targetPosition == 27)
            {
                targetPosition = 22; //�߾� �ѹ� ������
            }
        }
        if(currentPosition >= 25 && currentPosition <= 29)
        {
            if(targetPosition == 30)
            {
                targetPosition = 19;
            }
        }
        if (targetPosition == -1 || targetPosition == -2) //�鵵 �ɷ��� ��
        {
            targetPosition = 19;
        }

        HighlightPosition(targetPosition); // �Ϲ� ���
        if (isTwo)
        {
            HighlightPosition(secondTargetPo); //������ ���
        }
        // �̵� ������ ��ư�� Ŭ�� �̺�Ʈ �߰�
        foreach (var button in highlightedButtons)
        {
            button.onClick.RemoveAllListeners();

            // ��ư�� ��ġ ���� ��������
            int buttonPosition = button.GetComponent<HighlightData>().Position;

            // �Ϲ� ��� ��ư�� ���
            if (buttonPosition == targetPosition || (targetPosition > 30 && buttonPosition == 30))
            {
                button.onClick.AddListener(() => MovePiece(pieceIndex, targetPosition, true)); // �Ϲ� ��η� �̵�
            }
            // ������ ��� ��ư�� ���
            else if (isTwo && buttonPosition == secondTargetPo)
            {
                button.onClick.AddListener(() => MovePiece(pieceIndex, secondTargetPo, true)); // ������ ��η� �̵�
            }
        }
    }

    // Ư�� ĭ�� ���̶���Ʈ
    private void HighlightPosition(int position)
    {
        if(position > 30)
        {
            position = 30; //���� ���������� ũ�� �������� ����
        }
        /*if (boardButtons[position] != null)
        {
            boardButtons[position].gameObject.SetActive(true);

            highlightedButtons.Add(boardButtons[position]);
        }*/
        if (position < 0 || position >= 31)
            return;

        // ��ư ���� �� �ʱ�ȭ
        Button highlightButton = Instantiate(highlightPrefab, canvas.transform).GetComponent<Button>();
        highlightButton.transform.position = boardButtons[position].transform.position; // ��ư ��ġ�� ����

        // ��ư�� ��ġ �����͸� �߰�
        highlightButton.gameObject.AddComponent<HighlightData>().Position = position;

        // ���̶���Ʈ�� ��ư ����Ʈ�� �߰�
        highlightedButtons.Add(highlightButton);
    }

    // ��� ���̶���Ʈ ����
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

    // ���õ� ���� �̵�
    public void MovePiece(int pieceIndex, int targetPosition, bool isPlayer)
    {
        if (targetPosition >= 30) 
        { 
            targetPosition = 30;

            if (isPlayer)
            {
                // �� �����
                playerPieces[pieceIndex].gameObject.SetActive(false);
                playerArrived++;
                // ���� ���� ��ġ ����
                playerPositions[pieceIndex] = -2;

                // ���� ���� �Բ� ���� ó��
                foreach (int stackedIndex in stackedPieces[pieceIndex])
                {
                    playerPieces[stackedIndex].gameObject.SetActive(false);
                    playerArrived++;
                    Debug.Log($"���� �� {stackedIndex}�� �����߽��ϴ�.");
                    playerPositions[stackedIndex] = -2;
                }

                // �¸� ���� Ȯ��
                CheckWinCondition(true);
                // ���� �ʱ�ȭ �� �� ����
                ClearHighlights();
                ClearPieceButton();

                ChangeTurn(false); //�� �Ǵ�
                return;
            }
            else
            {
                // �� �����
                cpuPieces[pieceIndex].gameObject.SetActive(false);
                cpuArrived++;
                // ���� ���� ��ġ ����
                cpuPositions[pieceIndex] = -2;

                // ���� ���� �Բ� ���� ó��
                foreach (int stackedIndex in cpuStackedPieces[pieceIndex])
                {
                    cpuPieces[stackedIndex].gameObject.SetActive(false);
                    cpuArrived++;
                    Debug.Log($"���� �� {stackedIndex}�� �����߽��ϴ�.");
                    cpuPositions[stackedIndex] = -2;
                }

                // �¸� ���� Ȯ��
                CheckWinCondition(false);
                // ���� �ʱ�ȭ �� �� ����
                ClearHighlights();
                ClearPieceButton();

                ChangeTurn(false); //�� �Ǵ�
                return; 
            }
        }
        // �� �̵�

        if (isPlayer)
        {
           // playerPieces[pieceIndex].position = boardMap[targetPosition].position;
            StartCoroutine(MovePieceSmoothly(playerPieces[pieceIndex], boardMap[targetPosition].position)); // speed ���� �����Ͽ� �̵� �ӵ� ����

            // ���� ���� ��ġ ����
            playerPositions[pieceIndex] = targetPosition;

            // ���� �� �̵� ó��
            foreach (int stackedIndex in stackedPieces[pieceIndex])
            {
                playerPositions[stackedIndex] = targetPosition;
               // playerPieces[stackedIndex].position = boardMap[targetPosition].position; // ���� ���� ������ ��ġ�� �̵�
                StartCoroutine(MovePieceSmoothly(playerPieces[stackedIndex], boardMap[targetPosition].position));
                Debug.Log($"���� �� {stackedIndex}��(��) �Բ� �̵��߽��ϴ�.");

                pieceButtons[stackedIndex].GetComponent<RectTransform>().position =
                boardButtons[targetPosition].GetComponent<RectTransform>().position;
            }

            //�� ��ư �ű��
            pieceButtons[pieceIndex].GetComponent<RectTransform>().position =
            boardButtons[targetPosition].GetComponent<RectTransform>().position;
        }
        else
        {
            //cpuPieces[pieceIndex].position = boardMap[targetPosition].position;
            StartCoroutine(MovePieceSmoothly(cpuPieces[pieceIndex], boardMap[targetPosition].position));
            // ���� ���� ��ġ ����
            cpuPositions[pieceIndex] = targetPosition;

            // ���� �� �̵� ó��
            foreach (int stackedIndex in cpuStackedPieces[pieceIndex])
            {
                cpuPositions[stackedIndex] = targetPosition;
               // cpuPieces[stackedIndex].position = boardMap[targetPosition].position; // ���� ���� ������ ��ġ�� �̵�
                StartCoroutine(MovePieceSmoothly(cpuPieces[stackedIndex], boardMap[targetPosition].position));

                Debug.Log($"���� �� {stackedIndex}��(��) �Բ� �̵��߽��ϴ�.");
            }
        }

        bool isOneMore = false;
        // ��� �� ���
        if (CaptureOpponent(targetPosition, isPlayer) || moveCount >=4) //���� ��Ҵٸ�, �ƴϸ� �� �̻��̸�
        {
            isOneMore = true;
        }

        // ���� ó��
        StackPiece(targetPosition, isPlayer, pieceIndex);


        // ���� �ʱ�ȭ �� �� ����
        ClearHighlights();
        ClearPieceButton();

        ChangeTurn(isOneMore); //�� �Ǵ�
    }
    private void ChangeTurn(bool oneMore) //���⼭ �� �ѱ��� �ѹ� ���ϴ��� ����
    {
        if (!oneMore) //���� �ѹ��� �ƴ϶��
        {
            Debug.Log("�� ����. ���� �÷��̾��� �����Դϴ�!");
            isPlayerTurn = !isPlayerTurn;
        }
        else
        {
            Debug.Log("�� �ѹ���!");
        }
        EndTurn();
    }
    // �� ���� ó��
    private void EndTurn()
    {
        if (isPlayerTurn)
        {
            throwButton.SetActive(true); //������
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

    private bool CanCapture(int position) //��� �� �������ִ��� �Ǵ��ϴ� �ڵ�
    {
        int[] opponentPositions = playerPositions;

        // ��� ���� �ش� ��ġ�� �ִ��� Ȯ��
        for (int i = 0; i < opponentPositions.Length; i++)
        {
            if (opponentPositions[i] == position)
            {
                return true; // ���� �� �ִ� ���� ����
            }
        }
        return false; // ���� �� �ִ� �� ����
    }

    // ��� �� ��� ó��
    private bool CaptureOpponent(int position, bool isPlayer)
    {
        bool isCatch = false;
        int[] opponentPositions = isPlayer ? cpuPositions : playerPositions;
        Transform[] opponentPieces = isPlayer ? cpuPieces : playerPieces;
        List<int>[] opponentStackedPieces = isPlayer ? cpuStackedPieces : stackedPieces; // ��� ���� ����

        for (int i = 0; i < opponentPositions.Length; i++)
        {
            if (opponentPositions[i] == position)
            {
                opponentPositions[i] = -1; // ���� �� ��� ���·� �̵�
                opponentPieces[i].position = isPlayer ? cpuPiecesStart[i] : playerPiecesStart[i]; // ��� ��ġ
                Debug.Log($"��� �� ����! ��ġ: {position}");
                isCatch = true;
                if (!isPlayer)
                {
                    pieceButtons[i].GetComponent<RectTransform>().position = pieceButtonsStart[i];
                }

                // �ش� ���� ���� ���� �Բ� ó��
                foreach (int stackedIndex in opponentStackedPieces[i])
                {
                    opponentPositions[stackedIndex] = -1; // ���� ���� ��� ���·� �̵�
                    opponentPieces[stackedIndex].position = isPlayer ? cpuPiecesStart[stackedIndex] : playerPiecesStart[stackedIndex]; // ��� ��ġ
                    Debug.Log($"���� ��� �� {stackedIndex}�� �Բ� ����!");
                    if (!isPlayer)
                    {
                        pieceButtons[stackedIndex].GetComponent<RectTransform>().position = pieceButtonsStart[stackedIndex];
                    }
                }

                // ���� �ʱ�ȭ
                opponentStackedPieces[i].Clear();
            }
        }
        return isCatch;
    }

    // ���� ó��
    private void StackPiece(int position, bool isPlayer, int currentPieceIndex)
    {
        /*
        if (isPlayer)
        {
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // �ڽŰ� ���� ���� ���� �� ������, �̹� ���ÿ� ���� ��쿡�� �߰�
                // ���� �̵� ���� ���� �����ϰ� ���� ��ġ�� �ٸ� ���� �ִ��� Ȯ��
                if (i != currentPieceIndex && playerPositions[i] == position && !stackedPieces[currentPieceIndex].Contains(i))
                {
                    Debug.Log($"�� {i}��(��) �� {currentPieceIndex}�� �������ϴ�.");
                    stackedPieces[currentPieceIndex].Add(i); // ���� �� �߰�
                }
            }
        }
        else
        {
            for (int i = 0; i < cpuPositions.Length; i++)
            {
                // ���� �̵� ���� ���� �����ϰ� ���� ��ġ�� �ٸ� ���� �ִ��� Ȯ��
                if (i != currentPieceIndex && cpuPositions[i] == position && !cpuStackedPieces[currentPieceIndex].Contains(i))
                {
                    Debug.Log($"�� {i}��(��) �� {currentPieceIndex}�� �������ϴ�.");
                    cpuStackedPieces[currentPieceIndex].Add(i); // ���� �� �߰�
                }
            }
        }*/
        if (isPlayer)
        {
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // �ڽŰ� ���� ���� ���� �� ������, �̹� ���ÿ� ���� ��쿡�� �߰�
                // ���� �̵� ���� ���� �����ϰ� ���� ��ġ�� �ٸ� ���� �ִ��� Ȯ��
                if (i != currentPieceIndex && playerPositions[i] == position)
                {
                    // ���� ���� �ٸ� ���ÿ��� �̵��ؾ� �� ��� ó��
                    RemoveFromPreviousStack(i, stackedPieces);

                    // ���� ���ÿ� �߰�
                    Debug.Log($"�� {i}��(��) �� {currentPieceIndex}�� �����ϴ�.");
                    stackedPieces[currentPieceIndex].Add(i);

                    // ���� ���� ��(i)�� ������ ������ ���� currentPieceIndex�� �߰�
                    foreach (int subStacked in stackedPieces[i])
                    {
                        Debug.Log($"�� {subStacked}�� �� {currentPieceIndex}�� �����ϴ�.");
                        stackedPieces[currentPieceIndex].Add(subStacked);
                    }

                    // ���� ��(i)�� ���� �ʱ�ȭ (i�� ���� currentPieceIndex�� ���Ե�)
                    stackedPieces[i].Clear();
                }
            }
        }
        else
        {
            for (int i = 0; i < cpuPositions.Length; i++)
            {
                // ���� �̵� ���� ���� �����ϰ� ���� ��ġ�� �ٸ� ���� �ִ��� Ȯ��
                if (i != currentPieceIndex && cpuPositions[i] == position)
                {
                    // ���� ���� �ٸ� ���ÿ��� �̵��ؾ� �� ��� ó��
                    RemoveFromPreviousStack(i, cpuStackedPieces);

                    // ���� ���ÿ� �߰�
                    Debug.Log($"CPU �� {i}��(��) CPU �� {currentPieceIndex}�� �����ϴ�.");
                    cpuStackedPieces[currentPieceIndex].Add(i);

                    // ���� ���� ��(i)�� ������ ������ ���� currentPieceIndex�� �߰�
                    foreach (int subStacked in cpuStackedPieces[i])
                    {
                        Debug.Log($"CPU �� {subStacked}�� CPU �� {currentPieceIndex}�� �����ϴ�.");
                        cpuStackedPieces[currentPieceIndex].Add(subStacked);
                    }

                    // ���� ��(i)�� ���� �ʱ�ȭ
                    cpuStackedPieces[i].Clear();
                }
            }
        }
    }
    // ���� ���ÿ��� �ش� ���� �����ϴ� �Լ�
    private void RemoveFromPreviousStack(int pieceIndex, List<int>[] stackedPiecesArray)
    {
        for (int i = 0; i < stackedPiecesArray.Length; i++)
        {
            if (stackedPiecesArray[i].Contains(pieceIndex))
            {
                Debug.Log($"�� {pieceIndex}��(��) ���� ���� {i}���� ���ŵ˴ϴ�.");
                stackedPiecesArray[i].Remove(pieceIndex);
                break;
            }
        }
    }
    // CPU �̵�
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
        bool isTwo = false; // ������ ����
        int highestScore = int.MinValue; // ���� ���� ����

        // ���� ��ġ�� �������� �̵� ������ ���� Ž��
        for (int i = 0; i < cpuPositions.Length; i++)
        {
            isTwo = false;
            int currentPosition = cpuPositions[i];

            // �̹� ������ ���� ��ŵ
            if (currentPosition == -2)
                continue;


            // **���� ������ Ȯ��**
            bool isCarried = false;
            for (int j = 0; j < cpuStackedPieces.Length; j++)
            {
                if (cpuStackedPieces[j].Contains(i))
                {
                    isCarried = true;
                    break;
                }
            }
            // ���� ���̶�� �̵� ��󿡼� ����
            if (isCarried)
            {
                Debug.Log($"�� {i}�� ���� �����̹Ƿ� �̵����� ���ܵ˴ϴ�.");
                continue;
            }
            /*
            // ��� ���� ���̶�� �̵� ����
            if (currentPosition == -1)
            {
                int potentialPosition = -1 + count; // ���������� �̵�
                if (bestPieceIndex == -1 || !IsSafePosition(potentialPosition)) // ������ ��ġ Ȯ��
                {
                    bestPieceIndex = i;
                    bestTargetPosition = potentialPosition;
                }
                continue;
            }
            // �̵� ������ ���� �� ������ ������ ����
            int targetPosition = currentPosition + count;*/
            // �̵� ������ ���� ��ǥ ��ġ ���
            int targetPosition = currentPosition == -1 ? count - 1 : currentPosition + count;
            int currentScore = 0; // ���� ������ ����

            if (targetPosition == -2) //���� ���� ���ߴµ� �鵵���
            {
                continue;
            }

            if (currentPosition == 4) // 4������ ������
            {
                isTwo = true;
                secondBestTarget = currentPosition + moveCount + 15;
            }
            else if (currentPosition == 22) // �����濡�� ������
            {
                isTwo = true;
                if (moveCount >= 3) //���� �� �̻��̸�
                {
                    targetPosition = currentPosition + moveCount - 11; //�Ϲ� ��� �缳��
                }
                if (moveCount == 3) //���� ���̸�
                {
                    secondBestTarget = 19; //������ ��������
                }
                else if (moveCount > 3) //���� �� �̻��̸�
                {
                    secondBestTarget = 30; //������ ��������
                }
                else if (moveCount < 3) //���� �� ����
                {
                    secondBestTarget = currentPosition + moveCount + 5; //��� �缳��
                }
            }
            else if (currentPosition == 9) // �����濡�� ������
            {
                isTwo = true;
                secondBestTarget = currentPosition + moveCount + 15;
                if (secondBestTarget == 27)
                {
                    secondBestTarget = 22; //�߾� �ѹ� ������
                }
            }
            else if (currentPosition >= 15 && currentPosition <= 19)
            {
                if (targetPosition >= 20) //���� �����̶��
                {
                    targetPosition = 30;
                }
            }
            else if (currentPosition >= 20 && currentPosition <= 24)
            {
                if (targetPosition >= 25) //���� Ÿ������Ʈ ���ٵǸ�
                {
                    targetPosition = currentPosition + moveCount - 11;
                }
            }
            else if (currentPosition == 25 || currentPosition == 26)
            {
                if (targetPosition == 27)
                {
                    targetPosition = 22; //�߾� �ѹ� ������
                }
            }
            if (currentPosition >= 25 && currentPosition <= 29)
            {
                if (targetPosition == 30)
                {
                    targetPosition = 19;
                }
            }
            if (targetPosition == -1) //�鵵 �ɷ��� ��
            {
                targetPosition = 19;
            }


            if (isTwo) //���� �������̸�
            {
                int normalPathScore = EvaluateForkPath(targetPosition, currentPosition, moveCount, false);
                int shortcutPathScore = EvaluateForkPath(secondBestTarget, currentPosition, moveCount, true);

                Debug.Log($"�� {i}�� ������: �Ϲ� ��� ���� = {normalPathScore}, ������ ���� = {shortcutPathScore}");

                if (shortcutPathScore > normalPathScore) //���� ������ ������ ũ��
                {
                    targetPosition = secondBestTarget; //Ÿ�������� ����
                }
            }

            // ������ �� ���� (��: ��� �� ��� �Ǵ� ����)
            // ���� ����
            if (targetPosition >= 30)
            {
                currentScore += 500; // ������ �ֿ켱
                targetPosition = 30;
            }
            // ��� �� ��� ����
            if (CanCapture(targetPosition))
            {
                currentScore += 1500; // ���� �� ������ ū �̵�
            }
            // ������ ��ġ ����
            if (IsSafePosition(targetPosition))
            {
                currentScore += 200; // ������ ��ġ�� �̵��� ��ȣ
            }

            // ������ �� ��ȣ ����
            if (!IsSafePosition(currentPosition))
            {
                currentScore += 100; // ���� ������ ���̶�� �̵� �켱
            }

            // ���� �� �̵� ����
            if (cpuStackedPieces[i].Count > 0)
            {
                currentScore += 400; // ���� ���� ȿ������ �̵�
            }

            // ������ ���� ����
            if (currentScore > highestScore)
            {
                highestScore = currentScore;
                bestPieceIndex = i;
                bestTargetPosition = targetPosition;
            }
        }

        // �̵� ����
        if (bestPieceIndex != -1)
        {
            Debug.Log($"CPU�� �� {bestPieceIndex}�� {bestTargetPosition}���� �̵��մϴ�.");

            MovePiece(bestPieceIndex, bestTargetPosition, false);

        }
        else
        {
            Debug.Log("CPU�� �̵� ������ ���� �����ϴ�.");
            ChangeTurn(false); // �� �ѱ��
        }
    }

    // ������ ��ġ Ȯ�� (���� ���� ���� ���ϱ�)
    private bool IsSafePosition(int position)
    {
        foreach (int playerPos in playerPositions)
        {
            if (playerPos != -1 && playerPos - position <= 4) // ���� ����
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
            Debug.Log("�÷��̾� �¸�!");
            resultPanel.SetActive(true);
            resultPanel.GetComponent<FunResultPanel>().Render();
            gameStop = true;
        }
        else if (!isPlayer && cpuArrived == 3)
        {
            Debug.Log("CPU �¸�!");
            resultPanel.SetActive(true);
            resultPanel.GetComponent<FunResultPanel>().Render(true);
            gameStop = true;

        }
    }


    private IEnumerator MovePieceSmoothly(Transform piece, Vector3 targetPosition, float speed = 0.5f)
    {
        // ���� ���
        Vector3 direction = (targetPosition - piece.position).normalized;

        // ��ǥ ������ ������ ������ �̵�
        while (Vector3.Distance(piece.position, targetPosition) > 0.01f) // 0.01f�� ���� ��밪
        {
            piece.position += direction * speed * Time.deltaTime; // ���� �ӵ��� �̵�
            yield return null; // ���� ������ ���
        }
        piece.position = targetPosition; // ��Ȯ�� ��ġ�� ����
    }


    #region ForkPoint     //������ ���� ��� �뵵
    private int EvaluateForkPath(int targetPosition, int currentPosition, int moveCount, bool isShortcut)
    {
        int score = 0;

        // ���� ����
        if (targetPosition >= 30)
        {
            score += 500; // ���� �� ū ����
        }

        // ��� �� ���
        if (CanCapture(targetPosition))
        {
            score += 1000; // ��� �� ������ ����
        }

        // ������ �켱 ����
        if (isShortcut)
        {
            score += 300; // �������� ���� ���� ���ɼ�
        }

        // ������ ��ġ ����
        if (IsSafePosition(targetPosition))
        {
            score += 200; // ������ ��ġ�� ��ȣ
        }

        // ���� ȸ�� ����
        if (!IsSafePosition(currentPosition))
        {
            score += 100; // ���� �����ϴٸ� �̵� ��ȣ
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
