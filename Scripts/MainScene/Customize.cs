using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedPeopleSystem;

public class Customize : MonoBehaviour
{
    private CharacterCustomization CharacterCustomization;

    // HSV 값을 저장할 변수
    private float hue = 0f;
    private float saturation = 0f;
    private float value = 0f;
    private float eyeHue = 0f;
    private float eyeSaturation = 0f;
    private float eyeValue = 0f;
    private float hairHue = 0f;
    private float hairSaturation = 0f;
    private float hairValue = 0f;

    public Color skinClolr; // 기준 컬러

    void Start()
    {
        CharacterCustomization = GetComponent<CharacterCustomization>();
    }

    public void StartGame() //게임 시작할때 호출
    {
        CharacterCustomization.SaveCharacterToFileReal(CharacterCustomizationSetup.CharacterFileSaveFormat.Json,"",0);
    }

    public void SetSwitchSex(int i)
    {
        CharacterCustomization.SwitchCharacterSettings(i); //0이면 남자 1이면 여자
        //나중에 옷입히기 추가하기
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shirt, 0);
        CharacterCustomization.SetElementByIndex(CharacterElementType.Pants, 0);
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shoes, 0);
    }
    public void SetHeight(float value)
    {
        CharacterCustomization.SetHeight(value);
    }
    public void SetHeadSize(float value)
    {
        CharacterCustomization.SetHeadSize(value);
    }

    #region SkinColor
    // Slider 또는 값 변경을 통해 HSV 값이 변경될 때 호출하는 함수
    /*public void SetHue(float newHue)
    {
        hue = newHue;
        UpdateSkinColor();
    }

    public void SetSaturation(float newSaturation)
    {
        saturation = newSaturation;
        UpdateSkinColor();
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateSkinColor();
    }
    
    // HSV 값을 RGB로 변환하여 SetNewSkinColor 함수 호출
    private void UpdateSkinColor()
    {
        // HSV 값을 RGB로 변환
        Color newColor = Color.HSVToRGB(hue, saturation, value);

        // SetNewSkinColor 함수 호출 (피부 색상 변경)
        SetNewSkinColor(newColor);
    }*/
    public void SkinChange_Event(int index)
    {
        Color resultColor;

        if (index < 4)
        {
            float t = index / 4f; // 0~1
            resultColor = Color.Lerp(Color.white, skinClolr, t);
        }
        else if (index > 4)
        {
            float t = (index - 4) / 4f; // 0~1
            resultColor = Color.Lerp(skinClolr, Color.black, t);
        }
        else // index == 4
        {
            resultColor = skinClolr;
        }
        SetNewSkinColor(resultColor);
    }
    // 피부 색상 변경 함수
    public void SetNewSkinColor(Color color)
    {
        // 캐릭터의 피부 색상 변경
        CharacterCustomization.SetBodyColor(BodyColorPart.Skin, color);
    }
    #endregion

    #region HairColor
    // Slider 또는 값 변경을 통해 HSV 값이 변경될 때 호출하는 함수
    public void SetHairHue(float newHue)
    {
        hairHue = newHue;
        UpdateHairColor();
    }

    public void SetHairSaturation(float newSaturation)
    {
        hairSaturation = newSaturation;
        UpdateHairColor();
    }

    public void SetHairValue(float newValue)
    {
        hairValue = newValue;
        UpdateHairColor();
    }

    // HSV 값을 RGB로 변환하여 SetNewSkinColor 함수 호출
    private void UpdateHairColor()
    {
        // HSV 값을 RGB로 변환
        Color newColor = Color.HSVToRGB(hairHue, hairSaturation, hairValue);

        // SetNewSkinColor 함수 호출 (피부 색상 변경)
        SetNewHairColor(newColor);
    }

    // 피부 색상 변경 함수
    public void SetNewHairColor(Color color)
    {
        // 캐릭터의 피부 색상 변경
        CharacterCustomization.SetBodyColor(BodyColorPart.Hair, color);
    }
    #endregion

    //몸 체형 변경 함수
    public void BodyType(float value)
    {
        if (value >= 0) //만약 양수면
        {
            CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Fat, value);
            CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Thin, 0);
        }
        if (value <= 0)
        {
            CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Fat, 0);
            CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Thin, value * -1);
        }

    }

    #region EyeColor
    public void SetEyeHue(float newHue)
    {
        eyeHue = newHue;
        UpdateEyeColor();
    }

    public void SetEyeSaturation(float newSaturation)
    {
        eyeSaturation = newSaturation;
        UpdateEyeColor();
    }

    public void SetEyeValue(float newValue)
    {
        eyeValue = newValue;
        UpdateEyeColor();
    }

    // HSV 값을 RGB로 변환하여 SetNewSkinColor 함수 호출
    private void UpdateEyeColor()
    {
        // HSV 값을 RGB로 변환
        Color newColor = Color.HSVToRGB(eyeHue, eyeSaturation, eyeValue);

        // SetNewSkinColor 함수 호출 (피부 색상 변경)
        SetNewEyeColor(newColor);
    }

    // 피부 색상 변경 함수
    public void SetNewEyeColor(Color color)
    {
        // 캐릭터의 피부 색상 변경
        CharacterCustomization.SetBodyColor(BodyColorPart.Eye, color);
    }
    #endregion

    #region Outfit
    public void HairChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Hair, index -1);
    }
    public void BeardChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Beard, index -1);
    }
    public void ShirtChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shirt, index);
    }
    public void PantsChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Pants, index);
    }
    public void ShoesChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shoes, index);
    }
    #endregion

    #region Others
    public void EyeWidth(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Eye_Width,value);
    }
    public void EyeForm(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Eye_Form, value);
    }
    public void EyeHeight(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Eye_Offset, value);
    }
    public void EyeSize(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Eye_Size, value);
    }
    public void BrowShape(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Brow_Shape, value);
    }
    public void BrowThickness(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Brow_Thickness, value);
    }
    public void BrowLength(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Brow_Length, value);
    }
    public void BrowHeight(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Brow_Height, value);
    }

    public void NoseAngle(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Nose_Angle, value);
    }
    public void NoseHeight(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Nose_Offset, value);
    }
    public void NoseSize(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Nose_Size, value);
    }

    public void MouthLength(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Mouth_Width, value);
    }
    public void MouthHeight(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Mouth_Offset, value);
    }
    public void MouthSize(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Mouth_Size, value);
    }

    public void NeckThickness(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Neck_Width, value);
    }
    public void EarSize(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Ear_Size, value);
    }
    public void EarAngle(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Ear_Angle, value);
    }

    public void JawWidth(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Jaw_Width, value);
    }
    public void JawLength(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Jaw_Offset, value);
    }

    public void CheekSize(float value)
    {
        CharacterCustomization.SetBlendshapeValue(CharacterBlendShapeType.Cheek_Size, value);
    }
    #endregion
}
