using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : UiPrompt
{
    [Header("Card")]
    [SerializeField] private bool[] card;
    [SerializeField]
    protected GameObject[] cardObject; //6���� ī��

    [SerializeField] private Sprite[] sprites; //�� ��Ȳ�� �׸�
    [SerializeField] private Sprite noneCard; //�ƹ��͵� ���� �ո� �׸�

    [SerializeField] private float startX = 0.25f; //ī�� �� ����
    [SerializeField] private float endX = 0.55f;   //�� ������

    private int badCount; //�ɸ��� ī�� ����
    private System.Action<bool> onCardResultCallback; //�ٸ� �Լ����� ȣ���Ҷ� bool �ǵ����� �뵵
    private int currentSituation; //���� ��Ȳ����
    private bool isCatch; //�ɷȴ��� ����

    public void PanelOn(int whatSituation, System.Action<bool> onResult,bool isCrimial = false) //�г� ������ ȣ��
    {
        currentSituation = whatSituation;
        onCardResultCallback = onResult;

        int cardNum = Mathf.Max((4 - PlayerStats.Difficulty), 2);  //�� ī�� ���� ���ϱ�
        card = new bool[cardNum]; //�迭 ���� �ٲٱ�

        for (int i = 0; i < cardObject.Length; i++) //ī�� ���� �̻��� ī�� ������Ʈ ��Ȱ��ȭ
        {
            if (i >= cardNum && cardObject[i] != null)
            {
                cardObject[i].SetActive(false);
            }
            else
            {
                cardObject[i].SetActive(true);
            }
        }

        for (int i = 0; i < cardNum; i++) //ī�� �������� ��ġ ������
        {
            float normalizedX = (cardNum == 1) ? 0.5f : i / (float)(cardNum - 1);
            float posX = Mathf.Lerp(startX, endX, normalizedX);

            cardObject[i].GetComponent<UIFixedAnimation>().finalV3.x = posX;
        }

        if (isCrimial) //���˶��
        {
            badCount = Mathf.Min(StatusManager.instance.GetCrimianl(), cardNum); //������ ī�� ���� ���ϱ�
            if (badCount == 0)
            {
                badCount = 1; //�ּ� 1�� ���߱�
            }

            if (StatusManager.instance.GetSelectedPerk(2, 1) == 0) //���� �� ����ٸ�
            {
                badCount /= 2; //Ȯ�� ���� ����
            }
        }
        else //���� �ƴ϶��
        {
            badCount = 1;
        }

        for (int i = 0; i < cardNum; i++) //�ʱ�ȭ
        {
            card[i] = false;
        }
        // badCount ����ŭ Ʈ�� �ֱ�
        for (int i = 0; i < badCount; i++)
        {
            card[i] = true;
        }

        // ��ġ�� ���� ���, �迭�� �������� ����
        card = card.OrderBy(_ => Random.value).ToArray();

        for (int i = 0; i < card.Length; i++)
        {
            if (card[i]) //���� �ش� ī�尡 �ɸ��� ī���
            {
                cardObject[i].GetComponent<Card>().frontCard = sprites[currentSituation];  //ī�� �ո� ä���ֱ�
            }
            else
            {
                cardObject[i].GetComponent<Card>().frontCard = noneCard;  //ī�� �ո� ä���ֱ�
            }
        }
        Invoke(nameof(ButtonOn), cardNum*0.5f); //3�� �ڿ� ��ư ������ ����
    }

    public void ClickCard(int index) //ī�� ���ý� ȣ��Ǵ� �Լ�
    {
        isCatch = card[index]; //��÷ ī������ ����

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = false;  //��� ī�� ��Ȱ��ȭ
            if (i != index && cardObject[i].activeSelf)
            {
                cardObject[i].GetComponent<Card>().AfterRotate(); //������ ī�� ������
            }
        }

        Invoke(nameof(PanelOff), 2f); //3�� �ڿ� ��ư ������ ����
        
    }

    public void PanelOff() //ī�� �г� ������ �Լ�, Ȯ�� ��ư�̶� �����Ұ�
    {
        // ��� �ݹ� ����
        onCardResultCallback?.Invoke(isCatch);
        gameObject.SetActive(false); //ī�� �г� ������
    }
    protected void ButtonOn() //��ư ������ �Լ�
    {
        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = true;  //ī�� ��ư Ȱ��ȭ
        }

    }
}
