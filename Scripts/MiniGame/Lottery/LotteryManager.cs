using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LotteryManager : UiPrompt
{
    public Sprite[] imageSprites; // �� 10���� �̹��� (���� �̹����� ���)
    private int[] prizeAmounts =  {2000, 5000, 10000, 20000, 50000, 100000, 500000, 1000000, 10000000 }; // ���ǿ� �� ��÷ �ݾ� ǥ�õ�

    public Sprite[] prizeEx; // ���� �ݾ� �ѱ� ����Ʈ, 2000���� ��õ������

    // ��÷ �ݾ� �迭
    private int[] prizes = { 0, 2000, 5000, 10000, 20000, 50000, 100000, 500000, 1000000 }; //���� ��÷ �ݾ�
    // �⺻ Ȯ��
    private float[] baseProbabilities = { 0.5f, 0.25f, 0.12f, 0.06f, 0.03f, 0.02f, 0.01f, 0.005f, 0.0025f };

    // �� ��ġ (-10 ~ 10)
    private int luck;

    public int CalLottery() //���� Ȯ�� ���
    {
        //luck ��ġ �������°� �߰��Ұ�
        if(StatusManager.instance != null)
        {
            luck = StatusManager.instance.GetLuckLevel();
        }

        // �� ��ġ�� ������� Ȯ�� ����
        float[] adjustedProbabilities = AdjustProbabilities(baseProbabilities, luck);

        // ���� ������ ��÷ ����
        float randomValue = UnityEngine.Random.value; // 0.0 ~ 1.0 ���� ���� ��
        float cumulative = 0f;

        for (int i = 0; i < prizes.Length; i++)
        {
            cumulative += adjustedProbabilities[i];
            if (randomValue < cumulative)
            {
                return prizes[i]; // ��÷ �ݾ� ��ȯ
            }
        }

        return 0; // �⺻������ �� ��ȯ
    }

    private float[] AdjustProbabilities(float[] baseProbabilities, int luck)
    {
        float[] adjustedProbabilities = new float[baseProbabilities.Length];

        // ���� �⺻ Ȯ���� �� ��ġ�� ���� ����
        float missProbability = baseProbabilities[0];
        float luckFactor = luck / 100f;
        float adjustedMissProbability = Mathf.Clamp(missProbability - luckFactor, 0f, 1f);

        // ���� Ȯ��
        float remainingProbability = 1f - adjustedMissProbability;

        // ���� ������ �� Ȯ��
        float nonMissTotalProbability = 1f - missProbability;

        // Ȯ�� ��й� (���� ���� ����)
        for (int i = 0; i < baseProbabilities.Length; i++)
        {
            if (i == 0)
            {
                // �� Ȯ�� ������Ʈ
                adjustedProbabilities[0] = adjustedMissProbability;
            }
            else
            {
                // ������ �ݾ� Ȯ�� ������Ʈ (���� ���� ����)
                float proportion = baseProbabilities[i] / nonMissTotalProbability;
                adjustedProbabilities[i] = proportion * remainingProbability;
            }
        }

        return adjustedProbabilities;
    }

    /// <summary>
    /// ������ �����մϴ�.
    /// </summary>
    /// <param name="winningPrize">��÷ �ݾ� (null�̸� ��)</param>
    /// <returns>������ LotteryCard ��ü</returns>
    public LotteryCard CreateLottery()
    {
        int winningPrize = CalLottery(); //���� ��÷�ݾ� ����
        Debug.Log("Winnig Prize is " + winningPrize);


        Sprite[] images = new Sprite[12]; // ������ �� ĭ �̹��� �迭
        Sprite[] priceImage = new Sprite[6]; // ������ �� ĭ �ݾ� �ѱ�
        int[] prizeValues = new int[6]; // ������ �� ĭ �ݾ� �迭
        int winningIndex = -1; // ��÷�� ĭ�� �ε���, �⺻���� -1 (��)


        // 1. ��÷ ���� ����
        if (winningPrize != 0)
        {
            // ��÷ �ݾ��� prizeAmounts�� ���Ե��� ������ ����
            if (!Array.Exists(prizeAmounts, amount => amount == winningPrize))
            {
                Debug.LogError("Invalid winning prize amount!");
                return null;
            }

            // ��÷�� �� ĭ�� �������� ����
            winningIndex = UnityEngine.Random.Range(0, 6) * 2; //0,2,4,6,8,10
            Sprite winningSprite = imageSprites[UnityEngine.Random.Range(0, imageSprites.Length)];

            images[winningIndex] = winningSprite; // ù ��° ��÷ ĭ
            images[(winningIndex + 1)] = winningSprite; // �� ��° ��÷ ĭ (¦)

            // ������ ĭ�� ¦�� ���� ���� �ʵ��� ����
            for (int i = 0; i < 12; i++)
            {
                if (i == winningIndex || i == (winningIndex + 1)) continue;

                if (i % 2 == 0) // ¦���� �ε���
                {
                    images[i] = GetRandomSprite(); // ���� �̹��� ����
                }
                else // Ȧ���� �ε���
                {
                    Sprite pairedSprite;
                    do
                    {
                        pairedSprite = GetRandomSprite();
                    } while (pairedSprite == images[i - 1]); // ���� ¦������ �ٸ� �̹��� ����

                    images[i] = pairedSprite;
                }
            }

            // �� ĭ�� �ݾ� ����
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
                    prizeValues[i] = prizeAmounts[temp]; // ���� �ݾ� ����
                    priceImage[i] = prizeEx[temp];
                }
            }
        }
        else
        {
            // 2. �� ���� ���� (��� ĭ�� ¦�� ���� ����)
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0) // ¦���� �ε���
                {
                    images[i] = GetRandomSprite(); // ���� �̹��� ����
                }
                else // Ȧ���� �ε���
                {
                    Sprite pairedSprite;
                    do
                    {
                        pairedSprite = GetRandomSprite();
                    } while (pairedSprite == images[i - 1]); // ���� ¦������ �ٸ� �̹��� ����

                    images[i] = pairedSprite;
                }
            }

            // �� ĭ�� �ݾ� ����
            for (int i = 0; i < 6; i++)
            {
                int temp = GetRandomPrize();
                prizeValues[i] = prizeAmounts[temp]; // ���� �ݾ� ����
                priceImage[i] = prizeEx[temp];
            }
        }

        return new LotteryCard(images, priceImage , prizeValues, winningIndex, winningPrize);
    }

    /// <summary>
    /// ��� ���� �̹����� ������ ���� �̹����� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="usedSprites">���� ��� ���� �̹��� �迭</param>
    /// <returns>���� �̹���</returns>
    private Sprite GetRandomSprite()
    {
        return imageSprites[UnityEngine.Random.Range(0, imageSprites.Length)];
    }

    /// <summary>
    /// �������� �ݾ��� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>���� �ݾ�</returns>
    private int GetRandomPrize()
    {
        //return prizeAmounts[UnityEngine.Random.Range(0, prizeAmounts.Length)];
        return UnityEngine.Random.Range(0, prizeAmounts.Length);
    }
}

/// <summary>
/// ���� �����͸� �����ϴ� Ŭ����
/// </summary>
public class LotteryCard
{
    public Sprite[] images; // ������ �� ĭ �̹���
    public Sprite[] priceImage; // ������ �� ĭ �ݾ� �ѱ�
    public int[] prizeValues; // ������ �� ĭ �ݾ�
    public int winningIndex; // ��÷�� ĭ�� �ε��� (-1�̸� ��)
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
