using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScratchCard : MonoBehaviour
{
    public RawImage scratchImage; // ���� ���� ��ũ��ġ �̹���
    public Texture2D customScratchImage; // ����ڰ� ������ ��ũ��ġ �̹���
    public Image[] lotteryImages; // ���� �� ĭ�� �̹���
    public TextMeshProUGUI[] prizeTexts; // ���� �� ĭ�� �ݾ� �ؽ�Ʈ
    public Image[] prizeImage; // ���� �� ĭ�� �ݾ� �̹���
    public LotteryResult result; // ��� �ؽ�Ʈ
    public RectTransform scratchArea;


    private Texture2D scratchTexture; // �ܱ� ó���� ���� �ؽ�ó
    private bool isScratchingComplete = false; // �ܱ� �Ϸ� ����
    private float scratchedPercentage; // ���� ����
    private LotteryCard currentCard; // ���� ����

    // ���� �� �ִ� ���� ���� (512x512 �ؽ�ó ����)
    private Rect scratchableArea = new Rect(50, 100, 850, 480); // ����: �߽� �簢��
    private Vector2? previousMousePosition = null; // ���� ���콺 ��ġ
    private bool[,] scratchMask; // ���� �����͸� �����ϴ� �迭
    private bool needTextureUpdate = false; // �ؽ�ó ������Ʈ �÷���

    private int money;
    private Color32[] scratchPixels;

    void Start()
    {
        // ���� �� �ִ� ������ aaa ������Ʈ�� ũ��� ����
        //   UpdateScratchableArea();
        

        

        //InitializeScratchMask();

    }

    private void OnEnable()
    {
        isScratchingComplete = false;
        // ��ũ��ġ �ؽ�ó �ʱ�ȭ (����� ���� �̹��� ���)
        scratchTexture = new Texture2D(customScratchImage.width, customScratchImage.height, TextureFormat.ARGB32, false);
        scratchTexture.SetPixels(customScratchImage.GetPixels()); // ����� ���� �̹��� ����
        scratchTexture.Apply();
        scratchImage.texture = scratchTexture;
        InitializeScratchMask();
        CreateLottery();
    }

    public void CreateLottery()
    {
        // ���� ���� �� UI ����
        currentCard = GetComponent<LotteryManager>().CreateLottery(); // ����: 5���� ���� ����
        SetupLotteryUI(currentCard);
    }


    void Update()
    {
        if (Input.GetMouseButton(0) && !isScratchingComplete)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                scratchImage.rectTransform, Input.mousePosition, null, out localPoint);

            // Ư�� ������ �ܱ� ����
            if (IsWithinScratchableArea(localPoint))
            {
                if (previousMousePosition != null)
                {
                    // ���� ��ġ���� ���� ��ġ���� �ܱ�
                    ScratchBetweenPoints(previousMousePosition.Value, localPoint);
                }
                previousMousePosition = localPoint; // ���� ��ġ ������Ʈ
            }
        }
        else
        {
            previousMousePosition = null; // ���콺 ��ư�� ���� �ʱ�ȭ
        }
        // ���콺�� �� ���� ���� ���� ���
        // Apply�� ���⼭ 1�����ӿ� 1ȸ��
        if (needTextureUpdate)
        {
            scratchTexture.SetPixels32(scratchPixels);
            scratchTexture.Apply();
            needTextureUpdate = false;
        }

        if (Input.GetMouseButtonUp(0) && !isScratchingComplete)
        {
            scratchedPercentage = CalculateScratchedPercentage();
            if (scratchedPercentage >= 0.25f)
            {
                isScratchingComplete = true;
                ShowResult();
            }
        }
    }
    /// <summary>
    /// ��ũ��ġ ����ũ �ʱ�ȭ
    /// </summary>
    private void InitializeScratchMask()
    {
        scratchMask = new bool[scratchTexture.width, scratchTexture.height];
        scratchPixels = scratchTexture.GetPixels32(); // ��ü �ȼ� ĳ��
    }

    /// <summary>
    /// �� �� ���̸� �ܱ�
    /// </summary>
    private void ScratchBetweenPoints(Vector2 startPoint, Vector2 endPoint)
    {
        // �� �� ������ �Ÿ� ���
        float distance = Vector2.Distance(startPoint, endPoint);

        // ���� �������� ���� ���
        int steps = Mathf.CeilToInt(distance / 1f); // 1 �ȼ� �������� ����
        for (int i = 0; i <= steps; i++)
        {
            // ���� �����Ͽ� �� ��ġ ���
            Vector2 interpolatedPoint = Vector2.Lerp(startPoint, endPoint, i / (float)steps);
            ScratchAt(interpolatedPoint);
        }
    }

    /// <summary>
    /// Ư�� ���� �ȿ� �ִ��� Ȯ��
    /// </summary>
    private bool IsWithinScratchableArea(Vector2 localPoint)
    {
        // localPoint�� �ؽ�ó ��ǥ�� ��ȯ
        float x = (localPoint.x + scratchImage.rectTransform.rect.width / 2) * scratchTexture.width / scratchImage.rectTransform.rect.width;
        float y = (localPoint.y + scratchImage.rectTransform.rect.height / 2) * scratchTexture.height / scratchImage.rectTransform.rect.height;

        // ���� �� �ִ� ���� ������ Ȯ��
        return scratchableArea.Contains(new Vector2(x, y));
    }

    /// <summary>
    /// Ư�� ��ġ �ܱ�
    /// </summary>
    private void ScratchAt(Vector2 localPoint)
    {
        // ��ũ��ġ �ؽ�ó�� �ȼ� ��ǥ ���
        int x = (int)((localPoint.x + scratchImage.rectTransform.rect.width / 2) * scratchTexture.width / scratchImage.rectTransform.rect.width);
        int y = (int)((localPoint.y + scratchImage.rectTransform.rect.height / 2) * scratchTexture.height / scratchImage.rectTransform.rect.height);

        // ���� ������ ���� (20px)
        int radius = 28;
        int width = scratchTexture.width;
        int height = scratchTexture.height;


        // �ش� ��ġ �ֺ� �ȼ��� �� ������� �����ϰ� ����
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                float dist = Mathf.Sqrt(i * i + j * j);
                if (dist > radius) continue;

                int px = x + i;
                int py = y + j;

                if (px >= 0 && px < width && py >= 0 && py < height)
                {
                    if (scratchableArea.Contains(new Vector2(px, py)))
                    {
                        int index = py * width + px;
                        Color32 color = scratchPixels[index];
                        byte alphaReduction = (byte)(255 * (1f - dist / radius));
                        color.a = (byte)Mathf.Max(0, color.a - alphaReduction);
                        scratchPixels[index] = color;

                        scratchMask[px, py] = true;
                    }
                }
            }
        }
        needTextureUpdate = true; // Apply�� �� �����ӿ� �� ����
    }


    /// <summary>
    /// ���� ������ ���� ���
    /// </summary>
    private float CalculateScratchedPercentage()
    {
        int sampled = 0;
        int scratched = 0;

        int step = 4; // 4�ȼ� �������� ���ø�

        for (int x = 0; x < scratchTexture.width; x += step)
        {
            for (int y = 0; y < scratchTexture.height; y += step)
            {
                sampled++;
                if (scratchMask[x, y])
                    scratched++;
            }
        }

        return (float)scratched / sampled;
    }

    /// <summary>
    /// ���� UI �ʱ�ȭ
    /// </summary>
    private void SetupLotteryUI(LotteryCard card)
    {
        money = card.winningPrize;

        // ������ �̹��� �� �ݾ��� UI�� ����
        for (int i = 0; i < lotteryImages.Length; i++)
        {
            lotteryImages[i].sprite = card.images[i]; // ���� �̹����� ����        
        }
        for(int i = 0; i < 6; i++)
        {
            prizeTexts[i].text = "\u20A9" + card.prizeValues[i].ToString("N0"); // ������ �ݾ��� �ؽ�Ʈ�� ����

            prizeImage[i].sprite = card.priceImage[i];
        }
    }

    /// <summary>
    /// ��� ǥ��
    /// </summary>
    private void ShowResult()
    {

        result.gameObject.SetActive(true);
        result.Render(money);
    }
}