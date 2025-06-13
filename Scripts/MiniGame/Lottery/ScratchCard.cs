using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScratchCard : MonoBehaviour
{
    public RawImage scratchImage; // 위에 덮는 스크래치 이미지
    public Texture2D customScratchImage; // 사용자가 설정한 스크래치 이미지
    public Image[] lotteryImages; // 복권 각 칸의 이미지
    public TextMeshProUGUI[] prizeTexts; // 복권 각 칸의 금액 텍스트
    public Image[] prizeImage; // 복권 각 칸의 금액 이미지
    public LotteryResult result; // 결과 텍스트
    public RectTransform scratchArea;


    private Texture2D scratchTexture; // 긁기 처리를 위한 텍스처
    private bool isScratchingComplete = false; // 긁기 완료 여부
    private float scratchedPercentage; // 긁은 비율
    private LotteryCard currentCard; // 현재 복권

    // 긁을 수 있는 영역 설정 (512x512 텍스처 기준)
    private Rect scratchableArea = new Rect(50, 100, 850, 480); // 예제: 중심 사각형
    private Vector2? previousMousePosition = null; // 이전 마우스 위치
    private bool[,] scratchMask; // 긁힌 데이터를 저장하는 배열
    private bool needTextureUpdate = false; // 텍스처 업데이트 플래그

    private int money;
    private Color32[] scratchPixels;

    void Start()
    {
        // 긁을 수 있는 영역을 aaa 오브젝트의 크기로 설정
        //   UpdateScratchableArea();
        

        

        //InitializeScratchMask();

    }

    private void OnEnable()
    {
        isScratchingComplete = false;
        // 스크래치 텍스처 초기화 (사용자 설정 이미지 기반)
        scratchTexture = new Texture2D(customScratchImage.width, customScratchImage.height, TextureFormat.ARGB32, false);
        scratchTexture.SetPixels(customScratchImage.GetPixels()); // 사용자 설정 이미지 복사
        scratchTexture.Apply();
        scratchImage.texture = scratchTexture;
        InitializeScratchMask();
        CreateLottery();
    }

    public void CreateLottery()
    {
        // 복권 생성 및 UI 설정
        currentCard = GetComponent<LotteryManager>().CreateLottery(); // 예제: 5만원 복권 생성
        SetupLotteryUI(currentCard);
    }


    void Update()
    {
        if (Input.GetMouseButton(0) && !isScratchingComplete)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                scratchImage.rectTransform, Input.mousePosition, null, out localPoint);

            // 특정 영역만 긁기 가능
            if (IsWithinScratchableArea(localPoint))
            {
                if (previousMousePosition != null)
                {
                    // 이전 위치에서 현재 위치까지 긁기
                    ScratchBetweenPoints(previousMousePosition.Value, localPoint);
                }
                previousMousePosition = localPoint; // 이전 위치 업데이트
            }
        }
        else
        {
            previousMousePosition = null; // 마우스 버튼을 떼면 초기화
        }
        // 마우스를 뗄 때만 긁은 비율 계산
        // Apply를 여기서 1프레임에 1회만
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
    /// 스크래치 마스크 초기화
    /// </summary>
    private void InitializeScratchMask()
    {
        scratchMask = new bool[scratchTexture.width, scratchTexture.height];
        scratchPixels = scratchTexture.GetPixels32(); // 전체 픽셀 캐싱
    }

    /// <summary>
    /// 두 점 사이를 긁기
    /// </summary>
    private void ScratchBetweenPoints(Vector2 startPoint, Vector2 endPoint)
    {
        // 두 점 사이의 거리 계산
        float distance = Vector2.Distance(startPoint, endPoint);

        // 일정 간격으로 점을 찍기
        int steps = Mathf.CeilToInt(distance / 1f); // 1 픽셀 간격으로 보간
        for (int i = 0; i <= steps; i++)
        {
            // 선형 보간하여 점 위치 계산
            Vector2 interpolatedPoint = Vector2.Lerp(startPoint, endPoint, i / (float)steps);
            ScratchAt(interpolatedPoint);
        }
    }

    /// <summary>
    /// 특정 구역 안에 있는지 확인
    /// </summary>
    private bool IsWithinScratchableArea(Vector2 localPoint)
    {
        // localPoint를 텍스처 좌표로 변환
        float x = (localPoint.x + scratchImage.rectTransform.rect.width / 2) * scratchTexture.width / scratchImage.rectTransform.rect.width;
        float y = (localPoint.y + scratchImage.rectTransform.rect.height / 2) * scratchTexture.height / scratchImage.rectTransform.rect.height;

        // 긁을 수 있는 영역 내인지 확인
        return scratchableArea.Contains(new Vector2(x, y));
    }

    /// <summary>
    /// 특정 위치 긁기
    /// </summary>
    private void ScratchAt(Vector2 localPoint)
    {
        // 스크래치 텍스처의 픽셀 좌표 계산
        int x = (int)((localPoint.x + scratchImage.rectTransform.rect.width / 2) * scratchTexture.width / scratchImage.rectTransform.rect.width);
        int y = (int)((localPoint.y + scratchImage.rectTransform.rect.height / 2) * scratchTexture.height / scratchImage.rectTransform.rect.height);

        // 원형 반지름 설정 (20px)
        int radius = 28;
        int width = scratchTexture.width;
        int height = scratchTexture.height;


        // 해당 위치 주변 픽셀을 원 모양으로 투명하게 변경
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
        needTextureUpdate = true; // Apply는 한 프레임에 한 번만
    }


    /// <summary>
    /// 긁힌 영역의 비율 계산
    /// </summary>
    private float CalculateScratchedPercentage()
    {
        int sampled = 0;
        int scratched = 0;

        int step = 4; // 4픽셀 간격으로 샘플링

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
    /// 복권 UI 초기화
    /// </summary>
    private void SetupLotteryUI(LotteryCard card)
    {
        money = card.winningPrize;

        // 복권의 이미지 및 금액을 UI에 설정
        for (int i = 0; i < lotteryImages.Length; i++)
        {
            lotteryImages[i].sprite = card.images[i]; // 복권 이미지를 설정        
        }
        for(int i = 0; i < 6; i++)
        {
            prizeTexts[i].text = "\u20A9" + card.prizeValues[i].ToString("N0"); // 복권의 금액을 텍스트로 설정

            prizeImage[i].sprite = card.priceImage[i];
        }
    }

    /// <summary>
    /// 결과 표시
    /// </summary>
    private void ShowResult()
    {

        result.gameObject.SetActive(true);
        result.Render(money);
    }
}