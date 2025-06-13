using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("FOV Kick")]
    public float FOVKick = 5;
    public float overlayFOVKick = 5;
    public float FOVKickSmoothness = 10;
    public Camera mainCamera;
    //public Camera overlayCamera;


    [Header("Head Bob")]
    public float headbobAmount = 20;
    public float headbobRotationAmount = 30;

    private float headbobTimer;

    [SerializeField]private CharacterController characterController;
    private float movemnetPercentage;
    [HideInInspector] public float fieldOfView;
    [HideInInspector] public float overlayFieldOfView;


    #region a
    private float defaultFieldOfView;
    private float defaultOverlayFieldOfView;


    public bool Use_FOVKick = true;
    public bool Use_Headbob = true;
    #endregion

    public Vector3 ResultPosition
    {
        get
        {
            Vector3 result = Vector3.zero;
            return result += HeadbobPosition;
        }
    }

    public Vector3 ResultRotation
    {
        get
        {
            Vector3 result = Vector3.zero;
            return result +=  HeadbobRotation;
        }
    }

    public Vector3 HeadbobPosition { get; set; }
    public Vector3 HeadbobRotation { get; set; }

    #region Logic
    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;


        if (mainCamera)
        {
            fieldOfView = mainCamera.fieldOfView;
            defaultFieldOfView = mainCamera.fieldOfView;
        }

        /*if (overlayCamera)
        {
            overlayFieldOfView = overlayCamera.fieldOfView;
            defaultOverlayFieldOfView = overlayCamera.fieldOfView;
        }*/
    }


    private void Update()
    {

        if (characterController != null)
        {
            movemnetPercentage = characterController.velocity.magnitude / 10f;
            movemnetPercentage = Mathf.Clamp(movemnetPercentage, 0, 1.3f);
        }

        if (mainCamera)
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fieldOfView + FOVKickResult(), Time.deltaTime * FOVKickSmoothness);

        /*if (overlayCamera)
            overlayCamera.fieldOfView = Mathf.Lerp(overlayCamera.fieldOfView, overlayFieldOfView + OverlayFOVKickResult(), Time.deltaTime * FOVKickSmoothness);
        */
        if (Use_Headbob) UpdateHeadbob();

        transform.localPosition = ResultPosition;
        transform.localRotation = Quaternion.Euler(ResultRotation);

        if (StatusManager.instance != null)
        {
            if (StatusManager.instance.GetMoodle()[10].isActive)
            {
                int drunkenAmount = Mathf.Min(StatusManager.instance.GetMoodle()[10].timeLeft, 24 * 60); //최대 흔들림 24시간으로
                headbobRotationAmount = drunkenAmount * 4 + 30;
            }
            else
            {
                headbobRotationAmount = 30;
            }
        }
    }


    public void SetFieldOfView(float main, float overlay)
    {
        fieldOfView = main;
        overlayFieldOfView = overlay;
    }

    public void SetFieldOfView(float main, float overlay, float time)
    {
        fieldOfView = Mathf.Lerp(fieldOfView, main, time);
        overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, overlay, time);
    }

    public void SetFieldOfView(float main, float overlay, float timeMain, float timeOverlay)
    {
        fieldOfView = Mathf.Lerp(fieldOfView, main, timeMain);
        overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, overlay, timeOverlay);
    }

    public void ResetFieldOfView(float time)
    {
        fieldOfView = Mathf.Lerp(fieldOfView, defaultFieldOfView, time);
        overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, defaultOverlayFieldOfView, time);
    }

    private float FOVKickResult()
    {
        if (!Use_FOVKick) return 0;

        float value;

        if (movemnetPercentage > 0.8f)
        {
            value = FOVKick * movemnetPercentage * movemnetPercentage;
        }
        else
        {
            value = 0;
        }

        return value;
    }

    private float OverlayFOVKickResult()
    {
        if (!Use_FOVKick) return 0;

        float value;

        if (movemnetPercentage > 0.8f)
        {
            value = overlayFOVKick * movemnetPercentage;
        }
        else
        {
            value = 0;
        }

        return value;
    }


    private void UpdateHeadbob()
    {
        headbobTimer += Time.deltaTime * characterController.velocity.magnitude;

        float posX = 0f;
        float posY = 0f;
        float rotZ = 0;
        float multipler = characterController.velocity.magnitude / 10f;
        posX += ((headbobAmount / 100) / 2f * Mathf.Sin(headbobTimer) * multipler);
        posY += ((headbobAmount / 100) / 2f * Mathf.Sin(headbobTimer * 2f) * multipler);
        rotZ += ((headbobRotationAmount / 100) / 2 * Mathf.Sin(headbobTimer) * multipler);

        Vector3 posResult = new Vector3(posX, posY);
        Vector3 rotResult = new Vector3(0, 0, rotZ);

        //Debug.Log(characterController.velocity.magnitude + " adsfdfa" + characterController.isGrounded);
        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            HeadbobPosition = Vector3.Lerp(HeadbobPosition, posResult, Time.deltaTime * 5);
            HeadbobRotation = Vector3.Lerp(HeadbobRotation, rotResult, Time.deltaTime * 20);
        }
        else
        {
            HeadbobPosition = Vector3.Lerp(HeadbobPosition, Vector3.zero, Time.deltaTime * 5);
            HeadbobRotation = Vector3.Lerp(HeadbobRotation, Vector3.zero, Time.deltaTime * 5);
        }
    }

    #endregion
}
