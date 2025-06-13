using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LotteryManager : UiPrompt
{
    public Sprite[] imageSprites; // 총 10개의 이미지 (복권 이미지로 사용)
    private int[] prizeAmounts =  {2000, 5000, 10000, 20000, 50000, 100000, 500000, 1000000, 10000000 }; // 복권에 들어갈 당첨 금액 표시들

    public Sprite[] prizeEx; // 복권 금액 한글 리스트, 2000부터 일천만까지

    // 당첨 금액 배열
    private int[] prizes = { 0, 2000, 5000, 10000, 20000, 50000, 100000, 500000, 1000000 }; //실제 당첨 금액
    // 기본 확률
    private float[] baseProbabilities = { 0.5f, 0.25f, 0.12f, 0.06f, 0.03f, 0.02f, 0.01f, 0.005f, 0.0025f };

    // 운 수치 (-10 ~ 10)
    private int luck;

    public int CalLottery() //복권 확률 계산
    {
        //luck 수치 가져오는것 추가할것
        if(StatusManager.instance != null)
        {
            luck = StatusManager.instance.GetLuckLevel();
        }

        // 운 수치를 기반으로 확률 조정
        float[] adjustedProbabilities = AdjustProbabilities(baseProbabilities, luck);

        // 랜덤 값으로 당첨 결정
        float randomValue = UnityEngine.Random.value; // 0.0 ~ 1.0 사이 랜덤 값
        float cumulative = 0f;

        for (int i = 0; i < prizes.Length; i++)
        {
            cumulative += adjustedProbabilities[i];
            if (randomValue < cumulative)
            {
                return prizes[i]; // 당첨 금액 반환
            }
        }

        return 0; // 기본적으로 꽝 반환
    }

    private float[] AdjustProbabilities(float[] baseProbabilities, int luck)
    {
        float[] adjustedProbabilities = new float[baseProbabilities.Length];

        // 꽝의 기본 확률과 운 수치에 따른 조정
        float missProbability = baseProbabilities[0];
        float luckFactor = luck / 100f;
        float adjustedMissProbability = Mathf.Clamp(missProbability - luckFactor, 0f, 1f);

        // 남은 확률
        float remainingProbability = 1f - adjustedMissProbability;

        // 꽝을 제외한 총 확률
        float nonMissTotalProbability = 1f - missProbability;

        // 확률 재분배 (기존 비율 유지)
        for (int i = 0; i < baseProbabilities.Length; i++)
        {
            if (i == 0)
            {
                // 꽝 확률 업데이트
                adjustedProbabilities[0] = adjustedMissProbability;
            }
            else
            {
                // 나머지 금액 확률 업데이트 (기존 비율 유지)
                float proportion = baseProbabilities[i] / nonMissTotalProbability;
                adjustedProbabilities[i] = proportion * remainingProbability;
            }
        }

        return adjustedProbabilities;
    }

    /// <summary>
    /// 복권을 생성합니다.
    /// </summary>
    /// <param name="winningPrize">당첨 금액 (null이면 꽝)</param>
    /// <returns>생성된 LotteryCard 객체</returns>
    public LotteryCard CreateLottery()
    {
        int winningPrize = CalLottery(); //복권 당첨금액 리턴
        Debug.Log("Winnig Prize is " + winningPrize);


        Sprite[] images = new Sprite[12]; // 복권의 각 칸 이미지 배열
        Sprite[] priceImage = new Sprite[6]; // 복권의 각 칸 금액 한글
        int[] prizeValues = new int[6]; // 복권의 각 칸 금액 배열
        int winningIndex = -1; // 당첨된 칸의 인덱스, 기본값은 -1 (꽝)


        // 1. 당첨 복권 생성
        if (winningPrize != 0)
        {
            // 당첨 금액이 prizeAmounts에 포함되지 않으면 에러
            if (!Array.Exists(prizeAmounts, amount => amount == winningPrize))
            {
                Debug.LogError("Invalid winning prize amount!");
                return null;
            }

            // 당첨된 두 칸을 랜덤으로 선택
            winningIndex = UnityEngine.Random.Range(0, 6) * 2; //0,2,4,6,8,10
            Sprite winningSprite = imageSprites[UnityEngine.Random.Range(0, imageSprites.Length)];

            images[winningIndex] = winningSprite; // 첫 번째 당첨 칸
            images[(winningIndex + 1)] = winningSprite; // 두 번째 당첨 칸 (짝)

            // 나머지 칸은 짝이 절대 맞지 않도록 설정
            for (int i = 0; i < 12; i++)
            {
                if (i == winningIndex || i == (winningIndex + 1)) continue;

                if (i % 2 == 0) // 짝수번 인덱스
                {
                    images[i] = GetRandomSprite(); // 랜덤 이미지 설정
                }
                else // 홀수번 인덱스
                {
                    Sprite pairedSprite;
                    do
                    {
                        pairedSprite = GetRandomSprite();
                    } while (pairedSprite == images[i - 1]); // 앞의 짝수번과 다른 이미지 설정

                    images[i] = pairedSprite;
                }
            }

            // 각 칸의 금액 설정
            for (int i = 0; i < 6; i++)
            {
                prizeValues[i] = (i == winningIndex/2) ? winningPrize : GetRandomPrize();
                if(i == winningIndex / 2)
                {
                    prizeValues[i] = winningPrize;
                    priceImage[i] = prizeEx[Array.IndexOf(prizeAmounts, winningPrize)];
                }
                else
                {
                    int temp = GetRandomPrize();
                    prizeValues[i] = prizeAmounts[temp]; // 랜덤 금액 설정
                    priceImage[i] = prizeEx[temp];
                }
            }
        }
        else
        {
            // 2. 꽝 복권 생성 (모든 칸의 짝이 맞지 않음)
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0) // 짝수번 인덱스
                {
                    images[i] = GetRandomSprite(); // 랜덤 이미지 설정
                }
                else // 홀수번 인덱스
                {
                    Sprite pairedSprite;
                    do
                    {
                        pairedSprite = GetRandomSprite();
                    } while (pairedSprite == images[i - 1]); // 앞의 짝수번과 다른 이미지 설정

                    images[i] = pairedSprite;
                }
            }

            // 각 칸의 금액 설정
            for (int i = 0; i < 6; i++)
            {
                int temp = GetRandomPrize();
                prizeValues[i] = prizeAmounts[temp]; // 랜덤 금액 설정
                priceImage[i] = prizeEx[temp];
            }
        }

        return new LotteryCard(images, priceImage , prizeValues, winningIndex, winningPrize);
    }

    /// <summary>
    /// 사용 중인 이미지를 제외한 랜덤 이미지를 반환합니다.
    /// </summary>
    /// <param name="usedSprites">현재 사용 중인 이미지 배열</param>
    /// <returns>랜덤 이미지</returns>
    private Sprite GetRandomSprite()
    {
        return imageSprites[UnityEngine.Random.Range(0, imageSprites.Length)];
    }

    /// <summary>
    /// 랜덤으로 금액을 반환합니다.
    /// </summary>
    /// <returns>랜덤 금액</returns>
    private int GetRandomPrize()
    {
        //return prizeAmounts[UnityEngine.Random.Range(0, prizeAmounts.Length)];
        return UnityEngine.Random.Range(0, prizeAmounts.Length);
    }
}

/// <summary>
/// 복권 데이터를 저장하는 클래스
/// </summary>
public class LotteryCard
{
    public Sprite[] images; // 복권의 각 칸 이미지
    public Sprite[] priceImage; // 복권의 각 칸 금액 한글
    public int[] prizeValues; // 복권의 각 칸 금액
    public int winningIndex; // 당첨된 칸의 인덱스 (-1이면 꽝)
    public int winningPrize;

    public LotteryCard(Sprite[] images, Sprite[] priceImage, int[] prizeValues, int winningIndex, int winningPrize)
    {
        this.images = images;
        this.priceImage = priceImage;
        this.prizeValues = prizeValues;
        this.winningIndex = winningIndex;
        this.winningPrize = winningPrize;
    }
}
